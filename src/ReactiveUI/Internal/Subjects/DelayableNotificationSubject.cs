// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Internal;

/// <summary>
/// A subject that passes notifications through immediately while change notifications are not delayed, but buffers
/// them while delayed and emits a de-duplicated batch when <see cref="Flush"/> is called (on the delay window opening
/// or closing). Fuses the <c>Buffer(boundary).SelectMany(distinct).Publish().RefCount()</c> pipeline that ReactiveUI
/// used for delayable property-change notifications into one allocation-light sink.
/// </summary>
/// <typeparam name="T">The notification type.</typeparam>
/// <param name="isDelayed">Returns whether change notifications are currently delayed.</param>
/// <param name="flushDistinct">De-duplicates a buffered batch before it is emitted on flush.</param>
internal sealed class DelayableNotificationSubject<T>(
    Func<bool> isDelayed,
    Func<IList<T>, IEnumerable<T>> flushDistinct) : IReactiveSubject<T>, IDisposable
{
    /// <summary>Guards the observer set, buffer, and terminal state.</summary>
    #if NET9_0_OR_GREATER
    private readonly Lock _gate = new();
    #else
    private readonly object _gate = new();
    #endif

    /// <summary>The observers subscribed to this subject.</summary>
    [SuppressMessage("Major Code Smell", "S3459:Unassigned members should be removed", Justification = "Mutated in place via Broadcaster methods.")]
    private Broadcaster<T> _broadcaster;

    /// <summary>Holds notifications produced while delayed; null until the first buffered notification.</summary>
    private List<T>? _buffer;

    /// <summary>The terminal error, if the subject errored.</summary>
    private Exception? _error;

    /// <summary>Whether the subject has terminated (completed or errored).</summary>
    private bool _stopped;

    /// <inheritdoc/>
    public void OnNext(T value)
    {
        lock (_gate)
        {
            if (_stopped)
            {
                return;
            }

            if (isDelayed())
            {
                (_buffer ??= []).Add(value);
                return;
            }
        }

        _broadcaster.Next(value);
    }

    /// <inheritdoc/>
    public void OnError(Exception error)
    {
        lock (_gate)
        {
            if (_stopped)
            {
                return;
            }

            _stopped = true;
            _error = error;
        }

        _broadcaster.Error(error);
    }

    /// <inheritdoc/>
    public void OnCompleted()
    {
        lock (_gate)
        {
            if (_stopped)
            {
                return;
            }

            _stopped = true;
        }

        _broadcaster.Completed();
    }

    /// <summary>Emits any buffered notifications as a de-duplicated batch; called when the delay window opens or closes.</summary>
    public void Flush()
    {
        List<T> batch;
        lock (_gate)
        {
            if (_stopped || _buffer is null || _buffer.Count == 0)
            {
                return;
            }

            batch = [.. flushDistinct(_buffer)];
            _buffer.Clear();
        }

        for (var i = 0; i < batch.Count; i++)
        {
            _broadcaster.Next(batch[i]);
        }
    }

    /// <inheritdoc/>
    public IDisposable Subscribe(IObserver<T> observer)
    {
        ArgumentExceptionHelper.ThrowIfNull(observer);
        lock (_gate)
        {
            if (_error is not null)
            {
                observer.OnError(_error);
                return EmptyDisposable.Instance;
            }

            if (_stopped)
            {
                observer.OnCompleted();
                return EmptyDisposable.Instance;
            }

            _broadcaster.Add(observer);
        }

        return new Subscription(this, observer);
    }

    /// <inheritdoc/>
    public void Dispose()
    {
    }

    /// <summary>Removes an observer from the subject.</summary>
    /// <param name="observer">The observer to remove.</param>
    private void Unsubscribe(IObserver<T> observer)
    {
        lock (_gate)
        {
            _broadcaster.Remove(observer);
        }
    }

    /// <summary>Removes its observer from the subject when disposed.</summary>
    /// <param name="parent">The owning subject.</param>
    /// <param name="observer">The subscribed observer.</param>
    private sealed class Subscription(DelayableNotificationSubject<T> parent, IObserver<T> observer) : IDisposable
    {
        /// <inheritdoc/>
        public void Dispose() => parent.Unsubscribe(observer);
    }
}
