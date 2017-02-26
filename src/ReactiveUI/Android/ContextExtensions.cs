using System;
using System.Reactive.Linq;
using Android.Content;
using Android.OS;

namespace ReactiveUI
{
    /// <summary>
    /// Context Extensions
    /// </summary>
    public static class ContextExtensions
    {
        /// <summary>
        /// Binds the service.
        /// </summary>
        /// <returns>The observable sequence of service binders.</returns>
        /// <param name="context">The Context to bind the Service from.</param>
        /// <param name="intent">
        /// Identifies the service to connect to. The Intent may specify either an explicit component
        /// name, or a logical description (action, category, etc) to match an IntentFilter published
        /// by a service.
        /// </param>
        /// <param name="flags">Operation options for the binding. The default is Bind.None.</param>
        public static IObservable<IBinder> ServiceBound(this Context context, Intent intent, Bind flags = Bind.None)
        {
            return ServiceBound<IBinder>(context, intent, flags);
        }

        /// <summary>
        /// Binds the service.
        /// </summary>
        /// <returns>The observable sequence of service binders.</returns>
        /// <param name="context">The Context to bind the Service from.</param>
        /// <param name="intent">
        /// Identifies the service to connect to. The Intent may specify either an explicit component
        /// name, or a logical description (action, category, etc) to match an IntentFilter published
        /// by a service.
        /// </param>
        /// <param name="flags">Operation options for the binding. The default is Bind.None.</param>
        /// <typeparam name="TBinder">The type of the returned service binder.</typeparam>
        public static IObservable<TBinder> ServiceBound<TBinder>(this Context context, Intent intent, Bind flags = Bind.None)
        where TBinder
            : class
            , IBinder
        {
            return Observable.Create<TBinder>(observer => {
                var connection = new ServiceConnection<TBinder>(context, observer);
                try {
                    if (!context.BindService(intent, connection, flags))
                        observer.OnError(new Exception("Service bind failed!"));
                } catch (Exception ex) {
                    observer.OnError(ex);
                }

                return connection;
            });
        }

        /// <summary>
        /// A private implementation of IServiceConnection and IDisposable.
        /// </summary>
        /// <typeparam name="TBinder">The type of the binder.</typeparam>
        /// <seealso cref="Java.Lang.Object"/>
        /// <seealso cref="Android.Content.IServiceConnection"/>
        private class ServiceConnection<TBinder>
            : Java.Lang.Object
            , IServiceConnection
        where TBinder
            : class
            , IBinder
        {
            private readonly Context context;
            private readonly IObserver<TBinder> observer;

            private bool disposed;

            public ServiceConnection(Context context, IObserver<TBinder> observer)
            {
                this.context = context;
                this.observer = observer;
            }

            void IServiceConnection.OnServiceConnected(ComponentName name, IBinder binder)
            {
                this.observer.OnNext((TBinder)binder);
            }

            void IServiceConnection.OnServiceDisconnected(ComponentName name)
            {
                // lost connection to the remote service but it may be revived
                this.observer.OnNext(null);
            }

            protected override void Dispose(bool disposing)
            {
                if (!this.disposed) {
                    if (disposing) {
                        this.context.UnbindService(this);

                        this.disposed = true;
                    }
                }

                base.Dispose(disposing);
            }
        }
    }
}