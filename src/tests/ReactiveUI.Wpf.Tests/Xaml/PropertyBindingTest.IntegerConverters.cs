// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Globalization;
using System.Reactive.Disposables;
using System.Reactive.Disposables.Fluent;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using ReactiveUI.Tests.Xaml.Mocks;

namespace ReactiveUI.Tests.Xaml;

/// <content>
/// Tests property bindings using integral typed converters with trigger updates.
/// </content>
public partial class PropertyBindingTest
{
    /// <summary>
    /// Verifies bind with func to trigger update test view model to view with byte converter.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task BindWithFuncToTriggerUpdateTestViewModelToViewWithByteConverter()
    {
        var dis = new CompositeDisposable();
        var vm = new PropertyBindViewModel();
        var view = new PropertyBindView { ViewModel = vm };
        var update = new Subject<bool>();

        vm.JustAByte = InitialIntegral;
        await Assert.That(view.SomeTextBox.Text).IsNotEqualTo(vm.JustAByte.ToString(CultureInfo.InvariantCulture));

        var xToStringTypeConverter = new ByteToStringTypeConverter();

        view.Bind(
            vm,
            static x => x.JustAByte,
            static x => x.SomeTextBox.Text,
            update.AsObservable(),
            FormatHint,
            xToStringTypeConverter,
            xToStringTypeConverter,
            TriggerUpdate.ViewModelToView).DisposeWith(dis);

        vm.JustAByte = 1;

        // value should have pre bind value
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("123");

        // trigger UI update
        update.OnNext(true);
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("001");

        vm.JustAByte = IntegralTwo;
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("001");

        update.OnNext(true);
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("002");

        // test reverse bind no trigger required
        view.SomeTextBox.Text = "003";
        await Assert.That((int)vm.JustAByte).IsEqualTo(IntegralThree);

        view.SomeTextBox.Text = "004";
        await Assert.That((int)vm.JustAByte).IsEqualTo(IntegralFour);

        // test forward bind to ensure trigger is still honoured.
        vm.JustAByte = IntegralTwo;
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("004");

        update.OnNext(true);
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("002");

        dis.Dispose();
        await Assert.That(dis.IsDisposed).IsTrue();
    }

    /// <summary>
    /// Verifies bind with func to trigger update test view model to view with nullable byte converter.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task BindWithFuncToTriggerUpdateTestViewModelToViewWithNullableByteConverter()
    {
        var dis = new CompositeDisposable();
        var vm = new PropertyBindViewModel();
        var view = new PropertyBindView { ViewModel = vm };
        var update = new Subject<bool>();

        vm.JustANullByte = InitialIntegral;
        await Assert.That(view.SomeTextBox.Text).IsNotEqualTo(vm.JustANullByte.Value.ToString(CultureInfo.InvariantCulture));

        var xToStringTypeConverter = new NullableByteToStringTypeConverter();

        view.Bind(
            vm,
            static x => x.JustANullByte,
            static x => x.SomeTextBox.Text,
            update.AsObservable(),
            FormatHint,
            xToStringTypeConverter,
            xToStringTypeConverter,
            TriggerUpdate.ViewModelToView).DisposeWith(dis);

        vm.JustANullByte = 1;

        // value should have pre bind value
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("123");

        // trigger UI update
        update.OnNext(true);
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("001");

        vm.JustANullByte = IntegralTwo;
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("001");

        update.OnNext(true);
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("002");

        // test reverse bind no trigger required
        view.SomeTextBox.Text = "003";
        await Assert.That((int)vm.JustANullByte!.Value).IsEqualTo(IntegralThree);

        // test non numerical value
        view.SomeTextBox.Text = "ad4";
        await Assert.That((int)vm.JustANullByte!.Value).IsEqualTo(IntegralThree);

        view.SomeTextBox.Text = "004";
        await Assert.That((int)vm.JustANullByte!.Value).IsEqualTo(IntegralFour);

        // test forward bind to ensure trigger is still honoured.
        vm.JustANullByte = IntegralTwo;
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("004");

        update.OnNext(true);
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("002");

        dis.Dispose();
        await Assert.That(dis.IsDisposed).IsTrue();
    }

