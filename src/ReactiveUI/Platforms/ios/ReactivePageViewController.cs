using System;
using System.ComponentModel;
using System.Reactive;
using System.Reactive.Subjects;
using System.Reactive.Linq;
using Foundation;
using UIKit;

namespace ReactiveUI
{
    public abstract class ReactivePageViewController : UIPageViewController,
        IReactiveNotifyPropertyChanged<ReactivePageViewController>, IHandleObservableErrors, IReactiveObject, ICanActivate
    {
        protected ReactivePageViewController(UIPageViewControllerTransitionStyle style, UIPageViewControllerNavigationOrientation orientation) : base(style, orientation) { setupRxObj(); }
        protected ReactivePageViewController(UIPageViewControllerTransitionStyle style, UIPageViewControllerNavigationOrientation orientation, NSDictionary options) : base(style, orientation, options) { setupRxObj(); }
        protected ReactivePageViewController(UIPageViewControllerTransitionStyle style, UIPageViewControllerNavigationOrientation orientation, UIPageViewControllerSpineLocation spineLocation) : base(style, orientation, spineLocation) { setupRxObj(); }
        protected ReactivePageViewController(UIPageViewControllerTransitionStyle style, UIPageViewControllerNavigationOrientation orientation, UIPageViewControllerSpineLocation spineLocation, float interPageSpacing) : base(style, orientation, spineLocation, interPageSpacing) { setupRxObj(); }
        protected ReactivePageViewController(string nibName, NSBundle bundle) : base(nibName, bundle) { setupRxObj(); }
        protected ReactivePageViewController(IntPtr handle) : base(handle) { setupRxObj(); }
        protected ReactivePageViewController(NSObjectFlag t) : base(t) { setupRxObj(); }
        protected ReactivePageViewController(NSCoder coder) : base(coder) { setupRxObj(); }
        protected ReactivePageViewController() { setupRxObj(); }

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
        public IObservable<IReactivePropertyChangedEventArgs<ReactivePageViewController>> Changing {
            get { return this.getChangingObservable(); }
        }

        /// <summary>
        /// Represents an Observable that fires *after* a property has changed.
        /// </summary>
        public IObservable<IReactivePropertyChangedEventArgs<ReactivePageViewController>> Changed {
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
    }

    public abstract class ReactivePageViewController<TViewModel> : ReactivePageViewController, IViewFor<TViewModel>
        where TViewModel : class
    {
        protected ReactivePageViewController(UIPageViewControllerTransitionStyle style, UIPageViewControllerNavigationOrientation orientation) : base(style, orientation) { }
        protected ReactivePageViewController(UIPageViewControllerTransitionStyle style, UIPageViewControllerNavigationOrientation orientation, NSDictionary options) : base(style, orientation, options) { }
        protected ReactivePageViewController(UIPageViewControllerTransitionStyle style, UIPageViewControllerNavigationOrientation orientation, UIPageViewControllerSpineLocation spineLocation) : base(style, orientation, spineLocation) { }
        protected ReactivePageViewController(UIPageViewControllerTransitionStyle style, UIPageViewControllerNavigationOrientation orientation, UIPageViewControllerSpineLocation spineLocation, float interPageSpacing) : base(style, orientation, spineLocation, interPageSpacing) { }
        protected ReactivePageViewController(string nibName, NSBundle bundle) : base(nibName, bundle) { }
        protected ReactivePageViewController(IntPtr handle) : base(handle) { }
        protected ReactivePageViewController(NSObjectFlag t) : base(t) { }
        protected ReactivePageViewController(NSCoder coder) : base(coder) { }
        protected ReactivePageViewController() { }

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
