using System;
using System.Drawing;
using System.Runtime.Serialization;
using System.Reactive.Subjects;
using System.Reactive.Concurrency;
using System.Reflection;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Reactive.Disposables;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using Splat;
using System.Reactive;

#if UIKIT
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using NSViewController = MonoTouch.UIKit.UIViewController;
#else
using MonoMac.AppKit;
using MonoMac.Foundation;
#endif

namespace ReactiveUI.Cocoa
{
    /// <summary>
    /// This is an View that is both an NSViewController and has ReactiveObject powers 
    /// (i.e. you can call RaiseAndSetIfChanged)
    /// </summary>
    public class ReactiveViewController : NSViewController, 
	    IReactiveNotifyPropertyChanged<ReactiveViewController>, IHandleObservableErrors, IReactiveObject
#if UIKIT
        , ICanActivate
#endif
    {
        protected ReactiveViewController() : base()
        {
        }

        protected ReactiveViewController(NSCoder c) : base(c)
        {
        }

        protected ReactiveViewController(NSObjectFlag f) : base(f)
        {
        }

        protected ReactiveViewController(IntPtr handle) : base(handle)
        {
        }

        protected ReactiveViewController(string nibNameOrNull, NSBundle nibBundleOrNull) : base(nibNameOrNull, nibBundleOrNull)
        {
        }

        public event PropertyChangingEventHandler PropertyChanging
        {
            add { PropertyChangingEventManager.AddHandler(this, value); }
            remove { PropertyChangingEventManager.RemoveHandler(this, value); }
        }

        void IReactiveObject.RaisePropertyChanging(PropertyChangingEventArgs args)
        {
            PropertyChangingEventManager.DeliverEvent(this, args);
        }

        public event PropertyChangedEventHandler PropertyChanged
        {
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
        public IObservable<IObservedChange<ReactiveViewController, object>> Changing {
            get { return this.getChangingObservable(); }
        }

        /// <summary>
        /// Represents an Observable that fires *after* a property has changed.
        /// </summary>
        public IObservable<IObservedChange<ReactiveViewController, object>> Changed {
            get { return this.getChangedObservable(); }
        }

        public IObservable<Exception> ThrownExceptions { get { return this.getThrownExceptionsObservable(); } }

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
                
#if UIKIT
        Subject<Unit> activated = new Subject<Unit>();
        public IObservable<Unit> Activated { get { return activated; } }
        Subject<Unit> deactivated = new Subject<Unit>();
        public IObservable<Unit> Deactivated { get { return deactivated; } }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            activated.OnNext(Unit.Default);
        }

        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);
            deactivated.OnNext(Unit.Default);
        }
#endif
    }
}
