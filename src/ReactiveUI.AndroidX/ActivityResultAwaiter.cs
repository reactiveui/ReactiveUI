// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Runtime.CompilerServices;
using Android.Content;

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive.AndroidX;
#else
namespace ReactiveUI.AndroidX;
#endif

/// <summary>
/// Awaits the first activity result that matches a request code, then completes the task and detaches. A fused,
/// allocation-light replacement for <c>Where(...).Select(...).FirstAsync().ToTask()</c>: it is its own observer,
/// settles exactly once, and unsubscribes on completion. Shared by the AppCompat and Fragment reactive activities,
/// which also route their start-for-result requests through the <c>StartForResultAsync</c> overloads here so the
/// subscribe-before-start ordering is stated once.
/// </summary>
internal sealed class ActivityResultAwaiter : IObserver<(int RequestCode, Result Result, Intent? Intent)>, IDisposable
{
    /// <summary>The request code this awaiter is waiting for.</summary>
    private readonly int _requestCode;

    /// <summary>Completion source backing the returned task.</summary>
    private readonly TaskCompletionSource<(Result Result, Intent? Intent)> _completion =
        new(TaskCreationOptions.RunContinuationsAsynchronously);

    /// <summary>Holds the source subscription so it can be torn down once settled.</summary>
    private readonly OnceDisposable _subscription = new();

    /// <summary>Guards against settling more than once.</summary>
    private int _settled;

    /// <summary>Initializes a new instance of the <see cref="ActivityResultAwaiter"/> class.</summary>
    /// <param name="requestCode">The request code to await.</param>
    private ActivityResultAwaiter(int requestCode) => _requestCode = requestCode;

    /// <inheritdoc/>
    public void OnNext((int RequestCode, Result Result, Intent? Intent) value)
    {
        if (value.RequestCode != _requestCode || !TryClaimSettle())
        {
            return;
        }

        _ = _completion.TrySetResult((value.Result, value.Intent));
        Dispose();
    }

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void OnError(Exception error) => Settle(error);

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void OnCompleted() => Settle(null);

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Dispose() => _subscription.Dispose();

    /// <summary>Subscribes to the activity-result stream and returns a task for the first matching result.</summary>
    /// <param name="source">The activity-result stream.</param>
    /// <param name="requestCode">The request code to await.</param>
    /// <returns>A task that completes with the result and intent of the first matching activity result.</returns>
    internal static Task<(Result Result, Intent? Intent)> Await(
        IObservable<(int RequestCode, Result Result, Intent? Intent)> source,
        int requestCode)
    {
        var awaiter = new ActivityResultAwaiter(requestCode);
        awaiter._subscription.Disposable = source.Subscribe(awaiter);
        return awaiter._completion.Task;
    }

    /// <summary>Starts an activity for a result and returns a task for that result.</summary>
    /// <param name="activity">The activity issuing the request and publishing the result.</param>
    /// <param name="source">The activity-result stream of <paramref name="activity"/>.</param>
    /// <param name="intent">The intent describing the activity to start.</param>
    /// <param name="requestCode">The request code to await.</param>
    /// <returns>A task that completes with the result and intent of the matching activity result.</returns>
    internal static Task<(Result Result, Intent? Intent)> StartForResultAsync(
        Activity activity,
        IObservable<(int RequestCode, Result Result, Intent? Intent)> source,
        Intent intent,
        int requestCode)
    {
        // NB: It's important that we set up the subscription *before* we call ActivityForResult.
        var ret = Await(source, requestCode);
        activity.StartActivityForResult(intent, requestCode);
        return ret;
    }

    /// <summary>Starts an activity for a result and returns a task for that result.</summary>
    /// <param name="activity">The activity issuing the request and publishing the result.</param>
    /// <param name="source">The activity-result stream of <paramref name="activity"/>.</param>
    /// <param name="type">The type of the activity to start.</param>
    /// <param name="requestCode">The request code to await.</param>
    /// <returns>A task that completes with the result and intent of the matching activity result.</returns>
    internal static Task<(Result Result, Intent? Intent)> StartForResultAsync(
        Activity activity,
        IObservable<(int RequestCode, Result Result, Intent? Intent)> source,
        Type type,
        int requestCode)
    {
        // NB: It's important that we set up the subscription *before* we call ActivityForResult.
        var ret = Await(source, requestCode);
        activity.StartActivityForResult(type, requestCode);
        return ret;
    }

    /// <summary>Faults or cancels the awaiter, if this caller wins the race to settle it, and detaches.</summary>
    /// <param name="error">The error to fault with, or <see langword="null"/> to cancel instead.</param>
    private void Settle(Exception? error)
    {
        if (!TryClaimSettle())
        {
            return;
        }

        _ = error is null ? _completion.TrySetCanceled() : _completion.TrySetException(error);
        Dispose();
    }

    /// <summary>Takes the single settle slot, so exactly one of a value, an error or completion settles the task.</summary>
    /// <returns><see langword="true"/> for the one caller that took the slot; otherwise <see langword="false"/>.</returns>
    private bool TryClaimSettle() => Interlocked.Exchange(ref _settled, 1) == 0;
}
