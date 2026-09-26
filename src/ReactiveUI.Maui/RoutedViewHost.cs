// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.Maui.Controls;
using ReactiveUI.Internal;
using ReactiveUI.Primitives;
using Splat;

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive.Maui;
#else
namespace ReactiveUI.Maui;
#endif

/// <summary>This is a <see cref="NavigationPage"/> that serves as a router.</summary>
/// <remarks>
/// The host asks the view locator for each page by the view model's run-time type, without building any type at run
/// time, so it is safe to trim and to compile ahead of time. The default locator checks the view lookup the source
/// generator writes, then the views the app added with <c>Map</c>. A page registered only with the service locator
/// needs <see cref="RoutedViewHostUnsafe"/>.
/// </remarks>
/// <seealso cref="NavigationPage" />
/// <seealso cref="IActivatableView" />
[DebuggerDisplay("{Router}, {SetTitleOnNavigate}")]
public class RoutedViewHost : NavigationPage, IActivatableView, IEnableLogger
{
    /// <summary>The router bindable property.</summary>
    public static readonly BindableProperty RouterProperty = BindableProperty.Create(
        nameof(Router),
        typeof(RoutingState),
        typeof(RoutedViewHost));

    /// <summary>The Set Title on Navigate property.</summary>
    public static readonly BindableProperty SetTitleOnNavigateProperty = BindableProperty.Create(
        nameof(SetTitleOnNavigate),
        typeof(bool),
        typeof(RoutedViewHost),
        false);

    /// <summary>The number of trailing navigation pages the stack reconciliation keeps in place before pruning the pages beneath them.</summary>
    private const int TrailingPageCount = 2;

    /// <summary>The subscriptions created by this host.</summary>
    private readonly MultipleDisposable _subscriptions = [];

    /// <summary>Asks a view locator for the view of a view model under a contract.</summary>
    private readonly Func<IViewLocator, object, string?, IViewFor?> _resolveView;

    /// <summary>The name of the last navigation action that occurred.</summary>
    private string? _action;

    /// <summary>A value indicating whether a navigation operation is currently in progress.</summary>
    private bool _currentlyNavigating;

    /// <summary>Initializes a new instance of the <see cref="RoutedViewHost"/> class.</summary>
    /// <exception cref="InvalidOperationException">You *must* register an IScreen class representing your App's main Screen.</exception>
    public RoutedViewHost()
        : this(ViewHostResolution.ResolveViewWithoutReflection)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="RoutedViewHost"/> class with its view lookup.</summary>
    /// <param name="resolveView">Asks a view locator for the view of a view model under a contract.</param>
    /// <exception cref="InvalidOperationException">You *must* register an IScreen class representing your App's main Screen.</exception>
    [SuppressMessage(
        "Design",
        "SST2403:'this' escapes before construction finishes",
        Justification = "'this' is passed to the main-thread scheduler to marshal the initial navigation-stack sync onto the UI thread; the scheduled work runs after construction completes.")]
    private protected RoutedViewHost(Func<IViewLocator, object, string?, IViewFor?> resolveView)
    {
        _resolveView = resolveView;

        // Resolve the Router before wiring the subscriptions: SubscribeToNavigationStackChanges subscribes to
        // Router.NavigationStackChanged, so Router must already be set or it would dereference null.
        var screen = AppLocator.Current.GetService<IScreen>()
                     ?? throw new InvalidOperationException("You *must* register an IScreen class representing your App's main Screen");
        Router = screen.Router;

        // Subscribe directly without WhenActivated
        SubscribeToNavigationStackChanges();
        SubscribeToNavigateBack();
        SubscribeToNavigate();
        SubscribeToPopped();
        SubscribeToPoppedToRoot();

        // Perform the initial navigation-stack sync on the main thread rather than a thread-pool thread. MAUI
        // navigation state must only be mutated from the UI thread, and marshalling here serialises the initial
        // sync with any subsequent navigation instead of racing it on a background thread (the previous Task.Run
        // could interleave its own PushAsync with a caller's push and leave CurrentPage pointing at the wrong page).
        _ = RxSchedulers.MainThreadScheduler.Schedule(this, static (scheduler, state) =>
        {
            _ = state.PerformInitialNavigationSyncAsync();
            return EmptyDisposable.Instance;
        });
    }

