// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections;
using System.Text.Json;
using DynamicData;

namespace ReactiveUI.Tests;

/// <summary>
/// Tests for the reactive object.
/// </summary>
public class ReactiveObjectTests
{
    /// <summary>
    /// Test that changing values should always arrive before changed.
    /// </summary>
    [Fact]
    public void ChangingShouldAlwaysArriveBeforeChanged()
    {
        const string beforeSet = "Foo";
        const string afterSet = "Bar";

        var fixture = new TestFixture
        {
            IsOnlyOneWord = beforeSet
        };

        var beforeFired = false;
        fixture.Changing.Subscribe(
                                   x =>
                                   {
                                       // XXX: The content of these asserts don't actually get
                                       // propagated back, it only prevents before_fired from
                                       // being set - we have to enable 1st-chance exceptions
                                       // to see the real error
                                       Assert.Equal("IsOnlyOneWord", x.PropertyName);
                                       Assert.Equal(fixture.IsOnlyOneWord, beforeSet);
                                       beforeFired = true;
                                   });

        var afterFired = false;
        fixture.Changed.Subscribe(
                                  x =>
                                  {
                                      Assert.Equal("IsOnlyOneWord", x.PropertyName);
                                      Assert.Equal(fixture.IsOnlyOneWord, afterSet);
                                      afterFired = true;
                                  });

        fixture.IsOnlyOneWord = afterSet;

        Assert.True(beforeFired);
        Assert.True(afterFired);
    }

