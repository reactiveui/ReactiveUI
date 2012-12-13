#define HAS_AWAIT

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Subjects;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.

/* This file is substantially copied from http://rx.codeplex.com/SourceControl/changeset/view/ef6a42709f49#Rx.NET/System.Reactive.Linq/Reactive/Subjects/AsyncSubject.cs
 * Check LICENSE in this folder for licensing information */

namespace ReactiveUI
{
    /// <summary>
    /// Represents the result of an asynchronous operation.
    /// The last value before the OnCompleted notification, or the error received through OnError, is sent to all subscribed observers.
    /// </summary>
    /// <typeparam name="T">The type of the elements processed by the subject.</typeparam>
    public sealed class AwaitableAsyncSubject<T> : ISubject<T>, IDisposable , INotifyCompletion
    {
        private readonly object _gate = new object();

        private ImmutableList<IObserver<T>> _observers;
        private bool _isDisposed;
        private bool _isStopped;
        private T _value;
        private bool _hasValue;
        private Exception _exception;

        /// <summary>
        /// Creates a subject that can only receive one value and that value is cached for all future observations.
        /// </summary>
        public AwaitableAsyncSubject()
        {
            _observers = new ImmutableList<IObserver<T>>();
        }

        /// <summary>
        /// Notifies all subscribed observers about the end of the sequence, also causing the last received value to be sent out (if any).
        /// </summary>
        public void OnCompleted()
        {
            var os = default(IObserver<T>[]);

            var v = default(T);
            var hv = false;
            lock (_gate)
            {
                CheckDisposed();

                if (!_isStopped)
                {
                    os = _observers.Data;
                    _observers = new ImmutableList<IObserver<T>>();
                    _isStopped = true;
                    v = _value;
                    hv = _hasValue;
                }
            }

            if (os != null)
            {
                if (hv)
                {
                    foreach (var o in os)
                    {
                        o.OnNext(v);
                        o.OnCompleted();
                    }
                }
                else
                    foreach (var o in os)
                        o.OnCompleted();
            }
        }

        /// <summary>
        /// Notifies all subscribed observers about the exception.
        /// </summary>
        /// <param name="error">The exception to send to all observers.</param>
        /// <exception cref="ArgumentNullException"><paramref name="error"/> is null.</exception>
        public void OnError(Exception error)
        {
            if (error == null)
                throw new ArgumentNullException("error");

            var os = default(IObserver<T>[]);
            lock (_gate)
            {
                CheckDisposed();

                if (!_isStopped)
                {
                    os = _observers.Data;
                    _observers = new ImmutableList<IObserver<T>>();
                    _isStopped = true;
                    _exception = error;
                }
            }

            if (os != null)
                foreach (var o in os)
                    o.OnError(error);
        }

        /// <summary>
        /// Sends a value to the subject. The last value received before successful termination will be sent to all subscribed and future observers.
        /// </summary>
        /// <param name="value">The value to store in the subject.</param>
        public void OnNext(T value)
        {
            lock (_gate)
            {
                CheckDisposed();

                if (!_isStopped)
                {
                    _value = value;
                    _hasValue = true;
                }
            }
        }

        /// <summary>
        /// Subscribes an observer to the subject.
        /// </summary>
        /// <param name="observer">Observer to subscribe to the subject.</param>
        /// <returns>Disposable object that can be used to unsubscribe the observer from the subject.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="observer"/> is null.</exception>
        public IDisposable Subscribe(IObserver<T> observer)
        {
            if (observer == null)
                throw new ArgumentNullException("observer");

            var ex = default(Exception);
            var v = default(T);
            var hv = false;

            lock (_gate)
            {
                CheckDisposed();

                if (!_isStopped)
                {
                    _observers = _observers.Add(observer);
                    return new Subscription(this, observer);
                }

                ex = _exception;
                hv = _hasValue;
                v = _value;
            }

            if (ex != null)
                observer.OnError(ex);
            else if (hv)
            {
                observer.OnNext(v);
                observer.OnCompleted();
            }
            else
                observer.OnCompleted();

            return Disposable.Empty;
        }

