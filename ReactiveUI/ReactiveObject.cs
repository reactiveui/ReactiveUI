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
    public class ReactiveObject : IReactiveNotifyPropertyChanged<IReactiveObject>, IHandleObservableErrors, IReactiveObject
    {
#if NET_45        
        public event PropertyChangingEventHandler PropertyChanging;

        void IReactiveObject.RaisePropertyChanging(PropertyChangingEventArgs args)
        {
            var handler = this.PropertyChanging;
            if(handler != null) {
                handler(this, args);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        void IReactiveObject.RaisePropertyChanged(PropertyChangedEventArgs args)
        {
            var handler = this.PropertyChanged;
            if (handler != null) {
                handler(this, args);
            }
        }
#else
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
#endif

        /// <summary>
        /// Represents an Observable that fires *before* a property is about to
        /// be changed.
        /// </summary>
        [IgnoreDataMember]
        public IObservable<IReactivePropertyChangedEventArgs<IReactiveObject>> Changing {
            get { return ((IReactiveObject) this).getChangingObservable(); }
        }

        /// <summary>
        /// Represents an Observable that fires *after* a property has changed.
        /// </summary>
        [IgnoreDataMember]
        public IObservable<IReactivePropertyChangedEventArgs<IReactiveObject>> Changed {
            get { return ((IReactiveObject) this).getChangedObservable(); }
        }

        /// <summary>
        ///
        /// </summary>
        [IgnoreDataMember]
        public IObservable<Exception> ThrownExceptions { get { return this.getThrownExceptionsObservable(); } }

        protected ReactiveObject()
        {
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public IDisposable SuppressChangeNotifications() {
            return this.suppressChangeNotifications();
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public bool AreChangeNotificationsEnabled() {
            return this.areChangeNotificationsEnabled();
        }

        public IDisposable DelayChangeNotifications() {
            return this.delayChangeNotifications();
        }
    }
}

// vim: tw=120 ts=4 sw=4 et :
