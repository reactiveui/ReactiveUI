// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive.Concurrency;

namespace ReactiveUI.Internal;

/// <summary>
/// Forwards each notification to a downstream observer on a scheduler, replacing a per-observer <c>ObserveOn</c>.
/// </summary>
/// <typeparam name="T">The element type.</typeparam>
/// <param name="downstream">The observer that receives the scheduled notifications.</param>
/// <param name="scheduler">The scheduler each notification is delivered on.</param>
internal sealed class SchedulingObserver<T>(IObserver<T> downstream, IScheduler scheduler) : IObserver<T>
{
    /// <inheritdoc/>
    public void OnNext(T value) =>
        scheduler.ScheduleOrInline(
            (Observer: downstream, Value: value),
            static (_, state) =>
            {
                state.Observer.OnNext(state.Value);
                return EmptyDisposable.Instance;
            });

    /// <inheritdoc/>
    public void OnError(Exception error) =>
        scheduler.ScheduleOrInline(
            (Observer: downstream, Error: error),
            static (_, state) =>
            {
                state.Observer.OnError(state.Error);
                return EmptyDisposable.Instance;
            });

    /// <inheritdoc/>
    public void OnCompleted() =>
        scheduler.ScheduleOrInline(
            downstream,
            static (_, observer) =>
            {
                observer.OnCompleted();
                return EmptyDisposable.Instance;
            });
}
