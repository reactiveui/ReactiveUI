// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows;
using DynamicData;
using NUnit.Framework.Legacy;
using ReactiveUI.Testing;

namespace ReactiveUI.Tests.Wpf;

/// <summary>
/// Wpf Active Content Tests.
/// NOTE: Only one Test can create an AppDomain, all Active content tests must go in this class.
/// Add to WpfActiveContentApp to add any additional mock windows.
/// </summary>
public class WpfActiveContentTests
{
    /// <summary>
    /// Gets the fixture.
    /// </summary>
    public WpfActiveContentFixture Fixture { get; private set; } = null!;

    /// <summary>
    /// One-time setup to create the WPF test fixture.
    /// </summary>
    [OneTimeSetUp]
    public void OneTimeSetUp() => Fixture = new WpfActiveContentFixture();

    /// <summary>
    /// One-time teardown to clean up the WPF test fixture.
    /// </summary>
    [OneTimeTearDown]
    public void OneTimeTearDown() => Fixture?.Dispose();

    /// <summary>
    /// Validates binding logic for a list-backed view.
    /// </summary>
    [Test]
    [Apartment(ApartmentState.STA)]
    public void BindListFunctionalTest()
    {
        var window = Fixture.App?.WpfTestWindowFactory();
        var view = new MockBindListView();
        window!.RootGrid.Children.Add(view);

        var loaded = new RoutedEventArgs { RoutedEvent = FrameworkElement.LoadedEvent };
        window.RaiseEvent(loaded);
        view.RaiseEvent(loaded);

        var test1 = new MockBindListItemViewModel("Test1");
        view.ViewModel!.ActiveListItem.Add(test1);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(
                        view.ItemList.Items,
                        Has.Count.EqualTo(1));
            Assert.That(
                        view.ViewModel.ActiveItem,
                        Is.EqualTo(test1));
        }

