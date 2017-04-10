using System;
using System.ComponentModel;
using System.Reactive;
using System.Reactive.Subjects;
using System.Reactive.Linq;
using Foundation;
using UIKit;

namespace ReactiveUI
{
    public abstract class ReactiveCollectionViewController : UICollectionViewController,
        IReactiveNotifyPropertyChanged<ReactiveCollectionViewController>, IHandleObservableErrors, IReactiveObject, ICanActivate
    {
        protected ReactiveCollectionViewController(UICollectionViewLayout withLayout) : base(withLayout) { setupRxObj(); }
        protected ReactiveCollectionViewController(string nibName, NSBundle bundle) : base(nibName, bundle) { setupRxObj(); }
        protected ReactiveCollectionViewController(IntPtr handle) : base(handle) { setupRxObj(); }
        protected ReactiveCollectionViewController(NSObjectFlag t) : base(t) { setupRxObj(); }
        protected ReactiveCollectionViewController(NSCoder coder) : base(coder) { setupRxObj(); }
        protected ReactiveCollectionViewController() { setupRxObj(); }

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
        public IObservable<IReactivePropertyChangedEventArgs<ReactiveCollectionViewController>> Changing {
            get { return this.getChangingObservable(); }
        }

        /// <summary>
        /// Represents an Observable that fires *after* a property has changed.
        /// </summary>
        public IObservable<IReactivePropertyChangedEventArgs<ReactiveCollectionViewController>> Changed {
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

    public abstract class ReactiveCollectionViewController<TViewModel> : ReactiveCollectionViewController, IViewFor<TViewModel>
        where TViewModel : class
    {
        protected ReactiveCollectionViewController(UICollectionViewLayout withLayout) : base(withLayout) { }
        protected ReactiveCollectionViewController(string nibName, NSBundle bundle) : base(nibName, bundle) { }
        protected ReactiveCollectionViewController(IntPtr handle) : base(handle) { }
        protected ReactiveCollectionViewController(NSObjectFlag t) : base(t) { }
        protected ReactiveCollectionViewController(NSCoder coder) : base(coder) { }
        protected ReactiveCollectionViewController() { }

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
