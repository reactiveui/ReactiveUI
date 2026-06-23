// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections;
using System.ComponentModel;
using ReactiveUI.Tests.Utilities.Schedulers;
using TUnit.Core.Executors;

namespace ReactiveUI.Tests.ReactiveProperties;

/// <summary>Basic tests for ReactiveProperty covering core functionality.</summary>
public class ReactivePropertyBasicTests
{
    /// <summary>The initial value used when seeding a reactive property.</summary>
    private const int InitialValue = 42;

    /// <summary>The updated value applied to a reactive property in tests.</summary>
    private const int UpdatedValue = 100;

    /// <summary>The validation error message used for required-value tests.</summary>
    private const string RequiredError = "Required";

    /// <summary>The name of the Value property.</summary>
    private const string ValuePropertyName = "Value";

    /// <summary>Verifies the initial validation error is ignored while subsequent errors are detected.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task AddValidationErrorIgnoreInitialError()
    {
        using var rp = ReactiveProperty<string>.Create(null, Sequencer.Immediate, false, false);
        _ = rp.AddValidationError(x => string.IsNullOrEmpty(x) ? RequiredError : null, true);

        await Assert.That(rp.HasErrors).IsFalse(); // Initial error ignored

        rp.Value = string.Empty;
        await Assert.That(rp.HasErrors).IsTrue(); // Subsequent errors detected
    }

    /// <summary>Verifies validation errors supplied as an enumerable are applied and cleared.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task AddValidationErrorWithEnumerableFunction()
    {
        using var rp = ReactiveProperty<string>.Create(null, Sequencer.Immediate, false, false);
        _ = rp.AddValidationError(x => string.IsNullOrEmpty(x) ? new[] { RequiredError } : null);

        await Assert.That(rp.HasErrors).IsTrue();

        rp.Value = "test";
        await Assert.That(rp.HasErrors).IsFalse();
    }

    /// <summary>Verifies validation errors supplied via an observable function are applied and cleared.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task AddValidationErrorWithObservableFunction()
    {
        using var rp = ReactiveProperty<string>.Create(null, Sequencer.Immediate, false, false);
        _ = rp.AddValidationError(xs => xs.Select(x => string.IsNullOrEmpty(x) ? RequiredError : null));

        await Assert.That(rp.HasErrors).IsTrue();

        rp.Value = "test";
        await Assert.That(rp.HasErrors).IsFalse();
    }

    /// <summary>Verifies validation errors supplied via a synchronous function are applied and cleared.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task AddValidationErrorWithSyncFunction()
    {
        using var rp = ReactiveProperty<string>.Create(null, Sequencer.Immediate, false, false);
        _ = rp.AddValidationError(x => string.IsNullOrEmpty(x) ? RequiredError : null);

        await Assert.That(rp.HasErrors).IsTrue();

        rp.Value = "test";
        await Assert.That(rp.HasErrors).IsFalse();
    }

    /// <summary>Verifies that allowing duplicates emits identical values multiple times.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task AllowDuplicateValuesSendsMultipleIdenticalValues()
    {
        using var rp = ReactiveProperty<int>.Create(0, Sequencer.Immediate, false, true);
        var values = new List<int>();
        _ = rp.Subscribe(values.Add);

        var initialCount = values.Count;

        rp.Value = 0; // Same value
        await Assert.That(values.Count).IsGreaterThan(initialCount);
    }

    /// <summary>Verifies that invoking validation does not throw and leaves the value unchanged.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task CheckValidationInvokesValidation()
    {
        using var rp = ReactiveProperty<int>.Create(0);
        rp.CheckValidation(); // Should not throw
        await Assert.That(rp.Value).IsEqualTo(0);
    }

    /// <summary>Verifies the constructor stores the supplied initial value.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task ConstructorWithInitialValueSetsValue()
    {
        using var rp = ReactiveProperty<int>.Create(InitialValue);
        await Assert.That(rp.Value).IsEqualTo(InitialValue);
    }

    /// <summary>Verifies the default constructor creates a property with a null value.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task DefaultConstructorCreatesPropertyWithNullValue()
    {
        using var rp = ReactiveProperty<string>.Create();
        await Assert.That(rp.Value).IsNull();
    }

    /// <summary>Verifies that disposing the property sets its disposed flag.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task DisposeSetsIsDisposed()
    {
        var rp = ReactiveProperty<int>.Create();
        await Assert.That(rp.IsDisposed).IsFalse();

        rp.Dispose();
        await Assert.That(rp.IsDisposed).IsTrue();
    }

