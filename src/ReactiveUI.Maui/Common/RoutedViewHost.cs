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
/// </summary>
[RequiresUnreferencedCode("This class uses reflection to determine view model types at runtime through ViewLocator, which may be incompatible with trimming.")]
[RequiresDynamicCode("ViewLocator.ResolveView uses reflection which is incompatible with AOT compilation.")]
public partial class RoutedViewHost : TransitioningContentControl, IActivatableView, IEnableLogger
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

    /// <summary>The most recently observed view contract.</summary>
    private string? _viewContract;

    /// <summary>Initializes a new instance of the <see cref="RoutedViewHost"/> class.</summary>
    [SuppressMessage(
        "Design",
        "SST2403:'this' escapes before construction finishes",
        Justification = "The single-threaded UI control hands 'this' to MauiReactiveHelpers to observe its own dependency-property changes; it is never published to another thread.")]
    public RoutedViewHost()
    {
        HorizontalContentAlignment = HorizontalAlignment.Stretch;
        VerticalContentAlignment = VerticalAlignment.Stretch;

        var platform = AppLocator.Current.GetService<IPlatformOperations>();
        Func<string?> platformGetter = static () => default;

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
                        SizeChangedEventHandler handler = (_, _) => onNext(platformGetter());
                        SizeChanged += handler;
                        return new ActionDisposable(() => SizeChanged -= handler);
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
        _ = viewModelAndContract.DistinctUntilChanged()
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

    /// <summary>Resolves and hosts the view for the supplied view model/contract pair.</summary>
    /// <param name="x">The view model and contract to resolve a view for.</param>
    [RequiresUnreferencedCode("This method uses reflection to determine the view model type at runtime, which may be incompatible with trimming.")]
    [RequiresDynamicCode("If some of the generic arguments are annotated (either with DynamicallyAccessedMembersAttribute, " +
        "or generic constraints), trimming can't validate that the requirements of those annotations are met.")]
    private void ResolveViewForViewModel((IRoutableViewModel? viewModel, string? contract) x)
    {
        if (x.viewModel is null)
        {
            Content = DefaultContent;
            return;
        }

        var viewLocator = ViewLocator ?? ReactiveUI.ViewLocator.Current;
        var view = (viewLocator.ResolveView(x.viewModel, x.contract) ?? viewLocator.ResolveView(x.viewModel))
            ?? throw new InvalidOperationException($"Couldn't find view for '{x.viewModel}'.");
        view.ViewModel = x.viewModel;
        Content = view;
    }
}
#endif
