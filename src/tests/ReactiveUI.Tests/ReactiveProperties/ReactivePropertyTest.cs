// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections;
using ReactiveUI.Tests.ReactiveProperties.Mocks;

namespace ReactiveUI.Tests.ReactiveProperties;

public class ReactivePropertyTest
{
    [Test]
    public async Task CheckValidation()
    {
        var minValue = 0;
        using var rp = new ReactiveProperty<int>(0, ImmediateScheduler.Instance, false, false)
            .AddValidationError(x => x < minValue ? "Error" : null);
        await Assert.That(rp.GetErrors("Value") == null).IsTrue();

        minValue = 1;
        await Assert.That(rp.GetErrors("Value") == null).IsTrue();

        rp.CheckValidation();
        await Assert.That(rp.GetErrors("Value")?.OfType<string>()).IsEquivalentTo(["Error"]);
    }

    [Test]
    public async Task DefaultValueIsRaisedOnSubscribe()
    {
        using var rp = new ReactiveProperty<string>();
        await Assert.That(rp.Value).IsNull();
        rp.Subscribe(async x => await Assert.That(x).IsNull());
    }

    [Test]
    public async Task ErrorsChanged_EventIsRaised()
    {
        using var rp = new ReactiveProperty<string>(default, ImmediateScheduler.Instance, false, false)
            .AddValidationError(x => string.IsNullOrEmpty(x) ? "error" : null);

        DataErrorsChangedEventArgs? eventArgs = null;
        rp.ErrorsChanged += (sender, e) => eventArgs = e;

        rp.Value = "valid";
        await Task.Delay(10);

        await Assert.That(eventArgs).IsNotNull();
    }

    [Test]
    public async Task IgnoreInitErrorAndUpdateValue()
    {
        using var rp = new ReactiveProperty<string>(default, ImmediateScheduler.Instance, false, false)
            .AddValidationError(x => string.IsNullOrEmpty(x) ? "error" : null, true);

        await Assert.That(rp.HasErrors).IsFalse();
        rp.Value = string.Empty;
        await Assert.That(rp.HasErrors).IsTrue();
    }

    [Test]
    public async Task IgnoreInitialErrorAndCheckValidation()
    {
        using var rp = new ReactiveProperty<string>(default, ImmediateScheduler.Instance, false, false)
            .AddValidationError(x => string.IsNullOrEmpty(x) ? "error" : null, true);

        await Assert.That(rp.HasErrors).IsFalse();
        rp.CheckValidation();
        await Assert.That(rp.HasErrors).IsTrue();
    }

    [Test]
    public async Task InitialValue()
    {
        using var rp = new ReactiveProperty<string>("ReactiveUI");
        await Assert.That(rp.Value).IsEqualTo("ReactiveUI");
        rp.Subscribe(async x => await Assert.That(x).IsEqualTo("ReactiveUI"));
    }

    [Test]
    public async Task InitialValueSkipCurrent()
    {
        using var rp = new ReactiveProperty<string>("ReactiveUI", true, false);
        await Assert.That(rp.Value).IsEqualTo("ReactiveUI");

        // current value should be skipped
        rp.Subscribe(async x => await Assert.That(x).IsEqualTo("ReactiveUI 2"));
        rp.Value = "ReactiveUI 2";
        await Assert.That(rp.Value).IsEqualTo("ReactiveUI 2");
    }

