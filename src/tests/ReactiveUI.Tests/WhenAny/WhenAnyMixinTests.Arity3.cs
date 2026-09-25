// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.WhenAny;

/// <summary>Tests for the WhenAny and WhenAnyValue mixin overloads.</summary>
public partial class WhenAnyMixinTests
{
    /// <summary>Verifies the WhenAny overload for 3 properties with a selector.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task WhenAny_3Props_Sel()
    {
        var vm = new WhenAnyArityTestViewModel();
        var list = new List<string>();
        _ = vm.WhenAny(
            x => x.Property1,
            x => x.Property2,
            x => x.Property3,
            static (_, _, _) => "x").ObserveOn(Sequencer.Immediate).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    /// <summary>Verifies the WhenAnyValue overload for 3 properties with a selector.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task WhenAnyValue_3Props_Sel()
    {
        var vm = new WhenAnyArityTestViewModel();
        var list = new List<string>();
        _ = vm.WhenAnyValue(
            x => x.Property1,
            x => x.Property2,
            x => x.Property3,
            static (_, _, _) => "x").ObserveOn(Sequencer.Immediate).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    /// <summary>Verifies the tuple expression-based WhenAnyValue overload for 3 properties.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task WhenAnyValue_3Props_Tuple_Expr()
    {
        var vm = new WhenAnyArityTestViewModel();
        var list = new List<PropertyValues<string?, string?, string?>>();
        _ = vm.WhenAnyValue(
            x => x.Property1,
            x => x.Property2,
            x => x.Property3).ObserveOn(Sequencer.Immediate).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }
}
