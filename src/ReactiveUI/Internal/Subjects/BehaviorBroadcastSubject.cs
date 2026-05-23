// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Internal;

/// <summary>
/// A subject that remembers its latest value and replays it to each new subscriber, replacing
/// <c>System.Reactive.Subjects.BehaviorSubject&lt;T&gt;</c>.
/// </summary>
/// <typeparam name="T">The element type.</typeparam>
/// <param name="initialValue">The value replayed until the first <see cref="OnNext"/>.</param>
internal sealed class BehaviorBroadcastSubject<T>(T initialValue) : IReactiveSubject<T>, IDisposable
{
    /// <summary>Guards the latest value, observer set, and terminal state.</summary>
    #if NET9_0_OR_GREATER
    private readonly Lock _gate = new();
    #else
    private readonly object _gate = new();
    #endif

    /// <summary>The observers subscribed to this subject.</summary>
    [SuppressMessage("Major Code Smell", "S3459:Unassigned members should be removed", Justification = "Mutated in place via Broadcaster methods.")]
    private Broadcaster<T> _broadcaster;

    /// <summary>The latest value, replayed to new subscribers.</summary>
    private T _value = initialValue;

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

            _value = value;
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
        T current;
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
            current = _value;
        }

        observer.OnNext(current);
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
    private sealed class Subscription(BehaviorBroadcastSubject<T> parent, IObserver<T> observer) : IDisposable
    {
        /// <inheritdoc/>
        public void Dispose() => parent.Unsubscribe(observer);
    }
}
