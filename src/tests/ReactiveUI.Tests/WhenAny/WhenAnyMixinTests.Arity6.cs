// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.WhenAny;

/// <summary>Tests for the WhenAny and WhenAnyValue mixin overloads.</summary>
public partial class WhenAnyMixinTests
{
    /// <summary>Verifies the WhenAny overload for 6 properties with a selector.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task WhenAny_6Props_Sel()
    {
        var vm = new WhenAnyArityTestViewModel();
        var list = new List<string>();
        vm.WhenAny(
            x => x.Property1,
            x => x.Property2,
            x => x.Property3,
            x => x.Property4,
            x => x.Property5,
            x => x.Property6,
            (_, _, _, _, _, _) => "x").ObserveOn(Sequencer.Immediate).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    /// <summary>Verifies the WhenAny overload for 6 properties with a selector and distinct-until-changed flag.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task WhenAny_6Props_Sel_Dist()
    {
        var vm = new WhenAnyArityTestViewModel();
        var list = new List<string>();
        vm.WhenAny(
            x => x.Property1,
            x => x.Property2,
            x => x.Property3,
            x => x.Property4,
            x => x.Property5,
            x => x.Property6,
            (_, _, _, _, _, _) => "x",
            true).ObserveOn(Sequencer.Immediate).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    /// <summary>Verifies the string-based WhenAny overload for 6 properties with a selector.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task WhenAny_6Props_Sel_Str()
    {
        var vm = new WhenAnyArityTestViewModel();
        var list = new List<string>();
        vm.WhenAny<WhenAnyArityTestViewModel, string, string?, string?, string?, string?, string?, string?>(
            nameof(WhenAnyArityTestViewModel.Property1),
            nameof(WhenAnyArityTestViewModel.Property2),
            nameof(WhenAnyArityTestViewModel.Property3),
            nameof(WhenAnyArityTestViewModel.Property4),
            nameof(WhenAnyArityTestViewModel.Property5),
            nameof(WhenAnyArityTestViewModel.Property6),
            (_, _, _, _, _, _) => "x").ObserveOn(Sequencer.Immediate).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    /// <summary>Verifies the string-based WhenAny overload for 6 properties with a selector and distinct-until-changed flag.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task WhenAny_6Props_Sel_Str_Dist()
    {
        var vm = new WhenAnyArityTestViewModel();
        var list = new List<string>();
        vm.WhenAny<WhenAnyArityTestViewModel, string, string?, string?, string?, string?, string?, string?>(
            nameof(WhenAnyArityTestViewModel.Property1),
            nameof(WhenAnyArityTestViewModel.Property2),
            nameof(WhenAnyArityTestViewModel.Property3),
            nameof(WhenAnyArityTestViewModel.Property4),
            nameof(WhenAnyArityTestViewModel.Property5),
            nameof(WhenAnyArityTestViewModel.Property6),
            (_, _, _, _, _, _) => "x",
            false).ObserveOn(Sequencer.Immediate).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    /// <summary>Verifies the WhenAnyValue overload for 6 properties with a selector.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task WhenAnyValue_6Props_Sel()
    {
        var vm = new WhenAnyArityTestViewModel();
        var list = new List<string>();
        vm.WhenAnyValue(
            x => x.Property1,
            x => x.Property2,
            x => x.Property3,
            x => x.Property4,
            x => x.Property5,
            x => x.Property6,
            (_, _, _, _, _, _) => "x").ObserveOn(Sequencer.Immediate).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    /// <summary>Verifies the WhenAnyValue overload for 6 properties with a selector and distinct-until-changed flag.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task WhenAnyValue_6Props_Sel_Dist()
    {
        var vm = new WhenAnyArityTestViewModel();
        var list = new List<string>();
        vm.WhenAnyValue(
            x => x.Property1,
            x => x.Property2,
            x => x.Property3,
            x => x.Property4,
            x => x.Property5,
            x => x.Property6,
            (_, _, _, _, _, _) => "x",
            true).ObserveOn(Sequencer.Immediate).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    /// <summary>Verifies the string-based WhenAnyValue overload for 6 properties with a selector.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task WhenAnyValue_6Props_Sel_Str()
    {
        var vm = new WhenAnyArityTestViewModel();
        var list = new List<string>();
        vm.WhenAnyValue<WhenAnyArityTestViewModel, string, string?, string?, string?, string?, string?, string?>(
            nameof(WhenAnyArityTestViewModel.Property1),
            nameof(WhenAnyArityTestViewModel.Property2),
            nameof(WhenAnyArityTestViewModel.Property3),
            nameof(WhenAnyArityTestViewModel.Property4),
            nameof(WhenAnyArityTestViewModel.Property5),
            nameof(WhenAnyArityTestViewModel.Property6),
            (_, _, _, _, _, _) => "x").ObserveOn(Sequencer.Immediate).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    /// <summary>Verifies the string-based WhenAnyValue overload for 6 properties with a selector and distinct-until-changed flag.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task WhenAnyValue_6Props_Sel_Str_Dist()
    {
        var vm = new WhenAnyArityTestViewModel();
        var list = new List<string>();
        vm.WhenAnyValue<WhenAnyArityTestViewModel, string, string?, string?, string?, string?, string?, string?>(
            nameof(WhenAnyArityTestViewModel.Property1),
            nameof(WhenAnyArityTestViewModel.Property2),
            nameof(WhenAnyArityTestViewModel.Property3),
            nameof(WhenAnyArityTestViewModel.Property4),
            nameof(WhenAnyArityTestViewModel.Property5),
            nameof(WhenAnyArityTestViewModel.Property6),
            (_, _, _, _, _, _) => "x",
            false).ObserveOn(Sequencer.Immediate).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    /// <summary>Verifies the tuple expression-based WhenAnyValue overload for 6 properties.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task WhenAnyValue_6Props_Tuple_Expr()
    {
        var vm = new WhenAnyArityTestViewModel();
        var list = new List<(string?, string?, string?, string?, string?, string?)>();
        vm.WhenAnyValue(
            x => x.Property1,
            x => x.Property2,
            x => x.Property3,
            x => x.Property4,
            x => x.Property5,
            x => x.Property6).ObserveOn(Sequencer.Immediate).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    /// <summary>Verifies the tuple expression-based WhenAnyValue overload for 6 properties with a distinct-until-changed flag.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task WhenAnyValue_6Props_Tuple_Expr_Dist()
    {
        var vm = new WhenAnyArityTestViewModel();
        var list = new List<(string?, string?, string?, string?, string?, string?)>();
        vm.WhenAnyValue(
            x => x.Property1,
            x => x.Property2,
            x => x.Property3,
            x => x.Property4,
            x => x.Property5,
            x => x.Property6,
            true).ObserveOn(Sequencer.Immediate).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    /// <summary>Verifies the tuple string-based WhenAnyValue overload for 6 properties.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task WhenAnyValue_6Props_Tuple_Str()
    {
        var vm = new WhenAnyArityTestViewModel();
        var list = new List<(string?, string?, string?, string?, string?, string?)>();
        vm.WhenAnyValue<WhenAnyArityTestViewModel, string?, string?, string?, string?, string?, string?>(
            nameof(WhenAnyArityTestViewModel.Property1),
            nameof(WhenAnyArityTestViewModel.Property2),
            nameof(WhenAnyArityTestViewModel.Property3),
            nameof(WhenAnyArityTestViewModel.Property4),
            nameof(WhenAnyArityTestViewModel.Property5),
            nameof(WhenAnyArityTestViewModel.Property6)).ObserveOn(Sequencer.Immediate).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    /// <summary>Verifies the tuple string-based WhenAnyValue overload for 6 properties with a distinct-until-changed flag.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task WhenAnyValue_6Props_Tuple_Str_Dist()
    {
        var vm = new WhenAnyArityTestViewModel();
        var list = new List<(string?, string?, string?, string?, string?, string?)>();
        vm.WhenAnyValue<WhenAnyArityTestViewModel, string?, string?, string?, string?, string?, string?>(
            nameof(WhenAnyArityTestViewModel.Property1),
            nameof(WhenAnyArityTestViewModel.Property2),
            nameof(WhenAnyArityTestViewModel.Property3),
            nameof(WhenAnyArityTestViewModel.Property4),
            nameof(WhenAnyArityTestViewModel.Property5),
            nameof(WhenAnyArityTestViewModel.Property6),
            false).ObserveOn(Sequencer.Immediate).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }
}
