﻿// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Microsoft.Reactive.Testing;
using ReactiveUI.Testing;
using Xunit;

namespace ReactiveUI.Tests
{
    /// <summary>
    /// Tests for the ObservedChangedMixin.
    /// </summary>
    public class ObservedChangedMixinTest
    {
        /// <summary>
        /// Tests that getting the value should actually return the value.
        /// </summary>
        [Fact]
        public void GetValueShouldActuallyReturnTheValue()
        {
            var input = new[] { "Foo", "Bar", "Baz" };
            var output = new List<string>();

            new TestScheduler().With(scheduler =>
            {
                var fixture = new TestFixture();

                // ...whereas ObservableForProperty *is* guaranteed to.
                fixture.ObservableForProperty(x => x.IsOnlyOneWord)
                    .Select(x => x.GetValue())
                    .WhereNotNull()
                    .Subscribe(x =>
                    {
                        output.Add(x);
                    });

                foreach (var v in input)
                {
                    fixture.IsOnlyOneWord = v;
                }

                scheduler.AdvanceToMs(1000);

                input.AssertAreEqual(output);
            });
        }

        /// <summary>
        /// Tests that getting the value should return the value from a path.
        /// </summary>
        [Fact]
        public void GetValueShouldReturnTheValueFromAPath()
        {
            var input = new HostTestFixture
            {
                Child = new TestFixture { IsNotNullString = "Foo" },
            };

            Expression<Func<HostTestFixture, string>> expression = x => x!.Child!.IsNotNullString!;
            var fixture = new ObservedChange<HostTestFixture, string?>(input, expression.Body, default);

            Assert.Equal("Foo", fixture.GetValue());
        }

        /// <summary>
        /// Runs a smoke test that sets the value path.
        /// </summary>
        [Fact]
        public void SetValuePathSmokeTest()
        {
            var output = new HostTestFixture
            {
                Child = new TestFixture { IsNotNullString = "Foo" },
            };

            Expression<Func<TestFixture, string>> expression = x => x.IsOnlyOneWord!;
            var fixture = new ObservedChange<TestFixture, string?>(new TestFixture { IsOnlyOneWord = "Bar" }, expression.Body, default);

            fixture.SetValueToProperty(output, x => x.Child!.IsNotNullString);
            Assert.Equal("Bar", output.Child.IsNotNullString);
        }

        /// <summary>
        /// Runs a smoke test for the BindTo functionality.
        /// </summary>
        [Fact]
        public void BindToSmokeTest() =>
            new TestScheduler().With(scheduler =>
            {
                var input = new ScheduledSubject<string>(scheduler);
                var fixture = new HostTestFixture { Child = new TestFixture() };

                input.BindTo(fixture, x => x.Child!.IsNotNullString);

                Assert.Null(fixture.Child.IsNotNullString);

                input.OnNext("Foo");
                scheduler.Start();
                Assert.Equal("Foo", fixture.Child.IsNotNullString);

                input.OnNext("Bar");
                scheduler.Start();
                Assert.Equal("Bar", fixture.Child.IsNotNullString);
            });

        /// <summary>
        /// Tests to make sure that Disposing disconnects BindTo and updates are no longer pushed.
        /// </summary>
        [Fact]
        public void DisposingDisconnectsTheBindTo() =>
            new TestScheduler().With(scheduler =>
            {
                var input = new ScheduledSubject<string>(scheduler);
                var fixture = new HostTestFixture { Child = new TestFixture() };

                var subscription = input.BindTo(fixture, x => x.Child!.IsNotNullString);

                Assert.Null(fixture.Child.IsNotNullString);

                input.OnNext("Foo");
                scheduler.Start();
                Assert.Equal("Foo", fixture.Child.IsNotNullString);

                subscription.Dispose();

                input.OnNext("Bar");
                scheduler.Start();
                Assert.Equal("Foo", fixture.Child.IsNotNullString);
            });

        /// <summary>
        /// Tests to make sure that BindTo can handle intermediate object switching.
        /// </summary>
        [Fact]
        public void BindToIsNotFooledByIntermediateObjectSwitching() =>
            new TestScheduler().With(scheduler =>
            {
                var input = new ScheduledSubject<string>(scheduler);
                var fixture = new HostTestFixture { Child = new TestFixture() };

                input.BindTo(fixture, x => x.Child!.IsNotNullString);

                Assert.Null(fixture.Child.IsNotNullString);

                input.OnNext("Foo");
                scheduler.Start();
                Assert.Equal("Foo", fixture.Child.IsNotNullString);

                fixture.Child = new TestFixture();
                scheduler.Start();
                Assert.Equal("Foo", fixture.Child.IsNotNullString);

                input.OnNext("Bar");
                scheduler.Start();
                Assert.Equal("Bar", fixture.Child.IsNotNullString);
            });

        /// <summary>
        /// Tests to make sure that BindTo can handle Stack Overflow conditions.
        /// </summary>
        [Fact]
        public void BindToStackOverFlowTest()
        {
            // Before the code changes packed in the same commit
            // as this test the test would go into an infinite
            // event storm. The critical issue is that the
            // property StackOverflowTrigger will clone the
            // value before setting it.
            //
            // If this test executes through without hanging then
            // the problem has been fixed.
            new TestScheduler().With(_ =>
            {
                var fixtureA = new TestFixture();
                var fixtureB = new TestFixture();

                var source = new BehaviorSubject<List<string>>(new List<string>());

                source.BindTo(fixtureA, x => x.StackOverflowTrigger);
            });
        }
    }
}
