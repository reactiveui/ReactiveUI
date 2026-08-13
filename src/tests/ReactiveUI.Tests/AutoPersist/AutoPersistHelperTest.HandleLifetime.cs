// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.ObjectModel;
using ReactiveUI.Tests.ReactiveObjects.Mocks;
using ReactiveUI.Tests.Utilities.Schedulers;
using TUnit.Core.Executors;

namespace ReactiveUI.Tests.AutoPersist;

/// <summary>
/// Covers the lifetime contract of the handle every AutoPersist overload returns: it is live before the driver
/// exists, so disposing it early cancels the start, and disposing it late stops a running driver. The same
/// contract has to hold for the reflection overloads, the metadata overloads, and every item of a collection.
/// </summary>
public partial class AutoPersistHelperTest
{
    /// <summary>The number of threads used by the concurrent start-and-dispose test.</summary>
    private const int PersistenceThreadCount = 8;

    /// <summary>The number of start-and-dispose cycles each thread performs.</summary>
    private const int PersistenceCyclesPerThread = 250;

    /// <summary>The number of items used by the collection teardown tests.</summary>
    private const int CollectionItemCount = 3;

    /// <summary>The property value written to trigger a save.</summary>
    private const string ChangedValue = "changed";

    /// <summary>Disposing the handle before the scheduled start runs cancels the start, so no driver is ever built.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    [TestExecutor<WithVirtualTimeSchedulerExecutor>]
    public async Task AutoPersist_HandleDisposedBeforeScheduledStart_NeverSaves()
    {
        var scheduler = TestContext.Current.GetVirtualTimeScheduler();

        var fixture = new TestFixture();
        var saveCount = 0;

        var handle = fixture.AutoPersist(
            _ =>
            {
                saveCount++;
                return ImmutableReturnRxVoidSignal.Instance;
            },
            TimeSpan.FromSeconds(1));

        // The start is still queued on the main-thread scheduler: the handle exists, the driver does not.
        handle.Dispose();

        scheduler.AdvanceBy(TimeSpan.FromMilliseconds(InitialAdvanceMilliseconds));
        fixture.IsNotNullString = ChangedValue;
        scheduler.AdvanceBy(TimeSpan.FromSeconds(DefaultIntervalSeconds));

        await Assert.That(saveCount).IsEqualTo(0);
    }

    /// <summary>The metadata overload returns a handle with the same pre-start cancellation contract.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    [TestExecutor<WithVirtualTimeSchedulerExecutor>]
    public async Task AutoPersistWithMetadata_HandleDisposedBeforeScheduledStart_NeverSaves()
    {
        var scheduler = TestContext.Current.GetVirtualTimeScheduler();

        var fixture = new TestFixture();
        var saveCount = 0;

        var handle = fixture.AutoPersist(
            _ =>
            {
                saveCount++;
                return ImmutableReturnRxVoidSignal.Instance;
            },
            AutoPersistHelperMixins.CreateMetadata<TestFixture>(),
            TimeSpan.FromSeconds(1));

        handle.Dispose();

        scheduler.AdvanceBy(TimeSpan.FromMilliseconds(InitialAdvanceMilliseconds));
        fixture.IsNotNullString = ChangedValue;
        scheduler.AdvanceBy(TimeSpan.FromSeconds(DefaultIntervalSeconds));

        await Assert.That(saveCount).IsEqualTo(0);
    }

    /// <summary>The metadata overload's handle stops a driver that has already started.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    [TestExecutor<WithVirtualTimeSchedulerExecutor>]
    public async Task AutoPersistWithMetadata_HandleDisposedAfterStart_StopsSaving()
    {
        var scheduler = TestContext.Current.GetVirtualTimeScheduler();

        var fixture = new TestFixture();
        var saveCount = 0;

        var handle = fixture.AutoPersist(
            _ =>
            {
                saveCount++;
                return ImmutableReturnRxVoidSignal.Instance;
            },
            AutoPersistHelperMixins.CreateMetadata<TestFixture>(),
            TimeSpan.FromSeconds(1));

        scheduler.AdvanceBy(TimeSpan.FromMilliseconds(InitialAdvanceMilliseconds));
        fixture.IsNotNullString = "first";
        scheduler.AdvanceBy(TimeSpan.FromSeconds(1));

        await Assert.That(saveCount).IsEqualTo(1);

        handle.Dispose();

        fixture.IsNotNullString = "second";
        scheduler.AdvanceBy(TimeSpan.FromSeconds(DefaultIntervalSeconds));

        await Assert.That(saveCount).IsEqualTo(1);
    }