        var test2 = new MockBindListItemViewModel("Test2");
        view.ViewModel.ActiveListItem.Add(test2);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(
                        view.ItemList.Items,
                        Has.Count.EqualTo(2));
            Assert.That(
                        view.ViewModel.ActiveItem,
                        Is.EqualTo(test2));
        }

        var test3 = new MockBindListItemViewModel("Test3");
        view.ViewModel.ActiveListItem.Add(test3);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(
                        view.ItemList.Items,
                        Has.Count.EqualTo(3));
            Assert.That(
                        view.ViewModel.ActiveItem,
                        Is.EqualTo(test3));
        }

        view.ItemList.SelectedItem = view.ItemList.Items.GetItemAt(0);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(
                        view.ItemList.Items,
                        Has.Count.EqualTo(1));
            Assert.That(
                        view.ViewModel.ActiveItem,
                        Is.EqualTo(test1));
        }

        window.Close();
    }

    /// <summary>
    /// Ensures view resolution respects contracts and fallback behavior.
    /// </summary>
    [Test]
    [Apartment(ApartmentState.STA)]
    public void ViewModelHostViewTestFallback()
    {
        var resolver = new ModernDependencyResolver();
        resolver.InitializeSplat();
        resolver.InitializeReactiveUI();

        using (resolver.WithResolver())
        {
            ResolveViewBIfViewBIsRegistered(resolver);
            ResolveView0WithFallback(resolver);
            ResolveNoneWithFallbackBypass(resolver);
        }

        void ResolveViewBIfViewBIsRegistered(ModernDependencyResolver r)
        {
            r.Register(
                       () => new FakeViewWithContract.View0(),
                       typeof(IViewFor<FakeViewWithContract.MyViewModel>));
            r.Register(
                       () => new FakeViewWithContract.ViewA(),
                       typeof(IViewFor<FakeViewWithContract.MyViewModel>),
                       FakeViewWithContract.ContractA);
            r.Register(
                       () => new FakeViewWithContract.ViewB(),
                       typeof(IViewFor<FakeViewWithContract.MyViewModel>),
                       FakeViewWithContract.ContractB);

            var window = Fixture.App?.WpfTestWindowFactory();
            var vm = new FakeViewWithContract.MyViewModel();
            var host = new ViewModelViewHost { ViewModel = vm, ViewContract = FakeViewWithContract.ContractB, };

            window!.RootGrid.Children.Clear();
            window.RootGrid.Children.Add(host);

            var loaded = new RoutedEventArgs { RoutedEvent = FrameworkElement.LoadedEvent };
            window.RaiseEvent(loaded);
            host.RaiseEvent(loaded);

            Assert.That(host.Content, Is.Not.Null);
            Assert.That(host.Content, Is.InstanceOf<FakeViewWithContract.ViewB>());
            window.Close();
        }

        void ResolveView0WithFallback(ModernDependencyResolver r)
        {
            r.UnregisterCurrent(
                                typeof(IViewFor<FakeViewWithContract.MyViewModel>),
                                FakeViewWithContract.ContractB);

            var window = Fixture.App?.WpfTestWindowFactory();
            var vm = new FakeViewWithContract.MyViewModel();
            var host = new ViewModelViewHost
            {
                ViewModel = vm, ViewContract = FakeViewWithContract.ContractB, ContractFallbackByPass = false,
            };

            window!.RootGrid.Children.Clear();
            window.RootGrid.Children.Add(host);

            var loaded = new RoutedEventArgs { RoutedEvent = FrameworkElement.LoadedEvent };
            window.RaiseEvent(loaded);
            host.RaiseEvent(loaded);

            Assert.That(host.Content, Is.Not.Null);
            Assert.That(host.Content, Is.InstanceOf<FakeViewWithContract.View0>());
            window.Close();
        }

        void ResolveNoneWithFallbackBypass(ModernDependencyResolver r)
        {
            r.UnregisterCurrent(
                                typeof(IViewFor<FakeViewWithContract.MyViewModel>),
                                FakeViewWithContract.ContractB);

            var window = Fixture.App?.WpfTestWindowFactory();
            var vm = new FakeViewWithContract.MyViewModel();
            var host = new ViewModelViewHost
            {
                ViewModel = vm, ViewContract = FakeViewWithContract.ContractB, ContractFallbackByPass = true,
            };

            window!.RootGrid.Children.Clear();
            window.RootGrid.Children.Add(host);

            var loaded = new RoutedEventArgs { RoutedEvent = FrameworkElement.LoadedEvent };
            window.RaiseEvent(loaded);
            host.RaiseEvent(loaded);

            ClassicAssert.IsNull(host.Content);
            window.Close();
        }
    }

    /// <summary>
    /// Exercises TransitioningContentControl with all directions and transitions.
    /// </summary>
    [Test]
    [Apartment(ApartmentState.STA)]
    public void TransitioningContentControlTest()
    {
        var window = Fixture.App?.MockWindowFactory();
        window!.WhenActivated(async _ =>
        {
            window!.TransitioningContent.Duration = TimeSpan.FromMilliseconds(200);
            var transitioning = false;
            window.TransitioningContent.TransitionStarted += (_, _) => transitioning = true;
            window.TransitioningContent.TransitionCompleted += (_, _) => transitioning = false;

            await RunCycle(
                           TransitioningContentControl.TransitionDirection.Down,
                           TransitioningContentControl.TransitionType.Bounce).ConfigureAwait(true);
            await RunCycle(
                           TransitioningContentControl.TransitionDirection.Left,
                           TransitioningContentControl.TransitionType.Bounce).ConfigureAwait(true);
            await RunCycle(
                           TransitioningContentControl.TransitionDirection.Right,
                           TransitioningContentControl.TransitionType.Bounce).ConfigureAwait(true);
            await RunCycle(
                           TransitioningContentControl.TransitionDirection.Up,
                           TransitioningContentControl.TransitionType.Bounce).ConfigureAwait(true);
            await RunCycle(
                           TransitioningContentControl.TransitionDirection.Down,
                           TransitioningContentControl.TransitionType.Drop).ConfigureAwait(true);
            await RunCycle(
                           TransitioningContentControl.TransitionDirection.Left,
                           TransitioningContentControl.TransitionType.Drop).ConfigureAwait(true);
            await RunCycle(
                           TransitioningContentControl.TransitionDirection.Right,
                           TransitioningContentControl.TransitionType.Drop).ConfigureAwait(true);
            await RunCycle(
                           TransitioningContentControl.TransitionDirection.Up,
                           TransitioningContentControl.TransitionType.Drop).ConfigureAwait(true);
            await RunCycle(
                           TransitioningContentControl.TransitionDirection.Down,
                           TransitioningContentControl.TransitionType.Fade).ConfigureAwait(true);
            await RunCycle(
                           TransitioningContentControl.TransitionDirection.Left,
                           TransitioningContentControl.TransitionType.Fade).ConfigureAwait(true);
            await RunCycle(
                           TransitioningContentControl.TransitionDirection.Right,
                           TransitioningContentControl.TransitionType.Fade).ConfigureAwait(true);
            await RunCycle(
                           TransitioningContentControl.TransitionDirection.Up,
                           TransitioningContentControl.TransitionType.Fade).ConfigureAwait(true);
            await RunCycle(
                           TransitioningContentControl.TransitionDirection.Down,
                           TransitioningContentControl.TransitionType.Move).ConfigureAwait(true);
            await RunCycle(
                           TransitioningContentControl.TransitionDirection.Left,
                           TransitioningContentControl.TransitionType.Move).ConfigureAwait(true);
            await RunCycle(
                           TransitioningContentControl.TransitionDirection.Right,
                           TransitioningContentControl.TransitionType.Move).ConfigureAwait(true);
            await RunCycle(
                           TransitioningContentControl.TransitionDirection.Up,
                           TransitioningContentControl.TransitionType.Move).ConfigureAwait(true);
            await RunCycle(
                           TransitioningContentControl.TransitionDirection.Down,
                           TransitioningContentControl.TransitionType.Slide).ConfigureAwait(true);
            await RunCycle(
                           TransitioningContentControl.TransitionDirection.Left,
                           TransitioningContentControl.TransitionType.Slide).ConfigureAwait(true);
            await RunCycle(
                           TransitioningContentControl.TransitionDirection.Right,
                           TransitioningContentControl.TransitionType.Slide).ConfigureAwait(true);
            await RunCycle(
                           TransitioningContentControl.TransitionDirection.Up,
                           TransitioningContentControl.TransitionType.Slide).ConfigureAwait(true);

            async Task RunOnceAsync()
            {
                var v1 = new View1();
                window.TransitioningContent.Content = v1;
                Assert.That(
                            transitioning,
                            Is.True);
                while (transitioning)
                {
                    await Task.Delay(5).ConfigureAwait(true);
                }

                Assert.That(
                            window.TransitioningContent.Content,
                            Is.EqualTo(v1));
                ClassicAssert.IsFalse(transitioning);

                var v2 = new View2();
                window.TransitioningContent.Content = v2;
                Assert.That(
                            transitioning,
                            Is.True);
                while (transitioning)
                {
                    await Task.Delay(5).ConfigureAwait(true);
                }

                Assert.That(
                            window.TransitioningContent.Content,
                            Is.EqualTo(v2));
                ClassicAssert.IsFalse(transitioning);
            }

            async Task RunCycle(
                TransitioningContentControl.TransitionDirection dir,
                TransitioningContentControl.TransitionType type)
            {
                window.TransitioningContent.Direction = dir;
                window.TransitioningContent.Transition = type;
                using (Assert.EnterMultipleScope())
                {
                    Assert.That(
                                window.TransitioningContent.Direction,
                                Is.EqualTo(dir));
                    Assert.That(
                                window.TransitioningContent.Transition,
                                Is.EqualTo(type));
                }

                await RunOnceAsync().ConfigureAwait(true);
                await RunOnceAsync().ConfigureAwait(true);
            }

            window.Close();
        });

        window!.ShowDialog();
    }

    /// <summary>
    /// Verifies the dummy suspension driver calls.
    /// </summary>
    [Test]
    public void DummySuspensionDriverTest()
    {
        var dsd = new DummySuspensionDriver();
        dsd.LoadState().Select(_ => 1).Subscribe(v => Assert.That(
                                                                  v,
                                                                  Is.EqualTo(1)));
        dsd.SaveState("Save Me").Select(_ => 2).Subscribe(v => Assert.That(
                                                                           v,
                                                                           Is.EqualTo(2)));
        dsd.InvalidateState().Select(_ => 3).Subscribe(v => Assert.That(
                                                                        v,
                                                                        Is.EqualTo(3)));
    }

    /// <summary>
    /// DPI override scenarios for TransitioningContentControl.
    /// </summary>
    [Test]
    [Apartment(ApartmentState.STA)]
    public void TransitioninContentControlDpiTest()
    {
        var window = Fixture.App?.TCMockWindowFactory();
        const int delay = 2000;

        window!.WhenActivated(async _ =>
        {
            TransitioningContentControl.OverrideDpi = true;
            window!.TransitioningContent.Height = 500;
            window.TransitioningContent.Width = 500;
            window.TransitioningContent.Content = new FirstView();
            await Task.Delay(delay).ConfigureAwait(true);
            window.TransitioningContent.Content = new SecondView();
            await Task.Delay(delay).ConfigureAwait(true);

            window.TransitioningContent.Height = 300;
            window.TransitioningContent.Width = 300;
            window.TransitioningContent.Content = new FirstView();
            await Task.Delay(delay).ConfigureAwait(true);
            window.TransitioningContent.Content = new SecondView();

            window.TransitioningContent.Height = 0.25;
            window.TransitioningContent.Width = 0.25;
            window.TransitioningContent.Content = new FirstView();
            await Task.Delay(delay).ConfigureAwait(true);
            window.TransitioningContent.Content = new SecondView();

            window.TransitioningContent.Height = 500;
            window.TransitioningContent.Width = 500;
            window.TransitioningContent.Content = new FirstView();
            await Task.Delay(delay).ConfigureAwait(true);
            window.TransitioningContent.Content = new SecondView();
            await Task.Delay(delay).ConfigureAwait(true);

            window.TransitioningContent.Height = 300;
            window.TransitioningContent.Width = 300;
            window.TransitioningContent.Content = new FirstView();
            await Task.Delay(delay).ConfigureAwait(true);
            window.TransitioningContent.Content = new SecondView();

            window.TransitioningContent.Height = 0.25;
            window.TransitioningContent.Width = 0.25;
            window.TransitioningContent.Content = new FirstView();
            await Task.Delay(delay).ConfigureAwait(true);
            window.TransitioningContent.Content = new SecondView();

            window.Close();
        });

        window!.ShowDialog();
    }

    /// <summary>
    /// Ensures ReactiveCommand IsExecuting/CanExecute behave correctly on task thread.
    /// </summary>
    [Test]
    [Apartment(ApartmentState.STA)]
    public void ReactiveCommandRunningOnTaskThreadAllowsCanExecuteAndExecutingToFire()
    {
        LiveModeDetector.UseRuntimeThreads();
        var window = Fixture.App?.MockWindowFactory();

        window!.WhenActivated(async d =>
        {
            try
            {
                using var testSequencer = new TestSequencer();
                window!.TransitioningContent.VerticalContentAlignment = VerticalAlignment.Stretch;
                window.TransitioningContent.HorizontalContentAlignment = HorizontalAlignment.Stretch;

                var view = new CanExecuteExecutingView();
                window.TransitioningContent.Content = view;
                await Task.Delay(2000).ConfigureAwait(true);

                var sawExecuting = false;
                view!.ViewModel!.Command3.IsExecuting
                     .ObserveOn(RxApp.MainThreadScheduler)
                     .Subscribe(b =>
                     {
                         if (b)
                         {
                             sawExecuting = true;
                         }
                     })
                     .DisposeWith(d);

                int? result = null;
                view.ViewModel.Command3.Subscribe(async r =>
                {
                    result = r;
                    await testSequencer.AdvancePhaseAsync();
                });

                await view.ViewModel.Command3.Execute();
                await testSequencer.AdvancePhaseAsync();

                using (Assert.EnterMultipleScope())
                {
                    Assert.That(
                                result,
                                Is.EqualTo(100));
                    Assert.That(
                                sawExecuting,
                                Is.True);
                }
            }
            finally
            {
                window?.Close();
                LiveModeDetector.UseDefaultModeDetector();
            }
        });

        window!.ShowDialog();
    }
}
