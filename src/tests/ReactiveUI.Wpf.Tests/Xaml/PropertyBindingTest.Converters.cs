// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Globalization;
using ReactiveUI.Tests.Xaml.Mocks;

namespace ReactiveUI.Tests.Xaml;

/// <summary>Tests property bindings.</summary>
/// <content>
/// Tests property bindings using typed and func converters with trigger updates.
/// </content>
public partial class PropertyBindingTest
{
    /// <summary>Verifies bind with func to trigger update test view model to view.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task BindWithFuncToTriggerUpdateTestViewModelToView()
    {
        var dis = new MultipleDisposable();
        var vm = new PropertyBindViewModel();
        var view = new PropertyBindView { ViewModel = vm };
        var update = new Signal<bool>();

        vm.JustADecimal = InitialDecimal;
        await Assert.That(view.SomeTextBox.Text).IsNotEqualTo(vm.JustADecimal.ToString(CultureInfo.InvariantCulture));

        view.Bind(
            vm,
            static x => x.JustADecimal,
            static x => x.SomeTextBox.Text,
            update.AsObservable(),
            static d => d.ToString(CultureInfo.InvariantCulture),
            static t => decimal.TryParse(t, out var res) ? res : decimal.Zero,
            TriggerUpdate.ViewModelToView).DisposeWith(dis);

        vm.JustADecimal = DecimalOne;

        // value should have pre bind value
        await Assert.That(view.SomeTextBox.Text).IsEqualTo(InitialNumericText);

        // trigger UI update
        update.OnNext(true);
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("1.0");

        vm.JustADecimal = DecimalTwo;
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("1.0");

        update.OnNext(true);
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("2.0");

        // test reverse bind no trigger required
        view.SomeTextBox.Text = "3.0";
        await Assert.That(vm.JustADecimal).IsEqualTo(DecimalThree);

        view.SomeTextBox.Text = "4.0";
        await Assert.That(vm.JustADecimal).IsEqualTo(DecimalFour);

        // test forward bind to ensure trigger is still honoured.
        vm.JustADecimal = DecimalTwo;
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("4.0");

        update.OnNext(true);
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("2.0");

        dis.Dispose();
        await Assert.That(dis.IsDisposed).IsTrue();
    }

    /// <summary>Verifies bind with func to trigger update test view model to view with decimal converter.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task BindWithFuncToTriggerUpdateTestViewModelToViewWithDecimalConverter()
    {
        var dis = new MultipleDisposable();
        var vm = new PropertyBindViewModel();
        var view = new PropertyBindView { ViewModel = vm };
        var update = new Signal<bool>();

        vm.JustADecimal = InitialDecimal;
        await Assert.That(view.SomeTextBox.Text).IsNotEqualTo(vm.JustADecimal.ToString(CultureInfo.InvariantCulture));

        var decimalToStringTypeConverter = new DecimalToStringTypeConverter();

        view.Bind(
            vm,
            static x => x.JustADecimal,
            static x => x.SomeTextBox.Text,
            update.AsObservable(),
            RoundingHint,
            decimalToStringTypeConverter,
            decimalToStringTypeConverter,
            TriggerUpdate.ViewModelToView).DisposeWith(dis);

        vm.JustADecimal = DecimalOne;

        // value should have pre bind value
        await Assert.That(view.SomeTextBox.Text).IsEqualTo(InitialNumericText);

        // trigger UI update
        update.OnNext(true);
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("1.00");

        vm.JustADecimal = DecimalTwo;
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("1.00");

        update.OnNext(true);
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("2.00");

        // test reverse bind no trigger required
        view.SomeTextBox.Text = "3.00";
        await Assert.That(vm.JustADecimal).IsEqualTo(DecimalThree);

        view.SomeTextBox.Text = "4.00";
        await Assert.That(vm.JustADecimal).IsEqualTo(DecimalFour);

        // test forward bind to ensure trigger is still honoured.
        vm.JustADecimal = DecimalTwo;
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("4.00");

        update.OnNext(true);
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("2.00");

        dis.Dispose();
        await Assert.That(dis.IsDisposed).IsTrue();
    }

