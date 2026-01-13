// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections;
using System.Text.Json;
using DynamicData;
using ReactiveUI.Tests.ReactiveObjects.Mocks;
using ReactiveUI.Tests.Utilities;

namespace ReactiveUI.Tests.ReactiveObjects;

public class ReactiveObjectTests
{
    /// <summary>
    ///     Test that changing values should always arrive before changed.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task ChangingShouldAlwaysArriveBeforeChanged()
    {
        const string beforeSet = "Foo";
        const string afterSet = "Bar";

        var fixture = new TestFixture { IsOnlyOneWord = beforeSet };

        var beforeFired = false;
        fixture.Changing.Subscribe(async x =>
        {
            using (Assert.Multiple())
            {
                // XXX: The content of these asserts don't actually get
                // propagated back, it only prevents before_fired from
                // being set - we have to enable 1st-chance exceptions
                // to see the real error
                await Assert.That(x.PropertyName).IsEqualTo("IsOnlyOneWord");
                await Assert.That(fixture.IsOnlyOneWord).IsEqualTo(beforeSet);
            }

            beforeFired = true;
        });

        var afterFired = false;
        fixture.Changed.Subscribe(async x =>
        {
            using (Assert.Multiple())
            {
                await Assert.That(x.PropertyName).IsEqualTo("IsOnlyOneWord");
                await Assert.That(fixture.IsOnlyOneWord).IsEqualTo(afterSet);
            }

            afterFired = true;
        });

        fixture.IsOnlyOneWord = afterSet;

        using (Assert.Multiple())
        {
            await Assert.That(beforeFired).IsTrue();
            await Assert.That(afterFired).IsTrue();
        }
    }

