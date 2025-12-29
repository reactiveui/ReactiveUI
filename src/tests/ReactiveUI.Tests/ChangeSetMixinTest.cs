// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using DynamicData;

namespace ReactiveUI.Tests;

/// <summary>
/// Tests for <see cref="ChangeSetMixin"/>.
/// </summary>
public class ChangeSetMixinTest
{
    /// <summary>
    /// Tests that HasCountChanged returns true when adds are present.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task HasCountChanged_WithAdds_ReturnsTrue()
    {
        var changeSet = new ChangeSet<int>([new Change<int>(ListChangeReason.Add, 1, 0)]);

        var result = changeSet.HasCountChanged();

        await Assert.That(result).IsTrue();
    }

    /// <summary>
    /// Tests that HasCountChanged returns true when removes are present.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task HasCountChanged_WithRemoves_ReturnsTrue()
    {
        var changeSet = new ChangeSet<int>([new Change<int>(ListChangeReason.Remove, 1, 0)]);

        var result = changeSet.HasCountChanged();

        await Assert.That(result).IsTrue();
    }

    /// <summary>
    /// Tests that HasCountChanged returns false when only updates are present.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task HasCountChanged_WithOnlyUpdates_ReturnsFalse()
    {
        var changeSet = new ChangeSet<int>([new Change<int>(ListChangeReason.Replace, 2, 1, 0, 0)]);

        var result = changeSet.HasCountChanged();

        await Assert.That(result).IsFalse();
    }

    /// <summary>
    /// Tests that HasCountChanged throws for null changeSet.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task HasCountChanged_NullChangeSet_Throws()
    {
        IChangeSet changeSet = null!;

        await Assert.That(() => changeSet.HasCountChanged())
            .Throws<ArgumentNullException>();
    }

    /// <summary>
    /// Tests that CountChanged filters to only count changes.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task CountChanged_FiltersToOnlyCountChanges()
    {
        var subject = new Subject<IChangeSet>();
        var results = new List<IChangeSet>();

        subject.CountChanged().ObserveOn(ImmediateScheduler.Instance).Subscribe(results.Add);

        var addChangeSet = new ChangeSet<int>([new Change<int>(ListChangeReason.Add, 1, 0)]);
        var updateChangeSet = new ChangeSet<int>([new Change<int>(ListChangeReason.Replace, 2, 1, 0, 0)]);
        var removeChangeSet = new ChangeSet<int>([new Change<int>(ListChangeReason.Remove, 1, 0)]);

        subject.OnNext(addChangeSet);
        subject.OnNext(updateChangeSet);
        subject.OnNext(removeChangeSet);

        await Assert.That(results).Count().IsEqualTo(2);
        await Assert.That(results[0]).IsEqualTo(addChangeSet);
        await Assert.That(results[1]).IsEqualTo(removeChangeSet);
    }

    /// <summary>
    /// Tests that generic CountChanged filters to only count changes.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task CountChanged_Generic_FiltersToOnlyCountChanges()
    {
        var subject = new Subject<IChangeSet<int>>();
        var results = new List<IChangeSet<int>>();

        subject.CountChanged().ObserveOn(ImmediateScheduler.Instance).Subscribe(results.Add);

        var addChangeSet = new ChangeSet<int>([new Change<int>(ListChangeReason.Add, 1, 0)]);
        var updateChangeSet = new ChangeSet<int>([new Change<int>(ListChangeReason.Replace, 2, 1, 0, 0)]);
        var removeChangeSet = new ChangeSet<int>([new Change<int>(ListChangeReason.Remove, 1, 0)]);

        subject.OnNext(addChangeSet);
        subject.OnNext(updateChangeSet);
        subject.OnNext(removeChangeSet);

        await Assert.That(results).Count().IsEqualTo(2);
        await Assert.That(results[0]).IsEqualTo(addChangeSet);
        await Assert.That(results[1]).IsEqualTo(removeChangeSet);
    }
}
