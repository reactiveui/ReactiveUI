// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections;
using System.ComponentModel;
using System.Text.Json;
using ReactiveUI.Tests.ReactiveObjects.Mocks;
using ReactiveUI.Tests.Utilities;

namespace ReactiveUI.Tests.ReactiveObjects;

/// <summary>Tests for <see cref="ReactiveObject" /> change notification behavior.</summary>
public class ReactiveObjectTests
{
    /// <summary>The "Foo" text value used in property change tests.</summary>
    private const string FooText = "Foo";

    /// <summary>The "Bar" text value used in property change tests.</summary>
    private const string BarText = "Bar";

    /// <summary>The "Baz" text value used in property change tests.</summary>
    private const string BazText = "Baz";

    /// <summary>The property name for the IsNotNullString property.</summary>
    private const string IsNotNullStringName = "IsNotNullString";

    /// <summary>The property name for the IsOnlyOneWord property.</summary>
    private const string IsOnlyOneWordName = "IsOnlyOneWord";

    /// <summary>The property name for the NullableInt property.</summary>
    private const string NullableIntName = "NullableInt";

    /// <summary>Test that changing values should always arrive before changed.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task ChangingShouldAlwaysArriveBeforeChanged()
    {
        const string BeforeSet = FooText;
        const string AfterSet = BarText;

        var fixture = new TestFixture { IsOnlyOneWord = BeforeSet };

        var beforeFired = false;
        string? changingPropertyName = null;
        string? changingValue = null;
        _ = fixture.Changing.Subscribe(x =>
        {
            changingPropertyName = x.PropertyName;
            changingValue = fixture.IsOnlyOneWord;
            beforeFired = true;
        });

        var afterFired = false;
        string? changedPropertyName = null;
        string? changedValue = null;
        _ = fixture.Changed.Subscribe(x =>
        {
            changedPropertyName = x.PropertyName;
            changedValue = fixture.IsOnlyOneWord;
            afterFired = true;
        });

        fixture.IsOnlyOneWord = AfterSet;

        using (Assert.Multiple())
        {
            await Assert.That(beforeFired).IsTrue();
            await Assert.That(afterFired).IsTrue();
            await Assert.That(changingPropertyName).IsEqualTo(IsOnlyOneWordName);
            await Assert.That(changingValue).IsEqualTo(BeforeSet);
            await Assert.That(changedPropertyName).IsEqualTo(IsOnlyOneWordName);
            await Assert.That(changedValue).IsEqualTo(AfterSet);
        }
    }

    /// <summary>Test that deferring the notifications dont show up until undeferred.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task DeferredNotificationsDontShowUpUntilUndeferred()
    {
        const int FirstInt = 4;
        const int SecondInt = 5;
        const int ThirdInt = 6;
        const int ExpectedCountAfterUndefer = 3;
        var fixture = new TestFixture();
        var changing = fixture.Changing.Collect();
        var changed = fixture.Changed.Collect();
        var propertyChangingEvents = new List<PropertyChangingEventArgs>();
        fixture.PropertyChanging += (_, args) => propertyChangingEvents.Add(args);
        var propertyChangedEvents = new List<PropertyChangedEventArgs>();
        fixture.PropertyChanged += (_, args) => propertyChangedEvents.Add(args);

        await AssertCount(0, changing, changed, propertyChangingEvents, propertyChangedEvents);
        fixture.NullableInt = FirstInt;
        await AssertCount(1, changing, changed, propertyChangingEvents, propertyChangedEvents);

        var stopDelaying = fixture.DelayChangeNotifications();

        fixture.NullableInt = SecondInt;
        await AssertCount(1, changing, changed, propertyChangingEvents, propertyChangedEvents);

        fixture.IsNotNullString = BarText;
        await AssertCount(1, changing, changed, propertyChangingEvents, propertyChangedEvents);

        fixture.NullableInt = ThirdInt;
        await AssertCount(1, changing, changed, propertyChangingEvents, propertyChangedEvents);

        fixture.IsNotNullString = BazText;
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

        await AssertCount(ExpectedCountAfterUndefer, changing, changed, propertyChangingEvents, propertyChangedEvents);

        var expectedEventProperties = new[] { NullableIntName, NullableIntName, IsNotNullStringName };
        using (Assert.Multiple())
        {
            await Assert.That(changing.Select(e => e.PropertyName!)).IsEquivalentTo(expectedEventProperties);
            await Assert.That(changed.Select(e => e.PropertyName!)).IsEquivalentTo(expectedEventProperties);
            await Assert.That(propertyChangingEvents.Select(e => e.PropertyName!)).IsEquivalentTo(expectedEventProperties);
            await Assert.That(propertyChangedEvents.Select(e => e.PropertyName!)).IsEquivalentTo(expectedEventProperties);
        }
    }

    /// <summary>Test that exceptions thrown in subscribers should marshal to thrown exceptions.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task ExceptionsThrownInSubscribersShouldMarshalToThrownExceptions()
    {
        var fixture = new TestFixture { IsOnlyOneWord = FooText };

        _ = fixture.Changed.Subscribe(static _ => throw new InvalidOperationException("Die!"));
        var exceptionList = fixture.ThrownExceptions.Collect();

        fixture.IsOnlyOneWord = BarText;
        await Assert.That(exceptionList).Count().IsEqualTo(1);
    }

