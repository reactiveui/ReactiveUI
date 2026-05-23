// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive.Concurrency;

namespace ReactiveUI.Internal;

/// <summary>
/// Runs a function on a scheduler when subscribed, then emits its result and completes.
/// Replaces <c>Observable.Start</c> for internal use.
/// </summary>
/// <typeparam name="T">The result type.</typeparam>
/// <param name="function">The function to run on the scheduler.</param>
/// <param name="scheduler">The scheduler the function runs on.</param>
internal sealed class StartObservable<T>(Func<T> function, IScheduler scheduler) : IObservable<T>
{
    /// <inheritdoc/>
    public IDisposable Subscribe(IObserver<T> observer)
    {
        ArgumentExceptionHelper.ThrowIfNull(observer);
        return scheduler.Schedule(
            (Function: function, Observer: observer),
            static (_, state) =>
            {
                try
                {
                    var result = state.Function();
                    state.Observer.OnNext(result);
                    state.Observer.OnCompleted();
                }
                catch (Exception ex)
                {
                    state.Observer.OnError(ex);
                }

                return EmptyDisposable.Instance;
            });
    }
}
