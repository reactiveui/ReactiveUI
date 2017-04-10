using System;
using System.ComponentModel;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Subjects;
using System.Reactive.Linq;

#if UNIFIED
using Foundation;
using CoreGraphics;
#elif UIKIT
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
#else
using System.Drawing;
using MonoMac.AppKit;
using MonoMac.Foundation;
using UIControl = MonoMac.AppKit.NSControl;
#endif

#if UNIFIED && UIKIT
using UIKit;
#elif UNIFIED && COCOA
using AppKit;
using UIControl = AppKit.NSControl;
#endif


namespace ReactiveUI
{
    public class ReactiveControl : UIControl, IReactiveNotifyPropertyChanged<ReactiveControl>, IHandleObservableErrors, IReactiveObject, ICanActivate, ICanForceManualActivation
    {
#if UNIFIED
        protected ReactiveControl(CGRect frame) : base(frame) { }
#else
        protected ReactiveControl(RectangleF frame) : base(frame) { }
#endif

        protected ReactiveControl() { }
        protected ReactiveControl(NSCoder c) : base(c) { }
        protected ReactiveControl(NSObjectFlag f) : base(f) { }
        protected ReactiveControl(IntPtr handle) : base(handle) { }

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
        public IObservable<IReactivePropertyChangedEventArgs<ReactiveControl>> Changing {
            get { return this.getChangingObservable(); }
        }

        /// <summary>
        /// Represents an Observable that fires *after* a property has changed.
        /// </summary>
        public IObservable<IReactivePropertyChangedEventArgs<ReactiveControl>> Changed {
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

        void ICanForceManualActivation.Activate(bool activate)
        {
            RxApp.MainThreadScheduler.Schedule(() =>
                (activate ? activated : deactivated).OnNext(Unit.Default));
        }
    }

    public abstract class ReactiveControl<TViewModel> : ReactiveControl, IViewFor<TViewModel>
        where TViewModel : class
    {
        protected ReactiveControl() { }
        protected ReactiveControl(NSCoder c) : base(c) { }
        protected ReactiveControl(NSObjectFlag f) : base(f) { }
        protected ReactiveControl(IntPtr handle) : base(handle) { }
#if UNIFIED
        protected ReactiveControl(CGRect frame) : base(frame) { }
#else
        protected ReactiveControl(RectangleF frame) : base(frame) { }
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
