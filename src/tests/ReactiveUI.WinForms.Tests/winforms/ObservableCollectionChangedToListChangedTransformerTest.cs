// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.Specialized;
using ReactiveUI.Winforms;
using TUnit.Core.Executors;

namespace ReactiveUI.WinForms.Tests.Winforms;

/// <summary>
/// Tests for <see cref="ObservableCollectionChangedToListChangedTransformer"/>.
/// </summary>
[NotInParallel]
[TestExecutor<WinFormsTestExecutor>]

public class ObservableCollectionChangedToListChangedTransformerTest
{
    /// <summary>
    /// Tests that Reset action produces ListChangedType.Reset.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task AsListChangedEventArgs_Reset_ProducesResetEvent()
    {
        var eventArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);

        var results = eventArgs.AsListChangedEventArgs().ToList();

        await Assert.That(results).Count().IsEqualTo(1);
        await Assert.That(results[0].ListChangedType).IsEqualTo(ListChangedType.Reset);
        await Assert.That(results[0].NewIndex).IsEqualTo(-1);
    }

    /// <summary>
    /// Tests that Replace action produces ListChangedType.ItemChanged.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task AsListChangedEventArgs_Replace_ProducesItemChangedEvent()
    {
        var eventArgs = new NotifyCollectionChangedEventArgs(
            NotifyCollectionChangedAction.Replace,
            newItem: "new",
            oldItem: "old",
            index: 2);

        var results = eventArgs.AsListChangedEventArgs().ToList();

        await Assert.That(results).Count().IsEqualTo(1);
        await Assert.That(results[0].ListChangedType).IsEqualTo(ListChangedType.ItemChanged);
        await Assert.That(results[0].NewIndex).IsEqualTo(2);
    }

    /// <summary>
    /// Tests that Remove action produces ListChangedType.ItemDeleted events.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task AsListChangedEventArgs_Remove_ProducesItemDeletedEvents()
    {
        var removedItems = new[] { "item1", "item2", "item3" };
        var eventArgs = new NotifyCollectionChangedEventArgs(
            NotifyCollectionChangedAction.Remove,
            removedItems,
            startingIndex: 5);

        var results = eventArgs.AsListChangedEventArgs().ToList();

        await Assert.That(results).Count().IsEqualTo(3);
        await Assert.That(results[0].ListChangedType).IsEqualTo(ListChangedType.ItemDeleted);
        await Assert.That(results[0].NewIndex).IsEqualTo(5);
        await Assert.That(results[1].NewIndex).IsEqualTo(6);
        await Assert.That(results[2].NewIndex).IsEqualTo(7);
    }

    /// <summary>
    /// Tests that Add action produces ListChangedType.ItemAdded events.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task AsListChangedEventArgs_Add_ProducesItemAddedEvents()
    {
        var addedItems = new[] { "item1", "item2" };
        var eventArgs = new NotifyCollectionChangedEventArgs(
            NotifyCollectionChangedAction.Add,
            addedItems,
            startingIndex: 3);

        var results = eventArgs.AsListChangedEventArgs().ToList();

        await Assert.That(results).Count().IsEqualTo(2);
        await Assert.That(results[0].ListChangedType).IsEqualTo(ListChangedType.ItemAdded);
        await Assert.That(results[0].NewIndex).IsEqualTo(3);
        await Assert.That(results[1].NewIndex).IsEqualTo(4);
    }

    /// <summary>
    /// Tests that Move action produces ListChangedType.ItemMoved event.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task AsListChangedEventArgs_Move_ProducesItemMovedEvent()
    {
        var movedItems = new[] { "item" };
        var eventArgs = new NotifyCollectionChangedEventArgs(
            NotifyCollectionChangedAction.Move,
            movedItems,
            index: 8,
            oldIndex: 2);

        var results = eventArgs.AsListChangedEventArgs().ToList();

        await Assert.That(results).Count().IsEqualTo(1);
        await Assert.That(results[0].ListChangedType).IsEqualTo(ListChangedType.ItemMoved);
        await Assert.That(results[0].NewIndex).IsEqualTo(8);
        await Assert.That(results[0].OldIndex).IsEqualTo(2);
    }

    /// <summary>
    /// Tests that Remove with empty items list produces no events.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task AsListChangedEventArgs_RemoveWithEmptyList_ProducesNoEvents()
    {
        // Create a NotifyCollectionChangedEventArgs with Remove action but empty list
        var eventArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, Array.Empty<string>(), 0);

        var results = ObservableCollectionChangedToListChangedTransformer.AsListChangedEventArgs(eventArgs).ToList();

        await Assert.That(results).IsEmpty();
    }

    /// <summary>
    /// Tests that Add with empty items list produces no events.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task AsListChangedEventArgs_AddWithEmptyList_ProducesNoEvents()
    {
        // Create a NotifyCollectionChangedEventArgs with Add action but empty list
        var eventArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, Array.Empty<string>(), 0);

        var results = ObservableCollectionChangedToListChangedTransformer.AsListChangedEventArgs(eventArgs).ToList();

        await Assert.That(results).IsEmpty();
    }
}
