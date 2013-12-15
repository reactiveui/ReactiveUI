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
using Splat;

namespace ReactiveUI
{
    /// <summary>
    /// ReactiveObject is the base object for ViewModel classes, and it
    /// implements INotifyPropertyChanged. In addition, ReactiveObject provides
    /// Changing and Changed Observables to monitor object changes.
    /// </summary>
    [DataContract]
    public class ReactiveObject : IReactiveNotifyPropertyChanged<ReactiveObject>, IHandleObservableErrors, IReactiveObjectExtension
    {
        [field:IgnoreDataMember]
        public event PropertyChangingEventHandler PropertyChanging;

        void IReactiveObjectExtension.RaisePropertyChanging(PropertyChangingEventArgs args) 
        {
            var handler = PropertyChanging;

            if (handler != null) {
                handler(this, args);
            }
        }

        [field:IgnoreDataMember]
        public event PropertyChangedEventHandler PropertyChanged;

        void IReactiveObjectExtension.RaisePropertyChanged(PropertyChangedEventArgs args) 
        {
            var handler = PropertyChanged;

            if (handler != null) {
                handler(this, args);
            }
        }

        /// <summary>
        /// Represents an Observable that fires *before* a property is about to
        /// be changed.         
        /// </summary>
        [IgnoreDataMember]
        public IObservable<IObservedChange<ReactiveObject, object>> Changing {
            get { return this.getChangingObservable(); }
        }

        /// <summary>
        /// Represents an Observable that fires *after* a property has changed.
        /// </summary>
        [IgnoreDataMember]
        public IObservable<IObservedChange<ReactiveObject, object>> Changed
        {
            get { return this.getChangedObservable(); }
        }

        [IgnoreDataMember]
        public IObservable<Exception> ThrownExceptions { get { return this.getThrownExceptionsObservable(); } }
        
        protected ReactiveObject()
        {
            this.setupReactiveExtension();
        }

        [OnDeserialized]
        void setupRxObj(StreamingContext sc) { this.setupReactiveExtension(); }

        public IDisposable SuppressChangeNotifications() {
            return this.suppressChangeNotifications();
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
