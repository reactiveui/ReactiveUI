// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Components;

namespace ReactiveUI.Blazor.Internal;

/// <summary>
/// Internal helper methods for reactive Blazor components.
/// Provides shared functionality for activation wiring, property change observation, and state management.
/// </summary>
/// <remarks>
/// <para>
/// This class centralizes common reactive patterns used across all Blazor component base classes,
/// eliminating code duplication and providing a single source of truth for reactive behavior.
/// </para>
/// <para>
/// Performance: All methods are optimized to minimize allocations and use static delegates where possible
/// to avoid closure allocations. Observable creation patterns are designed for efficient subscription management.
/// </para>
/// </remarks>
internal static class ReactiveComponentHelpers
{
    /// <summary>
    /// Creates an observable that produces a <see cref="Unit"/> value for each
    /// <see cref="INotifyPropertyChanged.PropertyChanged"/> notification raised by <paramref name="source"/>.
    /// </summary>
    /// <param name="source">The source object that implements <see cref="INotifyPropertyChanged"/>.</param>
    /// <returns>
    /// An observable sequence of <see cref="Unit"/> pulses, one for each property change notification.
    /// </returns>
    /// <remarks>
    /// <para>
    /// This method uses Observable.Create to create a highly efficient event-based observable
    /// with direct event handler management, avoiding the overhead of Observable.FromEvent.
    /// </para>
    /// <para>
    /// Performance: Observable.Create is more efficient than Observable.FromEvent as it avoids
    /// delegate conversions and provides direct control over subscription lifecycle. The event
    /// handler is a local function that captures minimal state, optimizing allocation overhead.
    /// </para>
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="source"/> is <see langword="null"/>.</exception>
    public static IObservable<Unit> CreatePropertyChangedPulse(INotifyPropertyChanged source)
    {
        ArgumentNullException.ThrowIfNull(source);

        return Observable.Create<Unit>(observer =>
        {
            void Handler(object? sender, PropertyChangedEventArgs e) => observer.OnNext(Unit.Default);

            source.PropertyChanged += Handler;
            return Disposable.Create(() => source.PropertyChanged -= Handler);
        });
    }

    /// <summary>
    /// Wires ReactiveUI activation semantics to the specified view model if it implements <see cref="IActivatableViewModel"/>.
    /// </summary>
    /// <typeparam name="T">The view model type that implements <see cref="INotifyPropertyChanged"/>.</typeparam>
    /// <param name="viewModel">The view model to wire activation for.</param>
    /// <param name="state">The reactive component state that provides activation/deactivation observables.</param>
    /// <remarks>
    /// <para>
    /// This method sets up a two-way binding between the component's activation lifecycle and the view model's
    /// <see cref="ViewModelActivator"/>. When the component is activated, the view model's activator is triggered.
    /// When the component is deactivated, the view model's activator is deactivated.
    /// </para>
    /// <para>
    /// The activation subscription is added to <see cref="ReactiveComponentState{T}.LifetimeDisposables"/> to ensure
    /// it is disposed when the component is disposed. The deactivation subscription does not require explicit disposal
    /// as it is a fire-and-forget operation that completes when the component is disposed.
    /// </para>
    /// <para>
    /// Performance: This is a low-frequency setup operation that occurs once during component initialization.
    /// The guard check ensures no work is done if the view model doesn't support activation.
    /// </para>
    /// </remarks>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="viewModel"/> or <paramref name="state"/> is <see langword="null"/>.
    /// </exception>
    public static void WireActivationIfSupported<T>(T? viewModel, ReactiveComponentState<T> state)
        where T : class, INotifyPropertyChanged
    {
        ArgumentNullException.ThrowIfNull(state);

        if (viewModel is not IActivatableViewModel avm)
        {
            return;
        }

        // Subscribe to component activation and trigger view model activation
        state.Activated
            .Subscribe(_ => avm.Activator.Activate())
            .DisposeWith(state.LifetimeDisposables);

        // Deactivation subscription does not need disposal tracking beyond component lifetime
        state.Deactivated.Subscribe(_ => avm.Activator.Deactivate());
    }

