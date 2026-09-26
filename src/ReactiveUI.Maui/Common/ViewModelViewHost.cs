// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Microsoft.UI.Xaml;
#if REACTIVE_SHIM
using ReactiveUI.Reactive.Maui.Internal;
#else
using ReactiveUI.Maui.Internal;
#endif
using Splat;

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
/// This content control will automatically load the View associated with
/// the ViewModel property and display it. This control is very useful
/// inside a DataTemplate to display the View associated with a ViewModel.
/// </summary>
/// <remarks>
/// The host asks the view locator for the view by the view model's run-time type, without building any type at run
/// time, so it is safe to trim and to compile ahead of time. The default locator checks the view lookup the source
/// generator writes, then the views the app added with <c>Map</c>. A view registered only with the service locator
/// needs <see cref="ViewModelViewHostUnsafe"/>, which also asks the service locator for <see cref="IViewFor{T}"/>
/// closed over the view model's run-time type.
/// </remarks>
[DebuggerDisplay("{ViewContractObservable}, {DefaultContent}")]
public partial class ViewModelViewHost : TransitioningContentControl, IViewFor, IEnableLogger
{
    /// <summary>The default content dependency property.</summary>
    public static readonly DependencyProperty DefaultContentProperty =
        DependencyProperty.Register(nameof(DefaultContent), typeof(object), typeof(ViewModelViewHost), new(null));

    /// <summary>The view model dependency property.</summary>
    public static readonly DependencyProperty ViewModelProperty =
        DependencyProperty.Register(nameof(ViewModel), typeof(object), typeof(ViewModelViewHost), new(null));

    /// <summary>The view contract observable dependency property.</summary>
    public static readonly DependencyProperty ViewContractObservableProperty =
        DependencyProperty.Register(nameof(ViewContractObservable), typeof(IObservable<string>), typeof(ViewModelViewHost), new(Signal.Emit<string?>(null)));

    /// <summary>The ContractFallbackByPass dependency property.</summary>
    public static readonly DependencyProperty ContractFallbackByPassProperty =
        DependencyProperty.Register(nameof(ContractFallbackByPass), typeof(bool), typeof(ViewModelViewHost), new(false));

    /// <summary>The subscriptions created during construction, disposed together.</summary>
    private readonly MultipleDisposable _subscriptions = [];

    /// <summary>Asks a view locator for the view of a view model under a contract.</summary>
    private readonly Func<IViewLocator, object, string?, IViewFor?> _resolveView;

    /// <summary>The most recently observed view contract.</summary>
    private string? _viewContract;

    /// <summary>Initializes a new instance of the <see cref="ViewModelViewHost"/> class.</summary>
    public ViewModelViewHost()
        : this(ResolveViewWithoutReflection)
    {
    }

    /// <summary>Initializes a new instance of the <see cref="ViewModelViewHost"/> class with its view lookup.</summary>
    /// <param name="resolveView">Asks a view locator for the view of a view model under a contract.</param>
    [SuppressMessage(
        "Design",
        "SST2403:'this' escapes before construction finishes",
        Justification = "The single-threaded UI control hands 'this' to MauiReactiveHelpers to observe its own dependency-property changes; it is never published to another thread.")]
    private protected ViewModelViewHost(Func<IViewLocator, object, string?, IViewFor?> resolveView)
    {
        _resolveView = resolveView;
        MauiReactiveHelpers.InitializeViewModelViewHost(
            (this, this.Log(), observable => ViewContractObservable = observable),
            (nameof(ViewModel), ViewModelProperty, () => ViewModel),
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
    public object? ViewModel
    {
        get => GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
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
    protected virtual void ResolveViewForViewModel(object? viewModel, string? contract)
    {
        if (viewModel is null)
        {
            Content = DefaultContent;
            return;
        }

        var viewLocator = ViewLocator ?? GetCurrent();
        var viewInstance = _resolveView(viewLocator, viewModel, contract);
        if (viewInstance is null && !ContractFallbackByPass)
        {
            viewInstance = _resolveView(viewLocator, viewModel, null);
        }

        if (viewInstance is null)
        {
            Content = DefaultContent;
            this.Log().Warn(
                $"The {GetType().Name} could not find a valid view for the view model of type {viewModel.GetType()} and value {viewModel}. "
                + "The view locator checked the generated view lookup and its Map registrations; "
                + $"use {nameof(ViewModelViewHostUnsafe)} to also resolve a view registered only with the service locator.");
            return;
        }

        viewInstance.ViewModel = viewModel;

        Content = viewInstance;
    }

    /// <summary>Finds a view by the view model's run-time type without building any type at run time.</summary>
    /// <param name="viewLocator">The view locator to ask.</param>
    /// <param name="viewModel">The view model to find a view for.</param>
    /// <param name="contract">The contract to resolve under, or <see langword="null"/> for the default view.</param>
    /// <returns>The view, or <see langword="null"/> when neither the generated lookup nor a <c>Map</c> registration has one.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static IViewFor? ResolveViewWithoutReflection(IViewLocator viewLocator, object viewModel, string? contract) =>
        viewLocator.ResolveView(viewModel, contract);
}
