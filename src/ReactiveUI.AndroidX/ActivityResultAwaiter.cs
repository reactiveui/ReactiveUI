// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Android.Content;

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive.AndroidX;
#else
namespace ReactiveUI.AndroidX;
#endif

/// <summary>
/// Awaits the first activity result that matches a request code, then completes the task and detaches. A fused,
/// allocation-light replacement for <c>Where(...).Select(...).FirstAsync().ToTask()</c>: it is its own observer,
/// settles exactly once, and unsubscribes on completion. Shared by the AppCompat and Fragment reactive activities.
/// </summary>
internal sealed class ActivityResultAwaiter
    : IObserver<(int requestCode, Result result, Intent? intent)>, IDisposable
{
    /// <summary>The request code this awaiter is waiting for.</summary>
    private readonly int _requestCode;

    /// <summary>Completion source backing the returned task.</summary>
    private readonly TaskCompletionSource<(Result result, Intent? intent)> _completion =
        new(TaskCreationOptions.RunContinuationsAsynchronously);

    /// <summary>Holds the source subscription so it can be torn down once settled.</summary>
    private readonly OnceDisposable _subscription = new();

    /// <summary>Guards against settling more than once.</summary>
    private int _settled;

    /// <summary>Initializes a new instance of the <see cref="ActivityResultAwaiter"/> class.</summary>
    /// <param name="requestCode">The request code to await.</param>
    private ActivityResultAwaiter(int requestCode) => _requestCode = requestCode;

    /// <inheritdoc/>
    public void OnNext((int requestCode, Result result, Intent? intent) value)
    {
        if (value.requestCode != _requestCode || Interlocked.Exchange(ref _settled, 1) != 0)
        {
            return;
        }

        _ = _completion.TrySetResult((value.result, value.intent));
        Dispose();
    }

    /// <inheritdoc/>
    public void OnError(Exception error)
    {
        if (Interlocked.Exchange(ref _settled, 1) != 0)
        {
            return;
        }

        _ = _completion.TrySetException(error);
        Dispose();
    }

    /// <inheritdoc/>
    public void OnCompleted()
    {
        if (Interlocked.Exchange(ref _settled, 1) != 0)
        {
            return;
        }

        _ = _completion.TrySetCanceled();
        Dispose();
    }

    /// <inheritdoc/>
    public void Dispose() => _subscription.Dispose();

    /// <summary>Subscribes to the activity-result stream and returns a task for the first matching result.</summary>
    /// <param name="source">The activity-result stream.</param>
    /// <param name="requestCode">The request code to await.</param>
    /// <returns>A task that completes with the result and intent of the first matching activity result.</returns>
    internal static Task<(Result result, Intent? intent)> Await(
        IObservable<(int requestCode, Result result, Intent? intent)> source,
        int requestCode)
    {
        var awaiter = new ActivityResultAwaiter(requestCode);
        awaiter._subscription.Disposable = source.Subscribe(awaiter);
        return awaiter._completion.Task;
    }
}
