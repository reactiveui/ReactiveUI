// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.Specialized;
using System.Reflection;
using Microsoft.Maui.Controls;

namespace ReactiveUI.Maui;

/// <summary>
/// This is a <see cref="NavigationPage"/> that serves as a router.
/// </summary>
/// <seealso cref="NavigationPage" />
/// <seealso cref="IActivatableView" />
public class RoutedViewHost : NavigationPage, IActivatableView, IEnableLogger
{
    /// <summary>
    /// The router bindable property.
    /// </summary>
    public static readonly BindableProperty RouterProperty = BindableProperty.Create(
        nameof(Router),
        typeof(RoutingState),
        typeof(RoutedViewHost));

    /// <summary>
    /// The Set Title on Navigate property.
    /// </summary>
    public static readonly BindableProperty SetTitleOnNavigateProperty = BindableProperty.Create(
        nameof(SetTitleOnNavigate),
        typeof(bool),
        typeof(RoutedViewHost),
        false);

    /// <summary>
    /// The subscriptions created by this host.
    /// </summary>
    private readonly CompositeDisposable _subscriptions = [];

    /// <summary>
    /// The name of the last navigation action that occurred.
    /// </summary>
    private string? _action;

    /// <summary>
    /// A value indicating whether a navigation operation is currently in progress.
    /// </summary>
    private bool _currentlyNavigating;

    /// <summary>
    /// Initializes a new instance of the <see cref="RoutedViewHost"/> class.
    /// </summary>
    /// <exception cref="InvalidOperationException">You *must* register an IScreen class representing your App's main Screen.</exception>
    [RequiresUnreferencedCode(
        "This class uses reflection to determine view model types at runtime through ViewLocator, which may be incompatible with trimming.")]
    [RequiresDynamicCode("ViewLocator.ResolveView uses reflection which is incompatible with AOT compilation.")]
    public RoutedViewHost()
    {
        // Subscribe directly without WhenActivated
        SubscribeToNavigationStackChanges();
        SubscribeToNavigateBack();
        SubscribeToNavigate();
        SubscribeToPopped();
        SubscribeToPoppedToRoot();

        // Perform initial sync asynchronously
        _ = Task.Run(async () =>
        {
            try
            {
                await SyncNavigationStacksAsync();
            }
            catch (Exception ex)
            {
                this.Log().Error(ex, "Failed to perform initial navigation stack sync");
            }
        });

        var screen = AppLocator.Current.GetService<IScreen>() ??
                     throw new InvalidOperationException("You *must* register an IScreen class representing your App's main Screen");
        Router = screen.Router;
    }