    /// <summary>Gets or sets the <see cref="RoutingState"/> of the view model stack.</summary>
    public RoutingState Router
    {
        get => (RoutingState)GetValue(RouterProperty);
        set => SetValue(RouterProperty, value);
    }

    /// <summary>Gets or sets a value indicating whether gets or sets the Set Title of the view model stack.</summary>
    public bool SetTitleOnNavigate
    {
        get => (bool)GetValue(SetTitleOnNavigateProperty);
        set => SetValue(SetTitleOnNavigateProperty, value);
    }

    /// <summary>Pages for view model.</summary>
    /// <param name="vm">The vm.</param>
    /// <returns>An observable of the page associated to a <see cref="IRoutableViewModel"/>.</returns>
    protected virtual IObservable<Page> PagesForViewModel(IRoutableViewModel? vm)
    {
        if (vm is null)
        {
            return Signal.None<Page>();
        }

        var ret = _resolveView(ViewLocator.GetCurrent(), vm, null);
        if (ret is null)
        {
            return Signal.Fail<Page>(new InvalidOperationException(NoViewMessage(vm)));
        }

        ret.ViewModel = vm;

        var pg = (Page)ret;
        if (SetTitleOnNavigate)
        {
            pg.Title = vm.UrlPathSegment;
        }

        return Signal.Emit(pg);
    }

    /// <summary>Page for view model.</summary>
    /// <param name="vm">The vm.</param>
    /// <returns>An observable of the page associated to a <see cref="IRoutableViewModel"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="vm"/> is null.</exception>
    /// <exception cref="InvalidOperationException">No <c>IViewFor</c> is registered for the view model.</exception>
    protected virtual Page PageForViewModel(IRoutableViewModel vm)
    {
        ArgumentNullException.ThrowIfNull(vm);

        var ret = _resolveView(ViewLocator.GetCurrent(), vm, null) ?? throw new InvalidOperationException(NoViewMessage(vm));
        ret.ViewModel = vm;

        var pg = (Page)ret;

        if (SetTitleOnNavigate)
        {
            _ = RxSchedulers.MainThreadScheduler.Schedule((Page: pg, vm), static (_, state) =>
            {
                state.Page.Title = state.vm.UrlPathSegment;
                return EmptyDisposable.Instance;
            });
        }

        return pg;
    }

    /// <summary>Invalidates current page view model.</summary>
    protected void InvalidateCurrentViewModel()
    {
        var vm = Router?.GetCurrentViewModel();
        if (CurrentPage is not IViewFor page || vm is null)
        {
            return;
        }

        if (page.ViewModel?.GetType() == vm.GetType())
        {
            // don't replace view model if vm is null or an incompatible type.
            page.ViewModel = vm;
        }
        else
        {
            this.Log().Info(
                $"The view type '{page.GetType().FullName}' is not compatible with '{vm.GetType().FullName}' this was called by {_action}, the viewmodel was not invalidated");
        }
    }

