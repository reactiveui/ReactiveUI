// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections;
using FluentAssertions;
using Microsoft.Reactive.Testing;
using ReactiveUI.Testing;
using ReactiveUI.Tests.ReactiveProperty.Mocks;

namespace ReactiveUI.Tests.ReactiveProperty;

public class ReactivePropertyTest : ReactiveTest
{
    [Fact]
    public void DefaultValueIsRaisedOnSubscribe()
    {
        using var rp = new ReactiveProperty<string>();
        rp.Value.Should().BeNull();
        rp.Subscribe(Assert.Null);
    }

    [Fact]
    public void InitialValue()
    {
        using var rp = new ReactiveProperty<string>("ReactiveUI");
        Assert.Equal(rp.Value, "ReactiveUI");
        rp.Subscribe(x => Assert.Equal(x, "ReactiveUI"));
    }

    [Fact]
    public void InitialValueSkipCurrent()
    {
        using var rp = new ReactiveProperty<string>("ReactiveUI", true, false);
        Assert.Equal(rp.Value, "ReactiveUI");

        // current value should be skipped
        rp.Subscribe(x => Assert.Equal(x, "ReactiveUI 2"));
        rp.Value = "ReactiveUI 2";
        Assert.Equal(rp.Value, "ReactiveUI 2");
    }

    [Fact]
    public void SetValueRaisesEvents()
    {
        using var rp = new ReactiveProperty<string>();
        rp.Value.Should().BeNull();
        rp.Value = "ReactiveUI";
        Assert.Equal(rp.Value, "ReactiveUI");
        rp.Subscribe(x => Assert.Equal(x, "ReactiveUI"));
    }

    [Fact]
    public void ValidationLengthIsCorrectlyHandled()
    {
        var target = new ReactivePropertyVM();
        IEnumerable? error = default;
        target.LengthLessThanFiveProperty
            .ObserveErrorChanged
            .Subscribe(x => error = x);

        target.LengthLessThanFiveProperty.HasErrors.Should().BeTrue();
        Assert.Equal(error?.OfType<string>().First(), "required");

        target.LengthLessThanFiveProperty.Value = "a";
        target.LengthLessThanFiveProperty.HasErrors.Should().BeFalse();
        error.Should().BeNull();

        target.LengthLessThanFiveProperty.Value = "aaaaaa";
        target.LengthLessThanFiveProperty.HasErrors.Should().BeTrue();
        error.Should().NotBeNull();
        Assert.Equal(error?.OfType<string>().First(), "5over");

        target.LengthLessThanFiveProperty.Value = null;
        target.LengthLessThanFiveProperty.HasErrors.Should().BeTrue();
        Assert.Equal(error?.OfType<string>().First(), "required");
    }

    [Fact]
    public void ValidationIsRequiredIsCorrectlyHandled()
    {
        var target = new ReactivePropertyVM();
        var errors = new List<IEnumerable?>();
        target.IsRequiredProperty
            .ObserveErrorChanged
            .Where(x => x != null)
            .Subscribe(errors.Add);

        errors.Count.Should().Be(1);
        errors[0]?.Cast<string>().Should().Equal("error!");
        target.IsRequiredProperty.HasErrors.Should().BeTrue();

        target.IsRequiredProperty.Value = "a";
        errors.Count.Should().Be(1);
        target.IsRequiredProperty.HasErrors.Should().BeFalse();

        target.IsRequiredProperty.Value = null;
        errors.Count.Should().Be(2);
        errors[1]?.Cast<string>().Should().Equal("error!");
        target.IsRequiredProperty.HasErrors.Should().BeTrue();
    }

    [Fact]
    public void ValidationTaskTest()
    {
        var target = new ReactivePropertyVM();
        var errors = new List<IEnumerable?>();
        target.TaskValidationTestProperty
            .ObserveErrorChanged
            .Where(x => x != null)
            .Subscribe(errors.Add);
        errors.Count.Should().Be(1);
        errors[0]?.OfType<string>().Should().Equal("required");

        target.TaskValidationTestProperty.Value = "a";
        target.TaskValidationTestProperty.HasErrors.Should().BeFalse();
        errors.Count.Should().Be(1);

        target.TaskValidationTestProperty.Value = null;
        target.TaskValidationTestProperty.HasErrors.Should().BeTrue();
        errors.Count.Should().Be(2);
    }

