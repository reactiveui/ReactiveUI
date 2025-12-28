// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive.Disposables.Fluent;
using System.Windows;
using DynamicData;
using ReactiveUI.Testing;

using TUnit.Core.Executors;

namespace ReactiveUI.Tests.Wpf;

/// <summary>
/// Wpf Active Content Tests.
/// </summary>
[NotInParallel]
public class WpfActiveContentTests
{

    /// <summary>
    /// Validates binding logic for a list-backed view.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<STAThreadExecutor>]
    public async Task BindListFunctionalTest()
    {
        var view = new MockBindListView();
        var vm = view.ViewModel!;

        // Activate the view to trigger bindings
        view.RaiseEvent(new RoutedEventArgs(FrameworkElement.LoadedEvent));

        // Test 1: Add first item
        var test1 = new MockBindListItemViewModel("Test1");
        vm.ActiveListItem.Add(test1);
        using (Assert.Multiple())
        {
            await Assert.That(vm.ListItems.Count).IsEqualTo(1);
            await Assert.That(vm.ActiveItem).IsEqualTo(test1);
            await Assert.That(view.ItemList.Items.Count).IsEqualTo(1);
        }

        // Test 2: Add second item
        var test2 = new MockBindListItemViewModel("Test2");
        vm.ActiveListItem.Add(test2);
        using (Assert.Multiple())
        {
            await Assert.That(vm.ListItems.Count).IsEqualTo(2);
            await Assert.That(vm.ActiveItem).IsEqualTo(test2);
            await Assert.That(view.ItemList.Items.Count).IsEqualTo(2);
        }

        // Test 3: Add third item
        var test3 = new MockBindListItemViewModel("Test3");
        vm.ActiveListItem.Add(test3);
        using (Assert.Multiple())
        {
            await Assert.That(vm.ListItems.Count).IsEqualTo(3);
            await Assert.That(vm.ActiveItem).IsEqualTo(test3);
            await Assert.That(view.ItemList.Items.Count).IsEqualTo(3);
        }

        // Test 4: Select first item (should trigger command that removes items after it)
        await vm.SelectItem.Execute(test1);
        using (Assert.Multiple())
        {
            await Assert.That(vm.ListItems.Count).IsEqualTo(1);
            await Assert.That(vm.ActiveItem).IsEqualTo(test1);
            await Assert.That(view.ItemList.Items.Count).IsEqualTo(1);
        }
    }

    /// <summary>
    /// Ensures view resolution respects contracts and fallback behavior.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<STAThreadExecutor>]
    public async Task ViewModelHostViewTestFallback()
    {
        var resolver = new ModernDependencyResolver();
        resolver.InitializeSplat();
        resolver.InitializeReactiveUI();

        using (resolver.WithResolver())
        {
            await ResolveViewBIfViewBIsRegistered(resolver);
            await ResolveView0WithFallback(resolver);
            await ResolveNoneWithFallbackBypass(resolver);
        }

        async Task ResolveViewBIfViewBIsRegistered(ModernDependencyResolver r)
        {
            r.Register(
                       () => new FakeViewWithContract.View0(),
                       typeof(IViewFor<FakeViewWithContract.MyViewModel>));
            r.Register(
                       () => new FakeViewWithContract.ViewA(),
                       typeof(IViewFor<FakeViewWithContract.MyViewModel>),
                       FakeViewWithContract.ContractA);
            r.Register(
                       () => new FakeViewWithContract.ViewB(),
                       typeof(IViewFor<FakeViewWithContract.MyViewModel>),
                       FakeViewWithContract.ContractB);

            var vm = new FakeViewWithContract.MyViewModel();
            var host = new ViewModelViewHost { ViewModel = vm, ViewContract = FakeViewWithContract.ContractB, };

            // Simulate activation by raising the Loaded event
            var loaded = new RoutedEventArgs { RoutedEvent = FrameworkElement.LoadedEvent };
            host.RaiseEvent(loaded);

            await Assert.That(host.Content).IsNotNull();
            await Assert.That(host.Content).IsAssignableTo<FakeViewWithContract.ViewB>();
        }

        async Task ResolveView0WithFallback(ModernDependencyResolver r)
        {
            r.UnregisterCurrent(
                                typeof(IViewFor<FakeViewWithContract.MyViewModel>),
                                FakeViewWithContract.ContractB);

            var vm = new FakeViewWithContract.MyViewModel();
            var host = new ViewModelViewHost
            {
                ViewModel = vm,
                ViewContract = FakeViewWithContract.ContractB,
                ContractFallbackByPass = false,
            };

            // Simulate activation by raising the Loaded event
            var loaded = new RoutedEventArgs { RoutedEvent = FrameworkElement.LoadedEvent };
            host.RaiseEvent(loaded);

            await Assert.That(host.Content).IsNotNull();
            await Assert.That(host.Content).IsAssignableTo<FakeViewWithContract.View0>();
        }

        async Task ResolveNoneWithFallbackBypass(ModernDependencyResolver r)
        {
            r.UnregisterCurrent(
                                typeof(IViewFor<FakeViewWithContract.MyViewModel>),
                                FakeViewWithContract.ContractB);

            var vm = new FakeViewWithContract.MyViewModel();
            var host = new ViewModelViewHost
            {
                ViewModel = vm,
                ViewContract = FakeViewWithContract.ContractB,
                ContractFallbackByPass = true,
            };

            // Simulate activation by raising the Loaded event
            var loaded = new RoutedEventArgs { RoutedEvent = FrameworkElement.LoadedEvent };
            host.RaiseEvent(loaded);

            await Assert.That(host.Content).IsNull();
        }
    }

    /// <summary>
    /// Verifies the dummy suspension driver calls.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task DummySuspensionDriverTest()
    {
        var dsd = new DummySuspensionDriver();
        dsd.LoadState().Select(static _ => 1).Subscribe(async static v => await Assert.That(v).IsEqualTo(1));
        dsd.SaveState("Save Me").Select(static _ => 2).Subscribe(async static v => await Assert.That(v).IsEqualTo(2));
        dsd.InvalidateState().Select(static _ => 3).Subscribe(async static v => await Assert.That(v).IsEqualTo(3));
    }
}
