using System;
using System.Reactive.Linq;
using ReactiveUI;
using System.Reactive.Disposables;

#if UNIFIED && UIKIT
using UIKit;
using NSView = UIKit.UIView;
using NSViewController = UIKit.UIViewController;
#elif UNIFIED && COCOA
using AppKit;
#elif UIKIT
using MonoTouch.UIKit;
using NSView = MonoTouch.UIKit.UIView;
using NSViewController = MonoTouch.UIKit.UIViewController;
#else
using MonoMac.AppKit;
#endif

namespace ReactiveUI
{
    public class ViewModelViewHost : ReactiveViewController
    {
        private readonly SerialDisposable currentView;
        private readonly ObservableAsPropertyHelper<string> viewContract;
        private IViewLocator viewLocator;
        private NSViewController defaultContent;
        private IReactiveObject viewModel;
        private IObservable<string> viewContractObservable;

        public ViewModelViewHost()
        {
            this.currentView = new SerialDisposable();
            this.viewContract = this
                .WhenAnyObservable(x => x.ViewContractObservable)
                .ToProperty(this, x => x.ViewContract, scheduler: RxApp.MainThreadScheduler);

            this.Initialize();
        }

        public IViewLocator ViewLocator
        {
            get { return viewLocator; }
            set { this.RaiseAndSetIfChanged(ref viewLocator, value); }
        }

        public NSViewController DefaultContent
        {
            get { return defaultContent; }
            set { this.RaiseAndSetIfChanged(ref defaultContent, value); }
        }

        public IReactiveObject ViewModel
        {
            get { return viewModel; }
            set { this.RaiseAndSetIfChanged(ref viewModel, value); }
        }

        public IObservable<string> ViewContractObservable
        {
            get { return viewContractObservable; }
            set { this.RaiseAndSetIfChanged(ref viewContractObservable, value); }
        }

        public string ViewContract
        {
            get { return this.viewContract.Value; }
            set { ViewContractObservable = Observable.Return(value); }
        }

        private void Initialize()
        {
            var viewChange = Observable
                .CombineLatest(
                    this.WhenAnyValue(x => x.ViewModel),
                    this.WhenAnyObservable(x => x.ViewContractObservable).StartWith((string)null),
                    (vm, contract) => new { ViewModel = vm, Contract = contract })
                .Where(x => x.ViewModel != null);

            var defaultViewChange = Observable
                .CombineLatest(
                    this.WhenAnyValue(x => x.ViewModel),
                    this.WhenAnyValue(x => x.DefaultContent),
                    (vm, defaultContent) => new { ViewModel = vm, DefaultContent = defaultContent })
                .Where(x => x.ViewModel == null && x.DefaultContent != null)
                .Select(x => x.DefaultContent);

            viewChange
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(
                    x =>
                    {
                        var viewLocator = ViewLocator ?? ReactiveUI.ViewLocator.Current;
                        var view = viewLocator.ResolveView(x.ViewModel, x.Contract);

                        if (view == null)
                        {
                            var message = string.Format("Unable to resolve view for \"{0}\"", x.ViewModel.GetType());

                            if (x.Contract != null)
                            {
                                message += string.Format(" and contract \"{0}\"", x.Contract.GetType());
                            }

                            message += ".";
                            throw new Exception(message);
                        }

                        var viewController = view as NSViewController;

                        if (viewController == null)
                        {
                            throw new Exception(
                                string.Format(
                                    "Resolved view type '{0}' is not a '{1}'.", 
                                    viewController.GetType().FullName,
                                    typeof(NSViewController).FullName));
                        }

                        view.ViewModel = x.ViewModel;
                        Adopt(this, viewController);

                        var disposables = new CompositeDisposable();
                        disposables.Add(viewController);
                        disposables.Add(Disposable.Create(() => Disown(viewController)));
                        currentView.Disposable = disposables;
                    });

            defaultViewChange
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(x => Adopt(this, x));
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                this.currentView.Dispose();
                this.viewContract.Dispose();
            }
        }

        private static void Adopt(NSViewController parent, NSViewController child)
        {
            parent.AddChildViewController(child);
            parent.View.AddSubview(child.View);

            // ensure the child view fills our entire frame
            child.View.Frame = parent.View.Bounds;
#if UIKIT
            child.View.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
#else
            child.View.AutoresizingMask = NSViewResizingMask.WidthSizable | NSViewResizingMask.HeightSizable;
#endif
            child.View.TranslatesAutoresizingMaskIntoConstraints = true;

#if UIKIT
            child.DidMoveToParentViewController(parent);
#endif
        }