    /// <summary>Verifies bind with func to trigger update test view model to view with nullable decimal converter.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task BindWithFuncToTriggerUpdateTestViewModelToViewWithNullableDecimalConverter()
    {
        var dis = new MultipleDisposable();
        var vm = new PropertyBindViewModel();
        var view = new PropertyBindView { ViewModel = vm };
        var update = new Signal<bool>();

        vm.JustANullDecimal = InitialDecimal;
        await Assert.That(view.SomeTextBox.Text).IsNotEqualTo(vm.JustANullDecimal.Value.ToString(CultureInfo.InvariantCulture));

        var decimalToStringTypeConverter = new NullableDecimalToStringTypeConverter();

        view.Bind(
            vm,
            static x => x.JustANullDecimal,
            static x => x.SomeTextBox.Text,
            update.AsObservable(),
            RoundingHint,
            decimalToStringTypeConverter,
            decimalToStringTypeConverter,
            TriggerUpdate.ViewModelToView).DisposeWith(dis);

        vm.JustANullDecimal = DecimalOne;

        // value should have pre bind value
        await Assert.That(view.SomeTextBox.Text).IsEqualTo(InitialNumericText);

        // trigger UI update
        update.OnNext(true);
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("1.00");

        vm.JustANullDecimal = DecimalTwo;
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("1.00");

        update.OnNext(true);
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("2.00");

        // test reverse bind no trigger required
        view.SomeTextBox.Text = "3.00";
        await Assert.That(vm.JustANullDecimal).IsEqualTo(DecimalThree);

        // test non numerical
        view.SomeTextBox.Text = "ad3";
        await Assert.That(vm.JustANullDecimal).IsEqualTo(DecimalThree);

        view.SomeTextBox.Text = "4.00";
        await Assert.That(vm.JustANullDecimal).IsEqualTo(DecimalFour);

        // test forward bind to ensure trigger is still honoured.
        vm.JustANullDecimal = DecimalTwo;
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("4.00");

        update.OnNext(true);
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("2.00");

        dis.Dispose();
        await Assert.That(dis.IsDisposed).IsTrue();
    }

    /// <summary>Verifies bind with func to trigger update test view to view model.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task BindWithFuncToTriggerUpdateTestViewToViewModel()
    {
        var dis = new MultipleDisposable();
        var vm = new PropertyBindViewModel();
        var view = new PropertyBindView { ViewModel = vm };
        var update = new Signal<bool>();

        vm.JustADecimal = InitialDecimal;
        await Assert.That(view.SomeTextBox.Text).IsNotEqualTo(vm.JustADecimal.ToString(CultureInfo.InvariantCulture));

        view.Bind(
            vm,
            static x => x.JustADecimal,
            static x => x.SomeTextBox.Text,
            update.AsObservable(),
            static d => d.ToString(CultureInfo.InvariantCulture),
            static t => decimal.TryParse(t, out var res) ? res : decimal.Zero,
            TriggerUpdate.ViewToViewModel).DisposeWith(dis);

        view.SomeTextBox.Text = "1.0";

        // value should have pre bind value
        await Assert.That(vm.JustADecimal).IsEqualTo(InitialDecimal);

        // trigger UI update
        update.OnNext(true);
        await Assert.That(vm.JustADecimal).IsEqualTo(DecimalOne);

        view.SomeTextBox.Text = "2.0";
        await Assert.That(vm.JustADecimal).IsEqualTo(DecimalOne);

        update.OnNext(true);
        await Assert.That(vm.JustADecimal).IsEqualTo(DecimalTwo);

        // test reverse bind no trigger required
        vm.JustADecimal = DecimalThree;
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("3.0");

        vm.JustADecimal = DecimalFour;
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("4.0");

        // test forward bind to ensure trigger is still honoured.
        view.SomeTextBox.Text = "2.0";
        await Assert.That(vm.JustADecimal).IsEqualTo(DecimalFour);

        update.OnNext(true);
        await Assert.That(vm.JustADecimal).IsEqualTo(DecimalTwo);

        dis.Dispose();
        await Assert.That(dis.IsDisposed).IsTrue();
    }

    /// <summary>Verifies bind with func to trigger update test view model to view with double converter.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task BindWithFuncToTriggerUpdateTestViewModelToViewWithDoubleConverter()
    {
        var dis = new MultipleDisposable();
        var vm = new PropertyBindViewModel();
        var view = new PropertyBindView { ViewModel = vm };
        var update = new Signal<bool>();

        vm.JustADouble = InitialDouble;
        await Assert.That(view.SomeTextBox.Text).IsNotEqualTo(vm.JustADouble.ToString(CultureInfo.InvariantCulture));

        var toStringConverter = new DoubleToStringTypeConverter();

        view.Bind(
            vm,
            static x => x.JustADouble,
            static x => x.SomeTextBox.Text,
            update.AsObservable(),
            RoundingHint,
            toStringConverter,
            toStringConverter,
            TriggerUpdate.ViewModelToView).DisposeWith(dis);

        vm.JustADouble = DoubleOne;

        // value should have pre bind value
        await Assert.That(view.SomeTextBox.Text).IsEqualTo(InitialNumericText);

        // trigger UI update
        update.OnNext(true);
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("1.00");

        vm.JustADouble = DoubleTwo;
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("1.00");

        update.OnNext(true);
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("2.00");

        // test reverse bind no trigger required
        view.SomeTextBox.Text = "3.00";
        await Assert.That(vm.JustADouble).IsEqualTo(DoubleThree);

        view.SomeTextBox.Text = "4.00";
        await Assert.That(vm.JustADouble).IsEqualTo(DoubleFour);

        // test forward bind to ensure trigger is still honoured.
        vm.JustADouble = DoubleTwo;
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("4.00");

        update.OnNext(true);
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("2.00");

        dis.Dispose();
        await Assert.That(dis.IsDisposed).IsTrue();
    }

