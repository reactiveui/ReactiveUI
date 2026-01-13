// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows;
using DynamicData;
using ReactiveUI.Tests.Utilities;
using ReactiveUI.Tests.Xaml.Mocks;
using TUnit.Core.Executors;

namespace ReactiveUI.Tests.Xaml;

/// <summary>
/// Tests for RoutedViewHost.
/// </summary>
/// <remarks>
/// This test fixture is marked as NonParallelizable because tests modify
/// global service locator state.
/// </remarks>
[NotInParallel]
public class RoutedViewHostTests
{
    [After(Test)]
    public void TearDown()
    {
        RxAppBuilder.ResetForTesting();
    }

    [Test]
    [TestExecutor<STAThreadExecutor>]
    [Skip("Flaky test - needs investigation")]
    public async Task RoutedViewHostDefaultContentNotNull()
    {
        RxAppBuilder.CreateReactiveUIBuilder()
            .WithWpf()
            .WithCoreServices()
            .BuildApp();

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
    [TestExecutor<STAThreadExecutor>]
    [Skip("Flaky test - needs investigation")]
    public async Task RoutedViewHostDefaultContentNotNullWithViewModelAndActivated()
    {
        var router = new RoutingState(ImmediateScheduler.Instance);
        var viewModel = new TestViewModel();
        RxAppBuilder.CreateReactiveUIBuilder()
            .WithCoreServices()
            .WithWpf()
            .RegisterView<TestView, TestViewModel>()
            .WithRegistration(r => r.Register(() => new TestView()))
            .WithRegistration(r => r.RegisterConstant(viewModel))
            .ConfigureViewLocator(locator => locator.Map<TestViewModel, TestView>(() => new TestView()))
            .BuildApp();

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
    [TestExecutor<STAThreadExecutor>]
    [Skip("Flaky test - needs investigation")]
    public async Task RoutedViewHostDefaultContentNotNullWithViewModelAndNotActivated()
    {
        RxAppBuilder.CreateReactiveUIBuilder()
            .WithWpf()
            .RegisterView<TestView, TestViewModel>()
            .WithCoreServices()
            .BuildApp();

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
}
