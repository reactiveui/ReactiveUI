// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.Specialized;
using System.ComponentModel;
using TUnit.Core.Executors;

namespace ReactiveUI.WinForms.Tests.Winforms;

/// <summary>Tests for <see cref="ObservableCollectionChangedToListChangedExtensions"/>.</summary>
[NotInParallel]
[TestExecutor<WinFormsTestExecutor>]

public class ObservableCollectionChangedToListChangedTransformerTest
{
    /// <summary>The index reported by a reset list change.</summary>
    private const int ResetIndex = -1;

    /// <summary>The value two used in the tests.</summary>
    private const int Two = 2;

    /// <summary>The value three used in the tests.</summary>
    private const int Three = 3;

    /// <summary>The value four used in the tests.</summary>
    private const int Four = 4;

    /// <summary>The value five used in the tests.</summary>
    private const int Five = 5;

    /// <summary>The value six used in the tests.</summary>
    private const int Six = 6;

    /// <summary>The value seven used in the tests.</summary>
    private const int Seven = 7;

    /// <summary>The value eight used in the tests.</summary>
    private const int Eight = 8;

    /// <summary>Tests that Reset action produces ListChangedType.Reset.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task AsListChangedEventArgs_Reset_ProducesResetEvent()
    {
        var eventArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);

        var results = eventArgs.AsListChangedEventArgs().ToList();

        await Assert.That(results).Count().IsEqualTo(1);
        await Assert.That(results[0].ListChangedType).IsEqualTo(ListChangedType.Reset);
        await Assert.That(results[0].NewIndex).IsEqualTo(ResetIndex);
    }

    /// <summary>Tests that Replace action produces ListChangedType.ItemChanged.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task AsListChangedEventArgs_Replace_ProducesItemChangedEvent()
    {
        var eventArgs = new NotifyCollectionChangedEventArgs(
            NotifyCollectionChangedAction.Replace,
            newItem: "new",
            oldItem: "old",
            index: Two);

        var results = eventArgs.AsListChangedEventArgs().ToList();

        await Assert.That(results).Count().IsEqualTo(1);
        await Assert.That(results[0].ListChangedType).IsEqualTo(ListChangedType.ItemChanged);
        await Assert.That(results[0].NewIndex).IsEqualTo(Two);
    }

    /// <summary>Tests that Remove action produces ListChangedType.ItemDeleted events.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task AsListChangedEventArgs_Remove_ProducesItemDeletedEvents()
    {
        var removedItems = new[] { "item1", "item2", "item3" };
        var eventArgs = new NotifyCollectionChangedEventArgs(
            NotifyCollectionChangedAction.Remove,
            removedItems,
            startingIndex: Five);

        var results = eventArgs.AsListChangedEventArgs().ToList();

        await Assert.That(results).Count().IsEqualTo(Three);
        await Assert.That(results[0].ListChangedType).IsEqualTo(ListChangedType.ItemDeleted);
        await Assert.That(results[0].NewIndex).IsEqualTo(Five);
        await Assert.That(results[1].NewIndex).IsEqualTo(Six);
        await Assert.That(results[Two].NewIndex).IsEqualTo(Seven);
    }

    /// <summary>Tests that Add action produces ListChangedType.ItemAdded events.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task AsListChangedEventArgs_Add_ProducesItemAddedEvents()
    {
        var addedItems = new[] { "item1", "item2" };
        var eventArgs = new NotifyCollectionChangedEventArgs(
            NotifyCollectionChangedAction.Add,
            addedItems,
            startingIndex: Three);

        var results = eventArgs.AsListChangedEventArgs().ToList();

        await Assert.That(results).Count().IsEqualTo(Two);
        await Assert.That(results[0].ListChangedType).IsEqualTo(ListChangedType.ItemAdded);
        await Assert.That(results[0].NewIndex).IsEqualTo(Three);
        await Assert.That(results[1].NewIndex).IsEqualTo(Four);
    }

    /// <summary>Tests that Move action produces ListChangedType.ItemMoved event.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task AsListChangedEventArgs_Move_ProducesItemMovedEvent()
    {
        var movedItems = new[] { "item" };
        var eventArgs = new NotifyCollectionChangedEventArgs(
            NotifyCollectionChangedAction.Move,
            movedItems,
            index: Eight,
            oldIndex: Two);

        var results = eventArgs.AsListChangedEventArgs().ToList();

        await Assert.That(results).Count().IsEqualTo(1);
        await Assert.That(results[0].ListChangedType).IsEqualTo(ListChangedType.ItemMoved);
        await Assert.That(results[0].NewIndex).IsEqualTo(Eight);
        await Assert.That(results[0].OldIndex).IsEqualTo(Two);
    }

    /// <summary>Tests that Remove with empty items list produces no events.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task AsListChangedEventArgs_RemoveWithEmptyList_ProducesNoEvents()
    {
        // Create a NotifyCollectionChangedEventArgs with Remove action but empty list
        var eventArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, Array.Empty<string>(), 0);

        var results = eventArgs.AsListChangedEventArgs().ToList();

        await Assert.That(results).IsEmpty();
    }

    /// <summary>Tests that Add with empty items list produces no events.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task AsListChangedEventArgs_AddWithEmptyList_ProducesNoEvents()
    {
        // Create a NotifyCollectionChangedEventArgs with Add action but empty list
        var eventArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, Array.Empty<string>(), 0);

        var results = eventArgs.AsListChangedEventArgs().ToList();

        await Assert.That(results).IsEmpty();
    }
}
