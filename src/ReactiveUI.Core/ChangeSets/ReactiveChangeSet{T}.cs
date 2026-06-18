// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections;

namespace ReactiveUI;

/// <summary>
/// A batch of collection changes backed by a list of <see cref="ReactiveChange{T}"/>, exposing the add/remove counts
/// used to detect count changes. The name is deliberately distinct from the DynamicData and System.Reactive change-set
/// types so the two can be referenced side by side without collision.
/// </summary>
/// <typeparam name="T">The collection item type.</typeparam>
[System.Diagnostics.DebuggerDisplay("Count = {Count}, Adds = {Adds}, Removes = {Removes}")]
public sealed class ReactiveChangeSet<T> : IReactiveChangeSet<T>
{
    /// <summary>The changes in this batch.</summary>
    private readonly List<ReactiveChange<T>> _changes;

    /// <summary>Initializes a new instance of the <see cref="ReactiveChangeSet{T}"/> class.</summary>
    /// <param name="changes">The changes in this batch.</param>
    public ReactiveChangeSet(List<ReactiveChange<T>> changes)
    {
        ArgumentExceptionHelper.ThrowIfNull(changes);

        _changes = changes;
        for (var i = 0; i < changes.Count; i++)
        {
            var reason = changes[i].Reason;
            if (reason == ReactiveChangeReason.Add)
            {
                Adds++;
            }
            else if (reason == ReactiveChangeReason.Remove)
            {
                Removes++;
            }
        }
    }

    /// <inheritdoc/>
    public int Count => _changes.Count;

    /// <inheritdoc/>
    public int Adds { get; }

    /// <inheritdoc/>
    public int Removes { get; }

    /// <inheritdoc/>
    public ReactiveChange<T> this[int index] => _changes[index];

    /// <summary>Returns an enumerator over the changes.</summary>
    /// <returns>An enumerator over the changes.</returns>
    public List<ReactiveChange<T>>.Enumerator GetEnumerator() => _changes.GetEnumerator();

    /// <inheritdoc/>
    IEnumerator<ReactiveChange<T>> IEnumerable<ReactiveChange<T>>.GetEnumerator() => _changes.GetEnumerator();

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => _changes.GetEnumerator();
}