    [Test]
    public async Task MultipleSubscribersGetCurrentValue()
    {
        using var rp = new ReactiveProperty<int>(0, ImmediateScheduler.Instance, false, false);
        var collector1 = new List<int>();
        var collector2 = new List<int>();
        var obs = rp;
        obs.Subscribe(x => collector1.Add(x));

        await Assert.That(rp.Value).IsEqualTo(0);
        await Assert.That(collector1).IsEquivalentTo([0]);

        rp.Value = 1;
        await Assert.That(rp.Value).IsEqualTo(1);
        await Assert.That(collector1).IsEquivalentTo([0, 1]);

        rp.Value = 2;
        await Assert.That(rp.Value).IsEqualTo(2);
        await Assert.That(collector1).IsEquivalentTo([0, 1, 2]);

        // second subscriber
        obs.Subscribe(x => collector2.Add(x));
        await Assert.That(rp.Value).IsEqualTo(2);
        await Assert.That(collector2).IsEquivalentTo([2]);

        rp.Value = 3;
        await Assert.That(rp.Value).IsEqualTo(3);
        await Assert.That(collector1).IsEquivalentTo([0, 1, 2, 3]);
        await Assert.That(collector2).IsEquivalentTo([2, 3]);
    }

    [Test]
    public async Task ObserveErrors()
    {
        using var rp = new ReactiveProperty<string>(default, ImmediateScheduler.Instance, false, false)
            .AddValidationError(x => x == null ? "Error" : null);

        var results = new List<IEnumerable?>();
        rp.ObserveErrorChanged.Subscribe(results.Add);
        rp.Value = "OK";

        await Assert.That(results.Count).IsEqualTo(2);
        await Assert.That(results[0]?.OfType<string>()).IsEquivalentTo(["Error"]);
        await Assert.That(results[1] == null).IsTrue();
    }

    [Test]
    public async Task ObserveHasError()
    {
        using var rp = new ReactiveProperty<string>(default, ImmediateScheduler.Instance, false, false)
            .AddValidationError(x => x == null ? "Error" : null);

        var results = new List<bool>();
        rp.ObserveHasErrors.Subscribe(x => results.Add(x));
        rp.Value = "OK";

        await Assert.That(results.Count).IsEqualTo(2);
        await Assert.That(results[0]).IsTrue();
        await Assert.That(results[1]).IsFalse();
    }

    [Test]
    public async Task ObserveValidationErrors_HandlesMultipleErrors()
    {
        var target = new ReactivePropertyVM();
        var errors = new List<string?>();
        target.LengthLessThanFiveProperty
            .ObserveValidationErrors()
            .Subscribe(x => errors.Add(x));

        await Assert.That(errors).Count().IsEqualTo(1);
        await Assert.That(errors[0]).IsEqualTo("required");

        target.LengthLessThanFiveProperty.Value = "ok";
        await Assert.That(errors.Last()).IsNull();

        target.LengthLessThanFiveProperty.Value = "toolong";
        await Assert.That(errors.Last()).IsEqualTo("5over");
    }

    [Test]
    public async Task ObserveValidationErrors_ReturnsErrorMessages()
    {
        var target = new ReactivePropertyVM();
        var errors = new List<string?>();
        target.IsRequiredProperty
            .ObserveValidationErrors()
            .Subscribe(x => errors.Add(x));

        await Assert.That(errors).Count().IsEqualTo(1);
        await Assert.That(errors[0]).IsEqualTo("error!");

        target.IsRequiredProperty.Value = "valid";
        await Assert.That(errors).Count().IsEqualTo(2);
        await Assert.That(errors[1]).IsNull();

        target.IsRequiredProperty.Value = null;
        await Assert.That(errors).Count().IsEqualTo(3);
        await Assert.That(errors[2]).IsEqualTo("error!");
    }

    [Test]
    public async Task ObserveValidationErrors_ThrowsOnNull()
    {
        ReactiveProperty<string>? nullProperty = null;
        await Assert.That(() => nullProperty!.ObserveValidationErrors())
            .Throws<ArgumentNullException>();
    }

    [Test]
    public async Task Refresh()
    {
        using var rp = new ReactiveProperty<int>(0, ImmediateScheduler.Instance, false, false);
        var collector = new List<int>();
        rp.Subscribe(x => collector.Add(x));

        await Assert.That(collector).IsEquivalentTo([0]);

        // refresh should always produce a value even if it is the same and duplicates are not allowed
        rp.Refresh();
        await Assert.That(collector).IsEquivalentTo([0, 0]);
    }

