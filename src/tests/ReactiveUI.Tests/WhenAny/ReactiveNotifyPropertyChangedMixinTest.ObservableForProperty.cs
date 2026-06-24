// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using ReactiveUI.Tests.ReactiveObjects.Mocks;
using ReactiveUI.Tests.Utilities.Schedulers;
using ReactiveUI.Tests.WhenAny.Mockups;
using TUnit.Core.Executors;

namespace ReactiveUI.Tests.WhenAny;

/// <summary>Tests for the reactive notify property changed mixin (WhenAny, WhenAnyValue, ObservableForProperty).</summary>
public partial class ReactiveNotifyPropertyChangedMixinTest
{
    /// <summary>Tests ObservableForProperty with selector throws for null selector.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task ObservableForProperty_NullSelector_Throws()
    {
        var fixture = new TestFixture();

        await Assert.That(() => fixture.ObservableForProperty(x => x.IsOnlyOneWord, (Func<string?, int>)null!))
            .Throws<ArgumentNullException>();
    }

    /// <summary>Tests ObservableForProperty string overload with property name.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<WithSchedulerExecutor>]
    public async Task ObservableForProperty_StringPropertyName_ObservesProperty()
    {
        var fixture = new TestFixture();

        var results = new List<string?>();

        _ = fixture.ObservableForProperty<TestFixture, string?>(nameof(TestFixture.IsOnlyOneWord)).ObserveOn(Sequencer.Immediate).Subscribe(x => results.Add(x.Value));

        fixture.IsOnlyOneWord = Value1Text;

        // ImmediateScheduler executes synchronously
        await Assert.That(results).Count().IsEqualTo(1);

        await Assert.That(results[0]).IsEqualTo(Value1Text);

        fixture.IsOnlyOneWord = Value2Text;

        // ImmediateScheduler executes synchronously
        await Assert.That(results).Count().IsEqualTo(ExpectedCountAfterSecondChange);

        await Assert.That(results[1]).IsEqualTo(Value2Text);
    }

    /// <summary>Tests ObservableForProperty string overload with beforeChange.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<WithSchedulerExecutor>]
    public async Task ObservableForProperty_StringPropertyNameBeforeChange_ObservesBeforeChange()
    {
        var fixture = new TestFixture { IsOnlyOneWord = InitialText };

        var results = new List<string?>();

        _ = fixture.ObservableForProperty<TestFixture, string?>(nameof(TestFixture.IsOnlyOneWord), true).ObserveOn(Sequencer.Immediate).Subscribe(x => results.Add(x.Value));

        fixture.IsOnlyOneWord = ChangedText;

        // ImmediateScheduler executes synchronously
        await Assert.That(results).Count().IsEqualTo(1);

        await Assert.That(results[0]).IsEqualTo(InitialText);
    }

    /// <summary>Tests ObservableForProperty string overload without skipInitial.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<WithSchedulerExecutor>]
    public async Task ObservableForProperty_StringPropertyNameNoSkipInitial_EmitsInitialValue()
    {
        var fixture = new TestFixture { IsOnlyOneWord = InitialText };

        var results = new List<string?>();

        _ = fixture.ObservableForProperty<TestFixture, string?>(
            nameof(TestFixture.IsOnlyOneWord),
            false,
            false).ObserveOn(Sequencer.Immediate).Subscribe(x => results.Add(x.Value));

        // ImmediateScheduler executes synchronously
        await Assert.That(results).Count().IsEqualTo(1);

        await Assert.That(results[0]).IsEqualTo(InitialText);

        fixture.IsOnlyOneWord = ChangedText;

        // ImmediateScheduler executes synchronously
        await Assert.That(results).Count().IsEqualTo(ExpectedCountAfterSecondChange);

        await Assert.That(results[1]).IsEqualTo(ChangedText);
    }

    /// <summary>Tests ObservableForProperty string overload throws for null property name.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task ObservableForProperty_StringPropertyNameNull_Throws()
    {
        var fixture = new TestFixture();

        await Assert.That(() => fixture.ObservableForProperty<TestFixture, string?>((string)null!))
            .Throws<ArgumentNullException>();
    }

