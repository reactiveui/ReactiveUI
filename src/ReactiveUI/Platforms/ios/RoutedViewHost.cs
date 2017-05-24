using System;
using System.Reactive.Linq;
using System.Collections.Specialized;
using System.Reactive.Disposables;

#if UIKIT
using UIKit;
using NSView = UIKit.UIView;
using NSViewController = UIKit.UIViewController;
#else
using AppKit;
#endif

namespace ReactiveUI
{
    /// <summary>
    /// RoutedViewHost is a ReactiveNavigationController that monitors its RoutingState
    /// and keeps the navigation stack in line with it.
    /// </summary>
    public class RoutedViewHost : ReactiveNavigationController
    {
        private readonly SerialDisposable titleUpdater;
        private RoutingState router;
        private IObservable<string> viewContractObservable;
        private IViewLocator viewLocator;
        private bool routerInstigated;

        public RoutingState Router {
            get { return router; }
            set { this.RaiseAndSetIfChanged(ref router, value); }
        }

        public IObservable<string> ViewContractObservable {
            get { return viewContractObservable; }
            set { this.RaiseAndSetIfChanged(ref viewContractObservable, value); }
        }

        public IViewLocator ViewLocator {
            get { return this.viewLocator; }
            set { this.viewLocator = value; }
        }

        public RoutedViewHost()
        {
            this.ViewContractObservable = Observable.Return<string>(null);
            this.titleUpdater = new SerialDisposable();

            this.WhenActivated(
                d => {
                    d(this
                        .WhenAnyValue(x => x.Router)
                        .Where(x => x != null && x.NavigationStack.Count > 0 && this.ViewControllers.Length == 0)
                        .Subscribe(x => {
                            this.routerInstigated = true;
                            NSViewController view = null;

                            foreach (var viewModel in x.NavigationStack) {
                                view = this.ResolveView(this.Router.GetCurrentViewModel(), null);
                                this.PushViewController(view, false);
                            }

                            this.titleUpdater.Disposable = this.Router.GetCurrentViewModel()
                                .WhenAnyValue(y => y.UrlPathSegment)
                                .Subscribe(y => view.NavigationItem.Title = y);

                            this.routerInstigated = false;
                        }));

                    d(this
                        .WhenAnyValue(x => x.Router)
                        .Where(x => x != null)
                        .Select(x => x.NavigationStack.ItemsAdded)
                        .Switch()
                        .Where(x => x != null)
                        .Select(contract => new { View = this.ResolveView(this.Router.GetCurrentViewModel(), /*contract*/null), Animate = this.Router.NavigationStack.Count > 1 })
                        .Subscribe(x => {
                            if (this.routerInstigated) {
                                return;
                            }

                            this.titleUpdater.Disposable = this.Router.GetCurrentViewModel()
                                .WhenAnyValue(y => y.UrlPathSegment)
                                .Subscribe(y => x.View.NavigationItem.Title = y);

                            this.routerInstigated = true;

                            // super important that animate is false if it's the first view being pushed, otherwise iOS gets hella confused
                            // and calls PushViewController twice
                            this.PushViewController(x.View, x.Animate);

                            this.routerInstigated = false;
                        }));

                    d(this
                        .WhenAnyObservable(x => x.Router.NavigationStack.Changed)
                        .Where(x => x.Action == NotifyCollectionChangedAction.Reset)
                        .Subscribe(_ => {
                            this.routerInstigated = true;
                            this.PopToRootViewController(true);
                            this.routerInstigated = false;
                        }));

                    d(this
                        .WhenAnyObservable(x => x.Router.NavigateBack)
                        .Subscribe(x => {
                            this.routerInstigated = true;
                            this.PopViewController(true);
                            this.routerInstigated = false;
                        }));
                });
        }

        public override void PushViewController(NSViewController viewController, bool animated)
        {
            base.PushViewController(viewController, animated);

            if (!this.routerInstigated) {
                // code must be pushing a view directly against nav controller rather than using the router, so we need to manually sync up the router state
                // TODO: what should we _actually_ do here? Soft-check the view and VM type and ignore if they're not IViewFor/IRoutableViewModel?
                var view = (IViewFor)viewController;
                var viewModel = (IRoutableViewModel)view.ViewModel;
                this.Router.NavigationStack.Add(viewModel);
            }
        }

