// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Microsoft.Reactive.Testing;

using ReactiveUI.Testing;

using TUnit.Assertions;
using TUnit.Assertions.Extensions;
using TUnit.Core;

using static TUnit.Assertions.Assert;

namespace ReactiveUI.Tests;

/// <summary>
/// Tests for the ObservedChangedMixin.
/// </summary>
public class ObservedChangedMixinTest
{
    /// <summary>
    /// Tests that getting the value should actually return the value.
    /// </summary>
    [Test]
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
                .Subscribe(x => output.Add(x));

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
    [Test]
    public void GetValueShouldReturnTheValueFromAPath()
    {
        var input = new HostTestFixture
        {
            Child = new TestFixture { IsNotNullString = "Foo" },
        };

        Expression<Func<HostTestFixture, string>> expression = static x => x!.Child!.IsNotNullString!;
        var fixture = new ObservedChange<HostTestFixture, string?>(input, expression.Body, null);

        Assert.That(fixture.GetValue(), Is.EqualTo("Foo"));
    }

    /// <summary>
    /// Runs a smoke test that sets the value path.
    /// </summary>
    [Test]
    public void SetValuePathSmokeTest()
    {
        var output = new HostTestFixture
        {
            Child = new TestFixture { IsNotNullString = "Foo" },
        };

        Expression<Func<TestFixture, string>> expression = static x => x.IsOnlyOneWord!;
        var fixture = new ObservedChange<TestFixture, string?>(new TestFixture { IsOnlyOneWord = "Bar" }, expression.Body, null);

        fixture.SetValueToProperty(output, static x => x.Child!.IsNotNullString);
        Assert.That(output.Child.IsNotNullString, Is.EqualTo("Bar"));
    }

    /// <summary>
    /// Runs a smoke test for the BindTo functionality.
    /// </summary>
    [Test]
    public void BindToSmokeTest() =>
        new TestScheduler().With(static scheduler =>
        {
            var input = new ScheduledSubject<string>(scheduler);
            var fixture = new HostTestFixture { Child = new TestFixture() };

            input.BindTo(fixture, static x => x.Child!.IsNotNullString);

            Assert.That(fixture.Child.IsNotNullString, Is.Null);

            input.OnNext("Foo");
            scheduler.Start();
            Assert.That(fixture.Child.IsNotNullString, Is.EqualTo("Foo"));

            input.OnNext("Bar");
            scheduler.Start();
            Assert.That(fixture.Child.IsNotNullString, Is.EqualTo("Bar"));
        });

    /// <summary>
    /// Tests to make sure that Disposing disconnects BindTo and updates are no longer pushed.
    /// </summary>
    [Test]
    public void DisposingDisconnectsTheBindTo() =>
        new TestScheduler().With(static scheduler =>
        {
            var input = new ScheduledSubject<string>(scheduler);
            var fixture = new HostTestFixture { Child = new TestFixture() };

            var subscription = input.BindTo(fixture, static x => x.Child!.IsNotNullString);

            Assert.That(fixture.Child.IsNotNullString, Is.Null);

            input.OnNext("Foo");
            scheduler.Start();
            Assert.That(fixture.Child.IsNotNullString, Is.EqualTo("Foo"));

            subscription.Dispose();

            input.OnNext("Bar");
            scheduler.Start();
            Assert.That(fixture.Child.IsNotNullString, Is.EqualTo("Foo"));
        });

    /// <summary>
    /// Tests to make sure that BindTo can handle intermediate object switching.
    /// </summary>
    [Test]
    public void BindToIsNotFooledByIntermediateObjectSwitching() =>
        new TestScheduler().With(static scheduler =>
        {
            var input = new ScheduledSubject<string>(scheduler);
            var fixture = new HostTestFixture { Child = new TestFixture() };

            input.BindTo(fixture, static x => x.Child!.IsNotNullString);

            Assert.That(fixture.Child.IsNotNullString, Is.Null);

            input.OnNext("Foo");
            scheduler.Start();
            Assert.That(fixture.Child!.IsNotNullString, Is.EqualTo("Foo"));

            fixture.Child = new TestFixture();
            scheduler.Start();
            Assert.That(fixture.Child!.IsNotNullString, Is.EqualTo("Foo"));

            input.OnNext("Bar");
            scheduler.Start();
            Assert.That(fixture.Child!.IsNotNullString, Is.EqualTo("Bar"));
        });

    /// <summary>
    /// Tests to make sure that BindTo can handle Stack Overflow conditions.
    /// </summary>
    [Test]
    public void BindToStackOverFlowTest() =>
        new TestScheduler().With(static _ =>
        {
            // Before the code changes packed in the same commit
            // as this test the test would go into an infinite
            // event storm. The critical issue is that the
            // property StackOverflowTrigger will clone the
            // value before setting it.
            //
            // If this test executes through without hanging then
            // the problem has been fixed.
            var fixtureA = new TestFixture();
            var fixtureB = new TestFixture();

            var source = new BehaviorSubject<List<string>>([]);

            source.BindTo(fixtureA, static x => x.StackOverflowTrigger);
        });
}