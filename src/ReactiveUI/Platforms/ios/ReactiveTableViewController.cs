using System;
using System.ComponentModel;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Foundation;
using NSTableViewController = UIKit.UITableViewController;
using NSTableViewStyle = UIKit.UITableViewStyle;

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

    public abstract class ReactiveTableViewController<TViewModel> : ReactiveTableViewController, IViewFor<TViewModel>
        where TViewModel : class
    {
        protected ReactiveTableViewController(NSTableViewStyle withStyle) : base(withStyle) { }
        protected ReactiveTableViewController(string nibName, NSBundle bundle) : base(nibName, bundle) { }
        protected ReactiveTableViewController(IntPtr handle) : base(handle) { }
        protected ReactiveTableViewController(NSObjectFlag t) : base(t) { }
        protected ReactiveTableViewController(NSCoder coder) : base(coder) { }
        protected ReactiveTableViewController() { }

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

