// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive;
using System.Reactive.Concurrency;
using ReactiveUI.Helpers;

namespace ReactiveUI.Internal;

/// <summary>
/// A reset-on-reactivate idle timer: emits once each time the application stays deactivated for the idle timeout
/// without being reactivated. A fused replacement for
/// <c>deactivated.SelectMany(_ =&gt; Timer(idle)).TakeUntil(reactivated).Repeat()</c> — each deactivation
/// (re)starts the timer, a reactivation cancels the pending one, and on elapse it forwards an empty disposable.
/// </summary>
/// <param name="deactivated">Signals the application has been deactivated.</param>
/// <param name="reactivated">Signals the application has been reactivated (cancels a pending timer).</param>
/// <param name="idleTimeout">Supplies the idle timeout duration (evaluated when a timer starts).</param>
/// <param name="scheduler">The scheduler the timer runs on.</param>
internal sealed class IdleTimeoutObservable(
    IObservable<Unit> deactivated,
    IObservable<Unit> reactivated,
    Func<TimeSpan> idleTimeout,
    IScheduler scheduler) : IObservable<IDisposable>
{
    /// <inheritdoc/>
    public IDisposable Subscribe(IObserver<IDisposable> observer)
    {
        ArgumentExceptionHelper.ThrowIfNull(observer);

        var sink = new Sink(observer, idleTimeout, scheduler);
        sink.Run(deactivated, reactivated);
        return sink;
    }

    /// <summary>Manages the pending timer and the deactivate/reactivate subscriptions.</summary>
    /// <param name="downstream">The downstream observer.</param>
    /// <param name="idleTimeout">Supplies the idle timeout duration.</param>
    /// <param name="scheduler">The scheduler the timer runs on.</param>
    private sealed class Sink(IObserver<IDisposable> downstream, Func<TimeSpan> idleTimeout, IScheduler scheduler) : IDisposable
    {
/// <summary>Serializes notification forwarding across the sources.</summary>
        #if NET9_0_OR_GREATER
        private readonly Lock _gate = new();
#else
        private readonly object _gate = new();
#endif

        /// <summary>The pending idle timer, or <see langword="null"/> when none is running.</summary>
        private IDisposable? _timer;

        /// <summary>Subscription to the deactivation source.</summary>
        private IDisposable? _deactivatedSubscription;

        /// <summary>Subscription to the reactivation source.</summary>
        private IDisposable? _reactivatedSubscription;

        /// <summary>Whether the sink has been disposed.</summary>
        private bool _disposed;

        /// <summary>Subscribes to the deactivation and reactivation sources.</summary>
        /// <param name="deactivated">The deactivation source.</param>
        /// <param name="reactivated">The reactivation source.</param>
        public void Run(IObservable<Unit> deactivated, IObservable<Unit> reactivated)
        {
            _deactivatedSubscription = deactivated.Subscribe(new DelegateObserver<Unit>(_ => OnDeactivated()));
            _reactivatedSubscription = reactivated.Subscribe(new DelegateObserver<Unit>(_ => CancelTimer()));
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            lock (_gate)
            {
                _disposed = true;
                _timer?.Dispose();
                _timer = null;
            }

            _deactivatedSubscription?.Dispose();
            _reactivatedSubscription?.Dispose();
        }

        /// <summary>(Re)starts the idle timer on deactivation.</summary>
        private void OnDeactivated()
        {
            lock (_gate)
            {
                if (_disposed)
                {
                    return;
                }

                _timer?.Dispose();
                _timer = scheduler.Schedule(this, idleTimeout(), static (_, self) =>
                {
                    self.OnElapsed();
                    return EmptyDisposable.Instance;
                });
            }
        }

        /// <summary>Cancels the pending timer on reactivation.</summary>
        private void CancelTimer()
        {
            lock (_gate)
            {
                _timer?.Dispose();
                _timer = null;
            }
        }

        /// <summary>Forwards the idle signal when the timer elapses without a reactivation.</summary>
        private void OnElapsed()
        {
            lock (_gate)
            {
                if (_disposed)
                {
                    return;
                }

                _timer = null;
            }

            downstream.OnNext(EmptyDisposable.Instance);
        }
    }
}
