// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics;
using System.Runtime.CompilerServices;
using ReactiveUI.Internal;

using NSViewController = UIKit.UIViewController;

#if REACTIVE_SHIM
using static ReactiveUI.Binding.Reactive.ViewLocator;
#else
using static ReactiveUI.Binding.ViewLocator;
#endif

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
/// <see cref="RoutingState.NavigateBack"/>, and <see cref="RoutingState.NavigationStackChanged"/>. Manual calls to
/// <see cref="PushViewController(NSViewController?, bool)"/> and <see cref="PopViewController(bool)"/> also update the
/// router so that imperative navigation cannot desynchronize the stacks.
/// </para>
/// <para>
/// Provide a <see cref="ViewContractObservable"/> when multiple views are registered for the same view model. The host
/// will pass the latest contract to <see cref="ViewLocator"/> so that platform-specific or modal presentations render
/// the correct view controller.
/// </para>
/// <para>
/// The host finds each page's view through the view lookup the source generator writes while the app builds, so it
/// is safe to trim and to compile ahead of time. The lookup covers every view class that implements
/// <see cref="IViewFor{T}"/> in a project the ReactiveUI.Binding source generator runs in. A view the generator
/// cannot see, such as one only registered with the service locator, needs <see cref="RoutedViewHostUnsafe"/>.
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
[DebuggerDisplay("{Router}, {ViewLocator}")]
public class RoutedViewHost : ReactiveNavigationController
{
    /// <summary>The disposable that tracks the current title-update subscription.</summary>
    private readonly SwapDisposable _titleUpdater;

    /// <summary>Asks a view locator for the view of a view model under a contract.</summary>
    private readonly Func<IViewLocator, object, string?, IViewFor?> _resolveView;

    /// <summary>The backing field for the <see cref="Router"/> property.</summary>
    private RoutingState? _router;

    /// <summary>Whether the current navigation event was initiated by the router rather than the user.</summary>
    private bool _routerInstigated;

    /// <summary>The router's stack count after the last change the host mirrored.</summary>
    private int _stackCount;

