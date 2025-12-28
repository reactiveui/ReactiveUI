// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Forms;

using ReactiveUI.Winforms;

using TUnit.Core.Executors;

namespace ReactiveUI.Tests.Winforms;

/// <summary>
/// Tests for PanelSetMethodBindingConverter.
/// </summary>
public class PanelSetMethodBindingConverterTests
{
    [Test]
    [TestExecutor<STAThreadExecutor>]
    public async Task GetAffinityForObjects_Returns_Zero_When_ToType_Is_Not_ControlCollection()
    {
        var converter = new PanelSetMethodBindingConverter();
        var affinity = converter.GetAffinityForObjects(typeof(Button), typeof(string));

        await Assert.That(affinity).IsEqualTo(0);
    }

    [Test]
    [TestExecutor<STAThreadExecutor>]
    public async Task GetAffinityForObjects_Returns_Ten_When_FromType_Is_IEnumerable_Of_Control()
    {
        var converter = new PanelSetMethodBindingConverter();
        var affinity = converter.GetAffinityForObjects(typeof(List<Button>), typeof(Control.ControlCollection));

        await Assert.That(affinity).IsEqualTo(10);
    }

    [Test]
    [TestExecutor<STAThreadExecutor>]
    public async Task GetAffinityForObjects_Returns_Zero_When_FromType_Is_Not_IEnumerable_Of_Control()
    {
        var converter = new PanelSetMethodBindingConverter();
        var affinity = converter.GetAffinityForObjects(typeof(string), typeof(Control.ControlCollection));

        await Assert.That(affinity).IsEqualTo(0);
    }

    [Test]
    [TestExecutor<STAThreadExecutor>]
    public async Task GetAffinityForObjects_Returns_Zero_When_FromType_Is_Null()
    {
        var converter = new PanelSetMethodBindingConverter();
        var affinity = converter.GetAffinityForObjects(null, typeof(Control.ControlCollection));

        await Assert.That(affinity).IsEqualTo(0);
    }

    [Test]
    [TestExecutor<STAThreadExecutor>]
    public void PerformSet_Throws_When_ToTarget_Is_Null()
    {
        var converter = new PanelSetMethodBindingConverter();
        Assert.Throws<ArgumentNullException>(() =>
            converter.PerformSet(null, new List<Button>(), null));
    }

    [Test]
    [TestExecutor<STAThreadExecutor>]
    public void PerformSet_Throws_When_NewValue_Is_Not_IEnumerable_Control()
    {
        var converter = new PanelSetMethodBindingConverter();
        var panel = new Panel();
        Assert.Throws<ArgumentException>(() =>
            converter.PerformSet(panel.Controls, "not a collection", null));
    }

    [Test]
    [TestExecutor<STAThreadExecutor>]
    public async Task PerformSet_Adds_Controls_To_Collection()
    {
        var converter = new PanelSetMethodBindingConverter();
        var panel = new Panel();
        var button1 = new Button { Name = "Button1" };
        var button2 = new Button { Name = "Button2" };
        var controls = new List<Button> { button1, button2 };

        var result = converter.PerformSet(panel.Controls, controls, null);

        await Assert.That(panel.Controls.Count).IsEqualTo(2);
        await Assert.That(panel.Controls[0]).IsSameReferenceAs(button1);
        await Assert.That(panel.Controls[1]).IsSameReferenceAs(button2);
    }
}
