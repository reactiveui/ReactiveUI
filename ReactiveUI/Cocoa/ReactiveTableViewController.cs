using System;
using ReactiveUI;
using System.Runtime.Serialization;
using System.ComponentModel;
using System.Reflection;
using System.Reactive;
using System.Reactive.Subjects;
using System.Reactive.Concurrency;
using System.Linq;
using System.Threading;
using System.Reactive.Disposables;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Drawing;
using Splat;

#if UNIFIED
using Foundation;
using UIKit;
using NSTableViewController = UIKit.UITableViewController;
using NSTableViewStyle = UIKit.UITableViewStyle;
#else
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using NSTableViewController = MonoTouch.UIKit.UITableViewController;
using NSTableViewStyle = MonoTouch.UIKit.UITableViewStyle;
#endif

namespace ReactiveUI
{
    public abstract class ReactiveTableViewController : NSTableViewController, 
	IReactiveNotifyPropertyChanged<ReactiveTableViewController>, IHandleObservableErrors, IReactiveObject, ICanActivate
    {
        protected ReactiveTableViewController(NSTableViewStyle withStyle) : base(withStyle) { setupRxObj(); }
        protected ReactiveTableViewController(string nibName, NSBundle bundle) : base(nibName, bundle) { setupRxObj(); }
        protected ReactiveTableViewController(IntPtr handle) : base(handle) { setupRxObj(); }
        protected ReactiveTableViewController(NSObjectFlag t) : base(t) { setupRxObj(); }
        protected ReactiveTableViewController(NSCoder coder) : base(coder) { setupRxObj(); }
        protected ReactiveTableViewController() { setupRxObj(); }

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
        public IObservable<IReactivePropertyChangedEventArgs<ReactiveTableViewController>> Changing {
            get { return this.getChangingObservable(); }
        }

        /// <summary>
        /// Represents an Observable that fires *after* a property has changed.
        /// </summary>
        public IObservable<IReactivePropertyChangedEventArgs<ReactiveTableViewController>> Changed {
            get { return this.getChangedObservable(); }
        }

        public IObservable<Exception> ThrownExceptions { get { return this.getThrownExceptionsObservable(); } }

        void setupRxObj()
        {
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

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            activated.OnNext(Unit.Default);
            this.ActivateSubviews(true);
        }

        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);
            deactivated.OnNext(Unit.Default);
            this.ActivateSubviews(false);
        }
    }
}

