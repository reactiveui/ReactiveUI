// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Runtime.CompilerServices;
using ReactiveUI.Primitives.Disposables;

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive.Internal;
#else
namespace ReactiveUI.Internal;
#endif
/// <summary>
/// Forwards each notification to a downstream observer on a scheduler, replacing a per-observer <c>ObserveOn</c>.
/// Notifications are queued and delivered in arrival order by a single scheduled drain, so a concurrent scheduler
/// (such as the task pool) cannot run a later notification (for example <c>OnCompleted</c>) before an earlier one.
/// </summary>
/// <typeparam name="T">The element type.</typeparam>
/// <param name="downstream">The observer that receives the scheduled notifications.</param>
/// <param name="scheduler">The scheduler each notification is delivered on.</param>
internal sealed class SchedulingObserver<T>(IObserver<T> downstream, ISequencer scheduler) : IObserver<T>
{
    /// <summary>Guards <see cref="_queue"/> and <see cref="_draining"/>.</summary>
#if NET9_0_OR_GREATER
    private readonly Lock _gate = new();
#else
    private readonly object _gate = new();
#endif

    /// <summary>Notifications waiting for the drain, in arrival order.</summary>
    private readonly Queue<Notification> _queue = new();

    /// <summary>Whether a drain is scheduled or running.</summary>
    private bool _draining;

    /// <summary>The kind of a queued notification.</summary>
    private enum NotificationKind
    {
        /// <summary>An <c>OnNext</c> value.</summary>
        Next = 0,

        /// <summary>An <c>OnError</c> failure.</summary>
        Error = 1,

        /// <summary>An <c>OnCompleted</c> signal.</summary>
        Completed = 2,
    }

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void OnNext(T value) => Enqueue(new(NotificationKind.Next, value, null));

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void OnError(Exception error) => Enqueue(new(NotificationKind.Error, default!, error));

    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void OnCompleted() => Enqueue(new(NotificationKind.Completed, default!, null));

    /// <summary>Queues a notification and schedules a drain when none is pending.</summary>
    /// <param name="notification">The notification to deliver.</param>
    private void Enqueue(in Notification notification)
    {
        // The immediate scheduler runs work inline on the calling thread, so arrival order is already kept.
        if (ReferenceEquals(scheduler, Sequencer.Immediate))
        {
            Deliver(notification);
            return;
        }

        lock (_gate)
        {
            _queue.Enqueue(notification);
            if (_draining)
            {
                return;
            }

            _draining = true;
        }

        _ = scheduler.Schedule(this, static (_, self) =>
        {
            self.Drain();
            return EmptyDisposable.Instance;
        });
    }

    /// <summary>Delivers queued notifications in order until the queue is empty.</summary>
    private void Drain()
    {
        while (true)
        {
            Notification notification;
            lock (_gate)
            {
                if (_queue.Count == 0)
                {
                    _draining = false;
                    return;
                }

                notification = _queue.Dequeue();
            }

            Deliver(notification);
        }
    }

    /// <summary>Forwards one notification to the downstream observer.</summary>
    /// <param name="notification">The notification to forward.</param>
    private void Deliver(in Notification notification)
    {
        switch (notification.Kind)
        {
            case NotificationKind.Next:
                {
                    downstream.OnNext(notification.Value);
                    break;
                }

            case NotificationKind.Error:
                {
                    downstream.OnError(notification.Error!);
                    break;
                }

            default:
                {
                    downstream.OnCompleted();
                    break;
                }
        }
    }

    /// <summary>A queued notification.</summary>
    /// <param name="Kind">The notification kind.</param>
    /// <param name="Value">The value, for <see cref="NotificationKind.Next"/>.</param>
    /// <param name="Error">The failure, for <see cref="NotificationKind.Error"/>.</param>
    private readonly record struct Notification(NotificationKind Kind, T Value, Exception? Error);
}
