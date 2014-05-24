﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Reactive.Disposables;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Linq;
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
    public class ReactiveObject : IReactiveNotifyPropertyChanged<ReactiveObject>, IHandleObservableErrors, IReactiveObject
    {
        public event PropertyChangingEventHandler PropertyChanging {
            add { PropertyChangingEventManager.AddHandler(this, value); }
            remove { PropertyChangingEventManager.RemoveHandler(this, value); }
        }

        void IReactiveObject.RaisePropertyChanging(PropertyChangingEventArgs args) 
        {
            PropertyChangingEventManager.DeliverEvent(this, args);
        }

        public event PropertyChangedEventHandler PropertyChanged {
            add { PropertyChangedEventManager.AddHandler(this, value); }
            remove { PropertyChangedEventManager.RemoveHandler(this, value); }
        }

        void IReactiveObject.RaisePropertyChanged(PropertyChangedEventArgs args) 
        {
            PropertyChangedEventManager.DeliverEvent(this, args);
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
        public IObservable<IObservedChange<ReactiveObject, object>> Changed {
            get { return this.getChangedObservable(); }
        }

        [IgnoreDataMember]
        public IObservable<Exception> ThrownExceptions { get { return this.getThrownExceptionsObservable(); } }

        protected ReactiveObject()
        {
        }

        public IDisposable SuppressChangeNotifications()
        {
            return this.suppressChangeNotifications();
        }

        public bool AreChangeNotificationsEnabled() {
            return this.areChangeNotificationsEnabled();
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
