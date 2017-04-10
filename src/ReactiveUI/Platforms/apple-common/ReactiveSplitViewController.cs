using System;
using System.ComponentModel;
using System.Reactive;
using System.Reactive.Subjects;
using System.Reactive.Linq;
using Foundation;

#if UIKIT
using UIKit;
using NSSplitViewController = UIKit.UISplitViewController;
using NSView = UIKit.UIView;
#else
using AppKit;
#endif

namespace ReactiveUI
{
    public abstract class ReactiveSplitViewController : NSSplitViewController,
    IReactiveNotifyPropertyChanged<ReactiveSplitViewController>, IHandleObservableErrors, IReactiveObject, ICanActivate
    {
#if UIKIT
        protected ReactiveSplitViewController(string nibName, NSBundle bundle) : base(nibName, bundle) { setupRxObj(); }
#endif
        protected ReactiveSplitViewController(IntPtr handle) : base(handle) { setupRxObj(); }
        protected ReactiveSplitViewController(NSObjectFlag t) : base(t) { setupRxObj(); }
        protected ReactiveSplitViewController(NSCoder coder) : base(coder) { setupRxObj(); }
        protected ReactiveSplitViewController() { setupRxObj(); }

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
        public IObservable<IReactivePropertyChangedEventArgs<ReactiveSplitViewController>> Changing {
            get { return this.getChangingObservable(); }
        }

        /// <summary>
        /// Represents an Observable that fires *after* a property has changed.
        /// </summary>
        public IObservable<IReactivePropertyChangedEventArgs<ReactiveSplitViewController>> Changed {
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
        public IObservable<Unit> Activated { get { return activated.AsObservable(); } }
        Subject<Unit> deactivated = new Subject<Unit>();
        public IObservable<Unit> Deactivated { get { return deactivated.AsObservable(); } }

#if UIKIT
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            activated.OnNext(Unit.Default);
            this.ActivateSubviews(true);
        }

        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);
            deactivated.OnNext(Unit.Default);
            this.ActivateSubviews(false);
        }
#else
        public override void ViewWillAppear()
        {
            base.ViewWillAppear();
            activated.OnNext(Unit.Default);
            this.ActivateSubviews(true);
        }

        public override void ViewDidDisappear()
        {
            base.ViewDidDisappear();
            deactivated.OnNext(Unit.Default);
            this.ActivateSubviews(false);
        }
#endif
    }

    public abstract class ReactiveSplitViewController<TViewModel> : ReactiveSplitViewController, IViewFor<TViewModel>
        where TViewModel : class
    {
#if UIKIT
        protected ReactiveSplitViewController(string nibName, NSBundle bundle) : base(nibName, bundle) { }
#endif
        protected ReactiveSplitViewController(IntPtr handle) : base(handle) { }
        protected ReactiveSplitViewController(NSObjectFlag t) : base(t) { }
        protected ReactiveSplitViewController(NSCoder coder) : base(coder) { }
        protected ReactiveSplitViewController() { }

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