// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics;

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
using ReactiveUI.Primitives;
using Splat;

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
/// This content control will automatically load the View associated with
/// the ViewModel property and display it. This control is very useful
/// inside a DataTemplate to display the View associated with a ViewModel.
/// </summary>
/// <remarks>
/// The host asks the view locator for the view by the view model's run-time type, without building any type at run
/// time. The default locator checks the view lookup the source generator writes, then the views the app added with
/// <c>Map</c>. A view registered only with the service locator needs <see cref="ViewModelViewHostUnsafe"/>, which also
/// asks the service locator for <see cref="IViewFor{T}"/> closed over the view model's run-time type.
/// </remarks>
[DebuggerDisplay("{ViewContractObservable}, {DefaultContent}")]
public
#if HAS_UNO
    partial
#endif
    class ViewModelViewHost : TransitioningContentControl, IViewFor, IEnableLogger
{
    /// <summary>The default content dependency property.</summary>
    public static readonly DependencyProperty DefaultContentProperty =
        DependencyProperty.Register(nameof(DefaultContent), typeof(object), typeof(ViewModelViewHost), new(null));

    /// <summary>The view model dependency property.</summary>
    public static readonly DependencyProperty ViewModelProperty =
        DependencyProperty.Register(nameof(ViewModel), typeof(object), typeof(ViewModelViewHost), new(null));

    /// <summary>The view contract observable dependency property.</summary>
    public static readonly DependencyProperty ViewContractObservableProperty =
        DependencyProperty.Register(
            nameof(ViewContractObservable),
            typeof(IObservable<string>),
            typeof(ViewModelViewHost),
            new(Signal.Emit<string>(default!)));

    /// <summary>The ContractFallbackByPass dependency property.</summary>
    public static readonly DependencyProperty ContractFallbackByPassProperty =
        DependencyProperty.Register(nameof(ContractFallbackByPass), typeof(bool), typeof(ViewModelViewHost), new(false));

    /// <summary>Asks a view locator for the view of a view model under a contract.</summary>
    private readonly Func<IViewLocator, object, string?, IViewFor?> _resolveView;

    /// <summary>Stores the most recently observed view contract.</summary>
    private string? _viewContract;

    /// <summary>Initializes a new instance of the <see cref="ViewModelViewHost"/> class.</summary>
    public ViewModelViewHost()
        : this(ViewHostResolution.ResolveViewWithoutReflection)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="ViewModelViewHost"/> class with its view lookup.</summary>
    /// <param name="resolveView">Asks a view locator for the view of a view model under a contract.</param>
    private protected ViewModelViewHost(Func<IViewLocator, object, string?, IViewFor?> resolveView)
    {
        _resolveView = resolveView;
        var platformGetter = ViewContractObservableHelpers.GetPlatformOrientation(this.Log());
        ViewContractObservable = ViewContractObservableHelpers.Create(
            platformGetter,
            new FromEventObservable<string?>(onNext =>
            {
                SizeChangedEventHandler handler = (_, _) => onNext(platformGetter());
                SizeChanged += handler;
                return new ActionDisposable(() => SizeChanged -= handler);
            }));

        var contractChanged = new StartWithObservable<string?>(
            this.WhenAnyObservable(x => x.ViewContractObservable).Do(x => _viewContract = x),
            ViewContract);
        var viewModelChanged = new StartWithObservable<object?>(
            this.WhenAnyValue(static x => x.ViewModel),
            ViewModel);
        var viewModelAndContract = contractChanged.CombineLatest(
            viewModelChanged,
            static (contract, vm) => (ViewModel: vm, Contract: contract));

        if (this.GetIsDesignMode())
        {
            return;
        }

        _ = ((IActivatableView)this).WhenActivated(
            d =>
            {
                d(new ObserveOnObservable<string?>(contractChanged, RxSchedulers.MainThreadScheduler)
                    .Subscribe(new DelegateObserver<string?>(x => _viewContract = x ?? string.Empty)));

                d(viewModelAndContract.DistinctUntilChanged()
                    .Subscribe(new DelegateObserver<(object? ViewModel, string? Contract)>(x => ResolveViewForViewModel(x.ViewModel, x.Contract))));
            },
            viewModelChanged);
    }

    /// <summary>Gets or sets the view contract observable.</summary>
    public IObservable<string?> ViewContractObservable
    {
        get => (IObservable<string>)GetValue(ViewContractObservableProperty);
        set => SetValue(ViewContractObservableProperty, value);
    }

    /// <summary>Gets or sets the content displayed by default when no content is set.</summary>
    public object DefaultContent
    {
        get => GetValue(DefaultContentProperty);
        set => SetValue(DefaultContentProperty, value);
    }

    /// <summary>Gets or sets the ViewModel to display.</summary>
    public object? ViewModel
    {
        get => GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }

    /// <summary>Gets or sets the view contract.</summary>
    public string? ViewContract
    {
        get => _viewContract;
        set => ViewContractObservable = Signal.Emit(value);
    }

    /// <summary>Gets or sets the view locator.</summary>
    public IViewLocator? ViewLocator { get; set; }

    /// <summary>Gets or sets a value indicating whether should bypass the default contract fallback behavior.</summary>
    public bool ContractFallbackByPass
    {
        get => (bool)GetValue(ContractFallbackByPassProperty);
        set => SetValue(ContractFallbackByPassProperty, value);
    }

    /// <summary>Resolve view for view model with respect to contract.</summary>
    /// <param name="viewModel">ViewModel.</param>
    /// <param name="contract">contract used by ViewLocator.</param>
    protected virtual void ResolveViewForViewModel(object? viewModel, string? contract)
    {
        var viewInstance = ViewHostResolution.ResolveAttachedView(_resolveView, ViewLocator, viewModel, contract, ContractFallbackByPass);
        Content = viewInstance ?? DefaultContent;
        if (viewInstance is not null || viewModel is null)
        {
            return;
        }

        this.Log().Warn(ViewHostResolution.NoViewFoundWarning(GetType().Name, viewModel, nameof(ViewModelViewHostUnsafe)));
    }
}
