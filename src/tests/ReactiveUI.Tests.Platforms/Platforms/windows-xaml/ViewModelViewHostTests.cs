// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows;
using DynamicData;
using ReactiveUI.Tests.Wpf;

namespace ReactiveUI.Tests;

/// <summary>
/// Tests for ViewModelViewHost.
/// </summary>
/// <remarks>
/// This test fixture is marked as NonParallelizable because some tests call
/// Locator.CurrentMutable.Register() and access Locator.Current, which interact with
/// global service locator state. This state must not be mutated concurrently by parallel tests.
/// </remarks>
[TestFixture]
[NonParallelizable]
public class ViewModelViewHostTests
{
    [Test]
    [Apartment(ApartmentState.STA)]
    public void ViewModelViewHostDefaultContentNotNull()
    {
        var uc = new ViewModelViewHost
        {
            DefaultContent = new System.Windows.Controls.Label()
        };
        var window = new WpfTestWindow();
        window.RootGrid.Children.Add(uc);

        var activation = new ActivationForViewFetcher();

        activation.GetActivationForView(window)
             .ToObservableChangeSet(scheduler: ImmediateScheduler.Instance)
             .Bind(out var windowActivated)
             .Subscribe();

        activation.GetActivationForView(uc)
            .ToObservableChangeSet(scheduler: ImmediateScheduler.Instance)
            .Bind(out var controlActivated)
            .Subscribe();

        var loaded = new RoutedEventArgs
        {
            RoutedEvent = FrameworkElement.LoadedEvent
        };

        window.RaiseEvent(loaded);
        uc.RaiseEvent(loaded);

        new[] { true }.AssertAreEqual(windowActivated);
        new[] { true }.AssertAreEqual(controlActivated);

        Assert.That(uc.Content, Is.Not.Null);

        window.Dispatcher.InvokeShutdown();
    }

    [Test]
    [Apartment(ApartmentState.STA)]
    public void ViewModelViewHostContentNotNullWithViewModelAndActivated()
    {
        Locator.CurrentMutable.Register<TestViewModel>(static () => new());
        Locator.CurrentMutable.Register<IViewFor<TestViewModel>>(static () => new TestView());

        var uc = new ViewModelViewHost
        {
            DefaultContent = new System.Windows.Controls.Label(),
            ViewModel = Locator.Current.GetService<TestViewModel>()
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

        new[] { true }.AssertAreEqual(windowActivated);
        new[] { true }.AssertAreEqual(controlActivated);

        // Test IViewFor<ViewModel> after activated
        Assert.That(uc.Content, Is.TypeOf<TestView>());

        window.Dispatcher.InvokeShutdown();
    }
}
