// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using TUnit.Core.Executors;

namespace ReactiveUI.WinForms.Tests.Winforms;

/// <summary>Tests for TableContentSetMethodBindingConverter.</summary>
[NotInParallel]
[TestExecutor<WinFormsTestExecutor>]
public class TableContentSetMethodBindingConverterTests
{
    /// <summary>The affinity returned for an enumerable bound to a table layout control collection.</summary>
    private const int EnumerableControlAffinity = 10;

    /// <summary>The expected number of controls after binding.</summary>
    private const int ExpectedControlCount = 2;

    /// <summary>Tests that GetAffinityForObjects returns zero when the target type is not a table layout control collection.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task GetAffinityForObjects_Returns_Zero_When_ToType_Is_Not_TableLayoutControlCollection()
    {
        var converter = new TableContentSetMethodBindingConverter();
        var affinity = converter.GetAffinityForObjects(typeof(Button), typeof(string));

        await Assert.That(affinity).IsEqualTo(0);
    }

    /// <summary>Tests that GetAffinityForObjects returns a positive affinity when the source type is an enumerable of controls.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task GetAffinityForObjects_Returns_Fifteen_When_FromType_Is_IEnumerable_Of_Control()
    {
        var converter = new TableContentSetMethodBindingConverter();
        var affinity = converter.GetAffinityForObjects(typeof(List<Button>), typeof(TableLayoutControlCollection));

        await Assert.That(affinity).IsEqualTo(EnumerableControlAffinity);
    }

    /// <summary>Tests that GetAffinityForObjects returns zero when the source type is not an enumerable of controls.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task GetAffinityForObjects_Returns_Zero_When_FromType_Is_Not_IEnumerable_Of_Control()
    {
        var converter = new TableContentSetMethodBindingConverter();
        var affinity = converter.GetAffinityForObjects(typeof(string), typeof(TableLayoutControlCollection));

        await Assert.That(affinity).IsEqualTo(0);
    }

    /// <summary>Tests that GetAffinityForObjects returns zero when the source type is null.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task GetAffinityForObjects_Returns_Zero_When_FromType_Is_Null()
    {
        var converter = new TableContentSetMethodBindingConverter();
        var affinity = converter.GetAffinityForObjects(null, typeof(TableLayoutControlCollection));

        await Assert.That(affinity).IsEqualTo(0);
    }

    /// <summary>Tests that PerformSet throws when the target is null.</summary>
    [Test]
    public void PerformSet_Throws_When_ToTarget_Is_Null()
    {
        var converter = new TableContentSetMethodBindingConverter();
        _ = Assert.Throws<ArgumentNullException>(() =>
            converter.PerformSet(null, new List<Button>(), null));
    }

    /// <summary>Tests that PerformSet throws when the target is not a table layout control collection.</summary>
    [Test]
    public void PerformSet_Throws_When_ToTarget_Is_Not_TableLayoutControlCollection()
    {
        var converter = new TableContentSetMethodBindingConverter();
        var panel = new Panel();
        _ = Assert.Throws<ArgumentException>(() =>
            converter.PerformSet(panel.Controls, new List<Button>(), null));
    }

    /// <summary>Tests that PerformSet throws when the new value is not an enumerable of controls.</summary>
    [Test]
    public void PerformSet_Throws_When_NewValue_Is_Not_IEnumerable_Control()
    {
        var converter = new TableContentSetMethodBindingConverter();
        var tableLayoutPanel = new TableLayoutPanel();
        _ = Assert.Throws<ArgumentException>(() =>
            converter.PerformSet(tableLayoutPanel.Controls, "not a collection", null));
    }

    /// <summary>Tests that PerformSet adds the controls to the target collection.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task PerformSet_Adds_Controls_To_Collection()
    {
        var converter = new TableContentSetMethodBindingConverter();
        var tableLayoutPanel = new TableLayoutPanel();
        var button1 = new Button { Name = "Button1" };
        var button2 = new Button { Name = "Button2" };
        var controls = new List<Button> { button1, button2 };

        _ = converter.PerformSet(tableLayoutPanel.Controls, controls, null);

        await Assert.That(tableLayoutPanel.Controls.Count).IsEqualTo(ExpectedControlCount);
        await Assert.That(tableLayoutPanel.Controls[0]).IsSameReferenceAs(button1);
        await Assert.That(tableLayoutPanel.Controls[1]).IsSameReferenceAs(button2);
    }
}