    [Test]
    public async Task SetValueRaisesEvents()
    {
        using var rp = new ReactiveProperty<string>();
        await Assert.That(rp.Value).IsNull();
        rp.Value = "ReactiveUI";
        await Assert.That(rp.Value).IsEqualTo("ReactiveUI");
        rp.Subscribe(async x => await Assert.That(x).IsEqualTo("ReactiveUI"));
    }

    [Test]
    public async Task Subscribe_WithNullObserver_ReturnsEmptyDisposable()
    {
        using var rp = new ReactiveProperty<string>("test");
        var result = rp.Subscribe(null!);

        await Assert.That(result).IsNotNull();
    }

    [Test]
    public async Task TestMultipleSubstribers()
    {
        using var vm = new SubcribeTestViewModel(1000);
        await Assert.That(vm.SubscriberCount).IsEqualTo(1000);
        await Assert.That(vm.StartupTime).IsLessThan(2000);
        await Assert.That(vm.SubscriberEvents).IsEqualTo(1000);
    }

    [Test]
    public async Task ValidationErrorChangedTest()
    {
        var errors = new List<IEnumerable?>();
        using var rprop = new ReactiveProperty<string>(default, ImmediateScheduler.Instance, false, false)
            .AddValidationError(x => string.IsNullOrWhiteSpace(x) ? "error" : null);

        // old version behavior
        rprop.ObserveErrorChanged.Skip(1).Subscribe(errors.Add);

        await Assert.That(errors.Count).IsEqualTo(0);

        rprop.Value = "OK";
        await Assert.That(errors.Count).IsEqualTo(1);
        await Assert.That(errors.Last() == null).IsTrue();

        rprop.Value = null;
        await Assert.That(errors.Count).IsEqualTo(2);
        await Assert.That(errors.Last()?.OfType<string>()).IsEquivalentTo(["error"]);
    }

    [Test]
    public async Task ValidationIgnoreInitialErrorAndRefresh()
    {
        using var rp = new ReactiveProperty<string>(default, ImmediateScheduler.Instance, false, false)
            .AddValidationError(x => string.IsNullOrEmpty(x) ? "error" : null, true);

        await Assert.That(rp.HasErrors).IsFalse();
        rp.Refresh();
        await Assert.That(rp.HasErrors).IsTrue();
    }

    [Test]
    public async Task ValidationIsRequiredIsCorrectlyHandled()
    {
        var target = new ReactivePropertyVM();
        var errors = new List<IEnumerable?>();
        target.IsRequiredProperty
            .ObserveErrorChanged
            .Where(x => x != null)
            .Subscribe(errors.Add);

        await Assert.That(errors.Count).IsEqualTo(1);
        await Assert.That(errors[0]!.OfType<string>()).IsEquivalentTo(["error!"]);
        await Assert.That(target.IsRequiredProperty.HasErrors).IsTrue();

        target.IsRequiredProperty.Value = "a";
        await Assert.That(errors.Count).IsEqualTo(1);
        await Assert.That(target.IsRequiredProperty.HasErrors).IsFalse();

        target.IsRequiredProperty.Value = null;
        await Assert.That(errors.Count).IsEqualTo(2);
        await Assert.That(errors[1]!.OfType<string>()).IsEquivalentTo(["error!"]);
        await Assert.That(target.IsRequiredProperty.HasErrors).IsTrue();
    }

    [Test]
    public async Task ValidationLengthIsCorrectlyHandled()
    {
        var target = new ReactivePropertyVM();
        IEnumerable? error = null;
        target.LengthLessThanFiveProperty
            .ObserveErrorChanged
            .ObserveOn(ImmediateScheduler.Instance)
            .Subscribe(x => error = x);

        await Assert.That(target.LengthLessThanFiveProperty.HasErrors).IsTrue();
        await Assert.That(error!.OfType<string>().First()).IsEqualTo("required");

        target.LengthLessThanFiveProperty.Value = "a";
        await Assert.That(target.LengthLessThanFiveProperty.HasErrors).IsFalse();
        await Assert.That(error == null).IsTrue();

        target.LengthLessThanFiveProperty.Value = "aaaaaa";
        await Assert.That(target.LengthLessThanFiveProperty.HasErrors).IsTrue();
        await Assert.That(error).IsNotNull();
        await Assert.That(error!.OfType<string>().First()).IsEqualTo("5over");

        target.LengthLessThanFiveProperty.Value = null;
        await Assert.That(target.LengthLessThanFiveProperty.HasErrors).IsTrue();
        await Assert.That(error!.OfType<string>().First()).IsEqualTo("required");
    }

