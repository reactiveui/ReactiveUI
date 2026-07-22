// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections;
using System.ComponentModel;
using ReactiveUI.Tests.ReactiveProperties.Mocks;
using ReactiveUI.Tests.Utilities.Schedulers;
using TUnit.Core.Executors;

namespace ReactiveUI.Tests.ReactiveProperties;

/// <summary>Tests for <see cref="ReactiveProperty{T}"/> covering value, subscription and validation behaviour.</summary>
public class ReactivePropertyTest
{
    /// <summary>The name of the Value property.</summary>
    private const string ValuePropertyName = "Value";

    /// <summary>The capitalized error text used in validation tests.</summary>
    private const string ErrorValue = "Error";

    /// <summary>The lower-case error text used in validation tests.</summary>
    private const string ErrorLowerValue = "error";

    /// <summary>The first sample value used in subscription tests.</summary>
    private const string ReactiveUiValue = "ReactiveUI";

    /// <summary>The second sample value used in subscription tests.</summary>
    private const string ReactiveUiSecondValue = "ReactiveUI 2";

    /// <summary>The required-value validation error message.</summary>
    private const string RequiredErrorValue = "required";

    /// <summary>The error text with an exclamation used in validation tests.</summary>
    private const string ExclaimErrorValue = "error!";

    /// <summary>Verifies that explicitly checking validation surfaces an error that was not yet evaluated.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task CheckValidation()
    {
        var minValue = 0;
        using var rp = new ReactiveProperty<int>(0, Sequencer.Immediate, false, false)
            .AddValidationError(x => x < minValue ? ErrorValue : null);
        await Assert.That(rp.GetErrors(ValuePropertyName) is null).IsTrue();

        minValue = 1;
        await Assert.That(rp.GetErrors(ValuePropertyName) is null).IsTrue();

        rp.CheckValidation();
        await Assert.That(rp.GetErrors(ValuePropertyName)?.OfType<string>()).IsEquivalentTo([ErrorValue]);
    }

    /// <summary>Verifies that the default value is raised to subscribers on subscription.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task DefaultValueIsRaisedOnSubscribe()
    {
        using var rp = new ReactiveProperty<string>(null, Sequencer.Immediate, false, false);
        await Assert.That(rp.Value).IsNull();
        var receivedValue = false;
        _ = rp.Subscribe(x => receivedValue = true);

        await Assert.That(receivedValue).IsTrue();
    }

    /// <summary>Verifies the errors changed event is raised when the value changes.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task ErrorsChanged_EventIsRaised()
    {
        using var rp = new ReactiveProperty<string>(null, Sequencer.Immediate, false, false)
            .AddValidationError(static x => string.IsNullOrEmpty(x) ? ErrorLowerValue : null);

        DataErrorsChangedEventArgs? eventArgs = null;
        rp.ErrorsChanged += (_, e) => eventArgs = e;

        const int ErrorPropagationDelayMs = 10;
        rp.Value = "valid";
        await Task.Delay(ErrorPropagationDelayMs);

        await Assert.That(eventArgs).IsNotNull();
    }

    /// <summary>Verifies the initial error is ignored but a later value update produces an error.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task IgnoreInitErrorAndUpdateValue()
    {
        using var rp = new ReactiveProperty<string>(null, Sequencer.Immediate, false, false)
            .AddValidationError(static x => string.IsNullOrEmpty(x) ? ErrorLowerValue : null, true);

        await Assert.That(rp.HasErrors).IsFalse();
        rp.Value = string.Empty;
        await Assert.That(rp.HasErrors).IsTrue();
    }

    /// <summary>Verifies the initial error is ignored but invoking validation surfaces the error.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task IgnoreInitialErrorAndCheckValidation()
    {
        using var rp = new ReactiveProperty<string>(null, Sequencer.Immediate, false, false)
            .AddValidationError(static x => string.IsNullOrEmpty(x) ? ErrorLowerValue : null, true);

        await Assert.That(rp.HasErrors).IsFalse();
        rp.CheckValidation();
        await Assert.That(rp.HasErrors).IsTrue();
    }

