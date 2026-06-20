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
using ReactiveUI.Primitives;
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
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] TViewModel>
    : TransitioningContentControl, IActivatableView, IEnableLogger
    where TViewModel : class, IRoutableViewModel
{
    /// <summary>The router dependency property.</summary>
    public static readonly DependencyProperty RouterProperty =
        DependencyProperty.Register("Router", typeof(RoutingState), typeof(RoutedViewHost<TViewModel>), new(null));

    /// <summary>The default content property.</summary>
    public static readonly DependencyProperty DefaultContentProperty =
        DependencyProperty.Register("DefaultContent", typeof(object), typeof(RoutedViewHost<TViewModel>), new(null));

    /// <summary>The view contract observable property.</summary>
    public static readonly DependencyProperty ViewContractObservableProperty =
        DependencyProperty.Register("ViewContractObservable", typeof(IObservable<string>), typeof(RoutedViewHost<TViewModel>), new(Signal.Emit<string?>(null)));

    /// <summary>The subscriptions created during construction, disposed together.</summary>
    private readonly MultipleDisposable _subscriptions = [];

    /// <summary>The most recently observed view contract.</summary>
    private string? _viewContract;

    /// <summary>Initializes a new instance of the <see cref="RoutedViewHost{TViewModel}"/> class.</summary>
    [SuppressMessage("Major Bug", "S3366:Do not call overridable methods in constructors", Justification = "Wires reactive bindings to this control's own dependency properties during construction.")]
    public RoutedViewHost()
    {
        HorizontalContentAlignment = HorizontalAlignment.Stretch;
        VerticalContentAlignment = VerticalAlignment.Stretch;

        var platform = AppLocator.Current.GetService<IPlatformOperations>();
        Func<string?> platformGetter = () => default;

        if (platform is null)
        {
            // NB: This used to be an error but WPF design mode can't read
            // good or do other stuff good.
            this.Log().Error(
                "Couldn't find an IPlatformOperations implementation. Please make sure you have installed the latest " +
                "version of the ReactiveUI packages for your platform. See https://reactiveui.net/docs/getting-started/installation for guidance.");
        }
        else
        {
            platformGetter = platform.GetOrientation;
        }

        ViewContractObservable = ModeDetector.InUnitTestRunner()
            ? Signal.Silent<string>()

            // Replaces FromEvent(SizeChanged).StartWith(platformGetter()).DistinctUntilChanged().
            : new StartWithObservable<string?>(
                    new FromEventObservable<string?>(onNext =>
                    {
                        void Handler(object sender, SizeChangedEventArgs e) => onNext(platformGetter());
                        SizeChanged += Handler;
                        return new ActionDisposable(() => SizeChanged -= Handler);
                    }),
                    platformGetter())
                .DistinctUntilChanged();

        // Observe Router property changes using DependencyProperty (AOT-friendly)
        var routerChanged = MauiReactiveHelpers.CreatePropertyValueObservable(
            this,
            nameof(Router),
            RouterProperty,
            () => Router);

        // Observe ViewContractObservable property changes using DependencyProperty (AOT-friendly)
        var viewContractObservableChanged = MauiReactiveHelpers.CreatePropertyValueObservable(
            this,
            nameof(ViewContractObservable),
            ViewContractObservableProperty,
            () => ViewContractObservable);

        // Observe current view model from router. Replaces Where(...).SelectMany(r => r.CurrentViewModel).StartWith(null).
        var currentViewModel = new StartWithObservable<IRoutableViewModel?>(
            new KeepSignal<RoutingState?>(routerChanged, static router => router is not null)
                .SelectMany(static router => router!.CurrentViewModel),
            null);

        // Flatten the ViewContractObservable observable-of-observable.
        // Replaces SelectMany(x => x ?? Return(null)).Do(x => _viewContract = x).StartWith(ViewContract).
        var viewContract = new StartWithObservable<string?>(
            viewContractObservableChanged
                .SelectMany(static x => x ?? Signal.Emit<string?>(null))
                .Do(x => _viewContract = x),
            ViewContract);

        var viewModelAndContract = currentViewModel
            .CombineLatest(
                viewContract,
                static (viewModel, contract) => (viewModel, contract));

        // Subscribe directly without WhenActivated
        // NB: The DistinctUntilChanged is useful because most views in
        // WinRT will end up getting here twice - once for configuring
        // the RoutedViewHost's ViewModel, and once on load via SizeChanged
        viewModelAndContract.DistinctUntilChanged()
            .Subscribe(new DelegateObserver<(IRoutableViewModel? viewModel, string? contract)>(
                ResolveViewForViewModel,
                RxState.DefaultExceptionHandler.OnNext))
            .DisposeWith(_subscriptions);
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
    [SuppressMessage(
        "Critical Bug",
        "S4275:Getters and setters should access the expected fields",
        Justification = "Setter routes through ViewContractObservable; _viewContract is updated by its subscription.")]
    public string? ViewContract
    {
        get => _viewContract;
        set => ViewContractObservable = Signal.Emit<string?>(value);
    }

    /// <summary>Gets or sets the view locator.</summary>
    /// <value>
    /// The view locator.
    /// </value>
    public IViewLocator? ViewLocator { get; set; }

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
            ?? throw new InvalidOperationException($"Couldn't find view for '{typeof(TViewModel).Name}'.");
        view.ViewModel = x.viewModel as TViewModel;
        Content = view;
    }
}
#endif
