// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests;

/// <summary>Tests for <see cref="ChangeSetMixins" />.</summary>
public class ChangeSetMixinTest
{
    /// <summary>The value used for the replaced item in change-set test data.</summary>
    private const int ReplacedItemValue = 2;

    /// <summary>The number of count-change emissions expected by the test.</summary>
    private const int ExpectedCountChangeEmissions = 2;

    /// <summary>Tests that WhenCountChanged filters to only count changes.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task WhenCountChanged_FiltersToOnlyCountChanges()
    {
        var subject = new Signal<IReactiveChangeSet<int>>();
        var results = new List<IReactiveChangeSet<int>>();

        _ = subject.WhenCountChanged().ObserveOn(Sequencer.Immediate).Subscribe(results.Add);

        var addChangeSet = new ReactiveChangeSet<int>([new(ReactiveChangeReason.Add, 1, default, 0, -1)]);
        var updateChangeSet = new ReactiveChangeSet<int>([new(ReactiveChangeReason.Replace, ReplacedItemValue, 1, 0, 0)]);
        var removeChangeSet = new ReactiveChangeSet<int>([new(ReactiveChangeReason.Remove, 1, default, 0, -1)]);

        subject.OnNext(addChangeSet);
        subject.OnNext(updateChangeSet);
        subject.OnNext(removeChangeSet);

        await Assert.That(results).Count().IsEqualTo(ExpectedCountChangeEmissions);
        await Assert.That(results[0]).IsEqualTo(addChangeSet);
        await Assert.That(results[1]).IsEqualTo(removeChangeSet);
    }

    /// <summary>Tests that CountHasChanged throws for null changeSet.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task HasCountChanged_NullChangeSet_Throws()
    {
        const IReactiveChangeSet changeSet = null!;

        await Assert.That(changeSet.CountHasChanged)
            .Throws<ArgumentNullException>();
    }

    /// <summary>Tests that CountHasChanged returns true when adds are present.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task HasCountChanged_WithAdds_ReturnsTrue()
    {
        var changeSet = new ReactiveChangeSet<int>([new(ReactiveChangeReason.Add, 1, default, 0, -1)]);

        var result = changeSet.CountHasChanged();

        await Assert.That(result).IsTrue();
    }

    /// <summary>Tests that CountHasChanged returns false when only updates are present.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task HasCountChanged_WithOnlyUpdates_ReturnsFalse()
    {
        var changeSet = new ReactiveChangeSet<int>([new(ReactiveChangeReason.Replace, ReplacedItemValue, 1, 0, 0)]);

        var result = changeSet.CountHasChanged();

        await Assert.That(result).IsFalse();
    }

    /// <summary>Tests that CountHasChanged returns true when removes are present.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task HasCountChanged_WithRemoves_ReturnsTrue()
    {
        var changeSet = new ReactiveChangeSet<int>([new(ReactiveChangeReason.Remove, 1, default, 0, -1)]);

        var result = changeSet.CountHasChanged();

        await Assert.That(result).IsTrue();
    }
}