    /// <summary>Verifies that duplicate values are suppressed when distinct-until-changed is enabled.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task DistinctUntilChangedDoesNotSendDuplicates()
    {
        using var rp = ReactiveProperty<int>.Create(0, Sequencer.Immediate, false, false);
        var values = new List<int>();
        _ = rp.Subscribe(values.Add);

        var initialCount = values.Count;

        rp.Value = 0; // Same value, should not trigger
        await Assert.That(values.Count).IsEqualTo(initialCount);
    }

    /// <summary>Verifies the errors changed event fires when a validation error is added.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task ErrorsChangedEventFires()
    {
        using var rp = ReactiveProperty<string>.Create(null, Sequencer.Immediate, false, false);
        var fired = false;

        rp.ErrorsChanged += (_, _) => fired = true;
        _ = rp.AddValidationError(x => string.IsNullOrEmpty(x) ? RequiredError : null);

        await Assert.That(fired).IsTrue();
    }

    /// <summary>Verifies the <see cref="INotifyDataErrorInfo"/> implementation returns a non-null result when there are no errors.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task GetErrorsINotifyDataErrorInfoReturnsEmptyWhenNoErrors()
    {
        using var rp = ReactiveProperty<string>.Create();
        var errors = ((INotifyDataErrorInfo)rp).GetErrors(ValuePropertyName);
        await Assert.That(errors).IsNotNull();
    }

    /// <summary>Verifies that getting errors returns null when there are no errors.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task GetErrorsReturnsNullWhenNoErrors()
    {
        using var rp = ReactiveProperty<string>.Create();
        var errors = rp.GetErrors(ValuePropertyName);
        await Assert.That(errors is null).IsTrue();
    }

    /// <summary>Verifies that a newly created property reports no errors.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task HasErrorsInitiallyFalse()
    {
        using var rp = ReactiveProperty<string>.Create();
        await Assert.That(rp.HasErrors).IsFalse();
    }

    /// <summary>Verifies that multiple subscribers receive value updates, with later subscribers getting the current value.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task MultipleSubscribersReceiveUpdates()
    {
        const int SecondValue = 2;
        using var rp = ReactiveProperty<int>.Create(0, Sequencer.Immediate, false, false);
        var values1 = new List<int>();
        var values2 = new List<int>();

        _ = rp.Subscribe(values1.Add);

        rp.Value = 1;

        _ = rp.Subscribe(values2.Add);

        rp.Value = SecondValue;

        await Assert.That(values1).Contains(0);
        await Assert.That(values1).Contains(1);
        await Assert.That(values1).Contains(SecondValue);

        await Assert.That(values2).Contains(1); // Gets current value on subscribe
        await Assert.That(values2).Contains(SecondValue);
    }

    /// <summary>Verifies that multiple validation errors are combined for the property.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task MultipleValidationErrorsAreCombined()
    {
        const int MinimumLength = 3;
        using var rp = ReactiveProperty<string>.Create(null, Sequencer.Immediate, false, false);
        _ = rp.AddValidationError(x => string.IsNullOrEmpty(x) ? RequiredError : null!)
            .AddValidationError(x => x?.Length < MinimumLength ? "Too short" : null);

        await Assert.That(rp.HasErrors).IsTrue();

        var errors = rp.GetErrors(ValuePropertyName);
        await Assert.That(errors).IsNotNull();
    }

    /// <summary>Verifies that the observable error stream emits when validation errors change.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task ObserveErrorChangedEmitsErrors()
    {
        using var rp = ReactiveProperty<string>.Create(null, Sequencer.Immediate, false, false);
        var errors = new List<IEnumerable?>();
        _ = rp.ObserveErrorChanged.Subscribe(errors.Add);

        _ = rp.AddValidationError(x => string.IsNullOrEmpty(x) ? RequiredError : null);

        await Assert.That(errors.Count).IsGreaterThan(0);
    }

    /// <summary>Verifies that the observable has-errors stream emits the error state as it changes.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task ObserveHasErrorsEmitsErrorState()
    {
        using var rp = ReactiveProperty<string>.Create(null, Sequencer.Immediate, false, false);
        var hasErrorsValues = new List<bool>();
        _ = rp.ObserveHasErrors.Subscribe(hasErrorsValues.Add);

        _ = rp.AddValidationError(x => string.IsNullOrEmpty(x) ? RequiredError : null);

        await Assert.That(hasErrorsValues).Contains(true);

        rp.Value = "test";
        await Assert.That(hasErrorsValues).Contains(false);
    }

    /// <summary>Verifies the property changed event fires with the expected property name when the value is set.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task PropertyChangedEventFires()
    {
        using var rp = ReactiveProperty<int>.Create(0);
        var fired = false;
        var propertyName = string.Empty;

        ((INotifyPropertyChanged)rp).PropertyChanged += (_, args) =>
        {
            fired = true;
            propertyName = args.PropertyName ?? string.Empty;
        };

        rp.Value = InitialValue;

        using (Assert.Multiple())
        {
            await Assert.That(fired).IsTrue();
            await Assert.That(propertyName).IsEqualTo(nameof(ReactiveProperty<>.Value));
        }
    }