    /// <summary>Tests ObservableForProperty string overload throws for null item.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task ObservableForProperty_StringPropertyNameNullItem_Throws()
    {
        const TestFixture? fixture = null;

        await Assert.That(() => fixture.ObservableForProperty<TestFixture, string?>(nameof(TestFixture.IsOnlyOneWord)))
            .Throws<ArgumentNullException>();
    }

    /// <summary>Tests ObservableForProperty string overload with isDistinct parameter.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<WithSchedulerExecutor>]
    public async Task ObservableForProperty_StringPropertyNameWithIsDistinct_Works()
    {
        var fixture = new TestFixture();

        var results = new List<string?>();

        _ = fixture.ObservableForProperty<TestFixture, string?>(
            nameof(TestFixture.IsOnlyOneWord),
            false,
            true,
            true).ObserveOn(Sequencer.Immediate).Subscribe(x => results.Add(x.Value));

        fixture.IsOnlyOneWord = Value1Text;

        // ImmediateScheduler executes synchronously
        await Assert.That(results).Count().IsEqualTo(1);

        fixture.IsOnlyOneWord = Value2Text;

        // ImmediateScheduler executes synchronously
        await Assert.That(results).Count().IsEqualTo(ExpectedCountAfterSecondChange);
    }

    /// <summary>Tests ObservableForProperty with selector.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<WithSchedulerExecutor>]
    public async Task ObservableForProperty_WithSelector_TransformsValues()
    {
        const int HelloLength = 5;
        const int HiLength = 2;

        var fixture = new TestFixture { IsOnlyOneWord = TestText };

        var results = new List<int>();

        _ = fixture.ObservableForProperty(x => x.IsOnlyOneWord, value => value?.Length ?? 0).ObserveOn(Sequencer.Immediate).Subscribe(results.Add);

        fixture.IsOnlyOneWord = "Hello";

        // ImmediateScheduler executes synchronously
        await Assert.That(results).Count().IsEqualTo(1);

        await Assert.That(results[0]).IsEqualTo(HelloLength);

        fixture.IsOnlyOneWord = "Hi";

        // ImmediateScheduler executes synchronously
        await Assert.That(results).Count().IsEqualTo(ExpectedCountAfterSecondChange);

        await Assert.That(results[1]).IsEqualTo(HiLength);
    }

    /// <summary>Tests ObservableForProperty with selector and beforeChange.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<WithSchedulerExecutor>]
    public async Task ObservableForProperty_WithSelectorAndBeforeChange_TransformsBeforeValues()
    {
        const int ExpectedLength = 7;

        var fixture = new TestFixture { IsOnlyOneWord = InitialText };

        var results = new List<int>();

        _ = fixture.ObservableForProperty(x => x.IsOnlyOneWord, value => value?.Length ?? 0, true).ObserveOn(Sequencer.Immediate).Subscribe(results.Add);

        fixture.IsOnlyOneWord = ChangedText;

        // ImmediateScheduler executes synchronously
        await Assert.That(results).Count().IsEqualTo(1);

        await Assert.That(results[0]).IsEqualTo(ExpectedLength); // Length of InitialText

        fixture.IsOnlyOneWord = "New";

        // ImmediateScheduler executes synchronously
        await Assert.That(results).Count().IsEqualTo(ExpectedCountAfterSecondChange);

        await Assert.That(results[1]).IsEqualTo(ExpectedLength); // Length of ChangedText
    }

    /// <summary>Verifies child change notification behavior when the host property changes.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<WithSchedulerExecutor>]
    public async Task OfpChangingTheHostPropertyShouldFireAChildChangeNotificationOnlyIfThePreviousChildIsDifferent()
    {
        var fixture = new HostTestFixture { Child = new() };

        var changes = fixture.ObservableForProperty(static x => x.Child!.IsOnlyOneWord).Collect();

        fixture.Child.IsOnlyOneWord = FooText;

        // ImmediateScheduler executes synchronously
        await Assert.That(changes).Count().IsEqualTo(1);

        fixture.Child.IsOnlyOneWord = BarText;

        // ImmediateScheduler executes synchronously
        await Assert.That(changes).Count().IsEqualTo(ExpectedCountAfterSecondChange);

        fixture.Child = new() { IsOnlyOneWord = BarText };

        // ImmediateScheduler executes synchronously
        await Assert.That(changes).Count().IsEqualTo(ExpectedCountAfterSecondChange);
    }

