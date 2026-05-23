// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.Specialized;

namespace ReactiveUI;

/// <summary>
/// A single <see cref="INotifyCollectionChanged.CollectionChanged"/> notification, carrying the sender and the event
/// arguments. Replaces the <c>EventPattern&lt;NotifyCollectionChangedEventArgs&gt;</c> element emitted by
/// DynamicData's <c>ObserveCollectionChanges</c>.
/// </summary>
public readonly struct CollectionChanged : IEquatable<CollectionChanged>
{
    /// <summary>Initializes a new instance of the <see cref="CollectionChanged"/> struct.</summary>
    /// <param name="sender">The collection that raised the event.</param>
    /// <param name="eventArgs">The collection-changed event arguments.</param>
    public CollectionChanged(object? sender, NotifyCollectionChangedEventArgs eventArgs)
    {
        Sender = sender;
        EventArgs = eventArgs;
    }

    /// <summary>Gets the collection that raised the event.</summary>
    public object? Sender { get; }

    /// <summary>Gets the collection-changed event arguments.</summary>
    public NotifyCollectionChangedEventArgs EventArgs { get; }

    /// <summary>Determines whether two notifications are equal.</summary>
    /// <param name="left">The first notification.</param>
    /// <param name="right">The second notification.</param>
    /// <returns><see langword="true"/> if equal.</returns>
    public static bool operator ==(CollectionChanged left, CollectionChanged right) => left.Equals(right);

    /// <summary>Determines whether two notifications are unequal.</summary>
    /// <param name="left">The first notification.</param>
    /// <param name="right">The second notification.</param>
    /// <returns><see langword="true"/> if unequal.</returns>
    public static bool operator !=(CollectionChanged left, CollectionChanged right) => !left.Equals(right);

    /// <inheritdoc/>
    public bool Equals(CollectionChanged other) =>
        ReferenceEquals(Sender, other.Sender) && ReferenceEquals(EventArgs, other.EventArgs);

    /// <inheritdoc/>
    public override bool Equals(object? obj) => obj is CollectionChanged other && Equals(other);

    /// <inheritdoc/>
    public override int GetHashCode() => HashCode.Combine(Sender, EventArgs);
}
