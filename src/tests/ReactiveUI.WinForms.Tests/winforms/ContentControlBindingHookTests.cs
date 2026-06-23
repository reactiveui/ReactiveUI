// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Linq.Expressions;
using TUnit.Core.Executors;

namespace ReactiveUI.WinForms.Tests.Winforms;

/// <summary>Tests for ContentControlBindingHook.</summary>
[TestExecutor<WinFormsTestExecutor>]
[NotInParallel]
public class ContentControlBindingHookTests
{
    /// <summary>Tests that ExecuteHook throws when the current view properties accessor is null.</summary>
    [Test]
    public void ExecuteHook_Throws_When_GetCurrentViewProperties_Is_Null()
    {
        var hook = new ContentControlBindingHook();
        _ = Assert.Throws<ArgumentNullException>(() =>
            hook.ExecuteHook(null, new(), () => [], null!, BindingDirection.OneWay));
    }

    /// <summary>Tests that ExecuteHook returns true when the sender is not a panel.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ExecuteHook_Returns_True_When_Sender_Is_Not_Panel()
    {
        var hook = new ContentControlBindingHook();
        var button = new Button();
        Expression<Func<Button, Control.ControlCollection>> expr = x => x.Controls;
        var viewProperties = new[]
        {
            new ObservedChange<object, object>(button, expr.Body, button.Controls)
        };

        var result = hook.ExecuteHook(
            null,
            new(),
            () => [],
            () => viewProperties,
            BindingDirection.OneWay);

        await Assert.That(result).IsTrue();
    }

    /// <summary>Tests that ExecuteHook returns true when the sender is a panel.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ExecuteHook_Returns_True_When_Sender_Is_Panel()
    {
        var hook = new ContentControlBindingHook();
        var panel = new Panel();
        Expression<Func<Panel, Control.ControlCollection>> expr = x => x.Controls;
        var viewProperties = new[]
        {
            new ObservedChange<object, object>(panel, expr.Body, panel.Controls)
        };

        var result = hook.ExecuteHook(
            null,
            new(),
            () => [],
            () => viewProperties,
            BindingDirection.OneWay);

        await Assert.That(result).IsTrue();
    }

    /// <summary>Tests that ExecuteHook returns true when the bound property is not Controls.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ExecuteHook_Returns_True_When_Property_Is_Not_Controls()
    {
        var hook = new ContentControlBindingHook();
        var panel = new Panel();
        Expression<Func<Panel, int>> expr = x => x.Width;
        var viewProperties = new[]
        {
            new ObservedChange<object, object>(panel, expr.Body, panel.Width)
        };

        var result = hook.ExecuteHook(
            null,
            new(),
            () => [],
            () => viewProperties,
            BindingDirection.OneWay);

        await Assert.That(result).IsTrue();
    }

    /// <summary>Tests that ExecuteHook returns true when the view properties are empty.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ExecuteHook_Returns_True_When_ViewProperties_Is_Empty()
    {
        var hook = new ContentControlBindingHook();

        var result = hook.ExecuteHook(
            null,
            new(),
            () => [],
            () => [],
            BindingDirection.OneWay);

        await Assert.That(result).IsTrue();
    }

    /// <summary>Tests that ExecuteHook returns true when the sender is a panel and the property is Controls.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S4144:Methods should not have identical implementations", Justification = "Intentional duplicate test scenario.")]
    public async Task ExecuteHook_Returns_True_When_Sender_Is_Panel_And_Property_Is_Controls()
    {
        var hook = new ContentControlBindingHook();
        var panel = new Panel();
        Expression<Func<Panel, Control.ControlCollection>> expr = x => x.Controls;
        var viewProperties = new[]
        {
            new ObservedChange<object, object>(panel, expr.Body, panel.Controls)
        };

        var result = hook.ExecuteHook(
            null,
            new(),
            () => [],
            () => viewProperties,
            BindingDirection.OneWay);

        await Assert.That(result).IsTrue();
    }
}
