// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Microsoft.Reactive.Testing;

using ReactiveUI.Testing;

namespace ReactiveUI.Tests;

/// <summary>
/// Tests the AutoPersistHelper.
/// </summary>
/// <remarks>
/// This test fixture is marked as NonParallelizable because it uses HostTestFixture
/// which depends on ICreatesObservableForProperty from the service locator.
/// The service locator state must not be mutated concurrently by parallel tests.
/// </remarks>
[TestFixture]
[NonParallelizable]
public class AutoPersistHelperTest
{
    /// <summary>
    /// Test the automatic persist doesnt work on non data contract classes.
    /// </summary>
    [Test]
    public void AutoPersistDoesntWorkOnNonDataContractClasses()
    {
        var fixture = new HostTestFixture();

        var shouldDie = true;
        try
        {
            fixture.AutoPersist(static _ => Observables.Unit);
        }
        catch (Exception)
        {
            shouldDie = false;
        }

        Assert.That(shouldDie, Is.False);
    }

    /// <summary>
    /// Test the automatic persist helper shouldnt trigger on non persistable properties.
    /// </summary>
    [Test]
    public void AutoPersistHelperShouldntTriggerOnNonPersistableProperties() =>
        new TestScheduler().With(scheduler =>
        {
            var fixture = new TestFixture();
            var manualSave = new Subject<Unit>();

            var timesSaved = 0;
            fixture.AutoPersist(
                _ =>
                {
                    timesSaved++;
                    return Observables.Unit;
                },
                manualSave,
                TimeSpan.FromMilliseconds(100));

            // No changes = no saving
            scheduler.AdvanceByMs(2 * 100);
            Assert.That(timesSaved, Is.Zero);

            // Change to not serialized = no saving
            fixture.NotSerialized = "Foo";
            scheduler.AdvanceByMs(2 * 100);
            Assert.That(timesSaved, Is.Zero);
        });

    /// <summary>
    /// Tests the automatic persist helper saves on interval.
    /// </summary>
    [Test]
    public void AutoPersistHelperSavesOnInterval() =>
        new TestScheduler().With(scheduler =>
        {
            var fixture = new TestFixture();
            var manualSave = new Subject<Unit>();

            var timesSaved = 0;
            fixture.AutoPersist(
                _ =>
                {
                    timesSaved++;
                    return Observables.Unit;
                },
                manualSave,
                TimeSpan.FromMilliseconds(100));

            // No changes = no saving
            scheduler.AdvanceByMs(2 * 100);
            Assert.That(timesSaved, Is.Zero);

            // Change = one save
            fixture.IsNotNullString = "Foo";
            scheduler.AdvanceByMs(2 * 100);
            Assert.That(timesSaved, Is.EqualTo(1));

            // Two fast changes = one save
            fixture.IsNotNullString = "Foo";
            fixture.IsNotNullString = "Bar";
            scheduler.AdvanceByMs(2 * 100);
            Assert.That(timesSaved, Is.EqualTo(2));

            // Trigger save twice = one save
            manualSave.OnNext(Unit.Default);
            manualSave.OnNext(Unit.Default);
            scheduler.AdvanceByMs(2 * 100);
            Assert.That(timesSaved, Is.EqualTo(3));
        });

    /// <summary>
    /// Tests the automatic persist helper disconnects.
    /// </summary>
    [Test]
    public void AutoPersistHelperDisconnects() =>
        new TestScheduler().With(scheduler =>
        {
            var fixture = new TestFixture();
            var manualSave = new Subject<Unit>();

            var timesSaved = 0;
            var disp = fixture.AutoPersist(
                _ =>
                {
                    timesSaved++;
                    return Observables.Unit;
                },
                manualSave,
                TimeSpan.FromMilliseconds(100));

            // No changes = no saving
            scheduler.AdvanceByMs(2 * 100);
            Assert.That(timesSaved, Is.Zero);

            // Change = one save
            fixture.IsNotNullString = "Foo";
            scheduler.AdvanceByMs(2 * 100);
            Assert.That(timesSaved, Is.EqualTo(1));

            // Two changes after dispose = no save
            disp.Dispose();
            fixture.IsNotNullString = "Foo";
            fixture.IsNotNullString = "Bar";
            scheduler.AdvanceByMs(2 * 100);
            Assert.That(timesSaved, Is.EqualTo(1));

            // Trigger save after dispose = no save
            manualSave.OnNext(Unit.Default);
            scheduler.AdvanceByMs(2 * 100);
            Assert.That(timesSaved, Is.EqualTo(1));
        });
}
