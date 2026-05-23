// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive.Concurrency;
using ReactiveUI.Helpers;
using ReactiveUI.Internal;

namespace ReactiveUI.Blend;

/// <summary>
/// Subscribes to a source observable and marshals each notification to delegate callbacks on a scheduler.
/// </summary>
/// <remarks>
/// A dedicated, allocation-light fusion of <c>ObserveOn(scheduler).Subscribe(onNext, onError)</c> used by the
/// Blend behaviors: the sink is its own observer, schedules each value and error onto the supplied scheduler, and
/// disposes the source subscription on <see cref="Dispose"/> — no operator chain and no intermediate observable.
/// </remarks>
/// <typeparam name="T">The observed element type.</typeparam>
internal sealed class ScheduledObserver<T> : IObserver<T>, IDisposable
{
    /// <summary>The scheduler each notification is delivered on.</summary>
    private readonly IScheduler _scheduler;

    /// <summary>The per-value callback.</summary>
    private readonly Action<T> _onNext;

    /// <summary>The error callback.</summary>
    private readonly Action<Exception> _onError;

    /// <summary>The source subscription, torn down on dispose.</summary>
    private IDisposable? _subscription;

    /// <summary>Initializes a new instance of the <see cref="ScheduledObserver{T}"/> class.</summary>
    /// <param name="scheduler">The scheduler each notification is delivered on.</param>
    /// <param name="onNext">The per-value callback.</param>
    /// <param name="onError">The error callback.</param>
    private ScheduledObserver(IScheduler scheduler, Action<T> onNext, Action<Exception> onError)
    {
        _scheduler = scheduler;
        _onNext = onNext;
        _onError = onError;
    }

    /// <summary>
    /// Subscribes to <paramref name="source"/>, delivering notifications to the callbacks on <paramref name="scheduler"/>.
    /// </summary>
    /// <param name="source">The source observable.</param>
    /// <param name="scheduler">The scheduler each notification is delivered on.</param>
    /// <param name="onNext">The per-value callback.</param>
    /// <param name="onError">The error callback.</param>
    /// <returns>A disposable that tears down the subscription.</returns>
    public static IDisposable Subscribe(
        IObservable<T> source,
        IScheduler scheduler,
        Action<T> onNext,
        Action<Exception> onError)
    {
        ArgumentExceptionHelper.ThrowIfNull(source);

        var observer = new ScheduledObserver<T>(scheduler, onNext, onError);
        observer._subscription = source.Subscribe(observer);
        return observer;
    }

    /// <inheritdoc/>
    public void OnNext(T value) =>
        _scheduler.Schedule(
            (Observer: this, Value: value),
            static (_, state) =>
            {
                state.Observer._onNext(state.Value);
                return EmptyDisposable.Instance;
            });

    /// <inheritdoc/>
    public void OnError(Exception error) =>
        _scheduler.Schedule(
            (Observer: this, Error: error),
            static (_, state) =>
            {
                state.Observer._onError(state.Error);
                return EmptyDisposable.Instance;
            });

    /// <inheritdoc/>
    public void OnCompleted()
    {
    }

    /// <inheritdoc/>
    public void Dispose() => _subscription?.Dispose();
}
