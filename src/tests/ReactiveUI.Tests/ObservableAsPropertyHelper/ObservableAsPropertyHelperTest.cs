// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Tests.ReactiveObjects.Mocks;
using ReactiveUI.Tests.Utilities;
using ReactiveUI.Tests.Utilities.Schedulers;
using TUnit.Core.Executors;

namespace ReactiveUI.Tests.ObservableAsPropertyHelper;

/// <summary>Tests for the <see cref="ObservableAsPropertyHelper{T}"/> behavior.</summary>
public partial class ObservableAsPropertyHelperTest
{
    /// <summary>The value emitted by the source observables used across these tests.</summary>
    private const int EmittedValue = 42;

    /// <summary>No thrown-exceptions subscriber equals OAPH death.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<WithSchedulerExecutor>]
    public async Task NoThrownExceptionsSubscriberEqualsOaphDeath()
    {
        const int InitialValue = -5;
        const int SecondInput = 2;
        const int ThirdInput = 3;
        const int ExpectedLastValue = 4;
        var input = new Signal<int>();
        var fixture = new ObservableAsPropertyHelper<int>(input, static _ => { }, InitialValue, scheduler: Sequencer.Immediate);

        await Assert.That(fixture.Value).IsEqualTo(InitialValue);
        new[] { 1, SecondInput, ThirdInput, ExpectedLastValue }.Run(input.OnNext);

        var exception = Assert.Throws<UnhandledErrorException>(() => input.OnError(new InvalidOperationException("Die!")));

        using (Assert.Multiple())
        {
            await Assert.That(exception.InnerException?.Message).IsEqualTo("Die!");
            await Assert.That(fixture.Value).IsEqualTo(ExpectedLastValue);
        }
    }

    /// <summary>Nullable types test shouldn't need decorators with ToProperty.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task NullableTypesTestShouldNotNeedDecorators2_ToProperty()
    {
        const int ExpectedAccountsFound = 3;
        var fixture = new WhenAnyTestFixture();
        _ = fixture.WhenAnyValue(
            static x => x.ProjectService.ProjectsNullable,
            static x => x.AccountService.AccountUsersNullable).Where(static tuple => tuple.Value1.Count > 0 && tuple.Value2.Count > 0).Select(static tuple =>
                {
                    var (_, users) = tuple;
                    return users.Values.Count(static x => !string.IsNullOrWhiteSpace(x?.LastName));
                }).ToProperty(fixture, static x => x.AccountsFound, out var helper);

        fixture.AccountsFoundHelper = helper;

        await Assert.That(fixture.AccountsFound).IsEqualTo(ExpectedAccountsFound);
    }

    /// <summary>Defer subscription parameter defers subscription to source.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task OaphDeferSubscriptionParameterDefersSubscriptionToSource()
    {
        var isSubscribed = false;

        var observable = Signal.Create<int>(o =>
        {
            isSubscribed = true;
            o.OnNext(EmittedValue);
            o.OnCompleted();
            return Scope.Empty;
        });

        var fixture = new ObservableAsPropertyHelper<int>(observable, static _ => { }, 0, true);

        using (Assert.Multiple())
        {
            await Assert.That(isSubscribed).IsFalse();
            await Assert.That(fixture.Value).IsEqualTo(EmittedValue);
        }

        await Assert.That(isSubscribed).IsTrue();
    }

    /// <summary>Defer subscription: IsSubscribed is not true initially.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task OaphDeferSubscriptionParameterIsSubscribedIsNotTrueInitially()
    {
        var observable = Signal.Create<int>(static o =>
        {
            o.OnNext(EmittedValue);
            o.OnCompleted();
            return Scope.Empty;
        });

        var fixture = new ObservableAsPropertyHelper<int>(observable, static _ => { }, 0, true);

        using (Assert.Multiple())
        {
            await Assert.That(fixture.IsSubscribed).IsFalse();
            await Assert.That(fixture.Value).IsEqualTo(EmittedValue);
            await Assert.That(fixture.IsSubscribed).IsTrue();
        }
    }

