﻿// Copyright (c) 2019 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Reactive.Disposables;
using System.Reactive.Linq;

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
    /// A control which will use Splat dependency injection to determine the View
    /// to show. It uses.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleType", Justification = "Classes with the same class names within.")]
    [SuppressMessage("Design", "CA1010: Implement generic IEnumerable", Justification = "UI Kit exposes IEnumerable")]
    public class ViewModelViewHost : ReactiveViewController
    {
        private readonly SerialDisposable _currentView;
        private readonly ObservableAsPropertyHelper<string> _viewContract;
        private IViewLocator _viewLocator;
        private NSViewController _defaultContent;
        private IReactiveObject _viewModel;
        private IObservable<string> _viewContractObservable;

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModelViewHost"/> class.
        /// </summary>
        public ViewModelViewHost()
        {
            _currentView = new SerialDisposable();
            _viewContract = this
                .WhenAnyObservable(x => x.ViewContractObservable)
                .ToProperty(this, x => x.ViewContract, scheduler: RxApp.MainThreadScheduler);

            Initialize();
        }

        /// <summary>
        /// Gets or sets the view locator.
        /// </summary>
        /// <value>
        /// The view locator.
        /// </value>
        public IViewLocator ViewLocator
        {
            get => _viewLocator;
            set => this.RaiseAndSetIfChanged(ref _viewLocator, value);
        }

        /// <summary>
        /// Gets or sets the default content.
        /// </summary>
        /// <value>
        /// The default content.
        /// </value>
        public NSViewController DefaultContent
        {
            get => _defaultContent;
            set => this.RaiseAndSetIfChanged(ref _defaultContent, value);
        }

        /// <summary>
        /// Gets or sets the view model.
        /// </summary>
        public IReactiveObject ViewModel
        {
            get => _viewModel;
            set => this.RaiseAndSetIfChanged(ref _viewModel, value);
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
        /// Gets or sets the view contract.
        /// </summary>
        public string ViewContract
        {
            get => _viewContract.Value;
            set => ViewContractObservable = Observable.Return(value);
        }

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                _currentView.Dispose();
                _viewContract.Dispose();
            }
        }

        private static void Adopt(NSViewController parent, NSViewController child)
        {
            // ensure the child view fills our entire frame
            child.View.Frame = parent.View.Bounds;
#if UIKIT
            child.View.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
#else
            child.View.AutoresizingMask = NSViewResizingMask.WidthSizable | NSViewResizingMask.HeightSizable;
#endif
            child.View.TranslatesAutoresizingMaskIntoConstraints = true;

            parent.AddChildViewController(child);

#if UIKIT
            var parentAlreadyVisible = parent.IsViewLoaded && parent.View.Window != null;

            if (parentAlreadyVisible)
            {
                child.BeginAppearanceTransition(true, false);
            }
#endif

            parent.View.AddSubview(child.View);

#if UIKIT
            if (parentAlreadyVisible)
            {
                child.EndAppearanceTransition();
            }

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
                            var message = $"Unable to resolve view for \"{x.ViewModel.GetType()}\"";

                            if (x.Contract != null)
                            {
                                message += $" and contract \"{x.Contract.GetType()}\"";
                            }

                            message += ".";
                            throw new Exception(message);
                        }

                        var viewController = view as NSViewController;

                        if (viewController == null)
                        {
                            throw new Exception($"Resolved view type '{viewController.GetType().FullName}' is not a '{typeof(NSViewController).FullName}'.");
                        }

                        view.ViewModel = x.ViewModel;
                        Adopt(this, viewController);

                        var disposables = new CompositeDisposable();
                        disposables.Add(viewController);
                        disposables.Add(Disposable.Create(() => Disown(viewController)));
                        _currentView.Disposable = disposables;
                    });

            defaultViewChange
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(x => Adopt(this, x));
        }
    }
}