    /// <summary>
    /// Verifies bind with func to trigger update test view model to view with byte converter no hint.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task BindWithFuncToTriggerUpdateTestViewModelToViewWithByteConverterNoHint()
    {
        var dis = new CompositeDisposable();
        var vm = new PropertyBindViewModel();
        var view = new PropertyBindView { ViewModel = vm };
        var update = new Subject<bool>();

        vm.JustAByte = InitialIntegral;
        await Assert.That(view.SomeTextBox.Text).IsNotEqualTo(vm.JustAByte.ToString(CultureInfo.InvariantCulture));

        view.Bind(vm, static x => x.JustAByte, static x => x.SomeTextBox.Text, update.AsObservable(), null, null, null, triggerUpdate: TriggerUpdate.ViewModelToView).DisposeWith(dis);

        vm.JustAByte = 1;

        // value should have pre bind value
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("123");

        // trigger UI update
        update.OnNext(true);
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("1");

        vm.JustAByte = IntegralTwo;
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("1");

        update.OnNext(true);
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("2");

        // test reverse bind no trigger required
        view.SomeTextBox.Text = "3";
        await Assert.That((int)vm.JustAByte).IsEqualTo(IntegralThree);

        view.SomeTextBox.Text = "4";
        await Assert.That((int)vm.JustAByte).IsEqualTo(IntegralFour);

        // test forward bind to ensure trigger is still honoured.
        vm.JustAByte = IntegralTwo;
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("4");

        update.OnNext(true);
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("2");

        dis.Dispose();
        await Assert.That(dis.IsDisposed).IsTrue();
    }

    /// <summary>
    /// Verifies bind with func to trigger update test view model to view with short converter.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task BindWithFuncToTriggerUpdateTestViewModelToViewWithShortConverter()
    {
        var dis = new CompositeDisposable();
        var vm = new PropertyBindViewModel();
        var view = new PropertyBindView { ViewModel = vm };
        var update = new Subject<bool>();

        vm.JustAInt16 = InitialIntegral;
        await Assert.That(view.SomeTextBox.Text).IsNotEqualTo(vm.JustAInt16.ToString(CultureInfo.InvariantCulture));

        var xToStringTypeConverter = new ShortToStringTypeConverter();

        view.Bind(
            vm,
            static x => x.JustAInt16,
            static x => x.SomeTextBox.Text,
            update.AsObservable(),
            FormatHint,
            xToStringTypeConverter,
            xToStringTypeConverter,
            TriggerUpdate.ViewModelToView).DisposeWith(dis);

        vm.JustAInt16 = 1;

        // value should have pre bind value
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("123");

        // trigger UI update
        update.OnNext(true);
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("001");

        vm.JustAInt16 = IntegralTwo;
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("001");

        update.OnNext(true);
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("002");

        // test reverse bind no trigger required
        view.SomeTextBox.Text = "003";
        await Assert.That((int)vm.JustAInt16).IsEqualTo(IntegralThree);

        view.SomeTextBox.Text = "004";
        await Assert.That((int)vm.JustAInt16).IsEqualTo(IntegralFour);

        // test forward bind to ensure trigger is still honoured.
        vm.JustAInt16 = IntegralTwo;
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("004");

        update.OnNext(true);
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("002");

        dis.Dispose();
        await Assert.That(dis.IsDisposed).IsTrue();
    }

