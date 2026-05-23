// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Internal;

/// <summary>
/// A subject that remembers the last <c>bufferSize</c> values and replays them to each new subscriber, replacing
/// <c>System.Reactive.Subjects.ReplaySubject&lt;T&gt;</c> with a bounded buffer.
/// </summary>
/// <typeparam name="T">The element type.</typeparam>
/// <param name="bufferSize">The maximum number of recent values replayed to new subscribers.</param>
internal sealed class ReplayBroadcastSubject<T>(int bufferSize) : IReactiveSubject<T>, IDisposable
{
    /// <summary>Guards the buffer, observer set, and terminal state.</summary>
    #if NET9_0_OR_GREATER
    private readonly Lock _gate = new();
    #else
    private readonly object _gate = new();
    #endif

    /// <summary>The most recent values, capped at the buffer size, replayed to new subscribers.</summary>
    private readonly List<T> _buffer = new(bufferSize);

    /// <summary>The observers subscribed to this subject.</summary>
    [SuppressMessage("Major Code Smell", "S3459:Unassigned members should be removed", Justification = "Mutated in place via Broadcaster methods.")]
    private Broadcaster<T> _broadcaster;

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

            _buffer.Add(value);
            if (_buffer.Count > bufferSize)
            {
                _buffer.RemoveAt(0);
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

    /// <inheritdoc/>
    public IDisposable Subscribe(IObserver<T> observer)
    {
        ArgumentExceptionHelper.ThrowIfNull(observer);
        lock (_gate)
        {
            for (var i = 0; i < _buffer.Count; i++)
            {
                observer.OnNext(_buffer[i]);
            }

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
        lock (_gate)
        {
            _buffer.Clear();
        }
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
    private sealed class Subscription(ReplayBroadcastSubject<T> parent, IObserver<T> observer) : IDisposable
    {
        /// <inheritdoc/>
        public void Dispose() => parent.Unsubscribe(observer);
    }
}
