// Copyright (c) 2022 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Reactive.Linq;
using Android.Content;
using Android.OS;

namespace ReactiveUI;

/// <summary>
/// Extension methods for <see cref="Context"/>.
/// </summary>
public static class ContextExtensions
{
    /// <summary>
    /// Binds the service.
    /// </summary>
    /// <returns>The observable sequence of service binders.</returns>
    /// <param name="context">The Context to bind the Service from.</param>
    /// <param name="intent">Identifies the service to connect to. The Intent may specify either an explicit component name, or a logical description (action, category, etc) to match an IntentFilter published by a service.</param>
    /// <param name="flags">Operation options for the binding. The default is Bind.None.</param>
    public static IObservable<IBinder?> ServiceBound(this Context context, Intent intent, Bind flags = Bind.None) => // TODO: Create Test
        ServiceBound<IBinder>(context, intent, flags);

    /// <summary>
    /// Binds the service.
    /// </summary>
    /// <returns>The observable sequence of service binders.</returns>
    /// <param name="context">The Context to bind the Service from.</param>
    /// <param name="intent">Identifies the service to connect to. The Intent may specify either an explicit component name, or a logical description (action, category, etc) to match an IntentFilter published by a service.</param>
    /// <param name="flags">Operation options for the binding. The default is Bind.None.</param>
    /// <typeparam name="TBinder">The type of the returned service binder.</typeparam>
    public static IObservable<TBinder?> ServiceBound<TBinder>(this Context context, Intent intent, Bind flags = Bind.None) // TODO: Create Test
        where TBinder : class, IBinder =>
        Observable.Create<TBinder?>(observer =>
        {
            var connection = new ServiceConnection<TBinder>(context, observer);
            try
            {
                if (!context.BindService(intent, connection, flags))
                {
                    observer.OnError(new Exception("Service bind failed!"));
                }
            }
            catch (Exception ex)
            {
                observer.OnError(ex);
            }

            return connection;
        });

    /// <summary>
    /// A private implementation of IServiceConnection and IDisposable.
    /// </summary>
    /// <typeparam name="TBinder">The binder type.</typeparam>
    private class ServiceConnection<TBinder> : Java.Lang.Object, IServiceConnection
        where TBinder : class, IBinder
    {
        private readonly Context _context;
        private readonly IObserver<TBinder?> _observer;

        private bool _disposed;

        public ServiceConnection(Context context, IObserver<TBinder?> observer)
        {
            _context = context;
            _observer = observer;
        }

        void IServiceConnection.OnServiceConnected(ComponentName? name, IBinder? binder) => _observer.OnNext((TBinder?)binder);

        void IServiceConnection.OnServiceDisconnected(ComponentName? name) =>

            // lost connection to the remote service but it may be revived
            _observer.OnNext(null);

        protected override void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context.UnbindService(this);

                    _disposed = true;
                }
            }

            base.Dispose(disposing);
        }
    }
}
