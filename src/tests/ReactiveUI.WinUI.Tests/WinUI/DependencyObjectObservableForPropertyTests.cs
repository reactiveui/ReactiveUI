// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Linq.Expressions;
using ReactiveUI.Tests.WinUI.Mocks;
using TUnit.Core.Executors;

namespace ReactiveUI.Tests.WinUI;

/// <summary>Tests for <see cref="DependencyObjectObservableForProperty"/>.</summary>
[NotInParallel]
[TestExecutor<WinUITestExecutor>]
public class DependencyObjectObservableForPropertyTests
{
    /// <summary>The affinity a dependency property observation claims.</summary>
    private const int DependencyPropertyAffinity = 6;

    /// <summary>How long a scheduled fallback notification is given to arrive before the test fails.</summary>
    private static readonly TimeSpan NotificationTimeout = TimeSpan.FromSeconds(10);

    /// <summary>Supplies the type and property pairs a dependency property observation does not claim.</summary>
    /// <returns>The unclaimed candidates.</returns>
    public static IEnumerable<(Type? Type, string PropertyName)> UnclaimedProperties() =>
    [
        (null, nameof(string.Length)),
        (typeof(string), nameof(string.Length)),
        (typeof(DependencyObjectFixture), "NotADependencyProperty"),
        (typeof(DependencyObjectFixture), "UnsetField"),
        (typeof(DependencyPropertyAccessorFixture), "UnsetAccessor"),
    ];

    /// <summary>Verifies a backing dependency property is claimed ahead of the reflection-based observers.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task GetAffinityForObject_ForADependencyProperty_ClaimsTheProperty()
    {
        var observer = new DependencyObjectObservableForProperty();

        await Assert.That(observer.GetAffinityForObject(typeof(DependencyObjectFixture), nameof(DependencyObjectFixture.TestString)))
            .IsEqualTo(DependencyPropertyAffinity);
    }

    /// <summary>Verifies a dependency property inherited from a base type is still claimed.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task GetAffinityForObject_ForAnInheritedDependencyProperty_ClaimsTheProperty()
    {
        var observer = new DependencyObjectObservableForProperty();

        await Assert.That(observer.GetAffinityForObject(typeof(DerivedDependencyObjectFixture), nameof(DependencyObjectFixture.TestString)))
            .IsEqualTo(DependencyPropertyAffinity);
    }

    /// <summary>Verifies a dependency property exposed as a static property, not a field, is still claimed.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task GetAffinityForObject_ForADependencyPropertyExposedAsAnAccessor_ClaimsTheProperty()
    {
        var observer = new DependencyObjectObservableForProperty();

        await Assert.That(observer.GetAffinityForObject(typeof(DependencyPropertyAccessorFixture), nameof(DependencyPropertyAccessorFixture.Caption)))
            .IsEqualTo(DependencyPropertyAffinity);
    }

    /// <summary>Verifies types outside the dependency system are left to another observer.</summary>
    /// <param name="type">The candidate type.</param>
    /// <param name="propertyName">The candidate property.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [MethodDataSource(nameof(UnclaimedProperties))]
    public async Task GetAffinityForObject_ForAnythingElse_ClaimsNothing(Type? type, string propertyName)
    {
        var observer = new DependencyObjectObservableForProperty();

        await Assert.That(observer.GetAffinityForObject(type, propertyName, false)).IsEqualTo(0);
    }

    /// <summary>Verifies a dependency property change reaches the observer.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task GetNotificationForProperty_OnChange_NotifiesWithTheSenderAndExpression()
    {
        var observer = new DependencyObjectObservableForProperty();
        var fixture = new DependencyObjectFixture();
        Expression<Func<DependencyObjectFixture, object?>> expression = static x => x.TestString;
        var changes = new List<IObservedChange<object, object?>>();

        using var subscription = observer
            .GetNotificationForProperty(fixture, expression.Body, nameof(DependencyObjectFixture.TestString))
            .Subscribe(changes.Add);
        fixture.TestString = "changed";

        using (Assert.Multiple())
        {
            _ = await Assert.That(changes).HasSingleItem();
            await Assert.That(changes[0].Sender).IsSameReferenceAs(fixture);
            await Assert.That(changes[0].Expression).IsSameReferenceAs(expression.Body);
        }
    }

