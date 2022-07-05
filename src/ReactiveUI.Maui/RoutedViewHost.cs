// Copyright (c) 2022 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Reflection;
using Microsoft.Maui.Controls;
using Splat;

namespace ReactiveUI.Maui;

/// <summary>
/// This is a <see cref="NavigationPage"/> that serves as a router.
/// </summary>
/// <seealso cref="Microsoft.Maui.Controls.NavigationPage" />
/// <seealso cref="ReactiveUI.IActivatableView" />
[SuppressMessage("Readability", "RCS1090: Call 'ConfigureAwait(false)", Justification = "This class interacts with the UI thread.")]
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
    /// Initializes a new instance of the <see cref="RoutedViewHost"/> class.
    /// </summary>
    /// <exception cref="Exception">You *must* register an IScreen class representing your App's main Screen.</exception>
    public RoutedViewHost()
    {
        this.WhenActivated(disposable =>
        {
            var currentlyNavigating = false;

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

                    InvalidateCurrentViewModel();
                    SyncNavigationStacks();
                })
                .DisposeWith(disposable);

            Router?
                .Navigate
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

                    SyncNavigationStacks();

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

            // NB: Catch when the user hit back as opposed to the application
            // requesting Back via NavigateBack
            poppingEvent
                .Where(_ => !currentlyNavigating && Router is not null)
                .Subscribe(_ =>
                {

                    Router!.NavigationStack.RemoveAt(Router.NavigationStack.Count - 1);

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

            // NB: Catch when the user hit back as opposed to the application
            // requesting Back via NavigateBack
            poppingToRootEvent
                .Where(_ => !currentlyNavigating && Router is not null)
                .Subscribe(_ =>
                {
                    for (var i = Router!.NavigationStack.Count - 1; i > 0; i--)
                    {
                        Router.NavigationStack.RemoveAt(i);
                    }

                    InvalidateCurrentViewModel();
                })
                .DisposeWith(disposable);
        });

        var screen = Locator.Current.GetService<IScreen>();
        if (screen is null)
        {
            throw new Exception("You *must* register an IScreen class representing your App's main Screen");
        }

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
    /// Pages for view model.
    /// </summary>
    /// <param name="vm">The vm.</param>
    /// <returns>An observable of the page associated to a <see cref="IRoutableViewModel"/>.</returns>
    [SuppressMessage("Design", "CA1822: Can be made static", Justification = "Might be used by implementors.")]
    protected virtual IObservable<Page> PagesForViewModel(IRoutableViewModel? vm)
    {
        if (vm is null)
        {
            return Observable<Page>.Empty;
        }

        var ret = ViewLocator.Current.ResolveView(vm);
        if (ret is null)
        {
            var msg = $"Couldn't find a View for ViewModel. You probably need to register an IViewFor<{vm.GetType().Name}>";

            return Observable.Throw<Page>(new Exception(msg));
        }

        ret.ViewModel = vm;

        var pg = (Page)ret;
        // pg.Title = vm.UrlPathSegment;

        return Observable.Return(pg);
    }

    /// <summary>
    /// Page for view model.
    /// </summary>
    /// <param name="vm">The vm.</param>
    /// <returns>An observable of the page associated to a <see cref="IRoutableViewModel"/>.</returns>
    [SuppressMessage("Design", "CA1822: Can be made static", Justification = "Might be used by implementors.")]
    protected virtual Page PageForViewModel(IRoutableViewModel vm)
    {
        if (vm is null)
        {
            throw new ArgumentNullException(nameof(vm));
        }

        var ret = ViewLocator.Current.ResolveView(vm);
        if (ret is null)
        {
            var msg = $"Couldn't find a View for ViewModel. You probably need to register an IViewFor<{vm.GetType().Name}>";

            throw new Exception(msg);
        }

        ret.ViewModel = vm;

        var pg = (Page)ret;
        // pg.Title = vm.UrlPathSegment;

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
            // don't replace view model if vm is null
            page.ViewModel = vm;
        }
    }

    /// <summary>
    /// Syncs page's navigation stack  with <see cref="Router"/>
    /// to affect <see cref="Router"/> manipulations like Add or Clear.
    /// </summary>
    protected void SyncNavigationStacks()
    {
        if (Navigation.NavigationStack.Count != Router.NavigationStack.Count
            || StacksAreDifferent())
        {
            for (var i = Navigation.NavigationStack.Count - 2; i >= 0; i--)
            {
                Navigation.RemovePage(Navigation.NavigationStack[i]);
            }

            var rootPage = Navigation.NavigationStack[0];

            for (var i = 0; i < Router.NavigationStack.Count - 1; i++)
            {
                var page = PageForViewModel(Router.NavigationStack[i]);
                Navigation.InsertPageBefore(page, rootPage);
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