    [Fact]
    public void ValidationWithCustomErrorMessage()
    {
        var target = new ReactivePropertyVM();
        target.CustomValidationErrorMessageProperty.Value = string.Empty;
        var errorMessage = target?
            .CustomValidationErrorMessageProperty?
            .GetErrors(nameof(ReactivePropertyVM.CustomValidationErrorMessageProperty))!
            .Cast<string>()
            .First();

        Assert.Equal(errorMessage, "Custom validation error message for CustomValidationErrorMessageProperty");
    }

    [Fact]
    public void ValidationWithCustomErrorMessageWithDisplayName()
    {
        var target = new ReactivePropertyVM();
        target.CustomValidationErrorMessageWithDisplayNameProperty.Value = string.Empty;
        var errorMessage = target
            .CustomValidationErrorMessageWithDisplayNameProperty?
            .GetErrors(nameof(ReactivePropertyVM.CustomValidationErrorMessageWithDisplayNameProperty))!
            .Cast<string>()
            .First();

        Assert.Equal(errorMessage, "Custom validation error message for CustomName");
    }

    [Fact]
    public void ValidationWithCustomErrorMessageWithResource()
    {
        var target = new ReactivePropertyVM();
        target.CustomValidationErrorMessageWithResourceProperty.Value = string.Empty;
        var errorMessage = target
            .CustomValidationErrorMessageWithResourceProperty?
            .GetErrors(nameof(ReactivePropertyVM.CustomValidationErrorMessageWithResourceProperty))!
            .Cast<string>()
            .First();

        Assert.Equal(errorMessage, "Oops!? FromResource is required.");
    }

    [Fact]
    public async Task ValidationWithAsyncSuccessCase()
    {
        var tcs = new TaskCompletionSource<string?>();
        using var rp = new ReactiveProperty<string>().AddValidationError(_ => tcs.Task);

        IEnumerable? error = null;
        rp.ObserveErrorChanged.Subscribe(x => error = x);

        rp.HasErrors.Should().BeFalse();
        error.Should().BeNull();

        rp.Value = "dummy";
        tcs.SetResult(null);
        await Task.Yield();

        rp.HasErrors.Should().BeFalse();
        error.Should().BeNull();
    }

    [Fact]
    public async Task ValidationWithAsyncFailedCase()
    {
        var tcs = new TaskCompletionSource<string?>();
        using var rp = new ReactiveProperty<string>().AddValidationError(_ => tcs.Task);

        IEnumerable? error = null;
        rp.ObserveErrorChanged.Subscribe(x => error = x);

        rp.HasErrors.Should().BeFalse();
        error.Should().BeNull();

        var errorMessage = "error occured!!";
        rp.Value = "dummy";  //--- push value
        tcs.SetResult(errorMessage);    //--- validation error!
        await Task.Delay(10);

        rp.HasErrors.Should().BeTrue();
        error.Should().NotBeNull();
        error?.Cast<string>().Should().Equal(errorMessage);
        rp.GetErrors("Value")?.Cast<string>().Should().Equal(errorMessage);
    }

    [Fact]
    public void ValidationWithAsyncThrottleTest()
    {
        var scheduler = new TestScheduler();
        using var rp = new ReactiveProperty<string>()
                        .AddValidationError(xs => xs
                            .Throttle(TimeSpan.FromSeconds(1), scheduler)
                            .Select(x => string.IsNullOrEmpty(x) ? "required" : null));

        IEnumerable? error = null;
        rp.ObserveErrorChanged.Subscribe(x => error = x);

        scheduler.AdvanceTo(TimeSpan.FromMilliseconds(0).Ticks);
        rp.Value = string.Empty;
        rp.HasErrors.Should().BeFalse();
        error.Should().BeNull();

        scheduler.AdvanceTo(TimeSpan.FromMilliseconds(300).Ticks);
        rp.Value = "a";
        rp.HasErrors.Should().BeFalse();
        error.Should().BeNull();

        scheduler.AdvanceTo(TimeSpan.FromMilliseconds(700).Ticks);
        rp.Value = "b";
        rp.HasErrors.Should().BeFalse();
        error.Should().BeNull();

        scheduler.AdvanceTo(TimeSpan.FromMilliseconds(1100).Ticks);
        rp.Value = string.Empty;
        rp.HasErrors.Should().BeFalse();
        error.Should().BeNull();

        scheduler.AdvanceTo(TimeSpan.FromMilliseconds(2500).Ticks);
        rp.HasErrors.Should().BeTrue();
        error.Should().NotBeNull();
        error?.Cast<string>().Should().Equal("required");
    }

