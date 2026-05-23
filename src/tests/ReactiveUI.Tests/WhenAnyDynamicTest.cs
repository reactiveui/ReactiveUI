// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests;

/// <summary>
///     Tests for WhenAnyDynamic methods in VariadicTemplates.cs.
/// </summary>
public partial class WhenAnyDynamicTest
{
    /// <summary>
    ///     Verifies the WhenAnyDynamic overload for 1 property with a selector and distinct-until-changed enabled.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task WhenAnyDynamic_1Props_Selector_Distinct()
    {
        var vm = new TestViewModel();
        var param = System.Linq.Expressions.Expression.Parameter(typeof(TestViewModel), "x");
        var property1 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property1));
        var list = new List<string>();
        vm.WhenAnyDynamic(
            property1,
            _ => "x",
            true).ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
        vm.Property1 = "a";
        await Assert.That(list).Count().IsGreaterThan(1);
    }

    /// <summary>
    ///     Verifies the WhenAnyDynamic overload for 1 property with a selector and distinct-until-changed disabled.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task WhenAnyDynamic_1Props_Selector_NotDistinct()
    {
        var vm = new TestViewModel();
        var param = System.Linq.Expressions.Expression.Parameter(typeof(TestViewModel), "x");
        var property1 = System.Linq.Expressions.Expression.Property(param, nameof(TestViewModel.Property1));
        var list = new List<string>();
        vm.WhenAnyDynamic(
            property1,
            _ => "x",
            false).ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
        vm.Property1 = "a";
        await Assert.That(list).Count().IsGreaterThan(1);
    }

    /// <summary>
    ///     Verifies the WhenAnyDynamic overload for 2 properties with a selector.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
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
            (_, _) => "x").ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
        vm.Property1 = "a";
        await Assert.That(list).Count().IsGreaterThan(1);
    }

    /// <summary>
    ///     Verifies the WhenAnyDynamic overload for 2 properties with a selector and distinct-until-changed enabled.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
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
            (_, _) => "x",
            true).ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
        vm.Property1 = "a";
        await Assert.That(list).Count().IsGreaterThan(1);
    }

    /// <summary>
    ///     Verifies the WhenAnyDynamic overload for 2 properties with a selector and distinct-until-changed disabled.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
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
            (_, _) => "x",
            false).ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
        vm.Property1 = "a";
        await Assert.That(list).Count().IsGreaterThan(1);
    }

    /// <summary>
    ///     Verifies the WhenAnyDynamic overload for 3 properties with a selector.
    /// </summary>
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
        vm.WhenAnyDynamic(
            property1,
            property2,
            property3,
            (_, _, _) => "x").ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
        vm.Property1 = "a";
        await Assert.That(list).Count().IsGreaterThan(1);
    }

    /// <summary>
    ///     Verifies the WhenAnyDynamic overload for 3 properties with a selector and distinct-until-changed enabled.
    /// </summary>
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
        vm.WhenAnyDynamic(
            property1,
            property2,
            property3,
            (_, _, _) => "x",
            true).ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
        vm.Property1 = "a";
        await Assert.That(list).Count().IsGreaterThan(1);
    }

    /// <summary>
    ///     Verifies the WhenAnyDynamic overload for 3 properties with a selector and distinct-until-changed disabled.
    /// </summary>
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
        vm.WhenAnyDynamic(
            property1,
            property2,
            property3,
            (_, _, _) => "x",
            false).ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
        vm.Property1 = "a";
        await Assert.That(list).Count().IsGreaterThan(1);
    }

    /// <summary>
    ///     Verifies the WhenAnyDynamic overload for 4 properties with a selector.
    /// </summary>
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
        vm.WhenAnyDynamic(
            property1,
            property2,
            property3,
            property4,
            (_, _, _, _) => "x").ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
        vm.Property1 = "a";
        await Assert.That(list).Count().IsGreaterThan(1);
    }

    /// <summary>
    ///     Verifies the WhenAnyDynamic overload for 4 properties with a selector and distinct-until-changed enabled.
    /// </summary>
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
        vm.WhenAnyDynamic(
            property1,
            property2,
            property3,
            property4,
            (_, _, _, _) => "x",
            true).ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
        vm.Property1 = "a";
        await Assert.That(list).Count().IsGreaterThan(1);
    }

    /// <summary>
    ///     Verifies the WhenAnyDynamic overload for 4 properties with a selector and distinct-until-changed disabled.
    /// </summary>
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
        vm.WhenAnyDynamic(
            property1,
            property2,
            property3,
            property4,
            (_, _, _, _) => "x",
            false).ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
        vm.Property1 = "a";
        await Assert.That(list).Count().IsGreaterThan(1);
    }

    /// <summary>
    ///     Verifies the WhenAnyDynamic overload for 5 properties with a selector.
    /// </summary>
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
        vm.WhenAnyDynamic(
            property1,
            property2,
            property3,
            property4,
            property5,
            (_, _, _, _, _) => "x").ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
        vm.Property1 = "a";
        await Assert.That(list).Count().IsGreaterThan(1);
    }

    /// <summary>
    ///     Verifies the WhenAnyDynamic overload for 5 properties with a selector and distinct-until-changed enabled.
    /// </summary>
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
        vm.WhenAnyDynamic(
            property1,
            property2,
            property3,
            property4,
            property5,
            (_, _, _, _, _) => "x",
            true).ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
        vm.Property1 = "a";
        await Assert.That(list).Count().IsGreaterThan(1);
    }

    /// <summary>
    ///     Verifies the WhenAnyDynamic overload for 5 properties with a selector and distinct-until-changed disabled.
    /// </summary>
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
        vm.WhenAnyDynamic(
            property1,
            property2,
            property3,
            property4,
            property5,
            (_, _, _, _, _) => "x",
            false).ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
        vm.Property1 = "a";
        await Assert.That(list).Count().IsGreaterThan(1);
    }

    /// <summary>
    ///     Verifies the WhenAnyDynamic overload for 6 properties with a selector.
    /// </summary>
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
        vm.WhenAnyDynamic(
            property1,
            property2,
            property3,
            property4,
            property5,
            property6,
            (_, _, _, _, _, _) => "x").ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
        vm.Property1 = "a";
        await Assert.That(list).Count().IsGreaterThan(1);
    }

    /// <summary>
    ///     Verifies the WhenAnyDynamic overload for 6 properties with a selector and distinct-until-changed enabled.
    /// </summary>
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
        vm.WhenAnyDynamic(
            property1,
            property2,
            property3,
            property4,
            property5,
            property6,
            (_, _, _, _, _, _) => "x",
            true).ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
        vm.Property1 = "a";
        await Assert.That(list).Count().IsGreaterThan(1);
    }

    /// <summary>
    ///     Verifies the WhenAnyDynamic overload for 6 properties with a selector and distinct-until-changed disabled.
    /// </summary>
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
        vm.WhenAnyDynamic(
            property1,
            property2,
            property3,
            property4,
            property5,
            property6,
            (_, _, _, _, _, _) => "x",
            false).ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
        vm.Property1 = "a";
        await Assert.That(list).Count().IsGreaterThan(1);
    }

    /// <summary>
    ///     Verifies the WhenAnyDynamic overload for 7 properties with a selector.
    /// </summary>
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
        vm.WhenAnyDynamic(
            property1,
            property2,
            property3,
            property4,
            property5,
            property6,
            property7,
            (_, _, _, _, _, _, _) => "x").ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
        vm.Property1 = "a";
        await Assert.That(list).Count().IsGreaterThan(1);
    }

    /// <summary>
    ///     Verifies the WhenAnyDynamic overload for 7 properties with a selector and distinct-until-changed enabled.
    /// </summary>
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
        vm.WhenAnyDynamic(
            property1,
            property2,
            property3,
            property4,
            property5,
            property6,
            property7,
            (_, _, _, _, _, _, _) => "x",
            true).ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
        vm.Property1 = "a";
        await Assert.That(list).Count().IsGreaterThan(1);
    }

    /// <summary>
    ///     Verifies the WhenAnyDynamic overload for 7 properties with a selector and distinct-until-changed disabled.
    /// </summary>
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
        vm.WhenAnyDynamic(
            property1,
            property2,
            property3,
            property4,
            property5,
            property6,
            property7,
            (_, _, _, _, _, _, _) => "x",
            false).ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
        vm.Property1 = "a";
        await Assert.That(list).Count().IsGreaterThan(1);
    }

    /// <summary>
    ///     A test view model exposing multiple observable properties for WhenAnyDynamic tests.
    /// </summary>
    private sealed class TestViewModel : ReactiveObject
    {
        /// <summary>
        ///     The backing field for the <see cref="Property1" /> property.
        /// </summary>
        private string? _property1;

        /// <summary>
        ///     The backing field for the <see cref="Property10" /> property.
        /// </summary>
        private string? _property10;

        /// <summary>
        ///     The backing field for the <see cref="Property11" /> property.
        /// </summary>
        private string? _property11;

        /// <summary>
        ///     The backing field for the <see cref="Property12" /> property.
        /// </summary>
        private string? _property12;

        /// <summary>
        ///     The backing field for the <see cref="Property2" /> property.
        /// </summary>
        private string? _property2;

        /// <summary>
        ///     The backing field for the <see cref="Property3" /> property.
        /// </summary>
        private string? _property3;

        /// <summary>
        ///     The backing field for the <see cref="Property4" /> property.
        /// </summary>
        private string? _property4;

        /// <summary>
        ///     The backing field for the <see cref="Property5" /> property.
        /// </summary>
        private string? _property5;

        /// <summary>
        ///     The backing field for the <see cref="Property6" /> property.
        /// </summary>
        private string? _property6;

        /// <summary>
        ///     The backing field for the <see cref="Property7" /> property.
        /// </summary>
        private string? _property7;

        /// <summary>
        ///     The backing field for the <see cref="Property8" /> property.
        /// </summary>
        private string? _property8;

        /// <summary>
        ///     The backing field for the <see cref="Property9" /> property.
        /// </summary>
        private string? _property9;

        /// <summary>
        ///     Gets or sets the first test property.
        /// </summary>
        public string? Property1
        {
            get => _property1;
            set => this.RaiseAndSetIfChanged(ref _property1, value);
        }

        /// <summary>
        ///     Gets or sets the tenth test property.
        /// </summary>
        public string? Property10
        {
            get => _property10;
            set => this.RaiseAndSetIfChanged(ref _property10, value);
        }

        /// <summary>
        ///     Gets or sets the eleventh test property.
        /// </summary>
        public string? Property11
        {
            get => _property11;
            set => this.RaiseAndSetIfChanged(ref _property11, value);
        }

        /// <summary>
        ///     Gets or sets the twelfth test property.
        /// </summary>
        public string? Property12
        {
            get => _property12;
            set => this.RaiseAndSetIfChanged(ref _property12, value);
        }

        /// <summary>
        ///     Gets or sets the second test property.
        /// </summary>
        public string? Property2
        {
            get => _property2;
            set => this.RaiseAndSetIfChanged(ref _property2, value);
        }

        /// <summary>
        ///     Gets or sets the third test property.
        /// </summary>
        public string? Property3
        {
            get => _property3;
            set => this.RaiseAndSetIfChanged(ref _property3, value);
        }

        /// <summary>
        ///     Gets or sets the fourth test property.
        /// </summary>
        public string? Property4
        {
            get => _property4;
            set => this.RaiseAndSetIfChanged(ref _property4, value);
        }

        /// <summary>
        ///     Gets or sets the fifth test property.
        /// </summary>
        public string? Property5
        {
            get => _property5;
            set => this.RaiseAndSetIfChanged(ref _property5, value);
        }

        /// <summary>
        ///     Gets or sets the sixth test property.
        /// </summary>
        public string? Property6
        {
            get => _property6;
            set => this.RaiseAndSetIfChanged(ref _property6, value);
        }

        /// <summary>
        ///     Gets or sets the seventh test property.
        /// </summary>
        public string? Property7
        {
            get => _property7;
            set => this.RaiseAndSetIfChanged(ref _property7, value);
        }

        /// <summary>
        ///     Gets or sets the eighth test property.
        /// </summary>
        public string? Property8
        {
            get => _property8;
            set => this.RaiseAndSetIfChanged(ref _property8, value);
        }

        /// <summary>
        ///     Gets or sets the ninth test property.
        /// </summary>
        public string? Property9
        {
            get => _property9;
            set => this.RaiseAndSetIfChanged(ref _property9, value);
        }
    }
}
