// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.ComponentModel;
using ReactiveUI.Internal;

#if IS_WINUI
using Microsoft.UI.Xaml;
using ReactiveUI.Primitives;
#endif

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive.Maui.Internal;
#else
namespace ReactiveUI.Maui.Internal;
#endif

/// <summary>
/// Internal helper methods for reactive operations in MAUI controls.
/// These methods provide AOT-friendly alternatives to WhenAny* patterns.
/// </summary>
internal static class MauiReactiveHelpers
{
    /// <summary>
    /// Creates an observable that emits when the specified property changes on the source object.
    /// Uses PropertyChanged event directly without expression trees, making it AOT-compatible.
    /// </summary>
    /// <param name="source">The object to observe.</param>
    /// <param name="propertyName">The name of the property to observe (use nameof()).</param>
    /// <returns>An observable that emits RxVoid when the property changes.</returns>
    /// <remarks>
    /// This method uses Observable.Create for better performance compared to Observable.FromEvent.
    /// It filters PropertyChanged events to only emit when the specified property changes.
    /// </remarks>
    internal static IObservable<RxVoid> CreatePropertyChangedPulse(INotifyPropertyChanged source, string propertyName)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(propertyName);

        return new FromEventObservable<RxVoid>(onNext =>
        {
            void Handler(object? _, PropertyChangedEventArgs e)
            {
                if (!string.IsNullOrEmpty(e.PropertyName)
                    && !string.Equals(e.PropertyName, propertyName, StringComparison.Ordinal))
                {
                    return;
                }

                onNext(RxVoid.Default);
            }

            source.PropertyChanged += Handler;
            return new ActionDisposable(() => source.PropertyChanged -= Handler);
        });
    }

    /// <summary>
    /// Creates an observable that emits the current value of a property whenever it changes.
    /// Uses PropertyChanged event directly without expression trees, making it AOT-compatible.
    /// </summary>
    /// <typeparam name="T">The type of the property value.</typeparam>
    /// <param name="source">The object to observe (must implement INotifyPropertyChanged).</param>
    /// <param name="propertyName">The name of the property to observe (use nameof()).</param>
    /// <param name="getPropertyValue">A function to retrieve the current property value.</param>
    /// <returns>An observable that emits the property value when it changes.</returns>
    /// <remarks>
    /// This provides an AOT-friendly alternative to WhenAnyValue by avoiding expression trees.
    /// The observable immediately emits the current value upon subscription, then emits whenever the property changes.
    /// This overload works with any INotifyPropertyChanged implementation and is available for MAUI.
    /// </remarks>
    internal static IObservable<T> CreatePropertyValueObservable<T>(
        INotifyPropertyChanged source,
        string propertyName,
        Func<T> getPropertyValue)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(propertyName);
        ArgumentNullException.ThrowIfNull(getPropertyValue);

        return new FromEventObservable<T>(onNext =>
        {
            // Emit initial value
            onNext(getPropertyValue());

            void Handler(object? _, PropertyChangedEventArgs e)
            {
                if (!string.IsNullOrEmpty(e.PropertyName)
                    && !string.Equals(e.PropertyName, propertyName, StringComparison.Ordinal))
                {
                    return;
                }

                onNext(getPropertyValue());
            }

            source.PropertyChanged += Handler;
            return new ActionDisposable(() => source.PropertyChanged -= Handler);
        });
    }

