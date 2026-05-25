// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Helpers;

namespace ReactiveUI.Internal;

/// <summary>
/// Combines the latest values of two sources through a selector, emitting once both have produced a value and on
/// every subsequent change — a fused, allocation-light replacement for <c>CombineLatest(other, selector)</c>. It
/// holds the latest value of each source under a single gate and tears both subscriptions down on dispose.
/// </summary>
/// <typeparam name="TFirst">The first source's element type.</typeparam>
/// <typeparam name="TSecond">The second source's element type.</typeparam>
/// <typeparam name="TResult">The combined result type.</typeparam>
/// <param name="first">The first source.</param>
/// <param name="second">The second source.</param>
/// <param name="selector">Projects the latest pair of values into a result.</param>
internal sealed class CombineLatestObservable<TFirst, TSecond, TResult>(
    IObservable<TFirst> first,
    IObservable<TSecond> second,
    Func<TFirst, TSecond, TResult> selector) : IObservable<TResult>
{
    /// <inheritdoc/>
    public IDisposable Subscribe(IObserver<TResult> observer)
    {
        ArgumentExceptionHelper.ThrowIfNull(observer);

        var sink = new Sink(observer, selector);
        sink.Run(first, second);
        return sink;
    }

    /// <summary>Tracks the latest value of each source and forwards the combined result.</summary>
    /// <param name="downstream">The downstream observer.</param>
    /// <param name="selector">Projects the latest pair into a result.</param>
    private sealed class Sink(IObserver<TResult> downstream, Func<TFirst, TSecond, TResult> selector) : IDisposable
    {
        /// <summary>Serializes value tracking and emission across the two sources.</summary>
#if NET9_0_OR_GREATER
        private readonly Lock _gate = new();
#else
        private readonly object _gate = new();
#endif

        /// <summary>The latest value from the first source (valid once <see cref="_hasFirst"/> is set).</summary>
        private TFirst _first = default!;

        /// <summary>The latest value from the second source (valid once <see cref="_hasSecond"/> is set).</summary>
        private TSecond _second = default!;

        /// <summary>Whether the first source has produced a value.</summary>
        private bool _hasFirst;

        /// <summary>Whether the second source has produced a value.</summary>
        private bool _hasSecond;

        /// <summary>The first source subscription, torn down on dispose.</summary>
        private IDisposable? _firstSubscription;

        /// <summary>The second source subscription, torn down on dispose.</summary>
        private IDisposable? _secondSubscription;

        /// <summary>Subscribes to both sources.</summary>
        /// <param name="first">The first source.</param>
        /// <param name="second">The second source.</param>
        public void Run(IObservable<TFirst> first, IObservable<TSecond> second)
        {
            _firstSubscription = first.Subscribe(new FirstObserver(this));
            _secondSubscription = second.Subscribe(new SecondObserver(this));
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            _firstSubscription?.Dispose();
            _secondSubscription?.Dispose();
        }

        /// <summary>Records the latest first value and emits if both sources have produced one.</summary>
        /// <param name="value">The first source value.</param>
        private void OnFirst(TFirst value)
        {
            lock (_gate)
            {
                _first = value;
                _hasFirst = true;
                Emit();
            }
        }

        /// <summary>Records the latest second value and emits if both sources have produced one.</summary>
        /// <param name="value">The second source value.</param>
        private void OnSecond(TSecond value)
        {
            lock (_gate)
            {
                _second = value;
                _hasSecond = true;
                Emit();
            }
        }

        /// <summary>Emits the combined value when both sources have produced one. Caller holds the gate.</summary>
        private void Emit()
        {
            if (!_hasFirst || !_hasSecond)
            {
                return;
            }

            downstream.OnNext(selector(_first, _second));
        }

        /// <summary>Forwards an error downstream.</summary>
        /// <param name="error">The error.</param>
        private void OnError(Exception error)
        {
            lock (_gate)
            {
                downstream.OnError(error);
            }
        }

        /// <summary>Observes the first source.</summary>
        /// <param name="parent">The owning sink.</param>
        private sealed class FirstObserver(Sink parent) : IObserver<TFirst>
        {
            /// <inheritdoc/>
            public void OnNext(TFirst value) => parent.OnFirst(value);

            /// <inheritdoc/>
            public void OnError(Exception error) => parent.OnError(error);

            /// <inheritdoc/>
            public void OnCompleted()
            {
            }
        }

        /// <summary>Observes the second source.</summary>
        /// <param name="parent">The owning sink.</param>
        private sealed class SecondObserver(Sink parent) : IObserver<TSecond>
        {
            /// <inheritdoc/>
            public void OnNext(TSecond value) => parent.OnSecond(value);

            /// <inheritdoc/>
            public void OnError(Exception error) => parent.OnError(error);

            /// <inheritdoc/>
            public void OnCompleted()
            {
            }
        }
    }
}
