// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Runtime.CompilerServices;

namespace ReactiveUI.Internal;

/// <summary>
/// Bridges a <see cref="Task"/> to an observer: the task's outcome completes the sequence, and a fault or a
/// cancellation is forwarded as an error. Keeps the continuation wiring (synchronous, on the default scheduler)
/// and the fault-unwrapping policy in one place for the task-backed observables.
/// </summary>
internal static class TaskObserverBridge
{
    /// <summary>Forwards the result of <paramref name="task"/> to <paramref name="observer"/>.</summary>
    /// <typeparam name="T">The task's result type.</typeparam>
    /// <param name="task">The task to observe.</param>
    /// <param name="observer">The observer receiving the task's outcome.</param>
    /// <returns>A disposable that does nothing: the continuation is already scheduled and cannot be unhooked.</returns>
    internal static IDisposable Subscribe<T>(Task<T> task, IObserver<T> observer)
    {
        ArgumentExceptionHelper.ThrowIfNull(observer);
        _ = task.ContinueWith(
            static (completed, state) =>
            {
                var target = (IObserver<T>)state!;

                // "Ran to completion" spelled out because .NET Framework has no IsCompletedSuccessfully. The
                // continuation only runs on a completed task, so reading Result here never blocks.
                if (completed.IsCompleted && !completed.IsFaulted && !completed.IsCanceled)
                {
                    target.OnNext(completed.Result);
                    target.OnCompleted();
                    return;
                }

                ForwardFailure(completed, target);
            },
            observer,
            CancellationToken.None,
            TaskContinuationOptions.ExecuteSynchronously,
            TaskScheduler.Default);
        return EmptyDisposable.Instance;
    }

    /// <summary>Forwards the completion of a result-less <paramref name="task"/> as a single <paramref name="completionValue"/>.</summary>
    /// <typeparam name="T">The observed element type.</typeparam>
    /// <param name="task">The task to observe.</param>
    /// <param name="observer">The observer receiving the task's outcome.</param>
    /// <param name="completionValue">The value emitted when the task completes successfully.</param>
    /// <returns>A disposable that does nothing: the continuation is already scheduled and cannot be unhooked.</returns>
    internal static IDisposable Subscribe<T>(Task task, IObserver<T> observer, T completionValue)
    {
        ArgumentExceptionHelper.ThrowIfNull(observer);
        _ = task.ContinueWith(
            static (completed, state) =>
            {
                var (target, value) = ((IObserver<T> Observer, T Value))state!;
                if (completed.IsCompleted && !completed.IsFaulted && !completed.IsCanceled)
                {
                    target.OnNext(value);
                    target.OnCompleted();
                    return;
                }

                ForwardFailure(completed, target);
            },
            (Observer: observer, Value: completionValue),
            CancellationToken.None,
            TaskContinuationOptions.ExecuteSynchronously,
            TaskScheduler.Default);
        return EmptyDisposable.Instance;
    }

    /// <summary>Reports a faulted or cancelled task to <paramref name="observer"/> as an error.</summary>
    /// <typeparam name="T">The observed element type.</typeparam>
    /// <param name="completed">The failed task; faulted or cancelled.</param>
    /// <param name="observer">The observer receiving the error.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void ForwardFailure<T>(Task completed, IObserver<T> observer) =>
        observer.OnError(completed.IsFaulted
            ? completed.Exception?.InnerException ?? completed.Exception!
            : new TaskCanceledException(completed));
}
