// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using DynamicData;
using DynamicData.Binding;
using UIKit;
using NSView = UIKit.UIView;
using NSViewController = UIKit.UIViewController;

namespace ReactiveUI
{
    /// <summary>
    /// RoutedViewHost is a ReactiveNavigationController that monitors its RoutingState
    /// and keeps the navigation stack in line with it.
    /// </summary>
    public class RoutedViewHost : ReactiveNavigationController
    {
        private readonly SerialDisposable _titleUpdater;
        private RoutingState _router;
        private IObservable<string> _viewContractObservable;
        private bool _routerInstigated;

        /// <summary>
        /// Gets or sets the <see cref="RoutingState"/> of the view model stack.
        /// </summary>
        public RoutingState Router
        {
            get => _router;
            set => this.RaiseAndSetIfChanged(ref _router, value);
        }

        /// <summary>
        /// Gets or sets the view contract observable.
        /// </summary>
        public IObservable<string> ViewContractObservable
        {
            get => _viewContractObservable;
            set => this.RaiseAndSetIfChanged(ref _viewContractObservable, value);
        }

        /// <summary>
        /// Gets or sets the view locator.
        /// </summary>
        public IViewLocator ViewLocator { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RoutedViewHost"/> class.
        /// </summary>
        public RoutedViewHost()
        {
            ViewContractObservable = Observable.Return<string>(null);
            _titleUpdater = new SerialDisposable();

            this.WhenActivated(
                d =>
                {
                    d(this
                        .WhenAnyValue(x => x.Router)
                        .Where(x => x != null && x.NavigationStack.Count > 0 && ViewControllers.Length == 0)
                        .Subscribe(x =>
                        {
                            _routerInstigated = true;
                            NSViewController view = null;

                            foreach (var viewModel in x.NavigationStack)
                            {
                                view = ResolveView(Router.GetCurrentViewModel(), null);
                                PushViewController(view, false);
                            }

                            _titleUpdater.Disposable = Router.GetCurrentViewModel()
                                .WhenAnyValue(y => y.UrlPathSegment)
                                .Subscribe(y => view.NavigationItem.Title = y);

                            _routerInstigated = false;
                        }));

                    var navigationStackChanged = this.WhenAnyValue(x => x.Router)
                        .Where(x => x != null)
                        .Select(x => x.NavigationStack.ObserveCollectionChanges())
                        .Switch();

                    d(navigationStackChanged
                        .Where(x => x.EventArgs.Action == NotifyCollectionChangedAction.Add)
                        .Select(contract => new { View = ResolveView(Router.GetCurrentViewModel(), /*contract*/null), Animate = Router.NavigationStack.Count > 1 })
                        .Subscribe(x =>
                        {
                            if (_routerInstigated)
                            {
                                return;
                            }

                            _titleUpdater.Disposable = Router.GetCurrentViewModel()
                                .WhenAnyValue(y => y.UrlPathSegment)
                                .Subscribe(y => x.View.NavigationItem.Title = y);

                            _routerInstigated = true;

                            // super important that animate is false if it's the first view being pushed, otherwise iOS gets hella confused
                            // and calls PushViewController twice
                            PushViewController(x.View, x.Animate);

                            _routerInstigated = false;
                        }));

                    d(navigationStackChanged
                        .Where(x => x.EventArgs.Action == NotifyCollectionChangedAction.Reset)
                        .Subscribe(_ =>
                        {
                            _routerInstigated = true;
                            PopToRootViewController(true);
                            _routerInstigated = false;
                        }));

                    d(this
                        .WhenAnyObservable(x => x.Router.NavigateBack)
                        .Subscribe(x =>
                        {
                            _routerInstigated = true;
                            PopViewController(true);
                            _routerInstigated = false;
                        }));
                });
        }

        /// <inheritdoc/>
        public override void PushViewController(NSViewController viewController, bool animated)
        {
            base.PushViewController(viewController, animated);

            if (!_routerInstigated)
            {
                // code must be pushing a view directly against nav controller rather than using the router, so we need to manually sync up the router state
                // TODO: what should we _actually_ do here? Soft-check the view and VM type and ignore if they're not IViewFor/IRoutableViewModel?
                var view = (IViewFor)viewController;
                var viewModel = (IRoutableViewModel)view.ViewModel;
                Router.NavigationStack.Add(viewModel);
            }
        }

        /// <inheritdoc/>
        public override NSViewController PopViewController(bool animated)
        {
            if (!_routerInstigated)
            {
                // user must have clicked Back button in nav controller, so we need to manually sync up the router state
                Router.NavigationStack.RemoveAt(_router.NavigationStack.Count - 1);
            }

            return base.PopViewController(animated);
        }

        private UIViewController ResolveView(IRoutableViewModel viewModel, string contract)
        {
            if (viewModel == null)
            {
                return null;
            }

            var viewLocator = ViewLocator ?? ReactiveUI.ViewLocator.Current;
            var view = viewLocator.ResolveView(viewModel, contract);

            if (view == null)
            {
                throw new Exception($"Couldn't find a view for view model. You probably need to register an IViewFor<{viewModel.GetType().Name}>");
            }

            view.ViewModel = viewModel;
            var viewController = view as UIViewController;

            if (viewController == null)
            {
                throw new Exception($"View type {view.GetType().Name} for view model type {viewModel.GetType().Name} is not a UIViewController");
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
        private RoutingState _router;

#pragma warning disable SA1600 // Elements should be documented
        public RoutingState Router
        {
            get => _router;
            set => this.RaiseAndSetIfChanged(ref _router, value);
        }

        private IObservable<string> _viewContractObservable;

        public IObservable<string> ViewContractObservable
        {
            get => _viewContractObservable;
            set => this.RaiseAndSetIfChanged(ref _viewContractObservable, value);
        }

        private NSViewController _defaultContent;

        public NSViewController DefaultContent
        {
            get => _defaultContent;
            set => this.RaiseAndSetIfChanged(ref _defaultContent, value);
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

            vmAndContract.Subscribe(
                x =>
            {
                if (viewLastAdded != null)
                {
                    viewLastAdded.RemoveFromSuperview();
                }

                if (x.ViewModel == null)
                {
                    if (DefaultContent != null)
                    {
                        targetView.AddSubview(DefaultContent.View);
                    }

                    return;
                }

                var viewLocator = ViewLocator ?? ReactiveUI.ViewLocator.Current;
                var view = viewLocator.ResolveView(x.ViewModel, x.Contract) ?? viewLocator.ResolveView(x.ViewModel, null);
                view.ViewModel = x.ViewModel;

                if (view is NSViewController)
                {
                    viewLastAdded = ((NSViewController)view).View;
                }
                else if (view is NSView)
                {
                    viewLastAdded = (NSView)view;
                }
                else
                {
                    throw new Exception($"'{view.GetType().FullName}' must be an NSViewController or NSView");
                }

                targetView.AddSubview(viewLastAdded);
            }, RxApp.DefaultExceptionHandler.OnNext);
        }
#pragma warning restore SA1600 // Elements should be documented
    }
}
