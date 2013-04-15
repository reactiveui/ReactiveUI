using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Threading;
using ReactiveUI;
using System.Reactive.Concurrency;
using Microsoft.Reactive.Testing;


namespace ReactiveUI.Testing
{
    public static class TestUtils
    {
        static readonly object schedGate = 42;
        static readonly object mbGate = 42;

        /// <summary>
        /// WithScheduler overrides the default Deferred and Taskpool schedulers
        /// with the given scheduler until the return value is disposed. This
        /// is useful in a unit test runner to force RxXaml objects to schedule
        /// via a TestScheduler object.
        /// </summary>
        /// <param name="sched">The scheduler to use.</param>
        /// <returns>An object that when disposed, restores the previous default
        /// schedulers.</returns>
        public static IDisposable WithScheduler(IScheduler sched)
        {
            Monitor.Enter(schedGate);
            var prevDef = RxApp.DeferredScheduler;
            var prevTask = RxApp.TaskpoolScheduler;

            RxApp.DeferredScheduler = sched;
            RxApp.TaskpoolScheduler = sched;

            return Disposable.Create(() => {
                RxApp.DeferredScheduler = prevDef;
                RxApp.TaskpoolScheduler = prevTask;
                Monitor.Exit(schedGate);
            });
        }

        /// <summary>
        /// WithMessageBus allows you to override the default Message Bus 
        /// implementation until the object returned is disposed. If a 
        /// message bus is not specified, a default empty one is created.
        /// </summary>
        /// <param name="messageBus">The message bus to use, or null to create
        /// a new one using the default implementation.</param>
        /// <returns>An object that when disposed, restores the original 
        /// message bus.</returns>
        public static IDisposable WithMessageBus(this IMessageBus messageBus)
        {
            var origMessageBus = MessageBus.Current;

            Monitor.Enter(mbGate);
            MessageBus.Current = messageBus ?? new MessageBus();
            return Disposable.Create(() =>
            {
                MessageBus.Current = origMessageBus;
                Monitor.Exit(mbGate);
            });
        }

        /// <summary>
        /// With is an extension method that uses the given scheduler as the
        /// default Deferred and Taskpool schedulers for the given Func. Use
        /// this to initialize objects that store the default scheduler (most
        /// RxXaml objects).
        /// </summary>
        /// <param name="sched">The scheduler to use.</param>
        /// <param name="block">The function to execute.</param>
        /// <returns>The return value of the function.</returns>
        public static TRet With<T, TRet>(this T sched, Func<T, TRet> block)
            where T : IScheduler
        {
            TRet ret;
            using (WithScheduler(sched)) {
                ret = block(sched);
            }
            return ret;
        }

        /// <summary>
        /// With is an extension method that uses the given scheduler as the
        /// default Deferred and Taskpool schedulers for the given Action. 
        /// </summary>
        /// <param name="sched">The scheduler to use.</param>
        /// <param name="block">The action to execute.</param>
        public static void With<T>(this T sched, Action<T> block)
            where T : IScheduler
        {
            sched.With(x => { block(x); return 0; });
        }

        /// <summary>
        /// Override the default Message Bus during the specified block.
        /// </summary>
        /// <param name="messageBus">The message bus to use for the block.</param>
        /// <param name="block">The function to execute.</param>
        /// <returns>The return value of the function.</returns>
        public static TRet With<TRet>(this IMessageBus messageBus, Func<TRet> block)
        {
            using (messageBus.WithMessageBus()) {
                return block();
            }
        }

        /// <summary>
        /// Override the default Message Bus during the specified block.
        /// </summary>
        /// <param name="messageBus">The message bus to use for the block.</param>
        /// <param name="sched">The scheduler to use.</param>
        /// <param name="block">The action to execute.</param>
        public static void With(this IMessageBus messageBus, Action block)
        {
            using (messageBus.WithMessageBus()) {
                block();
            }
        }

        /// <summary>
        /// AdvanceToMs moves the TestScheduler to the specified time in
        /// milliseconds.
        /// </summary>
        /// <param name="milliseconds">The time offset to set the TestScheduler
        /// to, in milliseconds. Note that this is *not* additive or
        /// incremental, it sets the time.</param>
        public static void AdvanceToMs(this TestScheduler sched, double milliseconds)
        {
            sched.AdvanceTo(sched.FromTimeSpan(TimeSpan.FromMilliseconds(milliseconds)));
        }

        /// <summary>
        /// AdvanceToMs moves the TestScheduler along by the specified time in
        /// milliseconds.
        /// </summary>
        /// <param name="milliseconds">The time offset to set the TestScheduler
        /// to, in milliseconds. Note that this is *not* additive or
        /// incremental, it sets the time.</param>
        public static void AdvanceByMs(this TestScheduler sched, double milliseconds)
        {
            sched.AdvanceBy(sched.FromTimeSpan(TimeSpan.FromMilliseconds(milliseconds)));
        }

        /// <summary>
        /// OnNextAt is a method to help create simulated input Observables in
        /// conjunction with CreateHotObservable or CreateColdObservable.
        /// </summary>
        /// <param name="milliseconds">The time offset to fire the notification
        /// on the recorded notification.</param>
        /// <param name="value">The value to produce.</param>
        /// <returns>A recorded notification that can be provided to
        /// TestScheduler.CreateHotObservable.</returns>
        public static Recorded<Notification<T>> OnNextAt<T>(this TestScheduler sched, double milliseconds, T value)
        {
            return new Recorded<Notification<T>>(
                sched.FromTimeSpan(TimeSpan.FromMilliseconds(milliseconds)),
                Notification.CreateOnNext<T>(value));
        }

        /// <summary>
        /// OnErrorAt is a method to help create simulated input Observables in
        /// conjunction with CreateHotObservable or CreateColdObservable.
        /// </summary>
        /// <param name="milliseconds">The time offset to fire the notification
        /// on the recorded notification.</param>
        /// <param name="exception">The exception to terminate the Observable
        /// with.</param>
        /// <returns>A recorded notification that can be provided to
        /// TestScheduler.CreateHotObservable.</returns>
        public static Recorded<Notification<T>> OnErrorAt<T>(this TestScheduler sched, double milliseconds, Exception ex)
        {
            return new Recorded<Notification<T>>(
                sched.FromTimeSpan(TimeSpan.FromMilliseconds(milliseconds)),
                Notification.CreateOnError<T>(ex));
        }
        
        /// <summary>
        /// OnCompletedAt is a method to help create simulated input Observables in
        /// conjunction with CreateHotObservable or CreateColdObservable.
        /// </summary>
        /// <param name="milliseconds">The time offset to fire the notification
        /// on the recorded notification.</param>
        /// <returns>A recorded notification that can be provided to
        /// TestScheduler.CreateHotObservable.</returns>
        public static Recorded<Notification<T>> OnCompletedAt<T>(this TestScheduler sched, double milliseconds)
        {
            return new Recorded<Notification<T>>(
                sched.FromTimeSpan(TimeSpan.FromMilliseconds(milliseconds)),
                Notification.CreateOnCompleted<T>());
        }

        public static long FromTimeSpan(this TestScheduler sched, TimeSpan span)
        {
            return span.Ticks;
        }
    }
}

// vim: tw=120 ts=4 sw=4 et :