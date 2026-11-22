// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.WhenAny;

/// <summary>Tests for the WhenAny and WhenAnyValue mixin overloads.</summary>
public partial class WhenAnyMixinTests
{
    /// <summary>Verifies the WhenAny overload for 10 properties with a selector.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task WhenAny_10Props_Sel()
    {
        var vm = new WhenAnyArityTestViewModel();
        var list = new List<string>();
        _ = vm.WhenAny(
            static x => x.Property1,
            static x => x.Property2,
            static x => x.Property3,
            static x => x.Property4,
            static x => x.Property5,
            static x => x.Property6,
            static x => x.Property7,
            static x => x.Property8,
            static x => x.Property9,
            static x => x.Property10,
            static (_, _, _, _, _, _, _, _, _, _) => "x").ObserveOn(Sequencer.Immediate).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    /// <summary>Verifies the WhenAnyValue overload for 10 properties with a selector.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task WhenAnyValue_10Props_Sel()
    {
        var vm = new WhenAnyArityTestViewModel();
        var list = new List<string>();
        _ = vm.WhenAnyValue(
            static x => x.Property1,
            static x => x.Property2,
            static x => x.Property3,
            static x => x.Property4,
            static x => x.Property5,
            static x => x.Property6,
            static x => x.Property7,
            static x => x.Property8,
            static x => x.Property9,
            static x => x.Property10,
            static (_, _, _, _, _, _, _, _, _, _) => "x").ObserveOn(Sequencer.Immediate).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }
}
