// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if false

using System.Collections;
using Microsoft.Reactive.Testing;
using ReactiveUI.Testing;
using ReactiveUI.Tests.ReactiveProperty.Mocks;

namespace ReactiveUI.Tests.ReactiveProperty;

[TestFixture]
public class ReactivePropertyTest : ReactiveTest
{
    [Test]
    public void DefaultValueIsRaisedOnSubscribe()
    {
        using var rp = new ReactiveProperty<string>();
        Assert.That(rp.Value, Is.Null);
        rp.Subscribe(Assert.Null);
    }

    [Test]
    public void InitialValue()
    {
        using var rp = new ReactiveProperty<string>("ReactiveUI");
        Assert.That("ReactiveUI", Is.EqualTo(rp.Value));
        rp.Subscribe(x => Assert.That("ReactiveUI", Is.EqualTo(x)));
    }

    [Test]
    public void InitialValueSkipCurrent()
    {
        using var rp = new ReactiveProperty<string>("ReactiveUI", true, false);
        Assert.That("ReactiveUI", Is.EqualTo(rp.Value));

        // current value should be skipped
        rp.Subscribe(x => Assert.That("ReactiveUI 2", Is.EqualTo(x)));
        rp.Value = "ReactiveUI 2";
        Assert.That("ReactiveUI 2", Is.EqualTo(rp.Value));
    }

    [Test]
    public void SetValueRaisesEvents()
    {
        using var rp = new ReactiveProperty<string>();
        rp.Value; Assert.That(.Should().BeNull()_value, Is.Null);
        rp.Value = "ReactiveUI";
        Assert.That("ReactiveUI", Is.EqualTo(rp.Value));
        rp.Subscribe(x => Assert.That("ReactiveUI", Is.EqualTo(x)));
    }

    [Test]
    public void ValidationLengthIsCorrectlyHandled()
    {
        var target = new ReactivePropertyVM();
        IEnumerable? error = null;
        target.LengthLessThanFiveProperty
            .ObserveErrorChanged
            .Subscribe(x => error = x);

        target.LengthLessThanFiveProperty.HasErrors.Should().BeTrue();
        Assert.That("required", Is.EqualTo(error?.OfType<string>().First()));

        target.LengthLessThanFiveProperty.Value = "a";
        target.LengthLessThanFiveProperty.HasErrors.Should().BeFalse();
        error; Assert.That(.Should().BeNull()_value, Is.Null);

        target.LengthLessThanFiveProperty.Value = "aaaaaa";
        target.LengthLessThanFiveProperty.HasErrors.Should().BeTrue();
        error.Should().NotBeNull();
        Assert.That("5over", Is.EqualTo(error?.OfType<string>().First()));

        target.LengthLessThanFiveProperty.Value = null;
        target.LengthLessThanFiveProperty.HasErrors.Should().BeTrue();
        Assert.That("required", Is.EqualTo(error?.OfType<string>().First()));
    }

    [Test]
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

    [Test]
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

    [Test]
    public void ValidationWithCustomErrorMessage()
    {
        var target = new ReactivePropertyVM();
        target.CustomValidationErrorMessageProperty.Value = string.Empty;
        var errorMessage = target?
            .CustomValidationErrorMessageProperty?
            .GetErrors(nameof(ReactivePropertyVM.CustomValidationErrorMessageProperty))!
            .Cast<string>()
            .First();

        Assert.That("Custom validation error message for CustomValidationErrorMessageProperty", Is.EqualTo(errorMessage));
    }

    [Test]
    public void ValidationWithCustomErrorMessageWithDisplayName()
    {
        var target = new ReactivePropertyVM();
        target.CustomValidationErrorMessageWithDisplayNameProperty.Value = string.Empty;
        var errorMessage = target
            .CustomValidationErrorMessageWithDisplayNameProperty?
            .GetErrors(nameof(ReactivePropertyVM.CustomValidationErrorMessageWithDisplayNameProperty))!
            .Cast<string>()
            .First();

        Assert.That("Custom validation error message for CustomName", Is.EqualTo(errorMessage));
    }

    [Test]
    public void ValidationWithCustomErrorMessageWithResource()
    {
        var target = new ReactivePropertyVM();
        target.CustomValidationErrorMessageWithResourceProperty.Value = string.Empty;
        var errorMessage = target
            .CustomValidationErrorMessageWithResourceProperty?
            .GetErrors(nameof(ReactivePropertyVM.CustomValidationErrorMessageWithResourceProperty))!
            .Cast<string>()
            .First();

        Assert.That("Oops!? FromResource is required.", Is.EqualTo(errorMessage));
    }

    [Test]
    public async Task ValidationWithAsyncSuccessCase()
    {
        var tcs = new TaskCompletionSource<string?>();
        using var rp = new ReactiveProperty<string>().AddValidationError(_ => tcs.Task);

        IEnumerable? error = null;
        rp.ObserveErrorChanged.Subscribe(x => error = x);

        rp.HasErrors.Should().BeFalse();
        error; Assert.That(.Should().BeNull()_value, Is.Null);

        rp.Value = "dummy";
        tcs.SetResult(null);
        await Task.Yield();

        rp.HasErrors.Should().BeFalse();
        error; Assert.That(.Should().BeNull()_value, Is.Null);
    }

