// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics;
using Microsoft.Maui.Controls;
using ReactiveUI.Internal;
using ReactiveUI.Primitives;
using Splat;

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive.Maui;
#else
namespace ReactiveUI.Maui;
#endif

/// <summary>
/// This content view will automatically load and host the view for the given view model. The view model whose view is
/// to be displayed should be assigned to the <see cref="ViewModel"/> property. Optionally, the chosen view can be
/// customized by specifying a contract via <see cref="ViewContractObservable"/> or <see cref="ViewContract"/>.
/// </summary>
/// <remarks>
/// The host asks the view locator for the view by the view model's run-time type, without building any type at run
/// time, so it is safe to trim and to compile ahead of time. The default locator checks the view lookup the source
/// generator writes, then the views the app added with <c>Map</c>. A view registered only with the service locator
/// needs <see cref="ViewModelViewHostUnsafe"/>, which also asks the service locator for <see cref="IViewFor{T}"/>
/// closed over the view model's run-time type.
/// </remarks>
[DebuggerDisplay("{ViewModel}, {DefaultContent}")]
public class ViewModelViewHost : ContentView, IViewFor
{
    /// <summary>Identifies the <see cref="ViewModel"/> property.</summary>
    public static readonly BindableProperty ViewModelProperty = BindableProperty.Create(
        nameof(ViewModel),
        typeof(object),
        typeof(ViewModelViewHost),
        propertyChanged: OnViewModelPropertyChanged);

    /// <summary>Identifies the <see cref="DefaultContent"/> property.</summary>
    public static readonly BindableProperty DefaultContentProperty = BindableProperty.Create(
        nameof(DefaultContent),
        typeof(View),
        typeof(ViewModelViewHost));

    /// <summary>Identifies the <see cref="ViewContractObservable"/> property.</summary>
    public static readonly BindableProperty ViewContractObservableProperty = BindableProperty.Create(
        nameof(ViewContractObservable),
        typeof(IObservable<string>),
        typeof(ViewModelViewHost),
        Signal.Silent<string>(),
        propertyChanged: OnViewContractObservablePropertyChanged);

    /// <summary>The ContractFallbackByPass dependency property.</summary>
    public static readonly BindableProperty ContractFallbackByPassProperty = BindableProperty.Create(
        nameof(ContractFallbackByPass),
        typeof(bool),
        typeof(ViewModelViewHost),
        false);

    /// <summary>Asks a view locator for the view of a view model under a contract.</summary>
    private readonly Func<IViewLocator, object, string?, IViewFor?> _resolveView;

    /// <summary>The subscription to the current <see cref="ViewContractObservable"/>, replaced when the property changes.</summary>
    private IDisposable? _viewContractSubscription;

    /// <summary>The most recently observed view contract.</summary>
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

        // NB: InUnitTestRunner also returns true in Design Mode
        if (ModeDetector.InUnitTestRunner())
        {
            ViewContractObservable = Signal.Silent<string>();
            return;
        }

        // Assigning the property subscribes through OnViewContractObservablePropertyChanged, which re-resolves on
        // every contract; ViewModel changes are handled by OnViewModelPropertyChanged.
        ViewContractObservable = Signal.Emit<string?>(null);
    }

    /// <summary>Gets or sets the view model whose associated view is to be displayed.</summary>
    public object? ViewModel
    {
        get => GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }

    /// <summary>Gets or sets the content to display when <see cref="ViewModel"/> is <see langword="null"/>.</summary>
    public View DefaultContent
    {
        get => (View)GetValue(DefaultContentProperty);
        set => SetValue(DefaultContentProperty, value);
    }

    /// <summary>
    /// Gets or sets the observable which signals when the contract to use when resolving the view for the given view model has changed.
    /// </summary>
    public IObservable<string?> ViewContractObservable
    {
        get => (IObservable<string>)GetValue(ViewContractObservableProperty);
        set => SetValue(ViewContractObservableProperty, value);
    }

    /// <summary>Gets or sets the fixed contract to use when resolving the view for the given view model.</summary>
    /// <remarks>
    /// This property is a mere convenience so that a fixed contract can be assigned directly in XAML.
    /// </remarks>
    public string? ViewContract
    {
        get => _viewContract;
        set => ViewContractObservable = Signal.Emit(value);
    }

    /// <summary>Gets or sets a value indicating whether should bypass the default contract fallback behavior.</summary>
    public bool ContractFallbackByPass
    {
        get => (bool)GetValue(ContractFallbackByPassProperty);
        set => SetValue(ContractFallbackByPassProperty, value);
    }

    /// <summary>Gets or sets the override for the view locator to use when resolving the view. If unspecified, the locator registered in the service locator is used.</summary>
    public IViewLocator? ViewLocator { get; set; }

    /// <summary>Resolves a view for the view model using the specified contract.</summary>
    /// <param name="viewModel">ViewModel.</param>
    /// <param name="contract">contract used by ViewLocator.</param>
    /// <exception cref="InvalidOperationException">No view is registered for <paramref name="viewModel"/>.</exception>
    protected virtual void ResolveViewForViewModel(object? viewModel, string? contract)
    {
        if (viewModel is null)
        {
            Content = DefaultContent;
            return;
        }

        var viewInstance = ViewHostResolution.ResolveViewWithFallback(_resolveView, ViewLocator, viewModel, contract, ContractFallbackByPass)
            ?? throw new InvalidOperationException(
                $"Couldn't find view for '{viewModel}'. The view locator checked the generated view lookup and its Map registrations; "
                + $"use {nameof(ViewModelViewHostUnsafe)} to also resolve a view registered only with the service locator.");

        if (viewInstance is not View castView)
        {
            throw new InvalidOperationException(
                $"View '{viewInstance.GetType().FullName}' is not a subclass of '{typeof(View).FullName}'.");
        }

        viewInstance.ViewModel = viewModel;

        Content = castView;
    }

    /// <summary>Handles changes to the <see cref="ViewModel"/> property by re-resolving the view for the new value.</summary>
    /// <param name="bindable">The object whose property changed.</param>
    /// <param name="_">The previous value.</param>
    /// <param name="newValue">The new value.</param>
    private static void OnViewModelPropertyChanged(BindableObject bindable, object? _, object? newValue)
    {
        if (bindable is not ViewModelViewHost host || ModeDetector.InUnitTestRunner())
        {
            return;
        }

        host.ResolveViewForViewModel(newValue, host._viewContract);
    }

    /// <summary>Handles changes to the <see cref="ViewContractObservable"/> property by switching the contract subscription to the new observable.</summary>
    /// <param name="bindable">The object whose property changed.</param>
    /// <param name="_">The previous value.</param>
    /// <param name="newValue">The new value.</param>
    private static void OnViewContractObservablePropertyChanged(BindableObject bindable, object? _, object? newValue)
    {
        if (bindable is not ViewModelViewHost host || ModeDetector.InUnitTestRunner())
        {
            return;
        }

        // Drop the previous subscription so only the latest observable drives the view.
        host._viewContractSubscription?.Dispose();
        host._viewContractSubscription = (newValue as IObservable<string?>)?
            .Subscribe(new DelegateObserver<string?>(contract =>
            {
                host._viewContract = contract;
                host.ResolveViewForViewModel(host.ViewModel, contract);
            }));
    }
}
