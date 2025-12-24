// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows;
using DynamicData;
using ReactiveUI.Tests.Wpf;

using TUnit.Core.Executors;

namespace ReactiveUI.Tests;

/// <summary>
/// Tests for RoutedViewHost.
/// </summary>
/// <remarks>
/// This test fixture is marked as NonParallelizable because tests call
/// Locator.CurrentMutable.InitializeSplat() and Locator.CurrentMutable.InitializeReactiveUI(),
/// which mutate global service locator state. This state must not be mutated concurrently
/// by parallel tests.
/// </remarks>
[NotInParallel]
public class RoutedViewHostTests
{
    [Test]
    [TestExecutor<STAThreadExecutor>]
    public async Task RoutedViewHostDefaultContentNotNull()
    {
        Locator.CurrentMutable.InitializeSplat();
        Locator.CurrentMutable.InitializeReactiveUI();
        var uc = new RoutedViewHost
        {
            DefaultContent = new System.Windows.Controls.Label()
        };
        var window = new WpfTestWindow();
        window.RootGrid.Children.Add(uc);

        var activation = new ActivationForViewFetcher();

        activation.GetActivationForView(window).ToObservableChangeSet(scheduler: ImmediateScheduler.Instance).Bind(out var windowActivated).Subscribe();

        activation.GetActivationForView(uc).ToObservableChangeSet(scheduler: ImmediateScheduler.Instance).Bind(out var controlActivated).Subscribe();

        var loaded = new RoutedEventArgs
        {
            RoutedEvent = FrameworkElement.LoadedEvent
        };

        window.RaiseEvent(loaded);
        uc.RaiseEvent(loaded);

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        new[] { true }.AssertAreEqual(windowActivated);
        new[] { true }.AssertAreEqual(controlActivated);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

        await Assert.That(uc.Content).IsNotNull();

        window.Dispatcher.InvokeShutdown();
    }

    [Test]
    [TestExecutor<STAThreadExecutor>]
    public async Task RoutedViewHostDefaultContentNotNullWithViewModelAndActivated()
    {
        Locator.CurrentMutable.InitializeSplat();
        Locator.CurrentMutable.InitializeReactiveUI();
        Locator.CurrentMutable.Register<RoutingState>(static () => new());
        Locator.CurrentMutable.Register<TestViewModel>(static () => new());
        Locator.CurrentMutable.Register<IViewFor<TestViewModel>>(static () => new TestView());

        var uc = new RoutedViewHost
        {
            DefaultContent = new System.Windows.Controls.Label(),
            Router = Locator.Current.GetService<RoutingState>()!
        };
        var window = new WpfTestWindow();
        window.RootGrid.Children.Add(uc);

        var activation = new ActivationForViewFetcher();

        activation.GetActivationForView(window).ToObservableChangeSet(scheduler: ImmediateScheduler.Instance).Bind(out var windowActivated).Subscribe();

        activation.GetActivationForView(uc).ToObservableChangeSet(scheduler: ImmediateScheduler.Instance).Bind(out var controlActivated).Subscribe();

        var loaded = new RoutedEventArgs
        {
            RoutedEvent = FrameworkElement.LoadedEvent
        };

        window.RaiseEvent(loaded);
        uc.RaiseEvent(loaded);

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        new[] { true }.AssertAreEqual(windowActivated);
        new[] { true }.AssertAreEqual(controlActivated);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

        // Default Content
        await Assert.That(uc.Content).IsAssignableTo<System.Windows.Controls.Label>();

        // Test Navigation after activated
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        uc.Router.Navigate.Execute(Locator.Current.GetService<TestViewModel>()!);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        await Assert.That(uc.Content).IsAssignableTo<TestView>();

        window.Dispatcher.InvokeShutdown();
    }

    [Test]
    [TestExecutor<STAThreadExecutor>]
    public async Task RoutedViewHostDefaultContentNotNullWithViewModelAndNotActivated()
    {
        Locator.CurrentMutable.InitializeSplat();
        Locator.CurrentMutable.InitializeReactiveUI();
        Locator.CurrentMutable.Register<RoutingState>(static () => new());
        Locator.CurrentMutable.Register<TestViewModel>(static () => new());
        Locator.CurrentMutable.Register<IViewFor<TestViewModel>>(static () => new TestView());

        var uc = new RoutedViewHost
        {
            DefaultContent = new System.Windows.Controls.Label(),
            Router = Locator.Current.GetService<RoutingState>()!
        };
        var window = new WpfTestWindow();
        window.RootGrid.Children.Add(uc);

        var activation = new ActivationForViewFetcher();

        activation.GetActivationForView(window).ToObservableChangeSet(scheduler: ImmediateScheduler.Instance).Bind(out var windowActivated).Subscribe();

        activation.GetActivationForView(uc).ToObservableChangeSet(scheduler: ImmediateScheduler.Instance).Bind(out var controlActivated).Subscribe();

        var loaded = new RoutedEventArgs
        {
            RoutedEvent = FrameworkElement.LoadedEvent
        };

        // Test navigation before Activation.
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        uc.Router.Navigate.Execute(Locator.Current.GetService<TestViewModel>()!);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

        // Activate
        window.RaiseEvent(loaded);
        uc.RaiseEvent(loaded);

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        new[] { true }.AssertAreEqual(windowActivated);
        new[] { true }.AssertAreEqual(controlActivated);
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

        // Test Navigation before activated
        await Assert.That(uc.Content).IsAssignableTo<TestView>();

        window.Dispatcher.InvokeShutdown();
    }
}
