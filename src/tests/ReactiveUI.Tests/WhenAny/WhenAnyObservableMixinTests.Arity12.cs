// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.WhenAny;

/// <summary>Tests for the WhenAnyObservable mixin overloads.</summary>
public partial class WhenAnyObservableMixinTests
{
    /// <summary>Verifies the WhenAnyObservable overload for 12 observable properties.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task WhenAnyObservable_12Props()
    {
        var vm = new WhenAnyArityTestViewModel();
        var subj1 = new Signal<string>();
        vm.ObservableProperty1 = subj1;
        var subj2 = new Signal<string>();
        vm.ObservableProperty2 = subj2;
        var subj3 = new Signal<string>();
        vm.ObservableProperty3 = subj3;
        var subj4 = new Signal<string>();
        vm.ObservableProperty4 = subj4;
        var subj5 = new Signal<string>();
        vm.ObservableProperty5 = subj5;
        var subj6 = new Signal<string>();
        vm.ObservableProperty6 = subj6;
        var subj7 = new Signal<string>();
        vm.ObservableProperty7 = subj7;
        var subj8 = new Signal<string>();
        vm.ObservableProperty8 = subj8;
        var subj9 = new Signal<string>();
        vm.ObservableProperty9 = subj9;
        var subj10 = new Signal<string>();
        vm.ObservableProperty10 = subj10;
        var subj11 = new Signal<string>();
        vm.ObservableProperty11 = subj11;
        var subj12 = new Signal<string>();
        vm.ObservableProperty12 = subj12;
        var list = new List<string>();
        _ = vm.WhenAnyObservable(
            x => x.ObservableProperty1,
            x => x.ObservableProperty2,
            x => x.ObservableProperty3,
            x => x.ObservableProperty4,
            x => x.ObservableProperty5,
            x => x.ObservableProperty6,
            x => x.ObservableProperty7,
            x => x.ObservableProperty8,
            x => x.ObservableProperty9,
            x => x.ObservableProperty10,
            x => x.ObservableProperty11,
            x => x.ObservableProperty12).ObserveOn(Sequencer.Immediate).Subscribe(list.Add);
        subj1.OnNext("test");
        subj2.OnNext("test");
        subj3.OnNext("test");
        subj4.OnNext("test");
        subj5.OnNext("test");
        subj6.OnNext("test");
        subj7.OnNext("test");
        subj8.OnNext("test");
        subj9.OnNext("test");
        subj10.OnNext("test");
        subj11.OnNext("test");
        subj12.OnNext("test");
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    /// <summary>Verifies the WhenAnyObservable overload for 12 observable properties with a selector.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task WhenAnyObservable_12Props_Sel()
    {
        var vm = new WhenAnyArityTestViewModel();
        var subj1 = new Signal<string>();
        vm.ObservableProperty1 = subj1;
        var subj2 = new Signal<string>();
        vm.ObservableProperty2 = subj2;
        var subj3 = new Signal<string>();
        vm.ObservableProperty3 = subj3;
        var subj4 = new Signal<string>();
        vm.ObservableProperty4 = subj4;
        var subj5 = new Signal<string>();
        vm.ObservableProperty5 = subj5;
        var subj6 = new Signal<string>();
        vm.ObservableProperty6 = subj6;
        var subj7 = new Signal<string>();
        vm.ObservableProperty7 = subj7;
        var subj8 = new Signal<string>();
        vm.ObservableProperty8 = subj8;
        var subj9 = new Signal<string>();
        vm.ObservableProperty9 = subj9;
        var subj10 = new Signal<string>();
        vm.ObservableProperty10 = subj10;
        var subj11 = new Signal<string>();
        vm.ObservableProperty11 = subj11;
        var subj12 = new Signal<string>();
        vm.ObservableProperty12 = subj12;
        var list = new List<string>();
        _ = vm.WhenAnyObservable(
            x => x.ObservableProperty1,
            x => x.ObservableProperty2,
            x => x.ObservableProperty3,
            x => x.ObservableProperty4,
            x => x.ObservableProperty5,
            x => x.ObservableProperty6,
            x => x.ObservableProperty7,
            x => x.ObservableProperty8,
            x => x.ObservableProperty9,
            x => x.ObservableProperty10,
            x => x.ObservableProperty11,
            x => x.ObservableProperty12,
            static (_, _, _, _, _, _, _, _, _, _, _, _) => "x").ObserveOn(Sequencer.Immediate).Subscribe(list.Add);
        subj1.OnNext("test");
        subj2.OnNext("test");
        subj3.OnNext("test");
        subj4.OnNext("test");
        subj5.OnNext("test");
        subj6.OnNext("test");
        subj7.OnNext("test");
        subj8.OnNext("test");
        subj9.OnNext("test");
        subj10.OnNext("test");
        subj11.OnNext("test");
        subj12.OnNext("test");
        await Assert.That(list).Count().IsGreaterThan(0);
    }
}
