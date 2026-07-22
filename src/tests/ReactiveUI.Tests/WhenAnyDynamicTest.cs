// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests;

/// <summary>Tests for WhenAnyDynamic methods in VariadicTemplates.cs.</summary>
public partial class WhenAnyDynamicTest
{
    /// <summary>Verifies the WhenAnyDynamic overload for 1 property with a selector and distinct-until-changed enabled.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task WhenAnyDynamic_1Props_Selector_Distinct()
    {
        var vm = new TestViewModel();
        var param = System.Linq.Expressions.Expression.Parameter(typeof(TestViewModel), "x");
        var property1 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property1));
        var list = new List<string>();
        _ = vm.WhenAnyDynamic(
            property1,
            static _ => "x",
            true).ObserveOn(Sequencer.Immediate).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
        vm.Property1 = "a";
        await Assert.That(list).Count().IsGreaterThan(1);
    }

    /// <summary>Verifies the WhenAnyDynamic overload for 1 property with a selector and distinct-until-changed disabled.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task WhenAnyDynamic_1Props_Selector_NotDistinct()
    {
        var vm = new TestViewModel();
        var param = System.Linq.Expressions.Expression.Parameter(typeof(TestViewModel), "x");
        var property1 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property1));
        var list = new List<string>();
        _ = vm.WhenAnyDynamic(
            property1,
            static _ => "x",
            false).ObserveOn(Sequencer.Immediate).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
        vm.Property1 = "a";
        await Assert.That(list).Count().IsGreaterThan(1);
    }

    /// <summary>Verifies the WhenAnyDynamic overload for 2 properties with a selector.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task WhenAnyDynamic_2Props_Selector()
    {
        var vm = new TestViewModel();
        var param = System.Linq.Expressions.Expression.Parameter(typeof(TestViewModel), "x");
        var property1 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property1));
        var property2 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property2));
        var list = new List<string>();
        _ = vm.WhenAnyDynamic(
            property1,
            property2,
            static (_, _) => "x").ObserveOn(Sequencer.Immediate).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
        vm.Property1 = "a";
        await Assert.That(list).Count().IsGreaterThan(1);
    }

    /// <summary>Verifies the WhenAnyDynamic overload for 2 properties with a selector and distinct-until-changed enabled.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task WhenAnyDynamic_2Props_Selector_Distinct()
    {
        var vm = new TestViewModel();
        var param = System.Linq.Expressions.Expression.Parameter(typeof(TestViewModel), "x");
        var property1 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property1));
        var property2 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property2));
        var list = new List<string>();
        _ = vm.WhenAnyDynamic(
            property1,
            property2,
            static (_, _) => "x",
            true).ObserveOn(Sequencer.Immediate).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
        vm.Property1 = "a";
        await Assert.That(list).Count().IsGreaterThan(1);
    }

    /// <summary>Verifies the WhenAnyDynamic overload for 2 properties with a selector and distinct-until-changed disabled.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task WhenAnyDynamic_2Props_Selector_NotDistinct()
    {
        var vm = new TestViewModel();
        var param = System.Linq.Expressions.Expression.Parameter(typeof(TestViewModel), "x");
        var property1 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property1));
        var property2 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property2));
        var list = new List<string>();
        _ = vm.WhenAnyDynamic(
            property1,
            property2,
            static (_, _) => "x",
            false).ObserveOn(Sequencer.Immediate).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
        vm.Property1 = "a";
        await Assert.That(list).Count().IsGreaterThan(1);
    }

    /// <summary>Verifies the WhenAnyDynamic overload for 3 properties with a selector.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task WhenAnyDynamic_3Props_Selector()
    {
        var vm = new TestViewModel();
        var param = System.Linq.Expressions.Expression.Parameter(typeof(TestViewModel), "x");
        var property1 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property1));
        var property2 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property2));
        var property3 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property3));
        var list = new List<string>();
        _ = vm.WhenAnyDynamic(
            property1,
            property2,
            property3,
            static (_, _, _) => "x").ObserveOn(Sequencer.Immediate).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
        vm.Property1 = "a";
        await Assert.That(list).Count().IsGreaterThan(1);
    }

    /// <summary>Verifies the WhenAnyDynamic overload for 3 properties with a selector and distinct-until-changed enabled.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task WhenAnyDynamic_3Props_Selector_Distinct()
    {
        var vm = new TestViewModel();
        var param = System.Linq.Expressions.Expression.Parameter(typeof(TestViewModel), "x");
        var property1 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property1));
        var property2 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property2));
        var property3 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property3));
        var list = new List<string>();
        _ = vm.WhenAnyDynamic(
            property1,
            property2,
            property3,
            static (_, _, _) => "x",
            true).ObserveOn(Sequencer.Immediate).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
        vm.Property1 = "a";
        await Assert.That(list).Count().IsGreaterThan(1);
    }

    /// <summary>Verifies the WhenAnyDynamic overload for 3 properties with a selector and distinct-until-changed disabled.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task WhenAnyDynamic_3Props_Selector_NotDistinct()
    {
        var vm = new TestViewModel();
        var param = System.Linq.Expressions.Expression.Parameter(typeof(TestViewModel), "x");
        var property1 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property1));
        var property2 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property2));
        var property3 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property3));
        var list = new List<string>();
        _ = vm.WhenAnyDynamic(
            property1,
            property2,
            property3,
            static (_, _, _) => "x",
            false).ObserveOn(Sequencer.Immediate).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
        vm.Property1 = "a";
        await Assert.That(list).Count().IsGreaterThan(1);
    }

    /// <summary>Verifies the WhenAnyDynamic overload for 4 properties with a selector.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
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
        _ = vm.WhenAnyDynamic(
            property1,
            property2,
            property3,
            property4,
            static (_, _, _, _) => "x").ObserveOn(Sequencer.Immediate).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
        vm.Property1 = "a";
        await Assert.That(list).Count().IsGreaterThan(1);
    }

    /// <summary>Verifies the WhenAnyDynamic overload for 4 properties with a selector and distinct-until-changed enabled.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
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
        _ = vm.WhenAnyDynamic(
            property1,
            property2,
            property3,
            property4,
            static (_, _, _, _) => "x",
            true).ObserveOn(Sequencer.Immediate).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
        vm.Property1 = "a";
        await Assert.That(list).Count().IsGreaterThan(1);
    }

    /// <summary>Verifies the WhenAnyDynamic overload for 4 properties with a selector and distinct-until-changed disabled.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
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
        _ = vm.WhenAnyDynamic(
            property1,
            property2,
            property3,
            property4,
            static (_, _, _, _) => "x",
            false).ObserveOn(Sequencer.Immediate).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
        vm.Property1 = "a";
        await Assert.That(list).Count().IsGreaterThan(1);
    }

    /// <summary>Verifies the WhenAnyDynamic overload for 5 properties with a selector.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
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
        _ = vm.WhenAnyDynamic(
            property1,
            property2,
            property3,
            property4,
            property5,
            static (_, _, _, _, _) => "x").ObserveOn(Sequencer.Immediate).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
        vm.Property1 = "a";
        await Assert.That(list).Count().IsGreaterThan(1);
    }

    /// <summary>Verifies the WhenAnyDynamic overload for 5 properties with a selector and distinct-until-changed enabled.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
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
        _ = vm.WhenAnyDynamic(
            property1,
            property2,
            property3,
            property4,
            property5,
            static (_, _, _, _, _) => "x",
            true).ObserveOn(Sequencer.Immediate).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
        vm.Property1 = "a";
        await Assert.That(list).Count().IsGreaterThan(1);
    }

    /// <summary>Verifies the WhenAnyDynamic overload for 5 properties with a selector and distinct-until-changed disabled.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
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
        _ = vm.WhenAnyDynamic(
            property1,
            property2,
            property3,
            property4,
            property5,
            static (_, _, _, _, _) => "x",
            false).ObserveOn(Sequencer.Immediate).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
        vm.Property1 = "a";
        await Assert.That(list).Count().IsGreaterThan(1);
    }

    /// <summary>Verifies the WhenAnyDynamic overload for 6 properties with a selector.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
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
        _ = vm.WhenAnyDynamic(
            property1,
            property2,
            property3,
            property4,
            property5,
            property6,
            static (_, _, _, _, _, _) => "x").ObserveOn(Sequencer.Immediate).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
        vm.Property1 = "a";
        await Assert.That(list).Count().IsGreaterThan(1);
    }

    /// <summary>Verifies the WhenAnyDynamic overload for 6 properties with a selector and distinct-until-changed enabled.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
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
        _ = vm.WhenAnyDynamic(
            property1,
            property2,
            property3,
            property4,
            property5,
            property6,
            static (_, _, _, _, _, _) => "x",
            true).ObserveOn(Sequencer.Immediate).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
        vm.Property1 = "a";
        await Assert.That(list).Count().IsGreaterThan(1);
    }

    /// <summary>Verifies the WhenAnyDynamic overload for 6 properties with a selector and distinct-until-changed disabled.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
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
        _ = vm.WhenAnyDynamic(
            property1,
            property2,
            property3,
            property4,
            property5,
            property6,
            static (_, _, _, _, _, _) => "x",
            false).ObserveOn(Sequencer.Immediate).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
        vm.Property1 = "a";
        await Assert.That(list).Count().IsGreaterThan(1);
    }

    /// <summary>Verifies the WhenAnyDynamic overload for 7 properties with a selector.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
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
        _ = vm.WhenAnyDynamic(
            property1,
            property2,
            property3,
            property4,
            property5,
            property6,
            property7,
            static (_, _, _, _, _, _, _) => "x").ObserveOn(Sequencer.Immediate).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
        vm.Property1 = "a";
        await Assert.That(list).Count().IsGreaterThan(1);
    }

    /// <summary>Verifies the WhenAnyDynamic overload for 7 properties with a selector and distinct-until-changed enabled.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
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
        _ = vm.WhenAnyDynamic(
            property1,
            property2,
            property3,
            property4,
            property5,
            property6,
            property7,
            static (_, _, _, _, _, _, _) => "x",
            true).ObserveOn(Sequencer.Immediate).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
        vm.Property1 = "a";
        await Assert.That(list).Count().IsGreaterThan(1);
    }

    /// <summary>Verifies the WhenAnyDynamic overload for 7 properties with a selector and distinct-until-changed disabled.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
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
        _ = vm.WhenAnyDynamic(
            property1,
            property2,
            property3,
            property4,
            property5,
            property6,
            property7,
            static (_, _, _, _, _, _, _) => "x",
            false).ObserveOn(Sequencer.Immediate).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
        vm.Property1 = "a";
        await Assert.That(list).Count().IsGreaterThan(1);
    }

    /// <summary>A test view model exposing multiple observable properties for WhenAnyDynamic tests.</summary>
    private sealed class TestViewModel : ReactiveObject
    {
        /// <summary>Gets or sets the first test property.</summary>
        public string? Property1
        {
            get;
            set => this.RaiseAndSetIfChanged(ref field, value);
        }

        /// <summary>Gets or sets the tenth test property.</summary>
        public string? Property10
        {
            get;
            set => this.RaiseAndSetIfChanged(ref field, value);
        }

        /// <summary>Gets or sets the eleventh test property.</summary>
        public string? Property11
        {
            get;
            set => this.RaiseAndSetIfChanged(ref field, value);
        }

        /// <summary>Gets or sets the twelfth test property.</summary>
        public string? Property12
        {
            get;
            set => this.RaiseAndSetIfChanged(ref field, value);
        }

        /// <summary>Gets or sets the second test property.</summary>
        public string? Property2
        {
            get;
            set => this.RaiseAndSetIfChanged(ref field, value);
        }

        /// <summary>Gets or sets the third test property.</summary>
        public string? Property3
        {
            get;
            set => this.RaiseAndSetIfChanged(ref field, value);
        }

        /// <summary>Gets or sets the fourth test property.</summary>
        public string? Property4
        {
            get;
            set => this.RaiseAndSetIfChanged(ref field, value);
        }

        /// <summary>Gets or sets the fifth test property.</summary>
        public string? Property5
        {
            get;
            set => this.RaiseAndSetIfChanged(ref field, value);
        }

        /// <summary>Gets or sets the sixth test property.</summary>
        public string? Property6
        {
            get;
            set => this.RaiseAndSetIfChanged(ref field, value);
        }

        /// <summary>Gets or sets the seventh test property.</summary>
        public string? Property7
        {
            get;
            set => this.RaiseAndSetIfChanged(ref field, value);
        }

        /// <summary>Gets or sets the eighth test property.</summary>
        public string? Property8
        {
            get;
            set => this.RaiseAndSetIfChanged(ref field, value);
        }

        /// <summary>Gets or sets the ninth test property.</summary>
        public string? Property9
        {
            get;
            set => this.RaiseAndSetIfChanged(ref field, value);
        }
    }
}
