// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.ComponentModel;
using System.Diagnostics;
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
/// <remarks>
/// The host asks the view locator for each page's view by the view model's run-time type, without building any type
/// at run time. The default locator checks the view lookup the source generator writes, then the views the app added
/// with <c>Map</c>. A view registered only with the service locator needs <see cref="RoutedViewHostUnsafe"/>.
/// </remarks>
[DebuggerDisplay("{Router}, {DefaultContent}")]
public
#if HAS_UNO
    partial
#endif
    class RoutedViewHost : TransitioningContentControl, IActivatableView, IEnableLogger
{
    /// <summary>The router dependency property.</summary>
    public static readonly DependencyProperty RouterProperty =
        DependencyProperty.Register(nameof(Router), typeof(RoutingState), typeof(RoutedViewHost), new(null));

    /// <summary>The default content property.</summary>
    public static readonly DependencyProperty DefaultContentProperty =
        DependencyProperty.Register(nameof(DefaultContent), typeof(object), typeof(RoutedViewHost), new(null));

    /// <summary>The view contract observable property.</summary>
    public static readonly DependencyProperty ViewContractObservableProperty =
        DependencyProperty.Register(
            nameof(ViewContractObservable),
            typeof(IObservable<string>),
            typeof(RoutedViewHost),
            new(Signal.Emit<string>(default!)));

    /// <summary>Asks a view locator for the view of a view model under a contract.</summary>
    private readonly Func<IViewLocator, object, string?, IViewFor?> _resolveView;

    /// <summary>Stores the most recently observed view contract.</summary>
    private string? _viewContract;

    /// <summary>Initializes a new instance of the <see cref="RoutedViewHost"/> class.</summary>
    public RoutedViewHost()
        : this(ViewHostResolution.ResolveViewWithoutReflection)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="RoutedViewHost"/> class with its view lookup.</summary>
    /// <param name="resolveView">Asks a view locator for the view of a view model under a contract.</param>
    private protected RoutedViewHost(Func<IViewLocator, object, string?, IViewFor?> resolveView)
    {
        _resolveView = resolveView;
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

        IRoutableViewModel? currentViewModel = null;
        var viewModelAndContract = new StartWithObservable<IRoutableViewModel?>(
                this.WhenAnyObservable(x => x.Router.CurrentViewModel).Do(x => currentViewModel = x),
                currentViewModel)
            .CombineLatest(
                new StartWithObservable<string?>(
                    this.WhenAnyObservable(x => x.ViewContractObservable).Do(x => _viewContract = x),
                    ViewContract),
                static (viewModel, contract) => (viewModel, contract));

        // NB: The DistinctUntilChanged is useful because most views in
        // WinRT will end up getting here twice - once for configuring
        // the RoutedViewHost's ViewModel, and once on load via SizeChanged
        if (this.GetIsDesignMode())
        {
            return;
        }

        _ = ((IActivatableView)this).WhenActivated(
            d =>
                d(viewModelAndContract.DistinctUntilChanged()
                    .Subscribe(new DelegateObserver<(IRoutableViewModel? ViewModel, string? Contract)>(
                        ResolveViewForViewModel,
                        RxState.DefaultExceptionHandler.OnNext))),
            new ViewModelChangedSignal(this));
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
        set => ViewContractObservable = Signal.Emit(value);
    }

    /// <summary>Gets or sets the view locator.</summary>
    /// <value>
    /// The view locator.
    /// </value>
    public IViewLocator? ViewLocator { get; set; }

    /// <summary>Resolves and displays the view for the supplied view model and contract.</summary>
    /// <param name="x">The view model and contract to resolve a view for.</param>
    /// <exception cref="InvalidOperationException">No view is registered for the routed view model.</exception>
    private void ResolveViewForViewModel((IRoutableViewModel? ViewModel, string? Contract) x)
    {
        if (x.ViewModel is null)
        {
            Content = DefaultContent;
            return;
        }

        var view = ViewHostResolution.ResolveViewWithFallback(_resolveView, ViewLocator, x.ViewModel, x.Contract, contractFallbackByPass: false)
                   ?? throw new InvalidOperationException(
                       $"Couldn't find view for '{x.ViewModel}'. The view locator checked the generated view lookup and its Map registrations; "
                       + $"use {nameof(RoutedViewHostUnsafe)} to also resolve a view registered only with the service locator.");
        view.ViewModel = x.ViewModel;
        Content = view;
    }

    /// <summary>
    /// Emits the host's view model when a subclass makes the host an <see cref="IViewFor"/>: the current value on
    /// subscription, then the new value each time the host raises <see cref="INotifyPropertyChanged.PropertyChanged"/>
    /// for <see cref="IViewFor.ViewModel"/>. A host that is not an <see cref="IViewFor"/> has no view model, so the
    /// signal emits nothing.
    /// </summary>
    /// <param name="host">The host whose view model is observed.</param>
    private sealed class ViewModelChangedSignal(RoutedViewHost host) : IObservable<object?>
    {
        /// <inheritdoc/>
        public IDisposable Subscribe(IObserver<object?> observer)
        {
            ArgumentExceptionHelper.ThrowIfNull(observer);

            if (host is not IViewFor view)
            {
                return EmptyDisposable.Instance;
            }

            observer.OnNext(view.ViewModel);

            if (host is not INotifyPropertyChanged notifier)
            {
                return EmptyDisposable.Instance;
            }

            PropertyChangedEventHandler handler = (_, e) =>
            {
                if (e.PropertyName != nameof(IViewFor.ViewModel))
                {
                    return;
                }

                observer.OnNext(view.ViewModel);
            };

            notifier.PropertyChanged += handler;
            return new ActionDisposable(() => notifier.PropertyChanged -= handler);
        }
    }
}
