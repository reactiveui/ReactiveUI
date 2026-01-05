// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Forms;

using ReactiveUI.Winforms;

using TUnit.Core.Executors;

namespace ReactiveUI.Tests.Winforms;

/// <summary>
/// Tests for TableContentSetMethodBindingConverter.
/// </summary>
public class TableContentSetMethodBindingConverterTests
{
    [Test]
    [TestExecutor<STAThreadExecutor>]
    public async Task GetAffinityForObjects_Returns_Zero_When_ToType_Is_Not_TableLayoutControlCollection()
    {
        var converter = new TableContentSetMethodBindingConverter();
        var affinity = converter.GetAffinityForObjects(typeof(Button), typeof(string));

        await Assert.That(affinity).IsEqualTo(0);
    }

    [Test]
    [TestExecutor<STAThreadExecutor>]
    public async Task GetAffinityForObjects_Returns_Fifteen_When_FromType_Is_IEnumerable_Of_Control()
    {
        var converter = new TableContentSetMethodBindingConverter();
        var affinity = converter.GetAffinityForObjects(typeof(List<Button>), typeof(TableLayoutControlCollection));

        await Assert.That(affinity).IsEqualTo(15);
    }

    [Test]
    [TestExecutor<STAThreadExecutor>]
    public async Task GetAffinityForObjects_Returns_Zero_When_FromType_Is_Not_IEnumerable_Of_Control()
    {
        var converter = new TableContentSetMethodBindingConverter();
        var affinity = converter.GetAffinityForObjects(typeof(string), typeof(TableLayoutControlCollection));

        await Assert.That(affinity).IsEqualTo(0);
    }

    [Test]
    [TestExecutor<STAThreadExecutor>]
    public async Task GetAffinityForObjects_Returns_Zero_When_FromType_Is_Null()
    {
        var converter = new TableContentSetMethodBindingConverter();
        var affinity = converter.GetAffinityForObjects(null, typeof(TableLayoutControlCollection));

        await Assert.That(affinity).IsEqualTo(0);
    }

    [Test]
    [TestExecutor<STAThreadExecutor>]
    public void PerformSet_Throws_When_ToTarget_Is_Null()
    {
        var converter = new TableContentSetMethodBindingConverter();
        Assert.Throws<ArgumentNullException>(() =>
            converter.PerformSet(null, new List<Button>(), null));
    }

    [Test]
    [TestExecutor<STAThreadExecutor>]
    public void PerformSet_Throws_When_ToTarget_Is_Not_TableLayoutControlCollection()
    {
        var converter = new TableContentSetMethodBindingConverter();
        var panel = new Panel();
        Assert.Throws<ArgumentException>(() =>
            converter.PerformSet(panel.Controls, new List<Button>(), null));
    }

    [Test]
    [TestExecutor<STAThreadExecutor>]
    public void PerformSet_Throws_When_NewValue_Is_Not_IEnumerable_Control()
    {
        var converter = new TableContentSetMethodBindingConverter();
        var tableLayoutPanel = new TableLayoutPanel();
        Assert.Throws<ArgumentException>(() =>
            converter.PerformSet(tableLayoutPanel.Controls, "not a collection", null));
    }

    [Test]
    [TestExecutor<STAThreadExecutor>]
    public async Task PerformSet_Adds_Controls_To_Collection()
    {
        var converter = new TableContentSetMethodBindingConverter();
        var tableLayoutPanel = new TableLayoutPanel();
        var button1 = new Button { Name = "Button1" };
        var button2 = new Button { Name = "Button2" };
        var controls = new List<Button> { button1, button2 };

        var result = converter.PerformSet(tableLayoutPanel.Controls, controls, null);

        await Assert.That(tableLayoutPanel.Controls.Count).IsEqualTo(2);
        await Assert.That(tableLayoutPanel.Controls[0]).IsSameReferenceAs(button1);
        await Assert.That(tableLayoutPanel.Controls[1]).IsSameReferenceAs(button2);
    }
}
