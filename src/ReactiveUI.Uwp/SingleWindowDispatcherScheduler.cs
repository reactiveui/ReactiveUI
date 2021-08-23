// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.PlatformServices;
using System.Runtime.ExceptionServices;
using System.Threading;
using Windows.ApplicationModel.Core;
using Windows.System.Threading;
using Windows.UI.Core;
using Windows.UI.Xaml;

namespace ReactiveUI
{
    /// <summary>
    /// This scheduler forces all dispatching to go to the first window of the <see cref="CoreApplication.Views"/> enumeration.
    /// This makes the intended behavior of only supporting single window apps on UWP explicit.
    /// If your app creates multiple windows, you should explicitly supply a scheduler which marshals
    /// back to that window's <see cref="CoreDispatcher"/>.
    /// </summary>
    /// <remarks>
    /// This follows patterns set out in <see cref="CoreDispatcherScheduler"/> with some minor tweaks
    /// for thread-safety and performance.
    /// </remarks>
    /// <seealso cref="System.Reactive.Concurrency.IScheduler" />
    public class SingleWindowDispatcherScheduler : IScheduler
    {
        private const CoreDispatcherPriority Priority = default;
        private static CoreDispatcher? _dispatcher;

        /// <summary>
        /// Initializes a new instance of the <see cref="SingleWindowDispatcherScheduler"/> class.
        /// </summary>
        public SingleWindowDispatcherScheduler()
        {
            if (CoreApplication.Views.Count == 0)
            {
                return;
            }

            var coreDispatcher = TryGetDispatcher();

            Interlocked.CompareExchange(ref _dispatcher, coreDispatcher, null);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SingleWindowDispatcherScheduler"/> class with an explicit dispatcher.
        /// </summary>
        /// <param name="dispatcher">
        /// The explicit <see cref="CoreDispatcher"/> to use. If you supply a dispatcher here then all instances of
        /// <see cref="SingleWindowDispatcherScheduler"/> will dispatch to that dispatcher from instantiation on.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// dispatcher - To override the scheduler you must supply a non-null instance of CoreDispatcher.
        /// </exception>
        public SingleWindowDispatcherScheduler(CoreDispatcher dispatcher) => _dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher), "To override the scheduler you must supply a non-null instance of CoreDispatcher.");

        /// <inheritdoc/>
        public DateTimeOffset Now => SystemClock.UtcNow;

        /// <inheritdoc/>
        public IDisposable Schedule<TState>(TState state, Func<IScheduler, TState, IDisposable> action)
        {
            if (action is null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            if (CoreApplication.Views.Count == 0)
            {
                return CurrentThreadScheduler.Instance.Schedule(state, action);
            }

            return ScheduleOnDispatcherNow(state, action);
        }

        /// <inheritdoc/>
        public IDisposable Schedule<TState>(TState state, TimeSpan dueTime, Func<IScheduler, TState, IDisposable> action)
        {
            if (action is null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            if (CoreApplication.Views.Count == 0)
            {
                return CurrentThreadScheduler.Instance.Schedule(state, dueTime, action);
            }

            var dt = Scheduler.Normalize(dueTime);
            if (dt.Ticks == 0)
            {
                return ScheduleOnDispatcherNow(state, action);
            }

            return ScheduleSlow(state, dt, action);
        }

        /// <inheritdoc/>
        public IDisposable Schedule<TState>(TState state, DateTimeOffset dueTime, Func<IScheduler, TState, IDisposable> action)
        {
            if (action is null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            if (CoreApplication.Views.Count == 0)
            {
                return CurrentThreadScheduler.Instance.Schedule(state, dueTime, action);
            }

            var dt = Scheduler.Normalize(dueTime - DateTimeOffset.Now);
            if (dt.Ticks == 0)
            {
                return ScheduleOnDispatcherNow(state, action);
            }

            return ScheduleSlow(state, dt, action);
        }

        /// <summary>
        /// Work-around for the behavior of throwing from "async void" or an <see cref="IAsyncResult"/> not propagating
        /// the exception to the <see cref="Application.UnhandledException" /> event as users have come to expect from
        /// previous XAML stacks using Rx.
        /// </summary>
        /// <param name="ex">The exception.</param>
        private static void RaiseUnhandledException(Exception ex)
        {
            var timer = new DispatcherTimer
            {
                Interval = TimeSpan.Zero
            };

            timer.Tick += RaiseToDispatcher;

            timer.Start();
#pragma warning disable RCS1163 // Unused parameter.
            void RaiseToDispatcher(object sender, object e)
#pragma warning restore RCS1163 // Unused parameter.
            {
                timer.Stop();
                timer.Tick -= RaiseToDispatcher;
                timer = null;

                ExceptionDispatchInfo.Capture(ex).Throw();
            }
        }

        private static CoreDispatcher? TryGetDispatcher()
        {
            CoreDispatcher? coreDispatcher;

            try
            {
                coreDispatcher = CoreApplication.Views.Count > 0 ? CoreApplication.Views[0].Dispatcher : null;
            }
            catch
            {
                // in the XamlIsland case, accessing Views throws. Thus, falling back to the old way. This HAS to be initialized on the MainThread.
                coreDispatcher = Window.Current?.Dispatcher;
            }

            return coreDispatcher;
        }

        private IDisposable ScheduleOnDispatcherNow<TState>(TState state, Func<IScheduler, TState, IDisposable> action)
        {
            try
            {
                // if _dispatcher is still null (and only then) CompareExchange it with the dispatcher from the first view found
                if (_dispatcher is null)
                {
                    var dispatcher = TryGetDispatcher();
                    Interlocked.CompareExchange(ref _dispatcher, dispatcher, null);
                }
            }
            catch
            {
                // Ignore
            }

            if (_dispatcher is null || _dispatcher.HasThreadAccess)
            {
                return action(this, state);
            }

            var d = new SingleAssignmentDisposable();

            var dispatchResult = _dispatcher.RunAsync(
                Priority,
                () =>
                {
                    if (!d.IsDisposed)
                    {
                        try
                        {
                            d.Disposable = action(this, state);
                        }
                        catch (Exception ex)
                        {
                            RaiseUnhandledException(ex);
                        }
                    }
                });

            return StableCompositeDisposable.Create(
                d,
                Disposable.Create(() => dispatchResult.Cancel()));
        }

        private IDisposable ScheduleSlow<TState>(TState state, TimeSpan dueTime, Func<IScheduler, TState, IDisposable> action)
        {
            var d = new MultipleAssignmentDisposable();

            // Why ThreadPoolTimer?
            // --
            // Because, we can't guarantee that DispatcherTimer will dispatch to the correct CoreDispatcher if there are multiple
            // so we dispatch explicitly from our own method.
            var timer = ThreadPoolTimer.CreateTimer(_ => d.Disposable = ScheduleOnDispatcherNow(state, action), dueTime);

            d.Disposable = Disposable.Create(() =>
            {
                var t = Interlocked.Exchange(ref timer, null!);
                if (t is not null)
                {
                    t.Cancel();
                    action = (_, __) => Disposable.Empty;
                }
            });

            return d;
        }
    }
}
