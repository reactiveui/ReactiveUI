// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Linq.Expressions;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using TUnit.Core.Executors;

namespace ReactiveUI.Tests.WinUI;

/// <summary>Tests for <see cref="AutoDataTemplateBindingHook"/>.</summary>
/// <remarks>
/// The hook only ever supplies a template for an items control that is being bound through its
/// <see cref="ItemsControl.ItemsSource"/> and has expressed no opinion of its own about item presentation. Every
/// other shape has to be left exactly as the author wrote it.
/// </remarks>
[NotInParallel]
[TestExecutor<WinUITestExecutor>]
public class AutoDataTemplateBindingHookTests
{
    /// <summary>Verifies a missing view-property accessor is rejected.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ExecuteHook_WithNoViewPropertyAccessor_Throws()
    {
        var hook = new AutoDataTemplateBindingHook();

        await Assert.That(() => hook.ExecuteHook(null, new(), static () => [], null!, BindingDirection.OneWay))
            .Throws<ArgumentNullException>();
    }

    /// <summary>Verifies a binding with no view properties leaves the binding to proceed untouched.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ExecuteHook_WithNoViewProperties_LetsTheBindingProceed()
    {
        var hook = new AutoDataTemplateBindingHook();

        var proceed = hook.ExecuteHook(null, new(), static () => [], static () => [], BindingDirection.OneWay);

        await Assert.That(proceed).IsTrue();
    }

    /// <summary>Verifies a target that presents no item collection is left alone.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ExecuteHook_ForANonItemsControl_LetsTheBindingProceed()
    {
        var hook = new AutoDataTemplateBindingHook();
        var target = new TextBox();
        Expression<Func<TextBox, object?>> expression = static x => x.Text;

        var proceed = hook.ExecuteHook(null, target, static () => [], () => ViewProperties(target, expression.Body), BindingDirection.OneWay);

        await Assert.That(proceed).IsTrue();
    }

    /// <summary>Verifies an items control that already says how to display an item keeps its own presentation.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ExecuteHook_ForAnItemsControlWithADisplayMemberPath_LeavesTheTemplateUnset()
    {
        var hook = new AutoDataTemplateBindingHook();
        var itemsControl = new ListBox { DisplayMemberPath = "Name" };
        Expression<Func<ItemsControl, object?>> expression = static x => x.ItemsSource;

        var proceed = hook.ExecuteHook(null, itemsControl, static () => [], () => ViewProperties(itemsControl, expression.Body), BindingDirection.OneWay);

        using (Assert.Multiple())
        {
            await Assert.That(proceed).IsTrue();
            await Assert.That(itemsControl.ItemTemplate).IsNull();
        }
    }

    /// <summary>Verifies binding a property other than the item collection leaves the template unset.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ExecuteHook_ForABindingOtherThanItemsSource_LeavesTheTemplateUnset()
    {
        var hook = new AutoDataTemplateBindingHook();
        var itemsControl = new ListBox();
        Expression<Func<ItemsControl, object?>> expression = static x => x.Tag;

        var proceed = hook.ExecuteHook(null, itemsControl, static () => [], () => ViewProperties(itemsControl, expression.Body), BindingDirection.OneWay);

        using (Assert.Multiple())
        {
            await Assert.That(proceed).IsTrue();
            await Assert.That(itemsControl.ItemTemplate).IsNull();
        }
    }

    /// <summary>Verifies an author-supplied item template is never replaced.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ExecuteHook_ForAnItemsControlWithATemplate_KeepsThatTemplate()
    {
        var hook = new AutoDataTemplateBindingHook();
        var template = new DataTemplate();
        var itemsControl = new ListBox { ItemTemplate = template };
        Expression<Func<ItemsControl, object?>> expression = static x => x.ItemsSource;

        var proceed = hook.ExecuteHook(null, itemsControl, static () => [], () => ViewProperties(itemsControl, expression.Body), BindingDirection.OneWay);

        using (Assert.Multiple())
        {
            await Assert.That(proceed).IsTrue();
            await Assert.That(itemsControl.ItemTemplate).IsSameReferenceAs(template);
        }
    }

    /// <summary>Verifies an author-supplied template selector is never overridden by a fixed template.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ExecuteHook_ForAnItemsControlWithATemplateSelector_LeavesTheTemplateUnset()
    {
        var hook = new AutoDataTemplateBindingHook();
        var itemsControl = new ListBox { ItemTemplateSelector = new() };
        Expression<Func<ItemsControl, object?>> expression = static x => x.ItemsSource;

        var proceed = hook.ExecuteHook(null, itemsControl, static () => [], () => ViewProperties(itemsControl, expression.Body), BindingDirection.OneWay);

        using (Assert.Multiple())
        {
            await Assert.That(proceed).IsTrue();
            await Assert.That(itemsControl.ItemTemplate).IsNull();
        }
    }

    /// <summary>Verifies a plain item-collection binding is given the view-model-driven item template.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ExecuteHook_ForAPlainItemsSourceBinding_SuppliesTheDefaultItemTemplate()
    {
        var hook = new AutoDataTemplateBindingHook();
        var itemsControl = new ListBox();
        Expression<Func<ItemsControl, object?>> expression = static x => x.ItemsSource;

        var proceed = hook.ExecuteHook(null, itemsControl, static () => [], () => ViewProperties(itemsControl, expression.Body), BindingDirection.OneWay);

        using (Assert.Multiple())
        {
            await Assert.That(proceed).IsTrue();
            await Assert.That(itemsControl.ItemTemplate).IsSameReferenceAs(AutoDataTemplateBindingHook.DefaultItemTemplate.Value);
        }
    }

    /// <summary>
    /// Verifies the default template really produces a host that can display a view for the bound item, which is
    /// the whole point of supplying it. Parsing the template resolves the hook's own namespace, so this fails if
    /// the XAML ever names a namespace this assembly is not compiled into, or one WinUI's parser cannot read.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task DefaultItemTemplate_MaterializesAViewModelViewHost()
    {
        var content = AutoDataTemplateBindingHook.DefaultItemTemplate.Value.LoadContent();

        _ = await Assert.That(content).IsTypeOf<ViewModelViewHost>();
    }

    /// <summary>Builds the observed-change chain a property binding would hand the hook.</summary>
    /// <param name="sender">The bound target.</param>
    /// <param name="expression">The bound property expression.</param>
    /// <returns>The observed-change chain ending at the bound property.</returns>
    private static IObservedChange<object, object>[] ViewProperties(object sender, Expression expression) =>
        [new ObservedChange<object, object>(sender, Reflection.Rewrite(expression), null!)];
}