    /// <summary>
    /// Creates an observable that emits the current view model (if non-null) and then emits each
    /// subsequent non-null view model assignment.
    /// </summary>
    /// <typeparam name="T">The view model type that implements <see cref="INotifyPropertyChanged"/>.</typeparam>
    /// <param name="getCurrentViewModel">A function that returns the current view model value.</param>
    /// <param name="addPropertyChangedHandler">
    /// An action that adds a handler to the <see cref="INotifyPropertyChanged.PropertyChanged"/> event.
    /// </param>
    /// <param name="removePropertyChangedHandler">
    /// An action that removes a handler from the <see cref="INotifyPropertyChanged.PropertyChanged"/> event.
    /// </param>
    /// <param name="viewModelPropertyName">
    /// The name of the view model property to observe. Typically "ViewModel".
    /// </param>
    /// <returns>
    /// An observable sequence of non-null view models. Emits the current view model once (if non-null),
    /// then emits each subsequent non-null view model assignment.
    /// </returns>
    /// <remarks>
    /// <para>
    /// This method creates a cold observable using Observable.Create. Each subscription
    /// gets its own event handler that is properly cleaned up when the subscription is disposed.
    /// </para>
    /// <para>
    /// The observable filters property changes to only emit when the view model property changes (using
    /// ordinal string comparison for performance). Null view models are filtered out to ensure downstream
    /// operators always receive non-null values.
    /// </para>
    /// <para>
    /// Performance: Uses <see cref="StringComparison.Ordinal"/> for property name comparison, which is
    /// the fastest string comparison method and matches the typical behavior of <see langword="nameof"/> expressions.
    /// </para>
    /// </remarks>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="getCurrentViewModel"/>, <paramref name="addPropertyChangedHandler"/>,
    /// <paramref name="removePropertyChangedHandler"/>, or <paramref name="viewModelPropertyName"/> is <see langword="null"/>.
    /// </exception>
    public static IObservable<T> CreateViewModelChangedStream<T>(
        Func<T?> getCurrentViewModel,
        Action<PropertyChangedEventHandler> addPropertyChangedHandler,
        Action<PropertyChangedEventHandler> removePropertyChangedHandler,
        string viewModelPropertyName)
        where T : class, INotifyPropertyChanged
    {
        ArgumentNullException.ThrowIfNull(getCurrentViewModel);
        ArgumentNullException.ThrowIfNull(addPropertyChangedHandler);
        ArgumentNullException.ThrowIfNull(removePropertyChangedHandler);
        ArgumentNullException.ThrowIfNull(viewModelPropertyName);

        return Observable.Create<T>(
            observer =>
            {
                // Emit current value once to preserve the original "Skip(1)" behavior in consumers
                var current = getCurrentViewModel();
                if (current is not null)
                {
                    observer.OnNext(current);
                }

                // Handler for subsequent changes
                void Handler(object? sender, PropertyChangedEventArgs e)
                {
                    // Use ordinal comparison for best performance; nameof() produces ordinal strings
                    if (!string.Equals(e.PropertyName, viewModelPropertyName, StringComparison.Ordinal))
                    {
                        return;
                    }

                    var vm = getCurrentViewModel();
                    if (vm is not null)
                    {
                        observer.OnNext(vm);
                    }
                }

                addPropertyChangedHandler(Handler);
                return Disposable.Create(() => removePropertyChangedHandler(Handler));
            });
    }