    /// <summary>
    ///     Test that deferring the notifications dont show up until undeferred.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task DeferredNotificationsDontShowUpUntilUndeferred()
    {
        var fixture = new TestFixture();
        fixture.Changing.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var changing).Subscribe();
        fixture.Changed.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var changed).Subscribe();
        var propertyChangingEvents = new List<PropertyChangingEventArgs>();
        fixture.PropertyChanging += (sender, args) => propertyChangingEvents.Add(args);
        var propertyChangedEvents = new List<PropertyChangedEventArgs>();
        fixture.PropertyChanged += (sender, args) => propertyChangedEvents.Add(args);

        await AssertCount(0, changing, changed, propertyChangingEvents, propertyChangedEvents);
        fixture.NullableInt = 4;
        await AssertCount(1, changing, changed, propertyChangingEvents, propertyChangedEvents);

        var stopDelaying = fixture.DelayChangeNotifications();

        fixture.NullableInt = 5;
        await AssertCount(1, changing, changed, propertyChangingEvents, propertyChangedEvents);

        fixture.IsNotNullString = "Bar";
        await AssertCount(1, changing, changed, propertyChangingEvents, propertyChangedEvents);

        fixture.NullableInt = 6;
        await AssertCount(1, changing, changed, propertyChangingEvents, propertyChangedEvents);

        fixture.IsNotNullString = "Baz";
        await AssertCount(1, changing, changed, propertyChangingEvents, propertyChangedEvents);

        var stopDelayingMore = fixture.DelayChangeNotifications();

        fixture.IsNotNullString = "Bamf";
        await AssertCount(1, changing, changed, propertyChangingEvents, propertyChangedEvents);

        stopDelaying.Dispose();

        fixture.IsNotNullString = "Blargh";
        await AssertCount(1, changing, changed, propertyChangingEvents, propertyChangedEvents);

        // NB: Because we debounce queued up notifications, we should only
        // see a notification from the latest NullableInt and the latest
        // IsNotNullableString
        stopDelayingMore.Dispose();

        await AssertCount(3, changing, changed, propertyChangingEvents, propertyChangedEvents);

        var expectedEventProperties = new[] { "NullableInt", "NullableInt", "IsNotNullString" };
        using (Assert.Multiple())
        {
            await Assert.That(changing.Select(e => e.PropertyName!)).IsEquivalentTo(expectedEventProperties);
            await Assert.That(changed.Select(e => e.PropertyName!)).IsEquivalentTo(expectedEventProperties);
            await Assert.That(propertyChangingEvents.Select(e => e.PropertyName!))
                .IsEquivalentTo(expectedEventProperties);
            await Assert.That(propertyChangedEvents.Select(e => e.PropertyName!))
                .IsEquivalentTo(expectedEventProperties);
        }
    }

    /// <summary>
    ///     Test that exceptions thrown in subscribers should marshal to thrown exceptions.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task ExceptionsThrownInSubscribersShouldMarshalToThrownExceptions()
    {
        var fixture = new TestFixture { IsOnlyOneWord = "Foo" };

        fixture.Changed.Subscribe(static _ => throw new Exception("Die!"));
        fixture.ThrownExceptions.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var exceptionList)
            .Subscribe();

        fixture.IsOnlyOneWord = "Bar";
        await Assert.That(exceptionList).Count().IsEqualTo(1);
    }

    /// <summary>
    ///     Tests that ObservableForProperty using expression.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task ObservableForPropertyUsingExpression()
    {
        var fixture = new TestFixture { IsNotNullString = "Foo", IsOnlyOneWord = "Baz" };
        var output = new List<IObservedChange<TestFixture, string?>>();
        fixture.ObservableForProperty(x => x.IsNotNullString)
            .WhereNotNull()
            .Subscribe(x => output.Add(x));

        fixture.IsNotNullString = "Bar";
        fixture.IsNotNullString = "Baz";
        fixture.IsNotNullString = "Baz";

        fixture.IsOnlyOneWord = "Bamf";

        await Assert.That(output).Count().IsEqualTo(2);

        using (Assert.Multiple())
        {
            await Assert.That(output[0].Sender).IsEqualTo(fixture);
            await Assert.That(output[0].GetPropertyName()).IsEqualTo("IsNotNullString");
            await Assert.That(output[0].Value).IsEqualTo("Bar");

            await Assert.That(output[1].Sender).IsEqualTo(fixture);
            await Assert.That(output[1].GetPropertyName()).IsEqualTo("IsNotNullString");
            await Assert.That(output[1].Value).IsEqualTo("Baz");
        }
    }

    /// <summary>
    ///     Test raises and set using expression.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task RaiseAndSetUsingExpression()
    {
        var fixture = new TestFixture { IsNotNullString = "Foo", IsOnlyOneWord = "Baz" };
        var output = new List<string>();
        fixture.Changed
            .Where(x => x.PropertyName is not null)
            .Select(x => x.PropertyName!)
            .Subscribe(x => output.Add(x));

        fixture.UsesExprRaiseSet = "Foo";
        fixture.UsesExprRaiseSet = "Foo"; // This one shouldn't raise a change notification

        using (Assert.Multiple())
        {
            await Assert.That(fixture.UsesExprRaiseSet).IsEqualTo("Foo");
            await Assert.That(output).Count().IsEqualTo(1);
        }

        await Assert.That(output[0]).IsEqualTo("UsesExprRaiseSet");
    }

    [Test]
    public async Task ReactiveObjectCanSuppressChangeNotifications()
    {
        var fixture = new TestFixture();
        using (fixture.SuppressChangeNotifications())
        {
            await Assert.That(fixture.AreChangeNotificationsEnabled()).IsFalse();
        }

        await Assert.That(fixture.AreChangeNotificationsEnabled()).IsTrue();

        var ser = JsonSerializer.Serialize(fixture);
        await Assert.That(ser).IsNotEmpty();
        var deser = JsonSerializer.Deserialize<TestFixture>(ser);
        await Assert.That(deser).IsNotNull();

        using (deser.SuppressChangeNotifications())
        {
            await Assert.That(deser!.AreChangeNotificationsEnabled()).IsFalse();
        }

        await Assert.That(deser!.AreChangeNotificationsEnabled()).IsTrue();
    }

    /// <summary>
    ///     Test that ReactiveObject shouldn't serialize anything extra.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task ReactiveObjectShouldntSerializeAnythingExtra()
    {
        var fixture = new TestFixture { IsNotNullString = "Foo", IsOnlyOneWord = "Baz" };
        var json = JSONHelper.Serialize(fixture) ??
                   throw new InvalidOperationException("JSON string should not be null");

        using (Assert.Multiple())
        {
            // Should look something like:
            // {"IsNotNullString":"Foo","IsOnlyOneWord":"Baz","NullableInt":null,"PocoProperty":null,"StackOverflowTrigger":null,"TestCollection":[],"UsesExprRaiseSet":null}
            await Assert.That(json.Count(static x => x == ',')).IsEqualTo(6);
            await Assert.That(json.Count(static x => x == ':')).IsEqualTo(7);
            await Assert.That(json.Count(static x => x == '"')).IsEqualTo(18);
        }
    }

    /// <summary>
    ///     Tests to make sure that ReactiveObject doesn't rethrow exceptions.
    /// </summary>
    [Test]
    public void ReactiveObjectShouldRethrowException()
    {
        var fixture = new TestFixture();
        var observable = fixture.WhenAnyValue(x => x.IsOnlyOneWord).Skip(1);
        observable.Subscribe(_ => throw new Exception("This is a test."));

        Assert.Throws<Exception>(() => fixture.IsOnlyOneWord = "Two Words");
    }

    /// <summary>
    ///     Performs a ReactiveObject smoke test.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task ReactiveObjectSmokeTest()
    {
        var outputChanging = new List<string>();
        var output = new List<string>();
        var fixture = new TestFixture();

        fixture.Changing
            .Where(x => x.PropertyName is not null)
            .Select(x => x.PropertyName!)
            .Subscribe(x => outputChanging.Add(x));
        fixture.Changed
            .Where(x => x.PropertyName is not null)
            .Select(x => x.PropertyName!)
            .Subscribe(x => output.Add(x));

        fixture.IsNotNullString = "Foo Bar Baz";
        fixture.IsOnlyOneWord = "Foo";
        fixture.IsOnlyOneWord = "Bar";
        fixture.IsNotNullString = null; // Sorry.
        fixture.IsNotNullString = null;

        var results = new[] { "IsNotNullString", "IsOnlyOneWord", "IsOnlyOneWord", "IsNotNullString" };

        await Assert.That(output).Count().IsEqualTo(results.Length);

        await output.AssertAreEqual(outputChanging);
        await results.AssertAreEqual(output);
    }

    private static async Task AssertCount(int expected, params ICollection[] collections)
    {
        foreach (var collection in collections)
        {
            await Assert.That(collection.Count).IsEqualTo(expected);
        }
    }
}
