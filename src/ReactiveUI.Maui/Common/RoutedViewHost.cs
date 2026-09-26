// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if WINUI_TARGET
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Microsoft.UI.Xaml;
#if REACTIVE_SHIM
using ReactiveUI.Reactive.Maui.Internal;
#else
using ReactiveUI.Maui.Internal;
#endif
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

/// <summary>
/// This control hosts the View associated with a Router, and will display
/// the View and wire up the ViewModel whenever a new ViewModel is
/// navigated to. Put this control as the only control in your Window.
/// </summary>
/// <remarks>
/// The host asks the view locator for each page's view by the view model's run-time type, without building any type
/// at run time, so it is safe to trim and to compile ahead of time. The default locator checks the view lookup the
/// source generator writes, then the views the app added with <c>Map</c>. A view registered only with the service
/// locator needs <see cref="RoutedViewHostUnsafe"/>.
/// </remarks>
[DebuggerDisplay("Router = {Router}")]
public partial class RoutedViewHost : TransitioningContentControl, IActivatableView, IMauiRoutedViewHost
{
    /// <summary>The router dependency property.</summary>
    public static readonly DependencyProperty RouterProperty =
        DependencyProperty.Register(nameof(Router), typeof(RoutingState), typeof(RoutedViewHost), new(null));

    /// <summary>The default content property.</summary>
    public static readonly DependencyProperty DefaultContentProperty =
        DependencyProperty.Register(nameof(DefaultContent), typeof(object), typeof(RoutedViewHost), new(null));

    /// <summary>The view contract observable property.</summary>
    public static readonly DependencyProperty ViewContractObservableProperty =
        DependencyProperty.Register(nameof(ViewContractObservable), typeof(IObservable<string>), typeof(RoutedViewHost), new(Signal.Emit<string?>(null)));

    /// <summary>The subscriptions created during construction, disposed together.</summary>
    private readonly MultipleDisposable _subscriptions = [];

    /// <summary>Asks a view locator for the view of a view model under a contract.</summary>
    private readonly Func<IViewLocator, object, string?, IViewFor?> _resolveView;

    /// <summary>The most recently observed view contract.</summary>
    private string? _viewContract;

    /// <summary>Initializes a new instance of the <see cref="RoutedViewHost"/> class.</summary>
    public RoutedViewHost()
        : this(ResolveViewWithoutReflection)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="RoutedViewHost"/> class with its view lookup.</summary>
    /// <param name="resolveView">Asks a view locator for the view of a view model under a contract.</param>
    [SuppressMessage(
        "Design",
        "SST2403:'this' escapes before construction finishes",
        Justification = "The single-threaded UI control hands 'this' to MauiReactiveHelpers to observe its own dependency-property changes; it is never published to another thread.")]
    private protected RoutedViewHost(Func<IViewLocator, object, string?, IViewFor?> resolveView)
    {
        _resolveView = resolveView;
        HorizontalContentAlignment = HorizontalAlignment.Stretch;
        VerticalContentAlignment = VerticalAlignment.Stretch;

        MauiReactiveHelpers.InitializeRoutedViewHost(this, RouterProperty, ViewContractObservableProperty, _subscriptions, ResolveViewForViewModel);
    }

    /// <summary>Gets or sets the <see cref="RoutingState"/> of the view model stack.</summary>
    public RoutingState Router
    {
        get => (RoutingState)GetValue(RouterProperty);
        set => SetValue(RouterProperty, value);
    }

    /// <summary>Gets or sets the content displayed whenever there is no page currently routed.</summary>
    public object DefaultContent
    {
        get => GetValue(DefaultContentProperty);
        set => SetValue(DefaultContentProperty, value);
    }

    /// <summary>Gets or sets the view contract observable.</summary>
    /// <value>
    /// The view contract observable.
    /// </value>
    public IObservable<string?> ViewContractObservable
    {
        get => (IObservable<string?>)GetValue(ViewContractObservableProperty);
        set => SetValue(ViewContractObservableProperty, value);
    }

    /// <summary>Gets or sets the view contract.</summary>
    public string? ViewContract
    {
        get => _viewContract;
        set
        {
            _viewContract = value;
            ViewContractObservable = Signal.Emit(value);
        }
    }

    /// <summary>Gets or sets the view locator.</summary>
    /// <value>
    /// The view locator.
    /// </value>
    public IViewLocator? ViewLocator { get; set; }

    /// <summary>Finds a view by the view model's run-time type without building any type at run time.</summary>
    /// <param name="viewLocator">The view locator to ask.</param>
    /// <param name="viewModel">The view model to find a view for.</param>
    /// <param name="contract">The contract to resolve under, or <see langword="null"/> for the default view.</param>
    /// <returns>The view, or <see langword="null"/> when neither the generated lookup nor a <c>Map</c> registration has one.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static IViewFor? ResolveViewWithoutReflection(IViewLocator viewLocator, object viewModel, string? contract) =>
        viewLocator.ResolveView(viewModel, contract);

    /// <inheritdoc/>
    void IMauiRoutedViewHost.SetObservedViewContract(string? contract) => _viewContract = contract;

    /// <summary>Resolves and hosts the view for the supplied view model/contract pair.</summary>
    /// <param name="route">The view model and contract to resolve a view for.</param>
    /// <exception cref="InvalidOperationException">No view is registered for the routed view model.</exception>
    private void ResolveViewForViewModel((IRoutableViewModel? ViewModel, string? Contract) route)
    {
        if (route.ViewModel is null)
        {
            Content = DefaultContent;
            return;
        }

        var viewLocator = ViewLocator ?? GetCurrent();
        object viewModel = route.ViewModel;
        var view = (_resolveView(viewLocator, viewModel, route.Contract) ?? _resolveView(viewLocator, viewModel, null))
            ?? throw new InvalidOperationException(
                $"Couldn't find view for '{route.ViewModel}'. The view locator checked the generated view lookup and its Map registrations; "
                + $"use {nameof(RoutedViewHostUnsafe)} to also resolve a view registered only with the service locator.");
        view.ViewModel = route.ViewModel;
        Content = view;
    }
}
#endif
