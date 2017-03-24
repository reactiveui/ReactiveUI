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
    public abstract class ReactiveCollectionViewCell : UICollectionViewCell, IReactiveNotifyPropertyChanged<ReactiveCollectionViewCell>, IHandleObservableErrors, IReactiveObject, ICanActivate
    {
#if UNIFIED
        protected ReactiveCollectionViewCell(CGRect frame) : base(frame) { setupRxObj(); }
#else
        protected ReactiveCollectionViewCell(RectangleF frame) : base (frame) { setupRxObj(); }
#endif

        protected ReactiveCollectionViewCell(NSObjectFlag t) : base(t) { setupRxObj(); }
        protected ReactiveCollectionViewCell(NSCoder coder) : base(NSObjectFlag.Empty) { setupRxObj(); }
        protected ReactiveCollectionViewCell() : base() { setupRxObj(); }

        protected ReactiveCollectionViewCell(IntPtr handle) : base(handle) { setupRxObj(); }

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
        public IObservable<IReactivePropertyChangedEventArgs<ReactiveCollectionViewCell>> Changing {
            get { return this.getChangingObservable(); }
        }

        /// <summary>
        /// Represents an Observable that fires *after* a property has changed.
        /// </summary>
        public IObservable<IReactivePropertyChangedEventArgs<ReactiveCollectionViewCell>> Changed {
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

    public abstract class ReactiveCollectionViewCell<TViewModel> : ReactiveCollectionViewCell, IViewFor<TViewModel>
        where TViewModel : class
    {
        protected ReactiveCollectionViewCell(NSObjectFlag t) : base(t) { }
        protected ReactiveCollectionViewCell(NSCoder coder) : base(NSObjectFlag.Empty) { }
        protected ReactiveCollectionViewCell() : base() { }
#if UNIFIED
        protected ReactiveCollectionViewCell(CGRect frame) : base(frame) { }
#else
        protected ReactiveCollectionViewCell(RectangleF frame) : base (frame) { }
#endif
        protected ReactiveCollectionViewCell(IntPtr handle) : base(handle) { }

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
