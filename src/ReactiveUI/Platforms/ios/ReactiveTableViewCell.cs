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
    public abstract class ReactiveTableViewCell : UITableViewCell, IReactiveNotifyPropertyChanged<ReactiveTableViewCell>, IHandleObservableErrors, IReactiveObject, ICanActivate
    {
#if UNIFIED
        protected ReactiveTableViewCell(CGRect frame) : base(frame) { setupRxObj(); }
#else
        protected ReactiveTableViewCell(RectangleF frame) : base (frame) { setupRxObj(); }
#endif

        protected ReactiveTableViewCell(NSObjectFlag t) : base(t) { setupRxObj(); }
        protected ReactiveTableViewCell(NSCoder coder) : base(NSObjectFlag.Empty) { setupRxObj(); }
        protected ReactiveTableViewCell() : base() { setupRxObj(); }
        protected ReactiveTableViewCell(UITableViewCellStyle style, string reuseIdentifier) : base(style, reuseIdentifier) { setupRxObj(); }
        protected ReactiveTableViewCell(UITableViewCellStyle style, NSString reuseIdentifier) : base(style, reuseIdentifier) { setupRxObj(); }
        protected ReactiveTableViewCell(IntPtr handle) : base(handle) { setupRxObj(); }

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
        public IObservable<IReactivePropertyChangedEventArgs<ReactiveTableViewCell>> Changing {
            get { return this.getChangingObservable(); }
        }

        /// <summary>
        /// Represents an Observable that fires *after* a property has changed.
        /// </summary>
        public IObservable<IReactivePropertyChangedEventArgs<ReactiveTableViewCell>> Changed {
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

        public override void WillMoveToSuperview(UIView newsuper)
        {
            base.WillMoveToSuperview(newsuper);
            RxApp.MainThreadScheduler.Schedule(() => (newsuper != null ? activated : deactivated).OnNext(Unit.Default));
        }
    }

    public abstract class ReactiveTableViewCell<TViewModel> : ReactiveTableViewCell, IViewFor<TViewModel>
        where TViewModel : class
    {
#if UNIFIED
        protected ReactiveTableViewCell(CGRect frame) : base(frame) { }
#else
        protected ReactiveTableViewCell(RectangleF frame) : base (frame) { }
#endif

        protected ReactiveTableViewCell(NSObjectFlag t) : base(t) { }
        protected ReactiveTableViewCell(NSCoder coder) : base(NSObjectFlag.Empty) { }
        protected ReactiveTableViewCell() : base() { }
        protected ReactiveTableViewCell(UITableViewCellStyle style, string reuseIdentifier) : base(style, reuseIdentifier) { }
        protected ReactiveTableViewCell(UITableViewCellStyle style, NSString reuseIdentifier) : base(style, reuseIdentifier) { }
        protected ReactiveTableViewCell(IntPtr handle) : base(handle) { }

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
