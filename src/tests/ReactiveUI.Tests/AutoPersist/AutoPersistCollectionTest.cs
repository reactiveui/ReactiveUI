// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.ObjectModel;
using System.Collections.Specialized;
using ReactiveUI.Tests.ReactiveObjects.Mocks;
using ReactiveUI.Tests.Utilities.Schedulers;
using TUnit.Core.Executors;

namespace ReactiveUI.Tests.AutoPersist;

/// <summary>
///     Comprehensive test suite for AutoPersistCollection functionality.
///     Tests cover collection lifecycle, throttling, and disposal behavior.
/// </summary>
[NotInParallel]
public class AutoPersistCollectionTest
{
    /// <summary>Milliseconds to advance the scheduler past initial subscription setup.</summary>
    private const int InitialAdvanceMilliseconds = 10;

    /// <summary>The throttle interval, in milliseconds, used by the collection persistence tests.</summary>
    private const int ThrottleMilliseconds = 100;

    /// <summary>Milliseconds to advance past the throttle interval to allow a save to fire.</summary>
    private const int PastThrottleMilliseconds = 110;

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

        var manualSave = new Signal<RxVoid>();
        var item = new TestFixture();
        var fixture = new ResettableCollection<TestFixture> { item };
        var timesSaved = 0;

        var disp = fixture.AutoPersistCollection(
            _ =>
            {
                timesSaved++;
                return SingleValueObservable.Void;
            },
            manualSave,
            TimeSpan.FromMilliseconds(ThrottleMilliseconds));

        scheduler.AdvanceBy(TimeSpan.FromMilliseconds(InitialAdvanceMilliseconds));
        await Assert.That(timesSaved).IsEqualTo(0);

        item.IsNotNullString = "Foo";
        scheduler.AdvanceBy(TimeSpan.FromMilliseconds(PastThrottleMilliseconds));
        await Assert.That(timesSaved).IsEqualTo(1);

        disp.Dispose();

        fixture.Clear();
        scheduler.AdvanceBy(TimeSpan.FromMilliseconds(PastThrottleMilliseconds));
        await Assert.That(timesSaved).IsEqualTo(1);

        item.IsNotNullString = "Bar";
        scheduler.AdvanceBy(TimeSpan.FromMilliseconds(PastThrottleMilliseconds));
        await Assert.That(timesSaved).IsEqualTo(1);

        fixture.Add(item);
        scheduler.AdvanceBy(TimeSpan.FromMilliseconds(InitialAdvanceMilliseconds));

        item.IsNotNullString = "Baz";
        scheduler.AdvanceBy(TimeSpan.FromMilliseconds(PastThrottleMilliseconds));
        await Assert.That(timesSaved).IsEqualTo(1);

        fixture.RaiseReset();
        scheduler.AdvanceBy(TimeSpan.FromMilliseconds(InitialAdvanceMilliseconds));

        item.IsNotNullString = "Bamf";
        scheduler.AdvanceBy(TimeSpan.FromMilliseconds(PastThrottleMilliseconds));
        await Assert.That(timesSaved).IsEqualTo(1);

        fixture.RemoveAt(0);
        item.IsNotNullString = "Blomf";
        scheduler.AdvanceBy(TimeSpan.FromMilliseconds(PastThrottleMilliseconds));
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

        _ = fixture.AutoPersistCollection(
            _ =>
            {
                timesSaved++;
                return SingleValueObservable.Void;
            },
            TimeSpan.FromMilliseconds(ThrottleMilliseconds));

        scheduler.AdvanceBy(TimeSpan.FromMilliseconds(InitialAdvanceMilliseconds));

        fixture.Add(item);
        fixture.Add(item);
        scheduler.AdvanceBy(TimeSpan.FromMilliseconds(1));

        item.IsNotNullString = "Test";
        scheduler.AdvanceBy(TimeSpan.FromMilliseconds(PastThrottleMilliseconds));

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
        const int ExpectedSavesAfterReAdd = 2;
        const int ExpectedSavesAfterReset = 3;
        var scheduler = TestContext.Current.GetVirtualTimeScheduler();

        var manualSave = new Signal<RxVoid>();
        var item = new TestFixture();
        var fixture = new ResettableCollection<TestFixture> { item };
        var timesSaved = 0;

        _ = fixture.AutoPersistCollection(
            _ =>
            {
                timesSaved++;
                return SingleValueObservable.Void;
            },
            manualSave,
            TimeSpan.FromMilliseconds(ThrottleMilliseconds));

        scheduler.AdvanceBy(TimeSpan.FromMilliseconds(InitialAdvanceMilliseconds));
        await Assert.That(timesSaved).IsEqualTo(0);