    /// <summary>
    /// Verifies bind with func to trigger update test view model to view with nullable short converter.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task BindWithFuncToTriggerUpdateTestViewModelToViewWithNullableShortConverter()
    {
        var dis = new CompositeDisposable();
        var vm = new PropertyBindViewModel();
        var view = new PropertyBindView { ViewModel = vm };
        var update = new Subject<bool>();

        vm.JustANullInt16 = InitialIntegral;
        await Assert.That(view.SomeTextBox.Text).IsNotEqualTo(vm.JustANullInt16.Value.ToString(CultureInfo.InvariantCulture));

        var xToStringTypeConverter = new NullableShortToStringTypeConverter();

        view.Bind(
            vm,
            static x => x.JustANullInt16,
            static x => x.SomeTextBox.Text,
            update.AsObservable(),
            FormatHint,
            xToStringTypeConverter,
            xToStringTypeConverter,
            TriggerUpdate.ViewModelToView).DisposeWith(dis);

        vm.JustANullInt16 = 1;

        // value should have pre bind value
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("123");

        // trigger UI update
        update.OnNext(true);
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("001");

        vm.JustANullInt16 = IntegralTwo;
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("001");

        update.OnNext(true);
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("002");

        // test reverse bind no trigger required
        view.SomeTextBox.Text = "003";
        await Assert.That((int)vm.JustANullInt16!.Value).IsEqualTo(IntegralThree);

        // test non numerical value
        view.SomeTextBox.Text = "fa0";
        await Assert.That((int)vm.JustANullInt16!.Value).IsEqualTo(IntegralThree);

        view.SomeTextBox.Text = "004";
        await Assert.That((int)vm.JustANullInt16!.Value).IsEqualTo(IntegralFour);

        // test forward bind to ensure trigger is still honoured.
        vm.JustANullInt16 = IntegralTwo;
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("004");

        update.OnNext(true);
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("002");

        dis.Dispose();
        await Assert.That(dis.IsDisposed).IsTrue();
    }

    /// <summary>
    /// Verifies bind with func to trigger update test view model to view with short converter no hint.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task BindWithFuncToTriggerUpdateTestViewModelToViewWithShortConverterNoHint()
    {
        var dis = new CompositeDisposable();
        var vm = new PropertyBindViewModel();
        var view = new PropertyBindView { ViewModel = vm };
        var update = new Subject<bool>();

        vm.JustAInt16 = InitialIntegral;
        await Assert.That(view.SomeTextBox.Text).IsNotEqualTo(vm.JustAInt16.ToString(CultureInfo.InvariantCulture));

        view.Bind(vm, static x => x.JustAInt16, static x => x.SomeTextBox.Text, update.AsObservable(), null, null, null, triggerUpdate: TriggerUpdate.ViewModelToView).DisposeWith(dis);

        vm.JustAInt16 = 1;

        // value should have pre bind value
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("123");

        // trigger UI update
        update.OnNext(true);
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("1");

        vm.JustAInt16 = IntegralTwo;
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("1");

        update.OnNext(true);
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("2");

        // test reverse bind no trigger required
        view.SomeTextBox.Text = "3";
        await Assert.That((int)vm.JustAInt16).IsEqualTo(IntegralThree);

        view.SomeTextBox.Text = "4";
        await Assert.That((int)vm.JustAInt16).IsEqualTo(IntegralFour);

        // test forward bind to ensure trigger is still honoured.
        vm.JustAInt16 = IntegralTwo;
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("4");

        update.OnNext(true);
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("2");

        dis.Dispose();
        await Assert.That(dis.IsDisposed).IsTrue();
    }

