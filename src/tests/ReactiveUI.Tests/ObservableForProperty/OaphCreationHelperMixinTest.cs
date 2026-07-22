// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Linq.Expressions;

namespace ReactiveUI.Tests.ObservableForProperty;

/// <summary>Tests for <see cref="OAPHCreationHelperMixins" />.</summary>
public class OaphCreationHelperMixinTest
{
    /// <summary>The text value used as the observable source in tests.</summary>
    private const string TestText = "test";

    /// <summary>The text value used for updated emissions in tests.</summary>
    private const string NewValueText = "newValue";

    /// <summary>The text value used for the initial emission in tests.</summary>
    private const string InitialText = "initial";

    /// <summary>Tests that internal ObservableToProperty with Expression extracts correct property name.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task ObservableToProperty_WithExpression_ExtractsCorrectPropertyName()
    {
        var source = new TestReactiveObject();
        var observable = Signal.Emit(TestText);
        string? capturedPropertyName = null;

        source.PropertyChanged += (_, e) => capturedPropertyName = e.PropertyName;

        var result = source.ObservableToProperty(
            observable,
            x => x.TestProperty,
            scheduler: Sequencer.Immediate);

        await Assert.That(capturedPropertyName).IsEqualTo(nameof(source.TestProperty));

        result.Dispose();
    }

    /// <summary>Tests that internal ObservableToProperty with Expression and getInitialValue creates helper and raises notifications.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task ObservableToProperty_WithExpressionAndGetInitialValue_CreatesHelperAndRaisesNotifications()
    {
        var source = new TestReactiveObject();
        var observable = Signal.Emit(NewValueText);
        var changingFired = false;
        var changedFired = false;

        source.PropertyChanging += (_, e) =>
        {
            if (e.PropertyName != nameof(source.TestProperty))
            {
                return;
            }

            changingFired = true;
        };

        source.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName != nameof(source.TestProperty))
            {
                return;
            }

