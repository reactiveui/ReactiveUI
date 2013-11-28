using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Reactive.Disposables;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Subjects;
using System.Reflection;
using System.Runtime.Serialization;
using System.Threading;
using System.Reactive.Concurrency;
using System.Runtime.CompilerServices;

namespace ReactiveUI
{
    /// <summary>
    /// ReactiveObject is the base object for ViewModel classes, and it
    /// implements INotifyPropertyChanged. In addition, ReactiveObject provides
    /// Changing and Changed Observables to monitor object changes.
    /// </summary>
    [DataContract]
    public class ReactiveObject : IReactiveNotifyPropertyChanged, IHandleObservableErrors
    {
        [field:IgnoreDataMember]
        public event PropertyChangingEventHandler PropertyChanging;

        [field:IgnoreDataMember]
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Represents an Observable that fires *before* a property is about to
        /// be changed.         
        /// </summary>
        [IgnoreDataMember]
        public IObservable<IObservedChange<object, object>> Changing {
            get { return changingSubject; }
        }

        /// <summary>
        /// Represents an Observable that fires *after* a property has changed.
        /// </summary>
        [IgnoreDataMember]
        public IObservable<IObservedChange<object, object>> Changed {
            get { return changedSubject; }
        }

        [IgnoreDataMember]
        protected Lazy<PropertyInfo[]> allPublicProperties;

        [IgnoreDataMember]
        Subject<IObservedChange<object, object>> changingSubject;

        [IgnoreDataMember]
        Subject<IObservedChange<object, object>> changedSubject;

        [IgnoreDataMember]
        long changeNotificationsSuppressed = 0;

        [IgnoreDataMember]
        readonly ScheduledSubject<Exception> thrownExceptions = new ScheduledSubject<Exception>(Scheduler.Immediate, RxApp.DefaultExceptionHandler);

        [IgnoreDataMember]
        public IObservable<Exception> ThrownExceptions { get { return thrownExceptions; } }

        enum ESetupRxCallOrigin
        {
            Constructor,
            OnDeserialized,
        }

        protected ReactiveObject()
        {
            setupRxObj(ESetupRxCallOrigin.Constructor);
        }

        [OnDeserialized]
        void setupRxObj(StreamingContext sc) { setupRxObj(ESetupRxCallOrigin.OnDeserialized); }

        void setupRxObj(ESetupRxCallOrigin setupRxCallOrigin)
        {
            this.Log().Debug("{0:X} of type {1} created via {2}", this.GetHashCode(), this.GetType(), setupRxCallOrigin);

            changingSubject = new Subject<IObservedChange<object, object>>();
            changedSubject = new Subject<IObservedChange<object, object>>();

            allPublicProperties = new Lazy<PropertyInfo[]>(() =>
                GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).ToArray());
        }

        /// <summary>
        /// When this method is called, an object will not fire change
        /// notifications (neither traditional nor Observable notifications)
        /// until the return value is disposed.
        /// </summary>
        /// <returns>An object that, when disposed, reenables change
        /// notifications.</returns>
        public IDisposable SuppressChangeNotifications()
        {
            Interlocked.Increment(ref changeNotificationsSuppressed);
            return Disposable.Create(() =>
                Interlocked.Decrement(ref changeNotificationsSuppressed));
        }

        protected internal virtual void raisePropertyChanging(string propertyName)
        {
            Contract.Requires(propertyName != null);

            if (!areChangeNotificationsEnabled || changingSubject == null)
                return;

            var handler = this.PropertyChanging;
            if (handler != null) {
                var e = new PropertyChangingEventArgs(propertyName);
                handler(this, e);
            }

            notifyObservable(new ObservedChange<object, object>() {
                PropertyName = propertyName, Sender = this, Value = null
            }, changingSubject);
        }

        protected internal virtual void raisePropertyChanged(string propertyName)
        {
            Contract.Requires(propertyName != null);

            this.Log().Debug("{0:X}.{1} changed", this.GetHashCode(), propertyName);

            if (!areChangeNotificationsEnabled || changedSubject == null) {
                this.Log().Debug("Suppressed change");
                return;
            }

            var handler = this.PropertyChanged;
            if (handler != null) {
                var e = new PropertyChangedEventArgs(propertyName);
                handler(this, e);
            }

            notifyObservable(new ObservedChange<object, object>() {
                PropertyName = propertyName, Sender = this, Value = null
            }, changedSubject);
        }

        protected bool areChangeNotificationsEnabled {
            get {
                return (Interlocked.Read(ref changeNotificationsSuppressed) == 0);
            }
        }

        internal void notifyObservable<T>(T item, Subject<T> subject)
        {
            try {
                subject.OnNext(item);
            } catch (Exception ex) {
                this.Log().ErrorException("ReactiveObject Subscriber threw exception", ex);
                thrownExceptions.OnNext(ex);
            }
        }
    }

    public static class ReactiveObjectExpressionMixin
    {
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
                [CallerMemberName] string propertyName = null)
            where TObj : ReactiveObject
        {
            Contract.Requires(This != null);
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
        public static void RaisePropertyChanged<TObj>(
                this TObj This,
                [CallerMemberName] string propertyName = null)
            where TObj : ReactiveObject
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
        public static void RaisePropertyChanging<TObj>(
                this TObj This,
                [CallerMemberName] string propertyName = null)
            where TObj : ReactiveObject
        {
            This.raisePropertyChanging(propertyName);
        }
    }
}

namespace ReactiveUI.Testing
{
    public static class ReactiveObjectTestMixin
    {
        /// <summary>
        /// RaisePropertyChanging is a helper method intended for test / mock
        /// scenarios to manually fake a property change. 
        /// </summary>
        /// <param name="target">The ReactiveObject to invoke
        /// raisePropertyChanging on.</param>
        /// <param name="property">The property that will be faking a change.</param>
        public static void RaisePropertyChanging(ReactiveObject target, string property)
        {
            target.raisePropertyChanging(property);
        }

        /// <summary>
        /// RaisePropertyChanging is a helper method intended for test / mock
        /// scenarios to manually fake a property change. 
        /// </summary>
        /// <param name="target">The ReactiveObject to invoke
        /// raisePropertyChanging on.</param>
        /// <param name="property">The property that will be faking a change.</param>
        public static void RaisePropertyChanging<TSender, TValue>(TSender target, Expression<Func<TSender, TValue>> property)
            where TSender : ReactiveObject
        {
            RaisePropertyChanging(target, Reflection.SimpleExpressionToPropertyName(property));
        }

        /// <summary>
        /// RaisePropertyChanged is a helper method intended for test / mock
        /// scenarios to manually fake a property change. 
        /// </summary>
        /// <param name="target">The ReactiveObject to invoke
        /// raisePropertyChanging on.</param>
        /// <param name="property">The property that will be faking a change.</param>
        public static void RaisePropertyChanged(ReactiveObject target, string property)
        {
            target.raisePropertyChanged(property);
        }

        /// <summary>
        /// RaisePropertyChanged is a helper method intended for test / mock
        /// scenarios to manually fake a property change. 
        /// </summary>
        /// <param name="target">The ReactiveObject to invoke
        /// raisePropertyChanging on.</param>
        /// <param name="property">The property that will be faking a change.</param>
        public static void RaisePropertyChanged<TSender, TValue>(TSender target, Expression<Func<TSender, TValue>> property)
            where TSender : ReactiveObject
        {
            RaisePropertyChanged(target, Reflection.SimpleExpressionToPropertyName(property));
        }
    }
}

// vim: tw=120 ts=4 sw=4 et :