    /// <summary>
    /// Verifies bind with func to trigger update test view model to view with integer converter.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task BindWithFuncToTriggerUpdateTestViewModelToViewWithIntegerConverter()
    {
        var dis = new CompositeDisposable();
        var vm = new PropertyBindViewModel();
        var view = new PropertyBindView { ViewModel = vm };
        var update = new Subject<bool>();

        vm.JustAInt32 = InitialIntegral;
        await Assert.That(view.SomeTextBox.Text).IsNotEqualTo(vm.JustAInt32.ToString(CultureInfo.InvariantCulture));

        var xToStringTypeConverter = new IntegerToStringTypeConverter();

        view.Bind(
            vm,
            static x => x.JustAInt32,
            static x => x.SomeTextBox.Text,
            update.AsObservable(),
            FormatHint,
            xToStringTypeConverter,
            xToStringTypeConverter,
            TriggerUpdate.ViewModelToView).DisposeWith(dis);

        vm.JustAInt32 = 1;

        // value should have pre bind value
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("123");

        // trigger UI update
        update.OnNext(true);
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("001");

        vm.JustAInt32 = IntegralTwo;
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("001");

        update.OnNext(true);
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("002");

        // test reverse bind no trigger required
        view.SomeTextBox.Text = "003";
        await Assert.That(vm.JustAInt32).IsEqualTo(IntegralThree);

        view.SomeTextBox.Text = "004";
        await Assert.That(vm.JustAInt32).IsEqualTo(IntegralFour);

        // test forward bind to ensure trigger is still honoured.
        vm.JustAInt32 = IntegralTwo;
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("004");

        update.OnNext(true);
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("002");

        dis.Dispose();
        await Assert.That(dis.IsDisposed).IsTrue();
    }

    /// <summary>
    /// Verifies bind with func to trigger update test view model to view with nullable integer converter.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task BindWithFuncToTriggerUpdateTestViewModelToViewWithNullableIntegerConverter()
    {
        var dis = new CompositeDisposable();
        var vm = new PropertyBindViewModel();
        var view = new PropertyBindView { ViewModel = vm };
        var update = new Subject<bool>();

        vm.JustANullInt32 = InitialIntegral;
        await Assert.That(view.SomeTextBox.Text).IsNotEqualTo(vm.JustANullInt32!.Value.ToString(CultureInfo.InvariantCulture));

        var xToStringTypeConverter = new NullableIntegerToStringTypeConverter();

        view.Bind(
            vm,
            static x => x.JustANullInt32,
            static x => x.SomeTextBox.Text,
            update.AsObservable(),
            FormatHint,
            xToStringTypeConverter,
            xToStringTypeConverter,
            TriggerUpdate.ViewModelToView).DisposeWith(dis);

        vm.JustANullInt32 = 1;

        // value should have pre bind value
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("123");

        // trigger UI update
        update.OnNext(true);
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("001");

        vm.JustANullInt32 = IntegralTwo;
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("001");

        update.OnNext(true);
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("002");

        // test reverse bind no trigger required
        view.SomeTextBox.Text = "003";
        await Assert.That(vm.JustANullInt32).IsEqualTo(IntegralThree);

        // test if the binding handles a non number
        view.SomeTextBox.Text = "3a4";
        await Assert.That(vm.JustANullInt32).IsEqualTo(IntegralThree);

        view.SomeTextBox.Text = "004";
        await Assert.That(vm.JustANullInt32).IsEqualTo(IntegralFour);

        // test forward bind to ensure trigger is still honoured.
        vm.JustANullInt32 = IntegralTwo;
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("004");

        update.OnNext(true);
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("002");

        dis.Dispose();
        await Assert.That(dis.IsDisposed).IsTrue();
    }

    /// <summary>
    /// Verifies bind with func to trigger update test view model to view with integer converter no hint.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task BindWithFuncToTriggerUpdateTestViewModelToViewWithIntegerConverterNoHint()
    {
        var dis = new CompositeDisposable();
        var vm = new PropertyBindViewModel();
        var view = new PropertyBindView { ViewModel = vm };
        var update = new Subject<bool>();

        vm.JustAInt32 = InitialIntegral;
        await Assert.That(view.SomeTextBox.Text).IsNotEqualTo(vm.JustAInt32.ToString(CultureInfo.InvariantCulture));

        view.Bind(vm, static x => x.JustAInt32, static x => x.SomeTextBox.Text, update.AsObservable(), null, null, null, triggerUpdate: TriggerUpdate.ViewModelToView).DisposeWith(dis);

        vm.JustAInt32 = 1;

        // value should have pre bind value
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("123");

        // trigger UI update
        update.OnNext(true);
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("1");

        vm.JustAInt32 = IntegralTwo;
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("1");

        update.OnNext(true);
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("2");

        // test reverse bind no trigger required
        view.SomeTextBox.Text = "3";
        await Assert.That(vm.JustAInt32).IsEqualTo(IntegralThree);

        view.SomeTextBox.Text = "4";
        await Assert.That(vm.JustAInt32).IsEqualTo(IntegralFour);

        // test forward bind to ensure trigger is still honoured.
        vm.JustAInt32 = IntegralTwo;
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("4");

        update.OnNext(true);
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("2");

        dis.Dispose();
        await Assert.That(dis.IsDisposed).IsTrue();
    }

