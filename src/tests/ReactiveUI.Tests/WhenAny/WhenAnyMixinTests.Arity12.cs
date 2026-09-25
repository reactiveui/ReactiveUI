// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.WhenAny;

/// <summary>Tests for the WhenAny and WhenAnyValue mixin overloads.</summary>
public partial class WhenAnyMixinTests
{
    /// <summary>Verifies the WhenAny overload for 12 properties with a selector.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task WhenAny_12Props_Sel()
    {
        var vm = new WhenAnyArityTestViewModel();
        var list = new List<string>();
        _ = vm.WhenAny(
            x => x.Property1,
            x => x.Property2,
            x => x.Property3,
            x => x.Property4,
            x => x.Property5,
            x => x.Property6,
            x => x.Property7,
            x => x.Property8,
            x => x.Property9,
            x => x.Property10,
            x => x.Property11,
            x => x.Property12,
            static (_, _, _, _, _, _, _, _, _, _, _, _) => "x").ObserveOn(Sequencer.Immediate).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    /// <summary>Verifies the WhenAnyValue overload for 12 properties with a selector.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task WhenAnyValue_12Props_Sel()
    {
        var vm = new WhenAnyArityTestViewModel();
        var list = new List<string>();
        _ = vm.WhenAnyValue(
            x => x.Property1,
            x => x.Property2,
            x => x.Property3,
            x => x.Property4,
            x => x.Property5,
            x => x.Property6,
            x => x.Property7,
            x => x.Property8,
            x => x.Property9,
            x => x.Property10,
            x => x.Property11,
            x => x.Property12,
            static (_, _, _, _, _, _, _, _, _, _, _, _) => "x").ObserveOn(Sequencer.Immediate).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }
}
