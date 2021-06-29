// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Reactive.Concurrency;
using System.Windows;
using DynamicData;
using ReactiveUI.Tests.Wpf;
using Xunit;

namespace ReactiveUI.Tests
{
    public class RoutedViewHostTests
    {
        [StaFact]
        public void RoutedViewHostDefaultContentNotNull()
        {
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

            new[] { true }.AssertAreEqual(windowActivated);
            new[] { true }.AssertAreEqual(controlActivated);

            Assert.NotNull(uc.Content);

            window.Dispatcher.InvokeShutdown();
        }
    }
}
