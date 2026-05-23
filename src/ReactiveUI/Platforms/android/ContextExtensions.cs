// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using Android.Content;
using Android.OS;
using ReactiveUI.Helpers;
using Context = Android.Content.Context;

namespace ReactiveUI;

/// <summary>
/// Provides extension methods for binding to Android services using observable sequences.
/// </summary>
/// <remarks>The ContextExtensions class offers methods that simplify service binding in Android applications by
/// exposing service connection events as IObservable sequences. These methods enable reactive programming patterns for
/// service lifecycle management, making it easier to subscribe to service connection and disconnection events. All
/// members are static and intended to be used as extension methods on the Context type.</remarks>
public static class ContextExtensions
{
    /// <summary>
    /// Binds the service and exposes the service binder as an observable sequence.
    /// </summary>
    /// <returns>The observable sequence of service binders.</returns>
    /// <param name="context">The Context to bind the Service from.</param>
    /// <param name="intent">
    /// Identifies the service to connect to. The Intent may specify either an explicit component name,
    /// or a logical description (action, category, etc) to match an IntentFilter published by a service.
    /// </param>
    public static IObservable<IBinder?>
        ServiceBound(this Context context, Intent intent) =>
        ServiceBound<IBinder>(context, intent, Bind.None);

    /// <summary>
    /// Binds the service and exposes the service binder as an observable sequence.
    /// </summary>
    /// <returns>The observable sequence of service binders.</returns>
    /// <param name="context">The Context to bind the Service from.</param>
    /// <param name="intent">
    /// Identifies the service to connect to. The Intent may specify either an explicit component name,
    /// or a logical description (action, category, etc) to match an IntentFilter published by a service.
    /// </param>
    /// <param name="flags">Operation options for the binding.</param>
    public static IObservable<IBinder?>
        ServiceBound(this Context context, Intent intent, Bind flags) =>
        ServiceBound<IBinder>(context, intent, flags);

    /// <summary>
    /// Binds the service.
    /// </summary>
    /// <returns>The observable sequence of service binders.</returns>
    /// <param name="context">The Context to bind the Service from.</param>
    /// <param name="intent">
    /// Identifies the service to connect to. The Intent may specify either an explicit component name,
    /// or a logical description (action, category, etc) to match an IntentFilter published by a service.
    /// </param>
    /// <typeparam name="TBinder">The type of the returned service binder.</typeparam>
    [SuppressMessage(
        "Major Code Smell",
        "S4018:Generic methods should provide type parameter",
        Justification = "Generic type parameter is supplied explicitly by the caller by design; it identifies the target type and cannot be inferred from the method's parameters.")]
    public static IObservable<TBinder?> ServiceBound<TBinder>(
        this Context context,
        Intent intent)
        where TBinder : class, IBinder
        =>
        ServiceBound<TBinder>(context, intent, Bind.None);

    /// <summary>
    /// Binds the service.
    /// </summary>
    /// <returns>The observable sequence of service binders.</returns>
    /// <param name="context">The Context to bind the Service from.</param>
    /// <param name="intent">
    /// Identifies the service to connect to. The Intent may specify either an explicit component name,
    /// or a logical description (action, category, etc) to match an IntentFilter published by a service.
    /// </param>
    /// <param name="flags">Operation options for the binding.</param>
    /// <typeparam name="TBinder">The type of the returned service binder.</typeparam>
    [SuppressMessage(
        "Major Code Smell",
        "S4018:Generic methods should provide type parameter",
        Justification = "Generic type parameter is supplied explicitly by the caller by design; it identifies the target type and cannot be inferred from the method's parameters.")]
    public static IObservable<TBinder?> ServiceBound<TBinder>(
        this Context context,
        Intent intent,
        Bind flags)
        where TBinder : class, IBinder
        =>
        new ServiceBoundObservable<TBinder>(context, intent, flags);

    /// <summary>
    /// Binds the service on subscribe and surfaces its binder through the observer — replacing
    /// <c>Observable.Create</c>. The returned <see cref="ServiceConnection{TBinder}"/> unbinds the service on dispose.
    /// </summary>
    /// <typeparam name="TBinder">The binder type.</typeparam>
    /// <param name="context">The context used to bind and unbind the service.</param>
    /// <param name="intent">The intent identifying the service to bind.</param>
    /// <param name="flags">The bind flags.</param>
    private sealed class ServiceBoundObservable<TBinder>(Context context, Intent intent, Bind flags)
        : IObservable<TBinder?>
        where TBinder : class, IBinder
    {
        /// <inheritdoc/>
        public IDisposable Subscribe(IObserver<TBinder?> observer)
        {
            ArgumentExceptionHelper.ThrowIfNull(observer);

            ServiceConnection<TBinder> connection = new(context, observer);
            try
            {
                if (!context.BindService(intent, connection, flags))
                {
                    observer.OnError(new InvalidOperationException("Service bind failed!"));
                }
            }
            catch (Exception ex)
            {
                observer.OnError(ex);
            }

            return connection;
        }
    }

    /// <summary>
    /// A private implementation of IServiceConnection and IDisposable.
    /// </summary>
    /// <typeparam name="TBinder">The binder type.</typeparam>
    private sealed class ServiceConnection<TBinder>(Context context, IObserver<TBinder?> observer)
        : Java.Lang.Object, IServiceConnection
        where TBinder : class, IBinder
    {
        /// <summary>
        /// The Context used to bind and unbind the service.
        /// </summary>
        private readonly Context _context = context;

        /// <summary>
        /// The observer that receives the service binder notifications.
        /// </summary>
        private readonly IObserver<TBinder?> _observer = observer;

        /// <summary>
        /// Indicates whether this instance has already been disposed.
        /// </summary>
        private bool _disposed;

        /// <inheritdoc/>
        void IServiceConnection.OnServiceConnected(ComponentName? name, IBinder? binder) =>
            _observer.OnNext((TBinder?)binder);

        /// <inheritdoc/>
        void IServiceConnection.OnServiceDisconnected(ComponentName? name) => _observer.OnNext(null);

        /// <inheritdoc/>
        protected override void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                _context.UnbindService(this);
                _context.Dispose();
                _disposed = true;
            }

            base.Dispose(disposing);
        }
    }
}