        class Subscription : IDisposable
        {
            private readonly AwaitableAsyncSubject<T> _subject;
            private IObserver<T> _observer;

            public Subscription(AwaitableAsyncSubject<T> subject, IObserver<T> observer)
            {
                _subject = subject;
                _observer = observer;
            }

            public void Dispose()
            {
                if (_observer != null)
                {
                    lock (_subject._gate)
                    {
                        if (!_subject._isDisposed && _observer != null)
                        {
                            _subject._observers = _subject._observers.Remove(_observer);
                            _observer = null;
                        }
                    }
                }
            }
        }

        void CheckDisposed()
        {
            if (_isDisposed)
                throw new ObjectDisposedException(string.Empty);
        }

        /// <summary>
        /// Unsubscribe all observers and release resources.
        /// </summary>
        public void Dispose()
        {
            lock (_gate)
            {
                _isDisposed = true;
                _observers = null;
                _exception = null;
                _value = default(T);
            }
        }

#if HAS_AWAIT
        /// <summary>
        /// Gets an awaitable object for the current AsyncSubject.
        /// </summary>
        /// <returns>Object that can be awaited.</returns>
        public AwaitableAsyncSubject<T> GetAwaiter()
        {
            return this;
        }

        /// <summary>
        /// Specifies a callback action that will be invoked when the subject completes.
        /// </summary>
        /// <param name="continuation">Callback action that will be invoked when the subject completes.</param>
        /// <exception cref="ArgumentNullException"><paramref name="continuation"/> is null.</exception>
        public void OnCompleted(Action continuation)
        {
            if (continuation == null)
                throw new ArgumentNullException("continuation");

            OnCompleted(continuation, true);
        }
#endif

        private void OnCompleted(Action continuation, bool originalContext)
        {
            //
            // [OK] Use of unsafe Subscribe: this type's Subscribe implementation is safe.
            //
            this.Subscribe/*Unsafe*/(new AwaitObserver(continuation, originalContext));
        }

        class AwaitObserver : IObserver<T>
        {
#if HAS_AWAIT
            private readonly SynchronizationContext _context;
#endif
            private readonly Action _callback;

            public AwaitObserver(Action callback, bool originalContext)
            {
#if HAS_AWAIT
                if (originalContext)
                    _context = SynchronizationContext.Current;
#else
                System.Diagnostics.Debug.Assert(!originalContext);
#endif

                _callback = callback;
            }

            public void OnCompleted()
            {
                InvokeOnOriginalContext();
            }

            public void OnError(Exception error)
            {
                InvokeOnOriginalContext();
            }

            public void OnNext(T value)
            {
            }

            private void InvokeOnOriginalContext()
            {
#if HAS_AWAIT
                if (_context != null)
                {
                    //
                    // No need for OperationStarted and OperationCompleted calls here;
                    // this code is invoked through await support and will have a way
                    // to observe its start/complete behavior, either through returned
                    // Task objects or the async method builder's interaction with the
                    // SynchronizationContext object.
                    //
                    _context.Post(c => ((Action)c)(), _callback);
                }
                else
#endif
                {
                    _callback();
                }
            }
        }

        /// <summary>
        /// Gets whether the AsyncSubject has completed.
        /// </summary>
        public bool IsCompleted
        {
            get
            {
                return _isStopped;
            }
        }

        /// <summary>
        /// Gets the last element of the subject, potentially blocking until the subject completes successfully or exceptionally.
        /// </summary>
        /// <returns>The last element of the subject. Throws an InvalidOperationException if no element was received.</returns>
        /// <exception cref="InvalidOperationException">The source sequence is empty.</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Await pattern for C# and VB compilers.")]
        public T GetResult()
        {
            if (!_isStopped)
            {
                var e = new ManualResetEvent(false);
                OnCompleted(() => e.Set(), false);
                e.WaitOne();
            }

            if (_exception != null) {
                throw _exception;
            }

            if (!_hasValue)
                throw new InvalidOperationException("Sequence has no elements");

            return _value;
        }
    }
}