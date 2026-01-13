// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests;

/// <summary>
///     Tests for WhenAnyDynamic methods in VariadicTemplates.cs.
/// </summary>
public class WhenAnyDynamicTest
{
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
        vm.WhenAnyDynamic(
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
                (c1, c2, c3, c4, c5, c6, c7, c8, c9, c10) => "x").ObserveOn(ImmediateScheduler.Instance)
            .Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
        vm.Property1 = "a";
        await Assert.That(list).Count().IsGreaterThan(1);
    }

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
        vm.WhenAnyDynamic(
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
            (c1, c2, c3, c4, c5, c6, c7, c8, c9, c10) => "x",
            true).ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
        vm.Property1 = "a";
        await Assert.That(list).Count().IsGreaterThan(1);
    }

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
        vm.WhenAnyDynamic(
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
            (c1, c2, c3, c4, c5, c6, c7, c8, c9, c10) => "x",
            false).ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
        vm.Property1 = "a";
        await Assert.That(list).Count().IsGreaterThan(1);
    }

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
        vm.WhenAnyDynamic(
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
                (c1, c2, c3, c4, c5, c6, c7, c8, c9, c10, c11) => "x").ObserveOn(ImmediateScheduler.Instance)
            .Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
        vm.Property1 = "a";
        await Assert.That(list).Count().IsGreaterThan(1);
    }

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
        vm.WhenAnyDynamic(
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
            (c1, c2, c3, c4, c5, c6, c7, c8, c9, c10, c11) => "x",
            true).ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
        vm.Property1 = "a";
        await Assert.That(list).Count().IsGreaterThan(1);
    }

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
        vm.WhenAnyDynamic(
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
            (c1, c2, c3, c4, c5, c6, c7, c8, c9, c10, c11) => "x",
            false).ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
        vm.Property1 = "a";
        await Assert.That(list).Count().IsGreaterThan(1);
    }

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
        vm.WhenAnyDynamic(
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
                (c1, c2, c3, c4, c5, c6, c7, c8, c9, c10, c11, c12) => "x").ObserveOn(ImmediateScheduler.Instance)
            .Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
        vm.Property1 = "a";
        await Assert.That(list).Count().IsGreaterThan(1);
    }

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
        vm.WhenAnyDynamic(
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
            (c1, c2, c3, c4, c5, c6, c7, c8, c9, c10, c11, c12) => "x",
            true).ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
        vm.Property1 = "a";
        await Assert.That(list).Count().IsGreaterThan(1);
    }

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
        vm.WhenAnyDynamic(
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
            (c1, c2, c3, c4, c5, c6, c7, c8, c9, c10, c11, c12) => "x",
            false).ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
        vm.Property1 = "a";
        await Assert.That(list).Count().IsGreaterThan(1);
    }

    [Test]
    public async Task WhenAnyDynamic_1Props_Selector_Distinct()
    {
        var vm = new TestViewModel();
        var param = System.Linq.Expressions.Expression.Parameter(typeof(TestViewModel), "x");
        var property1 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property1));
        var list = new List<string>();
        vm.WhenAnyDynamic(
            property1,
            c1 => "x",
            true).ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
        vm.Property1 = "a";
        await Assert.That(list).Count().IsGreaterThan(1);
    }

    [Test]
    public async Task WhenAnyDynamic_1Props_Selector_NotDistinct()
    {
        var vm = new TestViewModel();
        var param = System.Linq.Expressions.Expression.Parameter(typeof(TestViewModel), "x");
        var property1 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property1));
        var list = new List<string>();
        vm.WhenAnyDynamic(
            property1,
            c1 => "x",
            false).ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
        vm.Property1 = "a";
        await Assert.That(list).Count().IsGreaterThan(1);
    }

    [Test]
    public async Task WhenAnyDynamic_2Props_Selector()
    {
        var vm = new TestViewModel();
        var param = System.Linq.Expressions.Expression.Parameter(typeof(TestViewModel), "x");
        var property1 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property1));
        var property2 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property2));
        var list = new List<string>();
        vm.WhenAnyDynamic(
            property1,
            property2,
            (c1, c2) => "x").ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
        vm.Property1 = "a";
        await Assert.That(list).Count().IsGreaterThan(1);
    }

    [Test]
    public async Task WhenAnyDynamic_2Props_Selector_Distinct()
    {
        var vm = new TestViewModel();
        var param = System.Linq.Expressions.Expression.Parameter(typeof(TestViewModel), "x");
        var property1 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property1));
        var property2 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property2));
        var list = new List<string>();
        vm.WhenAnyDynamic(
            property1,
            property2,
            (c1, c2) => "x",
            true).ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
        vm.Property1 = "a";
        await Assert.That(list).Count().IsGreaterThan(1);
    }

    [Test]
    public async Task WhenAnyDynamic_2Props_Selector_NotDistinct()
    {
        var vm = new TestViewModel();
        var param = System.Linq.Expressions.Expression.Parameter(typeof(TestViewModel), "x");
        var property1 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property1));
        var property2 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property2));
        var list = new List<string>();
        vm.WhenAnyDynamic(
            property1,
            property2,
            (c1, c2) => "x",
            false).ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
        vm.Property1 = "a";
        await Assert.That(list).Count().IsGreaterThan(1);
    }

    [Test]
    public async Task WhenAnyDynamic_3Props_Selector()
    {
        var vm = new TestViewModel();
        var param = System.Linq.Expressions.Expression.Parameter(typeof(TestViewModel), "x");
        var property1 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property1));
        var property2 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property2));
        var property3 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property3));
        var list = new List<string>();
        vm.WhenAnyDynamic(
            property1,
            property2,
            property3,
            (c1, c2, c3) => "x").ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
        vm.Property1 = "a";
        await Assert.That(list).Count().IsGreaterThan(1);
    }

    [Test]
    public async Task WhenAnyDynamic_3Props_Selector_Distinct()
    {
        var vm = new TestViewModel();
        var param = System.Linq.Expressions.Expression.Parameter(typeof(TestViewModel), "x");
        var property1 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property1));
        var property2 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property2));
        var property3 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property3));
        var list = new List<string>();
        vm.WhenAnyDynamic(
            property1,
            property2,
            property3,
            (c1, c2, c3) => "x",
            true).ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
        vm.Property1 = "a";
        await Assert.That(list).Count().IsGreaterThan(1);
    }

    [Test]
    public async Task WhenAnyDynamic_3Props_Selector_NotDistinct()
    {
        var vm = new TestViewModel();
        var param = System.Linq.Expressions.Expression.Parameter(typeof(TestViewModel), "x");
        var property1 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property1));
        var property2 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property2));
        var property3 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property3));
        var list = new List<string>();
        vm.WhenAnyDynamic(
            property1,
            property2,
            property3,
            (c1, c2, c3) => "x",
            false).ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
        vm.Property1 = "a";
        await Assert.That(list).Count().IsGreaterThan(1);
    }

    [Test]
    public async Task WhenAnyDynamic_4Props_Selector()
    {
        var vm = new TestViewModel();
        var param = System.Linq.Expressions.Expression.Parameter(typeof(TestViewModel), "x");
        var property1 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property1));
        var property2 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property2));
        var property3 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property3));
        var property4 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property4));
        var list = new List<string>();
        vm.WhenAnyDynamic(
            property1,
            property2,
            property3,
            property4,
            (c1, c2, c3, c4) => "x").ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
        vm.Property1 = "a";
        await Assert.That(list).Count().IsGreaterThan(1);
    }

    [Test]
    public async Task WhenAnyDynamic_4Props_Selector_Distinct()
    {
        var vm = new TestViewModel();
        var param = System.Linq.Expressions.Expression.Parameter(typeof(TestViewModel), "x");
        var property1 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property1));
        var property2 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property2));
        var property3 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property3));
        var property4 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property4));
        var list = new List<string>();
        vm.WhenAnyDynamic(
            property1,
            property2,
            property3,
            property4,
            (c1, c2, c3, c4) => "x",
            true).ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
        vm.Property1 = "a";
        await Assert.That(list).Count().IsGreaterThan(1);
    }

    [Test]
    public async Task WhenAnyDynamic_4Props_Selector_NotDistinct()
    {
        var vm = new TestViewModel();
        var param = System.Linq.Expressions.Expression.Parameter(typeof(TestViewModel), "x");
        var property1 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property1));
        var property2 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property2));
        var property3 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property3));
        var property4 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property4));
        var list = new List<string>();
        vm.WhenAnyDynamic(
            property1,
            property2,
            property3,
            property4,
            (c1, c2, c3, c4) => "x",
            false).ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
        vm.Property1 = "a";
        await Assert.That(list).Count().IsGreaterThan(1);
    }

    [Test]
    public async Task WhenAnyDynamic_5Props_Selector()
    {
        var vm = new TestViewModel();
        var param = System.Linq.Expressions.Expression.Parameter(typeof(TestViewModel), "x");
        var property1 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property1));
        var property2 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property2));
        var property3 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property3));
        var property4 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property4));
        var property5 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property5));
        var list = new List<string>();
        vm.WhenAnyDynamic(
            property1,
            property2,
            property3,
            property4,
            property5,
            (c1, c2, c3, c4, c5) => "x").ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
        vm.Property1 = "a";
        await Assert.That(list).Count().IsGreaterThan(1);
    }

    [Test]
    public async Task WhenAnyDynamic_5Props_Selector_Distinct()
    {
        var vm = new TestViewModel();
        var param = System.Linq.Expressions.Expression.Parameter(typeof(TestViewModel), "x");
        var property1 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property1));
        var property2 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property2));
        var property3 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property3));
        var property4 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property4));
        var property5 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property5));
        var list = new List<string>();
        vm.WhenAnyDynamic(
            property1,
            property2,
            property3,
            property4,
            property5,
            (c1, c2, c3, c4, c5) => "x",
            true).ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
        vm.Property1 = "a";
        await Assert.That(list).Count().IsGreaterThan(1);
    }

    [Test]
    public async Task WhenAnyDynamic_5Props_Selector_NotDistinct()
    {
        var vm = new TestViewModel();
        var param = System.Linq.Expressions.Expression.Parameter(typeof(TestViewModel), "x");
        var property1 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property1));
        var property2 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property2));
        var property3 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property3));
        var property4 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property4));
        var property5 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property5));
        var list = new List<string>();
        vm.WhenAnyDynamic(
            property1,
            property2,
            property3,
            property4,
            property5,
            (c1, c2, c3, c4, c5) => "x",
            false).ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
        vm.Property1 = "a";
        await Assert.That(list).Count().IsGreaterThan(1);
    }

    [Test]
    public async Task WhenAnyDynamic_6Props_Selector()
    {
        var vm = new TestViewModel();
        var param = System.Linq.Expressions.Expression.Parameter(typeof(TestViewModel), "x");
        var property1 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property1));
        var property2 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property2));
        var property3 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property3));
        var property4 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property4));
        var property5 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property5));
        var property6 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property6));
        var list = new List<string>();
        vm.WhenAnyDynamic(
            property1,
            property2,
            property3,
            property4,
            property5,
            property6,
            (c1, c2, c3, c4, c5, c6) => "x").ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
        vm.Property1 = "a";
        await Assert.That(list).Count().IsGreaterThan(1);
    }

    [Test]
    public async Task WhenAnyDynamic_6Props_Selector_Distinct()
    {
        var vm = new TestViewModel();
        var param = System.Linq.Expressions.Expression.Parameter(typeof(TestViewModel), "x");
        var property1 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property1));
        var property2 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property2));
        var property3 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property3));
        var property4 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property4));
        var property5 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property5));
        var property6 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property6));
        var list = new List<string>();
        vm.WhenAnyDynamic(
            property1,
            property2,
            property3,
            property4,
            property5,
            property6,
            (c1, c2, c3, c4, c5, c6) => "x",
            true).ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
        vm.Property1 = "a";
        await Assert.That(list).Count().IsGreaterThan(1);
    }

    [Test]
    public async Task WhenAnyDynamic_6Props_Selector_NotDistinct()
    {
        var vm = new TestViewModel();
        var param = System.Linq.Expressions.Expression.Parameter(typeof(TestViewModel), "x");
        var property1 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property1));
        var property2 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property2));
        var property3 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property3));
        var property4 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property4));
        var property5 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property5));
        var property6 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property6));
        var list = new List<string>();
        vm.WhenAnyDynamic(
            property1,
            property2,
            property3,
            property4,
            property5,
            property6,
            (c1, c2, c3, c4, c5, c6) => "x",
            false).ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
        vm.Property1 = "a";
        await Assert.That(list).Count().IsGreaterThan(1);
    }

    [Test]
    public async Task WhenAnyDynamic_7Props_Selector()
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
        var list = new List<string>();
        vm.WhenAnyDynamic(
            property1,
            property2,
            property3,
            property4,
            property5,
            property6,
            property7,
            (c1, c2, c3, c4, c5, c6, c7) => "x").ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
        vm.Property1 = "a";
        await Assert.That(list).Count().IsGreaterThan(1);
    }

    [Test]
    public async Task WhenAnyDynamic_7Props_Selector_Distinct()
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
        var list = new List<string>();
        vm.WhenAnyDynamic(
            property1,
            property2,
            property3,
            property4,
            property5,
            property6,
            property7,
            (c1, c2, c3, c4, c5, c6, c7) => "x",
            true).ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
        vm.Property1 = "a";
        await Assert.That(list).Count().IsGreaterThan(1);
    }

    [Test]
    public async Task WhenAnyDynamic_7Props_Selector_NotDistinct()
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
        var list = new List<string>();
        vm.WhenAnyDynamic(
            property1,
            property2,
            property3,
            property4,
            property5,
            property6,
            property7,
            (c1, c2, c3, c4, c5, c6, c7) => "x",
            false).ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
        vm.Property1 = "a";
        await Assert.That(list).Count().IsGreaterThan(1);
    }

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
        vm.WhenAnyDynamic(
            property1,
            property2,
            property3,
            property4,
            property5,
            property6,
            property7,
            property8,
            (c1, c2, c3, c4, c5, c6, c7, c8) => "x").ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
        vm.Property1 = "a";
        await Assert.That(list).Count().IsGreaterThan(1);
    }

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
        vm.WhenAnyDynamic(
            property1,
            property2,
            property3,
            property4,
            property5,
            property6,
            property7,
            property8,
            (c1, c2, c3, c4, c5, c6, c7, c8) => "x",
            true).ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
        vm.Property1 = "a";
        await Assert.That(list).Count().IsGreaterThan(1);
    }

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
        vm.WhenAnyDynamic(
            property1,
            property2,
            property3,
            property4,
            property5,
            property6,
            property7,
            property8,
            (c1, c2, c3, c4, c5, c6, c7, c8) => "x",
            false).ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
        vm.Property1 = "a";
        await Assert.That(list).Count().IsGreaterThan(1);
    }

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
        vm.WhenAnyDynamic(
            property1,
            property2,
            property3,
            property4,
            property5,
            property6,
            property7,
            property8,
            property9,
            (c1, c2, c3, c4, c5, c6, c7, c8, c9) => "x").ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
        vm.Property1 = "a";
        await Assert.That(list).Count().IsGreaterThan(1);
    }

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
        vm.WhenAnyDynamic(
            property1,
            property2,
            property3,
            property4,
            property5,
            property6,
            property7,
            property8,
            property9,
            (c1, c2, c3, c4, c5, c6, c7, c8, c9) => "x",
            true).ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
        vm.Property1 = "a";
        await Assert.That(list).Count().IsGreaterThan(1);
    }

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
        vm.WhenAnyDynamic(
            property1,
            property2,
            property3,
            property4,
            property5,
            property6,
            property7,
            property8,
            property9,
            (c1, c2, c3, c4, c5, c6, c7, c8, c9) => "x",
            false).ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
        vm.Property1 = "a";
        await Assert.That(list).Count().IsGreaterThan(1);
    }

    private class TestViewModel : ReactiveObject
    {
        private string? _property1;

        private string? _property10;

        private string? _property11;

        private string? _property12;

        private string? _property2;

        private string? _property3;

        private string? _property4;

        private string? _property5;

        private string? _property6;

        private string? _property7;

        private string? _property8;

        private string? _property9;

        public string? Property1
        {
            get => _property1;
            set => this.RaiseAndSetIfChanged(ref _property1, value);
        }

        public string? Property10
        {
            get => _property10;
            set => this.RaiseAndSetIfChanged(ref _property10, value);
        }

        public string? Property11
        {
            get => _property11;
            set => this.RaiseAndSetIfChanged(ref _property11, value);
        }

        public string? Property12
        {
            get => _property12;
            set => this.RaiseAndSetIfChanged(ref _property12, value);
        }

        public string? Property2
        {
            get => _property2;
            set => this.RaiseAndSetIfChanged(ref _property2, value);
        }

        public string? Property3
        {
            get => _property3;
            set => this.RaiseAndSetIfChanged(ref _property3, value);
        }

        public string? Property4
        {
            get => _property4;
            set => this.RaiseAndSetIfChanged(ref _property4, value);
        }

        public string? Property5
        {
            get => _property5;
            set => this.RaiseAndSetIfChanged(ref _property5, value);
        }

        public string? Property6
        {
            get => _property6;
            set => this.RaiseAndSetIfChanged(ref _property6, value);
        }

        public string? Property7
        {
            get => _property7;
            set => this.RaiseAndSetIfChanged(ref _property7, value);
        }

        public string? Property8
        {
            get => _property8;
            set => this.RaiseAndSetIfChanged(ref _property8, value);
        }

        public string? Property9
        {
            get => _property9;
            set => this.RaiseAndSetIfChanged(ref _property9, value);
        }
    }
}
