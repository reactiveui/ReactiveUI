using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Runtime.CompilerServices;
using System.Threading;
using Splat;

namespace ReactiveUI
{
    /// <summary>
    /// interface for ReactiveObject
    /// </summary>
    /// <seealso cref="System.ComponentModel.INotifyPropertyChanged"/>
    /// <seealso cref="ReactiveUI.INotifyPropertyChanging"/>
    /// <seealso cref="Splat.IEnableLogger"/>
    public interface IReactiveObject : INotifyPropertyChanged, INotifyPropertyChanging, IEnableLogger
    {
        /// <summary>
        /// Raises the property changed.
        /// </summary>
        /// <param name="args">
        /// The <see cref="PropertyChangedEventArgs"/> instance containing the event data.
        /// </param>
        void RaisePropertyChanged(PropertyChangedEventArgs args);

        /// <summary>
        /// Raises the property changing.
        /// </summary>
        /// <param name="args">
        /// The <see cref="PropertyChangingEventArgs"/> instance containing the event data.
        /// </param>
        void RaisePropertyChanging(PropertyChangingEventArgs args);
    }

    /// <summary>
    /// IReactive Object Extensions
    /// </summary>
    [Preserve(AllMembers = true)]
    public static class IReactiveObjectExtensions
    {
        private static ConditionalWeakTable<IReactiveObject, IExtensionState<IReactiveObject>> state = new ConditionalWeakTable<IReactiveObject, IExtensionState<IReactiveObject>>();

        private interface IExtensionState<out TSender> where TSender : IReactiveObject
        {
            IObservable<IReactivePropertyChangedEventArgs<TSender>> Changed { get; }

            IObservable<IReactivePropertyChangedEventArgs<TSender>> Changing { get; }

            IObservable<Exception> ThrownExceptions { get; }

            bool areChangeNotificationsDelayed();

            bool areChangeNotificationsEnabled();

            IDisposable delayChangeNotifications();

            void raisePropertyChanged(string propertyName);

            void raisePropertyChanging(string propertyName);

            IDisposable suppressChangeNotifications();
        }

        /// <summary>
        /// RaiseAndSetIfChanged fully implements a Setter for a read-write property on a
        /// ReactiveObject, using CallerMemberName to raise the notification and the ref to the
        /// backing field to set the property.
        /// </summary>
        /// <typeparam name="TObj">The type of the This.</typeparam>
        /// <typeparam name="TRet">The type of the return value.</typeparam>
        /// <param name="This">The <see cref="ReactiveObject"/> raising the notification.</param>
        /// <param name="backingField">A Reference to the backing field for this property.</param>
        /// <param name="newValue">The new value.</param>
        /// <param name="propertyName">
        /// The name of the property, usually automatically provided through the CallerMemberName attribute.
        /// </param>
        /// <returns>The newly set value, normally discarded.</returns>
        public static TRet RaiseAndSetIfChanged<TObj, TRet>(
            this TObj This,
            ref TRet backingField,
            TRet newValue,
            [CallerMemberName] string propertyName = null) where TObj : IReactiveObject
        {
            Contract.Requires(propertyName != null);

            if (EqualityComparer<TRet>.Default.Equals(backingField, newValue)) {
                return newValue;
            }

            This.raisePropertyChanging(propertyName);
            backingField = newValue;
            This.raisePropertyChanged(propertyName);
            return newValue;
        }

        /// <summary>
        /// Use this method in your ReactiveObject classes when creating custom properties where
        /// raiseAndSetIfChanged doesn't suffice.
        /// </summary>
        /// <param name="This">The instance of ReactiveObject on which the property has changed.</param>
        /// <param name="propertyName">
        /// A string representing the name of the property that has been changed. Leave <c>null</c>
        /// to let the runtime set to caller member name.
        /// </param>
        public static void RaisePropertyChanged<TSender>(this TSender This, [CallerMemberName] string propertyName = null)
            where TSender : IReactiveObject
        {
            This.raisePropertyChanged(propertyName);
        }

