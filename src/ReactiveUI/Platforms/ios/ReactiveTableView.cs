using System;
using System.ComponentModel;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Subjects;
using System.Reactive.Linq;

#if UNIFIED
using CoreGraphics;
using Foundation;
using UIKit;
#else
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
#endif

namespace ReactiveUI
{
    public abstract class ReactiveTableView : UITableView, IReactiveNotifyPropertyChanged<ReactiveTableView>, IHandleObservableErrors, IReactiveObject, ICanActivate, ICanForceManualActivation
    {
#if UNIFIED
        protected ReactiveTableView(CGRect frame) : base(frame) { }
        protected ReactiveTableView(CGRect frame, UITableViewStyle style) : base(frame, style) { }
#else
        protected ReactiveTableView(RectangleF frame) : base(frame) { }
        protected ReactiveTableView(RectangleF frame, UITableViewStyle style) : base(frame, style) { }
#endif

        protected ReactiveTableView(NSObjectFlag t) : base(t) { }
        protected ReactiveTableView(NSCoder coder) : base(coder) { }
        protected ReactiveTableView() { }

        protected ReactiveTableView(IntPtr handle) : base(handle) { }

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
        public IObservable<IReactivePropertyChangedEventArgs<ReactiveTableView>> Changing {
            get { return this.getChangingObservable(); }
        }

        /// <summary>
        /// Represents an Observable that fires *after* a property has changed.
        /// </summary>
        public IObservable<IReactivePropertyChangedEventArgs<ReactiveTableView>> Changed {
            get { return this.getChangedObservable(); }
        }

        public IDisposable SuppressChangeNotifications()
        {
            return this.suppressChangeNotifications();
        }

        public IObservable<Exception> ThrownExceptions { get { return this.getThrownExceptionsObservable(); } }

        Subject<Unit> activated = new Subject<Unit>();
        public IObservable<Unit> Activated { get { return activated.AsObservable(); } }
        Subject<Unit> deactivated = new Subject<Unit>();
        public IObservable<Unit> Deactivated { get { return deactivated.AsObservable(); } }

        public override void WillMoveToSuperview(UIView newsuper)
        {
            base.WillMoveToSuperview(newsuper);
            RxApp.MainThreadScheduler.Schedule(() => (newsuper != null ? activated : deactivated).OnNext(Unit.Default));
        }

        void ICanForceManualActivation.Activate(bool activate)
        {
            RxApp.MainThreadScheduler.Schedule(() =>
                (activate ? activated : deactivated).OnNext(Unit.Default));
        }
    }

    public abstract class ReactiveTableView<TViewModel> : ReactiveTableView, IViewFor<TViewModel>
        where TViewModel : class
    {
#if UNIFIED
        protected ReactiveTableView(CGRect frame) : base(frame) { }
        protected ReactiveTableView(CGRect frame, UITableViewStyle style) : base(frame, style) { }
#else
        protected ReactiveTableView(RectangleF frame) : base(frame) { }
        protected ReactiveTableView(RectangleF frame, UITableViewStyle style) : base(frame, style) { }
#endif

        protected ReactiveTableView(NSObjectFlag t) : base(t) { }
        protected ReactiveTableView(NSCoder coder) : base(coder) { }
        protected ReactiveTableView() { }

        protected ReactiveTableView(IntPtr handle) : base(handle) { }

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