#if IS_WINUI
    /// <summary>
    /// Creates an observable that emits the current value of a DependencyProperty whenever it changes.
    /// This is a WinUI-specific overload that avoids reflection by accepting the DependencyProperty directly.
    /// </summary>
    /// <typeparam name="T">The type of the property value.</typeparam>
    /// <param name="source">The DependencyObject to observe.</param>
    /// <param name="propertyName">The name of the property to observe (use nameof()).</param>
    /// <param name="property">The DependencyProperty to observe.</param>
    /// <param name="getPropertyValue">A function to retrieve the current property value.</param>
    /// <returns>An observable that emits the property value when it changes.</returns>
    /// <remarks>
    /// This provides an AOT-friendly alternative to WhenAnyValue by avoiding expression trees and reflection.
    /// The observable immediately emits the current value upon subscription, then emits whenever the property changes.
    /// </remarks>
    internal static IObservable<T> CreatePropertyValueObservable<T>(
        DependencyObject source,
        string propertyName,
        DependencyProperty property,
        Func<T> getPropertyValue)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(propertyName);
        ArgumentNullException.ThrowIfNull(property);
        ArgumentNullException.ThrowIfNull(getPropertyValue);

        return new FromEventObservable<T>(onNext =>
        {
            // Emit initial value
            onNext(getPropertyValue());

            // Register for property changes using the provided DependencyProperty
            var token = source.RegisterPropertyChangedCallback(property, (_, _) => onNext(getPropertyValue()));

            return new ActionDisposable(() => source.UnregisterPropertyChangedCallback(property, token));
        });
    }

    /// <summary>Subscribes a WinUI routed host to its router and view-contract properties.</summary>
    /// <param name="source">The host dependency object.</param>
    /// <param name="router">The router property metadata and accessor.</param>
    /// <param name="viewContractObservable">The view-contract observable property metadata and accessor.</param>
    /// <param name="getViewContract">Gets the current view contract.</param>
    /// <param name="setViewContract">Stores the latest view contract.</param>
    /// <param name="resolveView">Resolves a routed view model and contract.</param>
    /// <param name="subscriptions">Collects the host subscription.</param>
    internal static void SubscribeRoutedViewHost(
        DependencyObject source,
        (string Name, DependencyProperty Property, Func<RoutingState> GetValue) router,
        (string Name, DependencyProperty Property, Func<IObservable<string?>> GetValue) viewContractObservable,
        Func<string?> getViewContract,
        Action<string?> setViewContract,
        Action<(IRoutableViewModel? viewModel, string? contract)> resolveView,
        MultipleDisposable subscriptions)
    {
        var routerChanged = CreatePropertyValueObservable(source, router.Name, router.Property, router.GetValue);
        var viewContractObservableChanged = CreatePropertyValueObservable(
            source,
            viewContractObservable.Name,
            viewContractObservable.Property,
            viewContractObservable.GetValue);
        var currentViewModel = new StartWithObservable<IRoutableViewModel?>(
            new KeepSignal<RoutingState?>(routerChanged, static router => router is not null)
                .SelectMany(static router => router!.CurrentViewModel),
            null);
        var viewContract = new StartWithObservable<string?>(
            viewContractObservableChanged
                .SelectMany(static observable => observable ?? Signal.Emit<string?>(null))
                .Do(setViewContract),
            getViewContract());

        _ = currentViewModel
            .CombineLatest(viewContract, static (viewModel, contract) => (viewModel, contract))
            .DistinctUntilChanged()
            .Subscribe(new DelegateObserver<(IRoutableViewModel? viewModel, string? contract)>(
                resolveView,
                RxState.DefaultExceptionHandler.OnNext))
            .DisposeWith(subscriptions);
    }

    /// <summary>Subscribes a WinUI view-model host to its view model and contract.</summary>
    /// <typeparam name="TViewModel">The hosted view-model type.</typeparam>
    /// <param name="source">The host dependency object.</param>
    /// <param name="viewModel">The view-model property metadata and accessor.</param>
    /// <param name="viewContractObservable">The view-contract observable.</param>
    /// <param name="setViewContract">Stores the latest view contract.</param>
    /// <param name="resolveView">Resolves a view model and contract.</param>
    /// <param name="subscriptions">Collects the host subscriptions.</param>
    internal static void SubscribeViewModelViewHost<TViewModel>(
        DependencyObject source,
        (string Name, DependencyProperty Property, Func<TViewModel?> GetValue) viewModel,
        IObservable<string?> viewContractObservable,
        Action<string?> setViewContract,
        Action<TViewModel?, string?> resolveView,
        MultipleDisposable subscriptions)
    {
        var viewModelChanged = CreatePropertyValueObservable(
            source,
            viewModel.Name,
            viewModel.Property,
            viewModel.GetValue);
        var viewModelAndContract = viewContractObservable
            .Do(setViewContract)
            .CombineLatest(viewModelChanged, static (contract, viewModel) => (viewModel, contract));

        _ = new ObserveOnObservable<string?>(viewContractObservable, RxSchedulers.MainThreadScheduler)
            .Subscribe(new DelegateObserver<string?>(contract => setViewContract(contract ?? string.Empty)))
            .DisposeWith(subscriptions);
        _ = viewModelAndContract
            .DistinctUntilChanged()
            .Subscribe(new DelegateObserver<(TViewModel? viewModel, string? contract)>(
                pair => resolveView(pair.viewModel, pair.contract)))
            .DisposeWith(subscriptions);
    }
#endif

    /// <summary>Wires up activation for a view model that supports activation.</summary>
    /// <param name="viewModel">The view model to activate.</param>
    /// <param name="activatedSignal">Observable that signals when the view is activated.</param>
    /// <param name="deactivatedSignal">Observable that signals when the view is deactivated.</param>
    /// <returns>A disposable that manages the activation subscriptions.</returns>
    internal static IDisposable WireActivationIfSupported(
        object? viewModel,
        IObservable<RxVoid> activatedSignal,
        IObservable<RxVoid> deactivatedSignal)
    {
        if (viewModel is not IActivatableViewModel activatable)
        {
            return EmptyDisposable.Instance;
        }

        var activatedSub = activatedSignal.Subscribe(new DelegateObserver<RxVoid>(_ => activatable.Activator.Activate()));
        var deactivatedSub = deactivatedSignal.Subscribe(new DelegateObserver<RxVoid>(_ => activatable.Activator.Deactivate()));

        return new MultipleDisposable(activatedSub, deactivatedSub);
    }
}
