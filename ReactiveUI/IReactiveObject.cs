using System;
using System.Runtime.CompilerServices;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Concurrency;
using System.Threading;
using System.Reactive.Disposables;
using System.Diagnostics.Contracts;
using System.ComponentModel;
using Splat;
using System.Collections.Generic;

namespace ReactiveUI 
{
    public interface IReactiveObject : INotifyPropertyChanged, INotifyPropertyChanging, IEnableLogger 
    {
        event PropertyChangingEventHandler PropertyChanging;
        event PropertyChangedEventHandler PropertyChanged;

        void RaisePropertyChanging(PropertyChangingEventArgs args);
        void RaisePropertyChanged(PropertyChangedEventArgs args);
    }

    [Preserve(AllMembers = true)]
    internal static class IReactiveObjectExtensions
    {
        static ConditionalWeakTable<IReactiveObject, IExtensionState<IReactiveObject>> state = new ConditionalWeakTable<IReactiveObject, IExtensionState<IReactiveObject>>();
        
        public static IObservable<IObservedChange<TSender, object>> getChangedObservable<TSender>(this TSender This) where TSender : IReactiveObject
        {
            var val = state.GetValue(This, key => (IExtensionState<IReactiveObject>)new ExtensionState<TSender>(This));
            return val.Changed.Cast<IObservedChange<TSender, object>>();
        }

        public static IObservable<IObservedChange<TSender, object>> getChangingObservable<TSender>(this TSender This) where TSender : IReactiveObject 
        {
            var val = state.GetValue(This, key => (IExtensionState<IReactiveObject>)new ExtensionState<TSender>(This));
            return val.Changing.Cast<IObservedChange<TSender, object>>();
        }

        public static IObservable<Exception> getThrownExceptionsObservable<TSender>(this TSender This) where TSender : IReactiveObject 
        {
            var s = state.GetValue(This, key => (IExtensionState<IReactiveObject>)new ExtensionState<TSender>(This));
            return s.ThrownExceptions;
        }

        public static void raisePropertyChanging<TSender>(this TSender This, string propertyName) where TSender : IReactiveObject 
        {
            Contract.Requires(propertyName != null);

            var s = state.GetValue(This, key => (IExtensionState<IReactiveObject>)new ExtensionState<TSender>(This));

            s.raisePropertyChanging(propertyName);
        }

        public static void raisePropertyChanged<TSender>(this TSender This, string propertyName) where TSender : IReactiveObject 
        {
            Contract.Requires(propertyName != null);

            var s = state.GetValue(This, key => (IExtensionState<IReactiveObject>)new ExtensionState<TSender>(This));
            
            s.raisePropertyChanged(propertyName);
        }

        public static IDisposable suppressChangeNotifications<TSender>(this TSender This) where TSender : IReactiveObject
        {
            var s = state.GetValue(This, key => (IExtensionState<IReactiveObject>)new ExtensionState<TSender>(This));

            return s.suppressChangeNotifications();
        }

        public static bool areChangeNotificationsEnabled<TSender>(this TSender This) where TSender : IReactiveObject
        {
            var s = state.GetValue(This, key => (IExtensionState<IReactiveObject>)new ExtensionState<TSender>(This));

            return s.areChangeNotificationsEnabled();
        }

        /// <summary>
        /// RaiseAndSetIfChanged fully implements a Setter for a read-write
        /// property on a ReactiveObject, using CallerMemberName to raise the notification
        /// and the ref to the backing field to set the property.
        /// </summary>
        /// <typeparam name="TObj">The type of the This.</typeparam>
        /// <typeparam name="TRet">The type of the return value.</typeparam>
        /// <param name="This">The <see cref="ReactiveObject"/> raising the notification.</param>
        /// <param name="backingField">A Reference to the backing field for this
        /// property.</param>
        /// <param name="newValue">The new value.</param>
        /// <param name="propertyName">The name of the property, usually 
        /// automatically provided through the CallerMemberName attribute.</param>
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
        /// Use this method in your ReactiveObject classes when creating custom
        /// properties where raiseAndSetIfChanged doesn't suffice.
        /// </summary>
        /// <param name="This">The instance of ReactiveObject on which the property has changed.</param>
        /// <param name="propertyName">
        /// A string representing the name of the property that has been changed.
        /// Leave <c>null</c> to let the runtime set to caller member name.
        /// </param>
        public static void RaisePropertyChanged(this IReactiveObject This, [CallerMemberName] string propertyName = null)
        {
            This.raisePropertyChanged(propertyName);
        }

