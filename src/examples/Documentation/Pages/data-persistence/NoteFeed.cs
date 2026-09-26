// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections;
using System.Collections.Specialized;

namespace ReactiveUI.Documentation.DataPersistence;

/// <summary>
/// A feed of notes shared between devices. It is not an <see cref="System.Collections.ObjectModel.ObservableCollection{T}"/>,
/// only a plain list that raises <see cref="INotifyCollectionChanged.CollectionChanged"/>, the way a feed backed by a
/// sync client might.
/// </summary>
[System.Diagnostics.DebuggerDisplay("Count = {_notes.Count}")]
public sealed class NoteFeed : IEnumerable<Note>, INotifyCollectionChanged
{
    /// <summary>The notes currently in the feed.</summary>
    private readonly List<Note> _notes = [];

    /// <inheritdoc/>
    public event NotifyCollectionChangedEventHandler? CollectionChanged;

    /// <summary>Adds a note to the feed.</summary>
    /// <param name="note">The note to publish.</param>
    public void Publish(Note note)
    {
        _notes.Add(note);
        CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, note, _notes.Count - 1));
    }

    /// <summary>Removes a note from the feed.</summary>
    /// <param name="note">The note to retire.</param>
    public void Retire(Note note)
    {
        int index = _notes.IndexOf(note);
        _notes.RemoveAt(index);
        CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, note, index));
    }

    /// <inheritdoc/>
    public IEnumerator<Note> GetEnumerator() => _notes.GetEnumerator();

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