    /// <summary>Defer subscription should not throw if disposed.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task OaphDeferSubscriptionShouldNotThrowIfDisposed()
    {
        var observable = Signal.Create<int>(static o =>
        {
            o.OnNext(EmittedValue);
            o.OnCompleted();
            return Scope.Empty;
        });

        var fixture = new ObservableAsPropertyHelper<int>(observable, static _ => { }, 0, true);

        await Assert.That(fixture.IsSubscribed).IsFalse();
        fixture.Dispose();

        await Assert.That(() =>
        {
            _ = fixture.Value;
            return Task.CompletedTask;
        }).ThrowsNothing();

        var value = fixture.Value;
        await Assert.That(value).IsEqualTo(0);
    }

    /// <summary>
    ///     Verifies that deferred subscription with an initial value provided by a function emits the initial value
    ///     only when subscribed and confirms the function is accessed at that point.
    ///     Ensures that the subscription state and access status align with the expected behavior.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task OaphDeferSubscriptionWithInitialFuncValueEmitInitialValueWhenSubscribed()
    {
        var observable = Signal.None<int>(Sequencer.Immediate);
        var wasAccessed = false;

        var fixture = new ObservableAsPropertyHelper<int>(
            observable,
            static _ => { },
            getInitialValue: GetInitialValue,
            deferSubscription: true);

        using (Assert.Multiple())
        {
            await Assert.That(fixture.IsSubscribed).IsFalse();
            await Assert.That(wasAccessed).IsFalse();
        }

        var result = fixture.Value;

        using (Assert.Multiple())
        {
            await Assert.That(fixture.IsSubscribed).IsTrue();
            await Assert.That(wasAccessed).IsTrue();
            await Assert.That(result).IsEqualTo(EmittedValue);
        }

        return;

        int GetInitialValue()
        {
            wasAccessed = true;
            return EmittedValue;
        }
    }

    /// <summary>
    ///     Ensures that defer subscription with an initial function value does not trigger the OnChanged callback
    ///     when the source observable provides the same initial value.
    /// </summary>
    /// <param name="initialValue">The initial value provided to the ObservableAsPropertyHelper.</param>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    [Arguments(0)]
    [Arguments(42)]
    public async Task OaphDeferSubscriptionWithInitialFuncValueNotCallOnChangedWhenSourceProvidesInitialValue(
        int initialValue)
    {
        var observable = new Signal<int>();
        var wasOnChangingCalled = false;
        var wasOnChangedCalled = false;

        var fixture = new ObservableAsPropertyHelper<int>(observable, OnChanged, OnChanging, () => initialValue, true);

        var result = fixture.Value;
        await Assert.That(result).IsEqualTo(initialValue);

        observable.OnNext(initialValue);

        using (Assert.Multiple())
        {
            await Assert.That(wasOnChangingCalled).IsFalse();
            await Assert.That(wasOnChangedCalled).IsFalse();
        }

        return;

        void OnChanged(int _) => wasOnChangedCalled = true;

        void OnChanging(int _) => wasOnChangingCalled = true;
    }

    /// <summary>Verifies that deferring subscription with an initial function value does not trigger OnChanged when subscribed.</summary>
    /// <param name="initialValue">The initial value to set before any subscription occurs.</param>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    [Arguments(0)]
    [Arguments(42)]
    public async Task OaphDeferSubscriptionWithInitialFuncValueNotCallOnChangedWhenSubscribed(int initialValue)
    {
        var observable = Signal.None<int>(Sequencer.Immediate);

        var wasOnChangingCalled = false;
        var wasOnChangedCalled = false;

        var fixture = new ObservableAsPropertyHelper<int>(observable, OnChanged, OnChanging, () => initialValue, true);

        using (Assert.Multiple())
        {
            await Assert.That(fixture.IsSubscribed).IsFalse();
            await Assert.That(wasOnChangingCalled).IsFalse();
            await Assert.That(wasOnChangedCalled).IsFalse();
        }

        var result = fixture.Value;

        using (Assert.Multiple())
        {
            await Assert.That(fixture.IsSubscribed).IsTrue();
            await Assert.That(wasOnChangingCalled).IsFalse();
            await Assert.That(wasOnChangedCalled).IsFalse();
            await Assert.That(result).IsEqualTo(initialValue);
        }

        return;

        void OnChanged(int _) => wasOnChangedCalled = true;

        void OnChanging(int _) => wasOnChangingCalled = true;
    }

