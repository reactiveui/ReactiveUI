// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive.Internal;
#else
namespace ReactiveUI.Internal;
#endif
/// <summary>
/// Combines the latest of 12 sources, emitting the selector result once every source
/// has produced a value. Bails on a plain <c>if</c> when peers are not ready.
/// </summary>
/// <typeparam name="T1">The input type of source 1.</typeparam>
/// <typeparam name="T2">The input type of source 2.</typeparam>
/// <typeparam name="T3">The input type of source 3.</typeparam>
/// <typeparam name="T4">The input type of source 4.</typeparam>
/// <typeparam name="T5">The input type of source 5.</typeparam>
/// <typeparam name="T6">The input type of source 6.</typeparam>
/// <typeparam name="T7">The input type of source 7.</typeparam>
/// <typeparam name="T8">The input type of source 8.</typeparam>
/// <typeparam name="T9">The input type of source 9.</typeparam>
/// <typeparam name="T10">The input type of source 10.</typeparam>
/// <typeparam name="T11">The input type of source 11.</typeparam>
/// <typeparam name="T12">The input type of source 12.</typeparam>
/// <typeparam name="TResult">The type produced by the selector.</typeparam>
/// <param name="source1">Source observable 1.</param>
/// <param name="source2">Source observable 2.</param>
/// <param name="source3">Source observable 3.</param>
/// <param name="source4">Source observable 4.</param>
/// <param name="source5">Source observable 5.</param>
/// <param name="source6">Source observable 6.</param>
/// <param name="source7">Source observable 7.</param>
/// <param name="source8">Source observable 8.</param>
/// <param name="source9">Source observable 9.</param>
/// <param name="source10">Source observable 10.</param>
/// <param name="source11">Source observable 11.</param>
/// <param name="source12">Source observable 12.</param>
/// <param name="selector">Combines the ready values into a result.</param>
[SuppressMessage("Major Code Smell", "S107:Methods should not have too many parameters", Justification = "Parameter count is inherent to the arity of this WhenAny sink.")]
internal sealed class WhenAnyChangeSink<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>(
    IObservable<T1> source1,
    IObservable<T2> source2,
    IObservable<T3> source3,
    IObservable<T4> source4,
    IObservable<T5> source5,
    IObservable<T6> source6,
    IObservable<T7> source7,
    IObservable<T8> source8,
    IObservable<T9> source9,
    IObservable<T10> source10,
    IObservable<T11> source11,
    IObservable<T12> source12,
    Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult> selector) : IObservable<TResult>
{
    /// <inheritdoc/>
    public IDisposable Subscribe(IObserver<TResult> observer)
    {
        ArgumentExceptionHelper.ThrowIfNull(observer);
        var sink = new Sink(observer, selector);
        sink.Run(source1, source2, source3, source4, source5, source6, source7, source8, source9, source10, source11, source12);
        return sink;
    }

    /// <summary>Captures the latest value of each source under a single gate and emits the selector result once every source is ready.</summary>
    /// <param name="downstream">The observer receiving the combined results.</param>
    /// <param name="selector">Combines the ready values into a result.</param>
    [SuppressMessage("Major Code Smell", "S107:Methods should not have too many parameters", Justification = "Parameter count is inherent to the arity of this WhenAny sink.")]
    [SuppressMessage("Major Code Smell", "S1541:Methods and properties should not be too complex", Justification = "Cyclomatic complexity is inherent to the arity of this WhenAny sink.")]
    private sealed class Sink(IObserver<TResult> downstream, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult> selector) : IDisposable
    {
#if NET9_0_OR_GREATER
        /// <summary>Serializes value capture and emission across the sources.</summary>
        private readonly Lock _gate = new();
#else
        /// <summary>Serializes value capture and emission across the sources.</summary>
        private readonly object _gate = new();
#endif

        /// <summary>The per-source subscriptions, disposed together on teardown.</summary>
        private readonly IDisposable?[] _subscriptions = new IDisposable?[12];

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

        /// <summary>The latest value captured from source 6.</summary>
        private T6 _value6 = default!;

        /// <summary>The latest value captured from source 7.</summary>
        private T7 _value7 = default!;

        /// <summary>The latest value captured from source 8.</summary>
        private T8 _value8 = default!;

        /// <summary>The latest value captured from source 9.</summary>
        private T9 _value9 = default!;

        /// <summary>The latest value captured from source 10.</summary>
        private T10 _value10 = default!;

        /// <summary>The latest value captured from source 11.</summary>
        private T11 _value11 = default!;

        /// <summary>The latest value captured from source 12.</summary>
        private T12 _value12 = default!;

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

        /// <summary>Whether source 6 has produced a value yet.</summary>
        private bool _has6;

        /// <summary>Whether source 7 has produced a value yet.</summary>
        private bool _has7;

        /// <summary>Whether source 8 has produced a value yet.</summary>
        private bool _has8;

        /// <summary>Whether source 9 has produced a value yet.</summary>
        private bool _has9;

        /// <summary>Whether source 10 has produced a value yet.</summary>
        private bool _has10;

        /// <summary>Whether source 11 has produced a value yet.</summary>
        private bool _has11;

        /// <summary>Whether source 12 has produced a value yet.</summary>
        private bool _has12;

        /// <summary>The number of sources that have not yet completed.</summary>
        private int _active = 12;

        /// <summary>Subscribes to every source, wiring each notification back into the sink.</summary>
        /// <param name="source1">Source observable 1.</param>
        /// <param name="source2">Source observable 2.</param>
        /// <param name="source3">Source observable 3.</param>
        /// <param name="source4">Source observable 4.</param>
        /// <param name="source5">Source observable 5.</param>
        /// <param name="source6">Source observable 6.</param>
        /// <param name="source7">Source observable 7.</param>
        /// <param name="source8">Source observable 8.</param>
        /// <param name="source9">Source observable 9.</param>
        /// <param name="source10">Source observable 10.</param>
        /// <param name="source11">Source observable 11.</param>
        /// <param name="source12">Source observable 12.</param>
        public void Run(
            IObservable<T1> source1,
            IObservable<T2> source2,
            IObservable<T3> source3,
            IObservable<T4> source4,
            IObservable<T5> source5,
            IObservable<T6> source6,
            IObservable<T7> source7,
            IObservable<T8> source8,
            IObservable<T9> source9,
            IObservable<T10> source10,
            IObservable<T11> source11,
            IObservable<T12> source12)
        {
            var i = 0;
            _subscriptions[i++] = source1.Subscribe(new DelegateObserver<T1>(On1, OnError, OnSourceCompleted));
            _subscriptions[i++] = source2.Subscribe(new DelegateObserver<T2>(On2, OnError, OnSourceCompleted));
            _subscriptions[i++] = source3.Subscribe(new DelegateObserver<T3>(On3, OnError, OnSourceCompleted));
            _subscriptions[i++] = source4.Subscribe(new DelegateObserver<T4>(On4, OnError, OnSourceCompleted));
            _subscriptions[i++] = source5.Subscribe(new DelegateObserver<T5>(On5, OnError, OnSourceCompleted));
            _subscriptions[i++] = source6.Subscribe(new DelegateObserver<T6>(On6, OnError, OnSourceCompleted));
            _subscriptions[i++] = source7.Subscribe(new DelegateObserver<T7>(On7, OnError, OnSourceCompleted));
            _subscriptions[i++] = source8.Subscribe(new DelegateObserver<T8>(On8, OnError, OnSourceCompleted));
            _subscriptions[i++] = source9.Subscribe(new DelegateObserver<T9>(On9, OnError, OnSourceCompleted));
            _subscriptions[i++] = source10.Subscribe(new DelegateObserver<T10>(On10, OnError, OnSourceCompleted));
            _subscriptions[i++] = source11.Subscribe(new DelegateObserver<T11>(On11, OnError, OnSourceCompleted));
            _subscriptions[i++] = source12.Subscribe(new DelegateObserver<T12>(On12, OnError, OnSourceCompleted));
        }

        /// <summary>Captures the value from source 1 and emits when every source is ready.</summary>
        /// <param name="change">The notification from source 1.</param>
        public void On1(T1 change)
        {
            lock (_gate)
            {
                _value1 = change;
                _has1 = true;
                if (!(_has2 && _has3 && _has4 && _has5 && _has6 && _has7 && _has8 && _has9 && _has10 && _has11 && _has12))
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
                if (!(_has1 && _has3 && _has4 && _has5 && _has6 && _has7 && _has8 && _has9 && _has10 && _has11 && _has12))
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
                if (!(_has1 && _has2 && _has4 && _has5 && _has6 && _has7 && _has8 && _has9 && _has10 && _has11 && _has12))
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
                if (!(_has1 && _has2 && _has3 && _has5 && _has6 && _has7 && _has8 && _has9 && _has10 && _has11 && _has12))
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
                if (!(_has1 && _has2 && _has3 && _has4 && _has6 && _has7 && _has8 && _has9 && _has10 && _has11 && _has12))
                {
                    return;
                }

                Emit();
            }
        }

        /// <summary>Captures the value from source 6 and emits when every source is ready.</summary>
        /// <param name="change">The notification from source 6.</param>
        public void On6(T6 change)
        {
            lock (_gate)
            {
                _value6 = change;
                _has6 = true;
                if (!(_has1 && _has2 && _has3 && _has4 && _has5 && _has7 && _has8 && _has9 && _has10 && _has11 && _has12))
                {
                    return;
                }

                Emit();
            }
        }

        /// <summary>Captures the value from source 7 and emits when every source is ready.</summary>
        /// <param name="change">The notification from source 7.</param>
        public void On7(T7 change)
        {
            lock (_gate)
            {
                _value7 = change;
                _has7 = true;
                if (!(_has1 && _has2 && _has3 && _has4 && _has5 && _has6 && _has8 && _has9 && _has10 && _has11 && _has12))
                {
                    return;
                }

                Emit();
            }
        }

        /// <summary>Captures the value from source 8 and emits when every source is ready.</summary>
        /// <param name="change">The notification from source 8.</param>
        public void On8(T8 change)
        {
            lock (_gate)
            {
                _value8 = change;
                _has8 = true;
                if (!(_has1 && _has2 && _has3 && _has4 && _has5 && _has6 && _has7 && _has9 && _has10 && _has11 && _has12))
                {
                    return;
                }

                Emit();
            }
        }

        /// <summary>Captures the value from source 9 and emits when every source is ready.</summary>
        /// <param name="change">The notification from source 9.</param>
        public void On9(T9 change)
        {
            lock (_gate)
            {
                _value9 = change;
                _has9 = true;
                if (!(_has1 && _has2 && _has3 && _has4 && _has5 && _has6 && _has7 && _has8 && _has10 && _has11 && _has12))
                {
                    return;
                }

                Emit();
            }
        }

        /// <summary>Captures the value from source 10 and emits when every source is ready.</summary>
        /// <param name="change">The notification from source 10.</param>
        public void On10(T10 change)
        {
            lock (_gate)
            {
                _value10 = change;
                _has10 = true;
                if (!(_has1 && _has2 && _has3 && _has4 && _has5 && _has6 && _has7 && _has8 && _has9 && _has11 && _has12))
                {
                    return;
                }

                Emit();
            }
        }

        /// <summary>Captures the value from source 11 and emits when every source is ready.</summary>
        /// <param name="change">The notification from source 11.</param>
        public void On11(T11 change)
        {
            lock (_gate)
            {
                _value11 = change;
                _has11 = true;
                if (!(_has1 && _has2 && _has3 && _has4 && _has5 && _has6 && _has7 && _has8 && _has9 && _has10 && _has12))
                {
                    return;
                }

                Emit();
            }
        }

        /// <summary>Captures the value from source 12 and emits when every source is ready.</summary>
        /// <param name="change">The notification from source 12.</param>
        public void On12(T12 change)
        {
            lock (_gate)
            {
                _value12 = change;
                _has12 = true;
                if (!(_has1 && _has2 && _has3 && _has4 && _has5 && _has6 && _has7 && _has8 && _has9 && _has10 && _has11))
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
                result = selector(_value1, _value2, _value3, _value4, _value5, _value6, _value7, _value8, _value9, _value10, _value11, _value12);
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