    /// <summary>Syncs page's navigation stack with <see cref="Router"/> to affect <see cref="Router"/> manipulations like Add or Clear.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    protected async Task SyncNavigationStacksAsync()
    {
        if (Navigation.NavigationStack.Count == Router.NavigationStack.Count
            && !StacksAreDifferent())
        {
            return;
        }

        if (Navigation.NavigationStack.Count > TrailingPageCount)
        {
            for (var i = Navigation.NavigationStack.Count - TrailingPageCount; i >= 0; i--)
            {
                Navigation.RemovePage(Navigation.NavigationStack[i]);
            }
        }

        Page? rootPage;
        if (Navigation.NavigationStack.Count >= 1)
        {
            rootPage = Navigation.NavigationStack[0];
        }
        else
        {
            rootPage = PageForViewModel(Router.NavigationStack[0]);
            await Navigation.PushAsync(rootPage, false);
        }

        if (Router.NavigationStack.Count >= 1)
        {
            for (var i = 0; i < Router.NavigationStack.Count - 1; i++)
            {
                var page = PageForViewModel(Router.NavigationStack[i]);
                Navigation.InsertPageBefore(page, rootPage);
            }
        }
    }

    /// <summary>Builds the message for a view model the view locator has no page for.</summary>
    /// <param name="vm">The view model with no page.</param>
    /// <returns>The message.</returns>
    private static string NoViewMessage(IRoutableViewModel vm) =>
        $"Couldn't find a View for ViewModel '{vm.GetType().Name}'. The view locator checked the generated view lookup and its Map registrations; "
        + $"use {nameof(RoutedViewHostUnsafe)} to also resolve an IViewFor<{vm.GetType().Name}> registered only with the service locator.";

    /// <summary>Performs the one-time initial navigation-stack sync, logging any failure instead of faulting an unobserved task.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    private async Task PerformInitialNavigationSyncAsync()
    {
        try
        {
            await SyncNavigationStacksAsync();
        }
        catch (Exception ex)
        {
            this.Log().Error(ex, "Failed to perform initial navigation stack sync");
        }
    }

    /// <summary>Subscribes to <see cref="RoutingState.NavigationStackChanged"/> and resyncs when the stack is cleared.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void SubscribeToNavigationStackChanges() =>
        Router.NavigationStackChanged
            .Subscribe(new DelegateObserver<IReadOnlyList<IRoutableViewModel>>(stack =>
            {
                if (_currentlyNavigating || stack.Count != 0)
                {
                    return;
                }

                _ = SyncNavigationStacksAsync();
            }))
            .DisposeWith(_subscriptions);

    /// <summary>Subscribes to <see cref="RoutingState.NavigateBack"/> requests and pops the page accordingly.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void SubscribeToNavigateBack() =>
        Router?
            .NavigateBack
            .Subscribe(new DelegateObserver<IRoutableViewModel>(vm => _ = OnNavigateBackAsync()))
            .DisposeWith(_subscriptions);

    /// <summary>Pops the page for a back navigation request and resyncs the stacks.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    private async Task OnNavigateBackAsync()
    {
        try
        {
            _currentlyNavigating = true;
            await PopAsync();
        }
        finally
        {
            _currentlyNavigating = false;
        }

        _action = "NavigatedBack";
        InvalidateCurrentViewModel();
        await SyncNavigationStacksAsync();
    }

    /// <summary>Subscribes to <see cref="RoutingState.Navigate"/> requests and pushes the resolved page.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void SubscribeToNavigate() =>
        Router?
            .Navigate
            .Subscribe(new DelegateObserver<IRoutableViewModel>(_ => OnNavigateRequested()))
            .DisposeWith(_subscriptions);

    /// <summary>
    /// Handles a navigate request: skips when the stacks already match and otherwise marshals the push onto the main
    /// thread. Replaces the <c>Where(...).ObserveOn(...).SelectMany(...).SelectMany(async ...)</c> pipeline.
    /// </summary>
    private void OnNavigateRequested()
    {
        if (!StacksAreDifferent())
        {
            return;
        }

        _ = RxSchedulers.MainThreadScheduler.Schedule(this, static (scheduler, state) =>
        {
            _ = state.OnNavigateAsync();
            return EmptyDisposable.Instance;
        });
    }

