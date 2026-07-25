// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if WINUI_TARGET
using System.Diagnostics.CodeAnalysis;
using Microsoft.UI.Xaml;
using ReactiveUI.Internal;
#if REACTIVE_SHIM
using ReactiveUI.Reactive.Maui.Internal;
#else
using ReactiveUI.Maui.Internal;
#endif
using Splat;

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive;
#else
namespace ReactiveUI;
#endif

/// <summary>
/// This control hosts the View associated with a Router, and will display
/// the View and wire up the ViewModel whenever a new ViewModel is
/// navigated to. Put this control as the only control in your Window.
/// This generic version provides AOT-compatibility by using compile-time type information.
/// </summary>
/// <typeparam name="TViewModel">The type of the view model. Must have a public parameterless constructor and implement IRoutableViewModel.</typeparam>
public partial class RoutedViewHost<
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] TViewModel> : TransitioningContentControl, IActivatableView, IEnableLogger
    where TViewModel : class, IRoutableViewModel
{
    /// <summary>The router dependency property.</summary>
    public static readonly DependencyProperty RouterProperty =
        DependencyProperty.Register(nameof(Router), typeof(RoutingState), typeof(RoutedViewHost<TViewModel>), new(null));

    /// <summary>The default content property.</summary>
    public static readonly DependencyProperty DefaultContentProperty =
        DependencyProperty.Register(nameof(DefaultContent), typeof(object), typeof(RoutedViewHost<TViewModel>), new(null));

    /// <summary>The view contract observable property.</summary>
    public static readonly DependencyProperty ViewContractObservableProperty =
        DependencyProperty.Register(nameof(ViewContractObservable), typeof(IObservable<string>), typeof(RoutedViewHost<TViewModel>), new(Signal.Emit<string?>(null)));

    /// <summary>The subscriptions created during construction, disposed together.</summary>
    private readonly MultipleDisposable _subscriptions = [];

    /// <summary>The most recently observed view contract.</summary>
    private string? _viewContract;

    /// <summary>Initializes a new instance of the <see cref="RoutedViewHost{TViewModel}"/> class.</summary>
    [SuppressMessage(
        "Design",
        "SST2403:'this' escapes before construction finishes",
        Justification = "The single-threaded UI control hands 'this' to MauiReactiveHelpers to observe its own dependency-property changes; it is never published to another thread.")]
    public RoutedViewHost()
    {
        HorizontalContentAlignment = HorizontalAlignment.Stretch;
        VerticalContentAlignment = VerticalAlignment.Stretch;

        var platformGetter = ViewContractObservableHelpers.GetPlatformOrientation(this.Log());
        ViewContractObservable = ViewContractObservableHelpers.Create(
            platformGetter,
            new FromEventObservable<string?>(onNext =>
            {
                SizeChangedEventHandler handler = (_, _) => onNext(platformGetter());
                SizeChanged += handler;
                return new ActionDisposable(() => SizeChanged -= handler);
            }));

        MauiReactiveHelpers.SubscribeRoutedViewHost(
            this,
            (nameof(Router), RouterProperty, () => Router),
            (nameof(ViewContractObservable), ViewContractObservableProperty, () => ViewContractObservable),
            () => ViewContract,
            contract => _viewContract = contract,
            ResolveViewForViewModel,
            _subscriptions);
    }

    /// <summary>Gets or sets the <see cref="RoutingState"/> of the view model stack.</summary>
    public RoutingState Router
    {
        get => (RoutingState)GetValue(RouterProperty);
        set => SetValue(RouterProperty, value);
    }

    /// <summary>Gets or sets the view locator.</summary>
    /// <value>
    /// The view locator.
    /// </value>
    public IViewLocator? ViewLocator { get; set; }

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

    /// <summary>
    /// Resolves and displays the view for the given view model and contract.
    /// This method uses the generic ViewLocator.ResolveView{TViewModel} which is AOT-safe.
    /// </summary>
    /// <param name="x">Tuple containing the view model and contract.</param>
    private void ResolveViewForViewModel((IRoutableViewModel? viewModel, string? contract) x)
    {
        if (x.viewModel is null)
        {
            Content = DefaultContent;
            return;
        }

        var viewLocator = ViewLocator ?? ReactiveUI.ViewLocator.Current;

        // Use the generic ResolveView<TViewModel> method - this is AOT-safe!
        var view = viewLocator.ResolveView<TViewModel>(x.contract) ?? viewLocator.ResolveView<TViewModel>()
            ?? throw new InvalidOperationException($"Couldn't find view for '{nameof(TViewModel)}'.");
        view.ViewModel = x.viewModel as TViewModel;
        Content = view;
    }
}
#endif
