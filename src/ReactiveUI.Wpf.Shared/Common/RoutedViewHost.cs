// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using ReactiveUI.Primitives;
using Splat;

#if HAS_WINUI
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
#elif HAS_UNO
using System.Windows;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#else
using System.Windows;

#endif

#if HAS_UNO
namespace ReactiveUI.Uno
#else

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive;
#else
namespace ReactiveUI;
#endif
#endif

/// <summary>
/// This control hosts the View associated with a Router, and will display
/// the View and wire up the ViewModel whenever a new ViewModel is
/// navigated to. Put this control as the only control in your Window.
/// </summary>
public
#if HAS_UNO
    partial
#endif
    class RoutedViewHost : TransitioningContentControl, IActivatableView, IEnableLogger
{
    /// <summary>The router dependency property.</summary>
    public static readonly DependencyProperty RouterProperty =
        DependencyProperty.Register("Router", typeof(RoutingState), typeof(RoutedViewHost), new(null));

    /// <summary>The default content property.</summary>
    public static readonly DependencyProperty DefaultContentProperty =
        DependencyProperty.Register("DefaultContent", typeof(object), typeof(RoutedViewHost), new(null));

    /// <summary>The view contract observable property.</summary>
    public static readonly DependencyProperty ViewContractObservableProperty =
        DependencyProperty.Register(
            "ViewContractObservable",
            typeof(IObservable<string>),
            typeof(RoutedViewHost),
            new(Signal.Emit<string>(default!)));

    /// <summary>Stores the most recently observed view contract.</summary>
    private string? _viewContract;

    /// <summary>Initializes a new instance of the <see cref="RoutedViewHost"/> class.</summary>
    public RoutedViewHost()
    {
        HorizontalContentAlignment = HorizontalAlignment.Stretch;
        VerticalContentAlignment = VerticalAlignment.Stretch;

        var platform = AppLocator.Current.GetService<IPlatformOperations>();
        Func<string?> platformGetter = () => null;

        if (platform is null)
        {
            // NB: This used to be an error but WPF design mode can't read
            // good or do other stuff good.
            this.Log().Error(
                "Couldn't find an IPlatformOperations implementation. Please make sure you have installed " +
                "the latest version of the ReactiveUI packages for your platform. " +
                "See https://reactiveui.net/docs/getting-started/installation for guidance.");
        }
        else
        {
            platformGetter = platform.GetOrientation;
        }

        ViewContractObservable = ModeDetector.InUnitTestRunner()
            ? Signal.Silent<string?>()
            : new StartWithObservable<string?>(
                new FromEventObservable<string?>(onNext =>
                {
                    void Handler(object sender, SizeChangedEventArgs e) => onNext(platformGetter());
                    SizeChanged += Handler;
                    return new ActionDisposable(() => SizeChanged -= Handler);
                }),
                platformGetter())
                .DistinctUntilChanged();

        IRoutableViewModel? currentViewModel = null;
        var viewModelAndContract = new StartWithObservable<IRoutableViewModel?>(
                this.WhenAnyObservable(x => x.Router.CurrentViewModel).Do(x => currentViewModel = x),
                currentViewModel)
            .CombineLatest(
                new StartWithObservable<string?>(
                    this.WhenAnyObservable(x => x.ViewContractObservable).Do(x => _viewContract = x),
                    ViewContract),
                (viewModel, contract) => (viewModel, contract));

        // NB: The DistinctUntilChanged is useful because most views in
        // WinRT will end up getting here twice - once for configuring
        // the RoutedViewHost's ViewModel, and once on load via SizeChanged
        _ = this.WhenActivated(d =>
            d(viewModelAndContract.DistinctUntilChanged()
                .Subscribe(new DelegateObserver<(IRoutableViewModel? viewModel, string? contract)>(
                    ResolveViewForViewModel,
                    ex => RxState.DefaultExceptionHandler.OnNext(ex)))));
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
        Justification = "Setter intentionally routes through ViewContractObservable rather than the field.")]
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

    /// <summary>Resolves and displays the view for the supplied view model and contract.</summary>
    /// <param name="x">The view model and contract to resolve a view for.</param>
    private void ResolveViewForViewModel((IRoutableViewModel? viewModel, string? contract) x)
    {
        if (x.viewModel is null)
        {
            Content = DefaultContent;
            return;
        }

        var viewLocator = ViewLocator ?? ReactiveUI.ViewLocator.Current;
        var view = (viewLocator.ResolveView(x.viewModel, x.contract) ?? viewLocator.ResolveView(x.viewModel)) ??
                   throw new InvalidOperationException($"Couldn't find view for '{x.viewModel}'.");
        view.ViewModel = x.viewModel;
        Content = view;
    }
}