    /// <summary>Tests that ObservableForProperty using expression.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task ObservableForPropertyUsingExpression()
    {
        const int ExpectedCount = 2;
        var fixture = new TestFixture { IsNotNullString = FooText, IsOnlyOneWord = BazText };
        var output = new List<IObservedChange<TestFixture, string?>>();
        _ = ObservableMixins.WhereNotNull(fixture.ObservableForProperty(x => x.IsNotNullString)).Subscribe(output.Add);

        fixture.IsNotNullString = BarText;
        fixture.IsNotNullString = BazText;
        fixture.IsNotNullString = BazText;

        fixture.IsOnlyOneWord = "Bamf";

        await Assert.That(output).Count().IsEqualTo(ExpectedCount);

        using (Assert.Multiple())
        {
            await Assert.That(output[0].Sender).IsEqualTo(fixture);
            await Assert.That(output[0].GetPropertyName()).IsEqualTo(IsNotNullStringName);
            await Assert.That(output[0].Value).IsEqualTo(BarText);

            await Assert.That(output[1].Sender).IsEqualTo(fixture);
            await Assert.That(output[1].GetPropertyName()).IsEqualTo(IsNotNullStringName);
            await Assert.That(output[1].Value).IsEqualTo(BazText);
        }
    }

    /// <summary>Test raises and set using expression.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task RaiseAndSetUsingExpression()
    {
        var fixture = new TestFixture { IsNotNullString = FooText, IsOnlyOneWord = BazText };
        var output = new List<string>();
        _ = fixture.Changed.Where(x => x.PropertyName is not null).Select(x => x.PropertyName!).Subscribe(output.Add);

        fixture.UsesExprRaiseSet = FooText;
        fixture.UsesExprRaiseSet = FooText; // This one shouldn't raise a change notification

        using (Assert.Multiple())
        {
            await Assert.That(fixture.UsesExprRaiseSet).IsEqualTo(FooText);
            await Assert.That(output).Count().IsEqualTo(1);
        }

        await Assert.That(output[0]).IsEqualTo("UsesExprRaiseSet");
    }

    /// <summary>Test that change notifications can be suppressed and re-enabled, including after serialization.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
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
            await Assert.That(deser.AreChangeNotificationsEnabled()).IsFalse();
        }

        await Assert.That(deser.AreChangeNotificationsEnabled()).IsTrue();
    }

    /// <summary>Test that ReactiveObject shouldn't serialize anything extra.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task ReactiveObjectShouldntSerializeAnythingExtra()
    {
        var fixture = new TestFixture { IsNotNullString = FooText, IsOnlyOneWord = BazText };
        var json = JsonHelper.Serialize(fixture) ??
                   throw new InvalidOperationException("JSON string should not be null");

        using (Assert.Multiple())
        {
            // The serialized JSON is expected to contain IsNotNullString, IsOnlyOneWord, NullableInt,
            // StackOverflowTrigger, TestCollection and UsesExprRaiseSet members.
            // PocoProperty is excluded because it lacks the DataMember attribute.
            const int ExpectedCommaCount = 5;
            const int ExpectedColonCount = 6;
            const int ExpectedQuoteCount = 16;
            await Assert.That(json.Count(static x => x == ',')).IsEqualTo(ExpectedCommaCount);
            await Assert.That(json.Count(static x => x == ':')).IsEqualTo(ExpectedColonCount);
            await Assert.That(json.Count(static x => x == '"')).IsEqualTo(ExpectedQuoteCount);
        }
    }

    /// <summary>Tests to make sure that ReactiveObject doesn't rethrow exceptions.</summary>
    [Test]
    public void ReactiveObjectShouldRethrowException()
    {
        var fixture = new TestFixture();
        var observable = fixture.WhenAnyValue(x => x.IsOnlyOneWord).Skip(1);
        _ = observable.Subscribe(_ => throw new InvalidOperationException("This is a test."));

        _ = Assert.Throws<Exception>(() => fixture.IsOnlyOneWord = "Two Words");
    }

    /// <summary>Performs a ReactiveObject smoke test.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task ReactiveObjectSmokeTest()
    {
        var outputChanging = new List<string>();
        var output = new List<string>();
        var fixture = new TestFixture();

        _ = fixture.Changing.Where(x => x.PropertyName is not null).Select(x => x.PropertyName!).Subscribe(outputChanging.Add);
        _ = fixture.Changed.Where(x => x.PropertyName is not null).Select(x => x.PropertyName!).Subscribe(output.Add);

        fixture.IsNotNullString = "Foo Bar Baz";
        fixture.IsOnlyOneWord = FooText;
        fixture.IsOnlyOneWord = BarText;
        fixture.IsNotNullString = null; // Sorry.
        fixture.IsNotNullString = null;

        var results = new[] { IsNotNullStringName, IsOnlyOneWordName, IsOnlyOneWordName, IsNotNullStringName };

        await Assert.That(output).Count().IsEqualTo(results.Length);

        await output.AssertAreEqual(outputChanging);
        await results.AssertAreEqual(output);
    }

    /// <summary>Asserts that every supplied collection has the expected element count.</summary>
    /// <param name="expected">The expected count.</param>
    /// <param name="collections">The collections to check.</param>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    private static async Task AssertCount(int expected, params ICollection[] collections)
    {
        foreach (var collection in collections)
        {
            await Assert.That(collection.Count).IsEqualTo(expected);
        }
    }
}