    /// <summary>Observes a named property and verifies notifications and values.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<WithSchedulerExecutor>]
    public async Task OfpNamedPropertyTest()
    {
        var fixture = new TestFixture();

        var changes = fixture.ObservableForProperty(x => x.IsOnlyOneWord).Collect();

        fixture.IsOnlyOneWord = FooText;

        await Assert.That(changes).Count().IsEqualTo(1);

        fixture.IsOnlyOneWord = BarText;

        await Assert.That(changes).Count().IsEqualTo(ExpectedCountAfterSecondChange);

        fixture.IsOnlyOneWord = BazText;

        await Assert.That(changes).Count().IsEqualTo(ExpectedCountAfterThirdChange);

        fixture.IsOnlyOneWord = BazText;

        await Assert.That(changes).Count().IsEqualTo(ExpectedCountAfterThirdChange);

        using (Assert.Multiple())
        {
            await Assert.That(changes.All(x => x.Sender == fixture)).IsTrue();

            await Assert.That(changes.All(x => x.GetPropertyName() == IsOnlyOneWordName)).IsTrue();

            await Assert.That(changes.Select(x => x.Value!)).IsEquivalentTo([FooText, BarText, BazText]);
        }
    }

    /// <summary>Observes a named property before change and verifies notifications.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<WithSchedulerExecutor>]
    public async Task OfpNamedPropertyTestBeforeChange()
    {
        var fixture = new TestFixture { IsOnlyOneWord = PreText };

        var changes = fixture.ObservableForProperty(
            x => x.IsOnlyOneWord,
            true).Collect();

        await Assert.That(changes).IsEmpty();

        fixture.IsOnlyOneWord = FooText;

        await Assert.That(changes).Count().IsEqualTo(1);

        fixture.IsOnlyOneWord = BarText;

        await Assert.That(changes).Count().IsEqualTo(ExpectedCountAfterSecondChange);

        using (Assert.Multiple())
        {
            await Assert.That(changes.All(x => x.Sender == fixture)).IsTrue();

            await Assert.That(changes.All(x => x.GetPropertyName() == IsOnlyOneWordName)).IsTrue();

            await Assert.That(changes.Select(x => x.Value!)).IsEquivalentTo([PreText, FooText]);
        }
    }

    /// <summary>Observes a named property with no initial-skip and verifies notifications.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<WithSchedulerExecutor>]
    public async Task OfpNamedPropertyTestNoSkipInitial()
    {
        var fixture = new TestFixture { IsOnlyOneWord = PreText };

        var changes = fixture.ObservableForProperty(
            x => x.IsOnlyOneWord,
            false,
            false).Collect();

        // ImmediateScheduler executes synchronously
        await Assert.That(changes).Count().IsEqualTo(1);

        fixture.IsOnlyOneWord = FooText;

        // ImmediateScheduler executes synchronously
        await Assert.That(changes).Count().IsEqualTo(ExpectedCountAfterSecondChange);

        using (Assert.Multiple())
        {
            await Assert.That(changes.All(x => x.Sender == fixture)).IsTrue();

            await Assert.That(changes.All(x => x.GetPropertyName() == IsOnlyOneWordName)).IsTrue();

            await Assert.That(changes.Select(x => x.Value!)).IsEquivalentTo([PreText, FooText]);
        }
    }

    /// <summary>Verifies that repeated values are de-duplicated.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<WithSchedulerExecutor>]
    public async Task OfpNamedPropertyTestRepeats()
    {
        var fixture = new TestFixture();

        var changes = fixture.ObservableForProperty(x => x.IsOnlyOneWord).Collect();

        fixture.IsOnlyOneWord = FooText;

        // ImmediateScheduler executes synchronously
        await Assert.That(changes).Count().IsEqualTo(1);

        fixture.IsOnlyOneWord = BarText;

        // ImmediateScheduler executes synchronously
        await Assert.That(changes).Count().IsEqualTo(ExpectedCountAfterSecondChange);

        fixture.IsOnlyOneWord = BarText;

        // ImmediateScheduler executes synchronously
        await Assert.That(changes).Count().IsEqualTo(ExpectedCountAfterSecondChange);

        fixture.IsOnlyOneWord = FooText;

        // ImmediateScheduler executes synchronously
        await Assert.That(changes).Count().IsEqualTo(ExpectedCountAfterThirdChange);

        using (Assert.Multiple())
        {
            await Assert.That(changes.All(x => x.Sender == fixture)).IsTrue();

            await Assert.That(changes.All(x => x.GetPropertyName() == IsOnlyOneWordName)).IsTrue();

            await Assert.That(changes.Select(x => x.Value!)).IsEquivalentTo([FooText, BarText, FooText]);
        }
    }

