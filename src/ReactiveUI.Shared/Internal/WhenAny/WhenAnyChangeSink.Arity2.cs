// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive.Internal;
#else
namespace ReactiveUI.Internal;
#endif
/// <summary>
/// Combines the latest of 2 sources, emitting the selector result once every source
/// has produced a value. Bails on a plain <c>if</c> when peers are not ready.
/// </summary>
/// <typeparam name="T1">The input type of source 1.</typeparam>
/// <typeparam name="T2">The input type of source 2.</typeparam>
/// <typeparam name="TResult">The type produced by the selector.</typeparam>
/// <param name="source1">Source observable 1.</param>
/// <param name="source2">Source observable 2.</param>
/// <param name="selector">Combines the ready values into a result.</param>
internal sealed class WhenAnyChangeSink<T1, T2, TResult>(
    IObservable<T1> source1,
    IObservable<T2> source2,
    Func<T1, T2, TResult> selector) : IObservable<TResult>
{
    /// <inheritdoc/>
    public IDisposable Subscribe(IObserver<TResult> observer)
    {
        ArgumentExceptionHelper.ThrowIfNull(observer);
        var sink = new Sink(observer, selector);
        sink.Run(source1, source2);
        return sink;
    }

    /// <summary>Captures the latest value of each source under a single gate and emits the selector result once every source is ready.</summary>
    /// <param name="downstream">The observer receiving the combined results.</param>
    /// <param name="selector">Combines the ready values into a result.</param>
    private sealed class Sink(IObserver<TResult> downstream, Func<T1, T2, TResult> selector) : IDisposable
    {
#if NET9_0_OR_GREATER
        /// <summary>Serializes value capture and emission across the sources.</summary>
        private readonly Lock _gate = new();
#else
        /// <summary>Serializes value capture and emission across the sources.</summary>
        private readonly object _gate = new();
#endif

        /// <summary>The per-source subscriptions, disposed together on teardown.</summary>
        private readonly IDisposable?[] _subscriptions = new IDisposable?[2];

        /// <summary>The latest value captured from source 1.</summary>
        private T1 _value1 = default!;

        /// <summary>The latest value captured from source 2.</summary>
        private T2 _value2 = default!;

        /// <summary>Whether source 1 has produced a value yet.</summary>
        private bool _has1;

        /// <summary>Whether source 2 has produced a value yet.</summary>
        private bool _has2;

        /// <summary>The number of sources that have not yet completed.</summary>
        private int _active = 2;

        /// <summary>Subscribes to every source, wiring each notification back into the sink.</summary>
        /// <param name="source1">Source observable 1.</param>
        /// <param name="source2">Source observable 2.</param>
        public void Run(IObservable<T1> source1, IObservable<T2> source2)
        {
            var i = 0;
            _subscriptions[i] = source1.Subscribe(new DelegateObserver<T1>(On1, OnError, OnSourceCompleted));
            i++;
            _subscriptions[i] = source2.Subscribe(new DelegateObserver<T2>(On2, OnError, OnSourceCompleted));
            i++;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            for (var i = 0; i < _subscriptions.Length; i++)
            {
                _subscriptions[i]?.Dispose();
            }
        }

        /// <summary>Captures the value from source 1 and emits when every source is ready.</summary>
        /// <param name="change">The notification from source 1.</param>
        private void On1(T1 change)
        {
            lock (_gate)
            {
                _value1 = change;
                _has1 = true;
                if (!_has2)
                {
                    return;
                }

                Emit();
            }
        }

        /// <summary>Captures the value from source 2 and emits when every source is ready.</summary>
        /// <param name="change">The notification from source 2.</param>
        private void On2(T2 change)
        {
            lock (_gate)
            {
                _value2 = change;
                _has2 = true;
                if (!_has1)
                {
                    return;
                }

                Emit();
            }
        }

        /// <summary>Forwards an error from any source and tears down the subscriptions.</summary>
        /// <param name="error">The error to forward.</param>
        private void OnError(Exception error)
        {
            lock (_gate)
            {
                downstream.OnError(error);
            }

            Dispose();
        }

        /// <summary>Completes the result once every source has completed.</summary>
        private void OnSourceCompleted()
        {
            lock (_gate)
            {
                _active--;
                if (_active == 0)
                {
                    downstream.OnCompleted();
                }
            }
        }

        /// <summary>Emits the combined selector result once every source is ready.</summary>
        private void Emit()
        {
            TResult result;
            try
            {
                result = selector(_value1, _value2);
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
