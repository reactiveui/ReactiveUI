// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using ReactiveUI.Internal;

using NSViewController = UIKit.UIViewController;

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive;
#else
namespace ReactiveUI;
#endif
/// <summary>A <see cref="ReactiveNavigationController"/> that observes a <see cref="RoutingState"/> and mirrors its navigation stack into UIKit.</summary>
/// <remarks>
/// <para>
/// Use <see cref="RoutedViewHost"/> inside iOS or Mac Catalyst applications to keep push/pop transitions aligned with
/// <see cref="RoutingState"/> changes. The host resolves views via <see cref="ViewLocator"/> and updates titles using
/// <see cref="IRoutableViewModel.UrlPathSegment"/> so navigation remains consistent across app restarts.
/// </para>
/// <para>
/// Setting <see cref="Router"/> subscribes the host to <see cref="RoutingState.Navigate"/>,
/// <see cref="RoutingState.NavigateBack"/>, and collection change notifications. Manual calls to
/// <see cref="PushViewController(NSViewController?, bool)"/> and <see cref="PopViewController(bool)"/> also update the
/// router so that imperative navigation cannot desynchronize the stacks.
/// </para>
/// <para>
/// Provide a <see cref="ViewContractObservable"/> when multiple views are registered for the same view model. The host
/// will pass the latest contract to <see cref="ViewLocator"/> so that platform-specific or modal presentations render
/// the correct view controller.
/// </para>
/// </remarks>
/// <example>
/// <code language="csharp">
/// <![CDATA[
/// var host = new RoutedViewHost
/// {
///     Router = shell.Router,
///     ViewLocator = locator,
///     ViewContractObservable = shell.WhenAnyValue(x => x.SelectedContract)
/// };
///
/// shell.Router.Navigate.Execute(new DashboardViewModel(shell)).Subscribe();
/// ]]>
/// </code>
/// </example>
[RequiresUnreferencedCode("This method uses reflection to determine the view model type at runtime, which may be incompatible with trimming.")]
[RequiresDynamicCode(
    "If some of the generic arguments are annotated (either with DynamicallyAccessedMembersAttribute, " +
    "or generic constraints), trimming can't validate that the requirements of those annotations are met.")]
public class RoutedViewHost : ReactiveNavigationController
{
    /// <summary>The disposable that tracks the current title-update subscription.</summary>
    private readonly SwapDisposable _titleUpdater;

    /// <summary>The backing field for the <see cref="Router"/> property.</summary>
    private RoutingState? _router;

    /// <summary>Whether the current navigation event was initiated by the router rather than the user.</summary>
    private bool _routerInstigated;

    /// <summary>Initializes a new instance of the <see cref="RoutedViewHost"/> class.</summary>
    public RoutedViewHost()
    {
        ViewContractObservable = Signal.Emit<string?>(null);
        _titleUpdater = new();

        _ = this.WhenActivated(d =>
        {
            d(SubscribeToInitialStack());

            var navigationStackChanged = BuildNavigationStackChangedObservable();

            d(SubscribeToStackAdded(navigationStackChanged));
            d(SubscribeToStackReset(navigationStackChanged));
            d(SubscribeToNavigateBack());
        });
    }

    /// <summary>
    /// Gets or sets the <see cref="RoutingState"/> responsible for driving the navigation stack. Assigning a router wires
    /// the host up to all navigation observables.
    /// </summary>
    public RoutingState? Router
    {
        get => _router;
        set => this.RaiseAndSetIfChanged(ref _router, value);
    }

    /// <summary>Gets or sets the observable contract used when resolving views. When <see langword="null"/>, the default contract is applied.</summary>
    public IObservable<string?>? ViewContractObservable
    {
        get => field;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }

    /// <summary>Gets or sets the <see cref="IViewLocator"/> used to resolve view controllers for view models.</summary>
    public IViewLocator? ViewLocator { get; set; }

    /// <inheritdoc/>
    public override void PushViewController(NSViewController? viewController, bool animated)
    {
        ArgumentExceptionHelper.ThrowIfNull(viewController);

        base.PushViewController(viewController, animated);

        if (_routerInstigated)
        {
            return;
        }

        // A view is being pushed directly against the nav controller rather than via the router, so
        // sync the router state to keep the two stacks aligned. Views that don't implement
        // IViewFor of IRoutableViewModel are silently ignored.
        var view = (IViewFor)viewController;
        var viewModel = (IRoutableViewModel?)view.ViewModel;
        if (viewModel is null)
        {
            return;
        }

        Router?.NavigationStack.Add(viewModel);
    }