    /// <summary>Verifies the initial value is stored and raised to subscribers.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task InitialValue()
    {
        using var rp = new ReactiveProperty<string>(ReactiveUiValue, Sequencer.Immediate, false, false);
        await Assert.That(rp.Value).IsEqualTo(ReactiveUiValue);
        string? received = null;
        _ = rp.Subscribe(x => received = x);
        await Assert.That(received).IsEqualTo(ReactiveUiValue);
    }

    /// <summary>Verifies that the current value is skipped on subscribe when skip-current is enabled.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task InitialValueSkipCurrent()
    {
        using var rp = new ReactiveProperty<string>(ReactiveUiValue, Sequencer.Immediate, true, false);
        await Assert.That(rp.Value).IsEqualTo(ReactiveUiValue);

        // current value should be skipped
        string? received = null;
        _ = rp.Subscribe(x => received = x);
        rp.Value = ReactiveUiSecondValue;
        await Assert.That(received).IsEqualTo(ReactiveUiSecondValue);
        await Assert.That(rp.Value).IsEqualTo(ReactiveUiSecondValue);
    }

    /// <summary>Verifies that multiple subscribers each receive the current value and subsequent changes.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task MultipleSubscribersGetCurrentValue()
    {
        const int ThirdValue = 2;
        const int FourthValue = 3;
        using var rp = new ReactiveProperty<int>(0, Sequencer.Immediate, false, false);
        var collector1 = new List<int>();
        var collector2 = new List<int>();
        var obs = rp;
        _ = obs.Subscribe(collector1.Add);

        await Assert.That(rp.Value).IsEqualTo(0);
        await Assert.That(collector1).IsEquivalentTo([0]);

        rp.Value = 1;
        await Assert.That(rp.Value).IsEqualTo(1);
        await Assert.That(collector1).IsEquivalentTo([0, 1]);

        rp.Value = ThirdValue;
        await Assert.That(rp.Value).IsEqualTo(ThirdValue);
        await Assert.That(collector1).IsEquivalentTo([0, 1, ThirdValue]);

        // second subscriber
        _ = obs.Subscribe(collector2.Add);
        await Assert.That(rp.Value).IsEqualTo(ThirdValue);
        await Assert.That(collector2).IsEquivalentTo([ThirdValue]);

        rp.Value = FourthValue;
        await Assert.That(rp.Value).IsEqualTo(FourthValue);
        await Assert.That(collector1).IsEquivalentTo([0, 1, ThirdValue, FourthValue]);
        await Assert.That(collector2).IsEquivalentTo([ThirdValue, FourthValue]);
    }

    /// <summary>Verifies that the observable error stream emits the error and the cleared state.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task ObserveErrors()
    {
        using var rp = new ReactiveProperty<string>(null, Sequencer.Immediate, false, false)
            .AddValidationError(static x => x is null ? ErrorValue : null);

        const int ExpectedEmissionCount = 2;
        var results = new List<IEnumerable?>();
        _ = rp.ObserveErrorChanged.Subscribe(results.Add);
        rp.Value = "OK";

        await Assert.That(results.Count).IsEqualTo(ExpectedEmissionCount);
        await Assert.That(results[0]?.OfType<string>()).IsEquivalentTo([ErrorValue]);
        await Assert.That(results[1] is null).IsTrue();
    }

    /// <summary>Verifies that the observable has-errors stream emits the true and false states.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task ObserveHasError()
    {
        using var rp = new ReactiveProperty<string>(null, Sequencer.Immediate, false, false)
            .AddValidationError(static x => x is null ? ErrorValue : null);

        const int ExpectedEmissionCount = 2;
        var results = new List<bool>();
        _ = rp.ObserveHasErrors.Subscribe(results.Add);
        rp.Value = "OK";

        await Assert.That(results.Count).IsEqualTo(ExpectedEmissionCount);
        await Assert.That(results[0]).IsTrue();
        await Assert.That(results[1]).IsFalse();
    }

    /// <summary>Verifies that observing validation errors reports each distinct error message as the value changes.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task ObserveValidationErrors_HandlesMultipleErrors()
    {
        var target = new ReactivePropertyVm();
        var errors = new List<string?>();
        _ = target.LengthLessThanFiveProperty.ObserveValidationErrors().Subscribe(errors.Add);

        await Assert.That(errors).Count().IsEqualTo(1);
        await Assert.That(errors[0]).IsEqualTo(RequiredErrorValue);

        target.LengthLessThanFiveProperty.Value = "ok";
        await Assert.That(errors[^1]).IsNull();

        target.LengthLessThanFiveProperty.Value = "toolong";
        await Assert.That(errors[^1]).IsEqualTo("5over");
    }