    /// <summary>Resolves the page for the current view model. PagesForViewModel emits a single page synchronously
    /// (or signals an error), so the subscription resolves it inline.</summary>
    /// <returns>The resolved page, or <see langword="null"/> if none could be resolved.</returns>
    private Page? ResolveCurrentPage()
    {
        Page? page = null;
        PagesForViewModel(Router.GetCurrentViewModel())
            .Subscribe(new DelegateObserver<Page>(
                p => page = p,
                e => this.Log().Error(e, "Failed to resolve the page for navigation")))
            .Dispose();

        return page;
    }

    /// <summary>Resolves the page for the current view model and pushes it, then resyncs the stacks.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    private async Task OnNavigateAsync()
    {
        var page = ResolveCurrentPage();

        if (page is null)
        {
            return;
        }

        var animated = page.GetType().GetCustomAttribute<DisableAnimationAttribute>() is null;

        try
        {
            _currentlyNavigating = true;
            await PushAsync(page, animated);
        }
        finally
        {
            _currentlyNavigating = false;
        }

        await SyncNavigationStacksAsync();
    }

    /// <summary>Subscribes to the <see cref="NavigationPage.Popped"/> event to keep the router stack in sync when the user navigates back via the application back button.</summary>
    private void SubscribeToPopped()
    {
        var poppingEvent = new FromEventObservable<RxVoid>(onNext =>
        {
            EventHandler<NavigationEventArgs> handler = (_, _) => onNext(RxVoid.Default);
            Popped += handler;
            return new ActionDisposable(() => Popped -= handler);
        });

        // NB: User pressed the Application back as opposed to requesting Back via Router.NavigateBack.
        _ = poppingEvent
            .Subscribe(new DelegateObserver<RxVoid>(_ =>
            {
                // Replaces .Where(_ => !_currentlyNavigating && Router is not null).
                if (_currentlyNavigating || Router is null)
                {
                    return;
                }

                if (Router.NavigationStack.Count > 0)
                {
                    Router.NavigationStack.RemoveAt(Router.NavigationStack.Count - 1);
                }

                _action = "Popped";
                InvalidateCurrentViewModel();
            }))
            .DisposeWith(_subscriptions);
    }

    /// <summary>Subscribes to the <see cref="NavigationPage.PoppedToRoot"/> event to keep the router stack in sync when the user pops back to the root page.</summary>
    private void SubscribeToPoppedToRoot()
    {
        var poppingToRootEvent = new FromEventObservable<RxVoid>(onNext =>
        {
            EventHandler<NavigationEventArgs> handler = (_, _) => onNext(RxVoid.Default);
            PoppedToRoot += handler;
            return new ActionDisposable(() => PoppedToRoot -= handler);
        });

        _ = poppingToRootEvent
            .Subscribe(new DelegateObserver<RxVoid>(_ =>
            {
                // Replaces .Where(_ => !_currentlyNavigating && Router is not null).
                if (_currentlyNavigating || Router is null)
                {
                    return;
                }

                for (var i = Router.NavigationStack.Count - 1; i > 0; i--)
                {
                    Router.NavigationStack.RemoveAt(i);
                }

                _action = "PoppedToRoot";
                InvalidateCurrentViewModel();
            }))
            .DisposeWith(_subscriptions);
    }

    /// <summary>Determines whether the page navigation stack differs from the router navigation stack.</summary>
    /// <returns><see langword="true"/> if the stacks are different; otherwise, <see langword="false"/>.</returns>
    private bool StacksAreDifferent()
    {
        // Stacks of different lengths differ; checking first also keeps the loop below inside the page stack.
        if (Navigation.NavigationStack.Count != Router.NavigationStack.Count)
        {
            return true;
        }

        for (var i = 0; i < Router.NavigationStack.Count; i++)
        {
            var vm = Router.NavigationStack[i];
            var page = Navigation.NavigationStack[i];

            if (page is not IViewFor view || !ReferenceEquals(view.ViewModel, vm))
            {
                return true;
            }
        }

        return false;
    }
}
