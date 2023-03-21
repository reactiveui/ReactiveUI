// Copyright (c) 2023 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Reactive.Concurrency;
using System.Windows;

using DynamicData;
using Xunit;

using FactAttribute = Xunit.WpfFactAttribute;

namespace ReactiveUI.Tests.Wpf
{
    public class WpfActivationForViewFetcherTest
    {
        [Fact]
        public void FrameworkElementIsActivatedAndDeactivated()
        {
            var uc = new WpfTestUserControl();
            var activation = new ActivationForViewFetcher();

            var obs = activation.GetActivationForView(uc);
            obs.ToObservableChangeSet(scheduler: ImmediateScheduler.Instance).Bind(out var activated).Subscribe();

            var loaded = new RoutedEventArgs();
            loaded.RoutedEvent = FrameworkElement.LoadedEvent;

            uc.RaiseEvent(loaded);

            new[] { true }.AssertAreEqual(activated);

            var unloaded = new RoutedEventArgs();
            unloaded.RoutedEvent = FrameworkElement.UnloadedEvent;

            uc.RaiseEvent(unloaded);

            new[] { true, false }.AssertAreEqual(activated);
        }

        [Fact]
        public void IsHitTestVisibleActivatesFrameworkElement()
        {
            var uc = new WpfTestUserControl();
            uc.IsHitTestVisible = false;
            var activation = new ActivationForViewFetcher();

            var obs = activation.GetActivationForView(uc);
            obs.ToObservableChangeSet(scheduler: ImmediateScheduler.Instance).Bind(out var activated).Subscribe();

            var loaded = new RoutedEventArgs();
            loaded.RoutedEvent = FrameworkElement.LoadedEvent;

            uc.RaiseEvent(loaded);

            // Loaded has happened.
            new[] { true }.AssertAreEqual(activated);

            uc.IsHitTestVisible = true;

            // IsHitTestVisible true, we don't want the event to repeat unnecessarily.
            new[] { true }.AssertAreEqual(activated);

            var unloaded = new RoutedEventArgs();
            unloaded.RoutedEvent = FrameworkElement.UnloadedEvent;

            uc.RaiseEvent(unloaded);

            // We had both a loaded/hit test visible change/unloaded happen.
            new[] { true, false }.AssertAreEqual(activated);
        }

        [Fact]
        public void IsHitTestVisibleDeactivatesFrameworkElement()
        {
            var uc = new WpfTestUserControl();
            var activation = new ActivationForViewFetcher();

            var obs = activation.GetActivationForView(uc);
            obs.ToObservableChangeSet(scheduler: ImmediateScheduler.Instance).Bind(out var activated).Subscribe();

            var loaded = new RoutedEventArgs();
            loaded.RoutedEvent = FrameworkElement.LoadedEvent;

            uc.RaiseEvent(loaded);

            new[] { true }.AssertAreEqual(activated);

            uc.IsHitTestVisible = false;

            new[] { true, false }.AssertAreEqual(activated);
        }

        [Fact]
        public void FrameworkElementIsActivatedAndDeactivatedWithHitTest()
        {
            var uc = new WpfTestUserControl();
            var activation = new ActivationForViewFetcher();

            var obs = activation.GetActivationForView(uc);
            obs.ToObservableChangeSet(scheduler: ImmediateScheduler.Instance).Bind(out var activated).Subscribe();

            var loaded = new RoutedEventArgs();
            loaded.RoutedEvent = FrameworkElement.LoadedEvent;

            uc.RaiseEvent(loaded);

            new[] { true }.AssertAreEqual(activated);

            // this should deactivate it
            uc.IsHitTestVisible = false;

            new[] { true, false }.AssertAreEqual(activated);

            // this should activate it
            uc.IsHitTestVisible = true;

            new[] { true, false, true }.AssertAreEqual(activated);

            var unloaded = new RoutedEventArgs();
            unloaded.RoutedEvent = FrameworkElement.UnloadedEvent;

            uc.RaiseEvent(unloaded);

            new[] { true, false, true, false }.AssertAreEqual(activated);
        }
    }
}
