// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows;
using DynamicData;
using ReactiveUI.Tests.Infrastructure.StaticState;
using ReactiveUI.Tests.Wpf;

using TUnit.Core.Executors;

namespace ReactiveUI.Tests;

/// <summary>
/// Tests for RoutedViewHost.
/// </summary>
/// <remarks>
/// This test fixture is marked as NonParallelizable because tests modify
/// global service locator state. This state must not be mutated concurrently
/// by parallel tests.
/// </remarks>
[NotInParallel]
public class RoutedViewHostTests
{
    private WpfLocatorScope? _locatorScope;

    [Before(Test)]
    public void SetUp()
    {
        _locatorScope = new WpfLocatorScope();
    }

    [After(Test)]
    public void TearDown()
    {
        _locatorScope?.Dispose();
    }

    [Test]
    [TestExecutor<STAThreadExecutor>]
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
    [TestExecutor<STAThreadExecutor>]
    public async Task RoutedViewHostDefaultContentNotNullWithViewModelAndActivated()
    {
        Locator.CurrentMutable.Register<RoutingState>(static () => new());
        Locator.CurrentMutable.Register<TestViewModel>(static () => new());
        Locator.CurrentMutable.Register<IViewFor<TestViewModel>>(static () => new TestView());

        var uc = new RoutedViewHost
        {
            DefaultContent = new System.Windows.Controls.Label(),
            Router = Locator.Current.GetService<RoutingState>()!
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
        await uc.Router.Navigate.Execute(Locator.Current.GetService<TestViewModel>()!);
        await Assert.That(uc.Content).IsAssignableTo<TestView>();
    }

    [Test]
    [TestExecutor<STAThreadExecutor>]
    public async Task RoutedViewHostDefaultContentNotNullWithViewModelAndNotActivated()
    {
        Locator.CurrentMutable.Register<RoutingState>(static () => new());
        Locator.CurrentMutable.Register<TestViewModel>(static () => new());
        Locator.CurrentMutable.Register<IViewFor<TestViewModel>>(static () => new TestView());

        var uc = new RoutedViewHost
        {
            DefaultContent = new System.Windows.Controls.Label(),
            Router = Locator.Current.GetService<RoutingState>()!
        };

        var activation = new ActivationForViewFetcher();
        activation.GetActivationForView(uc).ToObservableChangeSet(scheduler: ImmediateScheduler.Instance).Bind(out var controlActivated).Subscribe();

        // Test navigation before Activation.
        await uc.Router.Navigate.Execute(Locator.Current.GetService<TestViewModel>()!);

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
