// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests;

/// <summary>Tests for WhenAnyDynamic methods in VariadicTemplates.cs.</summary>
public partial class WhenAnyDynamicTest
{
    /// <summary>Verifies the WhenAnyDynamic overload for 8 properties with a selector.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task WhenAnyDynamic_8Props_Selector()
    {
        var vm = new TestViewModel();
        var param = System.Linq.Expressions.Expression.Parameter(typeof(TestViewModel), "x");
        var property1 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property1));
        var property2 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property2));
        var property3 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property3));
        var property4 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property4));
        var property5 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property5));
        var property6 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property6));
        var property7 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property7));
        var property8 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property8));
        var list = new List<string>();
        _ = vm.WhenAnyDynamic(
            property1,
            property2,
            property3,
            property4,
            property5,
            property6,
            property7,
            property8,
            static (_, _, _, _, _, _, _, _) => "x").ObserveOn(Sequencer.Immediate).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
        vm.Property1 = "a";
        await Assert.That(list).Count().IsGreaterThan(1);
    }

    /// <summary>Verifies the WhenAnyDynamic overload for 8 properties with a selector and distinct-until-changed enabled.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task WhenAnyDynamic_8Props_Selector_Distinct()
    {
        var vm = new TestViewModel();
        var param = System.Linq.Expressions.Expression.Parameter(typeof(TestViewModel), "x");
        var property1 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property1));
        var property2 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property2));
        var property3 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property3));
        var property4 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property4));
        var property5 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property5));
        var property6 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property6));
        var property7 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property7));
        var property8 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property8));
        var list = new List<string>();
        _ = vm.WhenAnyDynamic(
            property1,
            property2,
            property3,
            property4,
            property5,
            property6,
            property7,
            property8,
            static (_, _, _, _, _, _, _, _) => "x",
            true).ObserveOn(Sequencer.Immediate).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
        vm.Property1 = "a";
        await Assert.That(list).Count().IsGreaterThan(1);
    }

    /// <summary>Verifies the WhenAnyDynamic overload for 8 properties with a selector and distinct-until-changed disabled.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task WhenAnyDynamic_8Props_Selector_NotDistinct()
    {
        var vm = new TestViewModel();
        var param = System.Linq.Expressions.Expression.Parameter(typeof(TestViewModel), "x");
        var property1 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property1));
        var property2 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property2));
        var property3 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property3));
        var property4 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property4));
        var property5 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property5));
        var property6 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property6));
        var property7 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property7));
        var property8 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property8));
        var list = new List<string>();
        _ = vm.WhenAnyDynamic(
            property1,
            property2,
            property3,
            property4,
            property5,
            property6,
            property7,
            property8,
            static (_, _, _, _, _, _, _, _) => "x",
            false).ObserveOn(Sequencer.Immediate).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
        vm.Property1 = "a";
        await Assert.That(list).Count().IsGreaterThan(1);
    }

    /// <summary>Verifies the WhenAnyDynamic overload for 9 properties with a selector.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task WhenAnyDynamic_9Props_Selector()
    {
        var vm = new TestViewModel();
        var param = System.Linq.Expressions.Expression.Parameter(typeof(TestViewModel), "x");
        var property1 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property1));
        var property2 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property2));
        var property3 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property3));
        var property4 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property4));
        var property5 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property5));
        var property6 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property6));
        var property7 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property7));
        var property8 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property8));
        var property9 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property9));
        var list = new List<string>();
        _ = vm.WhenAnyDynamic(
            property1,
            property2,
            property3,
            property4,
            property5,
            property6,
            property7,
            property8,
            property9,
            static (_, _, _, _, _, _, _, _, _) => "x").ObserveOn(Sequencer.Immediate).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
        vm.Property1 = "a";
        await Assert.That(list).Count().IsGreaterThan(1);
    }

    /// <summary>Verifies the WhenAnyDynamic overload for 9 properties with a selector and distinct-until-changed enabled.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task WhenAnyDynamic_9Props_Selector_Distinct()
    {
        var vm = new TestViewModel();
        var param = System.Linq.Expressions.Expression.Parameter(typeof(TestViewModel), "x");
        var property1 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property1));
        var property2 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property2));
        var property3 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property3));
        var property4 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property4));
        var property5 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property5));
        var property6 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property6));
        var property7 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property7));
        var property8 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property8));
        var property9 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property9));
        var list = new List<string>();
        _ = vm.WhenAnyDynamic(
            property1,
            property2,
            property3,
            property4,
            property5,
            property6,
            property7,
            property8,
            property9,
            static (_, _, _, _, _, _, _, _, _) => "x",
            true).ObserveOn(Sequencer.Immediate).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
        vm.Property1 = "a";
        await Assert.That(list).Count().IsGreaterThan(1);
    }

    /// <summary>Verifies the WhenAnyDynamic overload for 9 properties with a selector and distinct-until-changed disabled.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task WhenAnyDynamic_9Props_Selector_NotDistinct()
    {
        var vm = new TestViewModel();
        var param = System.Linq.Expressions.Expression.Parameter(typeof(TestViewModel), "x");
        var property1 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property1));
        var property2 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property2));
        var property3 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property3));
        var property4 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property4));
        var property5 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property5));
        var property6 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property6));
        var property7 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property7));
        var property8 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property8));
        var property9 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property9));
        var list = new List<string>();
        _ = vm.WhenAnyDynamic(
            property1,
            property2,
            property3,
            property4,
            property5,
            property6,
            property7,
            property8,
            property9,
            static (_, _, _, _, _, _, _, _, _) => "x",
            false).ObserveOn(Sequencer.Immediate).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
        vm.Property1 = "a";
        await Assert.That(list).Count().IsGreaterThan(1);
    }

    /// <summary>Verifies the WhenAnyDynamic overload for 10 properties with a selector.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task WhenAnyDynamic_10Props_Selector()
    {
        var vm = new TestViewModel();
        var param = System.Linq.Expressions.Expression.Parameter(typeof(TestViewModel), "x");
        var property1 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property1));
        var property2 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property2));
        var property3 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property3));
        var property4 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property4));
        var property5 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property5));
        var property6 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property6));
        var property7 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property7));
        var property8 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property8));
        var property9 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property9));
        var property10 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property10));
        var list = new List<string>();
        _ = vm.WhenAnyDynamic(
            property1,
            property2,
            property3,
            property4,
            property5,
            property6,
            property7,
            property8,
            property9,
            property10,
            static (_, _, _, _, _, _, _, _, _, _) => "x").ObserveOn(Sequencer.Immediate).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
        vm.Property1 = "a";
        await Assert.That(list).Count().IsGreaterThan(1);
    }

    /// <summary>Verifies the WhenAnyDynamic overload for 10 properties with a selector and distinct-until-changed enabled.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task WhenAnyDynamic_10Props_Selector_Distinct()
    {
        var vm = new TestViewModel();
        var param = System.Linq.Expressions.Expression.Parameter(typeof(TestViewModel), "x");
        var property1 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property1));
        var property2 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property2));
        var property3 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property3));
        var property4 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property4));
        var property5 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property5));
        var property6 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property6));
        var property7 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property7));
        var property8 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property8));
        var property9 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property9));
        var property10 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property10));
        var list = new List<string>();
        _ = vm.WhenAnyDynamic(
            property1,
            property2,
            property3,
            property4,
            property5,
            property6,
            property7,
            property8,
            property9,
            property10,
            static (_, _, _, _, _, _, _, _, _, _) => "x",
            true).ObserveOn(Sequencer.Immediate).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
        vm.Property1 = "a";
        await Assert.That(list).Count().IsGreaterThan(1);
    }

    /// <summary>Verifies the WhenAnyDynamic overload for 10 properties with a selector and distinct-until-changed disabled.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task WhenAnyDynamic_10Props_Selector_NotDistinct()
    {
        var vm = new TestViewModel();
        var param = System.Linq.Expressions.Expression.Parameter(typeof(TestViewModel), "x");
        var property1 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property1));
        var property2 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property2));
        var property3 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property3));
        var property4 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property4));
        var property5 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property5));
        var property6 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property6));
        var property7 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property7));
        var property8 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property8));
        var property9 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property9));
        var property10 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property10));
        var list = new List<string>();
        _ = vm.WhenAnyDynamic(
            property1,
            property2,
            property3,
            property4,
            property5,
            property6,
            property7,
            property8,
            property9,
            property10,
            static (_, _, _, _, _, _, _, _, _, _) => "x",
            false).ObserveOn(Sequencer.Immediate).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
        vm.Property1 = "a";
        await Assert.That(list).Count().IsGreaterThan(1);
    }

    /// <summary>Verifies the WhenAnyDynamic overload for 11 properties with a selector.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task WhenAnyDynamic_11Props_Selector()
    {
        var vm = new TestViewModel();
        var param = System.Linq.Expressions.Expression.Parameter(typeof(TestViewModel), "x");
        var property1 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property1));
        var property2 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property2));
        var property3 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property3));
        var property4 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property4));
        var property5 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property5));
        var property6 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property6));
        var property7 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property7));
        var property8 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property8));
        var property9 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property9));
        var property10 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property10));
        var property11 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property11));
        var list = new List<string>();
        _ = vm.WhenAnyDynamic(
            property1,
            property2,
            property3,
            property4,
            property5,
            property6,
            property7,
            property8,
            property9,
            property10,
            property11,
            static (_, _, _, _, _, _, _, _, _, _, _) => "x").ObserveOn(Sequencer.Immediate).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
        vm.Property1 = "a";
        await Assert.That(list).Count().IsGreaterThan(1);
    }

    /// <summary>Verifies the WhenAnyDynamic overload for 11 properties with a selector and distinct-until-changed enabled.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task WhenAnyDynamic_11Props_Selector_Distinct()
    {
        var vm = new TestViewModel();
        var param = System.Linq.Expressions.Expression.Parameter(typeof(TestViewModel), "x");
        var property1 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property1));
        var property2 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property2));
        var property3 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property3));
        var property4 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property4));
        var property5 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property5));
        var property6 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property6));
        var property7 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property7));
        var property8 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property8));
        var property9 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property9));
        var property10 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property10));
        var property11 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property11));
        var list = new List<string>();
        _ = vm.WhenAnyDynamic(
            property1,
            property2,
            property3,
            property4,
            property5,
            property6,
            property7,
            property8,
            property9,
            property10,
            property11,
            static (_, _, _, _, _, _, _, _, _, _, _) => "x",
            true).ObserveOn(Sequencer.Immediate).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
        vm.Property1 = "a";
        await Assert.That(list).Count().IsGreaterThan(1);
    }

    /// <summary>Verifies the WhenAnyDynamic overload for 11 properties with a selector and distinct-until-changed disabled.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task WhenAnyDynamic_11Props_Selector_NotDistinct()
    {
        var vm = new TestViewModel();
        var param = System.Linq.Expressions.Expression.Parameter(typeof(TestViewModel), "x");
        var property1 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property1));
        var property2 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property2));
        var property3 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property3));
        var property4 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property4));
        var property5 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property5));
        var property6 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property6));
        var property7 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property7));
        var property8 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property8));
        var property9 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property9));
        var property10 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property10));
        var property11 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property11));
        var list = new List<string>();
        _ = vm.WhenAnyDynamic(
            property1,
            property2,
            property3,
            property4,
            property5,
            property6,
            property7,
            property8,
            property9,
            property10,
            property11,
            static (_, _, _, _, _, _, _, _, _, _, _) => "x",
            false).ObserveOn(Sequencer.Immediate).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
        vm.Property1 = "a";
        await Assert.That(list).Count().IsGreaterThan(1);
    }

    /// <summary>Verifies the WhenAnyDynamic overload for 12 properties with a selector.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task WhenAnyDynamic_12Props_Selector()
    {
        var vm = new TestViewModel();
        var param = System.Linq.Expressions.Expression.Parameter(typeof(TestViewModel), "x");
        var property1 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property1));
        var property2 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property2));
        var property3 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property3));
        var property4 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property4));
        var property5 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property5));
        var property6 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property6));
        var property7 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property7));
        var property8 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property8));
        var property9 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property9));
        var property10 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property10));
        var property11 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property11));
        var property12 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property12));
        var list = new List<string>();
        _ = vm.WhenAnyDynamic(
            property1,
            property2,
            property3,
            property4,
            property5,
            property6,
            property7,
            property8,
            property9,
            property10,
            property11,
            property12,
            static (_, _, _, _, _, _, _, _, _, _, _, _) => "x").ObserveOn(Sequencer.Immediate).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
        vm.Property1 = "a";
        await Assert.That(list).Count().IsGreaterThan(1);
    }

    /// <summary>Verifies the WhenAnyDynamic overload for 12 properties with a selector and distinct-until-changed enabled.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task WhenAnyDynamic_12Props_Selector_Distinct()
    {
        var vm = new TestViewModel();
        var param = System.Linq.Expressions.Expression.Parameter(typeof(TestViewModel), "x");
        var property1 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property1));
        var property2 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property2));
        var property3 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property3));
        var property4 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property4));
        var property5 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property5));
        var property6 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property6));
        var property7 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property7));
        var property8 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property8));
        var property9 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property9));
        var property10 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property10));
        var property11 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property11));
        var property12 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property12));
        var list = new List<string>();
        _ = vm.WhenAnyDynamic(
            property1,
            property2,
            property3,
            property4,
            property5,
            property6,
            property7,
            property8,
            property9,
            property10,
            property11,
            property12,
            static (_, _, _, _, _, _, _, _, _, _, _, _) => "x",
            true).ObserveOn(Sequencer.Immediate).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
        vm.Property1 = "a";
        await Assert.That(list).Count().IsGreaterThan(1);
    }

    /// <summary>Verifies the WhenAnyDynamic overload for 12 properties with a selector and distinct-until-changed disabled.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task WhenAnyDynamic_12Props_Selector_NotDistinct()
    {
        var vm = new TestViewModel();
        var param = System.Linq.Expressions.Expression.Parameter(typeof(TestViewModel), "x");
        var property1 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property1));
        var property2 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property2));
        var property3 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property3));
        var property4 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property4));
        var property5 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property5));
        var property6 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property6));
        var property7 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property7));
        var property8 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property8));
        var property9 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property9));
        var property10 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property10));
        var property11 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property11));
        var property12 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property12));
        var list = new List<string>();
        _ = vm.WhenAnyDynamic(
            property1,
            property2,
            property3,
            property4,
            property5,
            property6,
            property7,
            property8,
            property9,
            property10,
            property11,
            property12,
            static (_, _, _, _, _, _, _, _, _, _, _, _) => "x",
            false).ObserveOn(Sequencer.Immediate).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
        vm.Property1 = "a";
        await Assert.That(list).Count().IsGreaterThan(1);
    }
}
