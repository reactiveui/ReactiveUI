// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;

namespace ReactiveUI.Blazor.Internal;

/// <summary>
/// Internal state container for reactive Blazor components.
/// Manages activation lifecycle, subscriptions, and disposal semantics.
/// </summary>
/// <typeparam name="T">The view model type that implements <see cref="INotifyPropertyChanged"/>.</typeparam>
/// <remarks>
/// <para>
/// This class encapsulates the common reactive infrastructure shared across all reactive Blazor component base classes,
/// eliminating code duplication and centralizing allocation patterns for better performance and maintainability.
/// </para>
/// <para>
/// Performance: All fields are initialized inline to minimize allocation overhead. The state instance should be
/// created once per component and reused throughout the component's lifetime.
/// </para>
/// </remarks>
internal sealed class ReactiveComponentState<T> : IDisposable
    where T : class, INotifyPropertyChanged
{
    /// <summary>
    /// Signals component activation. Emits <see cref="Unit.Default"/> when <see cref="NotifyActivated"/> is called.
    /// </summary>
    private readonly Subject<Unit> _initSubject = new();

    /// <summary>
    /// Signals component deactivation. Emits <see cref="Unit.Default"/> when <see cref="NotifyDeactivated"/> is called.
    /// </summary>
    /// <remarks>
    /// Suppressed CA2213 because this subject is used for signaling only and is disposed explicitly in <see cref="Dispose"/>.
    /// </remarks>
    [SuppressMessage("Design", "CA2213:Disposable fields should be disposed", Justification = "Disposed explicitly in Dispose method.")]
    private readonly Subject<Unit> _deactivateSubject = new();

    /// <summary>
    /// Holds subscriptions tied to the component lifetime. Disposed when the component is disposed.
    /// </summary>
    private readonly CompositeDisposable _lifetimeDisposables = [];

    /// <summary>
    /// Holds subscriptions created on first render.
    /// </summary>
    /// <remarks>
    /// This SerialDisposable avoids framework conflicts that occur when certain subscriptions are created
    /// during OnInitialized rather than OnAfterRender. The subscription is replaced each time
    /// <see cref="FirstRenderSubscriptions"/> is assigned.
    /// </remarks>
    private readonly SerialDisposable _firstRenderSubscriptions = new();

    /// <summary>
    /// Indicates whether the state has been disposed. Prevents double disposal.
    /// </summary>
    private bool _disposed;

    /// <summary>
    /// Gets an observable that emits when the component is activated.
    /// </summary>
    /// <remarks>
    /// This observable emits once during component initialization and can be used to trigger
    /// reactive activation patterns for view models implementing <see cref="IActivatableViewModel"/>.
    /// </remarks>
    public IObservable<Unit> Activated => _initSubject.AsObservable();

    /// <summary>
    /// Gets an observable that emits when the component is deactivated.
    /// </summary>
    /// <remarks>
    /// This observable emits during component disposal, allowing cleanup operations to execute
    /// while subscriptions are still active.
    /// </remarks>
    public IObservable<Unit> Deactivated => _deactivateSubject.AsObservable();

    /// <summary>
    /// Gets the composite disposable for lifetime subscriptions.
    /// </summary>
    /// <remarks>
    /// Use this to register subscriptions that should live for the entire component lifetime.
    /// All subscriptions added here will be disposed when the component is disposed.
    /// </remarks>
    [SuppressMessage("Style", "RCS1085:Use auto-implemented property", Justification = "Explicit field backing provides clarity and follows established pattern in this class.")]
    public CompositeDisposable LifetimeDisposables => _lifetimeDisposables;

    /// <summary>
    /// Gets or sets the disposable for first-render-only subscriptions.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This property wraps a <see cref="SerialDisposable"/> to ensure that setting a new subscription
    /// automatically disposes the previous one. Typically set once during OnAfterRender when firstRender is true.
    /// </para>
    /// <para>
    /// Performance: The property is intentionally implemented with explicit getters and setters rather than
    /// as an auto-property to provide controlled access to the underlying SerialDisposable's Disposable property,
    /// ensuring proper disposal semantics.
    /// </para>
    /// </remarks>
    [SuppressMessage("Style", "RCS1085:Use auto-implemented property", Justification = "Intentional wrapper for SerialDisposable.Disposable property to ensure proper disposal semantics.")]
    public IDisposable? FirstRenderSubscriptions
    {
        get => _firstRenderSubscriptions.Disposable;
        set => _firstRenderSubscriptions.Disposable = value;
    }

    /// <summary>
    /// Notifies observers that the component has been activated.
    /// </summary>
    /// <remarks>
    /// Call this method during component initialization (typically in OnInitialized) to signal
    /// that the component is now active and ready for reactive operations.
    /// </remarks>
    public void NotifyActivated() => _initSubject.OnNext(Unit.Default);

    /// <summary>
    /// Notifies observers that the component is being deactivated.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Call this method during component disposal to signal that the component is shutting down.
    /// This notification occurs before subscriptions are disposed, allowing observers to perform
    /// cleanup while their subscriptions are still active.
    /// </para>
    /// <para>
    /// Performance: This method is typically called once during disposal and incurs minimal overhead.
    /// </para>
    /// </remarks>
    public void NotifyDeactivated() => _deactivateSubject.OnNext(Unit.Default);

    /// <summary>
    /// Disposes all managed resources held by this state container.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Disposal order is critical for correct cleanup behavior:
    /// 1. First-render subscriptions are disposed first (may depend on lifetime subscriptions).
    /// 2. Lifetime subscriptions are disposed next (general cleanup).
    /// 3. Subjects are disposed last (signal completion to any remaining observers).
    /// </para>
    /// <para>
    /// This method is idempotent; calling it multiple times has no effect after the first call.
    /// </para>
    /// </remarks>
    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _firstRenderSubscriptions.Dispose();
        _lifetimeDisposables.Dispose();
        _initSubject.Dispose();
        _deactivateSubject.Dispose();

        _disposed = true;
    }
}