    /// <summary>Verifies re-subscription behavior when replacing the host.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<WithSchedulerExecutor>]
    public async Task OfpReplacingTheHostShouldResubscribeTheObservable()
    {
        var fixture = new HostTestFixture { Child = new() };

        var changes = fixture.ObservableForProperty(x => x.Child!.IsOnlyOneWord).Collect();

        fixture.Child.IsOnlyOneWord = FooText;

        // ImmediateScheduler executes synchronously
        await Assert.That(changes).Count().IsEqualTo(1);

        fixture.Child.IsOnlyOneWord = BarText;

        // ImmediateScheduler executes synchronously
        await Assert.That(changes).Count().IsEqualTo(ExpectedCountAfterSecondChange);

        // From BarText to null (new TestFixture with null IsOnlyOneWord)
        fixture.Child = new();

        // ImmediateScheduler executes synchronously
        await Assert.That(changes).Count().IsEqualTo(ExpectedCountAfterThirdChange);

        // Setting null again doesn't change
        fixture.Child.IsOnlyOneWord = null!;

        // ImmediateScheduler executes synchronously
        await Assert.That(changes).Count().IsEqualTo(ExpectedCountAfterThirdChange);

        fixture.Child.IsOnlyOneWord = BazText;

        // ImmediateScheduler executes synchronously
        await Assert.That(changes).Count().IsEqualTo(ExpectedCountAfterFourthChange);

        fixture.Child.IsOnlyOneWord = BazText;

        // ImmediateScheduler executes synchronously
        await Assert.That(changes).Count().IsEqualTo(ExpectedCountAfterFourthChange);

        using (Assert.Multiple())
        {
            await Assert.That(changes.All(x => x.Sender == fixture)).IsTrue();

            await Assert.That(changes.All(x => x.GetPropertyName() == ChildIsOnlyOneWordName)).IsTrue();

            await Assert.That(changes.Select(x => x.Value!)).IsEquivalentTo([FooText, BarText, null, BazText]);
        }
    }

    /// <summary>Verifies re-subscription behavior when host becomes null and then is restored.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<WithSchedulerExecutor>]
    public async Task OfpReplacingTheHostWithNullThenSettingItBackShouldResubscribeTheObservable()
    {
        var fixture = new HostTestFixture { Child = new() };

        var fixtureProp = fixture.ObservableForProperty(x => x.Child!.IsOnlyOneWord);

        var changes = fixtureProp.Collect();

        fixture.Child.IsOnlyOneWord = FooText;

        // ImmediateScheduler executes synchronously
        await Assert.That(changes).Count().IsEqualTo(1);

        fixture.Child.IsOnlyOneWord = BarText;

        // ImmediateScheduler executes synchronously
        await Assert.That(changes).Count().IsEqualTo(ExpectedCountAfterSecondChange);

        // Child becomes null
        fixture.Child = null!;

        // ImmediateScheduler executes synchronously
        await Assert.That(changes).Count().IsEqualTo(ExpectedCountAfterSecondChange);

        // From BarText to null (child restored but value is null)
        fixture.Child = new();

        // ImmediateScheduler executes synchronously
        await Assert.That(changes).Count().IsEqualTo(ExpectedCountAfterThirdChange);

        using (Assert.Multiple())
        {
            await Assert.That(changes.All(x => x.Sender == fixture)).IsTrue();

            await Assert.That(changes.All(x => x.GetPropertyName() == ChildIsOnlyOneWordName)).IsTrue();

            await Assert.That(changes.Select(x => x.Value!)).IsEquivalentTo([FooText, BarText, null]);
        }
    }

