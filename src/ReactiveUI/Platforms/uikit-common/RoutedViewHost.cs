﻿// Copyright (c) 2019 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using DynamicData;
using DynamicData.Binding;
using NSViewController = UIKit.UIViewController;

namespace ReactiveUI
{
    /// <summary>
    /// RoutedViewHost is a ReactiveNavigationController that monitors its RoutingState
    /// and keeps the navigation stack in line with it.
    /// </summary>
    [SuppressMessage("Design", "CA1010: Implement generic IEnumerable", Justification = "UI Kit exposes IEnumerable")]
    public class RoutedViewHost : ReactiveNavigationController
    {
        private readonly SerialDisposable _titleUpdater;
        private RoutingState _router;
        private IObservable<string> _viewContractObservable;
        private bool _routerInstigated;

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
                        .Select(_ => new { View = ResolveView(Router.GetCurrentViewModel(), /*contract*/null), Animate = Router.NavigationStack.Count > 1 })
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

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _titleUpdater?.Dispose();
            }

            base.Dispose(disposing);
        }

        private NSViewController ResolveView(IRoutableViewModel viewModel, string contract)
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

            if (!(view is NSViewController viewController))
            {
                throw new Exception($"View type {view.GetType().Name} for view model type {viewModel.GetType().Name} is not a UIViewController");
            }

            return viewController;
        }
    }
}