    /// <summary>Verifies disposing the subscription unregisters the dependency property callback.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task GetNotificationForProperty_AfterDisposal_StopsNotifying()
    {
        var observer = new DependencyObjectObservableForProperty();
        var fixture = new DependencyObjectFixture();
        Expression<Func<DependencyObjectFixture, object?>> expression = static x => x.TestString;
        var changes = new List<IObservedChange<object, object?>>();

        var subscription = observer
            .GetNotificationForProperty(fixture, expression.Body, nameof(DependencyObjectFixture.TestString), false)
            .Subscribe(changes.Add);
        fixture.TestString = "first";
        subscription.Dispose();
        fixture.TestString = "second";

        _ = await Assert.That(changes).HasSingleItem();
    }

    /// <summary>Verifies each subscriber gets its own dependency property callback.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task GetNotificationForProperty_WithSeveralSubscribers_NotifiesEachOfThem()
    {
        var observer = new DependencyObjectObservableForProperty();
        var fixture = new DependencyObjectFixture();
        Expression<Func<DependencyObjectFixture, object?>> expression = static x => x.TestString;
        var first = new List<IObservedChange<object, object?>>();
        var second = new List<IObservedChange<object, object?>>();
        var notifications = observer.GetNotificationForProperty(fixture, expression.Body, nameof(DependencyObjectFixture.TestString));

        using var firstSubscription = notifications.Subscribe(first.Add);
        using var secondSubscription = notifications.Subscribe(second.Add);
        fixture.TestString = "changed";

        using (Assert.Multiple())
        {
            _ = await Assert.That(first).HasSingleItem();
            _ = await Assert.That(second).HasSingleItem();
        }
    }

    /// <summary>Verifies a non-dependency sender is rejected, since there is no callback to register.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task GetNotificationForProperty_ForANonDependencyObject_Throws()
    {
        var observer = new DependencyObjectObservableForProperty();
        Expression<Func<DependencyObjectFixture, object?>> expression = static x => x.TestString;

        await Assert.That(() => observer.GetNotificationForProperty("not a dependency object", expression.Body, "Length"))
            .Throws<ArgumentException>();
    }

    /// <summary>Verifies a missing sender is rejected.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task GetNotificationForProperty_WithNoSender_Throws()
    {
        var observer = new DependencyObjectObservableForProperty();
        Expression<Func<DependencyObjectFixture, object?>> expression = static x => x.TestString;

        await Assert.That(() => observer.GetNotificationForProperty(null!, expression.Body, nameof(DependencyObjectFixture.TestString)))
            .Throws<ArgumentNullException>();
    }

    /// <summary>
    /// Verifies a before-change observation degrades to the single-value fallback: dependency property callbacks
    /// only ever run after the value has changed.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task GetNotificationForProperty_BeforeChanged_FallsBackToASingleValue()
    {
        var observer = new DependencyObjectObservableForProperty();
        var fixture = new DependencyObjectFixture();
        Expression<Func<DependencyObjectFixture, object?>> expression = static x => x.TestString;

        var change = await FirstChangeAsync(observer.GetNotificationForProperty(
            fixture,
            expression.Body,
            nameof(DependencyObjectFixture.TestString),
            true));

        await Assert.That(change.Sender).IsSameReferenceAs(fixture);
    }

    /// <summary>Verifies observing a plain CLR property degrades to the single-value fallback.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task GetNotificationForProperty_ForANonDependencyProperty_FallsBackToASingleValue()
    {
        var observer = new DependencyObjectObservableForProperty();
        var fixture = new DependencyObjectFixture();
        Expression<Func<DependencyObjectFixture, object?>> expression = static x => x.TestString;

        var change = await FirstChangeAsync(observer.GetNotificationForProperty(
            fixture,
            expression.Body,
            "NotADependencyProperty",
            false,
            true));

        await Assert.That(change.Sender).IsSameReferenceAs(fixture);
    }

    /// <summary>Awaits the first change a notification stream produces, however it is scheduled.</summary>
    /// <param name="notifications">The stream to observe.</param>
    /// <returns>The first observed change.</returns>
    private static async Task<IObservedChange<object, object?>> FirstChangeAsync(
        IObservable<IObservedChange<object, object?>> notifications)
    {
        var received = new TaskCompletionSource<IObservedChange<object, object?>>(TaskCreationOptions.RunContinuationsAsynchronously);
        using var subscription = notifications.Subscribe(change => received.TrySetResult(change));

        return await received.Task.WaitAsync(NotificationTimeout);
    }
}
