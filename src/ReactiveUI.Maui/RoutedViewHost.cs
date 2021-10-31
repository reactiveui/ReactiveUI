// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
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
            var currentlyPopping = false;
            var popToRootPending = false;
            var userInstigated = false;

            this.WhenAnyObservable(x => x.Router.NavigationChanged)
                .Where(_ => Router.NavigationStack.Count == 0)
                .Select(x =>
                {
                    // Xamarin Forms does not let us completely clear down the navigation stack
                    // instead, we have to delay this request momentarily until we receive the new root view
                    // then, we can insert the new root view first, and then pop to it
                    popToRootPending = true;
                    return x;
                })
                .Subscribe()
                .DisposeWith(disposable);

            Router?
                .NavigationChanged?
                .CountChanged()
                .Select(_ => Router.NavigationStack.Count)
                .StartWith(Router.NavigationStack.Count)
                .Buffer(2, 1)
                .Select(counts => new
                {
                    Delta = counts[0] - counts[1],
                    Current = counts[1],

                    // cache current viewmodel as it might change if some other Navigation command is executed midway
                    CurrentViewModel = Router.GetCurrentViewModel()
                })
                .Where(_ => !userInstigated)
                .Where(x => x.Delta > 0)
                .Select(
                        async x =>
                        {
                            // XF doesn't provide a means of navigating back more than one screen at a time apart from navigating right back to the root page
                            // since we want as sensible an animation as possible, we pop to root if that makes sense. Otherwise, we pop each individual
                            // screen until the delta is made up, animating only the last one
                            var popToRoot = x.Current == 1;
                            currentlyPopping = true;

                            try
                            {
                                if (popToRoot)
                                {
                                    await PopToRootAsync(true);
                                }
                                else if (!popToRootPending)
                                {
                                    for (var i = 0; i < x.Delta; ++i)
                                    {
                                        await PopAsync(i == x.Delta - 1);
                                    }
                                }
                            }
                            finally
                            {
                                currentlyPopping = false;
                                if (CurrentPage is IViewFor page && x.CurrentViewModel is not null)
                                {
                                    page.ViewModel = x.CurrentViewModel;
                                }
                            }

                            return Unit.Default;
                        })
                .Concat()
                .Subscribe()
                .DisposeWith(disposable);

            Router?
                .Navigate
                .SelectMany(_ => PageForViewModel(Router.GetCurrentViewModel()))
                .SelectMany(async page =>
                {
                    var animated = true;
                    var attribute = page.GetType().GetCustomAttribute<DisableAnimationAttribute>();
                    if (attribute is not null)
                    {
                        animated = false;
                    }

                    if (popToRootPending && Navigation.NavigationStack.Count > 0)
                    {
                        Navigation.InsertPageBefore(page, Navigation.NavigationStack[0]);
                        await PopToRootAsync(animated);
                    }
                    else
                    {
                        await PushAsync(page, animated);
                    }

                    popToRootPending = false;
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
                .Where(_ => !currentlyPopping && Router is not null)
                .Subscribe(_ =>
                {
                    userInstigated = true;

                    try
                    {
                        Router?.NavigationStack.RemoveAt(Router.NavigationStack.Count - 1);
                    }
                    finally
                    {
                        userInstigated = false;
                    }

                    var vm = Router?.GetCurrentViewModel();
                    if (CurrentPage is IViewFor page && vm is not null)
                    {
                        // don't replace view model if vm is null
                        page.ViewModel = vm;
                    }
                })
                .DisposeWith(disposable);
        });

        var screen = Locator.Current.GetService<IScreen>();
        if (screen is null)
        {
            throw new Exception("You *must* register an IScreen class representing your App's main Screen");
        }

        Router = screen.Router;

        this.WhenAnyValue(x => x.Router)
            .SelectMany(router => router!.NavigationStack
                                         .ToObservable()
                                         .Select(x => (Page)ViewLocator.Current.ResolveView(x)!)
                                         .SelectMany(x => PushAsync(x).ToObservable())
                                         .Finally(() =>
                                         {
                                             var vm = router.GetCurrentViewModel();
                                             if (vm is null)
                                             {
                                                 return;
                                             }

                                             ((IViewFor)CurrentPage).ViewModel = vm;
                                             CurrentPage.Title = vm.UrlPathSegment;
                                         }))
            .Subscribe();
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
    protected IObservable<Page> PageForViewModel(IRoutableViewModel? vm)
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
        pg.Title = vm.UrlPathSegment;

        return Observable.Return(pg);
    }
}