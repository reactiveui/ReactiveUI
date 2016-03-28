// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

using System;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;

namespace Microsoft.Reactive.Testing
{
    /// <summary>
    /// Virtual time scheduler used for testing applications and libraries built using Reactive Extensions.
    /// </summary>
    public class TestScheduler : VirtualTimeScheduler<long, long>
    {
        /// <summary>
        /// Schedules an action to be executed at the specified virtual time.
        /// </summary>
        /// <typeparam name="TState">The type of the state passed to the scheduled action.</typeparam>
        /// <param name="state">State passed to the action to be executed.</param>
        /// <param name="action">Action to be executed.</param>
        /// <param name="dueTime">Absolute virtual time at which to execute the action.</param>
        /// <returns>Disposable object used to cancel the scheduled action (best effort).</returns>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is null.</exception>
        public override IDisposable ScheduleAbsolute<TState>(TState state, long dueTime, Func<IScheduler, TState, IDisposable> action)
        {
            if (dueTime <= Clock)
                dueTime = Clock + 1;

            return base.ScheduleAbsolute<TState>(state, dueTime, action);
        }

        /// <summary>
        /// Adds a relative virtual time to an absolute virtual time value.
        /// </summary>
        /// <param name="absolute">Absolute virtual time value.</param>
        /// <param name="relative">Relative virtual time value to add.</param>
        /// <returns>Resulting absolute virtual time sum value.</returns>
        protected override long Add(long absolute, long relative)
        {
            return absolute + relative;
        }

        /// <summary>
        /// Converts the absolute virtual time value to a DateTimeOffset value.
        /// </summary>
        /// <param name="absolute">Absolute virtual time value to convert.</param>
        /// <returns>Corresponding DateTimeOffset value.</returns>
        protected override DateTimeOffset ToDateTimeOffset(long absolute)
        {
            return new DateTimeOffset(absolute, TimeSpan.Zero);
        }

        /// <summary>
        /// Converts the TimeSpan value to a relative virtual time value.
        /// </summary>
        /// <param name="timeSpan">TimeSpan value to convert.</param>
        /// <returns>Corresponding relative virtual time value.</returns>
        protected override long ToRelative(TimeSpan timeSpan)
        {
            return timeSpan.Ticks;
        }

        /// <summary>
        /// Starts the test scheduler and uses the specified virtual times to invoke the factory function, subscribe to the resulting sequence, and dispose the subscription.
        /// </summary>
        /// <typeparam name="T">The element type of the observable sequence being tested.</typeparam>
        /// <param name="create">Factory method to create an observable sequence.</param>
        /// <param name="created">Virtual time at which to invoke the factory to create an observable sequence.</param>
        /// <param name="subscribed">Virtual time at which to subscribe to the created observable sequence.</param>
        /// <param name="disposed">Virtual time at which to dispose the subscription.</param>
        /// <returns>Observer with timestamped recordings of notification messages that were received during the virtual time window when the subscription to the source sequence was active.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="create"/> is null.</exception>
        public ITestableObserver<T> Start<T>(Func<IObservable<T>> create, long created, long subscribed, long disposed)
        {
            if (create == null)
                throw new ArgumentNullException("create");

            var source = default(IObservable<T>);
            var subscription = default(IDisposable);
            var observer = CreateObserver<T>();

            ScheduleAbsolute(default(object), created, (scheduler, state) => { source = create(); return Disposable.Empty; });
            ScheduleAbsolute(default(object), subscribed, (scheduler, state) => { subscription = source.Subscribe(observer); return Disposable.Empty; });
            ScheduleAbsolute(default(object), disposed, (scheduler, state) => { subscription.Dispose(); return Disposable.Empty; });

            Start();

            return observer;
        }

        /// <summary>
        /// Starts the test scheduler and uses the specified virtual time to dispose the subscription to the sequence obtained through the factory function.
        /// Default virtual times are used for <see cref="ReactiveTest.Created">factory invocation</see> and <see cref="ReactiveTest.Subscribed">sequence subscription</see>.
        /// </summary>
        /// <typeparam name="T">The element type of the observable sequence being tested.</typeparam>
        /// <param name="create">Factory method to create an observable sequence.</param>
        /// <param name="disposed">Virtual time at which to dispose the subscription.</param>
        /// <returns>Observer with timestamped recordings of notification messages that were received during the virtual time window when the subscription to the source sequence was active.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="create"/> is null.</exception>
        public ITestableObserver<T> Start<T>(Func<IObservable<T>> create, long disposed)
        {
            if (create == null)
                throw new ArgumentNullException("create");

            return Start(create, ReactiveTest.Created, ReactiveTest.Subscribed, disposed);
        }

        /// <summary>
        /// Starts the test scheduler and uses default virtual times to <see cref="ReactiveTest.Created">invoke the factory function</see>, to <see cref="ReactiveTest.Subscribed">subscribe to the resulting sequence</see>, and to <see cref="ReactiveTest.Disposed">dispose the subscription</see>.
        /// </summary>
        /// <typeparam name="T">The element type of the observable sequence being tested.</typeparam>
        /// <param name="create">Factory method to create an observable sequence.</param>
        /// <returns>Observer with timestamped recordings of notification messages that were received during the virtual time window when the subscription to the source sequence was active.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="create"/> is null.</exception>
        public ITestableObserver<T> Start<T>(Func<IObservable<T>> create)
        {
            if (create == null)
                throw new ArgumentNullException("create");

            return Start(create, ReactiveTest.Created, ReactiveTest.Subscribed, ReactiveTest.Disposed);
        }

        /// <summary>
        /// Creates a hot observable using the specified timestamped notification messages.
        /// </summary>
        /// <typeparam name="T">The element type of the observable sequence being created.</typeparam>
        /// <param name="messages">Notifications to surface through the created sequence at their specified absolute virtual times.</param>
        /// <returns>Hot observable sequence that can be used to assert the timing of subscriptions and notifications.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="messages"/> is null.</exception>
        public ITestableObservable<T> CreateHotObservable<T>(params Recorded<Notification<T>>[] messages)
        {
            if (messages == null)
                throw new ArgumentNullException("messages");

            return new HotObservable<T>(this, messages);
        }

        /// <summary>
        /// Creates a cold observable using the specified timestamped notification messages.
        /// </summary>
        /// <typeparam name="T">The element type of the observable sequence being created.</typeparam>
        /// <param name="messages">Notifications to surface through the created sequence at their specified virtual time offsets from the sequence subscription time.</param>
        /// <returns>Cold observable sequence that can be used to assert the timing of subscriptions and notifications.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="messages"/> is null.</exception>
        public ITestableObservable<T> CreateColdObservable<T>(params Recorded<Notification<T>>[] messages)
        {
            if (messages == null)
                throw new ArgumentNullException("messages");

            return new ColdObservable<T>(this, messages);
        }

        /// <summary>
        /// Creates an observer that records received notification messages and timestamps those.
        /// </summary>
        /// <typeparam name="T">The element type of the observer being created.</typeparam>
        /// <returns>Observer that can be used to assert the timing of received notifications.</returns>
        public ITestableObserver<T> CreateObserver<T>()
        {
            return new MockObserver<T>(this);
        }
    }
}
