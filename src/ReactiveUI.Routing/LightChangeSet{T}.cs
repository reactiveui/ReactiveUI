// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections;
using DynamicData;

namespace ReactiveUI;

/// <summary>
/// A minimal <see cref="IChangeSet{T}"/> implementation that holds either a single change inline (the common case for
/// a collection that mutates one item at a time) or a small array, avoiding DynamicData's <c>List</c>-backed
/// <c>ChangeSet&lt;T&gt;</c> allocation. Only item changes are produced (never range changes), so the change tallies
/// are a direct count by reason.
/// </summary>
/// <typeparam name="T">The collection item type.</typeparam>
internal sealed class LightChangeSet<T> : IChangeSet<T>
    where T : notnull
{
    /// <summary>The single change when this set holds exactly one; otherwise <see langword="null"/>.</summary>
    private readonly Change<T>? _single;

    /// <summary>The backing array when this set holds more than one change; otherwise <see langword="null"/>.</summary>
    private readonly Change<T>[]? _many;

    /// <summary>Initializes a new instance of the <see cref="LightChangeSet{T}"/> class holding a single change.</summary>
    /// <param name="change">The single change.</param>
    public LightChangeSet(Change<T> change)
    {
        _single = change;
        _many = null;
        Count = 1;
        Tally(change);
    }

    /// <summary>Initializes a new instance of the <see cref="LightChangeSet{T}"/> class holding multiple changes.</summary>
    /// <param name="changes">The changes.</param>
    public LightChangeSet(Change<T>[] changes)
    {
        _single = null;
        _many = changes;
        Count = changes.Length;
        for (var i = 0; i < changes.Length; i++)
        {
            Tally(changes[i]);
        }
    }

    /// <inheritdoc/>
    public int Count { get; }

    /// <inheritdoc/>
    public int Capacity { get => Count; set => _ = value; }

    /// <inheritdoc/>
    public int Adds { get; private set; }

    /// <inheritdoc/>
    public int Removes { get; private set; }

    /// <inheritdoc/>
    public int Replaced { get; private set; }

    /// <inheritdoc/>
    public int Moves { get; private set; }

    /// <inheritdoc/>
    public int Refreshes { get; private set; }

    /// <inheritdoc/>
    public int TotalChanges => Count;

    /// <inheritdoc/>
    public IEnumerator<Change<T>> GetEnumerator()
    {
        if (_many is null)
        {
            yield return _single!;
            yield break;
        }

        for (var i = 0; i < _many.Length; i++)
        {
            yield return _many[i];
        }
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    /// <summary>Accumulates a change into the per-reason tallies.</summary>
    /// <param name="change">The change to tally.</param>
    private void Tally(Change<T> change)
    {
        switch (change.Reason)
        {
            case ListChangeReason.Add:
                {
                    Adds++;
                    break;
                }

            case ListChangeReason.Remove:
                {
                    Removes++;
                    break;
                }

            case ListChangeReason.Replace:
                {
                    Replaced++;
                    break;
                }

            case ListChangeReason.Moved:
                {
                    Moves++;
                    break;
                }

            case ListChangeReason.Refresh:
                {
                    Refreshes++;
                    break;
                }
        }
    }
}
