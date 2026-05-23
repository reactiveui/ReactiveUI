// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive.Subjects;

namespace ReactiveUI.Internal;

/// <summary>
/// A lean multicast subject backed by <see cref="Broadcaster{T}"/>, replacing <c>System.Reactive.Subjects.Subject&lt;T&gt;</c>.
/// Late subscribers receive only values produced after they subscribe; once terminated it replays the terminal state.
/// </summary>
/// <typeparam name="T">The element type.</typeparam>
internal sealed class BroadcastSubject<T> : IReactiveSubject<T>, ISubject<T>, IDisposable
{
    /// <summary>Guards the observer set and terminal state.</summary>
    #if NET9_0_OR_GREATER
    private readonly Lock _gate = new();
    #else
    private readonly object _gate = new();
    #endif

    /// <summary>The observers subscribed to this subject.</summary>
    [SuppressMessage("Major Code Smell", "S3459:Unassigned members should be removed", Justification = "Mutated in place via Broadcaster methods.")]
    private Broadcaster<T> _broadcaster;

    /// <summary>The terminal error, if the subject errored.</summary>
    private Exception? _error;

    /// <summary>Whether the subject has terminated (completed or errored).</summary>
    private bool _stopped;

    /// <summary>Whether the subject has been disposed.</summary>
    private bool _disposed;

    /// <inheritdoc/>
    public void OnNext(T value)
    {
        lock (_gate)
        {
            if (_stopped || _disposed)
            {
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
            if (_stopped || _disposed)
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
            if (_stopped || _disposed)
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
            _disposed = true;
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
    private sealed class Subscription(BroadcastSubject<T> parent, IObserver<T> observer) : IDisposable
    {
        /// <inheritdoc/>
        public void Dispose() => parent.Unsubscribe(observer);
    }
}
