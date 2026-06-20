// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using DynamicData;
using DynamicData.Kernel;

namespace ReactiveUI.Tests;

/// <summary>
/// Tests for the ReactiveUI change-set layer: <see cref="ChangeSetExtensions"/>, <c>ReactiveChangeSet</c>,
/// <c>LightChangeSet</c>, <c>DynamicDataInteropMixins</c>, <see cref="ChangeSetMixins"/>, <see cref="CollectionChangedExtensions"/>,
/// and the <see cref="ReactiveChange{T}"/> / <see cref="CollectionChanged"/> value types.
/// </summary>
public class ChangeSetsTests
{
    /// <summary>Verifies an observable collection projects an initial batch and per-mutation change sets.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ToReactiveChangeSet_EmitsInitialAndMutations()
    {
        var collection = new ObservableCollection<string> { "a" };
        var sets = new List<IReactiveChangeSet<string>>();

        using var sub = collection.ToReactiveChangeSet().Subscribe(sets.Add);
        collection.Add("b");
        collection.RemoveAt(0);
        collection[0] = "c";
        collection.Move(0, 0);

        // Initial seed + four mutations.
        await Assert.That(sets).IsNotEmpty();
        await Assert.That(sets[0][0].Reason).IsEqualTo(ReactiveChangeReason.Add);
        await Assert.That(sets[1][0].Current).IsEqualTo("b");
    }

    /// <summary>Verifies the reactive change-set stream adapts onto DynamicData's change-set surface.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ToDynamicDataChangeSet_ProjectsAddsAndRemoves()
    {
        var collection = new ObservableCollection<string> { "a", "b" };
        var sets = new List<IChangeSet<string>>();

        using var sub = collection.ToReactiveChangeSet().ToDynamicDataChangeSet().Subscribe(sets.Add);
        collection.Add("c");
        collection.RemoveAt(0);

        await Assert.That(sets).IsNotEmpty();
        await Assert.That(sets.Exists(static s => s.Adds > 0)).IsTrue();
        await Assert.That(sets.Exists(static s => s.Removes > 0)).IsTrue();
    }

    /// <summary>Verifies <see cref="ChangeSetMixins"/> count-change filtering drops non-count change sets.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task CountChanged_FiltersDynamicDataSets()
    {
        var collection = new ObservableCollection<string> { "a" };
        var countChanges = new List<IChangeSet<string>>();

        using var sub = collection.ToReactiveChangeSet()
            .ToDynamicDataChangeSet()
            .CountChanged()
            .Subscribe(countChanges.Add);
        collection.Add("b");
        collection[0] = "replaced";

        await Assert.That(countChanges).IsNotEmpty();
        await Assert.That(countChanges.TrueForAll(static s => s.Adds > 0 || s.Removes > 0)).IsTrue();
    }

    /// <summary>Verifies <see cref="ChangeSetMixins"/> count-change detection reflects whether a DynamicData set alters the count.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task HasCountChanged_ReflectsAddsAndRemoves()
    {
        IChangeSet<string> add = new LightChangeSet<string>(new Change<string>(ListChangeReason.Add, "a", 0));
        IChangeSet<string> refresh = new LightChangeSet<string>(new Change<string>(ListChangeReason.Refresh, "a", 0));

        await Assert.That(add.HasCountChanged()).IsTrue();
        await Assert.That(refresh.HasCountChanged()).IsFalse();
    }

    /// <summary>Verifies <c>LightChangeSet</c> tallies a single change and enumerates it.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task LightChangeSet_SingleChange_TalliesAndEnumerates()
    {
        var set = new LightChangeSet<string>(new Change<string>(ListChangeReason.Add, "a", 0));

        var enumerated = 0;
        foreach (var unused in set)
        {
            enumerated++;
        }

        await Assert.That(set.Count).IsEqualTo(1);
        await Assert.That(set.Adds).IsEqualTo(1);
        await Assert.That(set.TotalChanges).IsEqualTo(1);
        await Assert.That(set.Capacity).IsEqualTo(1);
        await Assert.That(enumerated).IsEqualTo(1);
    }

    /// <summary>Verifies <c>LightChangeSet</c> tallies every change reason across an array of changes.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task LightChangeSet_ManyChanges_TalliesEachReason()
    {
        Change<string>[] changes =
        [
            new(ListChangeReason.Add, "a", 0),
            new(ListChangeReason.Remove, "b", 0),
            new(ListChangeReason.Replace, "c", Optional.Some("old"), 0, 0),
            new("d", 1, 0),
            new(ListChangeReason.Refresh, "e", 0),
        ];
        var set = new LightChangeSet<string>(changes)
        {
            // Setting Capacity is a no-op on this read-only set; exercised here for coverage.
            Capacity = 0,
        };

        await Assert.That(set.Count).IsEqualTo(changes.Length);
        await Assert.That(set.Adds).IsEqualTo(1);
        await Assert.That(set.Removes).IsEqualTo(1);
        await Assert.That(set.Replaced).IsEqualTo(1);
        await Assert.That(set.Moves).IsEqualTo(1);
        await Assert.That(set.Refreshes).IsEqualTo(1);

        var enumerated = 0;
        foreach (var unused in (IEnumerable)set)
        {
            enumerated++;
        }

        await Assert.That(enumerated).IsEqualTo(changes.Length);
    }

    /// <summary>Verifies <see cref="CollectionChangedExtensions.ObserveCollectionChanges"/> forwards events.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ObserveCollectionChanges_ForwardsEvents()
    {
        var collection = new ObservableCollection<string>();
        var events = new List<CollectionChanged>();

        using var sub = collection.ObserveCollectionChanges().Subscribe(events.Add);
        collection.Add("a");

        await Assert.That(events).Count().IsEqualTo(1);
        await Assert.That(events[0].EventArgs.Action).IsEqualTo(NotifyCollectionChangedAction.Add);
        await Assert.That(events[0].Sender).IsEqualTo(collection);
    }

    /// <summary>Verifies <see cref="CollectionChanged"/> value equality and hashing.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task CollectionChanged_ValueEquality()
    {
        var args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
        var sender = new object();
        var a = new CollectionChanged(sender, args);
        var b = new CollectionChanged(sender, args);
        var c = new CollectionChanged(new(), args);

        await Assert.That(a).IsEqualTo(b);
        await Assert.That(a == b).IsTrue();
        await Assert.That(a != c).IsTrue();
        await Assert.That(a.GetHashCode()).IsEqualTo(b.GetHashCode());
    }

    /// <summary>Verifies <see cref="ReactiveChange{T}"/> value equality and hashing.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ReactiveChange_ValueEquality()
    {
        var a = new ReactiveChange<string>(ReactiveChangeReason.Add, "a", default, 0, -1);
        var b = new ReactiveChange<string>(ReactiveChangeReason.Add, "a", default, 0, -1);
        var c = new ReactiveChange<string>(ReactiveChangeReason.Remove, "a", default, 0, -1);

        await Assert.That(a).IsEqualTo(b);
        await Assert.That(a == b).IsTrue();
        await Assert.That(a != c).IsTrue();
        await Assert.That(a.GetHashCode()).IsEqualTo(b.GetHashCode());
        await Assert.That(a.Equals((object)b)).IsTrue();
    }
}
