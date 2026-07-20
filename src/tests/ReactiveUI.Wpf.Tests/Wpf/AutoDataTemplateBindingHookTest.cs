// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Linq.Expressions;
using System.Windows;
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

#if REACTIVE_SHIM
    /// <summary>
    /// Under REACTIVE_SHIM the shared source is recompiled into the
    /// ReactiveUI.Reactive namespace, so the inline XAML template's
    /// clr-namespace must also be ReactiveUI.Reactive. If it is left hardcoded
    /// to ReactiveUI, XamlReader.Parse throws a XamlObjectReaderException
    /// because ViewModelViewHost cannot be resolved in the ReactiveUI.Wpf.Reactive
    /// assembly. This regression test forces the lazy template to materialize and
    /// asserts it loads without throwing. See issue #4398.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task DefaultItemTemplate_LoadsUnderReactiveShim()
    {
        // Materializing the lazy value must not throw. Before the fix this threw
        // System.Xaml.XamlObjectReaderException: Cannot create unknown type
        // '{clr-namespace:ReactiveUI;assembly=ReactiveUI.Wpf.Reactive}ViewModelViewHost'.
        DataTemplate? template = null;
        await Assert.That(() => template = AutoDataTemplateBindingHook.DefaultItemTemplate.Value).ThrowsNothing();
        await Assert.That(template).IsNotNull();
    }
#endif

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

    /// <summary>An empty view-property list leaves the hook a no-op that returns true.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ExecuteHook_EmptyViewProperties_ReturnsTrue()
    {
        var hook = new AutoDataTemplateBindingHook();

        var result = hook.ExecuteHook(null, new object(), () => [], () => [], BindingDirection.OneWay);

        await Assert.That(result).IsTrue();
    }

    /// <summary>An ItemsControl with a non-empty DisplayMemberPath is left untouched and returns true.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ExecuteHook_ItemsControlWithDisplayMemberPath_ReturnsTrueWithoutTemplate()
    {
        var hook = new AutoDataTemplateBindingHook();
        var itemsControl = new ListBox { DisplayMemberPath = "Name" };
        Expression<Func<ItemsControl, object?>> expr = ic => ic.ItemsSource;
        IObservedChange<object, object>[] props = [new ObservedChange<object, object>(itemsControl, Reflection.Rewrite(expr.Body), null!)];

        var result = hook.ExecuteHook(null, itemsControl, () => [], () => props, BindingDirection.OneWay);

        using (Assert.Multiple())
        {
            await Assert.That(result).IsTrue();
            await Assert.That(itemsControl.ItemTemplate).IsNull();
        }
    }

    /// <summary>A bound property other than ItemsSource leaves the ItemsControl untouched and returns true.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ExecuteHook_NonItemsSourceProperty_ReturnsTrueWithoutTemplate()
    {
        var hook = new AutoDataTemplateBindingHook();
        var itemsControl = new ListBox();
        Expression<Func<ItemsControl, object?>> expr = ic => ic.Tag;
        IObservedChange<object, object>[] props = [new ObservedChange<object, object>(itemsControl, Reflection.Rewrite(expr.Body), null!)];

        var result = hook.ExecuteHook(null, itemsControl, () => [], () => props, BindingDirection.OneWay);

        using (Assert.Multiple())
        {
            await Assert.That(result).IsTrue();
            await Assert.That(itemsControl.ItemTemplate).IsNull();
        }
    }

    /// <summary>An ItemsControl that already has an ItemTemplate keeps it and returns true.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ExecuteHook_ItemsControlWithExistingItemTemplate_ReturnsTrueLeavingTemplate()
    {
        var hook = new AutoDataTemplateBindingHook();
        var existingTemplate = new DataTemplate();
        var itemsControl = new ListBox { ItemTemplate = existingTemplate };
        Expression<Func<ItemsControl, object?>> expr = ic => ic.ItemsSource;
        IObservedChange<object, object>[] props = [new ObservedChange<object, object>(itemsControl, Reflection.Rewrite(expr.Body), null!)];

        var result = hook.ExecuteHook(null, itemsControl, () => [], () => props, BindingDirection.OneWay);

        using (Assert.Multiple())
        {
            await Assert.That(result).IsTrue();
            await Assert.That(itemsControl.ItemTemplate).IsSameReferenceAs(existingTemplate);
        }
    }

    /// <summary>An ItemsControl that already has an ItemTemplateSelector is left untouched and returns true.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ExecuteHook_ItemsControlWithItemTemplateSelector_ReturnsTrueWithoutTemplate()
    {
        var hook = new AutoDataTemplateBindingHook();
        var itemsControl = new ListBox { ItemTemplateSelector = new() };
        Expression<Func<ItemsControl, object?>> expr = ic => ic.ItemsSource;
        IObservedChange<object, object>[] props = [new ObservedChange<object, object>(itemsControl, Reflection.Rewrite(expr.Body), null!)];

        var result = hook.ExecuteHook(null, itemsControl, () => [], () => props, BindingDirection.OneWay);

        using (Assert.Multiple())
        {
            await Assert.That(result).IsTrue();
            await Assert.That(itemsControl.ItemTemplate).IsNull();
        }
    }
}
