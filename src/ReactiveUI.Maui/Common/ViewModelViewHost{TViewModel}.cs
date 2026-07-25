// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

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
/// This content control will automatically load the View associated with
/// the ViewModel property and display it. This control is very useful
/// inside a DataTemplate to display the View associated with a ViewModel.
/// This generic version provides AOT-compatibility by using compile-time type information.
/// </summary>
/// <typeparam name="TViewModel">The type of the view model. Must have a public parameterless constructor.</typeparam>
public partial class ViewModelViewHost<
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] TViewModel> : TransitioningContentControl, IViewFor<TViewModel>, IEnableLogger
    where TViewModel : class
{
    /// <summary>The default content dependency property.</summary>
    public static readonly DependencyProperty DefaultContentProperty =
        DependencyProperty.Register(nameof(DefaultContent), typeof(object), typeof(ViewModelViewHost<TViewModel>), new(null));

    /// <summary>The view model dependency property.</summary>
    public static readonly DependencyProperty ViewModelProperty =
        DependencyProperty.Register(nameof(ViewModel), typeof(TViewModel), typeof(ViewModelViewHost<TViewModel>), new(null));

    /// <summary>The view contract observable dependency property.</summary>
    public static readonly DependencyProperty ViewContractObservableProperty =
        DependencyProperty.Register(nameof(ViewContractObservable), typeof(IObservable<string>), typeof(ViewModelViewHost<TViewModel>), new(Signal.Emit<string?>(null)));

    /// <summary>The ContractFallbackByPass dependency property.</summary>
    public static readonly DependencyProperty ContractFallbackByPassProperty =
        DependencyProperty.Register(nameof(ContractFallbackByPass), typeof(bool), typeof(ViewModelViewHost<TViewModel>), new(false));

    /// <summary>The subscriptions created during construction, disposed together.</summary>
    private readonly MultipleDisposable _subscriptions = [];

    /// <summary>The most recently observed view contract.</summary>
    private string? _viewContract;

    /// <summary>Initializes a new instance of the <see cref="ViewModelViewHost{TViewModel}"/> class.</summary>
    [SuppressMessage(
        "Design",
        "SST2403:'this' escapes before construction finishes",
        Justification = "The single-threaded UI control hands 'this' to MauiReactiveHelpers to observe its own dependency-property changes; it is never published to another thread.")]
    public ViewModelViewHost()
    {
        var platformGetter = ViewContractObservableHelpers.GetPlatformOrientation(this.Log());
        ViewContractObservable = ViewContractObservableHelpers.Create(
            platformGetter,
            new FromEventObservable<string?>(onNext =>
            {
                SizeChangedEventHandler handler = (_, _) => onNext(platformGetter());
                SizeChanged += handler;
                return new ActionDisposable(() => SizeChanged -= handler);
            }));

        MauiReactiveHelpers.SubscribeViewModelViewHost(
            this,
            (nameof(ViewModel), ViewModelProperty, () => ViewModel),
            ViewContractObservable,
            contract => _viewContract = contract,
            ResolveViewForViewModel,
            _subscriptions);
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
    public TViewModel? ViewModel
    {
        get => (TViewModel?)GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }

    /// <summary>Gets or sets the ViewModel to display (non-generic interface implementation).</summary>
    object? IViewFor.ViewModel
    {
        get => ViewModel;
        set => ViewModel = value as TViewModel;
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

    /// <summary>Gets or sets a value indicating whether should bypass the default contract fallback behavior.</summary>
    public bool ContractFallbackByPass
    {
        get => (bool)GetValue(ContractFallbackByPassProperty);
        set => SetValue(ContractFallbackByPassProperty, value);
    }

    /// <summary>Gets or sets the view locator.</summary>
    public IViewLocator? ViewLocator { get; set; }

    /// <summary>Resolve view for view model with respect to contract.</summary>
    /// <param name="viewModel">ViewModel.</param>
    /// <param name="contract">Contract used by ViewLocator.</param>
    protected virtual void ResolveViewForViewModel(TViewModel? viewModel, string? contract)
    {
        if (viewModel is null)
        {
            Content = DefaultContent;
            return;
        }

        var viewLocator = ViewLocator ?? ReactiveUI.ViewLocator.Current;

        // Use the generic ResolveView<TViewModel> method - this is AOT-safe!
        var viewInstance = viewLocator.ResolveView<TViewModel>(contract);
        if (viewInstance is null && !ContractFallbackByPass)
        {
            viewInstance = viewLocator.ResolveView<TViewModel>();
        }

        if (viewInstance is null)
        {
            Content = DefaultContent;
            this.Log().Warn($"The {nameof(ViewModelViewHost)} could not find a valid view for the view model of type {typeof(TViewModel)} and value {viewModel}.");
            return;
        }

        viewInstance.ViewModel = viewModel;

        Content = viewInstance;
    }
}