    /// <summary>Disposing the collection handle stops persistence for every item still in the collection.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    [TestExecutor<WithVirtualTimeSchedulerExecutor>]
    public async Task AutoPersistCollection_HandleDisposed_StopsSavingEveryItem()
    {
        var scheduler = TestContext.Current.GetVirtualTimeScheduler();

        var collection = new ObservableCollection<TestFixture>();
        for (var i = 0; i < CollectionItemCount; i++)
        {
            collection.Add(new());
        }

        var saveCount = 0;
        var handle = collection.AutoPersistCollection(
            _ =>
            {
                saveCount++;
                return ImmutableReturnRxVoidSignal.Instance;
            },
            TimeSpan.FromMilliseconds(ThrottleMilliseconds));

        scheduler.AdvanceBy(TimeSpan.FromMilliseconds(InitialAdvanceMilliseconds));
        handle.Dispose();

        foreach (var item in collection)
        {
            item.IsNotNullString = ChangedValue;
        }

        scheduler.AdvanceBy(TimeSpan.FromMilliseconds(PastThrottleMilliseconds));

        await Assert.That(saveCount).IsEqualTo(0);
    }

    /// <summary>An item removed from the collection stops being persisted while the remaining items keep going.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    [TestExecutor<WithVirtualTimeSchedulerExecutor>]
    public async Task AutoPersistCollection_ItemRemoved_StopsSavingOnlyThatItem()
    {
        var scheduler = TestContext.Current.GetVirtualTimeScheduler();

        var removed = new TestFixture();
        var kept = new TestFixture();
        var collection = new ObservableCollection<TestFixture> { removed, kept };

        var savedItems = new List<TestFixture>();
        using var handle = collection.AutoPersistCollection(
            item =>
            {
                savedItems.Add(item);
                return ImmutableReturnRxVoidSignal.Instance;
            },
            TimeSpan.FromMilliseconds(ThrottleMilliseconds));

        scheduler.AdvanceBy(TimeSpan.FromMilliseconds(InitialAdvanceMilliseconds));

        _ = collection.Remove(removed);

        removed.IsNotNullString = ChangedValue;
        kept.IsNotNullString = ChangedValue;
        scheduler.AdvanceBy(TimeSpan.FromMilliseconds(PastThrottleMilliseconds));

        await Assert.That(savedItems).IsEquivalentTo(new List<TestFixture> { kept });
    }

    /// <summary>
    /// Starting and disposing persistence from many threads at once never leaves a driver alive behind its handle:
    /// each object saves for the change made while its handle was live, and never for the change made after.
    /// </summary>
    /// <param name="cancellationToken">The token that aborts the test when its timeout elapses.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    /// <remarks>
    /// The handle is published to the caller before the driver is constructed, so a disposal can land on either
    /// side of that construction. Both outcomes are acceptable - the start is cancelled, or the constructed driver
    /// is disposed immediately - but a save after disposal is not.
    /// </remarks>
    [Test]
    [Timeout(120_000)]
    public async Task AutoPersist_StartedAndDisposedFromManyThreads_NeverSavesAfterDisposal(CancellationToken cancellationToken)
    {
        var savesAfterDisposal = 0;
        var savesBeforeDisposal = 0;

        using var start = new ManualResetEventSlim(false);
        var threads = new Thread[PersistenceThreadCount];
        for (var i = 0; i < PersistenceThreadCount; i++)
        {
            threads[i] = new(() =>
            {
                start.Wait(cancellationToken);

                for (var cycle = 0; cycle < PersistenceCyclesPerThread; cycle++)
                {
                    var fixture = new TestFixture();
                    var disposed = false;

                    // The immediate scheduler makes the zero-length debounce run inline, so a save the driver
                    // still accepts after disposal is observed here rather than being swallowed by a timer.
                    var handle = fixture.AutoPersist(
                        target =>
                        {
                            _ = target;
                            _ = disposed
                                ? Interlocked.Increment(ref savesAfterDisposal)
                                : Interlocked.Increment(ref savesBeforeDisposal);

                            return ImmutableReturnRxVoidSignal.Instance;
                        },
                        TimeSpan.Zero);

                    fixture.IsNotNullString = "live";

                    disposed = true;
                    handle.Dispose();

                    fixture.IsNotNullString = "after";
                }
            })
            { IsBackground = true };
            threads[i].Start();
        }

        start.Set();

        for (var i = 0; i < PersistenceThreadCount; i++)
        {
            threads[i].Join();
        }

        await Assert.That(savesAfterDisposal).IsEqualTo(0);
        await Assert.That(savesBeforeDisposal).IsEqualTo(PersistenceThreadCount * PersistenceCyclesPerThread);
    }
}
