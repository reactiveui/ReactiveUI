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
[TestFixture]
public class ReactiveObjectTests
{
    /// <summary>
    /// Test that changing values should always arrive before changed.
    /// </summary>
    [Test]
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
                                       using (Assert.EnterMultipleScope())
                                       {
                                           // XXX: The content of these asserts don't actually get
                                           // propagated back, it only prevents before_fired from
                                           // being set - we have to enable 1st-chance exceptions
                                           // to see the real error
                                           Assert.That(x.PropertyName, Is.EqualTo("IsOnlyOneWord"));
                                           Assert.That(fixture.IsOnlyOneWord, Is.EqualTo(beforeSet));
                                       }

                                       beforeFired = true;
                                   });

        var afterFired = false;
        fixture.Changed.Subscribe(
                                  x =>
                                  {
                                      using (Assert.EnterMultipleScope())
                                      {
                                          Assert.That(x.PropertyName, Is.EqualTo("IsOnlyOneWord"));
                                          Assert.That(fixture.IsOnlyOneWord, Is.EqualTo(afterSet));
                                      }

                                      afterFired = true;
                                  });

        fixture.IsOnlyOneWord = afterSet;

        using (Assert.EnterMultipleScope())
        {
            Assert.That(beforeFired, Is.True);
            Assert.That(afterFired, Is.True);
        }
    }

    /// <summary>
    /// Test that deferring the notifications dont show up until undeferred.
    /// </summary>
    [Test]
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
        using (Assert.EnterMultipleScope())
        {
            Assert.That(changing.Select(e => e.PropertyName), Is.EqualTo(expectedEventProperties));
            Assert.That(changed.Select(e => e.PropertyName), Is.EqualTo(expectedEventProperties));
            Assert.That(propertyChangingEvents.Select(e => e.PropertyName), Is.EqualTo(expectedEventProperties));
            Assert.That(propertyChangedEvents.Select(e => e.PropertyName), Is.EqualTo(expectedEventProperties));
        }
    }

    /// <summary>
    /// Test that exceptions thrown in subscribers should marshal to thrown exceptions.
    /// </summary>
    [Test]
    public void ExceptionsThrownInSubscribersShouldMarshalToThrownExceptions()
    {
        var fixture = new TestFixture
        {
            IsOnlyOneWord = "Foo"
        };

        fixture.Changed.Subscribe(_ => throw new Exception("Die!"));
        fixture.ThrownExceptions.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var exceptionList).Subscribe();

        fixture.IsOnlyOneWord = "Bar";
        Assert.That(exceptionList, Has.Count.EqualTo(1));
    }

    /// <summary>
    /// Tests that ObservableForProperty using expression.
    /// </summary>
    [Test]
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

        Assert.That(output, Has.Count.EqualTo(2));

        using (Assert.EnterMultipleScope())
        {
            Assert.That(output[0].Sender, Is.EqualTo(fixture));
            Assert.That(output[0].GetPropertyName(), Is.EqualTo("IsNotNullString"));
            Assert.That(output[0].Value, Is.EqualTo("Bar"));

            Assert.That(output[1].Sender, Is.EqualTo(fixture));
            Assert.That(output[1].GetPropertyName(), Is.EqualTo("IsNotNullString"));
            Assert.That(output[1].Value, Is.EqualTo("Baz"));
        }
    }

    /// <summary>
    /// Test raises and set using expression.
    /// </summary>
    [Test]
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

        using (Assert.EnterMultipleScope())
        {
            Assert.That(fixture.UsesExprRaiseSet, Is.EqualTo("Foo"));
            Assert.That(output, Has.Count.EqualTo(1));
        }

        Assert.That(output[0], Is.EqualTo("UsesExprRaiseSet"));
    }

    /// <summary>
    /// Test that ReactiveObject shouldn't serialize anything extra.
    /// </summary>
    [Test]
    public void ReactiveObjectShouldntSerializeAnythingExtra()
    {
        var fixture = new TestFixture
        {
            IsNotNullString = "Foo",
            IsOnlyOneWord = "Baz"
        };
        var json = JSONHelper.Serialize(fixture) ?? throw new InvalidOperationException("JSON string should not be null");

        using (Assert.EnterMultipleScope())
        {
            // Should look something like:
            // {"IsNotNullString":"Foo","IsOnlyOneWord":"Baz","NullableInt":null,"PocoProperty":null,"StackOverflowTrigger":null,"TestCollection":[],"UsesExprRaiseSet":null}
            Assert.That(json.Count(x => x == ','), Is.EqualTo(6));
            Assert.That(json.Count(x => x == ':'), Is.EqualTo(7));
            Assert.That(json.Count(x => x == '"'), Is.EqualTo(18));
        }
    }

    /// <summary>
    /// Performs a ReactiveObject smoke test.
    /// </summary>
    [Test]
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

        Assert.That(output, Has.Count.EqualTo(results.Length));

        output.AssertAreEqual(outputChanging);
        results.AssertAreEqual(output);
    }

    /// <summary>
    /// Tests to make sure that ReactiveObject doesn't rethrow exceptions.
    /// </summary>
    [Test]
    public void ReactiveObjectShouldRethrowException()
    {
        var fixture = new TestFixture();
        var observable = fixture.WhenAnyValue(x => x.IsOnlyOneWord).Skip(1);
        observable.Subscribe(_ => throw new Exception("This is a test."));

        Assert.Throws<Exception>(() => fixture.IsOnlyOneWord = "Two Words");
    }

    [Test]
    public void ReactiveObjectCanSuppressChangeNotifications()
    {
        var fixture = new TestFixture();
        using (fixture.SuppressChangeNotifications())
        {
            Assert.That(fixture.AreChangeNotificationsEnabled(), Is.False);
        }

        Assert.That(fixture.AreChangeNotificationsEnabled(), Is.True);

        var ser = JsonSerializer.Serialize(fixture);
        Assert.That(ser, Is.Not.Empty);
        var deser = JsonSerializer.Deserialize<TestFixture>(ser);
        Assert.That(deser, Is.Not.Null);

        using (deser.SuppressChangeNotifications())
        {
            Assert.That(deser!.AreChangeNotificationsEnabled(), Is.False);
        }

        Assert.That(deser!.AreChangeNotificationsEnabled(), Is.True);
    }

    private static void AssertCount(int expected, params ICollection[] collections)
    {
        foreach (var collection in collections)
        {
            Assert.That(collection, Has.Count.EqualTo(expected));
        }
    }
}
