// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive;

namespace ReactiveUI.Internal;

/// <summary>
/// Bridges a non-generic <see cref="Task"/> to an observable: emits <see cref="Unit.Default"/> and
/// completes when the task completes, or forwards its fault / cancellation as an error. Replaces
/// <c>task.ToObservable()</c> for internal use.
/// </summary>
/// <param name="task">The task to observe.</param>
internal sealed class TaskUnitObservable(Task task) : IObservable<Unit>
{
    /// <inheritdoc/>
    public IDisposable Subscribe(IObserver<Unit> observer)
    {
        ArgumentExceptionHelper.ThrowIfNull(observer);
        task.ContinueWith(
            static (completed, state) =>
            {
                var observer = (IObserver<Unit>)state!;
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
                    observer.OnNext(Unit.Default);
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
