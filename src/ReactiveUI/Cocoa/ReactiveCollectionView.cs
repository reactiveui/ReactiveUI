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
    public abstract class ReactiveCollectionView : UICollectionView,
        IReactiveNotifyPropertyChanged<ReactiveCollectionView>, IHandleObservableErrors, IReactiveObject, ICanActivate, ICanForceManualActivation
    {
#if UNIFIED
        protected ReactiveCollectionView(CGRect frame, UICollectionViewLayout layout) : base(frame, layout) { setupRxObj(); }
#else
        protected ReactiveCollectionView(RectangleF frame, UICollectionViewLayout layout) : base(frame, layout) { setupRxObj(); }
#endif

        protected ReactiveCollectionView(IntPtr handle) : base(handle) { setupRxObj(); }
        protected ReactiveCollectionView(NSObjectFlag t) : base(t) { setupRxObj(); }
        protected ReactiveCollectionView(NSCoder coder) : base(coder) { setupRxObj(); }

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
        public IObservable<IReactivePropertyChangedEventArgs<ReactiveCollectionView>> Changing {
            get { return this.getChangingObservable(); }
        }

        /// <summary>
        /// Represents an Observable that fires *after* a property has changed.
        /// </summary>
        public IObservable<IReactivePropertyChangedEventArgs<ReactiveCollectionView>> Changed {
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
            activated.OnNext(Unit.Default);
        }

        public override void RemoveFromSuperview()
        {
            base.RemoveFromSuperview();
            deactivated.OnNext(Unit.Default);
        }

        void ICanForceManualActivation.Activate(bool activate)
        {
            RxApp.MainThreadScheduler.Schedule(() =>
                (activate ? activated : deactivated).OnNext(Unit.Default));
        }
    }

    public abstract class ReactiveCollectionView<TViewModel> : ReactiveCollectionView, IViewFor<TViewModel>
        where TViewModel : class
    {
        protected ReactiveCollectionView(IntPtr handle) : base(handle) { }
        protected ReactiveCollectionView(NSObjectFlag t) : base(t) { }
        protected ReactiveCollectionView(NSCoder coder) : base(coder) { }
#if UNIFIED
        protected ReactiveCollectionView(CGRect frame, UICollectionViewLayout layout) : base(frame, layout) { }
#else
        protected ReactiveCollectionView(RectangleF frame, UICollectionViewLayout layout) : base(frame, layout) { }
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
