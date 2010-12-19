using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Diagnostics;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.Serialization;
using System.Linq.Expressions;
using System.Diagnostics.Contracts;
using System.Threading;

#if WINDOWS_PHONE
using Microsoft.Phone.Reactive;
#else
using System.Disposables;
#endif

namespace ReactiveXaml
{
    [DataContract]
    public class ReactiveObject : IReactiveNotifyPropertyChanged
    {
        [field:IgnoreDataMember]
        public event PropertyChangingEventHandler PropertyChanging;

        [field:IgnoreDataMember]
        public event PropertyChangedEventHandler PropertyChanged;

        [IgnoreDataMember]
        public IObservable<IObservedChange<object, object>> Changing {
            get { return changingSubject; }
        }

        [IgnoreDataMember]
        public IObservable<IObservedChange<object, object>> Changed {
            get { return changedSubject; }
        }

        [IgnoreDataMember]
        protected Lazy<PropertyInfo[]> allPublicProperties;

        [IgnoreDataMember] 
        Subject<IObservedChange<object, object>> changingSubject = new Subject<IObservedChange<object, object>>();

        [IgnoreDataMember]
        Subject<IObservedChange<object, object>> changedSubject = new Subject<IObservedChange<object, object>>();

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
            this.Log().Debug("Firing observable");
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
        public static TRet RaiseAndSetIfChanged<TObj, TRet>(this TObj This, Expression<Func<TObj, TRet>> Property, TRet Value)
            where TObj : ReactiveObject
        {
            Contract.Requires(This != null);
            Contract.Requires(Property != null);

            FieldInfo field;
            string prop_name = RxApp.expressionToPropertyName<TObj, TRet>(Property);

            field = RxApp.getFieldInfoForProperty<TObj>(prop_name);

            var field_val = field.GetValue(This);

            if (EqualityComparer<TRet>.Default.Equals((TRet)field_val, (TRet)Value))
                return Value;

            This.raisePropertyChanging(prop_name);
            field.SetValue(This, Value);
            This.raisePropertyChanged(prop_name);

            return Value;
        }
    }
}

// vim: tw=120 ts=4 sw=4 et :