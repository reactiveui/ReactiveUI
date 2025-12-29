// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Linq.Expressions;
using System.Reactive.Concurrency;

namespace ReactiveUI.Tests;

/// <summary>
/// Tests for <see cref="OAPHCreationHelperMixin"/>.
/// </summary>
public class OAPHCreationHelperMixinTest
{
    /// <summary>
    /// Tests that ToProperty with Expression throws ArgumentNullException when property is null.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ToProperty_WithExpression_ThrowsOnNullProperty()
    {
        var source = new TestReactiveObject();
        var observable = Observable.Return("test");

        await Assert.That(() => observable.ToProperty(source, (Expression<Func<TestReactiveObject, string>>)null!))
            .Throws<ArgumentNullException>();
    }

    /// <summary>
    /// Tests that ToProperty with Expression and initial value throws ArgumentNullException when property is null.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ToProperty_WithExpressionAndInitialValue_ThrowsOnNullProperty()
    {
        var source = new TestReactiveObject();
        var observable = Observable.Return("test");

        await Assert.That(() => observable.ToProperty(source, (Expression<Func<TestReactiveObject, string>>)null!, "initial"))
            .Throws<ArgumentNullException>();
    }

    /// <summary>
    /// Tests that ToProperty with string name throws ArgumentNullException when target is null.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ToProperty_WithStringName_ThrowsOnNullTarget()
    {
        var observable = Observable.Return("test");

        await Assert.That(() => observable.ToProperty<TestReactiveObject, string>(null!, "TestProperty"))
            .Throws<ArgumentNullException>();
    }

    /// <summary>
    /// Tests that ToProperty with string name throws ArgumentNullException when observable is null.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ToProperty_WithStringName_ThrowsOnNullObservable()
    {
        var source = new TestReactiveObject();

        await Assert.That(() => ((IObservable<string>)null!).ToProperty(source, "TestProperty"))
            .Throws<ArgumentNullException>();
    }

    /// <summary>
    /// Tests that ToProperty with string name throws ArgumentException when property name is null.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ToProperty_WithStringName_ThrowsOnNullPropertyName()
    {
        var source = new TestReactiveObject();
        var observable = Observable.Return("test");

        await Assert.That(() => observable.ToProperty(source, (string)null!))
            .Throws<ArgumentException>();
    }

    /// <summary>
    /// Tests that ToProperty with string name throws ArgumentException when property name is empty.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ToProperty_WithStringName_ThrowsOnEmptyPropertyName()
    {
        var source = new TestReactiveObject();
        var observable = Observable.Return("test");

        await Assert.That(() => observable.ToProperty(source, string.Empty))
            .Throws<ArgumentException>();
    }

    /// <summary>
    /// Tests that ToProperty with string name throws ArgumentException when property name is whitespace.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ToProperty_WithStringName_ThrowsOnWhitespacePropertyName()
    {
        var source = new TestReactiveObject();
        var observable = Observable.Return("test");

        await Assert.That(() => observable.ToProperty(source, "   "))
            .Throws<ArgumentException>();
    }

    /// <summary>
    /// Tests that ToProperty with string name and out parameter returns the helper through out parameter.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ToProperty_WithStringNameAndOut_ReturnsHelperThroughOutParameter()
    {
        var source = new TestReactiveObject();
        var observable = Observable.Return("test");

        var result = observable.ToProperty(source, nameof(source.TestProperty), out var outResult);

        await Assert.That(result).IsNotNull();
        await Assert.That(outResult).IsEqualTo(result);

        result.Dispose();
    }

    /// <summary>
    /// Tests that ToProperty with Expression and out parameter returns the helper through out parameter.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ToProperty_WithExpressionAndOut_ReturnsHelperThroughOutParameter()
    {
        var source = new TestReactiveObject();
        var observable = Observable.Return("test");

        var result = observable.ToProperty(source, x => x.TestProperty, out var outResult);

        await Assert.That(result).IsNotNull();
        await Assert.That(outResult).IsEqualTo(result);

        result.Dispose();
    }

    /// <summary>
    /// Tests that ToProperty with Expression creates a valid helper.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ToProperty_WithExpression_CreatesValidHelper()
    {
        var source = new TestReactiveObject();
        var observable = Observable.Return("test").ObserveOn(ImmediateScheduler.Instance);

        var result = observable.ToProperty(source, x => x.TestProperty, scheduler: ImmediateScheduler.Instance);

        await Assert.That(result).IsNotNull();
        await Assert.That(result.Value).IsEqualTo("test");

        result.Dispose();
    }

    /// <summary>
    /// Tests that ToProperty with Expression and initial value creates a helper with initial value.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ToProperty_WithExpressionAndInitialValue_CreatesHelperWithInitialValue()
    {
        var source = new TestReactiveObject();
        var observable = Observable.Never<string>();

        var result = observable.ToProperty(source, x => x.TestProperty, "initial", scheduler: ImmediateScheduler.Instance);

        await Assert.That(result).IsNotNull();
        await Assert.That(result.Value).IsEqualTo("initial");

        result.Dispose();
    }

    /// <summary>
    /// Tests that ToProperty with string name and initial value creates a helper with initial value.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ToProperty_WithStringNameAndInitialValue_CreatesHelperWithInitialValue()
    {
        var source = new TestReactiveObject();
        var observable = Observable.Never<string>();

        var result = observable.ToProperty(source, nameof(source.TestProperty), "initial", scheduler: ImmediateScheduler.Instance);

        await Assert.That(result).IsNotNull();
        await Assert.That(result.Value).IsEqualTo("initial");

        result.Dispose();
    }

    /// <summary>
    /// Tests that ToProperty with deferSubscription defers subscription until Value is accessed.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ToProperty_WithDeferSubscription_DefersSubscriptionUntilValueAccessed()
    {
        var source = new TestReactiveObject();
        var subscribed = false;
        var observable = Observable.Create<string>(observer =>
        {
            subscribed = true;
            observer.OnNext("test");
            observer.OnCompleted();
            return Disposable.Empty;
        });

        var result = observable.ToProperty(source, x => x.TestProperty, deferSubscription: true, scheduler: ImmediateScheduler.Instance);

        await Assert.That(subscribed).IsFalse();

        _ = result.Value;

        await Assert.That(subscribed).IsTrue();

        result.Dispose();
    }

    /// <summary>
    /// Tests that ToProperty without deferSubscription subscribes immediately.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ToProperty_WithoutDeferSubscription_SubscribesImmediately()
    {
        var source = new TestReactiveObject();
        var subscribed = false;
        var observable = Observable.Create<string>(observer =>
        {
            subscribed = true;
            observer.OnNext("test");
            observer.OnCompleted();
            return Disposable.Empty;
        }).ObserveOn(ImmediateScheduler.Instance);

        var result = observable.ToProperty(source, x => x.TestProperty, deferSubscription: false, scheduler: ImmediateScheduler.Instance);

        await Assert.That(subscribed).IsTrue();

        result.Dispose();
    }

    /// <summary>
    /// Tests that ToProperty with getInitialValue function uses the function.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ToProperty_WithGetInitialValue_UsesFunction()
    {
        var source = new TestReactiveObject();
        var observable = Observable.Never<string>();
        var functionCalled = false;

        var result = observable.ToProperty(
            source,
            x => x.TestProperty,
            () =>
            {
                functionCalled = true;
                return "fromFunction";
            },
            scheduler: ImmediateScheduler.Instance);

        await Assert.That(functionCalled).IsTrue();
        await Assert.That(result.Value).IsEqualTo("fromFunction");

        result.Dispose();
    }

    /// <summary>
    /// Test reactive object for testing.
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