    /// <summary>Ensures ObservableForProperty works with non-reactive INPC objects.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<WithSchedulerExecutor>]
    public async Task OfpShouldWorkWithInpcObjectsToo()
    {
        var fixture = new NonReactiveInpcObject { InpcProperty = null! };

        var changes = fixture.ObservableForProperty(static x => x.InpcProperty.IsOnlyOneWord).Collect();

        fixture.InpcProperty = new();

        // ImmediateScheduler executes synchronously
        await Assert.That(changes).Count().IsEqualTo(1);

        fixture.InpcProperty.IsOnlyOneWord = FooText;

        // ImmediateScheduler executes synchronously
        await Assert.That(changes).Count().IsEqualTo(ExpectedCountAfterSecondChange);

        fixture.InpcProperty.IsOnlyOneWord = BarText;

        // ImmediateScheduler executes synchronously
        await Assert.That(changes).Count().IsEqualTo(ExpectedCountAfterThirdChange);
    }

    /// <summary>Simple child property observation test.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<WithSchedulerExecutor>]
    public async Task OfpSimpleChildPropertyTest()
    {
        var fixture = new HostTestFixture { Child = new() };

        var changes = fixture.ObservableForProperty(x => x.Child!.IsOnlyOneWord).Collect();

        fixture.Child.IsOnlyOneWord = FooText;

        // ImmediateScheduler executes synchronously
        await Assert.That(changes).Count().IsEqualTo(1);

        fixture.Child.IsOnlyOneWord = BarText;

        // ImmediateScheduler executes synchronously
        await Assert.That(changes).Count().IsEqualTo(ExpectedCountAfterSecondChange);

        fixture.Child.IsOnlyOneWord = BazText;

        // ImmediateScheduler executes synchronously
        await Assert.That(changes).Count().IsEqualTo(ExpectedCountAfterThirdChange);

        fixture.Child.IsOnlyOneWord = BazText;

        // ImmediateScheduler executes synchronously
        await Assert.That(changes).Count().IsEqualTo(ExpectedCountAfterThirdChange);

        using (Assert.Multiple())
        {
            await Assert.That(changes.All(x => x.Sender == fixture)).IsTrue();

            await Assert.That(changes.All(x => x.GetPropertyName() == ChildIsOnlyOneWordName)).IsTrue();

            await Assert.That(changes.Select(x => x.Value!)).IsEquivalentTo([FooText, BarText, BazText]);
        }
    }

    /// <summary>Simple property observation test.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<WithSchedulerExecutor>]
    [SuppressMessage(
        "Major Code Smell",
        "S4144:Methods should not have identical implementations",
        Justification = "Intentional duplicate test scenario.")]
    public async Task OfpSimplePropertyTest()
    {
        var fixture = new TestFixture();

        var changes = fixture.ObservableForProperty(x => x.IsOnlyOneWord).Collect();

        fixture.IsOnlyOneWord = FooText;

        // ImmediateScheduler executes synchronously
        await Assert.That(changes).Count().IsEqualTo(1);

        fixture.IsOnlyOneWord = BarText;

        // ImmediateScheduler executes synchronously
        await Assert.That(changes).Count().IsEqualTo(ExpectedCountAfterSecondChange);

        fixture.IsOnlyOneWord = BazText;

        // ImmediateScheduler executes synchronously
        await Assert.That(changes).Count().IsEqualTo(ExpectedCountAfterThirdChange);

        fixture.IsOnlyOneWord = BazText;

        // ImmediateScheduler executes synchronously
        await Assert.That(changes).Count().IsEqualTo(ExpectedCountAfterThirdChange);

        using (Assert.Multiple())
        {
            await Assert.That(changes.All(x => x.Sender == fixture)).IsTrue();

            await Assert.That(changes.All(x => x.GetPropertyName() == IsOnlyOneWordName)).IsTrue();

            await Assert.That(changes.Select(x => x.Value!)).IsEquivalentTo([FooText, BarText, BazText]);
        }
    }

    /// <summary>Tests SubscribeToExpressionChain basic functionality.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<WithSchedulerExecutor>]
    public async Task SubscribeToExpressionChain_BasicUsage_NotifiesOnChange()
    {
        var fixture = new HostTestFixture { Child = new() };

        Expression<Func<HostTestFixture, string?>> expression = x => x.Child!.IsOnlyOneWord;

        var results = new List<string?>();

        _ = fixture.SubscribeToExpressionChain<HostTestFixture, string?>(expression.Body).ObserveOn(Sequencer.Immediate).Subscribe(x => results.Add(x.Value));

        fixture.Child.IsOnlyOneWord = "First";

        // ImmediateScheduler executes synchronously
        await Assert.That(results).Count().IsEqualTo(1);

        await Assert.That(results[0]).IsEqualTo("First");

        fixture.Child.IsOnlyOneWord = "Second";

        // ImmediateScheduler executes synchronously
        await Assert.That(results).Count().IsEqualTo(ExpectedCountAfterSecondChange);

        await Assert.That(results[1]).IsEqualTo("Second");
    }

