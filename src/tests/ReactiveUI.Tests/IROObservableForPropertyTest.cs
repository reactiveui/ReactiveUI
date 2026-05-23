// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests;

/// <summary>
///     Tests for <see cref="IROObservableForProperty" />.
/// </summary>
public class IroObservableForPropertyTest
{
    private const string TestPropertyName = "TestProperty";

    /// <summary>
    ///     Tests that GetAffinityForObject returns 10 for IReactiveObject types.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task GetAffinityForObject_IReactiveObjectType_Returns10()
    {
        var oaph = new IROObservableForProperty();

        var affinity = oaph.GetAffinityForObject(typeof(TestReactiveObject), TestPropertyName);

        await Assert.That(affinity).IsEqualTo(BindingAffinity.ExactType);
    }

    /// <summary>
    ///     Tests that GetAffinityForObject returns 0 for non-IReactiveObject types.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task GetAffinityForObject_NonReactiveObjectType_Returns0()
    {
        var oaph = new IROObservableForProperty();

        var affinity = oaph.GetAffinityForObject(typeof(string), TestPropertyName);

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

        var affinity = oaph.GetAffinityForObject(typeof(TestReactiveObject), TestPropertyName, true);

        await Assert.That(affinity).IsEqualTo(BindingAffinity.ExactType);
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

        await Assert.That(() => oaph.GetNotificationForProperty(sender, null!, TestPropertyName))
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
        oaph.GetNotificationForProperty(sender, expression, nameof(TestReactiveObject.TestProperty)).ObserveOn(ImmediateScheduler.Instance).Subscribe(changes.Add);

        sender.TestProperty = "value1";
        sender.TestProperty = "value2";

        const int ExpectedCount = 2;
        await Assert.That(changes).Count().IsEqualTo(ExpectedCount);
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
    private sealed class TestReactiveObject : ReactiveObject
    {
        /// <summary>
        ///     The backing field for the <see cref="TestProperty" /> property.
        /// </summary>
        private string? _testProperty;

        /// <summary>
        ///     Gets or sets the test property.
        /// </summary>
        public string? TestProperty
        {
            get => _testProperty;
            set => this.RaiseAndSetIfChanged(ref _testProperty, value);
        }
    }
}
