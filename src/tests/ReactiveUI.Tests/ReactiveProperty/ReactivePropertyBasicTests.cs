// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections;

using Microsoft.Reactive.Testing;

namespace ReactiveUI.Tests.ReactiveProperty;

/// <summary>
/// Basic tests for ReactiveProperty covering core functionality.
/// </summary>
public class ReactivePropertyBasicTests
{
    [Test]
    public async Task DefaultConstructorCreatesPropertyWithNullValue()
    {
        using var rp = ReactiveProperty<string>.Create();
        await Assert.That(rp.Value).IsNull();
    }

    [Test]
    public async Task ConstructorWithInitialValueSetsValue()
    {
        using var rp = ReactiveProperty<int>.Create(42);
        await Assert.That(rp.Value).IsEqualTo(42);
    }

    [Test]
    public async Task ValuePropertyGetterReturnsCurrentValue()
    {
        using var rp = ReactiveProperty<string>.Create("test");
        await Assert.That(rp.Value).IsEqualTo("test");
    }

    [Test]
    public async Task ValuePropertySetterUpdatesValue()
    {
        using var rp = ReactiveProperty<string>.Create();
        rp.Value = "new value";
        await Assert.That(rp.Value).IsEqualTo("new value");
    }

    [Test]
    public async Task SubscribeReceivesCurrentValue()
    {
        using var rp = ReactiveProperty<int>.Create(42, ImmediateScheduler.Instance, false, false);
        var received = 0;
        rp.Subscribe(x => received = x);

        await Assert.That(received).IsEqualTo(42);
    }

    [Test]
    public async Task SubscribeReceivesValueChanges()
    {
        using var rp = ReactiveProperty<int>.Create(0, ImmediateScheduler.Instance, false, false);
        var values = new List<int>();
        rp.Subscribe(x => values.Add(x));

        rp.Value = 1;
        rp.Value = 2;

        await Assert.That(values).Contains(0);
        await Assert.That(values).Contains(1);
        await Assert.That(values).Contains(2);
    }

    [Test]
    public async Task SkipCurrentValueOnSubscribe()
    {
        using var rp = ReactiveProperty<int>.Create(42, ImmediateScheduler.Instance, skipCurrentValueOnSubscribe: true, allowDuplicateValues: false);
        var values = new List<int>();
        rp.Subscribe(x => values.Add(x));

        await Assert.That(values).IsEmpty(); // Should not receive initial value

        rp.Value = 100;
        await Assert.That(values).Contains(100);
    }

    [Test]
    public async Task AllowDuplicateValuesSendsMultipleIdenticalValues()
    {
        using var rp = ReactiveProperty<int>.Create(0, ImmediateScheduler.Instance, skipCurrentValueOnSubscribe: false, allowDuplicateValues: true);
        var values = new List<int>();
        rp.Subscribe(x => values.Add(x));

        var initialCount = values.Count;

        rp.Value = 0; // Same value
        await Assert.That(values.Count).IsGreaterThan(initialCount);
    }

    [Test]
    public async Task DistinctUntilChangedDoesNotSendDuplicates()
    {
        using var rp = ReactiveProperty<int>.Create(0, ImmediateScheduler.Instance, skipCurrentValueOnSubscribe: false, allowDuplicateValues: false);
        var values = new List<int>();
        rp.Subscribe(x => values.Add(x));

        var initialCount = values.Count;

        rp.Value = 0; // Same value, should not trigger
        await Assert.That(values.Count).IsEqualTo(initialCount);
    }

    [Test]
    public async Task RefreshSendsCurrentValueEvenIfUnchanged()
    {
        using var rp = ReactiveProperty<int>.Create(42, ImmediateScheduler.Instance, false, false);
        var values = new List<int>();
        rp.Subscribe(x => values.Add(x));

        var countBefore = values.Count;

        rp.Refresh();
        await Assert.That(values.Count).IsGreaterThan(countBefore);
        await Assert.That(values.Last()).IsEqualTo(42);
    }

