// Copyright (c) 2022 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ReactiveUI;

/// <summary>
/// Signals.
/// </summary>
public static class Signal
{
    /// <summary>
    /// Froms the asynchronous.
    /// </summary>
    /// <typeparam name="T">The type of the return value.</typeparam>
    /// <param name="actionAsync">The action asynchronous.</param>
    /// <param name="scheduler">The scheduler.</param>
    /// <returns>An Observable of T.</returns>
#pragma warning disable RCS1047 // Non-asynchronous method name should not end with 'Async'.
    public static IObservable<T> FromAsync<T>(Func<CancellationToken, Task> actionAsync, IScheduler? scheduler = null) =>
        FromValue(ct => RunCancellationTask<T>(actionAsync, ct), scheduler);
#pragma warning restore RCS1047 // Non-asynchronous method name should not end with 'Async'.

    /// <summary>
    /// Froms the asynchronous.
    /// </summary>
    /// <typeparam name="T">The type of the return value.</typeparam>
    /// <param name="actionAsync">The action asynchronous.</param>
    /// <param name="scheduler">The scheduler.</param>
    /// <returns>An Observable of T.</returns>
    public static IObservable<T> FromValue<T>(Func<CancellationToken, T> actionAsync, IScheduler? scheduler = null)
    {
        var s = scheduler ?? CurrentThreadScheduler.Instance;
        return Observable.Defer(() => Observable.Create<T>(
            async obs =>
            {
                // CancelationToken
                var src = new CancellationTokenSource();
                var ct = src.Token;

#pragma warning disable CA2008 // Do not create tasks without passing a TaskScheduler
                var task = Task.Factory.StartNew(() => actionAsync(ct), ct);
#pragma warning restore CA2008 // Do not create tasks without passing a TaskScheduler
                try
                {
                    var result = await task.WhenCancelled(ct);
                    if (result != null)
                    {
                        obs.OnNext(result);
                        obs.OnCompleted();
                    }
                }
                catch (Exception ex)
                {
                    // Catch the exception and pass it to the observer if not user handled.
                    obs.OnError(ex);
                }

                return Disposable.Create(() =>
                {
                    ct.ThrowIfCancellationRequested();
                    src.Cancel();
                    src.Dispose();
                });
            })).ObserveOn(s);
    }

    /// <summary>
    /// Froms the asynchronous.
    /// </summary>
    /// <typeparam name="T">The type of the return value.</typeparam>
    /// <param name="actionAsync">The action asynchronous.</param>
    /// <param name="scheduler">The scheduler.</param>
    /// <returns>An Observable of T.</returns>
    public static IObservable<T> FromValue<T>(Action<CancellationToken> actionAsync, IScheduler? scheduler = null)
    {
        var s = scheduler ?? CurrentThreadScheduler.Instance;
        return Observable.Defer(() => Observable.Create<T>(
            async obs =>
            {
                // CancelationToken
                var src = new CancellationTokenSource();
                var ct = src.Token;

#pragma warning disable CA2008 // Do not create tasks without passing a TaskScheduler
                var task = Task.Factory.StartNew<T>(
                    () =>
                    {
                        actionAsync(ct);

                        return default!;
                    },
                    ct);
#pragma warning restore CA2008 // Do not create tasks without passing a TaskScheduler

                try
                {
                    var result = await task.WhenCancelled(ct);
                    if (result != null)
                    {
                        obs.OnNext(result);
                    }

                    obs.OnCompleted();
                }
                catch (Exception ex)
                {
                    // Catch the exception and pass it to the observer if not user handled.
                    obs.OnError(ex);
                }

                return Disposable.Create(() =>
                {
                    ct.ThrowIfCancellationRequested();
                    src.Cancel();
                    src.Dispose();
                });
            }).ObserveOn(s));
    }

    /// <summary>
    /// Runs from asynchronous.
    /// </summary>
    /// <typeparam name="T">The return type.</typeparam>
    /// <param name="execute">The execute.</param>
    /// <param name="ct">The ct.</param>
    /// <returns>A instance of type T.</returns>
    public static T RunCancellationTask<T>(Func<CancellationToken, Task> execute, CancellationToken ct)
    {
#pragma warning disable CA2008 // Do not create tasks without passing a TaskScheduler
        var tsk = Task.Factory.StartNew<T>(
            () =>
            {
                var task = execute(ct);
                task.HandleCancellation().Wait();

                return default!;
            },
            ct);
#pragma warning restore CA2008 // Do not create tasks without passing a TaskScheduler
        try
        {
            tsk.Wait(ct);
            return tsk.Result;
        }
        catch (Exception ex)
        {
            if (ex is AggregateException ae)
            {
                ThrowExceptions(ae);
            }

            throw;
        }
    }

    /// <summary>
    /// Handles the cancellation.
    /// </summary>
    /// <param name="asyncTask">The asynchronous task.</param>
    /// <param name="action">The action.</param>
    /// <returns>A Task.</returns>
    public static async Task HandleCancellation(this Task asyncTask, Action? action = null)
    {
        try
        {
            await asyncTask;
        }
        catch (OperationCanceledException)
        {
            action?.Invoke();
        }
    }

    /// <summary>
    /// Handles the cancellation.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="asyncTask">The asynchronous task.</param>
    /// <param name="action">The action.</param>
    /// <returns>A Task of TResult.</returns>
    public static async Task<TResult?> HandleCancellation<TResult>(this Task<TResult> asyncTask, Action action)
    {
        try
        {
            if (asyncTask?.IsCanceled == false && !asyncTask.IsFaulted && !asyncTask.IsCompleted)
            {
                return await asyncTask;
            }

            action?.Invoke();
        }
        catch (OperationCanceledException)
        {
            action?.Invoke();
        }

        return default;
    }

    private static void ThrowExceptions(AggregateException ae)
    {
        foreach (var innerEx in ae.InnerExceptions.Select(e => new Exception(e.Message, e)))
        {
            throw innerEx;
        }
    }

    private static async Task<TResult> WhenCancelled<TResult>(this Task<TResult> asyncTask, CancellationToken cancellationToken)
    {
        var tcs = new TaskCompletionSource<TResult>();
        cancellationToken.Register(() => tcs.TrySetCanceled(), useSynchronizationContext: false);
        var cancellationTask = tcs.Task;

        // Create a task that completes when either the async operation completes,
        // or cancellation is requested.
        var readyTask = await Task.WhenAny(asyncTask, cancellationTask);

        // In case of cancellation, register a continuation to observe any unhandled.
        // exceptions from the asynchronous operation (once it completes).
        if (readyTask == cancellationTask)
        {
#pragma warning disable CA2008 // Do not create tasks without passing a TaskScheduler
            await asyncTask.ContinueWith(_ => asyncTask.Exception, TaskContinuationOptions.OnlyOnFaulted | TaskContinuationOptions.ExecuteSynchronously);
#pragma warning restore CA2008 // Do not create tasks without passing a TaskScheduler
        }

        return await readyTask;
    }
}