    /// <summary>Verifies bind with func to trigger update test view model to view with nullable double converter.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task BindWithFuncToTriggerUpdateTestViewModelToViewWithNullableDoubleConverter()
    {
        var dis = new MultipleDisposable();
        var vm = new PropertyBindViewModel();
        var view = new PropertyBindView { ViewModel = vm };
        var update = new Signal<bool>();

        vm.JustANullDouble = InitialDouble;
        await Assert.That(view.SomeTextBox.Text).IsNotEqualTo(vm.JustANullDouble.Value.ToString(CultureInfo.InvariantCulture));

        var toStringConverter = new NullableDoubleToStringTypeConverter();

        view.Bind(
            vm,
            static x => x.JustANullDouble,
            static x => x.SomeTextBox.Text,
            update.AsObservable(),
            RoundingHint,
            toStringConverter,
            toStringConverter,
            TriggerUpdate.ViewModelToView).DisposeWith(dis);

        vm.JustANullDouble = DoubleOne;

        // value should have pre bind value
        await Assert.That(view.SomeTextBox.Text).IsEqualTo(InitialNumericText);

        // trigger UI update
        update.OnNext(true);
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("1.00");

        vm.JustANullDouble = DoubleTwo;
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("1.00");

        update.OnNext(true);
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("2.00");

        // test reverse bind no trigger required
        view.SomeTextBox.Text = "3.00";
        await Assert.That(vm.JustANullDouble).IsEqualTo(DoubleThree);

        // test non numerical value
        view.SomeTextBox.Text = "fa0";
        await Assert.That(vm.JustANullDouble).IsEqualTo(DoubleThree);

        view.SomeTextBox.Text = "4.00";
        await Assert.That(vm.JustANullDouble).IsEqualTo(DoubleFour);

        // test forward bind to ensure trigger is still honoured.
        vm.JustANullDouble = DoubleTwo;
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("4.00");

        update.OnNext(true);
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("2.00");

        dis.Dispose();
        await Assert.That(dis.IsDisposed).IsTrue();
    }

    /// <summary>Verifies bind with func to trigger update test view model to view with double converter no round.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task BindWithFuncToTriggerUpdateTestViewModelToViewWithDoubleConverterNoRound()
    {
        var dis = new MultipleDisposable();
        var vm = new PropertyBindViewModel();
        var view = new PropertyBindView { ViewModel = vm };
        var update = new Signal<bool>();

        vm.JustADouble = InitialDouble;
        await Assert.That(view.SomeTextBox.Text).IsNotEqualTo(vm.JustADouble.ToString(CultureInfo.InvariantCulture));

        view.Bind(vm, static x => x.JustADouble, static x => x.SomeTextBox.Text, update.AsObservable(), null, null, null, triggerUpdate: TriggerUpdate.ViewModelToView).DisposeWith(dis);

        vm.JustADouble = DoubleOne;

        // value should have pre bind value
        await Assert.That(view.SomeTextBox.Text).IsEqualTo(InitialNumericText);

        // trigger UI update
        update.OnNext(true);
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("1");

        vm.JustADouble = DoubleTwo;
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("1");

        update.OnNext(true);
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("2");

        // test reverse bind no trigger required
        view.SomeTextBox.Text = "3";
        await Assert.That(vm.JustADouble).IsEqualTo(DoubleThree);

        view.SomeTextBox.Text = "4";
        await Assert.That(vm.JustADouble).IsEqualTo(DoubleFour);

        // test forward bind to ensure trigger is still honoured.
        vm.JustADouble = DoubleTwo;
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("4");

        update.OnNext(true);
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("2");

        dis.Dispose();
        await Assert.That(dis.IsDisposed).IsTrue();
    }

