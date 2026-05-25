// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using System.Reactive.Concurrency;
using System.Reactive.Linq;

namespace ReactiveUI.Tests.WhenAny;

/// <content>
/// Arity-12 WhenAny and WhenAnyValue overload tests.
/// </content>
[SuppressMessage(
    "Major Code Smell",
    "S107:Methods should not have too many parameters",
    Justification = "Arity-12 variadic selectors intentionally accept more than seven parameters.")]
public partial class WhenAnyMixinTests
{
    /// <summary>
    ///     Verifies the WhenAny overload for 12 properties with a selector.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task WhenAny_12Props_Sel()
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
            x => x.Property7,
            x => x.Property8,
            x => x.Property9,
            x => x.Property10,
            x => x.Property11,
            x => x.Property12,
            (_, _, _, _, _, _, _, _, _, _, _, _) => "x").ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    /// <summary>
    ///     Verifies the WhenAny overload for 12 properties with a selector and distinct-until-changed flag.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task WhenAny_12Props_Sel_Dist()
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
            x => x.Property7,
            x => x.Property8,
            x => x.Property9,
            x => x.Property10,
            x => x.Property11,
            x => x.Property12,
            (_, _, _, _, _, _, _, _, _, _, _, _) => "x",
            true).ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    /// <summary>
    ///     Verifies the string-based WhenAny overload for 12 properties with a selector.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task WhenAny_12Props_Sel_Str()
    {
        var vm = new WhenAnyArityTestViewModel();
        var list = new List<string>();
        vm.WhenAny<WhenAnyArityTestViewModel, string, string?, string?, string?, string?, string?, string?, string?, string?, string?, string?, string?, string?>(
            nameof(WhenAnyArityTestViewModel.Property1),
            nameof(WhenAnyArityTestViewModel.Property2),
            nameof(WhenAnyArityTestViewModel.Property3),
            nameof(WhenAnyArityTestViewModel.Property4),
            nameof(WhenAnyArityTestViewModel.Property5),
            nameof(WhenAnyArityTestViewModel.Property6),
            nameof(WhenAnyArityTestViewModel.Property7),
            nameof(WhenAnyArityTestViewModel.Property8),
            nameof(WhenAnyArityTestViewModel.Property9),
            nameof(WhenAnyArityTestViewModel.Property10),
            nameof(WhenAnyArityTestViewModel.Property11),
            nameof(WhenAnyArityTestViewModel.Property12),
            (_, _, _, _, _, _, _, _, _, _, _, _) => "x").ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    /// <summary>
    ///     Verifies the string-based WhenAny overload for 12 properties with a selector and distinct-until-changed flag.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task WhenAny_12Props_Sel_Str_Dist()
    {
        var vm = new WhenAnyArityTestViewModel();
        var list = new List<string>();
        vm.WhenAny<WhenAnyArityTestViewModel, string, string?, string?, string?, string?, string?, string?, string?, string?, string?, string?, string?, string?>(
            nameof(WhenAnyArityTestViewModel.Property1),
            nameof(WhenAnyArityTestViewModel.Property2),
            nameof(WhenAnyArityTestViewModel.Property3),
            nameof(WhenAnyArityTestViewModel.Property4),
            nameof(WhenAnyArityTestViewModel.Property5),
            nameof(WhenAnyArityTestViewModel.Property6),
            nameof(WhenAnyArityTestViewModel.Property7),
            nameof(WhenAnyArityTestViewModel.Property8),
            nameof(WhenAnyArityTestViewModel.Property9),
            nameof(WhenAnyArityTestViewModel.Property10),
            nameof(WhenAnyArityTestViewModel.Property11),
            nameof(WhenAnyArityTestViewModel.Property12),
            (_, _, _, _, _, _, _, _, _, _, _, _) => "x",
            false).ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    /// <summary>
    ///     Verifies the WhenAnyValue overload for 12 properties with a selector.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task WhenAnyValue_12Props_Sel()
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
            x => x.Property7,
            x => x.Property8,
            x => x.Property9,
            x => x.Property10,
            x => x.Property11,
            x => x.Property12,
            (_, _, _, _, _, _, _, _, _, _, _, _) => "x").ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    /// <summary>
    ///     Verifies the WhenAnyValue overload for 12 properties with a selector and distinct-until-changed flag.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task WhenAnyValue_12Props_Sel_Dist()
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
            x => x.Property7,
            x => x.Property8,
            x => x.Property9,
            x => x.Property10,
            x => x.Property11,
            x => x.Property12,
            (_, _, _, _, _, _, _, _, _, _, _, _) => "x",
            true).ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    /// <summary>
    ///     Verifies the string-based WhenAnyValue overload for 12 properties with a selector.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task WhenAnyValue_12Props_Sel_Str()
    {
        var vm = new WhenAnyArityTestViewModel();
        var list = new List<string>();
        vm.WhenAnyValue<WhenAnyArityTestViewModel, string, string?, string?, string?, string?, string?, string?, string?, string?, string?, string?, string?, string?>(
            nameof(WhenAnyArityTestViewModel.Property1),
            nameof(WhenAnyArityTestViewModel.Property2),
            nameof(WhenAnyArityTestViewModel.Property3),
            nameof(WhenAnyArityTestViewModel.Property4),
            nameof(WhenAnyArityTestViewModel.Property5),
            nameof(WhenAnyArityTestViewModel.Property6),
            nameof(WhenAnyArityTestViewModel.Property7),
            nameof(WhenAnyArityTestViewModel.Property8),
            nameof(WhenAnyArityTestViewModel.Property9),
            nameof(WhenAnyArityTestViewModel.Property10),
            nameof(WhenAnyArityTestViewModel.Property11),
            nameof(WhenAnyArityTestViewModel.Property12),
            (_, _, _, _, _, _, _, _, _, _, _, _) => "x").ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    /// <summary>
    ///     Verifies the string-based WhenAnyValue overload for 12 properties with a selector and distinct-until-changed flag.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task WhenAnyValue_12Props_Sel_Str_Dist()
    {
        var vm = new WhenAnyArityTestViewModel();
        var list = new List<string>();
        vm.WhenAnyValue<WhenAnyArityTestViewModel, string, string?, string?, string?, string?, string?, string?, string?, string?, string?, string?, string?, string?>(
            nameof(WhenAnyArityTestViewModel.Property1),
            nameof(WhenAnyArityTestViewModel.Property2),
            nameof(WhenAnyArityTestViewModel.Property3),
            nameof(WhenAnyArityTestViewModel.Property4),
            nameof(WhenAnyArityTestViewModel.Property5),
            nameof(WhenAnyArityTestViewModel.Property6),
            nameof(WhenAnyArityTestViewModel.Property7),
            nameof(WhenAnyArityTestViewModel.Property8),
            nameof(WhenAnyArityTestViewModel.Property9),
            nameof(WhenAnyArityTestViewModel.Property10),
            nameof(WhenAnyArityTestViewModel.Property11),
            nameof(WhenAnyArityTestViewModel.Property12),
            (_, _, _, _, _, _, _, _, _, _, _, _) => "x",
            false).ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }
}