    /// <summary>Verifies that refreshing re-emits the current value even when it is unchanged.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task RefreshSendsCurrentValueEvenIfUnchanged()
    {
        using var rp = ReactiveProperty<int>.Create(InitialValue, Sequencer.Immediate, false, false);
        var values = new List<int>();
        _ = rp.Subscribe(values.Add);

        var countBefore = values.Count;

        rp.Refresh();
        await Assert.That(values.Count).IsGreaterThan(countBefore);
        await Assert.That(values[^1]).IsEqualTo(InitialValue);
    }

    /// <summary>Verifies that notifications are delivered on the supplied scheduler.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    [TestExecutor<WithVirtualTimeSchedulerExecutor>]
    public async Task SchedulerIsUsedForNotifications()
    {
        var scheduler = TestContext.Current.GetVirtualTimeScheduler();
        using var rp = ReactiveProperty<int>.Create(0, scheduler, false, false);
        var values = new List<int>();
        _ = rp.Subscribe(values.Add);

        // Value should not be received until scheduler advances
        await Assert.That(values).IsEmpty();

        scheduler.Start();
        await Assert.That(values).Contains(0);
    }

    /// <summary>Verifies that subscribers do not receive the current value when skip-current is enabled.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task SkipCurrentValueOnSubscribe()
    {
        using var rp = ReactiveProperty<int>.Create(InitialValue, Sequencer.Immediate, true, false);
        var values = new List<int>();
        _ = rp.Subscribe(values.Add);

        await Assert.That(values).IsEmpty(); // Should not receive initial value

        rp.Value = UpdatedValue;
        await Assert.That(values).Contains(UpdatedValue);
    }

    /// <summary>Verifies the static create method overloads produce properties with the expected values.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task StaticCreateMethodsWork()
    {
        using var rp1 = ReactiveProperty<int>.Create();
        await Assert.That(rp1).IsNotNull();

        using var rp2 = ReactiveProperty<int>.Create(InitialValue);
        await Assert.That(rp2.Value).IsEqualTo(InitialValue);

        using var rp3 = ReactiveProperty<int>.Create(InitialValue, true, false);
        await Assert.That(rp3.Value).IsEqualTo(InitialValue);

        using var rp4 = ReactiveProperty<int>.Create(InitialValue, Sequencer.Immediate, false, false);
        await Assert.That(rp4.Value).IsEqualTo(InitialValue);
    }

    /// <summary>Verifies that subscribing after disposal completes the subscription immediately.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task SubscribeAfterDisposeCompletesImmediately()
    {
        var rp = ReactiveProperty<int>.Create(InitialValue);
        rp.Dispose();

        var completed = false;
        _ = rp.Subscribe(
            _ => { },
            () => completed = true);

        await Assert.That(completed).IsTrue();
    }

    /// <summary>Verifies that a subscriber receives the current value upon subscribing.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task SubscribeReceivesCurrentValue()
    {
        using var rp = ReactiveProperty<int>.Create(InitialValue, Sequencer.Immediate, false, false);
        var received = 0;
        _ = rp.Subscribe(x => received = x);

        await Assert.That(received).IsEqualTo(InitialValue);
    }

    /// <summary>Verifies that a subscriber receives subsequent value changes.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task SubscribeReceivesValueChanges()
    {
        const int SecondValue = 2;
        using var rp = ReactiveProperty<int>.Create(0, Sequencer.Immediate, false, false);
        var values = new List<int>();
        _ = rp.Subscribe(values.Add);

        rp.Value = 1;
        rp.Value = SecondValue;

        await Assert.That(values).Contains(0);
        await Assert.That(values).Contains(1);
        await Assert.That(values).Contains(SecondValue);
    }

    /// <summary>Verifies that subscribing with a null observer returns a non-null disposable.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task SubscribeWithNullObserverReturnsEmptyDisposable()
    {
        using var rp = ReactiveProperty<int>.Create();
        var disposable = rp.Subscribe(null!);

        await Assert.That(disposable).IsNotNull();
    }

    /// <summary>Verifies the value getter returns the current value.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task ValuePropertyGetterReturnsCurrentValue()
    {
        using var rp = ReactiveProperty<string>.Create("test");
        await Assert.That(rp.Value).IsEqualTo("test");
    }

    /// <summary>Verifies the value setter updates the current value.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task ValuePropertySetterUpdatesValue()
    {
        using var rp = ReactiveProperty<string>.Create();
        rp.Value = "new value";
        await Assert.That(rp.Value).IsEqualTo("new value");
    }
}
