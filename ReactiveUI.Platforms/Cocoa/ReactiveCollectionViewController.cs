using System;
using ReactiveUI;
using System.Runtime.Serialization;
using System.ComponentModel;
using System.Reflection;
using System.Reactive.Subjects;
using System.Reactive.Concurrency;
using System.Linq;
using System.Threading;
using System.Reactive.Disposables;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Drawing;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace ReactiveUI.Cocoa
{
    public abstract class ReactiveCollectionViewController : UICollectionViewController, IReactiveNotifyPropertyChanged, IHandleObservableErrors
    {
        protected ReactiveCollectionViewController(UICollectionViewLayout withLayout) : base(withLayout) { setupRxObj(); }
        protected ReactiveCollectionViewController(string nibName, NSBundle bundle) : base(nibName, bundle) { setupRxObj(); }
        protected ReactiveCollectionViewController(IntPtr handle) : base(handle) { setupRxObj(); }
        protected ReactiveCollectionViewController(NSObjectFlag t) : base(t) { setupRxObj(); }
        protected ReactiveCollectionViewController(NSCoder coder) : base(coder) { setupRxObj(); }
        protected ReactiveCollectionViewController() { setupRxObj(); }

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

        [OnDeserialized]
        void setupRxObj(StreamingContext sc) { setupRxObj(); }

        void setupRxObj()
        {
            changingSubject = new Subject<IObservedChange<object, object>>();
            changedSubject = new Subject<IObservedChange<object, object>>();

            allPublicProperties = new Lazy<PropertyInfo[]>(() => GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).ToArray());
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
            return Disposable.Create(() => Interlocked.Decrement(ref changeNotificationsSuppressed));
        }

        protected internal void raisePropertyChanging(string propertyName)
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

        protected internal void raisePropertyChanged(string propertyName)
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
        public TRet RaiseAndSetIfChanged<TRet>(
            ref TRet backingField,
            TRet newValue,
            [CallerMemberName] string propertyName = null)
        {
            Contract.Requires(propertyName != null);

            if (EqualityComparer<TRet>.Default.Equals(backingField, newValue)) {
                return newValue;
            }

            raisePropertyChanging(propertyName);
            backingField = newValue;
            raisePropertyChanged(propertyName);
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
        public void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            raisePropertyChanged(propertyName);
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
        public void RaisePropertyChanging([CallerMemberName] string propertyName = null)
        {
            raisePropertyChanging(propertyName);
        }
    }
}
