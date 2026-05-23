// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI;

/// <summary>
/// Describes a single change to a collection: the reason, the affected item, and (where relevant) the previous item
/// and indices.
/// </summary>
/// <typeparam name="T">The collection item type.</typeparam>
public readonly struct ReactiveChange<T> : IEquatable<ReactiveChange<T>>
{
    /// <summary>Initializes a new instance of the <see cref="ReactiveChange{T}"/> struct.</summary>
    /// <param name="reason">The reason for the change.</param>
    /// <param name="current">The current (added/replacing/moved/refreshed) item.</param>
    /// <param name="previous">The previous item for a <see cref="ReactiveChangeReason.Replace"/>; otherwise default.</param>
    /// <param name="currentIndex">The item's current index, or -1 when unknown.</param>
    /// <param name="previousIndex">The item's previous index for a <see cref="ReactiveChangeReason.Move"/>, or -1.</param>
    public ReactiveChange(ReactiveChangeReason reason, T current, T? previous, int currentIndex, int previousIndex)
    {
        Reason = reason;
        Current = current;
        Previous = previous;
        CurrentIndex = currentIndex;
        PreviousIndex = previousIndex;
    }

    /// <summary>Gets the reason for the change.</summary>
    public ReactiveChangeReason Reason { get; }

    /// <summary>Gets the current item (added, replacing, moved, or refreshed).</summary>
    public T Current { get; }

    /// <summary>Gets the previous item for a <see cref="ReactiveChangeReason.Replace"/>; otherwise default.</summary>
    public T? Previous { get; }

    /// <summary>Gets the item's current index, or -1 when unknown.</summary>
    public int CurrentIndex { get; }

    /// <summary>Gets the item's previous index for a <see cref="ReactiveChangeReason.Move"/>, or -1.</summary>
    public int PreviousIndex { get; }

    /// <summary>Determines whether two changes are equal.</summary>
    /// <param name="left">The first change.</param>
    /// <param name="right">The second change.</param>
    /// <returns><see langword="true"/> if equal.</returns>
    public static bool operator ==(ReactiveChange<T> left, ReactiveChange<T> right) => left.Equals(right);

    /// <summary>Determines whether two changes are unequal.</summary>
    /// <param name="left">The first change.</param>
    /// <param name="right">The second change.</param>
    /// <returns><see langword="true"/> if unequal.</returns>
    public static bool operator !=(ReactiveChange<T> left, ReactiveChange<T> right) => !left.Equals(right);

    /// <inheritdoc/>
    public bool Equals(ReactiveChange<T> other) =>
        Reason == other.Reason &&
        EqualityComparer<T>.Default.Equals(Current, other.Current) &&
        EqualityComparer<T?>.Default.Equals(Previous, other.Previous) &&
        CurrentIndex == other.CurrentIndex &&
        PreviousIndex == other.PreviousIndex;

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is ReactiveChange<T> other && Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Reason, Current, Previous, CurrentIndex, PreviousIndex);
}