    /// <summary>Verifies that observing validation errors returns and clears the required error message.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task ObserveValidationErrors_ReturnsErrorMessages()
    {
        var target = new ReactivePropertyVm();
        var errors = new List<string?>();
        _ = target.IsRequiredProperty.ObserveValidationErrors().Subscribe(errors.Add);

        const int AfterValidCount = 2;
        const int AfterClearedCount = 3;
        await Assert.That(errors).Count().IsEqualTo(1);
        await Assert.That(errors[0]).IsEqualTo(ExclaimErrorValue);

        target.IsRequiredProperty.Value = "valid";
        await Assert.That(errors).Count().IsEqualTo(AfterValidCount);
        await Assert.That(errors[1]).IsNull();

        target.IsRequiredProperty.Value = null;
        await Assert.That(errors).Count().IsEqualTo(AfterClearedCount);
        await Assert.That(errors[^1]).IsEqualTo(ExclaimErrorValue);
    }

    /// <summary>Verifies that observing validation errors throws when the property is null.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task ObserveValidationErrors_ThrowsOnNull()
    {
        ReactiveProperty<string>? nullProperty = null;
        await Assert.That(() => nullProperty!.ObserveValidationErrors())
            .Throws<ArgumentNullException>();
    }

    /// <summary>Verifies that refreshing re-emits the current value even when duplicates are not allowed.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task Refresh()
    {
        using var rp = new ReactiveProperty<int>(0, Sequencer.Immediate, false, false);
        var collector = new List<int>();
        _ = rp.Subscribe(collector.Add);

        await Assert.That(collector).IsEquivalentTo([0]);

        // refresh should always produce a value even if it is the same and duplicates are not allowed
        rp.Refresh();
        await Assert.That(collector).IsEquivalentTo([0, 0]);
    }

    /// <summary>Verifies that setting the value updates it and raises it to later subscribers.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task SetValueRaisesEvents()
    {
        using var rp = new ReactiveProperty<string>(null, Sequencer.Immediate, false, false);
        await Assert.That(rp.Value).IsNull();
        rp.Value = ReactiveUiValue;
        await Assert.That(rp.Value).IsEqualTo(ReactiveUiValue);
        string? received = null;
        _ = rp.Subscribe(x => received = x);
        await Assert.That(received).IsEqualTo(ReactiveUiValue);
    }

    /// <summary>Verifies that subscribing with a null observer returns a non-null disposable.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task Subscribe_WithNullObserver_ReturnsEmptyDisposable()
    {
        using var rp = new ReactiveProperty<string>("test");
        var result = rp.Subscribe(null!);

        await Assert.That(result).IsNotNull();
    }

    /// <summary>Verifies that a large number of subscribers all receive events within an acceptable startup time.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task TestMultipleSubstribers()
    {
        const int SubscriberCount = 1_000;
        const int MaxStartupTimeMs = 2_000;
        using var vm = new SubcribeTestViewModel(SubscriberCount);
        await Assert.That(vm.SubscriberCount).IsEqualTo(SubscriberCount);
        await Assert.That(vm.StartupTime).IsLessThan(MaxStartupTimeMs);
        await Assert.That(vm.SubscriberEvents).IsEqualTo(SubscriberCount);
    }

    /// <summary>Verifies the error changed stream emits as the value transitions between valid and invalid.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task ValidationErrorChangedTest()
    {
        var errors = new List<IEnumerable?>();
        using var rprop = new ReactiveProperty<string>(null, Sequencer.Immediate, false, false)
            .AddValidationError(static x => string.IsNullOrWhiteSpace(x) ? ErrorLowerValue : null);

        // old version behavior
        _ = rprop.ObserveErrorChanged.Skip(1).Subscribe(errors.Add);

        await Assert.That(errors.Count).IsEqualTo(0);

        rprop.Value = "OK";
        await Assert.That(errors.Count).IsEqualTo(1);
        await Assert.That(errors[^1] is null).IsTrue();

        const int AfterRevalidationCount = 2;
        rprop.Value = null;
        await Assert.That(errors.Count).IsEqualTo(AfterRevalidationCount);
        await Assert.That(errors[^1]?.OfType<string>()).IsEquivalentTo([ErrorLowerValue]);
    }