    /// <summary>
    /// Wires reactivity that triggers UI re-rendering when the view model changes or when the current
    /// view model raises property changed events.
    /// </summary>
    /// <typeparam name="T">The view model type that implements <see cref="INotifyPropertyChanged"/>.</typeparam>
    /// <param name="getCurrentViewModel">A function that returns the current view model value.</param>
    /// <param name="addPropertyChangedHandler">
    /// An action that adds a handler to the <see cref="INotifyPropertyChanged.PropertyChanged"/> event.
    /// </param>
    /// <param name="removePropertyChangedHandler">
    /// An action that removes a handler from the <see cref="INotifyPropertyChanged.PropertyChanged"/> event.
    /// </param>
    /// <param name="viewModelPropertyName">
    /// The name of the view model property to observe. Typically "ViewModel".
    /// </param>
    /// <param name="stateHasChangedCallback">
    /// A callback to invoke when the UI should be re-rendered. Typically <see cref="ComponentBase.StateHasChanged"/>
    /// wrapped in <see cref="ComponentBase.InvokeAsync(Action)"/>.
    /// </param>
    /// <returns>
    /// A disposable that tears down all subscriptions when disposed. Should be assigned to
    /// <see cref="ReactiveComponentState{T}.FirstRenderSubscriptions"/>.
    /// </returns>
    /// <remarks>
    /// <para>
    /// This method creates two subscriptions that work together to provide comprehensive UI reactivity:
    /// 1. A subscription that triggers re-render when the view model instance changes (skipping the initial value).
    /// 2. A subscription that triggers re-render when any property on the current view model changes.
    /// </para>
    /// <para>
    /// The view model stream is created with Publish and RefCount operators
    /// to ensure the underlying observable is shared between both subscriptions, preventing duplicate event handler registrations.
    /// </para>
    /// <para>
    /// The Switch operator ensures that when the view model changes, the old view model's
    /// property changes are automatically unsubscribed and the new view model's property changes are subscribed.
    /// </para>
    /// <para>
    /// Performance: The Publish().RefCount() pattern minimizes allocations by sharing the underlying observable.
    /// The Switch operator efficiently manages subscription lifecycle to prevent memory leaks from old view models.
    /// </para>
    /// </remarks>
    /// <exception cref="ArgumentNullException">
    /// Thrown when any parameter is <see langword="null"/>.
    /// </exception>
    public static IDisposable WireViewModelChangeReactivity<T>(
        Func<T?> getCurrentViewModel,
        Action<PropertyChangedEventHandler> addPropertyChangedHandler,
        Action<PropertyChangedEventHandler> removePropertyChangedHandler,
        string viewModelPropertyName,
        Action stateHasChangedCallback)
        where T : class, INotifyPropertyChanged
    {
        ArgumentNullException.ThrowIfNull(getCurrentViewModel);
        ArgumentNullException.ThrowIfNull(addPropertyChangedHandler);
        ArgumentNullException.ThrowIfNull(removePropertyChangedHandler);
        ArgumentNullException.ThrowIfNull(viewModelPropertyName);
        ArgumentNullException.ThrowIfNull(stateHasChangedCallback);

        // Create a shared stream of non-null view models:
        // - Emits the current ViewModel once (if non-null)
        // - Emits subsequent non-null ViewModel assignments
        // The Publish().RefCount(2) pattern shares the subscription between two consumers
        var viewModelChanged = CreateViewModelChangedStream(
                getCurrentViewModel,
                addPropertyChangedHandler,
                removePropertyChangedHandler,
                viewModelPropertyName)
            .Publish()
            .RefCount(2);

        return new CompositeDisposable
        {
            // Skip the initial value to avoid an immediate extra render on first render
            viewModelChanged
                .Skip(1)
                .Subscribe(_ => stateHasChangedCallback()),

            // Re-render on any ViewModel property change
            // Switch unsubscribes from the previous ViewModel automatically when it changes
            viewModelChanged
                .Select(static vm => CreatePropertyChangedPulse(vm))
                .Switch()
                .Subscribe(_ => stateHasChangedCallback())
        };
    }
}
