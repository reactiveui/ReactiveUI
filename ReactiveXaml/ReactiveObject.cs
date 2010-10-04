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

#if WINDOWS_PHONE
using Microsoft.Phone.Reactive;
#endif

namespace ReactiveXaml
{
    public class ReactiveObject : IReactiveNotifyPropertyChanged, INotifyPropertyChanging, IEnableLogger
    {
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

#if DEBUG
            var propCache =
                new MemoizingMRUCache<string,System.Reflection.PropertyInfo>((s,_) => this.GetType().GetProperty(s), 25);

            this.PropertyChanged += (o, e) => 
                this.Log().DebugFormat("Property {0} changed to '{1}'",
                    e.PropertyName, propCache.Get(e.PropertyName).GetValue(this, null));
#endif
        }

        public IObservable<PropertyChangingEventArgs> BeforeChange {
            get { return changingSubject; }
        }

        [Obsolete("Use the RaiseAndSetIfChanged<TObj,TRet> extension method instead")]
        protected T RaiseAndSetIfChanged<T>(T oldValue, T newValue, Action<T> setter, string propertyName)
        {
            // if oldValue == newValue...
            if (EqualityComparer<T>.Default.Equals(oldValue, newValue))
                return newValue;

            RaisePropertyChanging(propertyName);
            setter(newValue);
            RaisePropertyChanged(propertyName);
            return newValue;
        }

        protected void WatchCollection<T>(IReactiveCollection<T> collection, string propertyName)
        {
            Observable.Merge(
                collection.ItemsAdded.Select(_ => true),
                collection.ItemsRemoved.Select(_ => true),
                collection.ItemPropertyChanged.Select(_ => true))
              .Subscribe(_ => RaisePropertyChanged(propertyName));
        }

        protected internal void RaisePropertyChanging(string propertyName)
        {
            Contract.Requires(propertyName != null);

            verifyPropertyName(propertyName);

            var handler = this.PropertyChanging;
            var e = new PropertyChangingEventArgs(propertyName);
            if (handler != null) {
                handler(this, e);
            }

            notifyObservable(e, changingSubject);
        }

        protected internal void RaisePropertyChanged(string propertyName)
        {
            Contract.Requires(propertyName != null);

            verifyPropertyName(propertyName);

            var handler = this.PropertyChanged;
            var e = new PropertyChangedEventArgs(propertyName);
            if (handler != null) {
                handler(this, e);
            }

            notifyObservable(e, changedSubject);
        }

        // Debugging Aides

        /// <summary>
        /// Warns the developer if this object does not have
        /// a public property with the specified name. This
        /// method does not exist in a Release build.
        /// </summary>
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

                if (this.ThrowOnInvalidPropertyName)
                    throw new ArgumentException(msg);
                else
                    Debug.Fail(msg);
            }
#endif
        }

        /// <summary>
        /// Returns whether an exception is thrown, or if a Debug.Fail() is used
        /// when an invalid property name is passed to the VerifyPropertyName method.
        /// The default value is false, but subclasses used by unit tests might
        /// override this property's getter to return true.
        /// </summary>
        protected virtual bool ThrowOnInvalidPropertyName { get; private set; }

        // INotifyPropertyChanged Members

        /// <summary>
        /// Raised when a property on this object will have a new value.
        /// </summary>
        [field:IgnoreDataMember]
        public event PropertyChangingEventHandler PropertyChanging;

        /// <summary>
        /// Raised when a property on this object has a new value.
        /// </summary>
        [field:IgnoreDataMember]
        public event PropertyChangedEventHandler PropertyChanged;

        [IgnoreDataMember]
        protected Lazy<PropertyInfo[]> allPublicProperties;

        [IgnoreDataMember]
        Subject<PropertyChangingEventArgs> changingSubject = new Subject<PropertyChangingEventArgs>();

        [IgnoreDataMember]
        Subject<PropertyChangedEventArgs> changedSubject = new Subject<PropertyChangedEventArgs>();

        // IObservable Members
        public IDisposable Subscribe(IObserver<PropertyChangedEventArgs> observer)
        {
            return changedSubject.Subscribe(observer);
        }

        void notifyObservable<T>(T item, Subject<T> subject)
        {
            try {
                subject.OnNext(item);
            } catch (Exception ex) {
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

            field.SetValue(This, Value);
            This.RaisePropertyChanged(prop_name);

            return Value;
        }


    }
}

// vim: tw=120 ts=4 sw=4 et enc=utf8 :