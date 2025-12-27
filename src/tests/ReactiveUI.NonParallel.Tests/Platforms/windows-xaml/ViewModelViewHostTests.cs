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
/// Tests for ViewModelViewHost.
/// </summary>
/// <remarks>
/// This test fixture is marked as NonParallelizable because some tests call
/// Locator.CurrentMutable.Register() and access Locator.Current, which interact with
/// global service locator state. This state must not be mutated concurrently by parallel tests.
/// </remarks>
[NotInParallel]
public class ViewModelViewHostTests
{
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
        Locator.CurrentMutable.Register<TestViewModel>(static () => new());
        Locator.CurrentMutable.Register<IViewFor<TestViewModel>>(static () => new TestView());

        var uc = new ViewModelViewHost
        {
            DefaultContent = new System.Windows.Controls.Label(),
            ViewModel = Locator.Current.GetService<TestViewModel>()
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
