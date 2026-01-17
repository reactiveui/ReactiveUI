// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.ObjectModel;
using DynamicData.Binding;
using ReactiveUI.Tests.ReactiveObjects.Mocks;

namespace ReactiveUI.Tests.AutoPersist;

/// <summary>
///     Comprehensive test suite for AutoPersistCollection functionality.
///     Tests cover collection lifecycle, throttling, and disposal behavior.
/// </summary>
[NotInParallel]
public class AutoPersistCollectionTest
{
    /// <summary>
    ///     Tests that disposing AutoPersistCollection stops all persistence operations.
    ///     Verifies that no saves occur after disposal, even when items change or are added/removed.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    [TestExecutor<WithVirtualTimeSchedulerExecutor>]
    public async Task AutoPersistCollection_Dispose_StopsAllPersistence()
    {
        var scheduler = TestContext.Current.GetVirtualTimeScheduler();

        var manualSave = new Subject<Unit>();
        var item = new TestFixture();
        var fixture = new ObservableCollectionExtended<TestFixture> { item };
        var timesSaved = 0;

        var disp = fixture.AutoPersistCollection(
            _ =>
            {
                timesSaved++;
                return Observables.Unit;
            },
            manualSave,
            TimeSpan.FromMilliseconds(100));

        scheduler.AdvanceBy(TimeSpan.FromMilliseconds(10));
        await Assert.That(timesSaved).IsEqualTo(0);

        item.IsNotNullString = "Foo";
        scheduler.AdvanceBy(TimeSpan.FromMilliseconds(110));
        await Assert.That(timesSaved).IsEqualTo(1);

        disp.Dispose();

        fixture.Clear();
        scheduler.AdvanceBy(TimeSpan.FromMilliseconds(110));
        await Assert.That(timesSaved).IsEqualTo(1);

        item.IsNotNullString = "Bar";
        scheduler.AdvanceBy(TimeSpan.FromMilliseconds(110));
        await Assert.That(timesSaved).IsEqualTo(1);

        fixture.Add(item);
        scheduler.AdvanceBy(TimeSpan.FromMilliseconds(10));

        item.IsNotNullString = "Baz";
        scheduler.AdvanceBy(TimeSpan.FromMilliseconds(110));
        await Assert.That(timesSaved).IsEqualTo(1);

        fixture.SuspendNotifications().Dispose();
        scheduler.AdvanceBy(TimeSpan.FromMilliseconds(10));

        item.IsNotNullString = "Bamf";
        scheduler.AdvanceBy(TimeSpan.FromMilliseconds(110));
        await Assert.That(timesSaved).IsEqualTo(1);

        fixture.RemoveAt(0);
        item.IsNotNullString = "Blomf";
        scheduler.AdvanceBy(TimeSpan.FromMilliseconds(110));
        await Assert.That(timesSaved).IsEqualTo(1);
    }

    /// <summary>
    ///     Tests that AutoPersistCollection handles duplicate adds correctly.
    ///     Verifies that adding the same item twice doesn't create duplicate persistence subscriptions.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    [TestExecutor<WithVirtualTimeSchedulerExecutor>]
    public async Task AutoPersistCollection_DuplicateAdd_NoDoubleSave()
    {
        var scheduler = TestContext.Current.GetVirtualTimeScheduler();

        var item = new TestFixture();
        var fixture = new ObservableCollection<TestFixture>();
        var timesSaved = 0;

        fixture.AutoPersistCollection(
            _ =>
            {
                timesSaved++;
                return Observables.Unit;
            },
            TimeSpan.FromMilliseconds(100));

        scheduler.AdvanceBy(TimeSpan.FromMilliseconds(10));

        fixture.Add(item);
        fixture.Add(item);
        scheduler.AdvanceBy(TimeSpan.FromMilliseconds(1));

        item.IsNotNullString = "Test";
        scheduler.AdvanceBy(TimeSpan.FromMilliseconds(110));

        await Assert.That(timesSaved).IsEqualTo(1);
    }

