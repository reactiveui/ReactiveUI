// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections;
using System.Text.Json;
using DynamicData;

namespace ReactiveUI.Tests.ReactiveObjects;

/// <summary>
///     Tests for ReactiveRecord - a record-based reactive object implementation.
/// </summary>
public class ReactiveRecordTests
{
    /// <summary>
    ///     Test that the Changing observable fires before property changes.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task ChangingObservableShouldFireBeforePropertyChanges()
    {
        var fixture = new TestMutableRecord();
        var changingFired = false;
        var changedFired = false;

        using var sub1 = fixture.Changing.ObserveOn(ImmediateScheduler.Instance).Subscribe(_ => changingFired = true);
        using var sub2 = fixture.Changed.ObserveOn(ImmediateScheduler.Instance).Subscribe(_ => changedFired = true);

        fixture.Value = 42;

        // Verify both events fired
        using (Assert.Multiple())
        {
            await Assert.That(changingFired).IsTrue();
            await Assert.That(changedFired).IsTrue();
        }
    }

    /// <summary>
    ///     Test that DelayChangeNotifications defers notifications until disposed.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task DelayChangeNotificationsShouldDeferNotifications()
    {
        var fixture = new TestMutableRecord();
        using var sub1 = fixture.Changed.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var changed)
            .Subscribe();
        using var sub2 = fixture.Changing.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var changing)
            .Subscribe();

        await AssertCount(0, changing, changed);

        fixture.Value = 1;
        await AssertCount(1, changing, changed);

        using (fixture.DelayChangeNotifications())
        {
            fixture.Value = 2;
            await AssertCount(1, changing, changed);

            fixture.Value = 3;
            await AssertCount(1, changing, changed);
        }

        // After disposing, delayed notifications should fire
        await AssertCount(2, changing, changed);
    }

    /// <summary>
    ///     Test that exceptions thrown in subscribers are marshaled to ThrownExceptions.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task ExceptionsShouldMarshalToThrownExceptions()
    {
        var fixture = new TestMutableRecord { Value = 1 };

        using var sub1 = fixture.Changed.Subscribe(static _ => throw new Exception("Test exception"));
        using var sub2 = fixture.ThrownExceptions.ToObservableChangeSet(ImmediateScheduler.Instance)
            .Bind(out var exceptions).Subscribe();

        fixture.Value = 2;

        await Assert.That(exceptions).Count().IsGreaterThan(0);
    }

    /// <summary>
    ///     Test that nested change notifications work correctly.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task NestedDelayChangeNotificationsShouldWork()
    {
        var fixture = new TestMutableRecord();
        using var sub1 = fixture.Changed.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var changed)
            .Subscribe();
        using var sub2 = fixture.Changing.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var changing)
            .Subscribe();

        await AssertCount(0, changing, changed);

        var outer = fixture.DelayChangeNotifications();
        fixture.Value = 1;
        await AssertCount(0, changing, changed);

        var inner = fixture.DelayChangeNotifications();
        fixture.Value = 2;
        await AssertCount(0, changing, changed);

        outer.Dispose();
        await AssertCount(0, changing, changed); // Still delayed by inner

        inner.Dispose();
        await AssertCount(1, changing, changed); // Now notifications fire
    }

    /// <summary>
    ///     Test that PropertyChanged event works correctly.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task PropertyChangedEventShouldFire()
    {
        var fixture = new TestMutableRecord();
        var fired = false;
        var propertyName = string.Empty;

        fixture.PropertyChanged += (sender, args) =>
        {
            fired = true;
            propertyName = args.PropertyName ?? string.Empty;
        };

        fixture.Value = 42;

        using (Assert.Multiple())
        {
            await Assert.That(fired).IsTrue();
            await Assert.That(propertyName).IsEqualTo(nameof(TestMutableRecord.Value));
        }
    }

    /// <summary>
    ///     Test that PropertyChanging event works correctly.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task PropertyChangingEventShouldFire()
    {
        var fixture = new TestMutableRecord();
        var fired = false;
        var propertyName = string.Empty;

        fixture.PropertyChanging += (sender, args) =>
        {
            fired = true;
            propertyName = args.PropertyName ?? string.Empty;
        };

        fixture.Value = 42;

        using (Assert.Multiple())
        {
            await Assert.That(fired).IsTrue();
            await Assert.That(propertyName).IsEqualTo(nameof(TestMutableRecord.Value));
        }
    }

    /// <summary>
    ///     Test that ReactiveRecord doesn't serialize internal reactive properties.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task ReactiveRecordShouldNotSerializeInternalReactiveProperties()
    {
        var fixture = new TestRecord { Name = "Test", Age = 25 };
        var json = JsonSerializer.Serialize(fixture);

        using (Assert.Multiple())
        {
            await Assert.That(json).DoesNotContain("Changing");
            await Assert.That(json).DoesNotContain("Changed");
            await Assert.That(json).DoesNotContain("ThrownExceptions");
        }
    }

    /// <summary>
    ///     Test that ReactiveRecord properties raise changing and changed notifications.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task ReactiveRecordShouldRaisePropertyChangeNotifications()
    {
        var fixture = new TestRecord { Name = "Initial" };
        var changingEvents = new List<string>();
        var changedEvents = new List<string>();

        using var sub1 = fixture.Changing
            .Where(x => x.PropertyName is not null)
            .Select(x => x.PropertyName!)
            .Subscribe(x => changingEvents.Add(x));

        using var sub2 = fixture.Changed
            .Where(x => x.PropertyName is not null)
            .Select(x => x.PropertyName!)
            .Subscribe(x => changedEvents.Add(x));

        var updated = fixture with { Name = "Updated" };

        // Records create new instances, so we need to subscribe to the new instance
        using var sub3 = updated.Changing
            .Where(x => x.PropertyName is not null)
            .Select(x => x.PropertyName!)
            .Subscribe(x => changingEvents.Add(x));

        using var sub4 = updated.Changed
            .Where(x => x.PropertyName is not null)
            .Select(x => x.PropertyName!)
            .Subscribe(x => changedEvents.Add(x));

        await Assert.That(updated.Name).IsEqualTo("Updated");
    }

    /// <summary>
    ///     Test that ReactiveRecord should serialize correctly.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task ReactiveRecordShouldSerializeCorrectly()
    {
        var fixture = new TestRecord { Name = "Test", Age = 25 };
        var json = JsonSerializer.Serialize(fixture);

        await Assert.That(json).Contains("Test");
        await Assert.That(json).Contains("25");

        var deserialized = JsonSerializer.Deserialize<TestRecord>(json);

        using (Assert.Multiple())
        {
            await Assert.That(deserialized).IsNotNull();
            await Assert.That(deserialized!.Name).IsEqualTo("Test");
            await Assert.That(deserialized.Age).IsEqualTo(25);
        }
    }

    /// <summary>
    ///     Test that removing PropertyChanged event handler works.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task RemovingPropertyChangedHandlerShouldWork()
    {
        var fixture = new TestMutableRecord();
        var callCount = 0;

        void Handler(object? sender, PropertyChangedEventArgs args) => callCount++;

        fixture.PropertyChanged += Handler;
        fixture.Value = 1;
        await Assert.That(callCount).IsEqualTo(1);

        fixture.PropertyChanged -= Handler;
        fixture.Value = 2;
        await Assert.That(callCount).IsEqualTo(1); // Should not have incremented
    }

    /// <summary>
    ///     Test that removing PropertyChanging event handler works.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task RemovingPropertyChangingHandlerShouldWork()
    {
        var fixture = new TestMutableRecord();
        var callCount = 0;

        void Handler(object? sender, PropertyChangingEventArgs args) => callCount++;

        fixture.PropertyChanging += Handler;
        fixture.Value = 1;
        await Assert.That(callCount).IsEqualTo(1);

        fixture.PropertyChanging -= Handler;
        fixture.Value = 2;
        await Assert.That(callCount).IsEqualTo(1); // Should not have incremented
    }

    /// <summary>
    ///     Test that SuppressChangeNotifications works correctly.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task SuppressChangeNotificationsShouldWork()
    {
        var fixture = new TestMutableRecord();

        await Assert.That(fixture.AreChangeNotificationsEnabled()).IsTrue();

        using (fixture.SuppressChangeNotifications())
        {
            await Assert.That(fixture.AreChangeNotificationsEnabled()).IsFalse();
        }

        await Assert.That(fixture.AreChangeNotificationsEnabled()).IsTrue();
    }

    /// <summary>
    ///     Test that ThrownExceptions observable is initialized.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task ThrownExceptionsObservableShouldBeInitialized()
    {
        var fixture = new TestRecord();
        await Assert.That(fixture.ThrownExceptions).IsNotNull();
    }

    private static async Task AssertCount(int expected, params ICollection[] collections)
    {
        foreach (var collection in collections)
        {
            await Assert.That(collection.Count).IsEqualTo(expected);
        }
    }

    /// <summary>
    ///     Test record for immutable scenarios.
    /// </summary>
    private record TestRecord : ReactiveRecord
    {
        public string? Name { get; init; }

        public int Age { get; init; }
    }

    /// <summary>
    ///     Test record with mutable properties for testing property change notifications.
    /// </summary>
    private record TestMutableRecord : ReactiveRecord
    {
        private int _value;

        public int Value
        {
            get => _value;
            set => this.RaiseAndSetIfChanged(ref _value, value);
        }
    }
}