    /// <summary>Verifies bind with func to trigger update test view model to view with single converter.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task BindWithFuncToTriggerUpdateTestViewModelToViewWithSingleConverter()
    {
        var dis = new MultipleDisposable();
        var vm = new PropertyBindViewModel();
        var view = new PropertyBindView { ViewModel = vm };
        var update = new Signal<bool>();

        vm.JustASingle = InitialSingle;
        await Assert.That(view.SomeTextBox.Text).IsNotEqualTo(vm.JustASingle.ToString(CultureInfo.InvariantCulture));

        var toStringConverter = new SingleToStringTypeConverter();

        view.Bind(
            vm,
            static x => x.JustASingle,
            static x => x.SomeTextBox.Text,
            update.AsObservable(),
            RoundingHint,
            toStringConverter,
            toStringConverter,
            TriggerUpdate.ViewModelToView).DisposeWith(dis);

        vm.JustASingle = SingleOne;

        // value should have pre bind value
        await Assert.That(view.SomeTextBox.Text).IsEqualTo(InitialNumericText);

        // trigger UI update
        update.OnNext(true);
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("1.00");

        vm.JustASingle = SingleTwo;
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("1.00");

        update.OnNext(true);
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("2.00");

        // test reverse bind no trigger required
        view.SomeTextBox.Text = "3.00";
        await Assert.That(vm.JustASingle).IsEqualTo(SingleThree);

        view.SomeTextBox.Text = "4.00";
        await Assert.That(vm.JustASingle).IsEqualTo(SingleFour);

        // test forward bind to ensure trigger is still honoured.
        vm.JustASingle = SingleTwo;
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("4.00");

        update.OnNext(true);
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("2.00");

        dis.Dispose();
        await Assert.That(dis.IsDisposed).IsTrue();
    }

    /// <summary>Verifies bind with func to trigger update test view model to view with nullable single converter.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task BindWithFuncToTriggerUpdateTestViewModelToViewWithNullableSingleConverter()
    {
        var dis = new MultipleDisposable();
        var vm = new PropertyBindViewModel();
        var view = new PropertyBindView { ViewModel = vm };
        var update = new Signal<bool>();

        vm.JustANullSingle = InitialSingle;
        await Assert.That(view.SomeTextBox.Text).IsNotEqualTo(vm.JustANullSingle.Value.ToString(CultureInfo.InvariantCulture));

        var toStringConverter = new NullableSingleToStringTypeConverter();

        view.Bind(
            vm,
            static x => x.JustANullSingle,
            static x => x.SomeTextBox.Text,
            update.AsObservable(),
            RoundingHint,
            toStringConverter,
            toStringConverter,
            TriggerUpdate.ViewModelToView).DisposeWith(dis);

        vm.JustANullSingle = SingleOne;

        // value should have pre bind value
        await Assert.That(view.SomeTextBox.Text).IsEqualTo(InitialNumericText);

        // trigger UI update
        update.OnNext(true);
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("1.00");

        vm.JustANullSingle = SingleTwo;
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("1.00");

        update.OnNext(true);
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("2.00");

        // test reverse bind no trigger required
        view.SomeTextBox.Text = "3.00";
        await Assert.That(vm.JustANullSingle).IsEqualTo(SingleThree);

        // test non numerical value
        view.SomeTextBox.Text = "fa0";
        await Assert.That(vm.JustANullSingle).IsEqualTo(SingleThree);

        view.SomeTextBox.Text = "4.00";
        await Assert.That(vm.JustANullSingle).IsEqualTo(SingleFour);

        // test forward bind to ensure trigger is still honoured.
        vm.JustANullSingle = SingleTwo;
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("4.00");

        update.OnNext(true);
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("2.00");

        dis.Dispose();
        await Assert.That(dis.IsDisposed).IsTrue();
    }

    /// <summary>Verifies bind with func to trigger update test view model to view with single converter no round.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task BindWithFuncToTriggerUpdateTestViewModelToViewWithSingleConverterNoRound()
    {
        var dis = new MultipleDisposable();
        var vm = new PropertyBindViewModel();
        var view = new PropertyBindView { ViewModel = vm };
        var update = new Signal<bool>();

        vm.JustASingle = InitialSingle;
        await Assert.That(view.SomeTextBox.Text).IsNotEqualTo(vm.JustASingle.ToString(CultureInfo.InvariantCulture));

        view.Bind(vm, static x => x.JustASingle, static x => x.SomeTextBox.Text, update.AsObservable(), null, null, null, triggerUpdate: TriggerUpdate.ViewModelToView).DisposeWith(dis);

        vm.JustASingle = SingleOne;

        // value should have pre bind value
        await Assert.That(view.SomeTextBox.Text).IsEqualTo(InitialNumericText);

        // trigger UI update
        update.OnNext(true);
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("1");

        vm.JustASingle = SingleTwo;
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("1");

        update.OnNext(true);
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("2");

        // test reverse bind no trigger required
        view.SomeTextBox.Text = "3";
        await Assert.That(vm.JustASingle).IsEqualTo(SingleThree);

        view.SomeTextBox.Text = "4";
        await Assert.That(vm.JustASingle).IsEqualTo(SingleFour);

        // test forward bind to ensure trigger is still honoured.
        vm.JustASingle = SingleTwo;
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("4");

        update.OnNext(true);
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("2");

        dis.Dispose();
        await Assert.That(dis.IsDisposed).IsTrue();
    }
}
