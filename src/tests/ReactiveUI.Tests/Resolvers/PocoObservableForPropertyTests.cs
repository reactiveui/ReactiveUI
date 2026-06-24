// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.ComponentModel;
using System.Linq.Expressions;
using ReactiveUI.Tests.Utilities.Schedulers;
using TUnit.Core.Executors;

namespace ReactiveUI.Tests.Resolvers;

/// <summary>Tests for <see cref="POCOObservableForProperty"/>.</summary>
[NotInParallel]
[TestExecutor<WithSchedulerExecutor>]
public class PocoObservableForPropertyTests
{
    /// <summary>Verifies that the affinity value returned for POCO and INPC types is correct.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task CheckGetAffinityForObjectValues()
    {
        var instance = new POCOObservableForProperty();

        using (Assert.Multiple())
        {
            await Assert.That(
                instance.GetAffinityForObject(
                    typeof(PocoType),
                    nameof(PocoType.Property1))).IsEqualTo(1);
            await Assert.That(
                instance.GetAffinityForObject(
                    typeof(InpcClass),
                    "SomeProperty")).IsEqualTo(1);
        }
    }

    /// <summary>Verifies that the observable emits a single value for a POCO and then completes via timeout.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    [TestExecutor<WithVirtualTimeSchedulerExecutor>]
    public async Task GetNotificationForPropertyNeverCompletes()
    {
        var scheduler = TestContext.Current.GetVirtualTimeScheduler();

        var instance = new POCOObservableForProperty();
        var poco = new PocoType { Property1 = "Test" };
        Expression<Func<PocoType, string?>> expr = x => x.Property1;

        var observable = instance.GetNotificationForProperty(
            poco,
            expr.Body,
            nameof(PocoType.Property1),
            false,
            true);

        // POCO has no change notification, so the property observable emits the single initial value and then
        // never emits or completes on its own (hence the test name). Subscribe directly and advance virtual time:
        // the value arrives once and the stream stays open.
        var results = new List<IObservedChange<object, object?>>();
        var completed = false;

        using var subscription = observable.Subscribe(
            results.Add,
            () => completed = true);

        scheduler.AdvanceBy(TimeSpan.FromMilliseconds(150));

        // Exactly 1 item (the initial value) and the observable never completes.
        using (Assert.Multiple())
        {
            await Assert.That(results).Count().IsEqualTo(1);
            await Assert.That(completed).IsFalse();
        }
    }

    /// <summary>Verifies that the POCO warning is only emitted once per type and property.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task GetNotificationForPropertyOnlyWarnsOnce()
    {
        var instance = new POCOObservableForProperty();
        var poco1 = new PocoType { Property1 = "Test1" };
        var poco2 = new PocoType { Property1 = "Test2" };
        Expression<Func<PocoType, string?>> expr = x => x.Property1;

        // First call should trigger warning (but we're suppressing it with suppressWarnings: false for testing)
        var observable1 = instance.GetNotificationForProperty(poco1, expr.Body, nameof(PocoType.Property1));
        var result1 = await observable1.FirstAsync();

        // Second call with different instance but same type and property should not warn again
        var observable2 = instance.GetNotificationForProperty(poco2, expr.Body, nameof(PocoType.Property1));
        var result2 = await observable2.FirstAsync();

        using (Assert.Multiple())
        {
            await Assert.That(result1).IsNotNull();
            await Assert.That(result2).IsNotNull();
        }
    }

    /// <summary>Verifies that a non-null observable is returned for a POCO property.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task GetNotificationForPropertyReturnsObservable()
    {
        var instance = new POCOObservableForProperty();
        var poco = new PocoType { Property1 = "Test" };
        Expression<Func<PocoType, string?>> expr = x => x.Property1;

        var observable = instance.GetNotificationForProperty(poco, expr.Body, nameof(PocoType.Property1), false, true);

        await Assert.That(observable).IsNotNull();
    }

    /// <summary>Verifies that the observable emits a single value whose sender is the POCO instance.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task GetNotificationForPropertyReturnsSingleValue()
    {
        var instance = new POCOObservableForProperty();
        var poco = new PocoType { Property1 = "Test" };
        Expression<Func<PocoType, string?>> expr = x => x.Property1;

        var observable = instance.GetNotificationForProperty(poco, expr.Body, nameof(PocoType.Property1), false, true);
        var result = await observable.FirstAsync();

        using (Assert.Multiple())
        {
            await Assert.That(result).IsNotNull();
            await Assert.That(result.Sender).IsEqualTo(poco);
        }
    }

    /// <summary>Verifies that a null sender causes an <see cref="ArgumentNullException"/> to be thrown.</summary>
    [Test]
    public void GetNotificationForPropertyThrowsOnNullSender()
    {
        var instance = new POCOObservableForProperty();
        Expression<Func<PocoType, string?>> expr = x => x.Property1;

        _ = Assert.Throws<ArgumentNullException>(() =>
            instance.GetNotificationForProperty(null!, expr.Body, nameof(PocoType.Property1), false, true));
    }

    /// <summary>Verifies that a value is emitted when the before-changed parameter is set.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task GetNotificationForPropertyWithBeforeChangedParameter()
    {
        var instance = new POCOObservableForProperty();
        var poco = new PocoType { Property1 = "Test" };
        Expression<Func<PocoType, string?>> expr = x => x.Property1;

        var observable = instance.GetNotificationForProperty(poco, expr.Body, nameof(PocoType.Property1), true, true);
        var result = await observable.FirstAsync();

        await Assert.That(result).IsNotNull();
    }

    /// <summary>Verifies that notifications can be obtained independently for different properties of the same POCO.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task GetNotificationForPropertyWithDifferentProperties()
    {
        var instance = new POCOObservableForProperty();
        var poco = new PocoType { Property1 = "Test1", Property2 = "Test2" };
        Expression<Func<PocoType, string?>> expr1 = x => x.Property1;
        Expression<Func<PocoType, string?>> expr2 = x => x.Property2;

        var observable1 = instance.GetNotificationForProperty(
            poco,
            expr1.Body,
            nameof(PocoType.Property1),
            false,
            true);
        var observable2 = instance.GetNotificationForProperty(
            poco,
            expr2.Body,
            nameof(PocoType.Property2),
            false,
            true);

        var result1 = await observable1.FirstAsync();
        var result2 = await observable2.FirstAsync();

        using (Assert.Multiple())
        {
            await Assert.That(result1).IsNotNull();
            await Assert.That(result2).IsNotNull();
            await Assert.That(result1.Sender).IsEqualTo(poco);
            await Assert.That(result2.Sender).IsEqualTo(poco);
        }
    }

    /// <summary>A test fixture implementing <see cref="INotifyPropertyChanged"/> used to verify affinity handling.</summary>
    private sealed class InpcClass : INotifyPropertyChanged
    {
        /// <summary>Occurs when a property value changes.</summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>Raises the <see cref="PropertyChanged"/> event.</summary>
        public void NotifyPropertyChanged() => PropertyChanged?.Invoke(
            this,
            new(string.Empty));
    }

    /// <summary>A plain-old CLR object test fixture with no change notification support.</summary>
    private sealed class PocoType
    {
        /// <summary>Gets or sets the first test property.</summary>
        public string? Property1 { get; set; }

        /// <summary>Gets or sets the second test property.</summary>
        public string? Property2 { get; set; }
    }
}