    /// <summary>
    /// Gets or sets the <see cref="RoutingState"/> of the view model stack.
    /// </summary>
    public RoutingState Router
    {
        get => (RoutingState)GetValue(RouterProperty);
        set => SetValue(RouterProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating whether gets or sets the Set Title of the view model stack.
    /// </summary>
    public bool SetTitleOnNavigate
    {
        get => (bool)GetValue(SetTitleOnNavigateProperty);
        set => SetValue(SetTitleOnNavigateProperty, value);
    }

    /// <summary>
    /// Pages for view model.
    /// </summary>
    /// <param name="vm">The vm.</param>
    /// <returns>An observable of the page associated to a <see cref="IRoutableViewModel"/>.</returns>
    [RequiresUnreferencedCode(
        "This method uses reflection to determine the view model type at runtime, which may be incompatible with trimming.")]
    [RequiresDynamicCode(
        "If some of the generic arguments are annotated (either with DynamicallyAccessedMembersAttribute, or generic constraints), " +
        "trimming can't validate that the requirements of those annotations are met.")]
    protected virtual IObservable<Page> PagesForViewModel(IRoutableViewModel? vm)
    {
        if (vm is null)
        {
            return Observable.Empty<Page>();
        }

        var ret = ViewLocator.Current.ResolveView(vm);
        if (ret is null)
        {
            var msg =
                $"Couldn't find a View for ViewModel. You probably need to register an IViewFor<{vm.GetType().Name}>";

            return Observable.Throw<Page>(new InvalidOperationException(msg));
        }

        ret.ViewModel = vm;

        var pg = (Page)ret;
        if (SetTitleOnNavigate)
        {
            pg.Title = vm.UrlPathSegment;
        }

        return Observable.Return(pg);
    }

    /// <summary>
    /// Page for view model.
    /// </summary>
    /// <param name="vm">The vm.</param>
    /// <returns>An observable of the page associated to a <see cref="IRoutableViewModel"/>.</returns>
    [RequiresUnreferencedCode(
        "This method uses reflection to determine the view model type at runtime, which may be incompatible with trimming.")]
    [RequiresDynamicCode(
        "If some of the generic arguments are annotated (either with DynamicallyAccessedMembersAttribute, or generic constraints), " +
        "trimming can't validate that the requirements of those annotations are met.")]
    protected virtual Page PageForViewModel(IRoutableViewModel vm)
    {
        ArgumentNullException.ThrowIfNull(vm);

        var ret = ViewLocator.Current.ResolveView(vm);
        if (ret is null)
        {
            var msg =
                $"Couldn't find a View for ViewModel. You probably need to register an IViewFor<{vm.GetType().Name}>";

            throw new InvalidOperationException(msg);
        }

        ret.ViewModel = vm;

        var pg = (Page)ret;

        if (SetTitleOnNavigate)
        {
            RxSchedulers.MainThreadScheduler.Schedule(() => pg.Title = vm.UrlPathSegment);
        }

        return pg;
    }

    /// <summary>
    /// Invalidates current page view model.
    /// </summary>
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

    /// <summary>
    /// Syncs page's navigation stack  with <see cref="Router"/>
    /// to affect <see cref="Router"/> manipulations like Add or Clear.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [RequiresUnreferencedCode(
        "This method uses reflection to determine the view model type at runtime, which may be incompatible with trimming.")]
    [RequiresDynamicCode(
        "If some of the generic arguments are annotated (either with DynamicallyAccessedMembersAttribute, or generic constraints), " +
        "trimming can't validate that the requirements of those annotations are met.")]
    protected async Task SyncNavigationStacksAsync()
    {
        if (Navigation.NavigationStack.Count == Router.NavigationStack.Count
            && !StacksAreDifferent())
        {
            return;
        }

        if (Navigation.NavigationStack.Count > 2)
        {
            for (var i = Navigation.NavigationStack.Count - 2; i >= 0; i--)
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

    /// <summary>
    /// Subscribes to <see cref="RoutingState.NavigationStack"/> changes and resyncs when the stack is cleared.
    /// </summary>
    [RequiresUnreferencedCode(
        "This method uses reflection to determine the view model type at runtime, which may be incompatible with trimming.")]
    [RequiresDynamicCode(
        "If some of the generic arguments are annotated (either with DynamicallyAccessedMembersAttribute, or generic constraints), " +
        "trimming can't validate that the requirements of those annotations are met.")]
    private void SubscribeToNavigationStackChanges() =>
        Observable.FromEventPattern<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(
                x => Router.NavigationStack.CollectionChanged += x,
                x => Router.NavigationStack.CollectionChanged -= x)
            .Where(_ => !_currentlyNavigating && Router?.NavigationStack.Count == 0)
            .Subscribe(async _ => await SyncNavigationStacksAsync())
            .DisposeWith(_subscriptions);

    /// <summary>
    /// Subscribes to <see cref="RoutingState.NavigateBack"/> requests and pops the page accordingly.
    /// </summary>
    [RequiresUnreferencedCode(
        "This method uses reflection to determine the view model type at runtime, which may be incompatible with trimming.")]
    [RequiresDynamicCode(
        "If some of the generic arguments are annotated (either with DynamicallyAccessedMembersAttribute, or generic constraints), " +
        "trimming can't validate that the requirements of those annotations are met.")]
    private void SubscribeToNavigateBack() =>
        Router?
            .NavigateBack
            .Subscribe(async _ =>
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
            })
            .DisposeWith(_subscriptions);

    /// <summary>
    /// Subscribes to <see cref="RoutingState.Navigate"/> requests and pushes the resolved page.
    /// </summary>
    [RequiresUnreferencedCode(
        "This method uses reflection to determine the view model type at runtime, which may be incompatible with trimming.")]
    [RequiresDynamicCode(
        "If some of the generic arguments are annotated (either with DynamicallyAccessedMembersAttribute, or generic constraints), " +
        "trimming can't validate that the requirements of those annotations are met.")]
    private void SubscribeToNavigate() =>
        Router?
            .Navigate
            .Where(_ => StacksAreDifferent())
            .ObserveOn(RxSchedulers.MainThreadScheduler)
            .SelectMany(_ => PagesForViewModel(Router.GetCurrentViewModel()))
            .SelectMany(async page =>
            {
                var animated = true;
                var attribute = page.GetType().GetCustomAttribute<DisableAnimationAttribute>();
                if (attribute is not null)
                {
                    animated = false;
                }

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

                return page;
            })
            .Subscribe()
            .DisposeWith(_subscriptions);

    /// <summary>
    /// Subscribes to the <see cref="NavigationPage.Popped"/> event to keep the router stack in sync
    /// when the user navigates back via the application back button.
    /// </summary>
    private void SubscribeToPopped()
    {
        var poppingEvent = Observable.FromEvent<EventHandler<NavigationEventArgs>, Unit>(
            eventHandler =>
            {
                void Handler(object? sender, NavigationEventArgs e) => eventHandler(Unit.Default);
                return Handler;
            },
            x => Popped += x,
            x => Popped -= x);

        // NB: User pressed the Application back as opposed to requesting Back via Router.NavigateBack.
        poppingEvent
            .Where(_ => !_currentlyNavigating && Router is not null)
            .Subscribe(_ =>
            {
                if (Router?.NavigationStack.Count > 0)
                {
                    Router.NavigationStack.RemoveAt(Router.NavigationStack.Count - 1);
                }

                _action = "Popped";
                InvalidateCurrentViewModel();
            })
            .DisposeWith(_subscriptions);
    }

    /// <summary>
    /// Subscribes to the <see cref="NavigationPage.PoppedToRoot"/> event to keep the router stack in sync
    /// when the user pops back to the root page.
    /// </summary>
    private void SubscribeToPoppedToRoot()
    {
        var poppingToRootEvent = Observable.FromEvent<EventHandler<NavigationEventArgs>, Unit>(
            eventHandler =>
            {
                void Handler(object? sender, NavigationEventArgs e) => eventHandler(Unit.Default);
                return Handler;
            },
            x => PoppedToRoot += x,
            x => PoppedToRoot -= x);

        poppingToRootEvent
            .Where(_ => !_currentlyNavigating && Router is not null)
            .Subscribe(_ =>
            {
                for (var i = Router?.NavigationStack.Count - 1; i > 0; i--)
                {
                    if (i.HasValue)
                    {
                        Router?.NavigationStack.RemoveAt(i.Value);
                    }
                }

                _action = "PoppedToRoot";
                InvalidateCurrentViewModel();
            })
            .DisposeWith(_subscriptions);
    }

    /// <summary>
    /// Determines whether the page navigation stack differs from the router navigation stack.
    /// </summary>
    /// <returns><see langword="true"/> if the stacks are different; otherwise, <see langword="false"/>.</returns>
    private bool StacksAreDifferent()
    {
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