    [Test]
    public async Task ValidationTaskTest()
    {
        var target = new ReactivePropertyVM();
        var errors = new List<IEnumerable?>();
        target.TaskValidationTestProperty
            .ObserveErrorChanged
            .Where(x => x != null)
            .Subscribe(errors.Add);
        await Assert.That(errors.Count).IsEqualTo(1);
        await Assert.That(errors[0]!.OfType<string>()).IsEquivalentTo(["required"]);

        target.TaskValidationTestProperty.Value = "a";
        await Assert.That(target.TaskValidationTestProperty.HasErrors).IsFalse();
        await Assert.That(errors.Count).IsEqualTo(1);

        target.TaskValidationTestProperty.Value = null;
        await Assert.That(target.TaskValidationTestProperty.HasErrors).IsTrue();
        await Assert.That(errors.Count).IsEqualTo(2);
    }

    [Test]
    public async Task ValidationWithAsyncFailedCase()
    {
        var errorMessage = "error occured!!";
        using var rp = new ReactiveProperty<string>(default, ImmediateScheduler.Instance, false, false)
            .AddValidationError(x => string.IsNullOrEmpty(x) ? null : errorMessage);

        IEnumerable? error = null;
        rp.ObserveErrorChanged
            .Subscribe(x => error = x);

        await Assert.That(rp.HasErrors).IsFalse();
        await Assert.That(error == null).IsTrue();

        rp.Value = "dummy"; //--- push value to trigger validation error

        await Assert.That(rp.HasErrors).IsTrue();
        await Assert.That(error).IsNotNull();
        await Assert.That(error!.OfType<string>()).IsEquivalentTo([errorMessage]);
        await Assert.That(rp.GetErrors("Value")!.OfType<string>()).IsEquivalentTo([errorMessage]);
    }

    [Test]
    public async Task ValidationWithAsyncSuccessCase()
    {
        var tcs = new TaskCompletionSource<string?>();
        using var rp = new ReactiveProperty<string>(default, ImmediateScheduler.Instance, false, false)
            .AddValidationError(_ => tcs.Task);

        IEnumerable? error = null;
        rp.ObserveErrorChanged.Subscribe(x => error = x);

        await Assert.That(rp.HasErrors).IsFalse();
        await Assert.That(error == null).IsTrue();

        rp.Value = "dummy";
        tcs.SetResult(null);
        await Task.Yield();

        await Assert.That(rp.HasErrors).IsFalse();
        await Assert.That(error == null).IsTrue();
    }

    [Test]

