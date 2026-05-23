// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace ReactiveUI.Tests.WhenAny;

/// <content>
/// Arity-4 WhenAnyObservable overload tests.
/// </content>
public partial class WhenAnyObservableMixinTests
{
    /// <summary>
    ///     Verifies the WhenAnyObservable overload for 4 observable properties.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task WhenAnyObservable_4Props()
    {
        var vm = new WhenAnyArityTestViewModel();
        var subj1 = new Subject<string>();
        vm.ObservableProperty1 = subj1;
        var subj2 = new Subject<string>();
        vm.ObservableProperty2 = subj2;
        var subj3 = new Subject<string>();
        vm.ObservableProperty3 = subj3;
        var subj4 = new Subject<string>();
        vm.ObservableProperty4 = subj4;
        var list = new List<string>();
        vm.WhenAnyObservable(
            x => x.ObservableProperty1,
            x => x.ObservableProperty2,
            x => x.ObservableProperty3,
            x => x.ObservableProperty4).ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        subj1.OnNext("test");
        subj2.OnNext("test");
        subj3.OnNext("test");
        subj4.OnNext("test");
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    /// <summary>
    ///     Verifies the WhenAnyObservable overload for 4 observable properties with a selector.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task WhenAnyObservable_4Props_Sel()
    {
        var vm = new WhenAnyArityTestViewModel();
        var subj1 = new Subject<string>();
        vm.ObservableProperty1 = subj1;
        var subj2 = new Subject<string>();
        vm.ObservableProperty2 = subj2;
        var subj3 = new Subject<string>();
        vm.ObservableProperty3 = subj3;
        var subj4 = new Subject<string>();
        vm.ObservableProperty4 = subj4;
        var list = new List<string>();
        vm.WhenAnyObservable(
            x => x.ObservableProperty1,
            x => x.ObservableProperty2,
            x => x.ObservableProperty3,
            x => x.ObservableProperty4,
            (_, _, _, _) => "x").ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        subj1.OnNext("test");
        subj2.OnNext("test");
        subj3.OnNext("test");
        subj4.OnNext("test");
        await Assert.That(list).Count().IsGreaterThan(0);
    }
}
