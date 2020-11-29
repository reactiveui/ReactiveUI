// Copyright (c) 2020 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using DynamicData.Binding;
using Microsoft.Reactive.Testing;
using ReactiveUI.Testing;
using Xunit;

namespace ReactiveUI.Tests
{
    public class AutoPersistHelperTest
    {
        [Fact]
        public void AutoPersistDoesntWorkOnNonDataContractClasses()
        {
            var fixture = new HostTestFixture();

            var shouldDie = true;
            try
            {
                fixture.AutoPersist(_ => Observables.Unit);
            }
            catch (Exception)
            {
                shouldDie = false;
            }

            Assert.False(shouldDie);
        }

        [Fact]
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
                Assert.Equal(0, timesSaved);

                // Change to not serialized = no saving
                fixture.NotSerialized = "Foo";
                scheduler.AdvanceByMs(2 * 100);
                Assert.Equal(0, timesSaved);
            });

        [Fact]
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
                Assert.Equal(0, timesSaved);

                // Change = one save
                fixture.IsNotNullString = "Foo";
                scheduler.AdvanceByMs(2 * 100);
                Assert.Equal(1, timesSaved);

                // Two fast changes = one save
                fixture.IsNotNullString = "Foo";
                fixture.IsNotNullString = "Bar";
                scheduler.AdvanceByMs(2 * 100);
                Assert.Equal(2, timesSaved);

                // Trigger save twice = one save
                manualSave.OnNext(Unit.Default);
                manualSave.OnNext(Unit.Default);
                scheduler.AdvanceByMs(2 * 100);
                Assert.Equal(3, timesSaved);
            });

        [Fact]
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
                Assert.Equal(0, timesSaved);

                // Change = one save
                fixture.IsNotNullString = "Foo";
                scheduler.AdvanceByMs(2 * 100);
                Assert.Equal(1, timesSaved);

                // Two changes after dispose = no save
                disp.Dispose();
                fixture.IsNotNullString = "Foo";
                fixture.IsNotNullString = "Bar";
                scheduler.AdvanceByMs(2 * 100);
                Assert.Equal(1, timesSaved);

                // Trigger save after dispose = no save
                manualSave.OnNext(Unit.Default);
                scheduler.AdvanceByMs(2 * 100);
                Assert.Equal(1, timesSaved);
            });
    }
}
