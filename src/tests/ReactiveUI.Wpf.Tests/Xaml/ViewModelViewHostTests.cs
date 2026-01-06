// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows;

using DynamicData;

using TUnit.Core.Executors;

namespace ReactiveUI.Tests;

/// <summary>
/// Tests for ViewModelViewHost.
/// </summary>
/// <remarks>
/// This test fixture is marked as NonParallelizable because tests modify
/// global service locator state.
/// </remarks>
[NotInParallel]
public class ViewModelViewHostTests
{
    [Before(HookType.Test)]
    public void Setup()
    {
        RxAppBuilder.ResetForTesting();
    }

    [After(Test)]
    public void TearDown()
    {
    }

    [Test]
    [TestExecutor<STAThreadExecutor>]
    public async Task ViewModelViewHostDefaultContentNotNull()
    {
        var uc = new ViewModelViewHost
        {
            DefaultContent = new System.Windows.Controls.Label()
        };

        var activation = new ActivationForViewFetcher();
        activation.GetActivationForView(uc)
            .ToObservableChangeSet(scheduler: ImmediateScheduler.Instance)
            .Bind(out var controlActivated)
            .Subscribe();

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
    public async Task ViewModelViewHostContentNotNullWithViewModelAndActivated()
    {
        RxAppBuilder.CreateReactiveUIBuilder()
            .WithWpf()
            .RegisterView<TestView, TestViewModel>()
            .WithCoreServices()
            .BuildApp();

        var viewModel = new TestViewModel();

        var uc = new ViewModelViewHost
        {
            DefaultContent = new System.Windows.Controls.Label(),
            ViewModel = viewModel
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

        // Test IViewFor<ViewModel> after activated
        await Assert.That(uc.Content).IsTypeOf<TestView>();
    }
}
