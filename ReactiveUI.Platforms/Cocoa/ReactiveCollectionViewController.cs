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
using Splat;
using System.Reactive;

namespace ReactiveUI.Cocoa
{
    public abstract class ReactiveCollectionViewController : UICollectionViewController, IReactiveNotifyPropertyChanged, IHandleObservableErrors, IReactiveObjectExtension, ICanActivate
    {
        protected ReactiveCollectionViewController(UICollectionViewLayout withLayout) : base(withLayout) { setupRxObj(); }
        protected ReactiveCollectionViewController(string nibName, NSBundle bundle) : base(nibName, bundle) { setupRxObj(); }
        protected ReactiveCollectionViewController(IntPtr handle) : base(handle) { setupRxObj(); }
        protected ReactiveCollectionViewController(NSObjectFlag t) : base(t) { setupRxObj(); }
        protected ReactiveCollectionViewController(NSCoder coder) : base(coder) { setupRxObj(); }
        protected ReactiveCollectionViewController() { setupRxObj(); }

        public event PropertyChangingEventHandler PropertyChanging;

        void IReactiveObjectExtension.RaisePropertyChanging(PropertyChangingEventArgs args) 
        {
            var handler = PropertyChanging;
            if (handler != null) {
                handler(this, args);
            }
        }

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
        public IObservable<IObservedChange<object, object>> Changing {
            get { return this.getChangingObservable(); }
        }

        /// <summary>
        /// Represents an Observable that fires *after* a property has changed.
        /// </summary>
        public IObservable<IObservedChange<object, object>> Changed {
            get { return this.getChangedObservable(); }
        }

        public IObservable<Exception> ThrownExceptions { get { return this.getThrownExceptionsObservable(); } }

        void setupRxObj()
        {
            this.setupReactiveExtension();
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
            return this.suppressChangeNotifications();
        }
                
        Subject<Unit> activated = new Subject<Unit>();
        public IObservable<Unit> Activated { get { return activated; } }
        Subject<Unit> deactivated = new Subject<Unit>();
        public IObservable<Unit> Deactivated { get { return deactivated; } }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            activated.OnNext(Unit.Default);
        }

        public override void ViewDidUnload()
        {
            base.ViewDidUnload();
            deactivated.OnNext(Unit.Default);
        }
    }
}
