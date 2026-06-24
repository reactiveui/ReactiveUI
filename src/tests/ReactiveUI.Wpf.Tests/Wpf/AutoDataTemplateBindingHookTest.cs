// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Linq.Expressions;
using System.Windows.Controls;
using TUnit.Core.Executors;

namespace ReactiveUI.Tests.Wpf;

/// <summary>Tests for <see cref="AutoDataTemplateBindingHook"/>.</summary>
[NotInParallel]
[TestExecutor<WpfTestExecutor>]
public class AutoDataTemplateBindingHookTest
{
    /// <summary>The default item template parses from XAML and is not null.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task DefaultItemTemplate_IsNotNull() =>
        await Assert.That(AutoDataTemplateBindingHook.DefaultItemTemplate.Value).IsNotNull();

    /// <summary>A null view-property accessor throws.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ExecuteHook_NullViewProperties_Throws()
    {
        var hook = new AutoDataTemplateBindingHook();

        await Assert.That(() => hook.ExecuteHook(null, new object(), () => [], null!, BindingDirection.OneWay))
            .Throws<ArgumentNullException>();
    }

    /// <summary>A non-ItemsControl target leaves the hook a no-op that returns true.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ExecuteHook_NonItemsControl_ReturnsTrue()
    {
        var hook = new AutoDataTemplateBindingHook();
        var target = new TextBox();
        Expression<Func<TextBox, object?>> expr = tb => tb.Text;
        IObservedChange<object, object>[] props = [new ObservedChange<object, object>(target, Reflection.Rewrite(expr.Body), null!)];

        var result = hook.ExecuteHook(null, target, () => [], () => props, BindingDirection.OneWay);

        await Assert.That(result).IsTrue();
    }

    /// <summary>An ItemsControl bound to ItemsSource with no template gets the default item template.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ExecuteHook_ItemsControlItemsSource_SetsDefaultItemTemplate()
    {
        var hook = new AutoDataTemplateBindingHook();
        var itemsControl = new ListBox();
        Expression<Func<ItemsControl, object?>> expr = ic => ic.ItemsSource;
        IObservedChange<object, object>[] props = [new ObservedChange<object, object>(itemsControl, Reflection.Rewrite(expr.Body), null!)];

        var result = hook.ExecuteHook(null, itemsControl, () => [], () => props, BindingDirection.OneWay);

        using (Assert.Multiple())
        {
            await Assert.That(result).IsTrue();
            await Assert.That(itemsControl.ItemTemplate).IsNotNull();
        }
    }
}
