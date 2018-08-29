// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
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
                        _currentView.Disposable = disposables;
                    });

            defaultViewChange
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(x => Adopt(this, x));
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
#pragma warning disable 1584, 1711, 1572, 1581, 1580
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="ReactiveUI.Cocoa.ViewModelViewHost"/>
        /// will automatically create Auto Layout constraints tying the sub view to the parent view.
        /// </summary>
        /// <value><c>true</c> if add layout contraints to sub view; otherwise, <c>false</c>.</value>
#pragma warning restore 1584, 1711, 1572, 1581, 1580
        public bool AddAutoLayoutConstraintsToSubView { get; set; }

#pragma warning disable SA1600 // Elements should be documented
        public ViewModelViewHostLegacy(NSView targetView)
        {
            if (targetView == null)
            {
                throw new ArgumentNullException(nameof(targetView));
            }

            NSView viewLastAdded = null;
            ViewContractObservable = Observable<string>.Default;

            var vmAndContract = Observable.CombineLatest(
                this.WhenAny(x => x.ViewModel, x => x.Value),
                this.WhenAnyObservable(x => x.ViewContractObservable),
                (vm, contract) => new { ViewModel = vm, Contract = contract, });

            vmAndContract.Subscribe(x =>
            {
                if (viewLastAdded != null)
                {
                    viewLastAdded.RemoveFromSuperview();
                }

                if (ViewModel == null)
                {
                    if (DefaultContent != null)
                    {
                        targetView.AddSubview(DefaultContent.View);
                    }

                    return;
                }

                // get an instance of the view controller for the supplied VM + Contract
                var viewLocator = ViewLocator ?? ReactiveUI.ViewLocator.Current;
                var viewController = viewLocator.ResolveView(x.ViewModel, x.Contract);

                // if not found, throw
                if (viewController == null)
                {
                    var message = string.Format("Unable to resolve view for \"{0}\"", x.ViewModel.GetType());

                    if (x.Contract != null)
                    {
                        message += string.Format(" and contract \"{0}\"", x.Contract.GetType());
                    }

                    message += ".";
                    throw new Exception(message);
                }

                // set the VM on the controller and stash a copy of the added view
                viewController.ViewModel = x.ViewModel;
                viewLastAdded = ((NSViewController)viewController).View;

                // sanity check, view controllers are expect to have a view
                if (viewLastAdded == null)
                {
                    var message = string.Format("No view associated with view controller {0}.", viewController.GetType());
                    throw new Exception(message);
                }

                if (AddAutoLayoutConstraintsToSubView)
                {
                    // see https://developer.apple.com/library/ios/documentation/userexperience/conceptual/AutolayoutPG/AdoptingAutoLayout/AdoptingAutoLayout.html
                    viewLastAdded.TranslatesAutoresizingMaskIntoConstraints = false;
                }

                targetView.AddSubview(viewLastAdded);

                if (AddAutoLayoutConstraintsToSubView)
                {
                    // Add edge constraints so that subview trails changes in parent
                    AddEdgeConstraint(NSLayoutAttribute.Left, targetView, viewLastAdded);
                    AddEdgeConstraint(NSLayoutAttribute.Right, targetView, viewLastAdded);
                    AddEdgeConstraint(NSLayoutAttribute.Top, targetView, viewLastAdded);
                    AddEdgeConstraint(NSLayoutAttribute.Bottom, targetView, viewLastAdded);
                }
            });
        }

        private void AddEdgeConstraint(NSLayoutAttribute edge, NSView parentView, NSView subView)
        {
            var constraint = NSLayoutConstraint.Create(subView, edge, NSLayoutRelation.Equal, parentView, edge, 1, 0);
            parentView.AddConstraint(constraint);
        }

        private NSViewController _defaultContent;

        public NSViewController DefaultContent
        {
            get => _defaultContent;
            set => this.RaiseAndSetIfChanged(ref _defaultContent, value);
        }

        private IReactiveObject _viewModel;

        public IReactiveObject ViewModel
        {
            get => _viewModel;
            set => this.RaiseAndSetIfChanged(ref _viewModel, value);
        }

        private IObservable<string> _viewContractObservable;

        public IObservable<string> ViewContractObservable
        {
            get => _viewContractObservable;
            set => this.RaiseAndSetIfChanged(ref _viewContractObservable, value);
        }

        public IViewLocator ViewLocator { get; set; }
#pragma warning restore SA1600 // Elements should be documented
    }
}