    /// <summary>
    /// Test that deferring the notifications dont show up until undeferred.
    /// </summary>
    [Fact]
    public void DeferredNotificationsDontShowUpUntilUndeferred()
    {
        var fixture = new TestFixture();
        fixture.Changing.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var changing).Subscribe();
        fixture.Changed.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var changed).Subscribe();
        var propertyChangingEvents = new List<PropertyChangingEventArgs>();
        fixture.PropertyChanging += (sender, args) => propertyChangingEvents.Add(args);
        var propertyChangedEvents = new List<PropertyChangedEventArgs>();
        fixture.PropertyChanged += (sender, args) => propertyChangedEvents.Add(args);

        AssertCount(0, changing, changed, propertyChangingEvents, propertyChangedEvents);
        fixture.NullableInt = 4;
        AssertCount(1, changing, changed, propertyChangingEvents, propertyChangedEvents);

        var stopDelaying = fixture.DelayChangeNotifications();

        fixture.NullableInt = 5;
        AssertCount(1, changing, changed, propertyChangingEvents, propertyChangedEvents);

        fixture.IsNotNullString = "Bar";
        AssertCount(1, changing, changed, propertyChangingEvents, propertyChangedEvents);

        fixture.NullableInt = 6;
        AssertCount(1, changing, changed, propertyChangingEvents, propertyChangedEvents);

        fixture.IsNotNullString = "Baz";
        AssertCount(1, changing, changed, propertyChangingEvents, propertyChangedEvents);

        var stopDelayingMore = fixture.DelayChangeNotifications();

        fixture.IsNotNullString = "Bamf";
        AssertCount(1, changing, changed, propertyChangingEvents, propertyChangedEvents);

        stopDelaying.Dispose();

        fixture.IsNotNullString = "Blargh";
        AssertCount(1, changing, changed, propertyChangingEvents, propertyChangedEvents);

        // NB: Because we debounce queued up notifications, we should only
        // see a notification from the latest NullableInt and the latest
        // IsNotNullableString
        stopDelayingMore.Dispose();

        AssertCount(3, changing, changed, propertyChangingEvents, propertyChangedEvents);

        var expectedEventProperties = new[] { "NullableInt", "NullableInt", "IsNotNullString" };
        Assert.Equal(expectedEventProperties, changing.Select(e => e.PropertyName));
        Assert.Equal(expectedEventProperties, changed.Select(e => e.PropertyName));
        Assert.Equal(expectedEventProperties, propertyChangingEvents.Select(e => e.PropertyName));
        Assert.Equal(expectedEventProperties, propertyChangedEvents.Select(e => e.PropertyName));
    }

    /// <summary>
    /// Test that exceptions thrown in subscribers should marshal to thrown exceptions.
    /// </summary>
    [Fact]
    public void ExceptionsThrownInSubscribersShouldMarshalToThrownExceptions()
    {
        var fixture = new TestFixture
        {
            IsOnlyOneWord = "Foo"
        };

        fixture.Changed.Subscribe(_ => throw new Exception("Die!"));
        fixture.ThrownExceptions.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var exceptionList).Subscribe();

        fixture.IsOnlyOneWord = "Bar";
        Assert.Equal(1, exceptionList.Count);
    }

    /// <summary>
    /// Tests that ObservableForProperty using expression.
    /// </summary>
    [Fact]
    public void ObservableForPropertyUsingExpression()
    {
        var fixture = new TestFixture
        {
            IsNotNullString = "Foo",
            IsOnlyOneWord = "Baz"
        };
        var output = new List<IObservedChange<TestFixture, string?>>();
        fixture.ObservableForProperty(x => x.IsNotNullString)
               .WhereNotNull()
               .Subscribe(x => output.Add(x));

        fixture.IsNotNullString = "Bar";
        fixture.IsNotNullString = "Baz";
        fixture.IsNotNullString = "Baz";

        fixture.IsOnlyOneWord = "Bamf";

        Assert.Equal(2, output.Count);

        Assert.Equal(fixture, output[0].Sender);
        Assert.Equal("IsNotNullString", output[0].GetPropertyName());
        Assert.Equal("Bar", output[0].Value);

        Assert.Equal(fixture, output[1].Sender);
        Assert.Equal("IsNotNullString", output[1].GetPropertyName());
        Assert.Equal("Baz", output[1].Value);
    }

    /// <summary>
    /// Test raises and set using expression.
    /// </summary>
    [Fact]
    public void RaiseAndSetUsingExpression()
    {
        var fixture = new TestFixture
        {
            IsNotNullString = "Foo",
            IsOnlyOneWord = "Baz"
        };
        var output = new List<string>();
        fixture.Changed
               .Where(x => x.PropertyName is not null)
               .Select(x => x.PropertyName!)
               .Subscribe(x => output.Add(x));

        fixture.UsesExprRaiseSet = "Foo";
        fixture.UsesExprRaiseSet = "Foo"; // This one shouldn't raise a change notification

        Assert.Equal("Foo", fixture.UsesExprRaiseSet);
        Assert.Equal(1, output.Count);
        Assert.Equal("UsesExprRaiseSet", output[0]);
    }

    /// <summary>
    /// Test that ReactiveObject shouldn't serialize anything extra.
    /// </summary>
    [Fact]
    public void ReactiveObjectShouldntSerializeAnythingExtra()
    {
        var fixture = new TestFixture
        {
            IsNotNullString = "Foo",
            IsOnlyOneWord = "Baz"
        };
        var json = JSONHelper.Serialize(fixture) ?? throw new InvalidOperationException("JSON string should not be null");

        // Should look something like:
        // {"IsNotNullString":"Foo","IsOnlyOneWord":"Baz","NullableInt":null,"PocoProperty":null,"StackOverflowTrigger":null,"TestCollection":[],"UsesExprRaiseSet":null}
        Assert.True(json.Count(x => x == ',') == 6);
        Assert.True(json.Count(x => x == ':') == 7);
        Assert.True(json.Count(x => x == '"') == 18);
    }

    /// <summary>
    /// Performs a ReactiveObject smoke test.
    /// </summary>
    [Fact]
    public void ReactiveObjectSmokeTest()
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

        Assert.Equal(results.Length, output.Count);

        output.AssertAreEqual(outputChanging);
        results.AssertAreEqual(output);
    }

    /// <summary>
    /// Tests to make sure that ReactiveObject doesn't rethrow exceptions.
    /// </summary>
    [Fact]
    public void ReactiveObjectShouldRethrowException()
    {
        var fixture = new TestFixture();
        var observable = fixture.WhenAnyValue(x => x.IsOnlyOneWord).Skip(1);
        observable.Subscribe(_ => throw new Exception("This is a test."));

        var result = Record.Exception(() => fixture.IsOnlyOneWord = "Two Words");
    }

    [Fact]
    public void ReactiveObjectCanSuppressChangeNotifications()
    {
        var fixture = new TestFixture();
        using (fixture.SuppressChangeNotifications())
        {
            Assert.False(fixture.AreChangeNotificationsEnabled());
        }

        Assert.True(fixture.AreChangeNotificationsEnabled());

        var ser = JsonSerializer.Serialize(fixture);
        Assert.True(ser.Length > 0);
        var deser = JsonSerializer.Deserialize<TestFixture>(ser);
        Assert.NotNull(deser);

        using (deser.SuppressChangeNotifications())
        {
            Assert.False(deser.AreChangeNotificationsEnabled());
        }

        Assert.True(deser.AreChangeNotificationsEnabled());
    }

    private static void AssertCount(int expected, params ICollection[] collections)
    {
        foreach (var collection in collections)
        {
            Assert.Equal(expected, collection.Count);
        }
    }
}
