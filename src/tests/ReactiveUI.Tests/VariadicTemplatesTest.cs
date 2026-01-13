// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests;

public class VariadicTemplatesTest
{
    [Test]
    public async Task WhenAny_10Props_Sel()
    {
        var vm = new TestViewModel();
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
                (v1, v2, v3, v4, v5, v6, v7, v8, v9, v10) => "x").ObserveOn(ImmediateScheduler.Instance)
            .Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    [Test]
    public async Task WhenAny_10Props_Sel_Dist()
    {
        var vm = new TestViewModel();
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
            (v1, v2, v3, v4, v5, v6, v7, v8, v9, v10) => "x",
            true).ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    [Test]
    public async Task WhenAny_10Props_Sel_Str()
    {
        var vm = new TestViewModel();
        var list = new List<string>();
        vm.WhenAny<TestViewModel, string, string?, string?, string?, string?, string?, string?, string?, string?, string?, string?>(
                nameof(TestViewModel.Property1),
                nameof(TestViewModel.Property2),
                nameof(TestViewModel.Property3),
                nameof(TestViewModel.Property4),
                nameof(TestViewModel.Property5),
                nameof(TestViewModel.Property6),
                nameof(TestViewModel.Property7),
                nameof(TestViewModel.Property8),
                nameof(TestViewModel.Property9),
                nameof(TestViewModel.Property10),
                (v1, v2, v3, v4, v5, v6, v7, v8, v9, v10) => "x").ObserveOn(ImmediateScheduler.Instance)
            .Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    [Test]
    public async Task WhenAny_10Props_Sel_Str_Dist()
    {
        var vm = new TestViewModel();
        var list = new List<string>();
        vm.WhenAny<TestViewModel, string, string?, string?, string?, string?, string?, string?, string?, string?, string?, string?>(
            nameof(TestViewModel.Property1),
            nameof(TestViewModel.Property2),
            nameof(TestViewModel.Property3),
            nameof(TestViewModel.Property4),
            nameof(TestViewModel.Property5),
            nameof(TestViewModel.Property6),
            nameof(TestViewModel.Property7),
            nameof(TestViewModel.Property8),
            nameof(TestViewModel.Property9),
            nameof(TestViewModel.Property10),
            (v1, v2, v3, v4, v5, v6, v7, v8, v9, v10) => "x",
            false).ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    [Test]
    public async Task WhenAny_11Props_Sel()
    {
        var vm = new TestViewModel();
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
                (v1, v2, v3, v4, v5, v6, v7, v8, v9, v10, v11) => "x").ObserveOn(ImmediateScheduler.Instance)
            .Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    [Test]
    public async Task WhenAny_11Props_Sel_Dist()
    {
        var vm = new TestViewModel();
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
            (v1, v2, v3, v4, v5, v6, v7, v8, v9, v10, v11) => "x",
            true).ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    [Test]
    public async Task WhenAny_11Props_Sel_Str()
    {
        var vm = new TestViewModel();
        var list = new List<string>();
        vm.WhenAny<TestViewModel, string, string?, string?, string?, string?, string?, string?, string?, string?, string?, string?, string?>(
                nameof(TestViewModel.Property1),
                nameof(TestViewModel.Property2),
                nameof(TestViewModel.Property3),
                nameof(TestViewModel.Property4),
                nameof(TestViewModel.Property5),
                nameof(TestViewModel.Property6),
                nameof(TestViewModel.Property7),
                nameof(TestViewModel.Property8),
                nameof(TestViewModel.Property9),
                nameof(TestViewModel.Property10),
                nameof(TestViewModel.Property11),
                (v1, v2, v3, v4, v5, v6, v7, v8, v9, v10, v11) => "x").ObserveOn(ImmediateScheduler.Instance)
            .Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    [Test]
    public async Task WhenAny_11Props_Sel_Str_Dist()
    {
        var vm = new TestViewModel();
        var list = new List<string>();
        vm.WhenAny<TestViewModel, string, string?, string?, string?, string?, string?, string?, string?, string?, string?, string?, string?>(
            nameof(TestViewModel.Property1),
            nameof(TestViewModel.Property2),
            nameof(TestViewModel.Property3),
            nameof(TestViewModel.Property4),
            nameof(TestViewModel.Property5),
            nameof(TestViewModel.Property6),
            nameof(TestViewModel.Property7),
            nameof(TestViewModel.Property8),
            nameof(TestViewModel.Property9),
            nameof(TestViewModel.Property10),
            nameof(TestViewModel.Property11),
            (v1, v2, v3, v4, v5, v6, v7, v8, v9, v10, v11) => "x",
            false).ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    [Test]
    public async Task WhenAny_12Props_Sel()
    {
        var vm = new TestViewModel();
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
                (v1, v2, v3, v4, v5, v6, v7, v8, v9, v10, v11, v12) => "x").ObserveOn(ImmediateScheduler.Instance)
            .Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    [Test]
    public async Task WhenAny_12Props_Sel_Dist()
    {
        var vm = new TestViewModel();
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
            (v1, v2, v3, v4, v5, v6, v7, v8, v9, v10, v11, v12) => "x",
            true).ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    [Test]
    public async Task WhenAny_12Props_Sel_Str()
    {
        var vm = new TestViewModel();
        var list = new List<string>();
        vm.WhenAny<TestViewModel, string, string?, string?, string?, string?, string?, string?, string?, string?, string?, string?, string?, string?>(
                nameof(TestViewModel.Property1),
                nameof(TestViewModel.Property2),
                nameof(TestViewModel.Property3),
                nameof(TestViewModel.Property4),
                nameof(TestViewModel.Property5),
                nameof(TestViewModel.Property6),
                nameof(TestViewModel.Property7),
                nameof(TestViewModel.Property8),
                nameof(TestViewModel.Property9),
                nameof(TestViewModel.Property10),
                nameof(TestViewModel.Property11),
                nameof(TestViewModel.Property12),
                (v1, v2, v3, v4, v5, v6, v7, v8, v9, v10, v11, v12) => "x").ObserveOn(ImmediateScheduler.Instance)
            .Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    [Test]
    public async Task WhenAny_12Props_Sel_Str_Dist()
    {
        var vm = new TestViewModel();
        var list = new List<string>();
        vm.WhenAny<TestViewModel, string, string?, string?, string?, string?, string?, string?, string?, string?, string?, string?, string?, string?>(
            nameof(TestViewModel.Property1),
            nameof(TestViewModel.Property2),
            nameof(TestViewModel.Property3),
            nameof(TestViewModel.Property4),
            nameof(TestViewModel.Property5),
            nameof(TestViewModel.Property6),
            nameof(TestViewModel.Property7),
            nameof(TestViewModel.Property8),
            nameof(TestViewModel.Property9),
            nameof(TestViewModel.Property10),
            nameof(TestViewModel.Property11),
            nameof(TestViewModel.Property12),
            (v1, v2, v3, v4, v5, v6, v7, v8, v9, v10, v11, v12) => "x",
            false).ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    [Test]
    public async Task WhenAny_1Props_Sel()
    {
        var vm = new TestViewModel();
        var list = new List<string>();
        vm.WhenAny(
            x => x.Property1,
            v1 => "x").ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    [Test]
    public async Task WhenAny_1Props_Sel_Dist()
    {
        var vm = new TestViewModel();
        var list = new List<string>();
        vm.WhenAny(
            x => x.Property1,
            v1 => "x",
            true).ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    [Test]
    public async Task WhenAny_1Props_Sel_Str()
    {
        var vm = new TestViewModel();
        var list = new List<string>();
        vm.WhenAny<TestViewModel, string, string?>(
            nameof(TestViewModel.Property1),
            v1 => "x").ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    [Test]
    public async Task WhenAny_1Props_Sel_Str_Dist()
    {
        var vm = new TestViewModel();
        var list = new List<string>();
        vm.WhenAny<TestViewModel, string, string?>(
            nameof(TestViewModel.Property1),
            v1 => "x",
            false).ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    [Test]
    public async Task WhenAny_2Props_Sel()
    {
        var vm = new TestViewModel();
        var list = new List<string>();
        vm.WhenAny(
            x => x.Property1,
            x => x.Property2,
            (v1, v2) => "x").ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    [Test]
    public async Task WhenAny_2Props_Sel_Dist()
    {
        var vm = new TestViewModel();
        var list = new List<string>();
        vm.WhenAny(
            x => x.Property1,
            x => x.Property2,
            (v1, v2) => "x",
            true).ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    [Test]
    public async Task WhenAny_2Props_Sel_Str()
    {
        var vm = new TestViewModel();
        var list = new List<string>();
        vm.WhenAny<TestViewModel, string, string?, string?>(
            nameof(TestViewModel.Property1),
            nameof(TestViewModel.Property2),
            (v1, v2) => "x").ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    [Test]
    public async Task WhenAny_2Props_Sel_Str_Dist()
    {
        var vm = new TestViewModel();
        var list = new List<string>();
        vm.WhenAny<TestViewModel, string, string?, string?>(
            nameof(TestViewModel.Property1),
            nameof(TestViewModel.Property2),
            (v1, v2) => "x",
            false).ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    [Test]
    public async Task WhenAny_3Props_Sel()
    {
        var vm = new TestViewModel();
        var list = new List<string>();
        vm.WhenAny(
            x => x.Property1,
            x => x.Property2,
            x => x.Property3,
            (v1, v2, v3) => "x").ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    [Test]
    public async Task WhenAny_3Props_Sel_Dist()
    {
        var vm = new TestViewModel();
        var list = new List<string>();
        vm.WhenAny(
            x => x.Property1,
            x => x.Property2,
            x => x.Property3,
            (v1, v2, v3) => "x",
            true).ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    [Test]
    public async Task WhenAny_3Props_Sel_Str()
    {
        var vm = new TestViewModel();
        var list = new List<string>();
        vm.WhenAny<TestViewModel, string, string?, string?, string?>(
            nameof(TestViewModel.Property1),
            nameof(TestViewModel.Property2),
            nameof(TestViewModel.Property3),
            (v1, v2, v3) => "x").ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    [Test]
    public async Task WhenAny_3Props_Sel_Str_Dist()
    {
        var vm = new TestViewModel();
        var list = new List<string>();
        vm.WhenAny<TestViewModel, string, string?, string?, string?>(
            nameof(TestViewModel.Property1),
            nameof(TestViewModel.Property2),
            nameof(TestViewModel.Property3),
            (v1, v2, v3) => "x",
            false).ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    [Test]
    public async Task WhenAny_4Props_Sel()
    {
        var vm = new TestViewModel();
        var list = new List<string>();
        vm.WhenAny(
            x => x.Property1,
            x => x.Property2,
            x => x.Property3,
            x => x.Property4,
            (v1, v2, v3, v4) => "x").ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    [Test]
    public async Task WhenAny_4Props_Sel_Dist()
    {
        var vm = new TestViewModel();
        var list = new List<string>();
        vm.WhenAny(
            x => x.Property1,
            x => x.Property2,
            x => x.Property3,
            x => x.Property4,
            (v1, v2, v3, v4) => "x",
            true).ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    [Test]
    public async Task WhenAny_4Props_Sel_Str()
    {
        var vm = new TestViewModel();
        var list = new List<string>();
        vm.WhenAny<TestViewModel, string, string?, string?, string?, string?>(
            nameof(TestViewModel.Property1),
            nameof(TestViewModel.Property2),
            nameof(TestViewModel.Property3),
            nameof(TestViewModel.Property4),
            (v1, v2, v3, v4) => "x").ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    [Test]
    public async Task WhenAny_4Props_Sel_Str_Dist()
    {
        var vm = new TestViewModel();
        var list = new List<string>();
        vm.WhenAny<TestViewModel, string, string?, string?, string?, string?>(
            nameof(TestViewModel.Property1),
            nameof(TestViewModel.Property2),
            nameof(TestViewModel.Property3),
            nameof(TestViewModel.Property4),
            (v1, v2, v3, v4) => "x",
            false).ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    [Test]
    public async Task WhenAny_5Props_Sel()
    {
        var vm = new TestViewModel();
        var list = new List<string>();
        vm.WhenAny(
            x => x.Property1,
            x => x.Property2,
            x => x.Property3,
            x => x.Property4,
            x => x.Property5,
            (v1, v2, v3, v4, v5) => "x").ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    [Test]
    public async Task WhenAny_5Props_Sel_Dist()
    {
        var vm = new TestViewModel();
        var list = new List<string>();
        vm.WhenAny(
            x => x.Property1,
            x => x.Property2,
            x => x.Property3,
            x => x.Property4,
            x => x.Property5,
            (v1, v2, v3, v4, v5) => "x",
            true).ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    [Test]
    public async Task WhenAny_5Props_Sel_Str()
    {
        var vm = new TestViewModel();
        var list = new List<string>();
        vm.WhenAny<TestViewModel, string, string?, string?, string?, string?, string?>(
            nameof(TestViewModel.Property1),
            nameof(TestViewModel.Property2),
            nameof(TestViewModel.Property3),
            nameof(TestViewModel.Property4),
            nameof(TestViewModel.Property5),
            (v1, v2, v3, v4, v5) => "x").ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    [Test]
    public async Task WhenAny_5Props_Sel_Str_Dist()
    {
        var vm = new TestViewModel();
        var list = new List<string>();
        vm.WhenAny<TestViewModel, string, string?, string?, string?, string?, string?>(
            nameof(TestViewModel.Property1),
            nameof(TestViewModel.Property2),
            nameof(TestViewModel.Property3),
            nameof(TestViewModel.Property4),
            nameof(TestViewModel.Property5),
            (v1, v2, v3, v4, v5) => "x",
            false).ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    [Test]
    public async Task WhenAny_6Props_Sel()
    {
        var vm = new TestViewModel();
        var list = new List<string>();
        vm.WhenAny(
            x => x.Property1,
            x => x.Property2,
            x => x.Property3,
            x => x.Property4,
            x => x.Property5,
            x => x.Property6,
            (v1, v2, v3, v4, v5, v6) => "x").ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    [Test]
    public async Task WhenAny_6Props_Sel_Dist()
    {
        var vm = new TestViewModel();
        var list = new List<string>();
        vm.WhenAny(
            x => x.Property1,
            x => x.Property2,
            x => x.Property3,
            x => x.Property4,
            x => x.Property5,
            x => x.Property6,
            (v1, v2, v3, v4, v5, v6) => "x",
            true).ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    [Test]
    public async Task WhenAny_6Props_Sel_Str()
    {
        var vm = new TestViewModel();
        var list = new List<string>();
        vm.WhenAny<TestViewModel, string, string?, string?, string?, string?, string?, string?>(
            nameof(TestViewModel.Property1),
            nameof(TestViewModel.Property2),
            nameof(TestViewModel.Property3),
            nameof(TestViewModel.Property4),
            nameof(TestViewModel.Property5),
            nameof(TestViewModel.Property6),
            (v1, v2, v3, v4, v5, v6) => "x").ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    [Test]
    public async Task WhenAny_6Props_Sel_Str_Dist()
    {
        var vm = new TestViewModel();
        var list = new List<string>();
        vm.WhenAny<TestViewModel, string, string?, string?, string?, string?, string?, string?>(
            nameof(TestViewModel.Property1),
            nameof(TestViewModel.Property2),
            nameof(TestViewModel.Property3),
            nameof(TestViewModel.Property4),
            nameof(TestViewModel.Property5),
            nameof(TestViewModel.Property6),
            (v1, v2, v3, v4, v5, v6) => "x",
            false).ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    [Test]
    public async Task WhenAny_7Props_Sel()
    {
        var vm = new TestViewModel();
        var list = new List<string>();
        vm.WhenAny(
            x => x.Property1,
            x => x.Property2,
            x => x.Property3,
            x => x.Property4,
            x => x.Property5,
            x => x.Property6,
            x => x.Property7,
            (v1, v2, v3, v4, v5, v6, v7) => "x").ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    [Test]
    public async Task WhenAny_7Props_Sel_Dist()
    {
        var vm = new TestViewModel();
        var list = new List<string>();
        vm.WhenAny(
            x => x.Property1,
            x => x.Property2,
            x => x.Property3,
            x => x.Property4,
            x => x.Property5,
            x => x.Property6,
            x => x.Property7,
            (v1, v2, v3, v4, v5, v6, v7) => "x",
            true).ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    [Test]
    public async Task WhenAny_7Props_Sel_Str()
    {
        var vm = new TestViewModel();
        var list = new List<string>();
        vm.WhenAny<TestViewModel, string, string?, string?, string?, string?, string?, string?, string?>(
            nameof(TestViewModel.Property1),
            nameof(TestViewModel.Property2),
            nameof(TestViewModel.Property3),
            nameof(TestViewModel.Property4),
            nameof(TestViewModel.Property5),
            nameof(TestViewModel.Property6),
            nameof(TestViewModel.Property7),
            (v1, v2, v3, v4, v5, v6, v7) => "x").ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    [Test]
    public async Task WhenAny_7Props_Sel_Str_Dist()
    {
        var vm = new TestViewModel();
        var list = new List<string>();
        vm.WhenAny<TestViewModel, string, string?, string?, string?, string?, string?, string?, string?>(
            nameof(TestViewModel.Property1),
            nameof(TestViewModel.Property2),
            nameof(TestViewModel.Property3),
            nameof(TestViewModel.Property4),
            nameof(TestViewModel.Property5),
            nameof(TestViewModel.Property6),
            nameof(TestViewModel.Property7),
            (v1, v2, v3, v4, v5, v6, v7) => "x",
            false).ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    [Test]
    public async Task WhenAny_8Props_Sel()
    {
        var vm = new TestViewModel();
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
            (v1, v2, v3, v4, v5, v6, v7, v8) => "x").ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    [Test]
    public async Task WhenAny_8Props_Sel_Dist()
    {
        var vm = new TestViewModel();
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
            (v1, v2, v3, v4, v5, v6, v7, v8) => "x",
            true).ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    [Test]
    public async Task WhenAny_8Props_Sel_Str()
    {
        var vm = new TestViewModel();
        var list = new List<string>();
        vm.WhenAny<TestViewModel, string, string?, string?, string?, string?, string?, string?, string?, string?>(
            nameof(TestViewModel.Property1),
            nameof(TestViewModel.Property2),
            nameof(TestViewModel.Property3),
            nameof(TestViewModel.Property4),
            nameof(TestViewModel.Property5),
            nameof(TestViewModel.Property6),
            nameof(TestViewModel.Property7),
            nameof(TestViewModel.Property8),
            (v1, v2, v3, v4, v5, v6, v7, v8) => "x").ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    [Test]
    public async Task WhenAny_8Props_Sel_Str_Dist()
    {
        var vm = new TestViewModel();
        var list = new List<string>();
        vm.WhenAny<TestViewModel, string, string?, string?, string?, string?, string?, string?, string?, string?>(
            nameof(TestViewModel.Property1),
            nameof(TestViewModel.Property2),
            nameof(TestViewModel.Property3),
            nameof(TestViewModel.Property4),
            nameof(TestViewModel.Property5),
            nameof(TestViewModel.Property6),
            nameof(TestViewModel.Property7),
            nameof(TestViewModel.Property8),
            (v1, v2, v3, v4, v5, v6, v7, v8) => "x",
            false).ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    [Test]
    public async Task WhenAny_9Props_Sel()
    {
        var vm = new TestViewModel();
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
            (v1, v2, v3, v4, v5, v6, v7, v8, v9) => "x").ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    [Test]
    public async Task WhenAny_9Props_Sel_Dist()
    {
        var vm = new TestViewModel();
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
            (v1, v2, v3, v4, v5, v6, v7, v8, v9) => "x",
            true).ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    [Test]
    public async Task WhenAny_9Props_Sel_Str()
    {
        var vm = new TestViewModel();
        var list = new List<string>();
        vm.WhenAny<TestViewModel, string, string?, string?, string?, string?, string?, string?, string?, string?, string?>(
            nameof(TestViewModel.Property1),
            nameof(TestViewModel.Property2),
            nameof(TestViewModel.Property3),
            nameof(TestViewModel.Property4),
            nameof(TestViewModel.Property5),
            nameof(TestViewModel.Property6),
            nameof(TestViewModel.Property7),
            nameof(TestViewModel.Property8),
            nameof(TestViewModel.Property9),
            (v1, v2, v3, v4, v5, v6, v7, v8, v9) => "x").ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    [Test]
    public async Task WhenAny_9Props_Sel_Str_Dist()
    {
        var vm = new TestViewModel();
        var list = new List<string>();
        vm.WhenAny<TestViewModel, string, string?, string?, string?, string?, string?, string?, string?, string?, string?>(
            nameof(TestViewModel.Property1),
            nameof(TestViewModel.Property2),
            nameof(TestViewModel.Property3),
            nameof(TestViewModel.Property4),
            nameof(TestViewModel.Property5),
            nameof(TestViewModel.Property6),
            nameof(TestViewModel.Property7),
            nameof(TestViewModel.Property8),
            nameof(TestViewModel.Property9),
            (v1, v2, v3, v4, v5, v6, v7, v8, v9) => "x",
            false).ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    [Test]
    public async Task WhenAnyObservable_10Props()
    {
        var vm = new TestViewModel();
        var subj1 = new Subject<string>();
        vm.ObservableProperty1 = subj1;
        var subj2 = new Subject<string>();
        vm.ObservableProperty2 = subj2;
        var subj3 = new Subject<string>();
        vm.ObservableProperty3 = subj3;
        var subj4 = new Subject<string>();
        vm.ObservableProperty4 = subj4;
        var subj5 = new Subject<string>();
        vm.ObservableProperty5 = subj5;
        var subj6 = new Subject<string>();
        vm.ObservableProperty6 = subj6;
        var subj7 = new Subject<string>();
        vm.ObservableProperty7 = subj7;
        var subj8 = new Subject<string>();
        vm.ObservableProperty8 = subj8;
        var subj9 = new Subject<string>();
        vm.ObservableProperty9 = subj9;
        var subj10 = new Subject<string>();
        vm.ObservableProperty10 = subj10;
        var list = new List<string>();
        vm.WhenAnyObservable(
            x => x.ObservableProperty1,
            x => x.ObservableProperty2,
            x => x.ObservableProperty3,
            x => x.ObservableProperty4,
            x => x.ObservableProperty5,
            x => x.ObservableProperty6,
            x => x.ObservableProperty7,
            x => x.ObservableProperty8,
            x => x.ObservableProperty9,
            x => x.ObservableProperty10).ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        subj1.OnNext("test");
        subj2.OnNext("test");
        subj3.OnNext("test");
        subj4.OnNext("test");
        subj5.OnNext("test");
        subj6.OnNext("test");
        subj7.OnNext("test");
        subj8.OnNext("test");
        subj9.OnNext("test");
        subj10.OnNext("test");
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    [Test]
    public async Task WhenAnyObservable_10Props_Sel()
    {
        var vm = new TestViewModel();
        var subj1 = new Subject<string>();
        vm.ObservableProperty1 = subj1;
        var subj2 = new Subject<string>();
        vm.ObservableProperty2 = subj2;
        var subj3 = new Subject<string>();
        vm.ObservableProperty3 = subj3;
        var subj4 = new Subject<string>();
        vm.ObservableProperty4 = subj4;
        var subj5 = new Subject<string>();
        vm.ObservableProperty5 = subj5;
        var subj6 = new Subject<string>();
        vm.ObservableProperty6 = subj6;
        var subj7 = new Subject<string>();
        vm.ObservableProperty7 = subj7;
        var subj8 = new Subject<string>();
        vm.ObservableProperty8 = subj8;
        var subj9 = new Subject<string>();
        vm.ObservableProperty9 = subj9;
        var subj10 = new Subject<string>();
        vm.ObservableProperty10 = subj10;
        var list = new List<string>();
        vm.WhenAnyObservable(
                x => x.ObservableProperty1,
                x => x.ObservableProperty2,
                x => x.ObservableProperty3,
                x => x.ObservableProperty4,
                x => x.ObservableProperty5,
                x => x.ObservableProperty6,
                x => x.ObservableProperty7,
                x => x.ObservableProperty8,
                x => x.ObservableProperty9,
                x => x.ObservableProperty10,
                (v1, v2, v3, v4, v5, v6, v7, v8, v9, v10) => "x").ObserveOn(ImmediateScheduler.Instance)
            .Subscribe(list.Add);
        subj1.OnNext("test");
        subj2.OnNext("test");
        subj3.OnNext("test");
        subj4.OnNext("test");
        subj5.OnNext("test");
        subj6.OnNext("test");
        subj7.OnNext("test");
        subj8.OnNext("test");
        subj9.OnNext("test");
        subj10.OnNext("test");
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    [Test]
    public async Task WhenAnyObservable_11Props()
    {
        var vm = new TestViewModel();
        var subj1 = new Subject<string>();
        vm.ObservableProperty1 = subj1;
        var subj2 = new Subject<string>();
        vm.ObservableProperty2 = subj2;
        var subj3 = new Subject<string>();
        vm.ObservableProperty3 = subj3;
        var subj4 = new Subject<string>();
        vm.ObservableProperty4 = subj4;
        var subj5 = new Subject<string>();
        vm.ObservableProperty5 = subj5;
        var subj6 = new Subject<string>();
        vm.ObservableProperty6 = subj6;
        var subj7 = new Subject<string>();
        vm.ObservableProperty7 = subj7;
        var subj8 = new Subject<string>();
        vm.ObservableProperty8 = subj8;
        var subj9 = new Subject<string>();
        vm.ObservableProperty9 = subj9;
        var subj10 = new Subject<string>();
        vm.ObservableProperty10 = subj10;
        var subj11 = new Subject<string>();
        vm.ObservableProperty11 = subj11;
        var list = new List<string>();
        vm.WhenAnyObservable(
            x => x.ObservableProperty1,
            x => x.ObservableProperty2,
            x => x.ObservableProperty3,
            x => x.ObservableProperty4,
            x => x.ObservableProperty5,
            x => x.ObservableProperty6,
            x => x.ObservableProperty7,
            x => x.ObservableProperty8,
            x => x.ObservableProperty9,
            x => x.ObservableProperty10,
            x => x.ObservableProperty11).ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        subj1.OnNext("test");
        subj2.OnNext("test");
        subj3.OnNext("test");
        subj4.OnNext("test");
        subj5.OnNext("test");
        subj6.OnNext("test");
        subj7.OnNext("test");
        subj8.OnNext("test");
        subj9.OnNext("test");
        subj10.OnNext("test");
        subj11.OnNext("test");
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    [Test]
    public async Task WhenAnyObservable_11Props_Sel()
    {
        var vm = new TestViewModel();
        var subj1 = new Subject<string>();
        vm.ObservableProperty1 = subj1;
        var subj2 = new Subject<string>();
        vm.ObservableProperty2 = subj2;
        var subj3 = new Subject<string>();
        vm.ObservableProperty3 = subj3;
        var subj4 = new Subject<string>();
        vm.ObservableProperty4 = subj4;
        var subj5 = new Subject<string>();
        vm.ObservableProperty5 = subj5;
        var subj6 = new Subject<string>();
        vm.ObservableProperty6 = subj6;
        var subj7 = new Subject<string>();
        vm.ObservableProperty7 = subj7;
        var subj8 = new Subject<string>();
        vm.ObservableProperty8 = subj8;
        var subj9 = new Subject<string>();
        vm.ObservableProperty9 = subj9;
        var subj10 = new Subject<string>();
        vm.ObservableProperty10 = subj10;
        var subj11 = new Subject<string>();
        vm.ObservableProperty11 = subj11;
        var list = new List<string>();
        vm.WhenAnyObservable(
                x => x.ObservableProperty1,
                x => x.ObservableProperty2,
                x => x.ObservableProperty3,
                x => x.ObservableProperty4,
                x => x.ObservableProperty5,
                x => x.ObservableProperty6,
                x => x.ObservableProperty7,
                x => x.ObservableProperty8,
                x => x.ObservableProperty9,
                x => x.ObservableProperty10,
                x => x.ObservableProperty11,
                (v1, v2, v3, v4, v5, v6, v7, v8, v9, v10, v11) => "x").ObserveOn(ImmediateScheduler.Instance)
            .Subscribe(list.Add);
        subj1.OnNext("test");
        subj2.OnNext("test");
        subj3.OnNext("test");
        subj4.OnNext("test");
        subj5.OnNext("test");
        subj6.OnNext("test");
        subj7.OnNext("test");
        subj8.OnNext("test");
        subj9.OnNext("test");
        subj10.OnNext("test");
        subj11.OnNext("test");
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    [Test]
    public async Task WhenAnyObservable_12Props()
    {
        var vm = new TestViewModel();
        var subj1 = new Subject<string>();
        vm.ObservableProperty1 = subj1;
        var subj2 = new Subject<string>();
        vm.ObservableProperty2 = subj2;
        var subj3 = new Subject<string>();
        vm.ObservableProperty3 = subj3;
        var subj4 = new Subject<string>();
        vm.ObservableProperty4 = subj4;
        var subj5 = new Subject<string>();
        vm.ObservableProperty5 = subj5;
        var subj6 = new Subject<string>();
        vm.ObservableProperty6 = subj6;
        var subj7 = new Subject<string>();
        vm.ObservableProperty7 = subj7;
        var subj8 = new Subject<string>();
        vm.ObservableProperty8 = subj8;
        var subj9 = new Subject<string>();
        vm.ObservableProperty9 = subj9;
        var subj10 = new Subject<string>();
        vm.ObservableProperty10 = subj10;
        var subj11 = new Subject<string>();
        vm.ObservableProperty11 = subj11;
        var subj12 = new Subject<string>();
        vm.ObservableProperty12 = subj12;
        var list = new List<string>();
        vm.WhenAnyObservable(
            x => x.ObservableProperty1,
            x => x.ObservableProperty2,
            x => x.ObservableProperty3,
            x => x.ObservableProperty4,
            x => x.ObservableProperty5,
            x => x.ObservableProperty6,
            x => x.ObservableProperty7,
            x => x.ObservableProperty8,
            x => x.ObservableProperty9,
            x => x.ObservableProperty10,
            x => x.ObservableProperty11,
            x => x.ObservableProperty12).ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        subj1.OnNext("test");
        subj2.OnNext("test");
        subj3.OnNext("test");
        subj4.OnNext("test");
        subj5.OnNext("test");
        subj6.OnNext("test");
        subj7.OnNext("test");
        subj8.OnNext("test");
        subj9.OnNext("test");
        subj10.OnNext("test");
        subj11.OnNext("test");
        subj12.OnNext("test");
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    [Test]
    public async Task WhenAnyObservable_12Props_Sel()
    {
        var vm = new TestViewModel();
        var subj1 = new Subject<string>();
        vm.ObservableProperty1 = subj1;
        var subj2 = new Subject<string>();
        vm.ObservableProperty2 = subj2;
        var subj3 = new Subject<string>();
        vm.ObservableProperty3 = subj3;
        var subj4 = new Subject<string>();
        vm.ObservableProperty4 = subj4;
        var subj5 = new Subject<string>();
        vm.ObservableProperty5 = subj5;
        var subj6 = new Subject<string>();
        vm.ObservableProperty6 = subj6;
        var subj7 = new Subject<string>();
        vm.ObservableProperty7 = subj7;
        var subj8 = new Subject<string>();
        vm.ObservableProperty8 = subj8;
        var subj9 = new Subject<string>();
        vm.ObservableProperty9 = subj9;
        var subj10 = new Subject<string>();
        vm.ObservableProperty10 = subj10;
        var subj11 = new Subject<string>();
        vm.ObservableProperty11 = subj11;
        var subj12 = new Subject<string>();
        vm.ObservableProperty12 = subj12;
        var list = new List<string>();
        vm.WhenAnyObservable(
                x => x.ObservableProperty1,
                x => x.ObservableProperty2,
                x => x.ObservableProperty3,
                x => x.ObservableProperty4,
                x => x.ObservableProperty5,
                x => x.ObservableProperty6,
                x => x.ObservableProperty7,
                x => x.ObservableProperty8,
                x => x.ObservableProperty9,
                x => x.ObservableProperty10,
                x => x.ObservableProperty11,
                x => x.ObservableProperty12,
                (v1, v2, v3, v4, v5, v6, v7, v8, v9, v10, v11, v12) => "x").ObserveOn(ImmediateScheduler.Instance)
            .Subscribe(list.Add);
        subj1.OnNext("test");
        subj2.OnNext("test");
        subj3.OnNext("test");
        subj4.OnNext("test");
        subj5.OnNext("test");
        subj6.OnNext("test");
        subj7.OnNext("test");
        subj8.OnNext("test");
        subj9.OnNext("test");
        subj10.OnNext("test");
        subj11.OnNext("test");
        subj12.OnNext("test");
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    [Test]
    public async Task WhenAnyObservable_1Prop()
    {
        var vm = new TestViewModel();
        var subj = new Subject<string>();
        vm.ObservableProperty1 = subj;
        var list = new List<string>();
        vm.WhenAnyObservable(x => x.ObservableProperty1).ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        subj.OnNext("test");
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    [Test]
    public async Task WhenAnyObservable_2Props()
    {
        var vm = new TestViewModel();
        var subj1 = new Subject<string>();
        vm.ObservableProperty1 = subj1;
        var subj2 = new Subject<string>();
        vm.ObservableProperty2 = subj2;
        var list = new List<string>();
        vm.WhenAnyObservable(
            x => x.ObservableProperty1,
            x => x.ObservableProperty2).ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        subj1.OnNext("test");
        subj2.OnNext("test");
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    [Test]
    public async Task WhenAnyObservable_2Props_Sel()
    {
        var vm = new TestViewModel();
        var subj1 = new Subject<string>();
        vm.ObservableProperty1 = subj1;
        var subj2 = new Subject<string>();
        vm.ObservableProperty2 = subj2;
        var list = new List<string>();
        vm.WhenAnyObservable(
            x => x.ObservableProperty1,
            x => x.ObservableProperty2,
            (v1, v2) => "x").ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        subj1.OnNext("test");
        subj2.OnNext("test");
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    [Test]
    public async Task WhenAnyObservable_3Props()
    {
        var vm = new TestViewModel();
        var subj1 = new Subject<string>();
        vm.ObservableProperty1 = subj1;
        var subj2 = new Subject<string>();
        vm.ObservableProperty2 = subj2;
        var subj3 = new Subject<string>();
        vm.ObservableProperty3 = subj3;
        var list = new List<string>();
        vm.WhenAnyObservable(
            x => x.ObservableProperty1,
            x => x.ObservableProperty2,
            x => x.ObservableProperty3).ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        subj1.OnNext("test");
        subj2.OnNext("test");
        subj3.OnNext("test");
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    [Test]
    public async Task WhenAnyObservable_3Props_Sel()
    {
        var vm = new TestViewModel();
        var subj1 = new Subject<string>();
        vm.ObservableProperty1 = subj1;
        var subj2 = new Subject<string>();
        vm.ObservableProperty2 = subj2;
        var subj3 = new Subject<string>();
        vm.ObservableProperty3 = subj3;
        var list = new List<string>();
        vm.WhenAnyObservable(
            x => x.ObservableProperty1,
            x => x.ObservableProperty2,
            x => x.ObservableProperty3,
            (v1, v2, v3) => "x").ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        subj1.OnNext("test");
        subj2.OnNext("test");
        subj3.OnNext("test");
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    [Test]
    public async Task WhenAnyObservable_4Props()
    {
        var vm = new TestViewModel();
        var subj1 = new Subject<string>();
        vm.ObservableProperty1 = subj1;
        var subj2 = new Subject<string>();
        vm.ObservableProperty2 = subj2;
        var subj3 = new Subject<string>();
        vm.ObservableProperty3 = subj3;
        var subj4 = new Subject<string>();
        vm.ObservableProperty4 = subj4;
        var list = new List<string>();
        vm.WhenAnyObservable(
            x => x.ObservableProperty1,
            x => x.ObservableProperty2,
            x => x.ObservableProperty3,
            x => x.ObservableProperty4).ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        subj1.OnNext("test");
        subj2.OnNext("test");
        subj3.OnNext("test");
        subj4.OnNext("test");
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    [Test]
    public async Task WhenAnyObservable_4Props_Sel()
    {
        var vm = new TestViewModel();
        var subj1 = new Subject<string>();
        vm.ObservableProperty1 = subj1;
        var subj2 = new Subject<string>();
        vm.ObservableProperty2 = subj2;
        var subj3 = new Subject<string>();
        vm.ObservableProperty3 = subj3;
        var subj4 = new Subject<string>();
        vm.ObservableProperty4 = subj4;
        var list = new List<string>();
        vm.WhenAnyObservable(
            x => x.ObservableProperty1,
            x => x.ObservableProperty2,
            x => x.ObservableProperty3,
            x => x.ObservableProperty4,
            (v1, v2, v3, v4) => "x").ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        subj1.OnNext("test");
        subj2.OnNext("test");
        subj3.OnNext("test");
        subj4.OnNext("test");
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    [Test]
    public async Task WhenAnyObservable_5Props()
    {
        var vm = new TestViewModel();
        var subj1 = new Subject<string>();
        vm.ObservableProperty1 = subj1;
        var subj2 = new Subject<string>();
        vm.ObservableProperty2 = subj2;
        var subj3 = new Subject<string>();
        vm.ObservableProperty3 = subj3;
        var subj4 = new Subject<string>();
        vm.ObservableProperty4 = subj4;
        var subj5 = new Subject<string>();
        vm.ObservableProperty5 = subj5;
        var list = new List<string>();
        vm.WhenAnyObservable(
            x => x.ObservableProperty1,
            x => x.ObservableProperty2,
            x => x.ObservableProperty3,
            x => x.ObservableProperty4,
            x => x.ObservableProperty5).ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        subj1.OnNext("test");
        subj2.OnNext("test");
        subj3.OnNext("test");
        subj4.OnNext("test");
        subj5.OnNext("test");
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    [Test]
    public async Task WhenAnyObservable_5Props_Sel()
    {
        var vm = new TestViewModel();
        var subj1 = new Subject<string>();
        vm.ObservableProperty1 = subj1;
        var subj2 = new Subject<string>();
        vm.ObservableProperty2 = subj2;
        var subj3 = new Subject<string>();
        vm.ObservableProperty3 = subj3;
        var subj4 = new Subject<string>();
        vm.ObservableProperty4 = subj4;
        var subj5 = new Subject<string>();
        vm.ObservableProperty5 = subj5;
        var list = new List<string>();
        vm.WhenAnyObservable(
            x => x.ObservableProperty1,
            x => x.ObservableProperty2,
            x => x.ObservableProperty3,
            x => x.ObservableProperty4,
            x => x.ObservableProperty5,
            (v1, v2, v3, v4, v5) => "x").ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        subj1.OnNext("test");
        subj2.OnNext("test");
        subj3.OnNext("test");
        subj4.OnNext("test");
        subj5.OnNext("test");
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    [Test]
    public async Task WhenAnyObservable_6Props()
    {
        var vm = new TestViewModel();
        var subj1 = new Subject<string>();
        vm.ObservableProperty1 = subj1;
        var subj2 = new Subject<string>();
        vm.ObservableProperty2 = subj2;
        var subj3 = new Subject<string>();
        vm.ObservableProperty3 = subj3;
        var subj4 = new Subject<string>();
        vm.ObservableProperty4 = subj4;
        var subj5 = new Subject<string>();
        vm.ObservableProperty5 = subj5;
        var subj6 = new Subject<string>();
        vm.ObservableProperty6 = subj6;
        var list = new List<string>();
        vm.WhenAnyObservable(
            x => x.ObservableProperty1,
            x => x.ObservableProperty2,
            x => x.ObservableProperty3,
            x => x.ObservableProperty4,
            x => x.ObservableProperty5,
            x => x.ObservableProperty6).ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        subj1.OnNext("test");
        subj2.OnNext("test");
        subj3.OnNext("test");
        subj4.OnNext("test");
        subj5.OnNext("test");
        subj6.OnNext("test");
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    [Test]
    public async Task WhenAnyObservable_6Props_Sel()
    {
        var vm = new TestViewModel();
        var subj1 = new Subject<string>();
        vm.ObservableProperty1 = subj1;
        var subj2 = new Subject<string>();
        vm.ObservableProperty2 = subj2;
        var subj3 = new Subject<string>();
        vm.ObservableProperty3 = subj3;
        var subj4 = new Subject<string>();
        vm.ObservableProperty4 = subj4;
        var subj5 = new Subject<string>();
        vm.ObservableProperty5 = subj5;
        var subj6 = new Subject<string>();
        vm.ObservableProperty6 = subj6;
        var list = new List<string>();
        vm.WhenAnyObservable(
            x => x.ObservableProperty1,
            x => x.ObservableProperty2,
            x => x.ObservableProperty3,
            x => x.ObservableProperty4,
            x => x.ObservableProperty5,
            x => x.ObservableProperty6,
            (v1, v2, v3, v4, v5, v6) => "x").ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        subj1.OnNext("test");
        subj2.OnNext("test");
        subj3.OnNext("test");
        subj4.OnNext("test");
        subj5.OnNext("test");
        subj6.OnNext("test");
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    [Test]
    public async Task WhenAnyObservable_7Props()
    {
        var vm = new TestViewModel();
        var subj1 = new Subject<string>();
        vm.ObservableProperty1 = subj1;
        var subj2 = new Subject<string>();
        vm.ObservableProperty2 = subj2;
        var subj3 = new Subject<string>();
        vm.ObservableProperty3 = subj3;
        var subj4 = new Subject<string>();
        vm.ObservableProperty4 = subj4;
        var subj5 = new Subject<string>();
        vm.ObservableProperty5 = subj5;
        var subj6 = new Subject<string>();
        vm.ObservableProperty6 = subj6;
        var subj7 = new Subject<string>();
        vm.ObservableProperty7 = subj7;
        var list = new List<string>();
        vm.WhenAnyObservable(
            x => x.ObservableProperty1,
            x => x.ObservableProperty2,
            x => x.ObservableProperty3,
            x => x.ObservableProperty4,
            x => x.ObservableProperty5,
            x => x.ObservableProperty6,
            x => x.ObservableProperty7).ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        subj1.OnNext("test");
        subj2.OnNext("test");
        subj3.OnNext("test");
        subj4.OnNext("test");
        subj5.OnNext("test");
        subj6.OnNext("test");
        subj7.OnNext("test");
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    [Test]
    public async Task WhenAnyObservable_7Props_Sel()
    {
        var vm = new TestViewModel();
        var subj1 = new Subject<string>();
        vm.ObservableProperty1 = subj1;
        var subj2 = new Subject<string>();
        vm.ObservableProperty2 = subj2;
        var subj3 = new Subject<string>();
        vm.ObservableProperty3 = subj3;
        var subj4 = new Subject<string>();
        vm.ObservableProperty4 = subj4;
        var subj5 = new Subject<string>();
        vm.ObservableProperty5 = subj5;
        var subj6 = new Subject<string>();
        vm.ObservableProperty6 = subj6;
        var subj7 = new Subject<string>();
        vm.ObservableProperty7 = subj7;
        var list = new List<string>();
        vm.WhenAnyObservable(
            x => x.ObservableProperty1,
            x => x.ObservableProperty2,
            x => x.ObservableProperty3,
            x => x.ObservableProperty4,
            x => x.ObservableProperty5,
            x => x.ObservableProperty6,
            x => x.ObservableProperty7,
            (v1, v2, v3, v4, v5, v6, v7) => "x").ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        subj1.OnNext("test");
        subj2.OnNext("test");
        subj3.OnNext("test");
        subj4.OnNext("test");
        subj5.OnNext("test");
        subj6.OnNext("test");
        subj7.OnNext("test");
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    [Test]
    public async Task WhenAnyObservable_8Props()
    {
        var vm = new TestViewModel();
        var subj1 = new Subject<string>();
        vm.ObservableProperty1 = subj1;
        var subj2 = new Subject<string>();
        vm.ObservableProperty2 = subj2;
        var subj3 = new Subject<string>();
        vm.ObservableProperty3 = subj3;
        var subj4 = new Subject<string>();
        vm.ObservableProperty4 = subj4;
        var subj5 = new Subject<string>();
        vm.ObservableProperty5 = subj5;
        var subj6 = new Subject<string>();
        vm.ObservableProperty6 = subj6;
        var subj7 = new Subject<string>();
        vm.ObservableProperty7 = subj7;
        var subj8 = new Subject<string>();
        vm.ObservableProperty8 = subj8;
        var list = new List<string>();
        vm.WhenAnyObservable(
            x => x.ObservableProperty1,
            x => x.ObservableProperty2,
            x => x.ObservableProperty3,
            x => x.ObservableProperty4,
            x => x.ObservableProperty5,
            x => x.ObservableProperty6,
            x => x.ObservableProperty7,
            x => x.ObservableProperty8).ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        subj1.OnNext("test");
        subj2.OnNext("test");
        subj3.OnNext("test");
        subj4.OnNext("test");
        subj5.OnNext("test");
        subj6.OnNext("test");
        subj7.OnNext("test");
        subj8.OnNext("test");
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    [Test]
    public async Task WhenAnyObservable_8Props_Sel()
    {
        var vm = new TestViewModel();
        var subj1 = new Subject<string>();
        vm.ObservableProperty1 = subj1;
        var subj2 = new Subject<string>();
        vm.ObservableProperty2 = subj2;
        var subj3 = new Subject<string>();
        vm.ObservableProperty3 = subj3;
        var subj4 = new Subject<string>();
        vm.ObservableProperty4 = subj4;
        var subj5 = new Subject<string>();
        vm.ObservableProperty5 = subj5;
        var subj6 = new Subject<string>();
        vm.ObservableProperty6 = subj6;
        var subj7 = new Subject<string>();
        vm.ObservableProperty7 = subj7;
        var subj8 = new Subject<string>();
        vm.ObservableProperty8 = subj8;
        var list = new List<string>();
        vm.WhenAnyObservable(
            x => x.ObservableProperty1,
            x => x.ObservableProperty2,
            x => x.ObservableProperty3,
            x => x.ObservableProperty4,
            x => x.ObservableProperty5,
            x => x.ObservableProperty6,
            x => x.ObservableProperty7,
            x => x.ObservableProperty8,
            (v1, v2, v3, v4, v5, v6, v7, v8) => "x").ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        subj1.OnNext("test");
        subj2.OnNext("test");
        subj3.OnNext("test");
        subj4.OnNext("test");
        subj5.OnNext("test");
        subj6.OnNext("test");
        subj7.OnNext("test");
        subj8.OnNext("test");
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    [Test]
    public async Task WhenAnyObservable_9Props()
    {
        var vm = new TestViewModel();
        var subj1 = new Subject<string>();
        vm.ObservableProperty1 = subj1;
        var subj2 = new Subject<string>();
        vm.ObservableProperty2 = subj2;
        var subj3 = new Subject<string>();
        vm.ObservableProperty3 = subj3;
        var subj4 = new Subject<string>();
        vm.ObservableProperty4 = subj4;
        var subj5 = new Subject<string>();
        vm.ObservableProperty5 = subj5;
        var subj6 = new Subject<string>();
        vm.ObservableProperty6 = subj6;
        var subj7 = new Subject<string>();
        vm.ObservableProperty7 = subj7;
        var subj8 = new Subject<string>();
        vm.ObservableProperty8 = subj8;
        var subj9 = new Subject<string>();
        vm.ObservableProperty9 = subj9;
        var list = new List<string>();
        vm.WhenAnyObservable(
            x => x.ObservableProperty1,
            x => x.ObservableProperty2,
            x => x.ObservableProperty3,
            x => x.ObservableProperty4,
            x => x.ObservableProperty5,
            x => x.ObservableProperty6,
            x => x.ObservableProperty7,
            x => x.ObservableProperty8,
            x => x.ObservableProperty9).ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        subj1.OnNext("test");
        subj2.OnNext("test");
        subj3.OnNext("test");
        subj4.OnNext("test");
        subj5.OnNext("test");
        subj6.OnNext("test");
        subj7.OnNext("test");
        subj8.OnNext("test");
        subj9.OnNext("test");
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    [Test]
    public async Task WhenAnyObservable_9Props_Sel()
    {
        var vm = new TestViewModel();
        var subj1 = new Subject<string>();
        vm.ObservableProperty1 = subj1;
        var subj2 = new Subject<string>();
        vm.ObservableProperty2 = subj2;
        var subj3 = new Subject<string>();
        vm.ObservableProperty3 = subj3;
        var subj4 = new Subject<string>();
        vm.ObservableProperty4 = subj4;
        var subj5 = new Subject<string>();
        vm.ObservableProperty5 = subj5;
        var subj6 = new Subject<string>();
        vm.ObservableProperty6 = subj6;
        var subj7 = new Subject<string>();
        vm.ObservableProperty7 = subj7;
        var subj8 = new Subject<string>();
        vm.ObservableProperty8 = subj8;
        var subj9 = new Subject<string>();
        vm.ObservableProperty9 = subj9;
        var list = new List<string>();
        vm.WhenAnyObservable(
            x => x.ObservableProperty1,
            x => x.ObservableProperty2,
            x => x.ObservableProperty3,
            x => x.ObservableProperty4,
            x => x.ObservableProperty5,
            x => x.ObservableProperty6,
            x => x.ObservableProperty7,
            x => x.ObservableProperty8,
            x => x.ObservableProperty9,
            (v1, v2, v3, v4, v5, v6, v7, v8, v9) => "x").ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        subj1.OnNext("test");
        subj2.OnNext("test");
        subj3.OnNext("test");
        subj4.OnNext("test");
        subj5.OnNext("test");
        subj6.OnNext("test");
        subj7.OnNext("test");
        subj8.OnNext("test");
        subj9.OnNext("test");
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    [Test]
    public async Task WhenAnyValue_10Props_Sel()
    {
        var vm = new TestViewModel();
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
                (v1, v2, v3, v4, v5, v6, v7, v8, v9, v10) => "x").ObserveOn(ImmediateScheduler.Instance)
            .Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    [Test]
    public async Task WhenAnyValue_10Props_Sel_Dist()
    {
        var vm = new TestViewModel();
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
            (v1, v2, v3, v4, v5, v6, v7, v8, v9, v10) => "x",
            true).ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    [Test]
    public async Task WhenAnyValue_10Props_Sel_Str()
    {
        var vm = new TestViewModel();
        var list = new List<string>();
        vm.WhenAnyValue<TestViewModel, string, string?, string?, string?, string?, string?, string?, string?, string?,
                string?, string?>(
                nameof(TestViewModel.Property1),
                nameof(TestViewModel.Property2),
                nameof(TestViewModel.Property3),
                nameof(TestViewModel.Property4),
                nameof(TestViewModel.Property5),
                nameof(TestViewModel.Property6),
                nameof(TestViewModel.Property7),
                nameof(TestViewModel.Property8),
                nameof(TestViewModel.Property9),
                nameof(TestViewModel.Property10),
                (v1, v2, v3, v4, v5, v6, v7, v8, v9, v10) => "x").ObserveOn(ImmediateScheduler.Instance)
            .Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    [Test]
    public async Task WhenAnyValue_10Props_Sel_Str_Dist()
    {
        var vm = new TestViewModel();
        var list = new List<string>();
        vm.WhenAnyValue<TestViewModel, string, string?, string?, string?, string?, string?, string?, string?, string?,
            string?, string?>(
            nameof(TestViewModel.Property1),
            nameof(TestViewModel.Property2),
            nameof(TestViewModel.Property3),
            nameof(TestViewModel.Property4),
            nameof(TestViewModel.Property5),
            nameof(TestViewModel.Property6),
            nameof(TestViewModel.Property7),
            nameof(TestViewModel.Property8),
            nameof(TestViewModel.Property9),
            nameof(TestViewModel.Property10),
            (v1, v2, v3, v4, v5, v6, v7, v8, v9, v10) => "x",
            false).ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    [Test]
    public async Task WhenAnyValue_11Props_Sel()
    {
        var vm = new TestViewModel();
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
                (v1, v2, v3, v4, v5, v6, v7, v8, v9, v10, v11) => "x").ObserveOn(ImmediateScheduler.Instance)
            .Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    [Test]
    public async Task WhenAnyValue_11Props_Sel_Dist()
    {
        var vm = new TestViewModel();
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
            (v1, v2, v3, v4, v5, v6, v7, v8, v9, v10, v11) => "x",
            true).ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    [Test]
    public async Task WhenAnyValue_11Props_Sel_Str()
    {
        var vm = new TestViewModel();
        var list = new List<string>();
        vm.WhenAnyValue<TestViewModel, string, string?, string?, string?, string?, string?, string?, string?, string?,
                string?, string?, string?>(
                nameof(TestViewModel.Property1),
                nameof(TestViewModel.Property2),
                nameof(TestViewModel.Property3),
                nameof(TestViewModel.Property4),
                nameof(TestViewModel.Property5),
                nameof(TestViewModel.Property6),
                nameof(TestViewModel.Property7),
                nameof(TestViewModel.Property8),
                nameof(TestViewModel.Property9),
                nameof(TestViewModel.Property10),
                nameof(TestViewModel.Property11),
                (v1, v2, v3, v4, v5, v6, v7, v8, v9, v10, v11) => "x").ObserveOn(ImmediateScheduler.Instance)
            .Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    [Test]
    public async Task WhenAnyValue_11Props_Sel_Str_Dist()
    {
        var vm = new TestViewModel();
        var list = new List<string>();
        vm.WhenAnyValue<TestViewModel, string, string?, string?, string?, string?, string?, string?, string?, string?,
            string?, string?, string?>(
            nameof(TestViewModel.Property1),
            nameof(TestViewModel.Property2),
            nameof(TestViewModel.Property3),
            nameof(TestViewModel.Property4),
            nameof(TestViewModel.Property5),
            nameof(TestViewModel.Property6),
            nameof(TestViewModel.Property7),
            nameof(TestViewModel.Property8),
            nameof(TestViewModel.Property9),
            nameof(TestViewModel.Property10),
            nameof(TestViewModel.Property11),
            (v1, v2, v3, v4, v5, v6, v7, v8, v9, v10, v11) => "x",
            false).ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    [Test]
    public async Task WhenAnyValue_12Props_Sel()
    {
        var vm = new TestViewModel();
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
                (v1, v2, v3, v4, v5, v6, v7, v8, v9, v10, v11, v12) => "x").ObserveOn(ImmediateScheduler.Instance)
            .Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    [Test]
    public async Task WhenAnyValue_12Props_Sel_Dist()
    {
        var vm = new TestViewModel();
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
            (v1, v2, v3, v4, v5, v6, v7, v8, v9, v10, v11, v12) => "x",
            true).ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    [Test]
    public async Task WhenAnyValue_12Props_Sel_Str()
    {
        var vm = new TestViewModel();
        var list = new List<string>();
        vm.WhenAnyValue<TestViewModel, string, string?, string?, string?, string?, string?, string?, string?, string?,
                string?, string?, string?, string?>(
                nameof(TestViewModel.Property1),
                nameof(TestViewModel.Property2),
                nameof(TestViewModel.Property3),
                nameof(TestViewModel.Property4),
                nameof(TestViewModel.Property5),
                nameof(TestViewModel.Property6),
                nameof(TestViewModel.Property7),
                nameof(TestViewModel.Property8),
                nameof(TestViewModel.Property9),
                nameof(TestViewModel.Property10),
                nameof(TestViewModel.Property11),
                nameof(TestViewModel.Property12),
                (v1, v2, v3, v4, v5, v6, v7, v8, v9, v10, v11, v12) => "x").ObserveOn(ImmediateScheduler.Instance)
            .Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    [Test]
    public async Task WhenAnyValue_12Props_Sel_Str_Dist()
    {
        var vm = new TestViewModel();
        var list = new List<string>();
        vm.WhenAnyValue<TestViewModel, string, string?, string?, string?, string?, string?, string?, string?, string?,
            string?, string?, string?, string?>(
            nameof(TestViewModel.Property1),
            nameof(TestViewModel.Property2),
            nameof(TestViewModel.Property3),
            nameof(TestViewModel.Property4),
            nameof(TestViewModel.Property5),
            nameof(TestViewModel.Property6),
            nameof(TestViewModel.Property7),
            nameof(TestViewModel.Property8),
            nameof(TestViewModel.Property9),
            nameof(TestViewModel.Property10),
            nameof(TestViewModel.Property11),
            nameof(TestViewModel.Property12),
            (v1, v2, v3, v4, v5, v6, v7, v8, v9, v10, v11, v12) => "x",
            false).ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    [Test]
    public async Task WhenAnyValue_1Prop_Expr()
    {
        var vm = new TestViewModel();
        var list = new List<string?>();
        vm.WhenAnyValue(x => x.Property1).ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
        vm.Property1 = "a";
        await Assert.That(list).Count().IsGreaterThan(1);
    }

    [Test]
    public async Task WhenAnyValue_1Prop_Expr_Dist()
    {
        var vm = new TestViewModel();
        var list = new List<string?>();
        vm.WhenAnyValue(x => x.Property1, true).ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    [Test]
    public async Task WhenAnyValue_1Prop_Str()
    {
        var vm = new TestViewModel();
        var list = new List<string?>();
        vm.WhenAnyValue<TestViewModel, string?>(nameof(TestViewModel.Property1)).ObserveOn(ImmediateScheduler.Instance)
            .Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    [Test]
    public async Task WhenAnyValue_1Prop_Str_Dist()
    {
        var vm = new TestViewModel();
        var list = new List<string?>();
        vm.WhenAnyValue<TestViewModel, string?>(nameof(TestViewModel.Property1), false)
            .ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    [Test]
    public async Task WhenAnyValue_1Props_Sel()
    {
        var vm = new TestViewModel();
        var list = new List<string>();
        Func<string?, string> selector = v1 => "x";
        vm.WhenAnyValue(x => x.Property1, selector).ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    [Test]
    public async Task WhenAnyValue_1Props_Sel_Dist()
    {
        var vm = new TestViewModel();
        var list = new List<string>();
        Func<string?, string> selector = v1 => "x";
        vm.WhenAnyValue(x => x.Property1, selector, true).ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    [Test]
    public async Task WhenAnyValue_1Props_Sel_Str()
    {
        var vm = new TestViewModel();
        var list = new List<string>();
        vm.WhenAnyValue<TestViewModel, string, string?>(
            nameof(TestViewModel.Property1),
            v1 => "x").ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    [Test]
    public async Task WhenAnyValue_1Props_Sel_Str_Dist()
    {
        var vm = new TestViewModel();
        var list = new List<string>();
        vm.WhenAnyValue<TestViewModel, string, string?>(
            nameof(TestViewModel.Property1),
            v1 => "x",
            false).ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    [Test]
    public async Task WhenAnyValue_2Props_Sel()
    {
        var vm = new TestViewModel();
        var list = new List<string>();
        vm.WhenAnyValue(
            x => x.Property1,
            x => x.Property2,
            (v1, v2) => "x").ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    [Test]
    public async Task WhenAnyValue_2Props_Sel_Dist()
    {
        var vm = new TestViewModel();
        var list = new List<string>();
        vm.WhenAnyValue(
            x => x.Property1,
            x => x.Property2,
            (v1, v2) => "x",
            true).ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    [Test]
    public async Task WhenAnyValue_2Props_Sel_Str()
    {
        var vm = new TestViewModel();
        var list = new List<string>();
        vm.WhenAnyValue<TestViewModel, string, string?, string?>(
            nameof(TestViewModel.Property1),
            nameof(TestViewModel.Property2),
            (v1, v2) => "x").ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    [Test]
    public async Task WhenAnyValue_2Props_Sel_Str_Dist()
    {
        var vm = new TestViewModel();
        var list = new List<string>();
        vm.WhenAnyValue<TestViewModel, string, string?, string?>(
            nameof(TestViewModel.Property1),
            nameof(TestViewModel.Property2),
            (v1, v2) => "x",
            false).ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    [Test]
    public async Task WhenAnyValue_2Props_Tuple_Expr()
    {
        var vm = new TestViewModel();
        var list = new List<(string?, string?)>();
        vm.WhenAnyValue(
            x => x.Property1,
            x => x.Property2).ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    [Test]
    public async Task WhenAnyValue_2Props_Tuple_Expr_Dist()
    {
        var vm = new TestViewModel();
        var list = new List<(string?, string?)>();
        vm.WhenAnyValue(
            x => x.Property1,
            x => x.Property2,
            true).ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    [Test]
    public async Task WhenAnyValue_2Props_Tuple_Str()
    {
        var vm = new TestViewModel();
        var list = new List<(string?, string?)>();
        vm.WhenAnyValue<TestViewModel, string?, string?>(
            nameof(TestViewModel.Property1),
            nameof(TestViewModel.Property2)).ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    [Test]
    public async Task WhenAnyValue_2Props_Tuple_Str_Dist()
    {
        var vm = new TestViewModel();
        var list = new List<(string?, string?)>();
        vm.WhenAnyValue<TestViewModel, string?, string?>(
            nameof(TestViewModel.Property1),
            nameof(TestViewModel.Property2),
            false).ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    [Test]
    public async Task WhenAnyValue_3Props_Sel()
    {
        var vm = new TestViewModel();
        var list = new List<string>();
        vm.WhenAnyValue(
            x => x.Property1,
            x => x.Property2,
            x => x.Property3,
            (v1, v2, v3) => "x").ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    [Test]
    public async Task WhenAnyValue_3Props_Sel_Dist()
    {
        var vm = new TestViewModel();
        var list = new List<string>();
        vm.WhenAnyValue(
            x => x.Property1,
            x => x.Property2,
            x => x.Property3,
            (v1, v2, v3) => "x",
            true).ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    [Test]
    public async Task WhenAnyValue_3Props_Sel_Str()
    {
        var vm = new TestViewModel();
        var list = new List<string>();
        vm.WhenAnyValue<TestViewModel, string, string?, string?, string?>(
            nameof(TestViewModel.Property1),
            nameof(TestViewModel.Property2),
            nameof(TestViewModel.Property3),
            (v1, v2, v3) => "x").ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    [Test]
    public async Task WhenAnyValue_3Props_Sel_Str_Dist()
    {
        var vm = new TestViewModel();
        var list = new List<string>();
        vm.WhenAnyValue<TestViewModel, string, string?, string?, string?>(
            nameof(TestViewModel.Property1),
            nameof(TestViewModel.Property2),
            nameof(TestViewModel.Property3),
            (v1, v2, v3) => "x",
            false).ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    [Test]
    public async Task WhenAnyValue_3Props_Tuple_Expr()
    {
        var vm = new TestViewModel();
        var list = new List<(string?, string?, string?)>();
        vm.WhenAnyValue(
            x => x.Property1,
            x => x.Property2,
            x => x.Property3).ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    [Test]
    public async Task WhenAnyValue_3Props_Tuple_Expr_Dist()
    {
        var vm = new TestViewModel();
        var list = new List<(string?, string?, string?)>();
        vm.WhenAnyValue(
            x => x.Property1,
            x => x.Property2,
            x => x.Property3,
            true).ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    [Test]
    public async Task WhenAnyValue_3Props_Tuple_Str()
    {
        var vm = new TestViewModel();
        var list = new List<(string?, string?, string?)>();
        vm.WhenAnyValue<TestViewModel, string?, string?, string?>(
            nameof(TestViewModel.Property1),
            nameof(TestViewModel.Property2),
            nameof(TestViewModel.Property3)).ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    [Test]
    public async Task WhenAnyValue_3Props_Tuple_Str_Dist()
    {
        var vm = new TestViewModel();
        var list = new List<(string?, string?, string?)>();
        vm.WhenAnyValue<TestViewModel, string?, string?, string?>(
            nameof(TestViewModel.Property1),
            nameof(TestViewModel.Property2),
            nameof(TestViewModel.Property3),
            false).ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    [Test]
    public async Task WhenAnyValue_4Props_Sel()
    {
        var vm = new TestViewModel();
        var list = new List<string>();
        vm.WhenAnyValue(
            x => x.Property1,
            x => x.Property2,
            x => x.Property3,
            x => x.Property4,
            (v1, v2, v3, v4) => "x").ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    [Test]
    public async Task WhenAnyValue_4Props_Sel_Dist()
    {
        var vm = new TestViewModel();
        var list = new List<string>();
        vm.WhenAnyValue(
            x => x.Property1,
            x => x.Property2,
            x => x.Property3,
            x => x.Property4,
            (v1, v2, v3, v4) => "x",
            true).ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    [Test]
    public async Task WhenAnyValue_4Props_Sel_Str()
    {
        var vm = new TestViewModel();
        var list = new List<string>();
        vm.WhenAnyValue<TestViewModel, string, string?, string?, string?, string?>(
            nameof(TestViewModel.Property1),
            nameof(TestViewModel.Property2),
            nameof(TestViewModel.Property3),
            nameof(TestViewModel.Property4),
            (v1, v2, v3, v4) => "x").ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    [Test]
    public async Task WhenAnyValue_4Props_Sel_Str_Dist()
    {
        var vm = new TestViewModel();
        var list = new List<string>();
        vm.WhenAnyValue<TestViewModel, string, string?, string?, string?, string?>(
            nameof(TestViewModel.Property1),
            nameof(TestViewModel.Property2),
            nameof(TestViewModel.Property3),
            nameof(TestViewModel.Property4),
            (v1, v2, v3, v4) => "x",
            false).ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    [Test]
    public async Task WhenAnyValue_4Props_Tuple_Expr()
    {
        var vm = new TestViewModel();
        var list = new List<(string?, string?, string?, string?)>();
        vm.WhenAnyValue(
            x => x.Property1,
            x => x.Property2,
            x => x.Property3,
            x => x.Property4).ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    [Test]
    public async Task WhenAnyValue_4Props_Tuple_Expr_Dist()
    {
        var vm = new TestViewModel();
        var list = new List<(string?, string?, string?, string?)>();
        vm.WhenAnyValue(
            x => x.Property1,
            x => x.Property2,
            x => x.Property3,
            x => x.Property4,
            true).ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    [Test]
    public async Task WhenAnyValue_4Props_Tuple_Str()
    {
        var vm = new TestViewModel();
        var list = new List<(string?, string?, string?, string?)>();
        vm.WhenAnyValue<TestViewModel, string?, string?, string?, string?>(
            nameof(TestViewModel.Property1),
            nameof(TestViewModel.Property2),
            nameof(TestViewModel.Property3),
            nameof(TestViewModel.Property4)).ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    [Test]
    public async Task WhenAnyValue_4Props_Tuple_Str_Dist()
    {
        var vm = new TestViewModel();
        var list = new List<(string?, string?, string?, string?)>();
        vm.WhenAnyValue<TestViewModel, string?, string?, string?, string?>(
            nameof(TestViewModel.Property1),
            nameof(TestViewModel.Property2),
            nameof(TestViewModel.Property3),
            nameof(TestViewModel.Property4),
            false).ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    [Test]
    public async Task WhenAnyValue_5Props_Sel()
    {
        var vm = new TestViewModel();
        var list = new List<string>();
        vm.WhenAnyValue(
            x => x.Property1,
            x => x.Property2,
            x => x.Property3,
            x => x.Property4,
            x => x.Property5,
            (v1, v2, v3, v4, v5) => "x").ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    [Test]
    public async Task WhenAnyValue_5Props_Sel_Dist()
    {
        var vm = new TestViewModel();
        var list = new List<string>();
        vm.WhenAnyValue(
            x => x.Property1,
            x => x.Property2,
            x => x.Property3,
            x => x.Property4,
            x => x.Property5,
            (v1, v2, v3, v4, v5) => "x",
            true).ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    [Test]
    public async Task WhenAnyValue_5Props_Sel_Str()
    {
        var vm = new TestViewModel();
        var list = new List<string>();
        vm.WhenAnyValue<TestViewModel, string, string?, string?, string?, string?, string?>(
            nameof(TestViewModel.Property1),
            nameof(TestViewModel.Property2),
            nameof(TestViewModel.Property3),
            nameof(TestViewModel.Property4),
            nameof(TestViewModel.Property5),
            (v1, v2, v3, v4, v5) => "x").ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    [Test]
    public async Task WhenAnyValue_5Props_Sel_Str_Dist()
    {
        var vm = new TestViewModel();
        var list = new List<string>();
        vm.WhenAnyValue<TestViewModel, string, string?, string?, string?, string?, string?>(
            nameof(TestViewModel.Property1),
            nameof(TestViewModel.Property2),
            nameof(TestViewModel.Property3),
            nameof(TestViewModel.Property4),
            nameof(TestViewModel.Property5),
            (v1, v2, v3, v4, v5) => "x",
            false).ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    [Test]
    public async Task WhenAnyValue_5Props_Tuple_Expr()
    {
        var vm = new TestViewModel();
        var list = new List<(string?, string?, string?, string?, string?)>();
        vm.WhenAnyValue(
            x => x.Property1,
            x => x.Property2,
            x => x.Property3,
            x => x.Property4,
            x => x.Property5).ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    [Test]
    public async Task WhenAnyValue_5Props_Tuple_Expr_Dist()
    {
        var vm = new TestViewModel();
        var list = new List<(string?, string?, string?, string?, string?)>();
        vm.WhenAnyValue(
            x => x.Property1,
            x => x.Property2,
            x => x.Property3,
            x => x.Property4,
            x => x.Property5,
            true).ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    [Test]
    public async Task WhenAnyValue_5Props_Tuple_Str()
    {
        var vm = new TestViewModel();
        var list = new List<(string?, string?, string?, string?, string?)>();
        vm.WhenAnyValue<TestViewModel, string?, string?, string?, string?, string?>(
            nameof(TestViewModel.Property1),
            nameof(TestViewModel.Property2),
            nameof(TestViewModel.Property3),
            nameof(TestViewModel.Property4),
            nameof(TestViewModel.Property5)).ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    [Test]
    public async Task WhenAnyValue_5Props_Tuple_Str_Dist()
    {
        var vm = new TestViewModel();
        var list = new List<(string?, string?, string?, string?, string?)>();
        vm.WhenAnyValue<TestViewModel, string?, string?, string?, string?, string?>(
            nameof(TestViewModel.Property1),
            nameof(TestViewModel.Property2),
            nameof(TestViewModel.Property3),
            nameof(TestViewModel.Property4),
            nameof(TestViewModel.Property5),
            false).ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    [Test]
    public async Task WhenAnyValue_6Props_Sel()
    {
        var vm = new TestViewModel();
        var list = new List<string>();
        vm.WhenAnyValue(
            x => x.Property1,
            x => x.Property2,
            x => x.Property3,
            x => x.Property4,
            x => x.Property5,
            x => x.Property6,
            (v1, v2, v3, v4, v5, v6) => "x").ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    [Test]
    public async Task WhenAnyValue_6Props_Sel_Dist()
    {
        var vm = new TestViewModel();
        var list = new List<string>();
        vm.WhenAnyValue(
            x => x.Property1,
            x => x.Property2,
            x => x.Property3,
            x => x.Property4,
            x => x.Property5,
            x => x.Property6,
            (v1, v2, v3, v4, v5, v6) => "x",
            true).ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    [Test]
    public async Task WhenAnyValue_6Props_Sel_Str()
    {
        var vm = new TestViewModel();
        var list = new List<string>();
        vm.WhenAnyValue<TestViewModel, string, string?, string?, string?, string?, string?, string?>(
            nameof(TestViewModel.Property1),
            nameof(TestViewModel.Property2),
            nameof(TestViewModel.Property3),
            nameof(TestViewModel.Property4),
            nameof(TestViewModel.Property5),
            nameof(TestViewModel.Property6),
            (v1, v2, v3, v4, v5, v6) => "x").ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    [Test]
    public async Task WhenAnyValue_6Props_Sel_Str_Dist()
    {
        var vm = new TestViewModel();
        var list = new List<string>();
        vm.WhenAnyValue<TestViewModel, string, string?, string?, string?, string?, string?, string?>(
            nameof(TestViewModel.Property1),
            nameof(TestViewModel.Property2),
            nameof(TestViewModel.Property3),
            nameof(TestViewModel.Property4),
            nameof(TestViewModel.Property5),
            nameof(TestViewModel.Property6),
            (v1, v2, v3, v4, v5, v6) => "x",
            false).ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    [Test]
    public async Task WhenAnyValue_6Props_Tuple_Expr()
    {
        var vm = new TestViewModel();
        var list = new List<(string?, string?, string?, string?, string?, string?)>();
        vm.WhenAnyValue(
            x => x.Property1,
            x => x.Property2,
            x => x.Property3,
            x => x.Property4,
            x => x.Property5,
            x => x.Property6).ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    [Test]
    public async Task WhenAnyValue_6Props_Tuple_Expr_Dist()
    {
        var vm = new TestViewModel();
        var list = new List<(string?, string?, string?, string?, string?, string?)>();
        vm.WhenAnyValue(
            x => x.Property1,
            x => x.Property2,
            x => x.Property3,
            x => x.Property4,
            x => x.Property5,
            x => x.Property6,
            true).ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    [Test]
    public async Task WhenAnyValue_6Props_Tuple_Str()
    {
        var vm = new TestViewModel();
        var list = new List<(string?, string?, string?, string?, string?, string?)>();
        vm.WhenAnyValue<TestViewModel, string?, string?, string?, string?, string?, string?>(
            nameof(TestViewModel.Property1),
            nameof(TestViewModel.Property2),
            nameof(TestViewModel.Property3),
            nameof(TestViewModel.Property4),
            nameof(TestViewModel.Property5),
            nameof(TestViewModel.Property6)).ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    [Test]
    public async Task WhenAnyValue_6Props_Tuple_Str_Dist()
    {
        var vm = new TestViewModel();
        var list = new List<(string?, string?, string?, string?, string?, string?)>();
        vm.WhenAnyValue<TestViewModel, string?, string?, string?, string?, string?, string?>(
            nameof(TestViewModel.Property1),
            nameof(TestViewModel.Property2),
            nameof(TestViewModel.Property3),
            nameof(TestViewModel.Property4),
            nameof(TestViewModel.Property5),
            nameof(TestViewModel.Property6),
            false).ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    [Test]
    public async Task WhenAnyValue_7Props_Sel()
    {
        var vm = new TestViewModel();
        var list = new List<string>();
        vm.WhenAnyValue(
            x => x.Property1,
            x => x.Property2,
            x => x.Property3,
            x => x.Property4,
            x => x.Property5,
            x => x.Property6,
            x => x.Property7,
            (v1, v2, v3, v4, v5, v6, v7) => "x").ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    [Test]
    public async Task WhenAnyValue_7Props_Sel_Dist()
    {
        var vm = new TestViewModel();
        var list = new List<string>();
        vm.WhenAnyValue(
            x => x.Property1,
            x => x.Property2,
            x => x.Property3,
            x => x.Property4,
            x => x.Property5,
            x => x.Property6,
            x => x.Property7,
            (v1, v2, v3, v4, v5, v6, v7) => "x",
            true).ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    [Test]
    public async Task WhenAnyValue_7Props_Sel_Str()
    {
        var vm = new TestViewModel();
        var list = new List<string>();
        vm.WhenAnyValue<TestViewModel, string, string?, string?, string?, string?, string?, string?, string?>(
            nameof(TestViewModel.Property1),
            nameof(TestViewModel.Property2),
            nameof(TestViewModel.Property3),
            nameof(TestViewModel.Property4),
            nameof(TestViewModel.Property5),
            nameof(TestViewModel.Property6),
            nameof(TestViewModel.Property7),
            (v1, v2, v3, v4, v5, v6, v7) => "x").ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    [Test]
    public async Task WhenAnyValue_7Props_Sel_Str_Dist()
    {
        var vm = new TestViewModel();
        var list = new List<string>();
        vm.WhenAnyValue<TestViewModel, string, string?, string?, string?, string?, string?, string?, string?>(
            nameof(TestViewModel.Property1),
            nameof(TestViewModel.Property2),
            nameof(TestViewModel.Property3),
            nameof(TestViewModel.Property4),
            nameof(TestViewModel.Property5),
            nameof(TestViewModel.Property6),
            nameof(TestViewModel.Property7),
            (v1, v2, v3, v4, v5, v6, v7) => "x",
            false).ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    [Test]
    public async Task WhenAnyValue_7Props_Tuple_Expr()
    {
        var vm = new TestViewModel();
        var list = new List<(string?, string?, string?, string?, string?, string?, string?)>();
        vm.WhenAnyValue(
            x => x.Property1,
            x => x.Property2,
            x => x.Property3,
            x => x.Property4,
            x => x.Property5,
            x => x.Property6,
            x => x.Property7).ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    [Test]
    public async Task WhenAnyValue_7Props_Tuple_Expr_Dist()
    {
        var vm = new TestViewModel();
        var list = new List<(string?, string?, string?, string?, string?, string?, string?)>();
        vm.WhenAnyValue(
            x => x.Property1,
            x => x.Property2,
            x => x.Property3,
            x => x.Property4,
            x => x.Property5,
            x => x.Property6,
            x => x.Property7,
            true).ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    [Test]
    public async Task WhenAnyValue_7Props_Tuple_Str()
    {
        var vm = new TestViewModel();
        var list = new List<(string?, string?, string?, string?, string?, string?, string?)>();
        vm.WhenAnyValue<TestViewModel, string?, string?, string?, string?, string?, string?, string?>(
            nameof(TestViewModel.Property1),
            nameof(TestViewModel.Property2),
            nameof(TestViewModel.Property3),
            nameof(TestViewModel.Property4),
            nameof(TestViewModel.Property5),
            nameof(TestViewModel.Property6),
            nameof(TestViewModel.Property7)).ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    [Test]
    public async Task WhenAnyValue_7Props_Tuple_Str_Dist()
    {
        var vm = new TestViewModel();
        var list = new List<(string?, string?, string?, string?, string?, string?, string?)>();
        vm.WhenAnyValue<TestViewModel, string?, string?, string?, string?, string?, string?, string?>(
            nameof(TestViewModel.Property1),
            nameof(TestViewModel.Property2),
            nameof(TestViewModel.Property3),
            nameof(TestViewModel.Property4),
            nameof(TestViewModel.Property5),
            nameof(TestViewModel.Property6),
            nameof(TestViewModel.Property7),
            false).ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    [Test]
    public async Task WhenAnyValue_8Props_Sel()
    {
        var vm = new TestViewModel();
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
            (v1, v2, v3, v4, v5, v6, v7, v8) => "x").ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    [Test]
    public async Task WhenAnyValue_8Props_Sel_Dist()
    {
        var vm = new TestViewModel();
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
            (v1, v2, v3, v4, v5, v6, v7, v8) => "x",
            true).ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    [Test]
    public async Task WhenAnyValue_8Props_Sel_Str()
    {
        var vm = new TestViewModel();
        var list = new List<string>();
        vm.WhenAnyValue<TestViewModel, string, string?, string?, string?, string?, string?, string?, string?, string?>(
            nameof(TestViewModel.Property1),
            nameof(TestViewModel.Property2),
            nameof(TestViewModel.Property3),
            nameof(TestViewModel.Property4),
            nameof(TestViewModel.Property5),
            nameof(TestViewModel.Property6),
            nameof(TestViewModel.Property7),
            nameof(TestViewModel.Property8),
            (v1, v2, v3, v4, v5, v6, v7, v8) => "x").ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    [Test]
    public async Task WhenAnyValue_8Props_Sel_Str_Dist()
    {
        var vm = new TestViewModel();
        var list = new List<string>();
        vm.WhenAnyValue<TestViewModel, string, string?, string?, string?, string?, string?, string?, string?, string?>(
            nameof(TestViewModel.Property1),
            nameof(TestViewModel.Property2),
            nameof(TestViewModel.Property3),
            nameof(TestViewModel.Property4),
            nameof(TestViewModel.Property5),
            nameof(TestViewModel.Property6),
            nameof(TestViewModel.Property7),
            nameof(TestViewModel.Property8),
            (v1, v2, v3, v4, v5, v6, v7, v8) => "x",
            false).ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    [Test]
    public async Task WhenAnyValue_9Props_Sel()
    {
        var vm = new TestViewModel();
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
            (v1, v2, v3, v4, v5, v6, v7, v8, v9) => "x").ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    [Test]
    public async Task WhenAnyValue_9Props_Sel_Dist()
    {
        var vm = new TestViewModel();
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
            (v1, v2, v3, v4, v5, v6, v7, v8, v9) => "x",
            true).ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    [Test]
    public async Task WhenAnyValue_9Props_Sel_Str()
    {
        var vm = new TestViewModel();
        var list = new List<string>();
        vm.WhenAnyValue<TestViewModel, string, string?, string?, string?, string?, string?, string?, string?, string?,
            string?>(
            nameof(TestViewModel.Property1),
            nameof(TestViewModel.Property2),
            nameof(TestViewModel.Property3),
            nameof(TestViewModel.Property4),
            nameof(TestViewModel.Property5),
            nameof(TestViewModel.Property6),
            nameof(TestViewModel.Property7),
            nameof(TestViewModel.Property8),
            nameof(TestViewModel.Property9),
            (v1, v2, v3, v4, v5, v6, v7, v8, v9) => "x").ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    [Test]
    public async Task WhenAnyValue_9Props_Sel_Str_Dist()
    {
        var vm = new TestViewModel();
        var list = new List<string>();
        vm.WhenAnyValue<TestViewModel, string, string?, string?, string?, string?, string?, string?, string?, string?,
            string?>(
            nameof(TestViewModel.Property1),
            nameof(TestViewModel.Property2),
            nameof(TestViewModel.Property3),
            nameof(TestViewModel.Property4),
            nameof(TestViewModel.Property5),
            nameof(TestViewModel.Property6),
            nameof(TestViewModel.Property7),
            nameof(TestViewModel.Property8),
            nameof(TestViewModel.Property9),
            (v1, v2, v3, v4, v5, v6, v7, v8, v9) => "x",
            false).ObserveOn(ImmediateScheduler.Instance).Subscribe(list.Add);
        await Assert.That(list).Count().IsGreaterThan(0);
    }

    private class TestViewModel : ReactiveObject
    {
        private IObservable<string>? _observableProperty1;
        private IObservable<string>? _observableProperty10;
        private IObservable<string>? _observableProperty11;
        private IObservable<string>? _observableProperty12;
        private IObservable<string>? _observableProperty2;
        private IObservable<string>? _observableProperty3;
        private IObservable<string>? _observableProperty4;
        private IObservable<string>? _observableProperty5;
        private IObservable<string>? _observableProperty6;
        private IObservable<string>? _observableProperty7;
        private IObservable<string>? _observableProperty8;
        private IObservable<string>? _observableProperty9;
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

        public IObservable<string>? ObservableProperty1
        {
            get => _observableProperty1;
            set => this.RaiseAndSetIfChanged(ref _observableProperty1, value);
        }

        public IObservable<string>? ObservableProperty10
        {
            get => _observableProperty10;
            set => this.RaiseAndSetIfChanged(ref _observableProperty10, value);
        }

        public IObservable<string>? ObservableProperty11
        {
            get => _observableProperty11;
            set => this.RaiseAndSetIfChanged(ref _observableProperty11, value);
        }

        public IObservable<string>? ObservableProperty12
        {
            get => _observableProperty12;
            set => this.RaiseAndSetIfChanged(ref _observableProperty12, value);
        }

        public IObservable<string>? ObservableProperty2
        {
            get => _observableProperty2;
            set => this.RaiseAndSetIfChanged(ref _observableProperty2, value);
        }

        public IObservable<string>? ObservableProperty3
        {
            get => _observableProperty3;
            set => this.RaiseAndSetIfChanged(ref _observableProperty3, value);
        }

        public IObservable<string>? ObservableProperty4
        {
            get => _observableProperty4;
            set => this.RaiseAndSetIfChanged(ref _observableProperty4, value);
        }

        public IObservable<string>? ObservableProperty5
        {
            get => _observableProperty5;
            set => this.RaiseAndSetIfChanged(ref _observableProperty5, value);
        }

        public IObservable<string>? ObservableProperty6
        {
            get => _observableProperty6;
            set => this.RaiseAndSetIfChanged(ref _observableProperty6, value);
        }

        public IObservable<string>? ObservableProperty7
        {
            get => _observableProperty7;
            set => this.RaiseAndSetIfChanged(ref _observableProperty7, value);
        }

        public IObservable<string>? ObservableProperty8
        {
            get => _observableProperty8;
            set => this.RaiseAndSetIfChanged(ref _observableProperty8, value);
        }

        public IObservable<string>? ObservableProperty9
        {
            get => _observableProperty9;
            set => this.RaiseAndSetIfChanged(ref _observableProperty9, value);
        }

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
