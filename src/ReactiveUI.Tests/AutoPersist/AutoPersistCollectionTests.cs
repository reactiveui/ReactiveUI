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
    public class AutoPersistCollectionTests
    {
        [Fact]
        public void AutoPersistCollectionSmokeTest() =>
            new TestScheduler().With(sched =>
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

                sched.AdvanceByMs(2 * 100);
                Assert.Equal(0, timesSaved);

                // By being added to collection, AutoPersist is enabled for item
                item.IsNotNullString = "Foo";
                sched.AdvanceByMs(2 * 100);
                Assert.Equal(1, timesSaved);

                // Removed from collection = no save
                fixture.Clear();
                sched.AdvanceByMs(2 * 100);
                Assert.Equal(1, timesSaved);

                // Item isn't in the collection, it doesn't get persisted anymore
                item.IsNotNullString = "Bar";
                sched.AdvanceByMs(2 * 100);
                Assert.Equal(1, timesSaved);

                // Added back item gets saved
                fixture.Add(item);
                sched.AdvanceByMs(100);  // Compensate for scheduling
                item.IsNotNullString = "Baz";
                sched.AdvanceByMs(2 * 100);
                Assert.Equal(2, timesSaved);

                // Even if we issue a reset
                fixture.SuspendNotifications().Dispose(); // Will cause a reset.

                sched.AdvanceByMs(100);  // Compensate for scheduling
                item.IsNotNullString = "Bamf";
                sched.AdvanceByMs(2 * 100);
                Assert.Equal(3, timesSaved);

                // Remove by hand = no save
                fixture.RemoveAt(0);
                item.IsNotNullString = "Blomf";
                sched.AdvanceByMs(2 * 100);
                Assert.Equal(3, timesSaved);
            });

        [Fact]
        public void AutoPersistCollectionDisconnectsOnDispose() =>
            new TestScheduler().With(sched =>
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

                sched.AdvanceByMs(2 * 100);
                Assert.Equal(0, timesSaved);

                // By being added to collection, AutoPersist is enabled for item
                item.IsNotNullString = "Foo";
                sched.AdvanceByMs(2 * 100);
                Assert.Equal(1, timesSaved);

                // Dispose = no save
                disp.Dispose();

                // Removed from collection = no save
                fixture.Clear();
                sched.AdvanceByMs(2 * 100);
                Assert.Equal(1, timesSaved);

                // Item isn't in the collection, it doesn't get persisted anymore
                item.IsNotNullString = "Bar";
                sched.AdvanceByMs(2 * 100);
                Assert.Equal(1, timesSaved);

                // Added back item + dispose = no save
                fixture.Add(item);
                sched.AdvanceByMs(100);  // Compensate for scheduling
                item.IsNotNullString = "Baz";
                sched.AdvanceByMs(2 * 100);
                Assert.Equal(1, timesSaved);

                // Even if we issue a reset, no save
                fixture.SuspendNotifications().Dispose(); // Will trigger a reset.
                sched.AdvanceByMs(100);  // Compensate for scheduling
                item.IsNotNullString = "Bamf";
                sched.AdvanceByMs(2 * 100);
                Assert.Equal(1, timesSaved);

                // Remove by hand = no save
                fixture.RemoveAt(0);
                item.IsNotNullString = "Blomf";
                sched.AdvanceByMs(2 * 100);
                Assert.Equal(1, timesSaved);
            });
    }
}