    [TestExecutor<WithVirtualTimeSchedulerExecutor>]
    public async Task ValidationWithAsyncThrottleTest()
    {
        var scheduler = TestContext.Current.GetVirtualTimeScheduler();
        using var rp = new ReactiveProperty<string>(default, scheduler, false, false)
            .AddValidationError(xs => xs
                .Throttle(TimeSpan.FromSeconds(1), scheduler)
                .Select(x => string.IsNullOrEmpty(x) ? "required" : null));

        IEnumerable? error = null;
        rp.ObserveErrorChanged.Subscribe(x => error = x);

        scheduler.AdvanceTo(DateTimeOffset.MinValue.Add(TimeSpan.FromMilliseconds(0)));
        rp.Value = string.Empty;
        await Assert.That(rp.HasErrors).IsFalse();
        await Assert.That(error == null).IsTrue();

        scheduler.AdvanceTo(DateTimeOffset.MinValue.Add(TimeSpan.FromMilliseconds(300)));
        rp.Value = "a";
        await Assert.That(rp.HasErrors).IsFalse();
        await Assert.That(error == null).IsTrue();

        scheduler.AdvanceTo(DateTimeOffset.MinValue.Add(TimeSpan.FromMilliseconds(700)));
        rp.Value = "b";
        await Assert.That(rp.HasErrors).IsFalse();
        await Assert.That(error == null).IsTrue();

        scheduler.AdvanceTo(DateTimeOffset.MinValue.Add(TimeSpan.FromMilliseconds(1100)));
        rp.Value = string.Empty;
        await Assert.That(rp.HasErrors).IsFalse();
        await Assert.That(error == null).IsTrue();

        scheduler.AdvanceTo(DateTimeOffset.MinValue.Add(TimeSpan.FromMilliseconds(2500)));
        await Assert.That(rp.HasErrors).IsTrue();
        await Assert.That(error).IsNotNull();
        await Assert.That(error!.OfType<string>()).IsEquivalentTo(["required"]);
    }

    [Test]
    public async Task ValidationWithCustomErrorMessage()
    {
        var target = new ReactivePropertyVM();
        target.CustomValidationErrorMessageProperty.Value = string.Empty;
        var errorMessage = target?
            .CustomValidationErrorMessageProperty?
            .GetErrors(nameof(ReactivePropertyVM.CustomValidationErrorMessageProperty))!
            .Cast<string>()
            .First();

        await Assert.That(errorMessage)
            .IsEqualTo("Custom validation error message for CustomValidationErrorMessageProperty");
    }

    [Test]
    public async Task ValidationWithCustomErrorMessageWithDisplayName()
    {
        var target = new ReactivePropertyVM();
        target.CustomValidationErrorMessageWithDisplayNameProperty.Value = string.Empty;
        var errorMessage = target
            .CustomValidationErrorMessageWithDisplayNameProperty?
            .GetErrors(nameof(ReactivePropertyVM.CustomValidationErrorMessageWithDisplayNameProperty))!
            .Cast<string>()
            .First();

        await Assert.That(errorMessage).IsEqualTo("Custom validation error message for CustomName");
    }

    [Test]
    public async Task ValidationWithCustomErrorMessageWithResource()
    {
        var target = new ReactivePropertyVM();
        target.CustomValidationErrorMessageWithResourceProperty.Value = string.Empty;
        var errorMessage = target
            .CustomValidationErrorMessageWithResourceProperty?
            .GetErrors(nameof(ReactivePropertyVM.CustomValidationErrorMessageWithResourceProperty))!
            .Cast<string>()
            .First();

        await Assert.That(errorMessage).IsEqualTo("Oops!? FromResource is required.");
    }

    [Test]
    public async Task ValueUpdatesMultipleTimesWithDifferentValues()
    {
        using var rp = new ReactiveProperty<int>(0, ImmediateScheduler.Instance, false, false);
        var collector = new List<int>();
        rp.Subscribe(x => collector.Add(x));

        await Assert.That(rp.Value).IsEqualTo(0);
        await Assert.That(collector).IsEquivalentTo([0]);

        rp.Value = 1;
        await Assert.That(rp.Value).IsEqualTo(1);
        await Assert.That(collector).IsEquivalentTo([0, 1]);

        rp.Value = 2;
        await Assert.That(rp.Value).IsEqualTo(2);
        await Assert.That(collector).IsEquivalentTo([0, 1, 2]);

        rp.Value = 3;
        await Assert.That(rp.Value).IsEqualTo(3);
        await Assert.That(collector).IsEquivalentTo([0, 1, 2, 3]);
    }

    [Test]
    public async Task ValueUpdatesMultipleTimesWithSameValues()
    {
        using var rp = new ReactiveProperty<int>(0, ImmediateScheduler.Instance, false, true);
        var collector = new List<int>();
        rp.Subscribe(x => collector.Add(x));

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
