// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.ComponentModel;
using Microsoft.AspNetCore.Components;
using ReactiveUI.Internal;
using ReactiveUI.Primitives;

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive.Blazor.Internal;
#else
namespace ReactiveUI.Blazor.Internal;
#endif
/// <summary>
/// Internal helper methods for reactive Blazor components.
/// Provides shared functionality for activation wiring and view model change reactivity.
/// </summary>
/// <remarks>
/// <para>
/// This class centralizes common reactive patterns used across all Blazor component base classes,
/// eliminating code duplication and providing a single source of truth for reactive behavior.
/// </para>
/// <para>
/// Performance: the wiring is implemented with direct event handlers and fused sinks rather than operator
/// pipelines, minimizing allocations and giving direct control over subscription lifecycles.
/// </para>
/// </remarks>
internal static class ReactiveComponentHelpers
{
    /// <summary>Wires ReactiveUI activation semantics to the specified view model if it implements <see cref="IActivatableViewModel"/>.</summary>
    /// <typeparam name="T">The view model type that implements <see cref="INotifyPropertyChanged"/>.</typeparam>
    /// <param name="viewModel">The view model to wire activation for.</param>
    /// <param name="state">The reactive component state that provides activation/deactivation observables.</param>
    /// <remarks>
    /// When the component is activated, the view model's <see cref="ViewModelActivator"/> is triggered; when the
    /// component is deactivated, the activator is deactivated. The activation subscription is tracked by
    /// <see cref="ReactiveComponentState.LifetimeDisposables"/> so it is disposed with the component.
    /// </remarks>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="viewModel"/> or <paramref name="state"/> is <see langword="null"/>.
    /// </exception>
    internal static void WireActivationIfSupported<T>(T? viewModel, ReactiveComponentState state)
        where T : class, INotifyPropertyChanged
    {
        ArgumentNullException.ThrowIfNull(state);

        if (viewModel is not IActivatableViewModel avm)
        {
            return;
        }

        // Subscribe to component activation and trigger view model activation.
        _ = state.Activated
            .Subscribe(new DelegateObserver<RxVoid>(_ => avm.Activator.Activate()))
            .DisposeWith(state.LifetimeDisposables);

        // Deactivation subscription does not need disposal tracking beyond component lifetime.
        _ = state.Deactivated.Subscribe(new DelegateObserver<RxVoid>(_ => avm.Activator.Deactivate()));
    }

    /// <summary>
    /// Wires reactivity that triggers UI re-rendering when the view model instance changes or when the current
    /// view model raises property changed events.
    /// </summary>
    /// <typeparam name="T">The view model type that implements <see cref="INotifyPropertyChanged"/>.</typeparam>
    /// <param name="getCurrentViewModel">A function that returns the current view model value.</param>
    /// <param name="addPropertyChangedHandler">
    /// An action that adds a handler to the component's <see cref="INotifyPropertyChanged.PropertyChanged"/> event.
    /// </param>
    /// <param name="removePropertyChangedHandler">
    /// An action that removes a handler from the component's <see cref="INotifyPropertyChanged.PropertyChanged"/> event.
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
    /// <see cref="ReactiveComponentState.FirstRenderSubscriptions"/>.
    /// </returns>
    /// <remarks>
    /// Re-renders when the view model instance is reassigned (after the first assignment) and whenever the current
    /// view model raises any property change, implemented as a single fused
    /// <see cref="ViewModelReactivitySink{T}"/> rather than an operator pipeline.
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown when any parameter is <see langword="null"/>.</exception>
    internal static IDisposable WireViewModelChangeReactivity<T>(
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

        return new ViewModelReactivitySink<T>(
            getCurrentViewModel,
            addPropertyChangedHandler,
            removePropertyChangedHandler,
            viewModelPropertyName,
            stateHasChangedCallback);
    }

    /// <summary>
    /// Performs the shared <c>OnInitialized</c> work for a reactive component: wires activation for the current view
    /// model (when supported) and signals component activation.
    /// </summary>
    /// <typeparam name="T">The view model type that implements <see cref="INotifyPropertyChanged"/>.</typeparam>
    /// <param name="viewModel">The current view model.</param>
    /// <param name="state">The reactive component state.</param>
    internal static void HandleInitialized<T>(T? viewModel, ReactiveComponentState state)
        where T : class, INotifyPropertyChanged
    {
        WireActivationIfSupported(viewModel, state);
        state.NotifyActivated();
    }

