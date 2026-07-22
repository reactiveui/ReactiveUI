// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.ComponentModel;
using ReactiveUI.Internal;

#if IS_WINUI
using Microsoft.UI.Xaml;
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
                if (!string.IsNullOrEmpty(e.PropertyName) &&
                    !string.Equals(e.PropertyName, propertyName, StringComparison.Ordinal))
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
                if (!string.IsNullOrEmpty(e.PropertyName) &&
                    !string.Equals(e.PropertyName, propertyName, StringComparison.Ordinal))
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