    /// <summary>Defer subscription with initial function value should not emit initial value nor access function.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task OaphDeferSubscriptionWithInitialFuncValueShouldNotEmitInitialValueNorAccessFunc()
    {
        var observable = Signal.None<int>(Sequencer.Immediate);

        var fixture = new ObservableAsPropertyHelper<int>(
            observable,
            static _ => { },
            getInitialValue: ThrowIfAccessed,
            deferSubscription: true);

        await Assert.That(fixture.IsSubscribed).IsFalse();

        int? emittedValue = null;
        _ = fixture.Source.Subscribe(val => emittedValue = val);

        using (Assert.Multiple())
        {
            await Assert.That(emittedValue).IsNull();
            await Assert.That(fixture.IsSubscribed).IsFalse();
        }

        return;

        static int ThrowIfAccessed() => throw new InvalidOperationException();
    }

    /// <summary>Ensures that defer subscription with an initial value emits the initial value upon subscription.</summary>
    /// <param name="initialValue">
    ///     The initial value set before any subscription occurs.
    /// </param>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    [Arguments(0)]
    [Arguments(42)]
    public async Task OaphDeferSubscriptionWithInitialValueEmitInitialValueWhenSubscribed(int initialValue)
    {
        var observable = Signal.None<int>(Sequencer.Immediate);
        var fixture = new ObservableAsPropertyHelper<int>(
            observable,
            static _ => { },
            initialValue,
            true);

        await Assert.That(fixture.IsSubscribed).IsFalse();

        var result = fixture.Value;

        using (Assert.Multiple())
        {
            await Assert.That(fixture.IsSubscribed).IsTrue();
            await Assert.That(result).IsEqualTo(initialValue);
        }
    }

    /// <summary>Verifies that deferring subscription with an initial value does not emit the initial value.</summary>
    /// <param name="initialValue">The initial value to test with.</param>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    [Arguments(0)]
    [Arguments(42)]
    public async Task OaphDeferSubscriptionWithInitialValueShouldNotEmitInitialValue(int initialValue)
    {
        var observable = Signal.None<int>(Sequencer.Immediate);
        var fixture = new ObservableAsPropertyHelper<int>(observable, static _ => { }, initialValue, true);

        await Assert.That(fixture.IsSubscribed).IsFalse();

        int? emittedValue = null;
        _ = fixture.Source.Subscribe(val => emittedValue = val);

        using (Assert.Multiple())
        {
            await Assert.That(emittedValue).IsNull();
            await Assert.That(fixture.IsSubscribed).IsFalse();
        }
    }

    /// <summary>Verifies that the initial value of an Observable As Property Helper is emitted correctly.</summary>
    /// <param name="initialValue">The initial value provided to the Observable As Property Helper.</param>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    [Arguments(0)]
    [Arguments(42)]
    public async Task OaphInitialValueShouldEmitInitialValue(int initialValue)
    {
        var observable = Signal.None<int>(Sequencer.Immediate);
        var fixture = new ObservableAsPropertyHelper<int>(observable, static _ => { }, initialValue);

        await Assert.That(fixture.IsSubscribed).IsTrue();

        int? emittedValue = null;
        _ = fixture.Source.Subscribe(val => emittedValue = val);

        await Assert.That(emittedValue).IsEqualTo(initialValue);
    }

    /// <summary>Tests that Observable As Property Helpers should fire change notifications.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<WithSchedulerExecutor>]
    public async Task OaphShouldFireChangeNotifications()
    {
        const int InitialValue = -5;
        const int SecondInput = 2;
        const int ThirdInput = 3;
        const int FourthInput = 4;
        var scheduler = TestContext.Current!.GetScheduler();
        var input = new[] { 1, SecondInput, ThirdInput, ThirdInput, FourthInput }.ToObservable();
        var output = new List<int>();

        var fixture = new ObservableAsPropertyHelper<int>(
            input,
            output.Add,
            InitialValue,
            scheduler: scheduler);

        // ImmediateScheduler executes synchronously, no need for scheduler.Start()
        using (Assert.Multiple())
        {
            await Assert.That(fixture.Value).IsEqualTo(await input.LastAsync());

            // Suppresses duplicate notifications (note single '3')
            await Assert.That(output).IsEquivalentTo([InitialValue, 1, SecondInput, ThirdInput, FourthInput]);
        }
    }

