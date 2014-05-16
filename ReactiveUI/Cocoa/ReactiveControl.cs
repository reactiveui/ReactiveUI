﻿using System;
using System.ComponentModel;
using System.Drawing;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Subjects;

#if !UIKIT
using MonoMac.Foundation;
using MonoMac.AppKit;
using UIControl = MonoMac.AppKit.NSControl;
#else
using MonoTouch.Foundation;
using MonoTouch.UIKit;
#endif

namespace ReactiveUI.Cocoa
{
    public class ReactiveControl : UIControl, IReactiveNotifyPropertyChanged<ReactiveControl>, IHandleObservableErrors, IReactiveObject, ICanActivate
    {
        protected ReactiveControl() : base()
        {
        }

        protected ReactiveControl(NSCoder c) : base(c)
        {
        }

        protected ReactiveControl(NSObjectFlag f) : base(f)
        {
        }

        protected ReactiveControl(IntPtr handle) : base(handle)
        {
        }

        protected ReactiveControl(RectangleF size) : base(size)
        {
        }

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
        public IObservable<IObservedChange<ReactiveControl, object>> Changing {
            get { return this.getChangingObservable(); }
        }

        /// <summary>
        /// Represents an Observable that fires *after* a property has changed.
        /// </summary>
        public IObservable<IObservedChange<ReactiveControl, object>> Changed {
            get { return this.getChangedObservable(); }
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

        public IObservable<Exception> ThrownExceptions { get { return this.getThrownExceptionsObservable(); } }

        Subject<Unit> activated = new Subject<Unit>();
        public IObservable<Unit> Activated { get { return activated; } }
        Subject<Unit> deactivated = new Subject<Unit>();
        public IObservable<Unit> Deactivated { get { return deactivated; } }

        #if UIKIT
        public override void WillMoveToSuperview(UIView newsuper)
        #else
        public override void ViewWillMoveToSuperview(NSView newsuper)
        #endif
        {
            #if UIKIT
            base.WillMoveToSuperview(newsuper);
            #else
            base.ViewWillMoveToSuperview(newsuper);
            #endif
            RxApp.MainThreadScheduler.Schedule(() => (newsuper != null ? activated : deactivated).OnNext(Unit.Default));
        }
    }
}