        /// <summary>
        /// Use this method in your ReactiveObject classes when creating custom properties where
        /// raiseAndSetIfChanged doesn't suffice.
        /// </summary>
        /// <param name="This">The instance of ReactiveObject on which the property has changed.</param>
        /// <param name="propertyName">
        /// A string representing the name of the property that has been changed. Leave <c>null</c>
        /// to let the runtime set to caller member name.
        /// </param>
        public static void RaisePropertyChanging<TSender>(this TSender This, [CallerMemberName] string propertyName = null)
            where TSender : IReactiveObject
        {
            This.raisePropertyChanging(propertyName);
        }

        internal static bool areChangeNotificationsEnabled<TSender>(this TSender This) where TSender : IReactiveObject
        {
            var s = state.GetValue(This, key => (IExtensionState<IReactiveObject>)new ExtensionState<TSender>(This));

            return s.areChangeNotificationsEnabled();
        }

        internal static IDisposable delayChangeNotifications<TSender>(this TSender This) where TSender : IReactiveObject
        {
            var s = state.GetValue(This, key => (IExtensionState<IReactiveObject>)new ExtensionState<TSender>(This));

            return s.delayChangeNotifications();
        }

        internal static IObservable<IReactivePropertyChangedEventArgs<TSender>> getChangedObservable<TSender>(this TSender This) where TSender : IReactiveObject
        {
            var val = state.GetValue(This, key => (IExtensionState<IReactiveObject>)new ExtensionState<TSender>(This));
            return val.Changed.Cast<IReactivePropertyChangedEventArgs<TSender>>();
        }

        internal static IObservable<IReactivePropertyChangedEventArgs<TSender>> getChangingObservable<TSender>(this TSender This) where TSender : IReactiveObject
        {
            var val = state.GetValue(This, key => (IExtensionState<IReactiveObject>)new ExtensionState<TSender>(This));
            return val.Changing.Cast<IReactivePropertyChangedEventArgs<TSender>>();
        }

        internal static IObservable<Exception> getThrownExceptionsObservable<TSender>(this TSender This) where TSender : IReactiveObject
        {
            var s = state.GetValue(This, key => (IExtensionState<IReactiveObject>)new ExtensionState<TSender>(This));
            return s.ThrownExceptions;
        }

        internal static void raisePropertyChanged<TSender>(this TSender This, string propertyName) where TSender : IReactiveObject
        {
            Contract.Requires(propertyName != null);

            var s = state.GetValue(This, key => (IExtensionState<IReactiveObject>)new ExtensionState<TSender>(This));

            s.raisePropertyChanged(propertyName);
        }

        internal static void raisePropertyChanging<TSender>(this TSender This, string propertyName) where TSender : IReactiveObject
        {
            Contract.Requires(propertyName != null);

            var s = state.GetValue(This, key => (IExtensionState<IReactiveObject>)new ExtensionState<TSender>(This));

            s.raisePropertyChanging(propertyName);
        }

        internal static IDisposable suppressChangeNotifications<TSender>(this TSender This) where TSender : IReactiveObject
        {
            var s = state.GetValue(This, key => (IExtensionState<IReactiveObject>)new ExtensionState<TSender>(This));

            return s.suppressChangeNotifications();
        }

        // Filter a list of change notifications, returning the last change for each PropertyName in
        // original order.
        private static IEnumerable<IReactivePropertyChangedEventArgs<TSender>> dedup<TSender>(IList<IReactivePropertyChangedEventArgs<TSender>> batch)
        {
            if (batch.Count <= 1) {
                return batch;
            }

            var seen = new HashSet<string>();
            var unique = new LinkedList<IReactivePropertyChangedEventArgs<TSender>>();

            for (int i = batch.Count - 1; i >= 0; i--) {
                if (seen.Add(batch[i].PropertyName)) {
                    unique.AddFirst(batch[i]);
                }
            }

            return unique;
        }

        private class ExtensionState<TSender> : IExtensionState<TSender> where TSender : IReactiveObject
        {
            private IObservable<IReactivePropertyChangedEventArgs<TSender>> changedObservable;
            private ISubject<IReactivePropertyChangedEventArgs<TSender>> changedSubject;
            private long changeNotificationsDelayed;
            private long changeNotificationsSuppressed;
            private IObservable<IReactivePropertyChangedEventArgs<TSender>> changingObservable;
            private ISubject<IReactivePropertyChangedEventArgs<TSender>> changingSubject;
            private TSender sender;
            private ISubject<Unit> startDelayNotifications;
            private ISubject<Exception> thrownExceptions;