        item.IsNotNullString = "Foo";
        scheduler.AdvanceBy(TimeSpan.FromMilliseconds(PastThrottleMilliseconds));
        await Assert.That(timesSaved).IsEqualTo(1);

        fixture.Clear();
        scheduler.AdvanceBy(TimeSpan.FromMilliseconds(InitialAdvanceMilliseconds));

        item.IsNotNullString = "Bar";
        scheduler.AdvanceBy(TimeSpan.FromMilliseconds(PastThrottleMilliseconds));
        await Assert.That(timesSaved).IsEqualTo(1);

        fixture.Add(item);
        scheduler.AdvanceBy(TimeSpan.FromMilliseconds(InitialAdvanceMilliseconds));

        item.IsNotNullString = "Baz";
        scheduler.AdvanceBy(TimeSpan.FromMilliseconds(PastThrottleMilliseconds));
        await Assert.That(timesSaved).IsEqualTo(ExpectedSavesAfterReAdd);

        fixture.RaiseReset();
        scheduler.AdvanceBy(TimeSpan.FromMilliseconds(InitialAdvanceMilliseconds));

        item.IsNotNullString = "Bamf";
        scheduler.AdvanceBy(TimeSpan.FromMilliseconds(PastThrottleMilliseconds));
        await Assert.That(timesSaved).IsEqualTo(ExpectedSavesAfterReset);

        fixture.RemoveAt(0);
        item.IsNotNullString = "Blomf";
        scheduler.AdvanceBy(TimeSpan.FromMilliseconds(PastThrottleMilliseconds));
        await Assert.That(timesSaved).IsEqualTo(ExpectedSavesAfterReset);
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

        var manualSave = new Signal<RxVoid>();
        var item = new TestFixture();
        var fixture = new ResettableCollection<TestFixture> { item };
        var timesSaved = 0;

        _ = fixture.AutoPersistCollection(
            _ =>
            {
                timesSaved++;
                return SingleValueObservable.Void;
            },
            manualSave,
            TimeSpan.FromMilliseconds(ThrottleMilliseconds));

        scheduler.AdvanceBy(TimeSpan.FromMilliseconds(InitialAdvanceMilliseconds));
        await Assert.That(timesSaved).IsEqualTo(0);

        manualSave.OnNext(RxVoid.Default);
        scheduler.AdvanceBy(TimeSpan.FromMilliseconds(PastThrottleMilliseconds));
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
        var fixture = new ResettableCollection<TestFixture> { item };
        var metadataProvider = AutoPersistHelperMixins.CreateMetadataProvider<TestFixture>();
        var timesSaved = 0;

        _ = fixture.AutoPersistCollection(
            _ =>
            {
                timesSaved++;
                return SingleValueObservable.Void;
            },
            ReactiveUI.Primitives.Signals.Signal.Silent<RxVoid>(),
            metadataProvider,
            TimeSpan.FromMilliseconds(ThrottleMilliseconds));

        scheduler.AdvanceBy(TimeSpan.FromMilliseconds(InitialAdvanceMilliseconds));

        item.IsNotNullString = "Test";
        scheduler.AdvanceBy(TimeSpan.FromMilliseconds(PastThrottleMilliseconds));
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
        const int ExpectedSavesAfterReset = 2;
        var scheduler = TestContext.Current.GetVirtualTimeScheduler();

        var item = new TestFixture();
        var fixture = new ResettableCollection<TestFixture> { item };
        var timesSaved = 0;

        _ = fixture.AutoPersistCollection(
            _ =>
            {
                timesSaved++;
                return SingleValueObservable.Void;
            },
            TimeSpan.FromMilliseconds(ThrottleMilliseconds));

        scheduler.AdvanceBy(TimeSpan.FromMilliseconds(InitialAdvanceMilliseconds));

        item.IsNotNullString = "Before";
        scheduler.AdvanceBy(TimeSpan.FromMilliseconds(PastThrottleMilliseconds));
        await Assert.That(timesSaved).IsEqualTo(1);

        fixture.RaiseReset();
        scheduler.AdvanceBy(TimeSpan.FromMilliseconds(InitialAdvanceMilliseconds));

        item.IsNotNullString = "After";
        scheduler.AdvanceBy(TimeSpan.FromMilliseconds(PastThrottleMilliseconds));
        await Assert.That(timesSaved).IsEqualTo(ExpectedSavesAfterReset);
    }

    /// <summary>An observable collection that can raise a collection-level reset without changing its contents.</summary>
    /// <typeparam name="T">The element type.</typeparam>
    private sealed class ResettableCollection<T> : ObservableCollection<T>
    {
        /// <summary>Raises a <see cref="NotifyCollectionChangedAction.Reset" /> notification, exercising the AutoPersist reset path.</summary>
        public void RaiseReset() =>
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
    }
}
