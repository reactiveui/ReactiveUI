// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Reactive.Concurrency;
using System.Threading;
using System.Threading.Tasks;
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
        public void WindowIsActivatedAndDeactivated()
        {
            var window = new WpfTestWindow();
            var activation = new ActivationForViewFetcher();

            var obs = activation.GetActivationForView(window);
            obs.ToObservableChangeSet(scheduler: ImmediateScheduler.Instance).Bind(out var activated).Subscribe();

            var loaded = new RoutedEventArgs();
            loaded.RoutedEvent = FrameworkElement.LoadedEvent;

            window.RaiseEvent(loaded);

            new[] { true }.AssertAreEqual(activated);

            window.Close();

            new[] { true, false }.AssertAreEqual(activated);
        }

        [StaFact]
        public void WindowAndFrameworkElementAreActivatedAndDeactivated()
        {
            var window = new WpfTestWindow();
            var uc = new WpfTestUserControl();

            window.RootGrid.Children.Add(uc);

            var activation = new ActivationForViewFetcher();

            var windowObs = activation.GetActivationForView(window);
            windowObs.ToObservableChangeSet(scheduler: ImmediateScheduler.Instance).Bind(out var windowActivated).Subscribe();

            var ucObs = activation.GetActivationForView(uc);
            ucObs.ToObservableChangeSet(scheduler: ImmediateScheduler.Instance).Bind(out var controlActivated).Subscribe();

            var loaded = new RoutedEventArgs();
            loaded.RoutedEvent = FrameworkElement.LoadedEvent;

            window.RaiseEvent(loaded);
            uc.RaiseEvent(loaded);

            new[] { true }.AssertAreEqual(windowActivated);
            new[] { true }.AssertAreEqual(controlActivated);

            window.Dispatcher.InvokeShutdown();

            new[] { true, false }.AssertAreEqual(windowActivated);
            new[] { true, false }.AssertAreEqual(controlActivated);
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

        [StaFact]
        public void TransitioninContentControlDpiTest()
        {
            var uiThread = new Thread(() =>
            {
                var window = new TCMockWindow();
                var app = new Application();

                window.WhenActivated(async d =>
                {
                    TransitioningContentControl.OverrideDpi = true;
                    window.TransitioningContent.Height = 500;
                    window.TransitioningContent.Width = 500;
                    window.TransitioningContent.Content = new FirstView();
                    await Task.Delay(5000).ConfigureAwait(true);
                    window.TransitioningContent.Content = new SecondView();
                    await Task.Delay(5000).ConfigureAwait(true);
                    window.TransitioningContent.Height = 300;
                    window.TransitioningContent.Width = 300;
                    window.TransitioningContent.Content = new FirstView();
                    await Task.Delay(5000).ConfigureAwait(true);
                    window.TransitioningContent.Content = new SecondView();
                    window.TransitioningContent.Height = 0.25;
                    window.TransitioningContent.Width = 0.25;
                    window.TransitioningContent.Content = new FirstView();
                    await Task.Delay(5000).ConfigureAwait(true);
                    window.TransitioningContent.Content = new SecondView();
                    window.TransitioningContent.Height = 500;
                    window.TransitioningContent.Width = 500;
                    window.TransitioningContent.Content = new FirstView();
                    await Task.Delay(5000).ConfigureAwait(true);
                    window.TransitioningContent.Content = new SecondView();
                    await Task.Delay(5000).ConfigureAwait(true);
                    window.TransitioningContent.Height = 300;
                    window.TransitioningContent.Width = 300;
                    window.TransitioningContent.Content = new FirstView();
                    await Task.Delay(5000).ConfigureAwait(true);
                    window.TransitioningContent.Content = new SecondView();
                    window.TransitioningContent.Height = 0.25;
                    window.TransitioningContent.Width = 0.25;
                    window.TransitioningContent.Content = new FirstView();
                    await Task.Delay(5000).ConfigureAwait(true);
                    window.TransitioningContent.Content = new SecondView();
                    window.Dispatcher.InvokeShutdown();
                    app.Shutdown();
                });
                app.Run(window);
            });
            uiThread.SetApartmentState(ApartmentState.STA);
            uiThread.Start();
            Thread.Sleep(100);
            uiThread.Join();
        }
    }
}