            changedFired = true;
        };

        var result = source.ObservableToProperty(
            observable,
            x => x.TestProperty,
            static () => InitialText,
            scheduler: Sequencer.Immediate);

        await Assert.That(result).IsNotNull();
        await Assert.That(result.Value).IsEqualTo(NewValueText);
        await Assert.That(changingFired).IsTrue();
        await Assert.That(changedFired).IsTrue();

        result.Dispose();
    }

    /// <summary>Tests that internal ObservableToProperty with Expression without initialValue uses default.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task ObservableToProperty_WithExpressionNoInitialValue_UsesDefault()
    {
        var source = new TestReactiveObject();
        var observable = Signal.Silent<string>();

        var result = source.ObservableToProperty(
            observable,
            x => x.TestProperty,
            scheduler: Sequencer.Immediate);

        await Assert.That(result).IsNotNull();
        await Assert.That(result.Value).IsNull();

        result.Dispose();
    }

    /// <summary>Tests that internal ObservableToProperty with string name and getInitialValue creates helper.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task ObservableToProperty_WithStringNameAndGetInitialValue_CreatesHelper()
    {
        var source = new TestReactiveObject();
        var observable = Signal.Emit(NewValueText);
        var changingFired = false;
        var changedFired = false;

        source.PropertyChanging += (_, e) =>
        {
            if (e.PropertyName != nameof(source.TestProperty))
            {
                return;
            }

            changingFired = true;
        };

        source.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName != nameof(source.TestProperty))
            {
                return;
            }

            changedFired = true;
        };

        var result = source.ObservableToProperty(
            observable,
            nameof(source.TestProperty),
            static () => InitialText,
            scheduler: Sequencer.Immediate);

        await Assert.That(result).IsNotNull();
        await Assert.That(result.Value).IsEqualTo(NewValueText);
        await Assert.That(changingFired).IsTrue();
        await Assert.That(changedFired).IsTrue();

        result.Dispose();
    }

    /// <summary>Tests that internal ObservableToProperty with string name without initialValue uses default.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task ObservableToProperty_WithStringNameNoInitialValue_UsesDefault()
    {
        var source = new TestReactiveObject();
        var observable = Signal.Silent<string>();

        var result = source.ObservableToProperty(
            observable,
            nameof(source.TestProperty),
            scheduler: Sequencer.Immediate);

        await Assert.That(result).IsNotNull();
        await Assert.That(result.Value).IsNull();

        result.Dispose();
    }

    /// <summary>Tests that ToProperty with deferSubscription defers subscription until Value is accessed.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task ToProperty_WithDeferSubscription_DefersSubscriptionUntilValueAccessed()
    {
        var source = new TestReactiveObject();
        var subscribed = false;
        var observable = Signal.Create<string>(observer =>
        {
            subscribed = true;
            observer.OnNext(TestText);
            observer.OnCompleted();
            return Scope.Empty;
        });

        var result = observable.ToProperty(source, x => x.TestProperty, deferSubscription: true, scheduler: Sequencer.Immediate);

        await Assert.That(subscribed).IsFalse();

        _ = result.Value;

        await Assert.That(subscribed).IsTrue();

        result.Dispose();
    }

    /// <summary>Tests that ToProperty with Expression creates a valid helper.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task ToProperty_WithExpression_CreatesValidHelper()
    {
        var source = new TestReactiveObject();
        var observable = Signal.Emit(TestText);

        var result = observable.ToProperty(source, x => x.TestProperty, scheduler: Sequencer.Immediate);

        await Assert.That(result).IsNotNull();
        await Assert.That(result.Value).IsEqualTo(TestText);

        result.Dispose();
    }

    /// <summary>Tests that ToProperty with Expression throws ArgumentNullException when property is null.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task ToProperty_WithExpression_ThrowsOnNullProperty()
    {
        var source = new TestReactiveObject();
        var observable = Signal.Emit(TestText);

        await Assert.That(() => observable.ToProperty(source, (Expression<Func<TestReactiveObject, string>>)null!))
            .Throws<ArgumentNullException>();
    }

    /// <summary>Tests that ToProperty with Expression and initial value creates a helper with initial value.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task ToProperty_WithExpressionAndInitialValue_CreatesHelperWithInitialValue()
    {
        var source = new TestReactiveObject();
        var observable = Signal.Silent<string>();

        var result = observable.ToProperty(
            source,
            x => x.TestProperty,
            InitialText,
            scheduler: Sequencer.Immediate);

        await Assert.That(result).IsNotNull();
        await Assert.That(result.Value).IsEqualTo(InitialText);

        result.Dispose();
    }

    /// <summary>Tests that ToProperty with Expression and initial value throws ArgumentNullException when property is null.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task ToProperty_WithExpressionAndInitialValue_ThrowsOnNullProperty()
    {
        var source = new TestReactiveObject();
        var observable = Signal.Emit(TestText);

        await Assert.That(() => observable.ToProperty(
            source,
            (Expression<Func<TestReactiveObject, string>>)null!,
            InitialText))
            .Throws<ArgumentNullException>();
    }

    /// <summary>Tests that ToProperty with Expression and out parameter returns the helper through out parameter.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task ToProperty_WithExpressionAndOut_ReturnsHelperThroughOutParameter()
    {
        var source = new TestReactiveObject();
        var observable = Signal.Emit(TestText);

        var result = observable.ToProperty(source, x => x.TestProperty, out var outResult);

        await Assert.That(result).IsNotNull();
        await Assert.That(outResult).IsEqualTo(result);

        result.Dispose();
    }

    /// <summary>Tests that ToProperty with getInitialValue function uses the function.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task ToProperty_WithGetInitialValue_UsesFunction()
    {
        var source = new TestReactiveObject();
        var observable = Signal.Silent<string>();
        var functionCalled = false;

        var result = observable.ToProperty(
            source,
            x => x.TestProperty,
            () =>
            {
                functionCalled = true;
                return "fromFunction";
            },
            scheduler: Sequencer.Immediate);

        await Assert.That(functionCalled).IsTrue();
        await Assert.That(result.Value).IsEqualTo("fromFunction");

        result.Dispose();
    }

    /// <summary>Tests that ToProperty without deferSubscription subscribes immediately.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task ToProperty_WithoutDeferSubscription_SubscribesImmediately()
    {
        var source = new TestReactiveObject();
        var subscribed = false;
        var observable = Signal.Create<string>(observer =>
        {
            subscribed = true;
            observer.OnNext(TestText);
            observer.OnCompleted();
            return Scope.Empty;
        });

        var result = observable.ToProperty(source, x => x.TestProperty, deferSubscription: false, scheduler: Sequencer.Immediate);

        await Assert.That(subscribed).IsTrue();

        result.Dispose();
    }

    /// <summary>Tests that ToProperty with string name throws ArgumentException when property name is empty.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task ToProperty_WithStringName_ThrowsOnEmptyPropertyName()
    {
        var source = new TestReactiveObject();
        var observable = Signal.Emit(TestText);

        await Assert.That(() => observable.ToProperty(source, string.Empty))
            .Throws<ArgumentException>();
    }

    /// <summary>Tests that ToProperty with string name throws ArgumentNullException when observable is null.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task ToProperty_WithStringName_ThrowsOnNullObservable()
    {
        var source = new TestReactiveObject();

        await Assert.That(() => ((IObservable<string>)null!).ToProperty(source, "TestProperty"))
            .Throws<ArgumentNullException>();
    }

    /// <summary>Tests that ToProperty with string name throws ArgumentException when property name is null.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task ToProperty_WithStringName_ThrowsOnNullPropertyName()
    {
        var source = new TestReactiveObject();
        var observable = Signal.Emit(TestText);

        await Assert.That(() => observable.ToProperty(source, (string)null!))
            .Throws<ArgumentException>();
    }

    /// <summary>Tests that ToProperty with string name throws ArgumentNullException when target is null.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task ToProperty_WithStringName_ThrowsOnNullTarget()
    {
        var observable = Signal.Emit(TestText);

        await Assert.That(() => observable.ToProperty((TestReactiveObject)null!, "TestProperty"))
            .Throws<ArgumentNullException>();
    }

    /// <summary>Tests that ToProperty with string name throws ArgumentException when property name is whitespace.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task ToProperty_WithStringName_ThrowsOnWhitespacePropertyName()
    {
        var source = new TestReactiveObject();
        var observable = Signal.Emit(TestText);

        await Assert.That(() => observable.ToProperty(source, "   "))
            .Throws<ArgumentException>();
    }

    /// <summary>Tests that ToProperty with string name and initial value creates a helper with initial value.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task ToProperty_WithStringNameAndInitialValue_CreatesHelperWithInitialValue()
    {
        var source = new TestReactiveObject();
        var observable = Signal.Silent<string>();

        var result = observable.ToProperty(
            source,
            nameof(source.TestProperty),
            InitialText,
            scheduler: Sequencer.Immediate);

        await Assert.That(result).IsNotNull();
        await Assert.That(result.Value).IsEqualTo(InitialText);

        result.Dispose();
    }

    /// <summary>Tests that ToProperty with string name and out parameter returns the helper through out parameter.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task ToProperty_WithStringNameAndOut_ReturnsHelperThroughOutParameter()
    {
        var source = new TestReactiveObject();
        var observable = Signal.Emit(TestText);

        var result = observable.ToProperty(source, nameof(source.TestProperty), out var outResult);

        await Assert.That(result).IsNotNull();
        await Assert.That(outResult).IsEqualTo(result);

        result.Dispose();
    }

    /// <summary>Test reactive object for testing.</summary>
    private sealed class TestReactiveObject : ReactiveObject
    {
        /// <summary>Gets or sets the test property.</summary>
        public string? TestProperty
        {
            get;
            set => this.RaiseAndSetIfChanged(ref field, value);
        }
    }
}
