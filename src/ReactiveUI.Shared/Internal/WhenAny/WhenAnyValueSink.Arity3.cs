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
/// Combines 3 observed property values, emitting the selector result once every
/// property has produced a value. Each per-source observer extracts the change value
/// inline and bails on a plain <c>if</c> when peers are not ready (no-alloc fast path).
/// </summary>
/// <typeparam name="TSender">The type of the object whose properties are observed.</typeparam>
/// <typeparam name="T1">The value type of observed property 1.</typeparam>
/// <typeparam name="T2">The value type of observed property 2.</typeparam>
/// <typeparam name="T3">The value type of observed property 3.</typeparam>
/// <typeparam name="TResult">The type produced by the selector.</typeparam>
/// <param name="source1">Source observable 1.</param>
/// <param name="source2">Source observable 2.</param>
/// <param name="source3">Source observable 3.</param>
/// <param name="selector">Combines the ready values into a result.</param>
internal sealed class WhenAnyValueSink<TSender, T1, T2, T3, TResult>(
    IObservable<IObservedChange<TSender, T1>> source1,
    IObservable<IObservedChange<TSender, T2>> source2,
    IObservable<IObservedChange<TSender, T3>> source3,
    Func<T1, T2, T3, TResult> selector) : IObservable<TResult>
{
    /// <inheritdoc/>
    public IDisposable Subscribe(IObserver<TResult> observer)
    {
        ArgumentExceptionHelper.ThrowIfNull(observer);
        var sink = new Sink(observer, selector);
        sink.Run(source1, source2, source3);
        return sink;
    }

    /// <summary>Captures the latest value of each source under a single gate and emits the selector result once every source is ready.</summary>
    /// <param name="downstream">The observer receiving the combined results.</param>
    /// <param name="selector">Combines the ready values into a result.</param>
    private sealed class Sink(IObserver<TResult> downstream, Func<T1, T2, T3, TResult> selector) : IDisposable
    {
#if NET9_0_OR_GREATER
        /// <summary>Serializes value capture and emission across the sources.</summary>
        private readonly Lock _gate = new();
#else
        /// <summary>Serializes value capture and emission across the sources.</summary>
        private readonly object _gate = new();
#endif

        /// <summary>The per-source subscriptions, disposed together on teardown.</summary>
        private readonly IDisposable?[] _subscriptions = new IDisposable?[3];

        /// <summary>The latest value captured from source 1.</summary>
        private T1 _value1 = default!;

        /// <summary>The latest value captured from source 2.</summary>
        private T2 _value2 = default!;

        /// <summary>The latest value captured from source 3.</summary>
        private T3 _value3 = default!;

        /// <summary>Whether source 1 has produced a value yet.</summary>
        private bool _has1;

        /// <summary>Whether source 2 has produced a value yet.</summary>
        private bool _has2;

        /// <summary>Whether source 3 has produced a value yet.</summary>
        private bool _has3;

        /// <summary>The number of sources that have not yet completed.</summary>
        private int _active = 3;

        /// <summary>Subscribes to every source, wiring each notification back into the sink.</summary>
        /// <param name="source1">Source observable 1.</param>
        /// <param name="source2">Source observable 2.</param>
        /// <param name="source3">Source observable 3.</param>
        public void Run(IObservable<IObservedChange<TSender, T1>> source1, IObservable<IObservedChange<TSender, T2>> source2, IObservable<IObservedChange<TSender, T3>> source3)
        {
            var i = 0;
            _subscriptions[i++] = source1.Subscribe(new DelegateObserver<IObservedChange<TSender, T1>>(On1, OnError, OnSourceCompleted));
            _subscriptions[i++] = source2.Subscribe(new DelegateObserver<IObservedChange<TSender, T2>>(On2, OnError, OnSourceCompleted));
            _subscriptions[i++] = source3.Subscribe(new DelegateObserver<IObservedChange<TSender, T3>>(On3, OnError, OnSourceCompleted));
        }

        /// <summary>Captures the value from source 1 and emits when every source is ready.</summary>
        /// <param name="change">The notification from source 1.</param>
        public void On1(IObservedChange<TSender, T1> change)
        {
            lock (_gate)
            {
                _value1 = change.Value;
                _has1 = true;
                if (!(_has2 && _has3))
                {
                    return;
                }

                Emit();
            }
        }

        /// <summary>Captures the value from source 2 and emits when every source is ready.</summary>
        /// <param name="change">The notification from source 2.</param>
        public void On2(IObservedChange<TSender, T2> change)
        {
            lock (_gate)
            {
                _value2 = change.Value;
                _has2 = true;
                if (!(_has1 && _has3))
                {
                    return;
                }

                Emit();
            }
        }

        /// <summary>Captures the value from source 3 and emits when every source is ready.</summary>
        /// <param name="change">The notification from source 3.</param>
        public void On3(IObservedChange<TSender, T3> change)
        {
            lock (_gate)
            {
                _value3 = change.Value;
                _has3 = true;
                if (!(_has1 && _has2))
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
                result = selector(_value1, _value2, _value3);
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
