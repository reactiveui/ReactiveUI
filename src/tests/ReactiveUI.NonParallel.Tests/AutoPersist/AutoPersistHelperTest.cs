// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Microsoft.Reactive.Testing;

using ReactiveUI.Testing;

namespace ReactiveUI.Tests.Core;

[NotInParallel]
public class AutoPersistHelperTest
{
    /// <summary>
    /// Test the automatic persist doesnt work on non data contract classes.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task AutoPersistDoesntWorkOnNonDataContractClasses()
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

        await Assert.That(shouldDie).IsFalse();
    }

    /// <summary>
    /// Test the automatic persist helper shouldnt trigger on non persistable properties.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task AutoPersistHelperShouldntTriggerOnNonPersistableProperties() =>
        await new TestScheduler().With(async scheduler =>
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
            await Assert.That(timesSaved).IsEqualTo(0);

            // Change to not serialized = no saving
            fixture.NotSerialized = "Foo";
            scheduler.AdvanceByMs(2 * 100);
            await Assert.That(timesSaved).IsEqualTo(0);
        });

    /// <summary>
    /// Tests the automatic persist helper saves on interval.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task AutoPersistHelperSavesOnInterval() =>
        await new TestScheduler().With(async scheduler =>
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
            await Assert.That(timesSaved).IsEqualTo(0);

            // Change = one save
            fixture.IsNotNullString = "Foo";
            scheduler.AdvanceByMs(2 * 100);
            await Assert.That(timesSaved).IsEqualTo(1);

            // Two fast changes = one save
            fixture.IsNotNullString = "Foo";
            fixture.IsNotNullString = "Bar";
            scheduler.AdvanceByMs(2 * 100);
            await Assert.That(timesSaved).IsEqualTo(2);

            // Trigger save twice = one save
            manualSave.OnNext(Unit.Default);
            manualSave.OnNext(Unit.Default);
            scheduler.AdvanceByMs(2 * 100);
            await Assert.That(timesSaved).IsEqualTo(3);
        });

    /// <summary>
    /// Tests the automatic persist helper disconnects.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task AutoPersistHelperDisconnects() =>
        await new TestScheduler().With(async scheduler =>
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
            await Assert.That(timesSaved).IsEqualTo(0);

            // Change = one save
            fixture.IsNotNullString = "Foo";
            scheduler.AdvanceByMs(2 * 100);
            await Assert.That(timesSaved).IsEqualTo(1);

            // Two changes after dispose = no save
            disp.Dispose();
            fixture.IsNotNullString = "Foo";
            fixture.IsNotNullString = "Bar";
            scheduler.AdvanceByMs(2 * 100);
            await Assert.That(timesSaved).IsEqualTo(1);

            // Trigger save after dispose = no save
            manualSave.OnNext(Unit.Default);
            scheduler.AdvanceByMs(2 * 100);
            await Assert.That(timesSaved).IsEqualTo(1);
        });
}
