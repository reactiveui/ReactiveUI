// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections;
using System.Collections.Specialized;

namespace ReactiveUI.Tests.ChangeSets;

/// <summary>Tests for the change-set translation edge cases in <see cref="ChangeSetExtensions"/>.</summary>
public class ChangeSetExtensionsTests
{
    /// <summary>An add with an unknown (-1) starting index appends to the end of the shadow.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task AddWithUnknownIndexAppends()
    {
        var collection = new RaisingCollection("a", "b");
        var sets = new List<IReactiveChangeSet<string>>();
        using var subscription = ChangeSetExtensions.ToReactiveChangeSet<RaisingCollection, string>(collection).Subscribe(sets.Add);

        collection.Raise(new(NotifyCollectionChangedAction.Add, "c", -1));

        var last = sets[^1];
        using (Assert.Multiple())
        {
            await Assert.That(last[0].Reason).IsEqualTo(ReactiveChangeReason.Add);
            await Assert.That(last[0].Current).IsEqualTo("c");
        }
    }

    /// <summary>A remove with an unknown (-1) starting index removes the item by value from the shadow.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task RemoveWithUnknownIndexRemovesByValue()
    {
        var collection = new RaisingCollection("a", "b");
        var sets = new List<IReactiveChangeSet<string>>();
        using var subscription = ChangeSetExtensions.ToReactiveChangeSet<RaisingCollection, string>(collection).Subscribe(sets.Add);

        collection.Raise(new(NotifyCollectionChangedAction.Remove, "a", -1));

        var last = sets[^1];
        using (Assert.Multiple())
        {
            await Assert.That(last[0].Reason).IsEqualTo(ReactiveChangeReason.Remove);
            await Assert.That(last[0].Current).IsEqualTo("a");
        }
    }

    /// <summary>A move event is translated into a move change.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task MoveEmitsMoveChange()
    {
        var collection = new RaisingCollection("a", "b");
        var sets = new List<IReactiveChangeSet<string>>();
        using var subscription = ChangeSetExtensions.ToReactiveChangeSet<RaisingCollection, string>(collection).Subscribe(sets.Add);

        collection.Raise(new(NotifyCollectionChangedAction.Move, "b", 0, 1));

        var last = sets[^1];
        await Assert.That(last[0].Reason).IsEqualTo(ReactiveChangeReason.Move);
    }

    /// <summary>A reset re-emits a remove for each prior item followed by an add for each surviving item.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task ResetReEmitsRemovesThenAdds()
    {
        var collection = new RaisingCollection("a", "b");
        var sets = new List<IReactiveChangeSet<string>>();
        using var subscription = ChangeSetExtensions.ToReactiveChangeSet<RaisingCollection, string>(collection).Subscribe(sets.Add);

        collection.Raise(new(NotifyCollectionChangedAction.Reset));

        var last = sets[^1];
        using (Assert.Multiple())
        {
            await Assert.That(last.Removes).IsGreaterThan(0);
            await Assert.That(last.Adds).IsGreaterThan(0);
        }
    }

    /// <summary>A reset over an empty collection produces no changes and emits nothing beyond the initial batch.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task ResetOnEmptyCollectionEmitsNoFurtherChangeSet()
    {
        var collection = new RaisingCollection();
        var sets = new List<IReactiveChangeSet<string>>();
        using var subscription = ChangeSetExtensions.ToReactiveChangeSet<RaisingCollection, string>(collection).Subscribe(sets.Add);

        // Initial (empty) batch only.
        var initialCount = sets.Count;

        // A reset with an empty shadow and empty collection yields zero changes, so nothing is emitted.
        collection.Raise(new(NotifyCollectionChangedAction.Reset));

        await Assert.That(sets).Count().IsEqualTo(initialCount);
    }

    /// <summary>A notifying collection whose events are raised manually, used to drive the edge-case translation paths.</summary>
    private sealed class RaisingCollection : INotifyCollectionChanged, IEnumerable<string>
    {
        /// <summary>The backing items.</summary>
        private readonly List<string> _items;

        /// <summary>Initializes a new instance of the <see cref="RaisingCollection"/> class.</summary>
        /// <param name="items">The initial items.</param>
        public RaisingCollection(params string[] items) => _items = [.. items];

        /// <inheritdoc/>
        public event NotifyCollectionChangedEventHandler? CollectionChanged;

        /// <summary>Raises the <see cref="CollectionChanged"/> event with the supplied arguments.</summary>
        /// <param name="e">The collection-changed event arguments.</param>
        public void Raise(NotifyCollectionChangedEventArgs e) => CollectionChanged?.Invoke(this, e);

        /// <inheritdoc/>
        public IEnumerator<string> GetEnumerator() => _items.GetEnumerator();

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => _items.GetEnumerator();
    }
}