    /// <summary>
    /// Performs the shared first-render work for a reactive component: on the first render it wires view-model change
    /// reactivity (stored on the state) and triggers an initial re-render to surface changes raised during activation.
    /// </summary>
    /// <typeparam name="T">The view model type that implements <see cref="INotifyPropertyChanged"/>.</typeparam>
    /// <param name="firstRender">Whether this is the first render of the component.</param>
    /// <param name="state">The reactive component state.</param>
    /// <param name="getCurrentViewModel">A function returning the current view model value.</param>
    /// <param name="addPropertyChangedHandler">Adds a handler to the component's <see cref="INotifyPropertyChanged.PropertyChanged"/> event.</param>
    /// <param name="removePropertyChangedHandler">Removes a handler from the component's <see cref="INotifyPropertyChanged.PropertyChanged"/> event.</param>
    /// <param name="stateHasChangedCallback">A callback that re-renders the component.</param>
    internal static void HandleFirstRender<T>(
        bool firstRender,
        ReactiveComponentState state,
        Func<T?> getCurrentViewModel,
        Action<PropertyChangedEventHandler> addPropertyChangedHandler,
        Action<PropertyChangedEventHandler> removePropertyChangedHandler,
        Action stateHasChangedCallback)
        where T : class, INotifyPropertyChanged
    {
        ArgumentNullException.ThrowIfNull(state);
        ArgumentNullException.ThrowIfNull(stateHasChangedCallback);

        if (!firstRender)
        {
            return;
        }

        // These subscriptions are intentionally created here (not OnInitialized) due to framework interop constraints.
        state.FirstRenderSubscriptions = WireViewModelChangeReactivity(
            getCurrentViewModel,
            addPropertyChangedHandler,
            removePropertyChangedHandler,
            nameof(IViewFor.ViewModel),
            stateHasChangedCallback);

        // Re-render to pick up any property changes that occurred during activation (OnInitialized)
        // before these subscriptions were wired.
        stateHasChangedCallback();
    }

    /// <summary>
    /// Performs the shared dispose work for a reactive component: idempotently notifies deactivation and disposes the
    /// state when disposing managed resources.
    /// </summary>
    /// <param name="disposed">The component's disposed flag, set to <see langword="true"/> on first call.</param>
    /// <param name="disposing"><see langword="true"/> to release managed resources.</param>
    /// <param name="state">The reactive component state.</param>
    internal static void HandleDispose(ref bool disposed, bool disposing, ReactiveComponentState state)
    {
        if (disposed)
        {
            return;
        }

        DeactivateAndDisposeState(disposing, state);
        disposed = true;
    }

    /// <summary>
    /// Notifies deactivation and disposes the state when disposing managed resources. Used by components whose own
    /// base class already provides dispose idempotency (e.g. <c>OwningComponentBase</c>).
    /// </summary>
    /// <param name="disposing"><see langword="true"/> to release managed resources.</param>
    /// <param name="state">The reactive component state.</param>
    internal static void DeactivateAndDisposeState(bool disposing, ReactiveComponentState state)
    {
        ArgumentNullException.ThrowIfNull(state);

        if (!disposing)
        {
            return;
        }

        // Notify deactivation first so observers can perform cleanup while subscriptions are still active.
        state.NotifyDeactivated();
        state.Dispose();
    }

    /// <summary>
    /// Assigns <paramref name="value"/> to <paramref name="field"/> when it differs, reporting whether a change occurred
    /// so the caller can raise its property-changed notification.
    /// </summary>
    /// <typeparam name="T">The view model type.</typeparam>
    /// <param name="field">The backing field to update.</param>
    /// <param name="value">The new value.</param>
    /// <returns><see langword="true"/> when the field changed; otherwise <see langword="false"/>.</returns>
    internal static bool SetIfChanged<T>(ref T? field, T? value)
        where T : class
    {
        if (EqualityComparer<T?>.Default.Equals(field, value))
        {
            return false;
        }

        field = value;
        return true;
    }
}