    [Test]
    public async Task DisposeSetsIsDisposed()
    {
        var rp = ReactiveProperty<int>.Create();
        await Assert.That(rp.IsDisposed).IsFalse();

        rp.Dispose();
        await Assert.That(rp.IsDisposed).IsTrue();
    }

    [Test]
    public async Task SubscribeAfterDisposeCompletesImmediately()
    {
        var rp = ReactiveProperty<int>.Create(42);
        rp.Dispose();

        var completed = false;
        rp.Subscribe(
            onNext: _ => { },
            onCompleted: () => completed = true);

        await Assert.That(completed).IsTrue();
    }

    [Test]
    public async Task SubscribeWithNullObserverReturnsEmptyDisposable()
    {
        using var rp = ReactiveProperty<int>.Create();
        var disposable = rp.Subscribe(null!);

        await Assert.That(disposable).IsNotNull();
    }

    [Test]
    public async Task HasErrorsInitiallyFalse()
    {
        using var rp = ReactiveProperty<string>.Create();
        await Assert.That(rp.HasErrors).IsFalse();
    }

    [Test]
    public async Task GetErrorsReturnsNullWhenNoErrors()
    {
        using var rp = ReactiveProperty<string>.Create();
        var errors = rp.GetErrors("Value");
        await Assert.That(errors == null).IsTrue();
    }

    [Test]
    public async Task GetErrorsINotifyDataErrorInfoReturnsEmptyWhenNoErrors()
    {
        using var rp = ReactiveProperty<string>.Create();
        var errors = ((INotifyDataErrorInfo)rp).GetErrors("Value");
        await Assert.That(errors).IsNotNull();
    }

    [Test]
    public async Task CheckValidationInvokesValidation()
    {
        using var rp = ReactiveProperty<int>.Create(0);
        rp.CheckValidation(); // Should not throw
        await Assert.That(rp.Value).IsEqualTo(0);
    }

    [Test]
    public async Task AddValidationErrorWithSyncFunction()
    {
        using var rp = ReactiveProperty<string>.Create(null, ImmediateScheduler.Instance, false, false);
        rp.AddValidationError(x => string.IsNullOrEmpty(x) ? "Required" : null);

        await Assert.That(rp.HasErrors).IsTrue();

        rp.Value = "test";
        await Assert.That(rp.HasErrors).IsFalse();
    }

    [Test]
    public async Task AddValidationErrorWithObservableFunction()
    {
        using var rp = ReactiveProperty<string>.Create(null, ImmediateScheduler.Instance, false, false);
        rp.AddValidationError(xs => xs.Select(x => string.IsNullOrEmpty(x) ? "Required" : null));

        await Assert.That(rp.HasErrors).IsTrue();

        rp.Value = "test";
        await Assert.That(rp.HasErrors).IsFalse();
    }

    [Test]
    public async Task AddValidationErrorWithEnumerableFunction()
    {
        using var rp = ReactiveProperty<string>.Create(null, ImmediateScheduler.Instance, false, false);
        rp.AddValidationError(x => string.IsNullOrEmpty(x) ? new[] { "Required" } : null);

        await Assert.That(rp.HasErrors).IsTrue();

        rp.Value = "test";
        await Assert.That(rp.HasErrors).IsFalse();
    }

    [Test]
    public async Task AddValidationErrorIgnoreInitialError()
    {
        using var rp = ReactiveProperty<string>.Create(null, ImmediateScheduler.Instance, false, false);
        rp.AddValidationError(x => string.IsNullOrEmpty(x) ? "Required" : null, ignoreInitialError: true);

        await Assert.That(rp.HasErrors).IsFalse(); // Initial error ignored

        rp.Value = string.Empty;
        await Assert.That(rp.HasErrors).IsTrue(); // Subsequent errors detected
    }