    /// <summary>Verifies the initial error is ignored but refreshing surfaces the error.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task ValidationIgnoreInitialErrorAndRefresh()
    {
        using var rp = new ReactiveProperty<string>(null, Sequencer.Immediate, false, false)
            .AddValidationError(static x => string.IsNullOrEmpty(x) ? ErrorLowerValue : null, true);

        await Assert.That(rp.HasErrors).IsFalse();
        rp.Refresh();
        await Assert.That(rp.HasErrors).IsTrue();
    }

    /// <summary>Verifies the required validation attribute reports and clears errors as the value changes.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task ValidationIsRequiredIsCorrectlyHandled()
    {
        var target = new ReactivePropertyVm();
        var errors = new List<IEnumerable?>();
        _ = target.IsRequiredProperty
                    .ObserveErrorChanged.Where(static x => x is not null).Subscribe(errors.Add);

        await Assert.That(errors.Count).IsEqualTo(1);
        await Assert.That(errors[0]!.OfType<string>()).IsEquivalentTo([ExclaimErrorValue]);
        await Assert.That(target.IsRequiredProperty.HasErrors).IsTrue();

        target.IsRequiredProperty.Value = "a";
        await Assert.That(errors.Count).IsEqualTo(1);
        await Assert.That(target.IsRequiredProperty.HasErrors).IsFalse();

        const int AfterRevalidationErrorCount = 2;
        target.IsRequiredProperty.Value = null;
        await Assert.That(errors.Count).IsEqualTo(AfterRevalidationErrorCount);
        await Assert.That(errors[1]!.OfType<string>()).IsEquivalentTo([ExclaimErrorValue]);
        await Assert.That(target.IsRequiredProperty.HasErrors).IsTrue();
    }

    /// <summary>Verifies the string length validation attribute reports and clears errors as the value changes.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task ValidationLengthIsCorrectlyHandled()
    {
        var target = new ReactivePropertyVm();
        IEnumerable? error = null;
        _ = target.LengthLessThanFiveProperty
                    .ObserveErrorChanged.Subscribe(x => error = x);

        await Assert.That(target.LengthLessThanFiveProperty.HasErrors).IsTrue();
        await Assert.That(error!.OfType<string>().First()).IsEqualTo(RequiredErrorValue);

        target.LengthLessThanFiveProperty.Value = "a";
        await Assert.That(target.LengthLessThanFiveProperty.HasErrors).IsFalse();
        await Assert.That(error is null).IsTrue();

        target.LengthLessThanFiveProperty.Value = "aaaaaa";
        await Assert.That(target.LengthLessThanFiveProperty.HasErrors).IsTrue();
        await Assert.That(error).IsNotNull();
        await Assert.That(error!.OfType<string>().First()).IsEqualTo("5over");

        target.LengthLessThanFiveProperty.Value = null;
        await Assert.That(target.LengthLessThanFiveProperty.HasErrors).IsTrue();
        await Assert.That(error!.OfType<string>().First()).IsEqualTo(RequiredErrorValue);
    }

    /// <summary>Verifies that task-based validation reports and clears errors as the value changes.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task ValidationTaskTest()
    {
        var target = new ReactivePropertyVm();
        var errors = new List<IEnumerable?>();
        _ = target.TaskValidationTestProperty
                    .ObserveErrorChanged.Where(static x => x is not null).Subscribe(errors.Add);
        await Assert.That(errors.Count).IsEqualTo(1);
        await Assert.That(errors[0]!.OfType<string>()).IsEquivalentTo([RequiredErrorValue]);

        target.TaskValidationTestProperty.Value = "a";
        await Assert.That(target.TaskValidationTestProperty.HasErrors).IsFalse();
        await Assert.That(errors.Count).IsEqualTo(1);

        const int AfterRevalidationErrorCount = 2;
        target.TaskValidationTestProperty.Value = null;
        await Assert.That(target.TaskValidationTestProperty.HasErrors).IsTrue();
        await Assert.That(errors.Count).IsEqualTo(AfterRevalidationErrorCount);
    }