    /// <summary>
    /// Verifies bind with func to trigger update test view model to view with long converter.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task BindWithFuncToTriggerUpdateTestViewModelToViewWithLongConverter()
    {
        var dis = new CompositeDisposable();
        var vm = new PropertyBindViewModel();
        var view = new PropertyBindView { ViewModel = vm };
        var update = new Subject<bool>();

        vm.JustAInt64 = InitialIntegral;
        await Assert.That(view.SomeTextBox.Text).IsNotEqualTo(vm.JustAInt64.ToString(CultureInfo.InvariantCulture));

        var xToStringTypeConverter = new LongToStringTypeConverter();

        view.Bind(
            vm,
            static x => x.JustAInt64,
            static x => x.SomeTextBox.Text,
            update.AsObservable(),
            FormatHint,
            xToStringTypeConverter,
            xToStringTypeConverter,
            TriggerUpdate.ViewModelToView).DisposeWith(dis);

        vm.JustAInt64 = 1;

        // value should have pre bind value
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("123");

        // trigger UI update
        update.OnNext(true);
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("001");

        vm.JustAInt64 = IntegralTwo;
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("001");

        update.OnNext(true);
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("002");

        // test reverse bind no trigger required
        view.SomeTextBox.Text = "003";
        await Assert.That(vm.JustAInt64).IsEqualTo(IntegralThree);

        view.SomeTextBox.Text = "004";
        await Assert.That(vm.JustAInt64).IsEqualTo(IntegralFour);

        // test forward bind to ensure trigger is still honoured.
        vm.JustAInt64 = IntegralTwo;
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("004");

        update.OnNext(true);
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("002");

        dis.Dispose();
        await Assert.That(dis.IsDisposed).IsTrue();
    }

    /// <summary>
    /// Verifies bind with func to trigger update test view model to view with long converter no hint.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task BindWithFuncToTriggerUpdateTestViewModelToViewWithLongConverterNoHint()
    {
        var dis = new CompositeDisposable();
        var vm = new PropertyBindViewModel();
        var view = new PropertyBindView { ViewModel = vm };
        var update = new Subject<bool>();

        vm.JustAInt64 = InitialIntegral;
        await Assert.That(view.SomeTextBox.Text).IsNotEqualTo(vm.JustAInt64.ToString(CultureInfo.InvariantCulture));

        view.Bind(vm, static x => x.JustAInt64, static x => x.SomeTextBox.Text, update.AsObservable(), null, null, null, triggerUpdate: TriggerUpdate.ViewModelToView).DisposeWith(dis);

        vm.JustAInt64 = 1;

        // value should have pre bind value
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("123");

        // trigger UI update
        update.OnNext(true);
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("1");

        vm.JustAInt64 = IntegralTwo;
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("1");

        update.OnNext(true);
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("2");

        // test reverse bind no trigger required
        view.SomeTextBox.Text = "3";
        await Assert.That(vm.JustAInt64).IsEqualTo(IntegralThree);

        view.SomeTextBox.Text = "4";
        await Assert.That(vm.JustAInt64).IsEqualTo(IntegralFour);

        // test forward bind to ensure trigger is still honoured.
        vm.JustAInt64 = IntegralTwo;
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("4");

        update.OnNext(true);
        await Assert.That(view.SomeTextBox.Text).IsEqualTo("2");

        dis.Dispose();
        await Assert.That(dis.IsDisposed).IsTrue();
    }
}