        public override NSViewController PopViewController(bool animated)
        {
            if (!this.routerInstigated) {
                // user must have clicked Back button in nav controller, so we need to manually sync up the router state
                this.Router.NavigationStack.RemoveAt(this.router.NavigationStack.Count - 1);
            }

            return base.PopViewController(animated);
        }

        private UIViewController ResolveView(IRoutableViewModel viewModel, string contract)
        {
            if (viewModel == null) {
                return null;
            }

            var viewLocator = this.ViewLocator ?? ReactiveUI.ViewLocator.Current;
            var view = viewLocator.ResolveView(viewModel, contract);

            if (view == null) {
                throw new Exception(
                    string.Format(
                        "Couldn't find a view for view model. You probably need to register an IViewFor<{0}>",
                        viewModel.GetType().Name));
            }

            view.ViewModel = viewModel;
            var viewController = view as UIViewController;

            if (viewController == null) {
                throw new Exception(
                    string.Format(
                        "View type {0} for view model type {1} is not a UIViewController",
                        view.GetType().Name,
                        viewModel.GetType().Name));
            }

            return viewController;
        }
    }

    /// <summary>
    /// RoutedViewHost is a helper class that will connect a RoutingState
    /// to an arbitrary NSView and attempt to load the View for the latest
    /// ViewModel as a child view of the target. Usually the target view will
    /// be the NSWindow.
    /// 
    /// This is a bit different than the XAML's RoutedViewHost in the sense
    /// that this isn't a Control itself, it only manipulates other Views.
    /// </summary>
    [Obsolete("Use RoutedViewHost instead. This class will be removed in a later release.")]
    public class RoutedViewHostLegacy : ReactiveObject
    {
        RoutingState _Router;
        public RoutingState Router {
            get { return _Router; }
            set { this.RaiseAndSetIfChanged(ref _Router, value); }
        }

        IObservable<string> _ViewContractObservable;
        public IObservable<string> ViewContractObservable {
            get { return _ViewContractObservable; }
            set { this.RaiseAndSetIfChanged(ref _ViewContractObservable, value); }
        }

        NSViewController _DefaultContent;
        public NSViewController DefaultContent {
            get { return _DefaultContent; }
            set { this.RaiseAndSetIfChanged(ref _DefaultContent, value); }
        }

        public IViewLocator ViewLocator { get; set; }

        public RoutedViewHostLegacy(NSView targetView)
        {
            NSView viewLastAdded = null;

            ViewContractObservable = Observable.Return(default(string));

            var vmAndContract = Observable.CombineLatest(
                this.WhenAnyObservable(x => x.Router.CurrentViewModel),
                this.WhenAnyObservable(x => x.ViewContractObservable),
                (vm, contract) => new { ViewModel = vm, Contract = contract, });

            vmAndContract.Subscribe(x => {
                if (viewLastAdded != null)
                    viewLastAdded.RemoveFromSuperview();

                if (x.ViewModel == null) {
                    if (DefaultContent != null)
                        targetView.AddSubview(DefaultContent.View);
                    return;
                }

                var viewLocator = ViewLocator ?? ReactiveUI.ViewLocator.Current;
                var view = viewLocator.ResolveView(x.ViewModel, x.Contract) ?? viewLocator.ResolveView(x.ViewModel, null);
                view.ViewModel = x.ViewModel;

                if (view is NSViewController) {
                    viewLastAdded = ((NSViewController)view).View;
                } else if (view is NSView) {
                    viewLastAdded = (NSView)view;
                } else {
                    throw new Exception(String.Format("'{0}' must be an NSViewController or NSView", view.GetType().FullName));
                }

                targetView.AddSubview(viewLastAdded);
            }, RxApp.DefaultExceptionHandler.OnNext);
        }
    }
}
