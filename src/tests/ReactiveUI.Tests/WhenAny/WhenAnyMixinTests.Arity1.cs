// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.WhenAny;

/// <summary>
/// Tests for the WhenAny and WhenAnyValue mixin overloads.
/// </summary>
public partial class WhenAnyMixinTests
{
    /// <summary>
    ///     Verifies the WhenAny overload for 1 properties with a selector.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task WhenAny_1Props_Sel()
    {
        var vm = new WhenAnyArityTestViewModel();
        var list = new List<string>();
        vm.WhenAny(
            x => x.Property1,
            _ => "x").ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    /// <summary>
    ///     Verifies the WhenAny overload for 1 properties with a selector and distinct-until-changed flag.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task WhenAny_1Props_Sel_Dist()
    {
        var vm = new WhenAnyArityTestViewModel();
        var list = new List<string>();
        vm.WhenAny(
            x => x.Property1,
            _ => "x",
            true).ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    /// <summary>
    ///     Verifies the string-based WhenAny overload for 1 properties with a selector.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task WhenAny_1Props_Sel_Str()
    {
        var vm = new WhenAnyArityTestViewModel();
        var list = new List<string>();
        vm.WhenAny<WhenAnyArityTestViewModel, string, string?>(
            nameof(WhenAnyArityTestViewModel.Property1),
            _ => "x").ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    /// <summary>
    ///     Verifies the string-based WhenAny overload for 1 properties with a selector and distinct-until-changed flag.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task WhenAny_1Props_Sel_Str_Dist()
    {
        var vm = new WhenAnyArityTestViewModel();
        var list = new List<string>();
        vm.WhenAny<WhenAnyArityTestViewModel, string, string?>(
            nameof(WhenAnyArityTestViewModel.Property1),
            _ => "x",
            false).ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    /// <summary>
    ///     Verifies the expression-based WhenAnyValue overload for 1 property.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task WhenAnyValue_1Prop_Expr()
    {
        var vm = new WhenAnyArityTestViewModel();
        var list = new List<string?>();
        vm.WhenAnyValue(x => x.Property1).ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
        vm.Property1 = "a";
        await Assert.That(list).Count().IsGreaterThan(1);
    }

    /// <summary>
    ///     Verifies the expression-based WhenAnyValue overload for 1 property with a distinct-until-changed flag.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task WhenAnyValue_1Prop_Expr_Dist()
    {
        var vm = new WhenAnyArityTestViewModel();
        var list = new List<string?>();
        vm.WhenAnyValue(x => x.Property1, true).ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    /// <summary>
    ///     Verifies the string-based WhenAnyValue overload for 1 property.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task WhenAnyValue_1Prop_Str()
    {
        var vm = new WhenAnyArityTestViewModel();
        var list = new List<string?>();
        vm.WhenAnyValue<WhenAnyArityTestViewModel, string?>(nameof(WhenAnyArityTestViewModel.Property1)).ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    /// <summary>
    ///     Verifies the string-based WhenAnyValue overload for 1 property with a distinct-until-changed flag.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task WhenAnyValue_1Prop_Str_Dist()
    {
        var vm = new WhenAnyArityTestViewModel();
        var list = new List<string?>();
        vm.WhenAnyValue<WhenAnyArityTestViewModel, string?>(nameof(WhenAnyArityTestViewModel.Property1), false).ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    /// <summary>
    ///     Verifies the WhenAnyValue overload for 1 property with a selector.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task WhenAnyValue_1Props_Sel()
    {
        var vm = new WhenAnyArityTestViewModel();
        var list = new List<string>();
        Func<string?, string> selector = _ => "x";
        vm.WhenAnyValue(x => x.Property1, selector).ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    /// <summary>
    ///     Verifies the WhenAnyValue overload for 1 property with a selector and distinct-until-changed flag.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task WhenAnyValue_1Props_Sel_Dist()
    {
        var vm = new WhenAnyArityTestViewModel();
        var list = new List<string>();
        Func<string?, string> selector = _ => "x";
        vm.WhenAnyValue(x => x.Property1, selector, true).ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    /// <summary>
    ///     Verifies the string-based WhenAnyValue overload for 1 properties with a selector.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task WhenAnyValue_1Props_Sel_Str()
    {
        var vm = new WhenAnyArityTestViewModel();
        var list = new List<string>();
        vm.WhenAnyValue<WhenAnyArityTestViewModel, string, string?>(
            nameof(WhenAnyArityTestViewModel.Property1),
            _ => "x").ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    /// <summary>
    ///     Verifies the string-based WhenAnyValue overload for 1 properties with a selector and distinct-until-changed flag.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task WhenAnyValue_1Props_Sel_Str_Dist()
    {
        var vm = new WhenAnyArityTestViewModel();
        var list = new List<string>();
        vm.WhenAnyValue<WhenAnyArityTestViewModel, string, string?>(
            nameof(WhenAnyArityTestViewModel.Property1),
            _ => "x",
            false).ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }
}
