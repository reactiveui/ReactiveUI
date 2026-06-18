// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using Android.Content;
using Android.OS;
using Context = Android.Content.Context;

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive;
#else
namespace ReactiveUI;
#endif
/// <summary>Provides extension methods for binding to Android services using observable sequences.</summary>
/// <remarks>The ContextExtensions class offers methods that simplify service binding in Android applications by
/// exposing service connection events as IObservable sequences. These methods enable reactive programming patterns for
/// service lifecycle management, making it easier to subscribe to service connection and disconnection events. All
/// members are static and intended to be used as extension methods on the Context type.</remarks>
public static class ContextExtensions
{
    /// <summary>The Context to bind the Service from.</summary>
    /// <param name="context">The <see cref="Context"/> on which the service-binding extension methods operate.</param>
    extension(Context context)
    {
        /// <summary>Binds the service using <see cref="Bind.None"/> and exposes the <see cref="IBinder"/> as an observable sequence.</summary>
        /// <returns>An observable sequence of <see cref="IBinder"/> instances for the bound service.</returns>
        /// <param name="intent">
        /// Identifies the service to connect to, either by an explicit component name or by a logical
        /// description (action, category, etc) matching an IntentFilter; bound here with default options.
        /// </param>
        public IObservable<IBinder?>
            ServiceBound(Intent intent) =>
            context.ServiceBound<IBinder>(intent, Bind.None);

        /// <summary>Binds the service using the supplied <paramref name="flags"/> and exposes the <see cref="IBinder"/> as an observable sequence.</summary>
        /// <returns>An observable sequence of <see cref="IBinder"/> instances for the service bound with the given flags.</returns>
        /// <param name="intent">
        /// Identifies the service to connect to, either by an explicit component name or by a logical
        /// description (action, category, etc) matching an IntentFilter; bound here with the supplied flags.
        /// </param>
        /// <param name="flags">The bind options applied when connecting to the weakly-typed service.</param>
        public IObservable<IBinder?>
            ServiceBound(Intent intent, Bind flags) =>
            context.ServiceBound<IBinder>(intent, flags);

        /// <summary>Binds the service using <see cref="Bind.None"/> and exposes a strongly-typed <typeparamref name="TBinder"/> as an observable sequence.</summary>
        /// <returns>An observable sequence of <typeparamref name="TBinder"/> instances for the bound service.</returns>
        /// <param name="intent">
        /// Identifies the typed service to connect to, either by an explicit component name or by a logical
        /// description (action, category, etc) matching an IntentFilter; bound here with default options.
        /// </param>
        /// <typeparam name="TBinder">The binder type to cast each connected service binder to (bound with default options).</typeparam>
        [SuppressMessage(
            "Major Code Smell",
            "S4018:Generic methods should provide type parameter",
            Justification = "Generic type parameter is supplied explicitly by the caller by design; it identifies the target type and cannot be inferred from the method's parameters.")]
        public IObservable<TBinder?> ServiceBound<TBinder>(
            Intent intent)
            where TBinder : class, IBinder
            =>
            context.ServiceBound<TBinder>(intent, Bind.None);

        /// <summary>Binds the service using the supplied <paramref name="flags"/> and exposes a strongly-typed <typeparamref name="TBinder"/> as an observable sequence.</summary>
        /// <returns>An observable sequence of <typeparamref name="TBinder"/> instances for the service bound with the given flags.</returns>
        /// <param name="intent">
        /// Identifies the typed service to connect to, either by an explicit component name or by a logical
        /// description (action, category, etc) matching an IntentFilter; bound here with the supplied flags.
        /// </param>
        /// <param name="flags">The bind options applied when connecting to the strongly-typed service.</param>
        /// <typeparam name="TBinder">The binder type to cast each connected service binder to (bound with the supplied flags).</typeparam>
        [SuppressMessage(
            "Major Code Smell",
            "S4018:Generic methods should provide type parameter",
            Justification = "Generic type parameter is supplied explicitly by the caller by design; it identifies the target type and cannot be inferred from the method's parameters.")]
        public IObservable<TBinder?> ServiceBound<TBinder>(
            Intent intent,
            Bind flags)
            where TBinder : class, IBinder
            =>
            new ServiceBoundObservable<TBinder>(context, intent, flags);
    }

    /// <summary>
    /// Binds the service on subscribe and surfaces its binder through the observer — replacing
    /// <c>Observable.Create</c>. The returned <see cref="ServiceConnection{TBinder}"/> unbinds the service on dispose.
    /// </summary>
    /// <typeparam name="TBinder">The type of binder surfaced by this observable.</typeparam>
    /// <param name="context">The context used to bind the service on subscription and unbind it on dispose.</param>
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

    /// <summary>A private implementation of IServiceConnection and IDisposable.</summary>
    /// <typeparam name="TBinder">The type of binder delivered through this service connection.</typeparam>
    /// <param name="context">The context held by the connection and used to unbind the service when disposed.</param>
    /// <param name="observer">The observer that receives the service binder notifications.</param>
    private sealed class ServiceConnection<TBinder>(Context context, IObserver<TBinder?> observer)
        : Java.Lang.Object, IServiceConnection
        where TBinder : class, IBinder
    {
        /// <summary>The Context used to bind and unbind the service.</summary>
        private readonly Context _context = context;

        /// <summary>The stored observer that is notified as the service connects and disconnects.</summary>
        private readonly IObserver<TBinder?> _observer = observer;

        /// <summary>Indicates whether this instance has already been disposed.</summary>
        private bool _disposed;

        /// <inheritdoc/>
        void IServiceConnection.OnServiceConnected(ComponentName? name, IBinder? service) =>
            _observer.OnNext((TBinder?)service);

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