            /// <summary>
            /// Initializes a new instance of the <see cref="ExtensionState{TSender}"/> class.
            /// </summary>
            public ExtensionState(TSender sender)
            {
                this.sender = sender;
                this.changingSubject = new Subject<IReactivePropertyChangedEventArgs<TSender>>();
                this.changedSubject = new Subject<IReactivePropertyChangedEventArgs<TSender>>();
                this.startDelayNotifications = new Subject<Unit>();
                this.thrownExceptions = new ScheduledSubject<Exception>(Scheduler.Immediate, RxApp.DefaultExceptionHandler);

                this.changedObservable = this.changedSubject
                    .Buffer(
                        Observable.Merge(
                            this.changedSubject.Where(_ => !areChangeNotificationsDelayed()).Select(_ => Unit.Default),
                            this.startDelayNotifications)
                    )
                    .SelectMany(batch => dedup(batch))
                    .Publish()
                    .RefCount();

                this.changingObservable = this.changingSubject
                    .Buffer(
                        Observable.Merge(
                            this.changingSubject.Where(_ => !areChangeNotificationsDelayed()).Select(_ => Unit.Default),
                            this.startDelayNotifications)
                    )
                    .SelectMany(batch => dedup(batch))
                    .Publish()
                    .RefCount();
            }

            public IObservable<IReactivePropertyChangedEventArgs<TSender>> Changed
            {
                get { return this.changedObservable; }
            }

            public IObservable<IReactivePropertyChangedEventArgs<TSender>> Changing
            {
                get { return this.changingObservable; }
            }

            public IObservable<Exception> ThrownExceptions
            {
                get { return this.thrownExceptions; }
            }

            public bool areChangeNotificationsDelayed()
            {
                return (Interlocked.Read(ref this.changeNotificationsDelayed) > 0);
            }

            public bool areChangeNotificationsEnabled()
            {
                return (Interlocked.Read(ref this.changeNotificationsSuppressed) == 0);
            }

            public IDisposable delayChangeNotifications()
            {
                if (Interlocked.Increment(ref this.changeNotificationsDelayed) == 1) {
                    this.startDelayNotifications.OnNext(Unit.Default);
                }

                return Disposable.Create(() => {
                    if (Interlocked.Decrement(ref this.changeNotificationsDelayed) == 0) {
                        this.startDelayNotifications.OnNext(Unit.Default);
                    };
                });
            }

            public void raisePropertyChanged(string propertyName)
            {
                if (!this.areChangeNotificationsEnabled())
                    return;

                var changed = new ReactivePropertyChangedEventArgs<TSender>(this.sender, propertyName);
                this.sender.RaisePropertyChanged(changed);

                this.notifyObservable(this.sender, changed, this.changedSubject);
            }

            public void raisePropertyChanging(string propertyName)
            {
                if (!this.areChangeNotificationsEnabled())
                    return;

                var changing = new ReactivePropertyChangingEventArgs<TSender>(this.sender, propertyName);
                this.sender.RaisePropertyChanging(changing);

                this.notifyObservable(this.sender, changing, this.changingSubject);
            }

            /// <summary>
            /// When this method is called, an object will not fire change notifications (neither
            /// traditional nor Observable notifications) until the return value is disposed.
            /// </summary>
            /// <returns>An object that, when disposed, reenables change notifications.</returns>
            public IDisposable suppressChangeNotifications()
            {
                Interlocked.Increment(ref this.changeNotificationsSuppressed);
                return Disposable.Create(() => Interlocked.Decrement(ref this.changeNotificationsSuppressed));
            }

            internal void notifyObservable<T>(TSender rxObj, T item, ISubject<T> subject)
            {
                try {
                    subject.OnNext(item);
                } catch (Exception ex) {
                    rxObj.Log().ErrorException("ReactiveObject Subscriber threw exception", ex);
                    this.thrownExceptions.OnNext(ex);
                }
            }
        }
    }
}