// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Primitives.Disposables;

namespace ReactiveUI.Internal;

/// <summary>
/// Bridges a <see cref="Task{TResult}"/> to an observable: emits the task's result and completes,
/// or forwards its fault / cancellation as an error. Replaces <c>task.ToObservable()</c> for internal use.
/// </summary>
/// <typeparam name="T">The task's result type.</typeparam>
/// <param name="task">The task to observe.</param>
public sealed class TaskObservable<T>(Task<T> task) : IObservable<T>
{
    /// <inheritdoc/>
    public IDisposable Subscribe(IObserver<T> observer)
    {
        ArgumentExceptionHelper.ThrowIfNull(observer);
        task.ContinueWith(
            static (completed, state) =>
            {
                var observer = (IObserver<T>)state!;
                if (completed.IsFaulted)
                {
                    observer.OnError(completed.Exception?.InnerException ?? completed.Exception!);
                }
                else if (completed.IsCanceled)
                {
                    observer.OnError(new TaskCanceledException(completed));
                }
                else
                {
                    observer.OnNext(completed.Result);
                    observer.OnCompleted();
                }
            },
            observer,
            CancellationToken.None,
            TaskContinuationOptions.ExecuteSynchronously,
            TaskScheduler.Default);
        return EmptyDisposable.Instance;
    }
}