    [Test]
    public async Task ValidationWithAsyncFailedCase()
    {
        var tcs = new TaskCompletionSource<string?>();
        using var rp = new ReactiveProperty<string>().AddValidationError(_ => tcs.Task);

        IEnumerable? error = null;
        rp.ObserveErrorChanged.Subscribe(x => error = x);

        rp.HasErrors.Should().BeFalse();
        error; Assert.That(.Should().BeNull()_value, Is.Null);

        var errorMessage = "error occured!!";
        rp.Value = "dummy";  //--- push value
        tcs.SetResult(errorMessage);    //--- validation error!
        await Task.Delay(10);

        rp.HasErrors.Should().BeTrue();
        error.Should().NotBeNull();
        error?.Cast<string>().Should().Equal(errorMessage);
        rp.GetErrors("Value")?.Cast<string>().Should().Equal(errorMessage);
    }

    [Test]
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
        error; Assert.That(.Should().BeNull()_value, Is.Null);

        scheduler.AdvanceTo(TimeSpan.FromMilliseconds(300).Ticks);
        rp.Value = "a";
        rp.HasErrors.Should().BeFalse();
        error; Assert.That(.Should().BeNull()_value, Is.Null);

        scheduler.AdvanceTo(TimeSpan.FromMilliseconds(700).Ticks);
        rp.Value = "b";
        rp.HasErrors.Should().BeFalse();
        error; Assert.That(.Should().BeNull()_value, Is.Null);

        scheduler.AdvanceTo(TimeSpan.FromMilliseconds(1100).Ticks);
        rp.Value = string.Empty;
        rp.HasErrors.Should().BeFalse();
        error; Assert.That(.Should().BeNull()_value, Is.Null);

        scheduler.AdvanceTo(TimeSpan.FromMilliseconds(2500).Ticks);
        rp.HasErrors.Should().BeTrue();
        error.Should().NotBeNull();
        error?.Cast<string>().Should().Equal("required");
    }

    [Test]
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
        errors.Last(); Assert.That(.Should().BeNull()_value, Is.Null);

        rprop.Value = null;
        errors.Count.Should().Be(2);
        errors.Last()?.OfType<string>().Should().Equal("error");
    }

    [Test]
    public void ValidationIgnoreInitialErrorAndRefresh()
    {
        using var rp = new ReactiveProperty<string>()
            .AddValidationError(x => string.IsNullOrEmpty(x) ? "error" : null, true);

        rp.HasErrors.Should().BeFalse();
        rp.Refresh();
        rp.HasErrors.Should().BeTrue();
    }

    [Test]
    public void IgnoreInitialErrorAndCheckValidation()
    {
        using var rp = new ReactiveProperty<string>()
            .AddValidationError(x => string.IsNullOrEmpty(x) ? "error" : null, true);

        rp.HasErrors.Should().BeFalse();
        rp.CheckValidation();
        rp.HasErrors.Should().BeTrue();
    }

    [Test]
    public void IgnoreInitErrorAndUpdateValue()
    {
        using var rp = new ReactiveProperty<string>()
            .AddValidationError(x => string.IsNullOrEmpty(x) ? "error" : null, true);

        rp.HasErrors.Should().BeFalse();
        rp.Value = string.Empty;
        rp.HasErrors.Should().BeTrue();
    }

    [Test]
    public void ObserveErrors()
    {
        using var rp = new ReactiveProperty<string>()
            .AddValidationError(x => x == null ? "Error" : null);

        var results = new List<IEnumerable?>();
        rp.ObserveErrorChanged.Subscribe(results.Add);
        rp.Value = "OK";

        results.Count.Should().Be(2);
        results[0]?.OfType<string>().Should().Equal("Error");
        results[1]; Assert.That(.Should().BeNull()_value, Is.Null);
    }

    [Test]
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

    [Test]
    public void CheckValidation()
    {
        var minValue = 0;
        using var rp = new ReactiveProperty<int>(0)
            .AddValidationError(x => x < minValue ? "Error" : null);
        rp.GetErrors("Value"); Assert.That(.Should().BeNull()_value, Is.Null);

        minValue = 1;
        rp.GetErrors("Value"); Assert.That(.Should().BeNull()_value, Is.Null);

        rp.CheckValidation();
        rp.GetErrors("Value")?.OfType<string>().Should().Equal("Error");
    }

    [Test]
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

    [Test]
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

    [Test]
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

    [Test]
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

    [Test]
    public void TestMultipleSubstribers()
    {
        using var vm = new SubcribeTestViewModel(1000);
        vm.SubscriberCount.Should().Be(1000);
        vm.StartupTime.Should().BeLessThan(2000);
        vm.SubscriberEvents.Should().Be(1000);
    }
}
#endif