    [Fact]
    public void ValidationErrorChangedTest()
    {
        var errors = new List<IEnumerable?>();
        using var rprop = new ReactiveProperty<string>()
            .AddValidationError(x => string.IsNullOrWhiteSpace(x) ? "error" : null);

        // old version behavior
        rprop.ObserveErrorChanged.Skip(1).Subscribe(errors.Add);

        errors.Count.Should().Be(0);

        rprop.Value = "OK";
        errors.Count.Should().Be(1);
        errors.Last().Should().BeNull();

        rprop.Value = null;
        errors.Count.Should().Be(2);
        errors.Last()?.OfType<string>().Should().Equal("error");
    }

    [Fact]
    public void ValidationIgnoreInitialErrorAndRefresh()
    {
        using var rp = new ReactiveProperty<string>()
            .AddValidationError(x => string.IsNullOrEmpty(x) ? "error" : null, true);

        rp.HasErrors.Should().BeFalse();
        rp.Refresh();
        rp.HasErrors.Should().BeTrue();
    }

    [Fact]
    public void IgnoreInitialErrorAndCheckValidation()
    {
        using var rp = new ReactiveProperty<string>()
            .AddValidationError(x => string.IsNullOrEmpty(x) ? "error" : null, true);

        rp.HasErrors.Should().BeFalse();
        rp.CheckValidation();
        rp.HasErrors.Should().BeTrue();
    }

    [Fact]
    public void IgnoreInitErrorAndUpdateValue()
    {
        using var rp = new ReactiveProperty<string>()
            .AddValidationError(x => string.IsNullOrEmpty(x) ? "error" : null, true);

        rp.HasErrors.Should().BeFalse();
        rp.Value = string.Empty;
        rp.HasErrors.Should().BeTrue();
    }

    [Fact]
    public void ObserveErrors()
    {
        using var rp = new ReactiveProperty<string>()
            .AddValidationError(x => x == null ? "Error" : null);

        var results = new List<IEnumerable?>();
        rp.ObserveErrorChanged.Subscribe(results.Add);
        rp.Value = "OK";

        results.Count.Should().Be(2);
        results[0]?.OfType<string>().Should().Equal("Error");
        results[1].Should().BeNull();
    }

    [Fact]
    public void ObserveHasError()
    {
        using var rp = new ReactiveProperty<string>()
            .AddValidationError(x => x == null ? "Error" : null);

        var results = new List<bool>();
        rp.ObserveHasErrors.Subscribe(x => results.Add(x));
        rp.Value = "OK";

        results.Count.Should().Be(2);
        results[0].Should().BeTrue();
        results[1].Should().BeFalse();
    }

    [Fact]
    public void CheckValidation()
    {
        var minValue = 0;
        using var rp = new ReactiveProperty<int>(0)
            .AddValidationError(x => x < minValue ? "Error" : null);
        rp.GetErrors("Value").Should().BeNull();

        minValue = 1;
        rp.GetErrors("Value").Should().BeNull();

        rp.CheckValidation();
        rp.GetErrors("Value")?.OfType<string>().Should().Equal("Error");
    }

