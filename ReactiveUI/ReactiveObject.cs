﻿using System;
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

#if DOTNETISOLDANDSAD || WINDOWS_PHONE
using System.Concurrency;
#endif

namespace ReactiveUI
{
    /// <summary>
    /// ReactiveObject is the base object for ViewModel classes, and it
    /// implements INotifyPropertyChanged. In addition, ReactiveObject provides
    /// Changing and Changed Observables to monitor object changes.
    /// </summary>
    [DataContract]
    public class ReactiveObject : IReactiveNotifyPropertyChanged
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
            get {
#if DEBUG
                this.Log().DebugFormat("Changed Subject 0x{0:X}", changedSubject.GetHashCode());
#endif
                return changedSubject;
            }
        }

        [IgnoreDataMember]
        protected Lazy<PropertyInfo[]> allPublicProperties;

        [IgnoreDataMember] 
        readonly Subject<IObservedChange<object, object>> changingSubject = 
            new Subject<IObservedChange<object, object>>();

        [IgnoreDataMember]
        readonly Subject<IObservedChange<object, object>> changedSubject = 
            new Subject<IObservedChange<object, object>>();

        [IgnoreDataMember]
        long changeNotificationsSuppressed = 0;
        
        // Constructor
        protected ReactiveObject()
        {
            setupRxObj();
        }

        [OnDeserialized]
        void setupRxObj(StreamingContext sc) { setupRxObj(); }

        void setupRxObj()
        {
            allPublicProperties = new Lazy<PropertyInfo[]>(() =>
                GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance));
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

        protected internal void raisePropertyChanging(string propertyName)
        {
            Contract.Requires(propertyName != null);

            verifyPropertyName(propertyName);
            if (!areChangeNotificationsEnabled)
                return;

            notifyObservable(new ObservedChange<object, object>() {
                PropertyName = propertyName, Sender = this, Value = null
            }, changingSubject);

            var handler = this.PropertyChanging;
            if (handler != null) {
                var e = new PropertyChangingEventArgs(propertyName);
                handler(this, e);
            }
		}

        protected internal void raisePropertyChanged(string propertyName)
        {
            Contract.Requires(propertyName != null);

            verifyPropertyName(propertyName);
            this.Log().DebugFormat("{0:X}.{1} changed", this.GetHashCode(), propertyName);

            if (!areChangeNotificationsEnabled) {
                this.Log().DebugFormat("Suppressed change");
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

        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        void verifyPropertyName(string propertyName)
        {
            Contract.Requires(propertyName != null);

            // If you raise PropertyChanged and do not specify a property name,
            // all properties on the object are considered to be changed by the binding system.
            if (String.IsNullOrEmpty(propertyName))
                return;

#if !SILVERLIGHT
            // Verify that the property name matches a real,
            // public, instance property on this object.
            if (TypeDescriptor.GetProperties(this)[propertyName] == null) {
                string msg = "Invalid property name: " + propertyName;
                this.Log().Error(msg);
            }
#endif
        }

        protected bool areChangeNotificationsEnabled {
            get { 
#if SILVERLIGHT
                // N.B. On most architectures, machine word aligned reads are 
                // guaranteed to be atomic - sorry WP7, you're out of luck
                return changeNotificationsSuppressed == 0;
#else
                return (Interlocked.Read(ref changeNotificationsSuppressed) == 0); 
#endif
            }
        }

        void notifyObservable<T>(T item, Subject<T> subject)
        {
#if DEBUG
            this.Log().DebugFormat("Firing observable to subject 0x{0:X}", subject.GetHashCode());
#endif
            try {
                subject.OnNext(item);
            } catch (Exception ex) {
                this.Log().Error(ex);
                subject.OnError(ex);
            }
        }
    } 

    public static class ReactiveObjectExpressionMixin
    {
        /// <summary>
        /// RaiseAndSetIfChanged fully implements a Setter for a read-write
        /// property on a ReactiveObject, making the assumption that the
        /// property has a backing field named "_NameOfProperty". To change this
        /// assumption, set RxApp.GetFieldNameForPropertyNameFunc.
        /// </summary>
        /// <param name="property">An Expression representing the property (i.e.
        /// 'x => x.SomeProperty'</param>
        /// <param name="newValue">The new value to set the property to, almost
        /// always the 'value' keyword.</param>
        /// <returns>The newly set value, normally discarded.</returns>
        public static TRet RaiseAndSetIfChanged<TObj, TRet>(
                this TObj This, 
                Expression<Func<TObj, TRet>> property, 
                TRet newValue)
            where TObj : ReactiveObject
        {
            Contract.Requires(This != null);
            Contract.Requires(property != null);

            FieldInfo field;
            string prop_name = RxApp.simpleExpressionToPropertyName<TObj, TRet>(property);

            field = RxApp.getFieldInfoForProperty<TObj>(prop_name);

            var field_val = field.GetValue(This);

            if (EqualityComparer<TRet>.Default.Equals((TRet)field_val, (TRet)newValue)) {
                return newValue;
            }

            This.raisePropertyChanging(prop_name);
            field.SetValue(This, newValue);
            This.raisePropertyChanged(prop_name);

            return newValue;
        }
        

        /// <summary>
        /// RaiseAndSetIfChanged fully implements a Setter for a read-write
        /// property on a ReactiveObject, making the assumption that the
        /// property has a backing field named "_NameOfProperty". To change this
        /// assumption, set RxApp.GetFieldNameForPropertyNameFunc.  This
        /// overload is intended for Silverlight and WP7 where reflection
        /// cannot access the private backing field.
        /// </summary>
        /// <param name="property">An Expression representing the property (i.e.
        /// 'x => x.SomeProperty'</param>
        /// <param name="backingField">A Reference to the backing field for this
        /// property.</param>
        /// <param name="newValue">The new value to set the property to, almost
        /// always the 'value' keyword.</param>
        /// <returns>The newly set value, normally discarded.</returns>
        public static TRet RaiseAndSetIfChanged<TObj, TRet>(
                this TObj This,
                Expression<Func<TObj, TRet>> property,
                ref TRet backingField,
                TRet newValue)
            where TObj : ReactiveObject
        {
            Contract.Requires(This != null);
            Contract.Requires(property != null);

            if (EqualityComparer<TRet>.Default.Equals(backingField, newValue)) {
                return newValue;
            }

            string prop_name = RxApp.simpleExpressionToPropertyName(property);

            This.raisePropertyChanging(prop_name);
            backingField = newValue;
            This.raisePropertyChanged(prop_name);
            return newValue;
        }

        /// <summary>
        /// Use this method in your ReactiveObject classes when creating custom
        /// properties where raiseAndSetIfChanged doesn't suffice.
        /// </summary>
        /// <param name="property">An Expression representing the property (i.e.
        /// 'x => x.SomeProperty'</param>
        public static void RaisePropertyChanging<TObj, TRet>(
                this TObj This,
                Expression<Func<TObj, TRet>> property)
            where TObj : ReactiveObject
        {
            var propName = RxApp.simpleExpressionToPropertyName(property);
            This.raisePropertyChanging(propName);
        }

        /// <summary>
        /// Use this method in your ReactiveObject classes when creating custom
        /// properties where raiseAndSetIfChanged doesn't suffice.
        /// </summary>
        /// <param name="property">An Expression representing the property (i.e.
        /// 'x => x.SomeProperty'</param>
        public static void RaisePropertyChanged<TObj, TRet>(
                this TObj This,
                Expression<Func<TObj, TRet>> property)
            where TObj : ReactiveObject
        {
            var propName = RxApp.simpleExpressionToPropertyName(property);
            This.raisePropertyChanged(propName);
        }
    }

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
            RaisePropertyChanging(target, RxApp.simpleExpressionToPropertyName(property));
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
            RaisePropertyChanged(target, RxApp.simpleExpressionToPropertyName(property));
        }
    }
}

// vim: tw=120 ts=4 sw=4 et :
