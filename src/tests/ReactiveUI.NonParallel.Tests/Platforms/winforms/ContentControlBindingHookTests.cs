// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Linq.Expressions;
using System.Windows.Forms;

using ReactiveUI.Winforms;

using TUnit.Core.Executors;

namespace ReactiveUI.Tests.Winforms;

/// <summary>
/// Tests for ContentControlBindingHook.
/// </summary>
public class ContentControlBindingHookTests
{
    [Test]
    [TestExecutor<STAThreadExecutor>]
    public void ExecuteHook_Throws_When_GetCurrentViewProperties_Is_Null()
    {
        var hook = new ContentControlBindingHook();
        Assert.Throws<ArgumentNullException>(() =>
            hook.ExecuteHook(null, new object(), () => [], null!, BindingDirection.OneWay));
    }

    [Test]
    [TestExecutor<STAThreadExecutor>]
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
            new object(),
            () => [],
            () => viewProperties,
            BindingDirection.OneWay);

        await Assert.That(result).IsTrue();
    }

    [Test]
    [TestExecutor<STAThreadExecutor>]
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
            new object(),
            () => [],
            () => viewProperties,
            BindingDirection.OneWay);

        await Assert.That(result).IsTrue();
    }

    [Test]
    [TestExecutor<STAThreadExecutor>]
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
            new object(),
            () => [],
            () => viewProperties,
            BindingDirection.OneWay);

        await Assert.That(result).IsTrue();
    }

    [Test]
    [TestExecutor<STAThreadExecutor>]
    public async Task ExecuteHook_Returns_True_When_ViewProperties_Is_Empty()
    {
        var hook = new ContentControlBindingHook();

        var result = hook.ExecuteHook(
            null,
            new object(),
            () => [],
            () => [],
            BindingDirection.OneWay);

        await Assert.That(result).IsTrue();
    }

    [Test]
    [TestExecutor<STAThreadExecutor>]
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
            new object(),
            () => [],
            () => viewProperties,
            BindingDirection.OneWay);

        await Assert.That(result).IsTrue();
    }
}