        /// <summary>
        /// Use this method in your ReactiveObject classes when creating custom
        /// properties where raiseAndSetIfChanged doesn't suffice.
        /// </summary>
        /// <param name="This">The instance of ReactiveObject on which the property has changed.</param>
        /// <param name="propertyName">
        /// A string representing the name of the property that has been changed.
        /// Leave <c>null</c> to let the runtime set to caller member name.
        /// </param>
        public static void RaisePropertyChanging(this IReactiveObject This, [CallerMemberName] string propertyName = null)
        {
            This.raisePropertyChanging(propertyName);
        }

        class ExtensionState<TSender> : IExtensionState<TSender> where TSender : IReactiveObject
        {
            private long changeNotificationsSuppressed;
            private ISubject<IObservedChange<TSender, object>> changingSubject;
            private ISubject<IObservedChange<TSender, object>> changedSubject;
            private ISubject<Exception> thrownExceptions;

            private TSender sender;

            /// <summary>
            /// Initializes a new instance of the <see cref="ExtensionState{TSender}"/> class.
            /// </summary>
            public ExtensionState(TSender sender) 
            {
                this.sender = sender;
                this.changingSubject = new Subject<IObservedChange<TSender, object>>();
                this.changedSubject = new Subject<IObservedChange<TSender, object>>();
                this.thrownExceptions = new ScheduledSubject<Exception>(Scheduler.Immediate, RxApp.DefaultExceptionHandler);
            }

            public IObservable<IObservedChange<TSender, object>> Changing {
                get { return this.changingSubject; }
            }           

            public IObservable<IObservedChange<TSender, object>> Changed {
                get { return this.changedSubject; }
            }

            public IObservable<Exception> ThrownExceptions {
                get { return thrownExceptions; }
            }

            public bool areChangeNotificationsEnabled()
            {
                return (Interlocked.Read(ref changeNotificationsSuppressed) == 0);
            }            

            /// <summary>
            /// When this method is called, an object will not fire change
            /// notifications (neither traditional nor Observable notifications)
            /// until the return value is disposed.
            /// </summary>
            /// <returns>An object that, when disposed, reenables change
            /// notifications.</returns>
            public IDisposable suppressChangeNotifications()
            {
                Interlocked.Increment(ref changeNotificationsSuppressed);
                return Disposable.Create(() => Interlocked.Decrement(ref changeNotificationsSuppressed));
            }

            public void raisePropertyChanging(string propertyName)
            {
                if (!this.areChangeNotificationsEnabled())
                    return;

                sender.RaisePropertyChanging(new PropertyChangingEventArgs(propertyName));

                this.notifyObservable(sender, new ObservedChange<TSender, object>(sender, propertyName, null), this.changingSubject);
            }

            public void raisePropertyChanged(string propertyName)
            {
                if (!this.areChangeNotificationsEnabled())
                    return;

                sender.RaisePropertyChanged(new PropertyChangedEventArgs(propertyName));

                this.notifyObservable(sender, new ObservedChange<TSender, object>(sender, propertyName, null), this.changedSubject);
            }

            internal void notifyObservable<T>(IReactiveObject rxObj, T item, ISubject<T> subject)
            {
                try {
                    subject.OnNext(item);
                } catch (Exception ex) {
                    rxObj.Log().ErrorException("ReactiveObject Subscriber threw exception", ex);
                    thrownExceptions.OnNext(ex);
                }
            }
        }

        interface IExtensionState<out TSender> where TSender: IReactiveObject
        {
            IObservable<IObservedChange<TSender, object>> Changing { get; }

            IObservable<IObservedChange<TSender, object>> Changed { get; }

            void raisePropertyChanging(string propertyName);

            void raisePropertyChanged(string propertyName);

            IObservable<Exception> ThrownExceptions { get; }

            bool areChangeNotificationsEnabled();

            IDisposable suppressChangeNotifications();
        }
    }
}
