// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Windows;

using DynamicData;
using ReactiveUI.Builder;
using ReactiveUI.Tests.Utilities.AppBuilder;
using ReactiveUI.Tests.Wpf.Mocks.ViewModelViewHosts;
using ReactiveUI.Tests.Xaml.Mocks;
using TUnit.Core.Executors;

namespace ReactiveUI.Tests.Wpf;

/// <summary>
/// Wpf Active Content Tests.
/// </summary>
[NotInParallel]
public class WpfActiveContentTests
{
    private const int TwoItems = 2;
    private const int ThreeItems = 3;
    private const int SaveResultValue = 2;
    private const int InvalidateResultValue = 3;

    /// <summary>
    /// Validates binding logic for a list-backed view.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<WpfTestExecutor>]
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
            await Assert.That(vm.ListItems.Count).IsEqualTo(TwoItems);
            await Assert.That(vm.ActiveItem).IsEqualTo(test2);
            await Assert.That(view.ItemList.Items.Count).IsEqualTo(TwoItems);
        }

        // Test 3: Add third item
        var test3 = new MockBindListItemViewModel("Test3");
        vm.ActiveListItem.Add(test3);
        using (Assert.Multiple())
        {
            await Assert.That(vm.ListItems.Count).IsEqualTo(ThreeItems);
            await Assert.That(vm.ActiveItem).IsEqualTo(test3);
            await Assert.That(view.ItemList.Items.Count).IsEqualTo(ThreeItems);
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
    /// Verifies that ViewB is resolved when registered with the correct contract.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<ViewBRegisteredExecutor>]
    public async Task ResolveViewBIfViewBIsRegistered()
    {
        var vm = new FakeViewWithContract.MyViewModel();
        var host = new ViewModelViewHost
        {
            ViewModel = vm,
            ViewContract = FakeViewWithContract.ContractB,
        };

        // Simulate activation by raising the Loaded event
        var loaded = new RoutedEventArgs { RoutedEvent = FrameworkElement.LoadedEvent };
        host.RaiseEvent(loaded);

        await Assert.That(host.Content).IsNotNull();
        await Assert.That(host.Content).IsAssignableTo<FakeViewWithContract.ViewB>();
    }

    /// <summary>
    /// Verifies that View0 is used as fallback when ViewB is not registered.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<View0FallbackExecutor>]
    public async Task ResolveView0WithFallback()
    {
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

    /// <summary>
    /// Verifies that no view is resolved when fallback bypass is enabled and ViewB is not registered.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<NoneWithBypassExecutor>]
    public async Task ResolveNoneWithFallbackBypass()
    {
        var vm = new FakeViewWithContract.MyViewModel();
        var host = new ViewModelViewHost
        {
            ContractFallbackByPass = true,
            ViewContract = FakeViewWithContract.ContractB,
            ViewModel = vm,
        };

        // Simulate activation by raising the Loaded event
        var loaded = new RoutedEventArgs { RoutedEvent = FrameworkElement.LoadedEvent };
        host.RaiseEvent(loaded);

        await Assert.That(host.Content).IsNull();
    }

    /// <summary>
    /// Verifies the dummy suspension driver calls.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task DummySuspensionDriverTest()
    {
        var dsd = new DummySuspensionDriver();
        int? loadResult = null;
        int? saveResult = null;
        int? invalidateResult = null;
        dsd.LoadState().Select(static _ => 1).Subscribe(v => loadResult = v);
        dsd.SaveState("Save Me").Select(static _ => SaveResultValue).Subscribe(v => saveResult = v);
        dsd.InvalidateState().Select(static _ => InvalidateResultValue).Subscribe(v => invalidateResult = v);
        await Assert.That(loadResult).IsEqualTo(1);
        await Assert.That(saveResult).IsEqualTo(SaveResultValue);
        await Assert.That(invalidateResult).IsEqualTo(InvalidateResultValue);
    }

    /// <summary>
    /// A test executor that registers the default, ContractA and ContractB views.
    /// </summary>
    public class ViewBRegisteredExecutor : STAThreadExecutor
    {
        /// <summary>
        /// Helper that manages app builder setup and teardown for the test.
        /// </summary>
        private readonly AppBuilderTestHelper _helper = new();

        /// <inheritdoc/>
        protected override void Initialize()
        {
            base.Initialize();

            _helper.Initialize(builder =>
            {
                builder
                    .WithWpf()
                    .WithRegistration(r => r.RegisterConstant<IViewFor<FakeViewWithContract.MyViewModel>>(new FakeViewWithContract.View0()))
                    .WithRegistration(r => r.RegisterConstant<IViewFor<FakeViewWithContract.MyViewModel>>(new FakeViewWithContract.ViewA(), FakeViewWithContract.ContractA))
                    .WithRegistration(r => r.RegisterConstant<IViewFor<FakeViewWithContract.MyViewModel>>(new FakeViewWithContract.ViewB(), FakeViewWithContract.ContractB))
                    .WithMainThreadScheduler(ImmediateScheduler.Instance)
                    .WithTaskPoolScheduler(ImmediateScheduler.Instance)
                    .WithCoreServices();
            });
        }

        /// <inheritdoc/>
        protected override void CleanUp()
        {
            _helper.CleanUp();
            base.CleanUp();
        }
    }

    /// <summary>
    /// A test executor that registers the default and ContractA views to test fallback resolution.
    /// </summary>
    public class View0FallbackExecutor : STAThreadExecutor
    {
        /// <summary>
        /// Helper that manages app builder setup and teardown for the test.
        /// </summary>
        private readonly AppBuilderTestHelper _helper = new();

        /// <inheritdoc/>
        protected override void Initialize()
        {
            base.Initialize();

            _helper.Initialize(builder =>
            {
                builder
                    .WithWpf()
                    .WithRegistration(r => r.RegisterConstant<IViewFor<FakeViewWithContract.MyViewModel>>(new FakeViewWithContract.View0()))
                    .WithRegistration(r => r.RegisterConstant<IViewFor<FakeViewWithContract.MyViewModel>>(new FakeViewWithContract.ViewA(), FakeViewWithContract.ContractA))
                    .WithMainThreadScheduler(ImmediateScheduler.Instance)
                    .WithTaskPoolScheduler(ImmediateScheduler.Instance)
                    .WithCoreServices();
            });
        }

        /// <inheritdoc/>
        protected override void CleanUp()
        {
            _helper.CleanUp();
            base.CleanUp();
        }
    }

    /// <summary>
    /// A test executor that registers the default and ContractA views for bypass-resolution tests.
    /// </summary>
    public class NoneWithBypassExecutor : STAThreadExecutor
    {
        /// <summary>
        /// Helper that manages app builder setup and teardown for the test.
        /// </summary>
        private readonly AppBuilderTestHelper _helper = new();

        /// <inheritdoc/>
        protected override void Initialize()
        {
            base.Initialize();

            _helper.Initialize(builder =>
            {
                builder
                    .WithWpf()
                    .WithRegistration(r => r.RegisterConstant<IViewFor<FakeViewWithContract.MyViewModel>>(new FakeViewWithContract.View0()))
                    .WithRegistration(r => r.RegisterConstant<IViewFor<FakeViewWithContract.MyViewModel>>(new FakeViewWithContract.ViewA(), FakeViewWithContract.ContractA))
                    .WithMainThreadScheduler(ImmediateScheduler.Instance)
                    .WithTaskPoolScheduler(ImmediateScheduler.Instance)
                    .WithCoreServices();
            });
        }

        /// <inheritdoc/>
        protected override void CleanUp()
        {
            _helper.CleanUp();
            base.CleanUp();
        }
    }

    /// <summary>
    /// A test executor that registers the default, ContractA and ContractB views.
    /// </summary>
    public class ExecutorBIfViewBIsRegistered : STAThreadExecutor
    {
        /// <summary>
        /// Helper that manages app builder setup and teardown for the test.
        /// </summary>
        private readonly AppBuilderTestHelper _helper = new();

        /// <inheritdoc/>
        protected override void Initialize()
        {
            base.Initialize();

            _helper.Initialize(builder =>
            {
                builder
                    .WithWpf()
                    .WithRegistration(r => r.RegisterConstant<IViewFor<FakeViewWithContract.MyViewModel>>(new FakeViewWithContract.View0()))
                    .WithRegistration(r => r.RegisterConstant<IViewFor<FakeViewWithContract.MyViewModel>>(new FakeViewWithContract.ViewA(), FakeViewWithContract.ContractA))
                    .WithRegistration(r => r.RegisterConstant<IViewFor<FakeViewWithContract.MyViewModel>>(new FakeViewWithContract.ViewB(), FakeViewWithContract.ContractB))
                    .WithMainThreadScheduler(ImmediateScheduler.Instance)
                    .WithTaskPoolScheduler(ImmediateScheduler.Instance)
                    .WithCoreServices();
            });
        }

        /// <inheritdoc/>
        protected override void CleanUp()
        {
            _helper.CleanUp();
            base.CleanUp();
        }
    }
}