    /// <summary>Verifies that asynchronous validation reports an error when the value is invalid.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task ValidationWithAsyncFailedCase()
    {
        const string ErrorMessage = "error occured!!";
        using var rp = new ReactiveProperty<string>(null, Sequencer.Immediate, false, false)
            .AddValidationError(static x => string.IsNullOrEmpty(x) ? null : ErrorMessage);

        IEnumerable? error = null;
        _ = rp.ObserveErrorChanged.Subscribe(x => error = x);

        await Assert.That(rp.HasErrors).IsFalse();
        await Assert.That(error is null).IsTrue();

        rp.Value = "dummy"; // --- push value to trigger validation error

        await Assert.That(rp.HasErrors).IsTrue();
        await Assert.That(error).IsNotNull();
        await Assert.That(error!.OfType<string>()).IsEquivalentTo([ErrorMessage]);
        await Assert.That(rp.GetErrors(ValuePropertyName)!.OfType<string>()).IsEquivalentTo([ErrorMessage]);
    }

    /// <summary>Verifies that asynchronous validation reports no error when the value is valid.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task ValidationWithAsyncSuccessCase()
    {
        var tcs = new TaskCompletionSource<string?>(TaskCreationOptions.RunContinuationsAsynchronously);
        using var rp = new ReactiveProperty<string>(null, Sequencer.Immediate, false, false)
            .AddValidationError(_ => tcs.Task);

        IEnumerable? error = null;
        _ = rp.ObserveErrorChanged.Subscribe(x => error = x);

        await Assert.That(rp.HasErrors).IsFalse();
        await Assert.That(error is null).IsTrue();

        rp.Value = "dummy";
        tcs.SetResult(null);
        await Task.Yield();

        await Assert.That(rp.HasErrors).IsFalse();
        await Assert.That(error is null).IsTrue();
    }

    /// <summary>Verifies that throttled asynchronous validation reports the error only after the throttle window elapses.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    [TestExecutor<WithVirtualTimeSchedulerExecutor>]
    public async Task ValidationWithAsyncThrottleTest()
    {
        const double FirstAdvanceMs = 300;
        const double SecondAdvanceMs = 700;
        const double ThirdAdvanceMs = 1_100;
        const double FinalAdvanceMs = 2_500;
        var scheduler = TestContext.Current.GetVirtualTimeScheduler();
        using var rp = new ReactiveProperty<string>(null, scheduler, false, false)
#if REACTIVE_SHIM
            .AddValidationError(xs => ReactiveUI.Primitives.Reactive.LinqExtensions
                .Calm(xs, TimeSpan.FromSeconds(1), scheduler)
                .Select(static x => string.IsNullOrEmpty(x) ? RequiredErrorValue : null));
#else
            .AddValidationError(xs => ReactiveUI.Primitives.LinqExtensions
                .Calm(xs, TimeSpan.FromSeconds(1), scheduler)
                .Select(static x => string.IsNullOrEmpty(x) ? RequiredErrorValue : null));
#endif

        IEnumerable? error = null;
        _ = rp.ObserveErrorChanged.Subscribe(x => error = x);

        scheduler.AdvanceTo(DateTimeOffset.MinValue.Add(TimeSpan.FromMilliseconds(0)));
        rp.Value = string.Empty;
        await Assert.That(rp.HasErrors).IsFalse();
        await Assert.That(error is null).IsTrue();

        scheduler.AdvanceTo(DateTimeOffset.MinValue.Add(TimeSpan.FromMilliseconds(FirstAdvanceMs)));
        rp.Value = "a";
        await Assert.That(rp.HasErrors).IsFalse();
        await Assert.That(error is null).IsTrue();

        scheduler.AdvanceTo(DateTimeOffset.MinValue.Add(TimeSpan.FromMilliseconds(SecondAdvanceMs)));
        rp.Value = "b";
        await Assert.That(rp.HasErrors).IsFalse();
        await Assert.That(error is null).IsTrue();

        scheduler.AdvanceTo(DateTimeOffset.MinValue.Add(TimeSpan.FromMilliseconds(ThirdAdvanceMs)));
        rp.Value = string.Empty;
        await Assert.That(rp.HasErrors).IsFalse();
        await Assert.That(error is null).IsTrue();

        scheduler.AdvanceTo(DateTimeOffset.MinValue.Add(TimeSpan.FromMilliseconds(FinalAdvanceMs)));
        await Assert.That(rp.HasErrors).IsTrue();
        await Assert.That(error).IsNotNull();
        await Assert.That(error!.OfType<string>()).IsEquivalentTo([RequiredErrorValue]);
    }