    /// <summary>Tests that OAPH should provide initial value immediately regardless of scheduler.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<WithSchedulerExecutor>]
    public async Task OaphShouldProvideInitialValueImmediatelyRegardlessOfScheduler()
    {
        const int InitialValue = 32;
        var scheduler = TestContext.Current!.GetScheduler();
        var output = new List<int>();

        var fixture = new ObservableAsPropertyHelper<int>(
            ReactiveUI.Primitives.Signals.Signal.Silent<int>(),
            output.Add,
            InitialValue,
            scheduler: scheduler);

        await Assert.That(fixture.Value).IsEqualTo(InitialValue);
    }

    /// <summary>Tests that OAPH should provide latest value.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<WithSchedulerExecutor>]
    public async Task OaphShouldProvideLatestValue()
    {
        const int InitialValue = -5;
        const int SecondInput = 2;
        const int ThirdInput = 3;
        const int ExpectedLastValue = 4;
        var scheduler = TestContext.Current!.GetScheduler();
        var input = new Signal<int>();

        var fixture = new ObservableAsPropertyHelper<int>(
            input,
            static _ => { },
            InitialValue,
            scheduler: scheduler);

        await Assert.That(fixture.Value).IsEqualTo(InitialValue);

        new[] { 1, SecondInput, ThirdInput, ExpectedLastValue }.Run(input.OnNext);

        // ImmediateScheduler executes synchronously, no need for scheduler.Start()
        await Assert.That(fixture.Value).IsEqualTo(ExpectedLastValue);

        input.OnCompleted();

        // ImmediateScheduler executes synchronously, no need for scheduler.Start()
        await Assert.That(fixture.Value).IsEqualTo(ExpectedLastValue);
    }

    /// <summary>OAPH should rethrow errors via ThrownExceptions.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<WithSchedulerExecutor>]
    public async Task OaphShouldRethrowErrors()
    {
        const int InitialValue = -5;
        const int SecondInput = 2;
        const int ThirdInput = 3;
        const int ExpectedLastValue = 4;
        var scheduler = TestContext.Current!.GetScheduler();
        var input = new Signal<int>();
        var fixture = new ObservableAsPropertyHelper<int>(input, static _ => { }, InitialValue, scheduler: scheduler);
        var errors = new List<Exception>();

        await Assert.That(fixture.Value).IsEqualTo(InitialValue);
        new[] { 1, SecondInput, ThirdInput, ExpectedLastValue }.Run(input.OnNext);

        _ = fixture.ThrownExceptions.Subscribe(errors.Add);

        // ImmediateScheduler executes synchronously, no need for scheduler.Start()
        await Assert.That(fixture.Value).IsEqualTo(ExpectedLastValue);

        input.OnError(new InvalidOperationException("Die!"));

        // ImmediateScheduler executes synchronously, no need for scheduler.Start()
        using (Assert.Multiple())
        {
            await Assert.That(fixture.Value).IsEqualTo(ExpectedLastValue);
            await Assert.That(errors).Count().IsEqualTo(1);
        }
    }

    /// <summary>Tests that Observable As Property Helpers should skip first value if it matches the initial value.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<WithSchedulerExecutor>]
    public async Task OaphShouldSkipFirstValueIfItMatchesTheInitialValue()
    {
        const int SecondInput = 2;
        const int ThirdInput = 3;
        var scheduler = TestContext.Current!.GetScheduler();
        var input = new[] { 1, SecondInput, ThirdInput }.ToObservable();
        var output = new List<int>();

        var fixture = new ObservableAsPropertyHelper<int>(
            input,
            output.Add,
            1,
            scheduler: scheduler);

        // ImmediateScheduler executes synchronously, no need for scheduler.Start()
        using (Assert.Multiple())
        {
            await Assert.That(fixture.Value).IsEqualTo(await input.LastAsync());
            await Assert.That(output).IsEquivalentTo([1, SecondInput, ThirdInput]);
        }
    }
}
