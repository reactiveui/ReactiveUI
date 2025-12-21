// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using DynamicData.Binding;

using Microsoft.Reactive.Testing;

using ReactiveUI.Testing;

namespace ReactiveUI.Tests;

/// <summary>
/// Tests to make sure that the auto persist collection works.
/// </summary>
[TestFixture]
public class AutoPersistCollectionTests
{
    /// <summary>
    /// Test the automatic persist collection smoke test.
    /// </summary>
    [Test]
    public void AutoPersistCollectionSmokeTest() =>
        new TestScheduler().With(scheduler =>
        {
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

            scheduler.AdvanceByMs(2 * 100);
            Assert.That(timesSaved, Is.Zero);

            // By being added to collection, AutoPersist is enabled for item
            item.IsNotNullString = "Foo";
            scheduler.AdvanceByMs(2 * 100);
            Assert.That(timesSaved, Is.EqualTo(1));

            // Removed from collection = no save
            fixture.Clear();
            scheduler.AdvanceByMs(2 * 100);
            Assert.That(timesSaved, Is.EqualTo(1));

            // Item isn't in the collection, it doesn't get persisted anymore
            item.IsNotNullString = "Bar";
            scheduler.AdvanceByMs(2 * 100);
            Assert.That(timesSaved, Is.EqualTo(1));

            // Added back item gets saved
            fixture.Add(item);
            scheduler.AdvanceByMs(100);  // Compensate for scheduling
            item.IsNotNullString = "Baz";
            scheduler.AdvanceByMs(2 * 100);
            Assert.That(timesSaved, Is.EqualTo(2));

            // Even if we issue a reset
            fixture.SuspendNotifications().Dispose(); // Will cause a reset.

            scheduler.AdvanceByMs(100);  // Compensate for scheduling
            item.IsNotNullString = "Bamf";
            scheduler.AdvanceByMs(2 * 100);
            Assert.That(timesSaved, Is.EqualTo(3));

            // Remove by hand = no save
            fixture.RemoveAt(0);
            item.IsNotNullString = "Blomf";
            scheduler.AdvanceByMs(2 * 100);
            Assert.That(timesSaved, Is.EqualTo(3));
        });

    /// <summary>
    /// Test the automatic persist collection disconnects on dispose.
    /// </summary>
    [Test]
    public void AutoPersistCollectionDisconnectsOnDispose() =>
        new TestScheduler().With(scheduler =>
        {
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

            scheduler.AdvanceByMs(2 * 100);
            Assert.That(timesSaved, Is.Zero);

            // By being added to collection, AutoPersist is enabled for item
            item.IsNotNullString = "Foo";
            scheduler.AdvanceByMs(2 * 100);
            Assert.That(timesSaved, Is.EqualTo(1));

            // Dispose = no save
            disp.Dispose();

            // Removed from collection = no save
            fixture.Clear();
            scheduler.AdvanceByMs(2 * 100);
            Assert.That(timesSaved, Is.EqualTo(1));

            // Item isn't in the collection, it doesn't get persisted anymore
            item.IsNotNullString = "Bar";
            scheduler.AdvanceByMs(2 * 100);
            Assert.That(timesSaved, Is.EqualTo(1));

            // Added back item + dispose = no save
            fixture.Add(item);
            scheduler.AdvanceByMs(100);  // Compensate for scheduling
            item.IsNotNullString = "Baz";
            scheduler.AdvanceByMs(2 * 100);
            Assert.That(timesSaved, Is.EqualTo(1));

            // Even if we issue a reset, no save
            fixture.SuspendNotifications().Dispose(); // Will trigger a reset.
            scheduler.AdvanceByMs(100);  // Compensate for scheduling
            item.IsNotNullString = "Bamf";
            scheduler.AdvanceByMs(2 * 100);
            Assert.That(timesSaved, Is.EqualTo(1));

            // Remove by hand = no save
            fixture.RemoveAt(0);
            item.IsNotNullString = "Blomf";
            scheduler.AdvanceByMs(2 * 100);
            Assert.That(timesSaved, Is.EqualTo(1));
        });
}