    /// <summary>Tests SubscribeToExpressionChain with beforeChange parameter.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<WithSchedulerExecutor>]
    public async Task SubscribeToExpressionChain_WithBeforeChange_NotifiesBeforeChange()
    {
        var fixture = new HostTestFixture { Child = new() { IsOnlyOneWord = InitialText } };

        Expression<Func<HostTestFixture, string?>> expression = x => x.Child!.IsOnlyOneWord;

        var results = new List<string?>();

        _ = fixture.SubscribeToExpressionChain<HostTestFixture, string?>(expression.Body, true).ObserveOn(Sequencer.Immediate).Subscribe(x => results.Add(x.Value));

        fixture.Child.IsOnlyOneWord = ChangedText;

        // ImmediateScheduler executes synchronously
        await Assert.That(results).Count().IsEqualTo(1);

        await Assert.That(results[0]).IsEqualTo(InitialText);
    }

    /// <summary>Tests SubscribeToExpressionChain with beforeChange and skipInitial.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<WithSchedulerExecutor>]
    public async Task SubscribeToExpressionChain_WithBeforeChangeAndSkipInitial_SkipsFirst()
    {
        var fixture = new HostTestFixture { Child = new() { IsOnlyOneWord = InitialText } };

        Expression<Func<HostTestFixture, string?>> expression = x => x.Child!.IsOnlyOneWord;

        var results = new List<string?>();

        _ = fixture.SubscribeToExpressionChain<HostTestFixture, string?>(
            expression.Body,
            true,
            true).ObserveOn(Sequencer.Immediate).Subscribe(x => results.Add(x.Value));

        // ImmediateScheduler executes synchronously
        await Assert.That(results).IsEmpty();

        fixture.Child.IsOnlyOneWord = ChangedText;

        // ImmediateScheduler executes synchronously
        await Assert.That(results).Count().IsEqualTo(1);

        await Assert.That(results[0]).IsEqualTo(InitialText);
    }

    /// <summary>Tests SubscribeToExpressionChain with isDistinct parameter.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<WithSchedulerExecutor>]
    public async Task SubscribeToExpressionChain_WithIsDistinct_Works()
    {
        var fixture = new HostTestFixture { Child = new() };

        Expression<Func<HostTestFixture, string?>> expression = x => x.Child!.IsOnlyOneWord;

        var results = new List<string?>();

        _ = fixture.SubscribeToExpressionChain<HostTestFixture, string?>(
            expression.Body,
            false,
            true,
            false,
            true).ObserveOn(Sequencer.Immediate).Subscribe(x => results.Add(x.Value));

        fixture.Child.IsOnlyOneWord = Value1Text;

        // ImmediateScheduler executes synchronously
        await Assert.That(results).Count().IsEqualTo(1);

        fixture.Child.IsOnlyOneWord = Value2Text;

        // ImmediateScheduler executes synchronously
        await Assert.That(results).Count().IsEqualTo(ExpectedCountAfterSecondChange);
    }

    /// <summary>Tests SubscribeToExpressionChain with suppressWarnings parameter.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<WithSchedulerExecutor>]
    public async Task SubscribeToExpressionChain_WithSuppressWarnings_DoesNotWarn()
    {
        var fixture = new HostTestFixture { Child = new() };

        Expression<Func<HostTestFixture, string?>> expression = x => x.Child!.IsOnlyOneWord;

        var results = new List<string?>();

        _ = fixture.SubscribeToExpressionChain<HostTestFixture, string?>(
            expression.Body,
            false,
            true,
            true).ObserveOn(Sequencer.Immediate).Subscribe(x => results.Add(x.Value));

        fixture.Child.IsOnlyOneWord = TestText;

        // ImmediateScheduler executes synchronously
        await Assert.That(results).Count().IsEqualTo(1);

        await Assert.That(results[0]).IsEqualTo(TestText);
    }
}
