// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using ReactiveUI.Helpers;

using NSViewController = UIKit.UIViewController;

namespace ReactiveUI;

/// <summary>
/// A <see cref="ReactiveNavigationController"/> that observes a <see cref="RoutingState"/> and mirrors its
/// navigation stack into UIKit.
/// </summary>
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
[SuppressMessage("Design", "CA1010: Implement generic IEnumerable", Justification = "UI Kit exposes IEnumerable")]

[RequiresUnreferencedCode("This method uses reflection to determine the view model type at runtime, which may be incompatible with trimming.")]
[RequiresDynamicCode(
    "If some of the generic arguments are annotated (either with DynamicallyAccessedMembersAttribute, " +
    "or generic constraints), trimming can't validate that the requirements of those annotations are met.")]
public class RoutedViewHost : ReactiveNavigationController
{
    /// <summary>The disposable that tracks the current title-update subscription.</summary>
    private readonly SerialDisposable _titleUpdater;

    /// <summary>The backing field for the <see cref="Router"/> property.</summary>
    private RoutingState? _router;

    /// <summary>The backing field for the <see cref="ViewContractObservable"/> property.</summary>
    private IObservable<string?>? _viewContractObservable;

    /// <summary>Whether the current navigation event was initiated by the router rather than the user.</summary>
    private bool _routerInstigated;

    /// <summary>
    /// Initializes a new instance of the <see cref="RoutedViewHost"/> class.
    /// </summary>
    public RoutedViewHost()
    {
        ViewContractObservable = Observable.Return<string?>(null);
        _titleUpdater = new SerialDisposable();

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

    /// <summary>
    /// Gets or sets the observable contract used when resolving views. When <see langword="null"/>, the default contract
    /// is applied.
    /// </summary>
    public IObservable<string?>? ViewContractObservable
    {
        get => _viewContractObservable;
        set => this.RaiseAndSetIfChanged(ref _viewContractObservable, value);
    }

    /// <summary>
    /// Gets or sets the <see cref="IViewLocator"/> used to translate <see cref="IRoutableViewModel"/> instances into
    /// UIKit view controllers. Defaults to <see cref="ViewLocator.Current"/>.
    /// </summary>
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

        // A view is being pushed directly against the nav controller rather than via the router;
        // sync the router state so the two stacks stay aligned. Views that don't implement
        // IViewFor<IRoutableViewModel> are silently ignored.
        var view = (IViewFor)viewController;
        var viewModel = (IRoutableViewModel?)view.ViewModel;
        if (viewModel is not null)
        {
            Router?.NavigationStack.Add(viewModel);
        }
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

    /// <summary>Builds the observable that emits collection-change events for the active navigation stack.</summary>
    /// <returns>An observable of collection-changed event patterns for the router's navigation stack.</returns>
    private IObservable<System.Reactive.EventPattern<NotifyCollectionChangedEventArgs>> BuildNavigationStackChangedObservable() =>
        this.WhenAnyValue<RoutedViewHost, RoutingState?>(nameof(Router))
            .Where(x => x is not null)
            .Select(x => x!.NavigationStack.ObserveCollectionChanges())
            .Switch();

    /// <summary>
    /// Subscribes to the initial router state and pushes any pre-existing view models onto the navigation stack.
    /// </summary>
    /// <returns>A disposable that represents the subscription.</returns>
    private IDisposable SubscribeToInitialStack() =>
        this.WhenAnyValue<RoutedViewHost, RoutingState?>(nameof(Router))
            .Where(x => x?.NavigationStack.Count > 0 && ViewControllers?.Length == 0)
            .Subscribe(x =>
            {
                _routerInstigated = true;
                NSViewController? view = null;

                if (Router is not null && x is not null)
                {
                    foreach (var viewModel in x.NavigationStack)
                    {
                        view = ResolveView(Router.GetCurrentViewModel(), null);
                        if (view is null)
                        {
                            throw new InvalidOperationException(nameof(view));
                        }

                        PushViewController(view, false);
                    }

                    if (view is not null)
                    {
                        _titleUpdater.Disposable = SubscribeToTitleUpdates(Router, view);
                    }
                }

                _routerInstigated = false;
            });

    /// <summary>Subscribes to stack-add events and pushes the resolved view controller.</summary>
    /// <param name="navigationStackChanged">The observable that emits navigation-stack change events.</param>
    /// <returns>A disposable that represents the subscription.</returns>
    private IDisposable SubscribeToStackAdded(
        IObservable<System.Reactive.EventPattern<NotifyCollectionChangedEventArgs>> navigationStackChanged) =>
        navigationStackChanged
            .Where(x => x.EventArgs.Action == NotifyCollectionChangedAction.Add)
            .Select(_ => new { View = ResolveView(Router?.GetCurrentViewModel(), null), Animate = Router?.NavigationStack.Count > 1 })
            .Subscribe(x =>
            {
                if (_routerInstigated || Router is null)
                {
                    return;
                }

                if (x?.View is not null)
                {
                    _titleUpdater.Disposable = SubscribeToTitleUpdates(Router, x.View);
                }

                _routerInstigated = true;

                // Animate must be false for the first view pushed; otherwise iOS calls PushViewController twice.
                PushViewController(x?.View, x?.Animate ?? false);

                _routerInstigated = false;
            });

    /// <summary>Subscribes to stack-reset events and pops to the root view controller.</summary>
    /// <param name="navigationStackChanged">The observable that emits navigation-stack change events.</param>
    /// <returns>A disposable that represents the subscription.</returns>
    private IDisposable SubscribeToStackReset(
        IObservable<System.Reactive.EventPattern<NotifyCollectionChangedEventArgs>> navigationStackChanged) =>
        navigationStackChanged
            .Where(x => x.EventArgs.Action == NotifyCollectionChangedAction.Reset)
            .Subscribe(_ =>
            {
                _routerInstigated = true;
                PopToRootViewController(true);
                _routerInstigated = false;
            });

    /// <summary>Subscribes to the router's <see cref="RoutingState.NavigateBack"/> signal and pops the top view controller.</summary>
    /// <returns>A disposable that represents the subscription.</returns>
    private IDisposable SubscribeToNavigateBack() =>
        this.WhenAnyObservable(x => x.Router!.NavigateBack!)
            .Subscribe(_ =>
            {
                _routerInstigated = true;
                PopViewController(true);
                _routerInstigated = false;
            });

    /// <summary>
    /// Creates a subscription that keeps <paramref name="viewController"/>'s navigation-item title in sync with the
    /// current view model's <see cref="IRoutableViewModel.UrlPathSegment"/>.
    /// </summary>
    /// <param name="router">The routing state providing the current view model.</param>
    /// <param name="viewController">The view controller whose title is updated.</param>
    /// <returns>A disposable that represents the title-update subscription.</returns>
    private static IDisposable SubscribeToTitleUpdates(RoutingState router, NSViewController viewController) =>
        router
            .WhenAnyValue(y => y.GetCurrentViewModel())
            .WhereNotNull()
            .Select(vm => vm.WhenAnyValue<IRoutableViewModel, string?>(nameof(vm.UrlPathSegment)))
            .Switch()
            .Subscribe(title => viewController.NavigationItem.Title = title);

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
