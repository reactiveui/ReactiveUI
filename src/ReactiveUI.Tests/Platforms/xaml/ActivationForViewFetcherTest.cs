// Copyright (c) 2019 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using DynamicData;
using Microsoft.Reactive.Testing;
using ReactiveUI.Testing;
using Xunit;

namespace ReactiveUI.Tests
{
    public class ActivationForViewFetcherTest
    {
        [WpfFact]
        public void FrameworkElementIsActivatedAndDeactivated()
        {
            var uc = new TestUserControl();
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

        [WpfFact]
        public void IsHitTestVisibleActivatesFrameworkElement()
        {
            var uc = new TestUserControl();
            uc.IsHitTestVisible = false;
            var activation = new ActivationForViewFetcher();

            var obs = activation.GetActivationForView(uc);
            obs.ToObservableChangeSet(scheduler: ImmediateScheduler.Instance).Bind(out var activated).Subscribe();

            var loaded = new RoutedEventArgs();
            loaded.RoutedEvent = FrameworkElement.LoadedEvent;

            uc.RaiseEvent(loaded);

            // IsHitTestVisible still false
            Array.Empty<bool>().AssertAreEqual(activated);

            uc.IsHitTestVisible = true;

            // IsHitTestVisible true
            new[] { true }.AssertAreEqual(activated);

            var unloaded = new RoutedEventArgs();
            unloaded.RoutedEvent = FrameworkElement.UnloadedEvent;

            uc.RaiseEvent(unloaded);

            new[] { true, false }.AssertAreEqual(activated);
        }

        [WpfFact]
        public void IsHitTestVisibleDeactivatesFrameworkElement()
        {
            var uc = new TestUserControl();
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

        [WpfFact]
        public void FrameworkElementIsActivatedAndDeactivatedWithHitTest()
        {
            var uc = new TestUserControl();
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

        public class TestUserControl : UserControl, IActivatable
        {
        }
    }
}