    /// <inheritdoc/>
    public override NSViewController PopViewController(bool animated)
    {
        if (!_routerInstigated)
        {
            // user must have clicked Back button in nav controller, so we need to manually sync up the router state
            Router?.NavigationStack.RemoveAt(_router!.NavigationStack.Count - 1);
        }

        return base.PopViewController(animated);
    }

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _titleUpdater.Dispose();
        }

        base.Dispose(disposing);
    }

    /// <summary>Keeps <paramref name="viewController"/>'s navigation title in sync with the view model.</summary>
    /// <param name="router">The routing state providing the current view model.</param>
    /// <param name="viewController">The view controller whose title is updated.</param>
    /// <returns>A disposable that represents the title-update subscription.</returns>
    private static IDisposable SubscribeToTitleUpdates(RoutingState router, NSViewController viewController) =>
        router
            .WhenAnyValue(y => y.GetCurrentViewModel())
            .SwitchSubscribe(
                static vm => vm.WhenAnyValue<IRoutableViewModel, string?>(nameof(IRoutableViewModel.UrlPathSegment)),
                title => viewController.NavigationItem.Title = title);

    /// <summary>Builds the observable that emits collection-change events for the active navigation stack.</summary>
    /// <returns>An observable of collection-changed notifications for the router's navigation stack.</returns>
    private IObservable<CollectionChanged> BuildNavigationStackChangedObservable() =>
        this.WhenAnyValue<RoutedViewHost, RoutingState?>(nameof(Router))
            .SwitchSelect(static router => router.NavigationStack.ObserveCollectionChanges());

    /// <summary>Subscribes to the initial router state and pushes any pre-existing view models onto the navigation stack.</summary>
    /// <returns>A disposable that represents the subscription.</returns>
    private IDisposable SubscribeToInitialStack() =>
        this.WhenAnyValue<RoutedViewHost, RoutingState?>(nameof(Router))
            .Subscribe(new DelegateObserver<RoutingState?>(x =>
            {
                if (x is null || Router is null || x.NavigationStack.Count == 0 || ViewControllers?.Length != 0)
                {
                    return;
                }

                _routerInstigated = true;
                NSViewController? view = null;

                foreach (var viewModel in x.NavigationStack)
                {
                    view = ResolveView(Router.GetCurrentViewModel(), null) ?? throw new InvalidOperationException(nameof(view));

                    PushViewController(view, false);
                }

                if (view is not null)
                {
                    _titleUpdater.Disposable = SubscribeToTitleUpdates(Router, view);
                }

                _routerInstigated = false;
            }));

    /// <summary>Subscribes to stack-add events and pushes the resolved view controller.</summary>
    /// <param name="navigationStackChanged">The observable that emits navigation-stack change events.</param>
    /// <returns>A disposable that represents the subscription.</returns>
    private IDisposable SubscribeToStackAdded(
        IObservable<CollectionChanged> navigationStackChanged) =>
        navigationStackChanged
            .Subscribe(new DelegateObserver<CollectionChanged>(change =>
            {
                if (change.EventArgs.Action != NotifyCollectionChangedAction.Add)
                {
                    return;
                }

                var view = ResolveView(Router?.GetCurrentViewModel(), null);
                var animate = Router?.NavigationStack.Count > 1;

                if (_routerInstigated || Router is null)
                {
                    return;
                }

                if (view is not null)
                {
                    _titleUpdater.Disposable = SubscribeToTitleUpdates(Router, view);
                }

                _routerInstigated = true;

                // Animate must be false for the first view pushed; otherwise iOS calls PushViewController twice.
                PushViewController(view, animate);

                _routerInstigated = false;
            }));

    /// <summary>Subscribes to stack-reset events and pops to the root view controller.</summary>
    /// <param name="navigationStackChanged">The observable that emits navigation-stack change events.</param>
    /// <returns>A disposable that represents the subscription.</returns>
    private IDisposable SubscribeToStackReset(
        IObservable<CollectionChanged> navigationStackChanged) =>
        navigationStackChanged
            .Subscribe(new DelegateObserver<CollectionChanged>(change =>
            {
                if (change.EventArgs.Action != NotifyCollectionChangedAction.Reset)
                {
                    return;
                }

                _routerInstigated = true;
                _ = PopToRootViewController(true);
                _routerInstigated = false;
            }));

    /// <summary>Subscribes to the router's <see cref="RoutingState.NavigateBack"/> signal and pops the top view controller.</summary>
    /// <returns>A disposable that represents the subscription.</returns>
    private IDisposable SubscribeToNavigateBack() =>
        this.WhenAnyObservable(x => x.Router!.NavigateBack!)
            .Subscribe(new DelegateObserver<IRoutableViewModel>(navigateBack =>
            {
                _ = navigateBack;
                _routerInstigated = true;
                _ = PopViewController(true);
                _routerInstigated = false;
            }));

    /// <summary>Resolves and returns the <see cref="NSViewController"/> for <paramref name="viewModel"/>.</summary>
    /// <param name="viewModel">The view model to resolve a view for; returns <see langword="null"/> when <see langword="null"/>.</param>
    /// <param name="contract">An optional contract string passed to the view locator.</param>
    /// <returns>The resolved <see cref="NSViewController"/>, or <see langword="null"/> when <paramref name="viewModel"/> is <see langword="null"/>.</returns>
    private NSViewController? ResolveView(IRoutableViewModel? viewModel, string? contract)
    {
        if (viewModel is null)
        {
            return null;
        }

        var viewLocator = ViewLocator ?? ReactiveUI.ViewLocator.Current;
        var view = viewLocator.ResolveView(viewModel, contract)
            ?? throw new InvalidOperationException($"Couldn't find a view for view model. You probably need to register an IViewFor<{viewModel.GetType().Name}>");
        view.ViewModel = viewModel;

        return view is not NSViewController viewController
            ? throw new InvalidOperationException($"View type {view.GetType().Name} for view model type {viewModel.GetType().Name} is not a UIViewController")
            : viewController;
    }
}
