// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.WhenAny;

/// <summary>Tests for the WhenAnyObservable mixin overloads.</summary>
public partial class WhenAnyObservableMixinTests
{
    /// <summary>Verifies the WhenAnyObservable overload for 1 observable property.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task WhenAnyObservable_1Prop()
    {
        var vm = new WhenAnyArityTestViewModel();
        var subj = new Signal<string>();
        vm.ObservableProperty1 = subj;
        var list = new List<string>();
        _ = vm.WhenAnyObservable(x => x.ObservableProperty1).ObserveOn(Sequencer.Immediate).Subscribe(list.Add);
        subj.OnNext("test");
        await Assert.That(list).Count().IsGreaterThan(0);
    }
}
