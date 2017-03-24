using System;
using System.ComponentModel;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Subjects;
using System.Reactive.Linq;

#if UNIFIED
using CoreGraphics;
using Foundation;
#elif UIKIT
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using NSView = MonoTouch.UIKit.UIView;
#else
using System.Drawing;
using MonoMac.AppKit;
using MonoMac.Foundation;
#endif

#if UNIFIED && UIKIT
using NSView = UIKit.UIView;
using UIKit;
#elif UNIFIED && COCOA
using AppKit;
#endif

namespace ReactiveUI
{
    /// <summary>
    /// This is an View that is both an NSView and has ReactiveObject powers 
    /// (i.e. you can call RaiseAndSetIfChanged)
    /// </summary>
    public class ReactiveView : NSView, IReactiveNotifyPropertyChanged<ReactiveView>, IHandleObservableErrors, IReactiveObject, ICanActivate, ICanForceManualActivation
    {
        protected ReactiveView() { }
        protected ReactiveView(NSCoder c) : base(c) { }
        protected ReactiveView(NSObjectFlag f) : base(f) { }
        protected ReactiveView(IntPtr handle) : base(handle) { }
#if UNIFIED
        protected ReactiveView(CGRect frame) : base(frame) { }
#else
        protected ReactiveView(RectangleF size) : base(size) { }
#endif

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
        public IObservable<IReactivePropertyChangedEventArgs<ReactiveView>> Changing {
            get { return this.getChangingObservable(); }
        }

        /// <summary>
        /// Represents an Observable that fires *after* a property has changed.
        /// </summary>
        public IObservable<IReactivePropertyChangedEventArgs<ReactiveView>> Changed {
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
        public IObservable<Unit> Activated { get { return activated.AsObservable(); } }
        Subject<Unit> deactivated = new Subject<Unit>();
        public IObservable<Unit> Deactivated { get { return deactivated.AsObservable(); } }

#if UIKIT
        public override void WillMoveToSuperview(NSView newsuper)
#else
        public override void ViewWillMoveToSuperview(NSView newsuper)
#endif
        {
#if UIKIT
            base.WillMoveToSuperview(newsuper);
#else
            // Xamarin throws ArgumentNullException if newsuper is null
            if (newsuper != null) {
                base.ViewWillMoveToSuperview(newsuper);
            }
#endif
            RxApp.MainThreadScheduler.Schedule(() => (newsuper != null ? activated : deactivated).OnNext(Unit.Default));
        }

        void ICanForceManualActivation.Activate(bool activate)
        {
            RxApp.MainThreadScheduler.Schedule(() =>
                (activate ? activated : deactivated).OnNext(Unit.Default));
        }
    }

    public abstract class ReactiveView<TViewModel> : ReactiveView, IViewFor<TViewModel>
        where TViewModel : class
    {
        protected ReactiveView() { }
        protected ReactiveView(NSCoder c) : base(c) { }
        protected ReactiveView(NSObjectFlag f) : base(f) { }
        protected ReactiveView(IntPtr handle) : base(handle) { }
#if UNIFIED
        protected ReactiveView(CGRect frame) : base(frame) { }
#else
        protected ReactiveView(RectangleF size) : base(size) { }
#endif

        TViewModel _viewModel;
        public TViewModel ViewModel {
            get { return _viewModel; }
            set { this.RaiseAndSetIfChanged(ref _viewModel, value); }
        }

        object IViewFor.ViewModel {
            get { return ViewModel; }
            set { ViewModel = (TViewModel)value; }
        }
    }
}
