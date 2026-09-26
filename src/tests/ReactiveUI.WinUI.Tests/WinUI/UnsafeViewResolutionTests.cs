// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Linq.Expressions;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using ReactiveUI.Tests.WinUI.Mocks;
using Splat;
using TUnit.Core.Executors;

namespace ReactiveUI.Tests.WinUI;

/// <summary>
/// Tests the split between the default view hosts, which resolve through the generated view lookup, and their Unsafe
/// twins, which also resolve by the view model's run-time type.
/// </summary>
[NotInParallel]
[TestExecutor<WinUITestExecutor>]
public class UnsafeViewResolutionTests
{
    /// <summary>Verifies the default host never asks the locator by run-time type.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ViewModelViewHost_ResolvesThroughTheGeneratedLookup()
    {
        var view = new PlainTestView();
        var locator = new StubViewLocator { ContractlessView = view };

        var host = new ViewModelViewHost { ViewLocator = locator, ViewModel = new PlainTestViewModel() };

        using (Assert.Multiple())
        {
            await Assert.That(host.Content).IsSameReferenceAs(view);
            await Assert.That(locator.RuntimeTypeLookups).IsEqualTo(0);
        }
    }

    /// <summary>Verifies the Unsafe host asks the locator by run-time type.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ViewModelViewHostUnsafe_ResolvesByRunTimeType()
    {
        var view = new PlainTestView();
        var locator = new StubViewLocator { ContractlessView = view };

        var host = new ViewModelViewHostUnsafe { ViewLocator = locator, ViewModel = new PlainTestViewModel() };

        using (Assert.Multiple())
        {
            await Assert.That(host.Content).IsSameReferenceAs(view);
            await Assert.That(locator.RuntimeTypeLookups).IsGreaterThan(0);
        }
    }

    /// <summary>Verifies the default routed host never asks the locator by run-time type.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task RoutedViewHost_ResolvesThroughTheGeneratedLookup()
    {
        var view = new RoutedTestView();
        var router = new RoutingState(Sequencer.Immediate);
        var locator = new StubViewLocator { ContractlessView = view };
        var host = new RoutedViewHost { ViewLocator = locator, Router = router };

        using var navigation = router.Navigate.Execute(new RoutedTestViewModel()).Subscribe();

        using (Assert.Multiple())
        {
            await Assert.That(host.Content).IsSameReferenceAs(view);
            await Assert.That(locator.RuntimeTypeLookups).IsEqualTo(0);
        }
    }

    /// <summary>Verifies the Unsafe routed host asks the locator by run-time type.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task RoutedViewHostUnsafe_ResolvesByRunTimeType()
    {
        var view = new RoutedTestView();
        var router = new RoutingState(Sequencer.Immediate);
        var locator = new StubViewLocator { ContractlessView = view };
        var host = new RoutedViewHostUnsafe { ViewLocator = locator, Router = router };

        using var navigation = router.Navigate.Execute(new RoutedTestViewModel()).Subscribe();

        using (Assert.Multiple())
        {
            await Assert.That(host.Content).IsSameReferenceAs(view);
            await Assert.That(locator.RuntimeTypeLookups).IsGreaterThan(0);
        }
    }

    /// <summary>Verifies the Unsafe hook replaces the default template the default hook assigned.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task AutoDataTemplateBindingHookUnsafe_ReplacesTheDefaultTemplate()
    {
        var itemsControl = new ListBox();
        Expression<Func<ItemsControl, object?>> expression = static x => x.ItemsSource;

        _ = new AutoDataTemplateBindingHook().ExecuteHook(null, itemsControl, static () => [], () => ViewProperties(itemsControl, expression.Body), BindingDirection.OneWay);
        var proceed = new AutoDataTemplateBindingHookUnsafe().ExecuteHook(null, itemsControl, static () => [], () => ViewProperties(itemsControl, expression.Body), BindingDirection.OneWay);

        using (Assert.Multiple())
        {
            await Assert.That(proceed).IsTrue();
            await Assert.That(itemsControl.ItemTemplate).IsSameReferenceAs(AutoDataTemplateBindingHookUnsafe.DefaultItemTemplate.Value);
        }
    }

    /// <summary>Verifies the Unsafe hook leaves a template the app set itself alone.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task AutoDataTemplateBindingHookUnsafe_KeepsAnAppTemplate()
    {
        var template = new DataTemplate();
        var itemsControl = new ListBox { ItemTemplate = template };
        Expression<Func<ItemsControl, object?>> expression = static x => x.ItemsSource;

        _ = new AutoDataTemplateBindingHookUnsafe().ExecuteHook(null, itemsControl, static () => [], () => ViewProperties(itemsControl, expression.Body), BindingDirection.OneWay);

        await Assert.That(itemsControl.ItemTemplate).IsSameReferenceAs(template);
    }

    /// <summary>Verifies the Unsafe template hosts each item in the Unsafe host.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task AutoDataTemplateBindingHookUnsafe_HostsTheItemInAViewModelViewHostUnsafe()
    {
        var viewModel = new PlainTestViewModel();
        var content = (ContentControl)AutoDataTemplateBindingHookUnsafe.DefaultItemTemplate.Value.LoadContent();

        content.DataContext = viewModel;

        var host = content.Content as ViewModelViewHostUnsafe;
        using (Assert.Multiple())
        {
            await Assert.That(host).IsNotNull();
            await Assert.That(host!.ViewModel).IsSameReferenceAs(viewModel);
        }
    }

    /// <summary>
    /// Verifies the documented registration of the Unsafe hook compiles, registers the hook alongside the default one,
    /// and makes a binding get the Unsafe template.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<WinUIUnsafeTemplateHookExecutor>]
    public async Task AutoDataTemplateBindingHookUnsafe_DocumentedRegistration_GivesABindingTheUnsafeTemplate()
    {
        var hooks = AppLocator.Current.GetServices<IPropertyBindingHook>().ToArray();
        var itemsControl = new ListBox();
        Expression<Func<ItemsControl, object?>> expression = static x => x.ItemsSource;

        var proceed = BindingHooks.ShouldBind(null, itemsControl, static () => [], () => ViewProperties(itemsControl, expression.Body), BindingDirection.OneWay);

        using (Assert.Multiple())
        {
            await Assert.That(hooks.OfType<AutoDataTemplateBindingHookUnsafe>().Count()).IsEqualTo(1);
            await Assert.That(hooks.OfType<AutoDataTemplateBindingHook>().Count()).IsEqualTo(1);
            await Assert.That(proceed).IsTrue();
            await Assert.That(itemsControl.ItemTemplate).IsSameReferenceAs(AutoDataTemplateBindingHookUnsafe.DefaultItemTemplate.Value);
        }
    }

    /// <summary>Builds the observed-change chain a property binding would hand a hook.</summary>
    /// <param name="sender">The bound target.</param>
    /// <param name="expression">The bound property expression.</param>
    /// <returns>The observed-change chain ending at the bound property.</returns>
    private static IObservedChange<object, object>[] ViewProperties(object sender, Expression expression) =>
        [new ObservedChange<object, object>(sender, Reflection.Rewrite(expression), null!)];
}