        private static void Disown(NSViewController child)
        {
#if UIKIT
            child.WillMoveToParentViewController(null);
#endif
            child.View.RemoveFromSuperview();
            child.RemoveFromParentViewController();
        }
    }



    /// <summary>
    /// ViewModelViewHost is a helper class that will connect a ViewModel
    /// to an arbitrary NSView and attempt to load the View for the current
    /// ViewModel as a child view of the target. 
    /// 
    /// This is a bit different than the XAML's ViewModelViewHost in the sense
    /// that this isn't a Control itself, it only manipulates other Views.
    /// </summary>
    [Obsolete("Use ViewModelViewHost instead. This class will be removed in a later release.")]
    public class ViewModelViewHostLegacy : ReactiveObject 
    {
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ReactiveUI.Cocoa.ViewModelViewHost"/>
        /// will automatically create Auto Layout constraints tying the sub view to the parent view.
        /// </summary>
        /// <value><c>true</c> if add layout contraints to sub view; otherwise, <c>false</c>.</value>
        public bool AddAutoLayoutConstraintsToSubView { get; set; } 

        public ViewModelViewHostLegacy(NSView targetView)
        {
            if (targetView == null) throw new ArgumentNullException("targetView");

            NSView viewLastAdded = null;
            ViewContractObservable = Observable<string>.Default;

            var vmAndContract = Observable.CombineLatest(
                this.WhenAny(x => x.ViewModel, x => x.Value),
                this.WhenAnyObservable(x => x.ViewContractObservable),
                (vm, contract) => new { ViewModel = vm, Contract = contract, });

            vmAndContract.Subscribe(x => {
                if (viewLastAdded != null) viewLastAdded.RemoveFromSuperview();

                if (ViewModel == null) {
                    if (DefaultContent != null) targetView.AddSubview(DefaultContent.View);
                    return;
                }

                // get an instance of the view controller for the supplied VM + Contract
                var viewLocator = ViewLocator ?? ReactiveUI.ViewLocator.Current;
                var viewController = viewLocator.ResolveView(x.ViewModel, x.Contract);

                // if not found, throw
                if (viewController == null) {
                    var message = String.Format("Unable to resolve view for \"{0}\"", x.ViewModel.GetType());

                    if (x.Contract != null) {
                        message += String.Format(" and contract \"{0}\"", x.Contract.GetType());
                    }

                    message += ".";
                    throw new Exception(message);
                }

                // set the VM on the controller and stash a copy of the added view
                viewController.ViewModel = x.ViewModel;
                viewLastAdded = ((NSViewController)viewController).View;

                // sanity check, view controllers are expect to have a view
                if (viewLastAdded == null) {
                    var message = string.Format("No view associated with view controller {0}.", viewController.GetType());
                    throw new Exception(message);
                }

                if (AddAutoLayoutConstraintsToSubView) {
                    // see https://developer.apple.com/library/ios/documentation/userexperience/conceptual/AutolayoutPG/AdoptingAutoLayout/AdoptingAutoLayout.html
                    viewLastAdded.TranslatesAutoresizingMaskIntoConstraints = false;
                }

                targetView.AddSubview(viewLastAdded);

                if (AddAutoLayoutConstraintsToSubView) {
                    // Add edge constraints so that subview trails changes in parent
                    addEdgeConstraint(NSLayoutAttribute.Left,  targetView, viewLastAdded);
                    addEdgeConstraint(NSLayoutAttribute.Right, targetView, viewLastAdded);
                    addEdgeConstraint(NSLayoutAttribute.Top, targetView, viewLastAdded);
                    addEdgeConstraint(NSLayoutAttribute.Bottom,  targetView, viewLastAdded);
                }
            });
        }

        void addEdgeConstraint(NSLayoutAttribute edge, NSView parentView, NSView subView)
        {
            var constraint = NSLayoutConstraint.Create(subView, edge, NSLayoutRelation.Equal, parentView, edge, 1, 0);
            parentView.AddConstraint(constraint);
        }

        NSViewController _DefaultContent;
        public NSViewController DefaultContent {
            get { return _DefaultContent; }
            set { this.RaiseAndSetIfChanged(ref _DefaultContent, value); }
        }

        IReactiveObject _ViewModel;
        public IReactiveObject ViewModel {
            get { return _ViewModel; }
            set { this.RaiseAndSetIfChanged(ref _ViewModel, value); }
        }
        
        IObservable<string> _ViewContractObservable;
        public IObservable<string> ViewContractObservable {
            get { return _ViewContractObservable; }
            set { this.RaiseAndSetIfChanged(ref _ViewContractObservable, value); }
        }
       
        public IViewLocator ViewLocator { get; set; }
    }
}
