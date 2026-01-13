// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests;

/// <summary>
///     Tests for <see cref="IROObservableForProperty" />.
/// </summary>
public class IROObservableForPropertyTest
{
    /// <summary>
    ///     Tests that GetAffinityForObject returns 10 for IReactiveObject types.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task GetAffinityForObject_IReactiveObjectType_Returns10()
    {
        var oaph = new IROObservableForProperty();

        var affinity = oaph.GetAffinityForObject(typeof(TestReactiveObject), "TestProperty");

        await Assert.That(affinity).IsEqualTo(10);
    }

    /// <summary>
    ///     Tests that GetAffinityForObject returns 0 for non-IReactiveObject types.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task GetAffinityForObject_NonReactiveObjectType_Returns0()
    {
        var oaph = new IROObservableForProperty();

        var affinity = oaph.GetAffinityForObject(typeof(string), "TestProperty");

        await Assert.That(affinity).IsEqualTo(0);
    }

    /// <summary>
    ///     Tests that GetAffinityForObject returns 10 regardless of beforeChanged parameter.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task GetAffinityForObject_WithBeforeChanged_Returns10()
    {
        var oaph = new IROObservableForProperty();

        var affinity = oaph.GetAffinityForObject(typeof(TestReactiveObject), "TestProperty", true);

        await Assert.That(affinity).IsEqualTo(10);
    }

    /// <summary>
    ///     Tests that GetNotificationForProperty throws for non-IReactiveObject sender.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task GetNotificationForProperty_NonReactiveObjectSender_Throws()
    {
        var oaph = new IROObservableForProperty();
        var sender = new object();
        Expression<Func<object, string?>> expression = x => x.ToString();

        await Assert.That(() => oaph.GetNotificationForProperty(sender, expression.Body, "ToString"))
            .Throws<ArgumentException>();
    }

    /// <summary>
    ///     Tests that GetNotificationForProperty throws for null expression.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task GetNotificationForProperty_NullExpression_Throws()
    {
        var oaph = new IROObservableForProperty();
        var sender = new TestReactiveObject();

        await Assert.That(() => oaph.GetNotificationForProperty(sender, null!, "TestProperty"))
            .Throws<ArgumentNullException>();
    }

    /// <summary>
    ///     Tests that GetNotificationForProperty emits when property changes.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task GetNotificationForProperty_PropertyChanges_EmitsNotification()
    {
        var oaph = new IROObservableForProperty();
        var sender = new TestReactiveObject();
        var param = System.Linq.Expressions.Expression.Parameter(typeof(TestReactiveObject), "x");
        var expression = System.Linq.Expressions.Expression.Property(param, nameof(TestReactiveObject.TestProperty));

        var changes = new List<IObservedChange<object, object?>>();
        oaph.GetNotificationForProperty(sender, expression, nameof(TestReactiveObject.TestProperty))
            .ObserveOn(ImmediateScheduler.Instance)
            .Subscribe(changes.Add);

        sender.TestProperty = "value1";
        sender.TestProperty = "value2";

        await Assert.That(changes).Count().IsEqualTo(2);
    }

    /// <summary>
    ///     Tests that GetNotificationForProperty returns observable for property changes.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task GetNotificationForProperty_ValidSender_ReturnsObservable()
    {
        var oaph = new IROObservableForProperty();
        var sender = new TestReactiveObject();
        var param = System.Linq.Expressions.Expression.Parameter(typeof(TestReactiveObject), "x");
        var expression = System.Linq.Expressions.Expression.Property(param, nameof(TestReactiveObject.TestProperty));

        var observable = oaph.GetNotificationForProperty(sender, expression, nameof(TestReactiveObject.TestProperty));

        await Assert.That(observable).IsNotNull();
    }

    /// <summary>
    ///     Test reactive object for testing.
    /// </summary>
    private class TestReactiveObject : ReactiveObject
    {
        private string? _testProperty;

        public string? TestProperty
        {
            get => _testProperty;
            set => this.RaiseAndSetIfChanged(ref _testProperty, value);
        }
    }
}