    /// <summary>Initializes a new instance of the <see cref="RoutedViewHost"/> class.</summary>
    public RoutedViewHost()
        : this(ResolveGeneratedView)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="RoutedViewHost"/> class with its view lookup.</summary>
    /// <param name="resolveView">Asks a view locator for the view of a view model under a contract.</param>
    private protected RoutedViewHost(Func<IViewLocator, object, string?, IViewFor?> resolveView)
    {
        _resolveView = resolveView;
        ViewContractObservable = Signal.Emit<string?>(null);
        _titleUpdater = new();

        _ = this.WhenActivated(
            d =>
            {
                d(SubscribeToInitialStack());

                d(SubscribeToStackChanges());
                d(SubscribeToNavigateBack());
            },
            new ViewModelChangedSignal(this));
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
        var viewModel = (IRoutableViewModel?)((IViewFor)viewController).ViewModel;
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
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static IDisposable SubscribeToTitleUpdates(RoutingState router, NSViewController viewController) =>
        router
            .CurrentViewModel
            .SwitchSubscribe(
                static vm => vm.WhenAnyValue(static x => x.UrlPathSegment),
                title => viewController.NavigationItem.Title = title);

    /// <summary>Finds a view through the view lookup the source generator writes, which needs no reflection.</summary>
    /// <param name="viewLocator">The view locator to ask.</param>
    /// <param name="viewModel">The view model to find a view for.</param>
    /// <param name="contract">The contract to resolve under, or <see langword="null"/> for the default view.</param>
    /// <returns>The view, or <see langword="null"/> when the generated lookup has none.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static IViewFor? ResolveGeneratedView(IViewLocator viewLocator, object viewModel, string? contract) =>
        viewLocator.ResolveView<object>(viewModel, contract);

    /// <summary>Subscribes to the initial router state and pushes any pre-existing view models onto the navigation stack.</summary>
    /// <returns>A disposable that represents the subscription.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private IDisposable SubscribeToInitialStack() =>
        this.WhenAnyValue(static x => x.Router)
            .Subscribe(new DelegateObserver<RoutingState?>(x =>
            {
                // A new router starts a new stack history: later stack changes are compared with this count.
                _stackCount = x?.NavigationStack.Count ?? 0;

                if (x is null || Router is null || x.NavigationStack.Count == 0 || ViewControllers?.Length != 0)
                {
                    return;
                }

                _routerInstigated = true;
                NSViewController? view = null;

                foreach (var viewModel in x.NavigationStack)
                {
                    view = ResolveView(viewModel, null) ?? throw new InvalidOperationException(nameof(view));

                    PushViewController(view, false);
                }

                if (view is not null)
                {
                    _titleUpdater.Disposable = SubscribeToTitleUpdates(Router, view);
                }

                _routerInstigated = false;
            }));

    /// <summary>
    /// Subscribes to <see cref="RoutingState.NavigationStackChanged"/> on the current router. A stack that grew pushes
    /// the view for the new current view model; an emptied stack pops to the root view controller.
    /// </summary>
    /// <returns>A disposable that represents the subscription.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private IDisposable SubscribeToStackChanges() =>
        this.WhenAnyValue(static x => x.Router)
            .SwitchSelect(static router => router.NavigationStackChanged)
            .Subscribe(new DelegateObserver<IReadOnlyList<IRoutableViewModel>>(OnStackChanged));

    /// <summary>Mirrors one change of the router's stack into the navigation controller.</summary>
    /// <param name="stack">The router's stack after the change.</param>
    private void OnStackChanged(IReadOnlyList<IRoutableViewModel> stack)
    {
        var previousCount = _stackCount;
        _stackCount = stack.Count;

        if (stack.Count == 0)
        {
            _routerInstigated = true;
            _ = PopToRootViewController(true);
            _routerInstigated = false;
            return;
        }

        if (stack.Count <= previousCount)
        {
            return;
        }

        var view = ResolveView(stack[stack.Count - 1], null);
        var animate = stack.Count > 1;

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
    }

    /// <summary>Subscribes to the router's <see cref="RoutingState.NavigateBack"/> signal and pops the top view controller.</summary>
    /// <returns>A disposable that represents the subscription.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
    /// <exception cref="InvalidOperationException">Thrown when no view is registered for <paramref name="viewModel"/> under
    /// <paramref name="contract"/>, or when the resolved view is not a <see cref="NSViewController"/> and so cannot be pushed
    /// onto the navigation stack.</exception>
    private NSViewController? ResolveView(IRoutableViewModel? viewModel, string? contract)
    {
        if (viewModel is null)
        {
            return null;
        }

        var view = _resolveView(ViewLocator ?? GetCurrent(), viewModel, contract)
            ?? throw new InvalidOperationException(
                $"Couldn't find a view for view model type {viewModel.GetType().Name}. The generated view lookup finds views that implement "
                + $"IViewFor<T>; use {nameof(RoutedViewHostUnsafe)} to also resolve a view registered only by run-time type.");
        view.ViewModel = viewModel;

        return view is not NSViewController viewController
            ? throw new InvalidOperationException($"View type {view.GetType().Name} for view model type {viewModel.GetType().Name} is not a UIViewController")
            : viewController;
    }

    /// <summary>
    /// Emits the host's view model when a subclass makes the host an <see cref="IViewFor"/>: the current value on
    /// subscription, then the new value on each <see cref="IViewFor.ViewModel"/> change. A host that is not an
    /// <see cref="IViewFor"/> has no view model, so the signal emits nothing.
    /// </summary>
    /// <param name="host">The host whose view model is observed.</param>
    private sealed class ViewModelChangedSignal(RoutedViewHost host) : IObservable<object?>
    {
        /// <inheritdoc/>
        public IDisposable Subscribe(IObserver<object?> observer)
        {
            ArgumentExceptionHelper.ThrowIfNull(observer);

            if (host is not IViewFor view)
            {
                return EmptyDisposable.Instance;
            }

            observer.OnNext(view.ViewModel);

            return new KeepSignal<IReactivePropertyChangedEventArgs<ReactiveNavigationController>>(
                    host.Changed,
                    static e => e.PropertyName == nameof(IViewFor.ViewModel))
                .Subscribe(new DelegateObserver<IReactivePropertyChangedEventArgs<ReactiveNavigationController>>(
                    _ => observer.OnNext(view.ViewModel)));
        }
    }
}
