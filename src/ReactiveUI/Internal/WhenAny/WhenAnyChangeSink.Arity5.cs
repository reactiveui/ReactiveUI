// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Helpers;

namespace ReactiveUI.Internal;

/// <summary>
/// Combines the latest of 5 sources, emitting the selector result once every source
/// has produced a value. Bails on a plain <c>if</c> when peers are not ready.
/// </summary>
/// <typeparam name="T1">The input type of source 1.</typeparam>
/// <typeparam name="T2">The input type of source 2.</typeparam>
/// <typeparam name="T3">The input type of source 3.</typeparam>
/// <typeparam name="T4">The input type of source 4.</typeparam>
/// <typeparam name="T5">The input type of source 5.</typeparam>
/// <typeparam name="TResult">The type produced by the selector.</typeparam>
/// <param name="source1">Source observable 1.</param>
/// <param name="source2">Source observable 2.</param>
/// <param name="source3">Source observable 3.</param>
/// <param name="source4">Source observable 4.</param>
/// <param name="source5">Source observable 5.</param>
/// <param name="selector">Combines the ready values into a result.</param>
internal sealed class WhenAnyChangeSink<T1, T2, T3, T4, T5, TResult>(
    IObservable<T1> source1,
    IObservable<T2> source2,
    IObservable<T3> source3,
    IObservable<T4> source4,
    IObservable<T5> source5,
    Func<T1, T2, T3, T4, T5, TResult> selector) : IObservable<TResult>
{
    /// <inheritdoc/>
    public IDisposable Subscribe(IObserver<TResult> observer)
    {
        ArgumentExceptionHelper.ThrowIfNull(observer);
        var sink = new Sink(observer, selector);
        sink.Run(source1, source2, source3, source4, source5);
        return sink;
    }

    /// <summary>
    /// Captures the latest value of each source under a single gate and emits the
    /// selector result once every source is ready.
    /// </summary>
    /// <param name="downstream">The observer receiving the combined results.</param>
    /// <param name="selector">Combines the ready values into a result.</param>
    private sealed class Sink(IObserver<TResult> downstream, Func<T1, T2, T3, T4, T5, TResult> selector) : IDisposable
    {
#if NET9_0_OR_GREATER
        /// <summary>Serializes value capture and emission across the sources.</summary>
        private readonly Lock _gate = new();
#else
        /// <summary>Serializes value capture and emission across the sources.</summary>
        private readonly object _gate = new();
#endif

        /// <summary>The per-source subscriptions, disposed together on teardown.</summary>
        private readonly IDisposable?[] _subscriptions = new IDisposable?[5];

        /// <summary>The latest value captured from source 1.</summary>
        private T1 _value1 = default!;

        /// <summary>The latest value captured from source 2.</summary>
        private T2 _value2 = default!;

        /// <summary>The latest value captured from source 3.</summary>
        private T3 _value3 = default!;

        /// <summary>The latest value captured from source 4.</summary>
        private T4 _value4 = default!;

        /// <summary>The latest value captured from source 5.</summary>
        private T5 _value5 = default!;

        /// <summary>Whether source 1 has produced a value yet.</summary>
        private bool _has1;

        /// <summary>Whether source 2 has produced a value yet.</summary>
        private bool _has2;

        /// <summary>Whether source 3 has produced a value yet.</summary>
        private bool _has3;

        /// <summary>Whether source 4 has produced a value yet.</summary>
        private bool _has4;

        /// <summary>Whether source 5 has produced a value yet.</summary>
        private bool _has5;

        /// <summary>The number of sources that have not yet completed.</summary>
        private int _active = 5;

        /// <summary>Subscribes to every source, wiring each notification back into the sink.</summary>
        /// <param name="source1">Source observable 1.</param>
        /// <param name="source2">Source observable 2.</param>
        /// <param name="source3">Source observable 3.</param>
        /// <param name="source4">Source observable 4.</param>
        /// <param name="source5">Source observable 5.</param>
        public void Run(IObservable<T1> source1, IObservable<T2> source2, IObservable<T3> source3, IObservable<T4> source4, IObservable<T5> source5)
        {
            var i = 0;
            _subscriptions[i++] = source1.Subscribe(new DelegateObserver<T1>(On1, OnError, OnSourceCompleted));
            _subscriptions[i++] = source2.Subscribe(new DelegateObserver<T2>(On2, OnError, OnSourceCompleted));
            _subscriptions[i++] = source3.Subscribe(new DelegateObserver<T3>(On3, OnError, OnSourceCompleted));
            _subscriptions[i++] = source4.Subscribe(new DelegateObserver<T4>(On4, OnError, OnSourceCompleted));
            _subscriptions[i++] = source5.Subscribe(new DelegateObserver<T5>(On5, OnError, OnSourceCompleted));
        }

        /// <summary>Captures the value from source 1 and emits when every source is ready.</summary>
        /// <param name="change">The notification from source 1.</param>
        public void On1(T1 change)
        {
            lock (_gate)
            {
                _value1 = change;
                _has1 = true;
                if (!(_has2 && _has3 && _has4 && _has5))
                {
                    return;
                }

                Emit();
            }
        }

        /// <summary>Captures the value from source 2 and emits when every source is ready.</summary>
        /// <param name="change">The notification from source 2.</param>
        public void On2(T2 change)
        {
            lock (_gate)
            {
                _value2 = change;
                _has2 = true;
                if (!(_has1 && _has3 && _has4 && _has5))
                {
                    return;
                }

                Emit();
            }
        }

        /// <summary>Captures the value from source 3 and emits when every source is ready.</summary>
        /// <param name="change">The notification from source 3.</param>
        public void On3(T3 change)
        {
            lock (_gate)
            {
                _value3 = change;
                _has3 = true;
                if (!(_has1 && _has2 && _has4 && _has5))
                {
                    return;
                }

                Emit();
            }
        }

        /// <summary>Captures the value from source 4 and emits when every source is ready.</summary>
        /// <param name="change">The notification from source 4.</param>
        public void On4(T4 change)
        {
            lock (_gate)
            {
                _value4 = change;
                _has4 = true;
                if (!(_has1 && _has2 && _has3 && _has5))
                {
                    return;
                }

                Emit();
            }
        }

        /// <summary>Captures the value from source 5 and emits when every source is ready.</summary>
        /// <param name="change">The notification from source 5.</param>
        public void On5(T5 change)
        {
            lock (_gate)
            {
                _value5 = change;
                _has5 = true;
                if (!(_has1 && _has2 && _has3 && _has4))
                {
                    return;
                }

                Emit();
            }
        }

        /// <summary>Forwards an error from any source and tears down the subscriptions.</summary>
        /// <param name="error">The error to forward.</param>
        public void OnError(Exception error)
        {
            lock (_gate)
            {
                downstream.OnError(error);
            }

            Dispose();
        }

        /// <summary>Completes the result once every source has completed.</summary>
        public void OnSourceCompleted()
        {
            lock (_gate)
            {
                if (--_active == 0)
                {
                    downstream.OnCompleted();
                }
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            for (var i = 0; i < _subscriptions.Length; i++)
            {
                _subscriptions[i]?.Dispose();
            }
        }

        /// <summary>Emits the combined selector result once every source is ready.</summary>
        private void Emit()
        {
            TResult result;
            try
            {
                result = selector(_value1, _value2, _value3, _value4, _value5);
            }
            catch (Exception ex)
            {
                downstream.OnError(ex);
                return;
            }

            downstream.OnNext(result);
        }
    }
}