    [Test]
    public async Task ObserveErrorChangedEmitsErrors()
    {
        using var rp = ReactiveProperty<string>.Create(null, ImmediateScheduler.Instance, false, false);
        var errors = new List<IEnumerable?>();
        rp.ObserveErrorChanged.Subscribe(errors.Add);

        rp.AddValidationError(x => string.IsNullOrEmpty(x) ? "Required" : null);

        await Assert.That(errors.Count).IsGreaterThan(0);
    }

    [Test]
    public async Task ObserveHasErrorsEmitsErrorState()
    {
        using var rp = ReactiveProperty<string>.Create(null, ImmediateScheduler.Instance, false, false);
        var hasErrorsValues = new List<bool>();
        rp.ObserveHasErrors.Subscribe(hasErrorsValues.Add);

        rp.AddValidationError(x => string.IsNullOrEmpty(x) ? "Required" : null);

        await Assert.That(hasErrorsValues).Contains(true);

        rp.Value = "test";
        await Assert.That(hasErrorsValues).Contains(false);
    }

    [Test]
    public async Task ErrorsChangedEventFires()
    {
        using var rp = ReactiveProperty<string>.Create(null, ImmediateScheduler.Instance, false, false);
        var fired = false;

        rp.ErrorsChanged += (_, _) => fired = true;
        rp.AddValidationError(x => string.IsNullOrEmpty(x) ? "Required" : null);

        await Assert.That(fired).IsTrue();
    }

    [Test]
    public async Task MultipleValidationErrorsAreCombined()
    {
        using var rp = ReactiveProperty<string>.Create(null, ImmediateScheduler.Instance, false, false);
        rp.AddValidationError(x => string.IsNullOrEmpty(x) ? "Required" : null!)
            .AddValidationError(x => x?.Length < 3 ? "Too short" : null);

        await Assert.That(rp.HasErrors).IsTrue();

        var errors = rp.GetErrors("Value");
        await Assert.That(errors).IsNotNull();
    }

    [Test]
    public async Task StaticCreateMethodsWork()
    {
        using var rp1 = ReactiveProperty<int>.Create();
        await Assert.That(rp1).IsNotNull();

        using var rp2 = ReactiveProperty<int>.Create(42);
        await Assert.That(rp2.Value).IsEqualTo(42);

        using var rp3 = ReactiveProperty<int>.Create(42, true, false);
        await Assert.That(rp3.Value).IsEqualTo(42);

        using var rp4 = ReactiveProperty<int>.Create(42, ImmediateScheduler.Instance, false, false);
        await Assert.That(rp4.Value).IsEqualTo(42);
    }

    [Test]
    public async Task SchedulerIsUsedForNotifications()
    {
        var scheduler = new TestScheduler();
        using var rp = ReactiveProperty<int>.Create(0, scheduler, false, false);
        var values = new List<int>();
        rp.Subscribe(x => values.Add(x));

        // Value should not be received until scheduler advances
        await Assert.That(values).IsEmpty();

        scheduler.Start();
        await Assert.That(values).Contains(0);
    }

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

        rp.Value = 42;

        using (Assert.Multiple())
        {
            await Assert.That(fired).IsTrue();
            await Assert.That(propertyName).IsEqualTo(nameof(ReactiveProperty<int>.Value));
        }
    }

    [Test]
    public async Task MultipleSubscribersReceiveUpdates()
    {
        using var rp = ReactiveProperty<int>.Create(0, ImmediateScheduler.Instance, false, false);
        var values1 = new List<int>();
        var values2 = new List<int>();

        rp.Subscribe(x => values1.Add(x));

        rp.Value = 1;

        rp.Subscribe(x => values2.Add(x));

        rp.Value = 2;

        await Assert.That(values1).Contains(0);
        await Assert.That(values1).Contains(1);
        await Assert.That(values1).Contains(2);

        await Assert.That(values2).Contains(1); // Gets current value on subscribe
        await Assert.That(values2).Contains(2);
    }
}