    /// <summary>
    ///     Tests the complete lifecycle of AutoPersistCollection with add, remove, and re-add operations.
    ///     Verifies that persistence is enabled for items in the collection and disabled when removed.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    [TestExecutor<WithVirtualTimeSchedulerExecutor>]
    public async Task AutoPersistCollection_Lifecycle_ManagesPersistence()
    {
        var scheduler = TestContext.Current.GetVirtualTimeScheduler();

        var manualSave = new Subject<Unit>();
        var item = new TestFixture();
        var fixture = new ObservableCollectionExtended<TestFixture> { item };
        var timesSaved = 0;

        fixture.AutoPersistCollection(
            _ =>
            {
                timesSaved++;
                return Observables.Unit;
            },
            manualSave,
            TimeSpan.FromMilliseconds(100));

        scheduler.AdvanceBy(TimeSpan.FromMilliseconds(10));
        await Assert.That(timesSaved).IsEqualTo(0);

        item.IsNotNullString = "Foo";
        scheduler.AdvanceBy(TimeSpan.FromMilliseconds(110));
        await Assert.That(timesSaved).IsEqualTo(1);

        fixture.Clear();
        scheduler.AdvanceBy(TimeSpan.FromMilliseconds(10));

        item.IsNotNullString = "Bar";
        scheduler.AdvanceBy(TimeSpan.FromMilliseconds(110));
        await Assert.That(timesSaved).IsEqualTo(1);

        fixture.Add(item);
        scheduler.AdvanceBy(TimeSpan.FromMilliseconds(10));

        item.IsNotNullString = "Baz";
        scheduler.AdvanceBy(TimeSpan.FromMilliseconds(110));
        await Assert.That(timesSaved).IsEqualTo(2);

        fixture.SuspendNotifications().Dispose();
        scheduler.AdvanceBy(TimeSpan.FromMilliseconds(10));

        item.IsNotNullString = "Bamf";
        scheduler.AdvanceBy(TimeSpan.FromMilliseconds(110));
        await Assert.That(timesSaved).IsEqualTo(3);

        fixture.RemoveAt(0);
        item.IsNotNullString = "Blomf";
        scheduler.AdvanceBy(TimeSpan.FromMilliseconds(110));
        await Assert.That(timesSaved).IsEqualTo(3);
    }

    /// <summary>
    ///     Tests that AutoPersistCollection manual save signal triggers immediate save.
    ///     Verifies manual save functionality works in addition to automatic throttled saves.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    [TestExecutor<WithVirtualTimeSchedulerExecutor>]
    public async Task AutoPersistCollection_ManualSave_TriggersImmediateSave()
    {
        var scheduler = TestContext.Current.GetVirtualTimeScheduler();

        var manualSave = new Subject<Unit>();
        var item = new TestFixture();
        var fixture = new ObservableCollection<TestFixture> { item };
        var timesSaved = 0;

        fixture.AutoPersistCollection(
            _ =>
            {
                timesSaved++;
                return Observables.Unit;
            },
            manualSave,
            TimeSpan.FromMilliseconds(100));

        scheduler.AdvanceBy(TimeSpan.FromMilliseconds(10));
        await Assert.That(timesSaved).IsEqualTo(0);

        manualSave.OnNext(Unit.Default);
        scheduler.AdvanceBy(TimeSpan.FromMilliseconds(110));
        await Assert.That(timesSaved).IsEqualTo(1);
    }

    /// <summary>
    ///     Tests that AutoPersistCollection with metadata provider works correctly.
    ///     Verifies that metadata provider is called for each item and persistence works.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    [TestExecutor<WithVirtualTimeSchedulerExecutor>]
    public async Task AutoPersistCollection_MetadataProvider_WorksCorrectly()
    {
        var scheduler = TestContext.Current.GetVirtualTimeScheduler();

        var item = new TestFixture();
        var fixture = new ObservableCollection<TestFixture> { item };
        var metadataProvider = AutoPersistHelper.CreateMetadataProvider<TestFixture>();
        var timesSaved = 0;

        fixture.AutoPersistCollection(
            _ =>
            {
                timesSaved++;
                return Observables.Unit;
            },
            Observable<Unit>.Never,
            metadataProvider,
            TimeSpan.FromMilliseconds(100));

        scheduler.AdvanceBy(TimeSpan.FromMilliseconds(10));

        item.IsNotNullString = "Test";
        scheduler.AdvanceBy(TimeSpan.FromMilliseconds(110));
        await Assert.That(timesSaved).IsEqualTo(1);
    }

    /// <summary>
    ///     Tests that AutoPersistCollection handles collection reset events correctly.
    ///     Verifies that persistence continues after a reset operation.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous unit test.</returns>
    [Test]
    [TestExecutor<WithVirtualTimeSchedulerExecutor>]
    public async Task AutoPersistCollection_Reset_ContinuesPersistence()
    {
        var scheduler = TestContext.Current.GetVirtualTimeScheduler();

        var item = new TestFixture();
        var fixture = new ObservableCollectionExtended<TestFixture> { item };
        var timesSaved = 0;

        fixture.AutoPersistCollection(
            _ =>
            {
                timesSaved++;
                return Observables.Unit;
            },
            TimeSpan.FromMilliseconds(100));

        scheduler.AdvanceBy(TimeSpan.FromMilliseconds(10));

        item.IsNotNullString = "Before";
        scheduler.AdvanceBy(TimeSpan.FromMilliseconds(110));
        await Assert.That(timesSaved).IsEqualTo(1);

        fixture.SuspendNotifications().Dispose();
        scheduler.AdvanceBy(TimeSpan.FromMilliseconds(10));

        item.IsNotNullString = "After";
        scheduler.AdvanceBy(TimeSpan.FromMilliseconds(110));
        await Assert.That(timesSaved).IsEqualTo(2);
    }
}