    [Fact]
    public async Task ValueUpdatesMultipleTimesWithDifferentValues()
    {
        using var testSequencer = new TestSequencer();
        using var rp = new ReactiveProperty<int>(0);
        var collector = new List<int>();
        rp.Subscribe(async x =>
        {
            collector.Add(x);
            await testSequencer.AdvancePhaseAsync();
        });

        rp.Value.Should().Be(0);
        await testSequencer.AdvancePhaseAsync();
        collector.Should().Equal(0);
        rp.Value = 1;
        rp.Value.Should().Be(1);
        await testSequencer.AdvancePhaseAsync();
        collector.Should().Equal(0, 1);
        rp.Value = 2;
        rp.Value.Should().Be(2);
        await testSequencer.AdvancePhaseAsync();
        collector.Should().Equal(0, 1, 2);
        rp.Value = 3;
        rp.Value.Should().Be(3);
        await testSequencer.AdvancePhaseAsync();
        collector.Should().Equal(0, 1, 2, 3);
    }

    [Fact]
    public async Task Refresh()
    {
        using var testSequencer = new TestSequencer();
        using var rp = new ReactiveProperty<int>(0);
        var collector = new List<int>();
        rp.Subscribe(async x =>
        {
            collector.Add(x);
            await testSequencer.AdvancePhaseAsync();
        });

        await testSequencer.AdvancePhaseAsync();
        collector.Should().Equal(0);

        // refresh should always produce a value even if it is the same and duplicates are not allowed
        rp.Refresh();
        await testSequencer.AdvancePhaseAsync();
        collector.Should().Equal(0, 0);
    }

    [Fact]
    public async Task ValueUpdatesMultipleTimesWithSameValues()
    {
        using var testSequencer = new TestSequencer();
        using var rp = new ReactiveProperty<int>(0, false, true);
        var collector = new List<int>();
        rp.Subscribe(async x =>
        {
            collector.Add(x);
            await testSequencer.AdvancePhaseAsync();
        });

        rp.Value.Should().Be(0);
        await testSequencer.AdvancePhaseAsync();
        collector.Should().Equal(0);
        rp.Value = 0;
        rp.Value.Should().Be(0);
        await testSequencer.AdvancePhaseAsync();
        collector.Should().Equal(0, 0);
        rp.Value = 0;
        rp.Value.Should().Be(0);
        await testSequencer.AdvancePhaseAsync();
        collector.Should().Equal(0, 0, 0);
        rp.Value = 0;
        rp.Value.Should().Be(0);
        await testSequencer.AdvancePhaseAsync();
        collector.Should().Equal(0, 0, 0, 0);
    }

    [Fact]
    public async Task MultipleSubscribersGetCurrentValue()
    {
        using var testSequencer1 = new TestSequencer();
        using var testSequencer2 = new TestSequencer();
        using var rp = new ReactiveProperty<int>(0);
        var collector1 = new List<int>();
        var collector2 = new List<int>();
        rp.Subscribe(async x =>
        {
            collector1.Add(x);
            await testSequencer1.AdvancePhaseAsync();
        });

        rp.Value.Should().Be(0);
        await testSequencer1.AdvancePhaseAsync();
        collector1.Should().Equal(0);
        rp.Value = 1;
        rp.Value.Should().Be(1);
        await testSequencer1.AdvancePhaseAsync();
        collector1.Should().Equal(0, 1);
        rp.Value = 2;
        rp.Value.Should().Be(2);
        await testSequencer1.AdvancePhaseAsync();
        collector1.Should().Equal(0, 1, 2);

        // second subscriber
        rp.Subscribe(async x =>
        {
            collector2.Add(x);
            await testSequencer2.AdvancePhaseAsync();
        });
        rp.Value.Should().Be(2);
        await testSequencer2.AdvancePhaseAsync();
        collector2.Should().Equal(2);

        rp.Value = 3;
        rp.Value.Should().Be(3);
        await testSequencer1.AdvancePhaseAsync();
        collector1.Should().Equal(0, 1, 2, 3);
        await testSequencer2.AdvancePhaseAsync();
        collector2.Should().Equal(2, 3);
    }

    [Fact]
    public void TestMultipleSubstribers()
    {
        using var vm = new SubcribeTestViewModel(1000);
        vm.SubscriberCount.Should().Be(1000);
        vm.StartupTime.Should().BeLessThan(2000);
        vm.SubscriberEvents.Should().Be(1000);
    }
}
