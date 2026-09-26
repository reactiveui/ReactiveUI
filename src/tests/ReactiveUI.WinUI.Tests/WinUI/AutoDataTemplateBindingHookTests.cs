// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Linq.Expressions;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using ReactiveUI.Tests.WinUI.Mocks;
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
    /// <summary>The key the default template's converter is stored under in the application's resources.</summary>
    private static readonly string ConverterKey = $"{typeof(ViewModelViewHost).FullName}Converter";

    /// <summary>
    /// Verifies the default template's converter sits in the application's resources, turns a view model into its
    /// host, and refuses to convert a host back, since the template binds one way only.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task DefaultItemTemplate_ConverterInTheApplicationResources_ConvertsOneWayOnly()
    {
        _ = AutoDataTemplateBindingHook.DefaultItemTemplate.Value;
        var converter = Application.Current.Resources[ConverterKey] as IValueConverter;
        var viewModel = new PlainTestViewModel();

        var host = converter?.Convert(viewModel, typeof(object), null!, string.Empty) as ViewModelViewHost;

        using (Assert.Multiple())
        {
            await Assert.That(converter).IsNotNull();
            await Assert.That(host?.ViewModel).IsSameReferenceAs(viewModel);
            await Assert.That(() => converter!.ConvertBack(host!, typeof(object), null!, string.Empty)).Throws<NotSupportedException>();
        }
    }

    /// <summary>Verifies the hook puts the converter back when the application's resources no longer hold it.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ExecuteHook_WhenTheApplicationLostTheConverter_RegistersItAgain()
    {
        _ = AutoDataTemplateBindingHook.DefaultItemTemplate.Value;
        _ = Application.Current.Resources.Remove(ConverterKey);
        var hook = new AutoDataTemplateBindingHook();
        var itemsControl = new ListBox();
        Expression<Func<ItemsControl, object?>> expression = static x => x.ItemsSource;

        _ = hook.ExecuteHook(null, itemsControl, static () => [], () => ViewProperties(itemsControl, expression.Body), BindingDirection.OneWay);

        await Assert.That(Application.Current.Resources.ContainsKey(ConverterKey)).IsTrue();
    }

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
    /// Verifies the default template really produces a host that displays the bound item, which is the whole point
    /// of supplying it. The test application has no <c>IXamlMetadataProvider</c>, like an app with no .xaml files,
    /// so this fails if the template ever names a ReactiveUI type the XAML parser would have to look up.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task DefaultItemTemplate_HostsTheItemInAViewModelViewHost()
    {
        var viewModel = new PlainTestViewModel();
        var content = (ContentControl)AutoDataTemplateBindingHook.DefaultItemTemplate.Value.LoadContent();

        content.DataContext = viewModel;

        var host = content.Content as ViewModelViewHost;
        using (Assert.Multiple())
        {
            await Assert.That(host).IsTypeOf<ViewModelViewHost>();
            await Assert.That(host!.ViewModel).IsSameReferenceAs(viewModel);
            await Assert.That(host.IsTabStop).IsFalse();
        }
    }

    /// <summary>Verifies the host the default template creates shows the view the generated view lookup finds for the item.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<WinUIViewRegistrationExecutor>]
    public async Task DefaultItemTemplate_ShowsTheGeneratedViewForTheItem()
    {
        var viewModel = new PlainTestViewModel();
        var content = (ContentControl)AutoDataTemplateBindingHook.DefaultItemTemplate.Value.LoadContent();

        content.DataContext = viewModel;

        var view = ((ViewModelViewHost)content.Content).Content as PlainTestView;
        using (Assert.Multiple())
        {
            await Assert.That(view).IsNotNull();
            await Assert.That(view!.ViewModel).IsSameReferenceAs(viewModel);
        }
    }

    /// <summary>Builds the observed-change chain a property binding would hand the hook.</summary>
    /// <param name="sender">The bound target.</param>
    /// <param name="expression">The bound property expression.</param>
    /// <returns>The observed-change chain ending at the bound property.</returns>
    private static IObservedChange<object, object>[] ViewProperties(object sender, Expression expression) =>
        [new ObservedChange<object, object>(sender, Reflection.Rewrite(expression), null!)];
}
