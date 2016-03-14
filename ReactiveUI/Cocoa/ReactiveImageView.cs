using System;
using System.ComponentModel;
using System.Drawing;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Subjects;
using ReactiveUI;

#if UNIFIED
using CoreGraphics;
using Foundation;
#elif UIKIT
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using NSImageView = MonoTouch.UIKit.UIImageView;
using NSImage = MonoTouch.UIKit.UIImage;
using NSView = MonoTouch.UIKit.UIView;
#else
using MonoMac.AppKit;
#endif

#if UNIFIED && UIKIT
using UIKit;
using NSImage = UIKit.UIImage;
using NSImageView = UIKit.UIImageView;
using NSView = UIKit.UIView;
#elif UNIFIED && COCOA
using AppKit;
#endif


namespace ReactiveUI
{
    public abstract class ReactiveImageView : NSImageView, IReactiveNotifyPropertyChanged<ReactiveImageView>, IHandleObservableErrors, IReactiveObject, ICanActivate, ICanForceManualActivation
    {
#if UNIFIED
        public ReactiveImageView(CGRect frame) : base(frame) { }
#else
        public ReactiveImageView(RectangleF frame) : base(frame) { }
#endif

        public ReactiveImageView() { }

#if UIKIT
        public ReactiveImageView(NSImage image) : base(image) { }
        public ReactiveImageView(NSObjectFlag t) : base(t) { }
        public ReactiveImageView(NSImage image, NSImage highlightedImage) : base(image, highlightedImage) { }
        public ReactiveImageView(NSCoder coder) : base(coder) { }
#endif

        protected ReactiveImageView(IntPtr handle) : base(handle) { }

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
        public IObservable<IReactivePropertyChangedEventArgs<ReactiveImageView>> Changing {
            get { return this.getChangingObservable(); }
        }

        /// <summary>
        /// Represents an Observable that fires *after* a property has changed.
        /// </summary>
        public IObservable<IReactivePropertyChangedEventArgs<ReactiveImageView>> Changed {
            get { return this.getChangedObservable(); }
        }

        public IDisposable SuppressChangeNotifications() {
            return this.suppressChangeNotifications();
        }

        public IObservable<Exception> ThrownExceptions { get { return this.getThrownExceptionsObservable(); } }
        
        Subject<Unit> activated = new Subject<Unit>();
        public IObservable<Unit> Activated { get { return activated; } }
        Subject<Unit> deactivated = new Subject<Unit>();
        public IObservable<Unit> Deactivated { get { return deactivated; } }

#if UIKIT
        public override void WillMoveToSuperview(NSView newsuper)
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

        void ICanForceManualActivation.Activate(bool activate) 
        {
            RxApp.MainThreadScheduler.Schedule(() => 
                (activate ? activated : deactivated).OnNext(Unit.Default));
        }
    }
}
