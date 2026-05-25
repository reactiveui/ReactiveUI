// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Helpers;

namespace ReactiveUI.Internal;

/// <summary>
/// A sink that combines the latest value from two sources.
/// </summary>
/// <typeparam name="TFirst">The first source's element type.</typeparam>
/// <typeparam name="TSecond">The second source's element type.</typeparam>
internal sealed class CombineLatestSink<TFirst, TSecond> : IDisposable
{
    /// <summary>The callback invoked with the latest pair once both sources have produced a value.</summary>
    private readonly Action<TFirst, TSecond> _onNext;

    /// <summary>The optional error callback, invoked when either source errors.</summary>
    private readonly Action<Exception>? _onError;

    /// <summary>Serializes value tracking and emission across the two sources.</summary>
#if NET9_0_OR_GREATER
    private readonly Lock _gate = new();
#else
    private readonly object _gate = new();
#endif

    /// <summary>The first source subscription, torn down on dispose.</summary>
    private readonly IDisposable _firstSubscription;

    /// <summary>The second source subscription, torn down on dispose.</summary>
    private readonly IDisposable _secondSubscription;

    /// <summary>The latest value from the first source (valid once <see cref="_hasFirst"/> is set).</summary>
    private TFirst _first = default!;

    /// <summary>The latest value from the second source (valid once <see cref="_hasSecond"/> is set).</summary>
    private TSecond _second = default!;

    /// <summary>Whether the first source has produced a value.</summary>
    private bool _hasFirst;

    /// <summary>Whether the second source has produced a value.</summary>
    private bool _hasSecond;

    /// <summary>Initializes a new instance of the <see cref="CombineLatestSink{TFirst, TSecond}"/> class and subscribes both sources.</summary>
    /// <param name="first">The first source.</param>
    /// <param name="second">The second source.</param>
    /// <param name="onNext">Invoked with the latest pair once both sources have produced a value, and on every subsequent change.</param>
    /// <param name="onError">Optional callback invoked when either source errors.</param>
    public CombineLatestSink(
        IObservable<TFirst> first,
        IObservable<TSecond> second,
        Action<TFirst, TSecond> onNext,
        Action<Exception>? onError = null)
    {
        ArgumentExceptionHelper.ThrowIfNull(first);
        ArgumentExceptionHelper.ThrowIfNull(second);
        ArgumentExceptionHelper.ThrowIfNull(onNext);

        _onNext = onNext;
        _onError = onError;
        _firstSubscription = first.Subscribe(new FirstObserver(this));
        _secondSubscription = second.Subscribe(new SecondObserver(this));
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        _firstSubscription.Dispose();
        _secondSubscription.Dispose();
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

    /// <summary>Invokes the callback when both sources have produced a value. Caller holds the gate.</summary>
    private void Emit()
    {
        if (!_hasFirst || !_hasSecond)
        {
            return;
        }

        _onNext(_first, _second);
    }

    /// <summary>Forwards an error to the error callback.</summary>
    /// <param name="error">The error.</param>
    private void OnError(Exception error)
    {
        lock (_gate)
        {
            _onError?.Invoke(error);
        }
    }

    /// <summary>Observes the first source.</summary>
    /// <param name="parent">The owning sink.</param>
    private sealed class FirstObserver(CombineLatestSink<TFirst, TSecond> parent) : IObserver<TFirst>
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
    private sealed class SecondObserver(CombineLatestSink<TFirst, TSecond> parent) : IObserver<TSecond>
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