    /// <summary>Verifies that a custom validation error message is produced with the property name substituted.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task ValidationWithCustomErrorMessage()
    {
        var target = new ReactivePropertyVm { CustomValidationErrorMessageProperty = { Value = string.Empty } };
        var errorMessage = target
            .CustomValidationErrorMessageProperty?
            .GetErrors(nameof(ReactivePropertyVm.CustomValidationErrorMessageProperty))!
            .Cast<string>()
            .First();

        await Assert.That(errorMessage).IsEqualTo("Custom validation error message for CustomValidationErrorMessageProperty");
    }

    /// <summary>Verifies that a custom validation error message uses the configured display name.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task ValidationWithCustomErrorMessageWithDisplayName()
    {
        var target = new ReactivePropertyVm { CustomValidationErrorMessageWithDisplayNameProperty = { Value = string.Empty } };
        var errorMessage = target
            .CustomValidationErrorMessageWithDisplayNameProperty?
            .GetErrors(nameof(ReactivePropertyVm.CustomValidationErrorMessageWithDisplayNameProperty))!
            .Cast<string>()
            .First();

        await Assert.That(errorMessage).IsEqualTo("Custom validation error message for CustomName");
    }

    /// <summary>Verifies that a custom validation error message is sourced from a resource.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task ValidationWithCustomErrorMessageWithResource()
    {
        var target = new ReactivePropertyVm { CustomValidationErrorMessageWithResourceProperty = { Value = string.Empty } };
        var errorMessage = target
            .CustomValidationErrorMessageWithResourceProperty?
            .GetErrors(nameof(ReactivePropertyVm.CustomValidationErrorMessageWithResourceProperty))!
            .Cast<string>()
            .First();

        await Assert.That(errorMessage).IsEqualTo("Oops!? FromResource is required.");
    }

    /// <summary>Verifies that subscribers receive each distinct value when the value updates multiple times.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task ValueUpdatesMultipleTimesWithDifferentValues()
    {
        const int ThirdValue = 2;
        const int FourthValue = 3;
        using var rp = new ReactiveProperty<int>(0, Sequencer.Immediate, false, false);
        var collector = new List<int>();
        _ = rp.Subscribe(collector.Add);

        await Assert.That(rp.Value).IsEqualTo(0);
        await Assert.That(collector).IsEquivalentTo([0]);

        rp.Value = 1;
        await Assert.That(rp.Value).IsEqualTo(1);
        await Assert.That(collector).IsEquivalentTo([0, 1]);

        rp.Value = ThirdValue;
        await Assert.That(rp.Value).IsEqualTo(ThirdValue);
        await Assert.That(collector).IsEquivalentTo([0, 1, ThirdValue]);

        rp.Value = FourthValue;
        await Assert.That(rp.Value).IsEqualTo(FourthValue);
        await Assert.That(collector).IsEquivalentTo([0, 1, ThirdValue, FourthValue]);
    }

    /// <summary>Verifies that subscribers receive each emission when duplicate values are allowed.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task ValueUpdatesMultipleTimesWithSameValues()
    {
        using var rp = new ReactiveProperty<int>(0, Sequencer.Immediate, false, true);
        var collector = new List<int>();
        _ = rp.Subscribe(collector.Add);

        await Assert.That(rp.Value).IsEqualTo(0);
        await Assert.That(collector).IsEquivalentTo([0]);

        rp.Value = 0;
        await Assert.That(rp.Value).IsEqualTo(0);
        await Assert.That(collector).IsEquivalentTo([0, 0]);

        rp.Value = 0;
        await Assert.That(rp.Value).IsEqualTo(0);
        await Assert.That(collector).IsEquivalentTo([0, 0, 0]);

        rp.Value = 0;
        await Assert.That(rp.Value).IsEqualTo(0);
        await Assert.That(collector).IsEquivalentTo([0, 0, 0, 0]);
    }
}
