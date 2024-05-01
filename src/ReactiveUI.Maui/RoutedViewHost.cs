// Copyright (c) 2024 .NET Foundation and Contributors. All rights reserved.
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
     typeof(RoutedViewHost),
     default(RoutingState));

    /// <summary>
    /// The Set Title on Navigate property.
    /// </summary>
    public static readonly BindableProperty SetTitleOnNavigateProperty = BindableProperty.Create(
     nameof(SetTitleOnNavigate),
     typeof(bool),
     typeof(RoutedViewHost),
     false);

    private string? _action;

    /// <summary>
    /// Initializes a new instance of the <see cref="RoutedViewHost"/> class.
    /// </summary>
    /// <exception cref="Exception">You *must* register an IScreen class representing your App's main Screen.</exception>
    public RoutedViewHost()
    {
        this.WhenActivated(async disposable =>
        {
            var currentlyNavigating = false;

            Observable.FromEventPattern<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(
             x => Router!.NavigationStack.CollectionChanged += x,
             x => Router!.NavigationStack.CollectionChanged -= x)
            .Where(_ => !currentlyNavigating && Router?.NavigationStack.Count == 0)
            .Subscribe(async _ => await SyncNavigationStacksAsync())
            .DisposeWith(disposable);

            Router?
                .NavigateBack
                .Subscribe(async _ =>
                {
                    try
                    {
                        currentlyNavigating = true;
                        await PopAsync();
                    }
                    finally
                    {
                        currentlyNavigating = false;
                    }

                    _action = "NavigatedBack";
                    InvalidateCurrentViewModel();
                    await SyncNavigationStacksAsync();
                })
                .DisposeWith(disposable);

            Router?
                .Navigate
                .Where(_ => Navigation.NavigationStack.Count != Router.NavigationStack.Count)
                .ObserveOn(RxApp.MainThreadScheduler)
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
                        currentlyNavigating = true;
                        await PushAsync(page, animated);
                    }
                    finally
                    {
                        currentlyNavigating = false;
                    }

                    await SyncNavigationStacksAsync();

                    return page;
                })
                .Subscribe()
                .DisposeWith(disposable);

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
                .Where(_ => !currentlyNavigating && Router is not null)
                .Subscribe(_ =>
                {
                    if (Router?.NavigationStack.Count > 0)
                    {
                        Router.NavigationStack.RemoveAt(Router.NavigationStack.Count - 1);
                    }

                    _action = "Popped";
                    InvalidateCurrentViewModel();
                })
                .DisposeWith(disposable);

            var poppingToRootEvent = Observable.FromEvent<EventHandler<NavigationEventArgs>, Unit>(
             eventHandler =>
             {
                 void Handler(object? sender, NavigationEventArgs e) => eventHandler(Unit.Default);
                 return Handler;
             },
             x => PoppedToRoot += x,
             x => PoppedToRoot -= x);

            poppingToRootEvent
                .Where(_ => !currentlyNavigating && Router is not null)
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
                .DisposeWith(disposable);
            await SyncNavigationStacksAsync();
        });

        var screen = Locator.Current.GetService<IScreen>() ?? throw new Exception("You *must* register an IScreen class representing your App's main Screen");
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
    protected virtual IObservable<Page> PagesForViewModel(IRoutableViewModel? vm)
    {
        if (vm is null)
        {
            return Observable.Empty<Page>();
        }

        var ret = ViewLocator.Current.ResolveView(vm);
        if (ret is null)
        {
            var msg = $"Couldn't find a View for ViewModel. You probably need to register an IViewFor<{vm.GetType().Name}>";

            return Observable.Throw<Page>(new Exception(msg));
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
    protected virtual Page PageForViewModel(IRoutableViewModel vm)
    {
        ArgumentNullException.ThrowIfNull(vm);

        var ret = ViewLocator.Current.ResolveView(vm);
        if (ret is null)
        {
            var msg = $"Couldn't find a View for ViewModel. You probably need to register an IViewFor<{vm.GetType().Name}>";

            throw new Exception(msg);
        }

        ret.ViewModel = vm;

        var pg = (Page)ret;

        if (SetTitleOnNavigate)
        {
            RxApp.MainThreadScheduler.Schedule(() => pg.Title = vm.UrlPathSegment);
        }

        return pg;
    }

    /// <summary>
    /// Invalidates current page view model.
    /// </summary>
    protected void InvalidateCurrentViewModel()
    {
        var vm = Router?.GetCurrentViewModel();
        if (CurrentPage is IViewFor page && vm is not null)
        {
            if (page.ViewModel?.GetType() == vm.GetType())
            {
                // don't replace view model if vm is null or an incompatible type.
                page.ViewModel = vm;
            }
            else
            {
                this.Log().Info($"The view type '{page.GetType().FullName}' is not compatible with '{vm.GetType().FullName}' this was called by {_action}, the viewmodel was not invalidated");
            }
        }
    }

    /// <summary>
    /// Syncs page's navigation stack  with <see cref="Router"/>
    /// to affect <see cref="Router"/> manipulations like Add or Clear.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    protected async Task SyncNavigationStacksAsync()
    {
        if (Navigation.NavigationStack.Count != Router.NavigationStack.Count
            || StacksAreDifferent())
        {
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
    }

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
