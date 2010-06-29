using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Diagnostics;
using System.ComponentModel;
using System.Reflection;

namespace ReactiveXaml
{
    public class ReactiveObject : IReactiveNotifyPropertyChanged, IEnableLogger
    {
        // Constructor
        protected ReactiveObject()
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

        protected T RaiseAndSetIfChanged<T>(T oldValue, T newValue, Action<T> setter, string propertyName)
        {
            // if oldValue == newValue...
            if (EqualityComparer<T>.Default.Equals(oldValue, newValue))
                return newValue;

            setter(newValue);
            RaisePropertyChanged(propertyName);
            return newValue;
        }

        protected void RaisePropertyChanged(string propertyName)
        {
            this.VerifyPropertyName(propertyName);

            PropertyChangedEventHandler handler = this.PropertyChanged;
            var e = new PropertyChangedEventArgs(propertyName);
            if (handler != null)
            {
                handler(this, e);
            }

            notifyObservable(e);
        }

        // Debugging Aides

        /// <summary>
        /// Warns the developer if this object does not have
        /// a public property with the specified name. This
        /// method does not exist in a Release build.
        /// </summary>
        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        public void VerifyPropertyName(string propertyName)
        {
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
        /// Raised when a property on this object has a new value.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        protected readonly Lazy<PropertyInfo[]> allPublicProperties;
        readonly Subject<PropertyChangedEventArgs> subject = new Subject<PropertyChangedEventArgs>(); 

        // IObservable Members
        public IDisposable Subscribe(IObserver<PropertyChangedEventArgs> observer)
        {
            return subject.Subscribe(observer);
        }

        void notifyObservable(PropertyChangedEventArgs item)
        {
            subject.OnNext(item);
        }
    } 
}