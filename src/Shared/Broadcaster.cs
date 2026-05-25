// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Internal;

/// <summary>
/// A lean broadcast helper used in place of a general subject. The observer set is held in a single
/// reference field — <see langword="null"/> for none, the lone <see cref="IObserver{T}"/> for one, and a
/// copy-on-write <see cref="IObserver{T}"/> array for two or more — so the common one-subscriber case
/// allocates nothing. As a value type it lives inline in its owner, adding no allocation of its own.
/// </summary>
/// <remarks>
/// The owner serializes <see cref="Add"/> / <see cref="Remove"/> through its own gate (those mutate the
/// observer set copy-on-write). Delivery via <see cref="Next"/> / <see cref="Error"/> / <see cref="Completed"/>
/// is lock-free: it takes a single <see cref="Volatile"/> snapshot of the field and delivers against it, so the
/// owner's gate is never shared with — or held during — delivery.
/// </remarks>
/// <typeparam name="T">The broadcast element type.</typeparam>
internal record struct Broadcaster<T>
{
    /// <summary>
    /// The observer set: <see langword="null"/> for none, an <see cref="IObserver{T}"/> for exactly one, or a
    /// copy-on-write <see cref="IObserver{T}"/> array for two or more. Published with release semantics under the
    /// owner's gate and read with <see cref="Volatile"/> acquire semantics during delivery.
    /// </summary>
    private object? _observers;

    /// <summary>Gets a value indicating whether any observer is currently subscribed.</summary>
    public readonly bool HasObservers => _observers is not null;

    /// <summary>
    /// Adds an observer. Must be called under the owner's gate.
    /// </summary>
    /// <param name="observer">The observer to add.</param>
    public void Add(IObserver<T> observer)
    {
        if (_observers is IObserver<T>[] many)
        {
            var copy = new IObserver<T>[many.Length + 1];
            Array.Copy(many, copy, many.Length);
            copy[many.Length] = observer;
            Volatile.Write(ref _observers, copy);
        }
        else if (_observers is IObserver<T> single)
        {
            Volatile.Write(ref _observers, new[] { single, observer });
        }
        else
        {
            Volatile.Write(ref _observers, observer);
        }
    }

    /// <summary>
    /// Removes an observer if present. Must be called under the owner's gate.
    /// </summary>
    /// <param name="observer">The observer to remove.</param>
    public void Remove(IObserver<T> observer)
    {
        if (ReferenceEquals(_observers, observer))
        {
            Volatile.Write(ref _observers, null);
            return;
        }

        if (_observers is not IObserver<T>[] many)
        {
            return;
        }

        var index = Array.IndexOf(many, observer);
        if (index < 0)
        {
            return;
        }

        if (many.Length == 2)
        {
            Volatile.Write(ref _observers, many[index == 0 ? 1 : 0]);
            return;
        }

        var copy = new IObserver<T>[many.Length - 1];
        for (var i = 0; i < index; i++)
        {
            copy[i] = many[i];
        }

        for (var i = index + 1; i < many.Length; i++)
        {
            copy[i - 1] = many[i];
        }

        Volatile.Write(ref _observers, copy);
    }

    /// <summary>
    /// Broadcasts a value to every current observer. Lock-free: delivers against a <see cref="Volatile"/> snapshot.
    /// </summary>
    /// <param name="value">The value to broadcast.</param>
    public void Next(T value)
    {
        var snapshot = Volatile.Read(ref _observers);
        if (snapshot is IObserver<T> one)
        {
            one.OnNext(value);
        }
        else if (snapshot is IObserver<T>[] many)
        {
            for (var i = 0; i < many.Length; i++)
            {
                many[i].OnNext(value);
            }
        }
    }

    /// <summary>
    /// Broadcasts an error to every current observer. Lock-free: delivers against a <see cref="Volatile"/> snapshot.
    /// </summary>
    /// <param name="errorException">The error to broadcast.</param>
    public void Error(Exception errorException)
    {
        var snapshot = Volatile.Read(ref _observers);
        if (snapshot is IObserver<T> one)
        {
            one.OnError(errorException);
        }
        else if (snapshot is IObserver<T>[] many)
        {
            for (var i = 0; i < many.Length; i++)
            {
                many[i].OnError(errorException);
            }
        }
    }

    /// <summary>
    /// Broadcasts completion to every current observer. Lock-free: delivers against a <see cref="Volatile"/> snapshot.
    /// </summary>
    public void Completed()
    {
        var snapshot = Volatile.Read(ref _observers);
        if (snapshot is IObserver<T> one)
        {
            one.OnCompleted();
        }
        else if (snapshot is IObserver<T>[] many)
        {
            for (var i = 0; i < many.Length; i++)
            {
                many[i].OnCompleted();
            }
        }
    }
}
