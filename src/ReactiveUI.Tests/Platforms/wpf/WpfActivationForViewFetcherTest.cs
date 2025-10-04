// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Threading;
using System.Windows;

using DynamicData;

namespace ReactiveUI.Tests.Wpf;

[TestFixture]
[Apartment(ApartmentState.STA)]
public class WpfActivationForViewFetcherTest
{
    [Test]
    [Apartment(ApartmentState.STA)]
    public void FrameworkElementIsActivatedAndDeactivated()
    {
        var uc = new WpfTestUserControl();
        var activation = new ActivationForViewFetcher();

        var obs = activation.GetActivationForView(uc);
        obs.ToObservableChangeSet(scheduler: ImmediateScheduler.Instance).Bind(out var activated).Subscribe();

        var loaded = new RoutedEventArgs
        {
            RoutedEvent = FrameworkElement.LoadedEvent
        };

        uc.RaiseEvent(loaded);

        new[] { true }.AssertAreEqual(activated);

        var unloaded = new RoutedEventArgs
        {
            RoutedEvent = FrameworkElement.UnloadedEvent
        };

        uc.RaiseEvent(unloaded);

        new[] { true, false }.AssertAreEqual(activated);
    }

    [Test]
    [Apartment(ApartmentState.STA)]
    public void IsHitTestVisibleActivatesFrameworkElement()
    {
        var uc = new WpfTestUserControl
        {
            IsHitTestVisible = false
        };
        var activation = new ActivationForViewFetcher();

        var obs = activation.GetActivationForView(uc);
        obs.ToObservableChangeSet(scheduler: ImmediateScheduler.Instance).Bind(out var activated).Subscribe();

        var loaded = new RoutedEventArgs
        {
            RoutedEvent = FrameworkElement.LoadedEvent
        };

        uc.RaiseEvent(loaded);

        // Loaded has happened.
        new[] { true }.AssertAreEqual(activated);

        uc.IsHitTestVisible = true;

        // IsHitTestVisible true, we don't want the event to repeat unnecessarily.
        new[] { true }.AssertAreEqual(activated);

        var unloaded = new RoutedEventArgs
        {
            RoutedEvent = FrameworkElement.UnloadedEvent
        };

        uc.RaiseEvent(unloaded);

        // We had both a loaded/hit test visible change/unloaded happen.
        new[] { true, false }.AssertAreEqual(activated);
    }

    [Test]
    [Apartment(ApartmentState.STA)]
    public void IsHitTestVisibleDeactivatesFrameworkElement()
    {
        var uc = new WpfTestUserControl();
        var activation = new ActivationForViewFetcher();

        var obs = activation.GetActivationForView(uc);
        obs.ToObservableChangeSet(scheduler: ImmediateScheduler.Instance).Bind(out var activated).Subscribe();

        var loaded = new RoutedEventArgs
        {
            RoutedEvent = FrameworkElement.LoadedEvent
        };

        uc.RaiseEvent(loaded);

        new[] { true }.AssertAreEqual(activated);

        uc.IsHitTestVisible = false;

        new[] { true, false }.AssertAreEqual(activated);
    }

    [Test]
    [Apartment(ApartmentState.STA)]
    public void FrameworkElementIsActivatedAndDeactivatedWithHitTest()
    {
        var uc = new WpfTestUserControl();
        var activation = new ActivationForViewFetcher();

        var obs = activation.GetActivationForView(uc);
        obs.ToObservableChangeSet(scheduler: ImmediateScheduler.Instance).Bind(out var activated).Subscribe();

        var loaded = new RoutedEventArgs
        {
            RoutedEvent = FrameworkElement.LoadedEvent
        };

        uc.RaiseEvent(loaded);

        new[] { true }.AssertAreEqual(activated);

        // this should deactivate it
        uc.IsHitTestVisible = false;

        new[] { true, false }.AssertAreEqual(activated);

        // this should activate it
        uc.IsHitTestVisible = true;

        new[] { true, false, true }.AssertAreEqual(activated);

        var unloaded = new RoutedEventArgs
        {
            RoutedEvent = FrameworkElement.UnloadedEvent
        };

        uc.RaiseEvent(unloaded);

        new[] { true, false, true, false }.AssertAreEqual(activated);
    }
}
