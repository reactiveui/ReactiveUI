// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Microsoft.Reactive.Testing;

using ReactiveUI.Testing;

namespace ReactiveUI.Tests;

/// <summary>
/// Tests for the ObservedChangedMixin.
/// </summary>
public class ObservedChangedMixinTest
{
    /// <summary>
    /// Tests that getting the value should actually return the value.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task GetValueShouldActuallyReturnTheValue()
    {
        var input = new[] { "Foo", "Bar", "Baz" };
        var output = new List<string>();

        await new TestScheduler().With(async scheduler =>
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

            await input.AssertAreEqual(output);
        });
    }

    /// <summary>
    /// Tests that getting the value should return the value from a path.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task GetValueShouldReturnTheValueFromAPath()
    {
        var input = new HostTestFixture
        {
            Child = new TestFixture { IsNotNullString = "Foo" },
        };

        Expression<Func<HostTestFixture, string>> expression = static x => x!.Child!.IsNotNullString!;
        var fixture = new ObservedChange<HostTestFixture, string?>(input, expression.Body, null);

        await Assert.That(fixture.GetValue()).IsEqualTo("Foo");
    }

    /// <summary>
    /// Runs a smoke test that sets the value path.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task SetValuePathSmokeTest()
    {
        var output = new HostTestFixture
        {
            Child = new TestFixture { IsNotNullString = "Foo" },
        };

        Expression<Func<TestFixture, string>> expression = static x => x.IsOnlyOneWord!;
        var fixture = new ObservedChange<TestFixture, string?>(new TestFixture { IsOnlyOneWord = "Bar" }, expression.Body, null);

        fixture.SetValueToProperty(output, static x => x.Child!.IsNotNullString);
        await Assert.That(output.Child.IsNotNullString).IsEqualTo("Bar");
    }

    /// <summary>
    /// Runs a smoke test for the BindTo functionality.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task BindToSmokeTest() =>
        await new TestScheduler().With(static async scheduler =>
        {
            var input = new ScheduledSubject<string>(scheduler);
            var fixture = new HostTestFixture { Child = new TestFixture() };

            input.BindTo(fixture, static x => x.Child!.IsNotNullString);

            await Assert.That(fixture.Child.IsNotNullString).IsNull();

            input.OnNext("Foo");
            scheduler.Start();
            await Assert.That(fixture.Child.IsNotNullString).IsEqualTo("Foo");

            input.OnNext("Bar");
            scheduler.Start();
            await Assert.That(fixture.Child.IsNotNullString).IsEqualTo("Bar");
        });

    /// <summary>
    /// Tests to make sure that Disposing disconnects BindTo and updates are no longer pushed.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task DisposingDisconnectsTheBindTo() =>
        await new TestScheduler().With(static async scheduler =>
        {
            var input = new ScheduledSubject<string>(scheduler);
            var fixture = new HostTestFixture { Child = new TestFixture() };

            var subscription = input.BindTo(fixture, static x => x.Child!.IsNotNullString);

            await Assert.That(fixture.Child.IsNotNullString).IsNull();

            input.OnNext("Foo");
            scheduler.Start();
            await Assert.That(fixture.Child.IsNotNullString).IsEqualTo("Foo");

            subscription.Dispose();

            input.OnNext("Bar");
            scheduler.Start();
            await Assert.That(fixture.Child.IsNotNullString).IsEqualTo("Foo");
        });

    /// <summary>
    /// Tests to make sure that BindTo can handle intermediate object switching.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task BindToIsNotFooledByIntermediateObjectSwitching() =>
        await new TestScheduler().With(static async scheduler =>
        {
            var input = new ScheduledSubject<string>(scheduler);
            var fixture = new HostTestFixture { Child = new TestFixture() };

            input.BindTo(fixture, static x => x.Child!.IsNotNullString);

            await Assert.That(fixture.Child.IsNotNullString).IsNull();

            input.OnNext("Foo");
            scheduler.Start();
            await Assert.That(fixture.Child!.IsNotNullString).IsEqualTo("Foo");

            fixture.Child = new TestFixture();
            scheduler.Start();
            await Assert.That(fixture.Child!.IsNotNullString).IsEqualTo("Foo");

            input.OnNext("Bar");
            scheduler.Start();
            await Assert.That(fixture.Child!.IsNotNullString).IsEqualTo("Bar");
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

    /// <summary>
    /// Tests that GetValueOrDefault returns value when property is not null.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task GetValueOrDefault_WithValue_ReturnsValue()
    {
        var input = new HostTestFixture
        {
            Child = new TestFixture { IsNotNullString = "Foo" },
        };

        Expression<Func<HostTestFixture, string>> expression = static x => x!.Child!.IsNotNullString!;
        var fixture = new ObservedChange<HostTestFixture, string?>(input, expression.Body, null);

        await Assert.That(fixture.GetValueOrDefault()).IsEqualTo("Foo");
    }

    /// <summary>
    /// Tests that GetValueOrDefault returns default when property chain is null.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task GetValueOrDefault_WithNullInChain_ReturnsDefault()
    {
        var input = new HostTestFixture
        {
            Child = null,
        };

        Expression<Func<HostTestFixture, string>> expression = static x => x!.Child!.IsNotNullString!;
        var fixture = new ObservedChange<HostTestFixture, string?>(input, expression.Body, null);

        await Assert.That(fixture.GetValueOrDefault()).IsNull();
    }

    /// <summary>
    /// Tests that GetValueOrDefault throws for null item.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task GetValueOrDefault_NullItem_Throws()
    {
        IObservedChange<TestFixture, string?> item = null!;

        await Assert.That(() => item.GetValueOrDefault())
            .Throws<ArgumentNullException>();
    }

    /// <summary>
    /// Tests that Value extension method converts changes to values.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task Value_ConvertsChangesToValues()
    {
        var input = new[] { "Foo", "Bar", "Baz" };
        var output = new List<string>();

        await new TestScheduler().With(async scheduler =>
        {
            var fixture = new TestFixture();

            fixture.ObservableForProperty(x => x.IsOnlyOneWord)
                .Value()
                .WhereNotNull()
                .Subscribe(x => output.Add(x));

            foreach (var v in input)
            {
                fixture.IsOnlyOneWord = v;
            }

            scheduler.AdvanceToMs(1000);

            await input.AssertAreEqual(output);
        });
    }

    /// <summary>
    /// Tests that GetPropertyName returns the property name.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task GetPropertyName_ReturnsPropertyName()
    {
        var input = new TestFixture { IsOnlyOneWord = "Foo" };
        Expression<Func<TestFixture, string>> expression = static x => x.IsOnlyOneWord!;
        var fixture = new ObservedChange<TestFixture, string?>(input, expression.Body, null);

        await Assert.That(fixture.GetPropertyName()).IsEqualTo("IsOnlyOneWord");
    }

    /// <summary>
    /// Tests that GetPropertyName throws for null item.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task GetPropertyName_NullItem_Throws()
    {
        IObservedChange<TestFixture, string?> item = null!;

        await Assert.That(() => item.GetPropertyName())
            .Throws<ArgumentNullException>();
    }

    /// <summary>
    /// Tests that GetValue throws for null item.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task GetValue_NullItem_Throws()
    {
        IObservedChange<TestFixture, string?> item = null!;

        await Assert.That(() => item.GetValue())
            .Throws<ArgumentNullException>();
    }
}
