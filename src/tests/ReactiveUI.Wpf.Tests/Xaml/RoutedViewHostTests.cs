// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows;
using System.Windows.Threading;

using DynamicData;

using ReactiveUI.Tests.Utilities.AppBuilder;
using ReactiveUI.Tests.Wpf;
using ReactiveUI.Tests.Xaml.Mocks;

namespace ReactiveUI.Tests.Xaml;

/// <summary>
/// Tests for RoutedViewHost.
/// </summary>
/// <remarks>
/// This test fixture is marked as NonParallelizable because tests modify
/// global service locator state.
/// </remarks>
[NotInParallel]
[TestExecutor<WpfTestExecutor>]
public class RoutedViewHostTests
{
    [Test]
    public async Task RoutedViewHostDefaultContentNotNull()
    {
        var uc = new RoutedViewHost
        {
            DefaultContent = new System.Windows.Controls.Label()
        };

        var activation = new ActivationForViewFetcher();
        activation.GetActivationForView(uc).ToObservableChangeSet(scheduler: ImmediateScheduler.Instance).Bind(out var controlActivated).Subscribe();

        // Simulate activation by raising the Loaded event
        var loaded = new RoutedEventArgs
        {
            RoutedEvent = FrameworkElement.LoadedEvent
        };
        uc.RaiseEvent(loaded);

        await new[] { true }.AssertAreEqual(controlActivated);

        await Assert.That(uc.Content).IsNotNull();
    }

    [Test]
    [TestExecutor<WpfWithViewAndRoutingExecutor>]
    public async Task RoutedViewHostDefaultContentNotNullWithViewModelAndActivated()
    {
        var router = new RoutingState(ImmediateScheduler.Instance);
        var viewModel = new TestViewModel();

        var uc = new RoutedViewHost
        {
            DefaultContent = new System.Windows.Controls.Label(),
            Router = router
        };

        var activation = new ActivationForViewFetcher();
        activation.GetActivationForView(uc).ToObservableChangeSet(scheduler: ImmediateScheduler.Instance).Bind(out var controlActivated).Subscribe();

        // Simulate activation by raising the Loaded event
        var loaded = new RoutedEventArgs
        {
            RoutedEvent = FrameworkElement.LoadedEvent
        };
        uc.RaiseEvent(loaded);

        await new[] { true }.AssertAreEqual(controlActivated);

        // Default Content
        await Assert.That(uc.Content).IsAssignableTo<System.Windows.Controls.Label>();

        // Test Navigation after activated
        router.Navigate.Execute(viewModel).Subscribe();
        await Assert.That(uc.Content).IsAssignableTo<TestView>();
    }

    [Test]
    [TestExecutor<WpfWithViewAndRoutingExecutor>]
    public async Task RoutedViewHostDefaultContentNotNullWithViewModelAndNotActivated()
    {
        var router = new RoutingState();
        var viewModel = new TestViewModel();

        var uc = new RoutedViewHost
        {
            DefaultContent = new System.Windows.Controls.Label(),
            Router = router
        };

        var activation = new ActivationForViewFetcher();
        activation.GetActivationForView(uc).ToObservableChangeSet(scheduler: ImmediateScheduler.Instance).Bind(out var controlActivated).Subscribe();

        // Test navigation before Activation.
        router.Navigate.Execute(viewModel).Subscribe();

        // Activate by raising the Loaded event
        var loaded = new RoutedEventArgs
        {
            RoutedEvent = FrameworkElement.LoadedEvent
        };
        uc.RaiseEvent(loaded);

        await new[] { true }.AssertAreEqual(controlActivated);

        // Test Navigation before activated
        await Assert.That(uc.Content).IsAssignableTo<TestView>();
    }

    /// <summary>
    /// Test executor for RoutedViewHost tests that require view registration.
    /// </summary>
    public class WpfWithViewAndRoutingExecutor : STAThreadExecutor
    {
        private readonly AppBuilderTestHelper _helper = new();

        /// <inheritdoc/>
        protected override void Initialize()
        {
            base.Initialize();

            _helper.Initialize(builder =>
            {
                // Include WPF platform services and register test view for routing tests
                builder
                    .WithWpf()
                    .RegisterView<TestView, TestViewModel>()
                    .WithCoreServices();

                // Configure WPF scheduler for test execution
                // Note: WithWpf() skips scheduler setup when InUnitTestRunner() is true,
                // so we must manually configure it for tests that need WPF controls
                var dispatcher = Dispatcher.CurrentDispatcher;
                RxSchedulers.MainThreadScheduler = new DispatcherScheduler(dispatcher);
                RxSchedulers.TaskpoolScheduler = TaskPoolScheduler.Default;
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
