using System;
using System.ComponentModel;
using System.Reactive;
using System.Reactive.Subjects;
using System.Reactive.Linq;
using Foundation;

#if UIKIT
using UIKit;
using NSViewController = UIKit.UIViewController;
using NSView = UIKit.UIView;
#else
using AppKit; 
#endif

namespace ReactiveUI
{
    /// <summary>
    /// This is an View that is both an NSViewController and has ReactiveObject powers 
    /// (i.e. you can call RaiseAndSetIfChanged)
    /// </summary>
    public class ReactiveViewController : NSViewController,
        IReactiveNotifyPropertyChanged<ReactiveViewController>, IHandleObservableErrors, IReactiveObject, ICanActivate
    {
        protected ReactiveViewController() { }
        protected ReactiveViewController(NSCoder c) : base(c) { }
        protected ReactiveViewController(NSObjectFlag f) : base(f) { }
        protected ReactiveViewController(IntPtr handle) : base(handle) { }
        protected ReactiveViewController(string nibNameOrNull, NSBundle nibBundleOrNull) : base(nibNameOrNull, nibBundleOrNull) { }

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
        public IObservable<IReactivePropertyChangedEventArgs<ReactiveViewController>> Changing {
            get { return this.getChangingObservable(); }
        }

        /// <summary>
        /// Represents an Observable that fires *after* a property has changed.
        /// </summary>
        public IObservable<IReactivePropertyChangedEventArgs<ReactiveViewController>> Changed {
            get { return this.getChangedObservable(); }
        }

        public IObservable<Exception> ThrownExceptions { get { return this.getThrownExceptionsObservable(); } }

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

    public abstract class ReactiveViewController<TViewModel> : ReactiveViewController, IViewFor<TViewModel>
        where TViewModel : class
    {
        protected ReactiveViewController() { }
        protected ReactiveViewController(NSCoder c) : base(c) { }
        protected ReactiveViewController(NSObjectFlag f) : base(f) { }
        protected ReactiveViewController(IntPtr handle) : base(handle) { }
        protected ReactiveViewController(string nibNameOrNull, NSBundle nibBundleOrNull) : base(nibNameOrNull, nibBundleOrNull) { }

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

    static class UIViewControllerMixins
    {
        internal static void ActivateSubviews(this NSViewController This, bool activate)
        {
            This.View.ActivateSubviews(activate);
        }

        static void ActivateSubviews(this NSView This, bool activate)
        {
            foreach (var view in This.Subviews) {
                var subview = view as ICanForceManualActivation;

                if (subview != null) {
                    subview.Activate(activate);
                }

                view.ActivateSubviews(activate);
            }
        }
    }
}
