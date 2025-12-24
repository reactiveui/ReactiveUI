// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive.Disposables.Fluent;
using System.Windows;
using DynamicData;
using ReactiveUI.Testing;

using TUnit.Core.Executors;

namespace ReactiveUI.Tests.Wpf;

/// <summary>
/// Wpf Active Content Tests.
/// NOTE: Only one Test can create an AppDomain, all Active content tests must go in this class.
/// Add to WpfActiveContentApp to add any additional mock windows.
/// </summary>
[NotInParallel]
public class WpfActiveContentTests
{
    /// <summary>
    /// Gets the fixture.
    /// </summary>
    public static WpfActiveContentFixture Fixture { get; private set; } = null!;

    /// <summary>
    /// One-time setup to create the WPF test fixture.
    /// </summary>
    [Before(HookType.Class)]
    public static void OneTimeSetUp() => Fixture = new WpfActiveContentFixture();

    /// <summary>
    /// One-time teardown to clean up the WPF test fixture.
    /// </summary>
    [After(HookType.Class)]
    public static void OneTimeTearDown() => Fixture?.Dispose();

    /// <summary>
    /// Validates binding logic for a list-backed view.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<STAThreadExecutor>]
    public async Task BindListFunctionalTest()
    {
        var view = new MockBindListView();
        var vm = view.ViewModel!;

        // Activate the view to trigger bindings
        view.RaiseEvent(new RoutedEventArgs(FrameworkElement.LoadedEvent));

        // Test 1: Add first item
        var test1 = new MockBindListItemViewModel("Test1");
        vm.ActiveListItem.Add(test1);
        using (Assert.Multiple())
        {
            await Assert.That(vm.ListItems.Count).IsEqualTo(1);
            await Assert.That(vm.ActiveItem).IsEqualTo(test1);
            await Assert.That(view.ItemList.Items.Count).IsEqualTo(1);
        }

        // Test 2: Add second item
        var test2 = new MockBindListItemViewModel("Test2");
        vm.ActiveListItem.Add(test2);
        using (Assert.Multiple())
        {
            await Assert.That(vm.ListItems.Count).IsEqualTo(2);
            await Assert.That(vm.ActiveItem).IsEqualTo(test2);
            await Assert.That(view.ItemList.Items.Count).IsEqualTo(2);
        }

        // Test 3: Add third item
        var test3 = new MockBindListItemViewModel("Test3");
        vm.ActiveListItem.Add(test3);
        using (Assert.Multiple())
        {
            await Assert.That(vm.ListItems.Count).IsEqualTo(3);
            await Assert.That(vm.ActiveItem).IsEqualTo(test3);
            await Assert.That(view.ItemList.Items.Count).IsEqualTo(3);
        }

        // Test 4: Select first item (should trigger command that removes items after it)
        await vm.SelectItem.Execute(test1);
        using (Assert.Multiple())
        {
            await Assert.That(vm.ListItems.Count).IsEqualTo(1);
            await Assert.That(vm.ActiveItem).IsEqualTo(test1);
            await Assert.That(view.ItemList.Items.Count).IsEqualTo(1);
        }
    }

    /// <summary>
    /// Ensures view resolution respects contracts and fallback behavior.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<STAThreadExecutor>]
    public async Task ViewModelHostViewTestFallback()
    {
        var resolver = new ModernDependencyResolver();
        resolver.InitializeSplat();
        resolver.InitializeReactiveUI();

        using (resolver.WithResolver())
        {
            await ResolveViewBIfViewBIsRegistered(resolver);
            await ResolveView0WithFallback(resolver);
            await ResolveNoneWithFallbackBypass(resolver);
        }

        async Task ResolveViewBIfViewBIsRegistered(ModernDependencyResolver r)
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

            await Assert.That(host.Content).IsNotNull();
            await Assert.That(host.Content).IsAssignableTo<FakeViewWithContract.ViewB>();
            window.Close();
        }

        async Task ResolveView0WithFallback(ModernDependencyResolver r)
        {
            r.UnregisterCurrent(
                                typeof(IViewFor<FakeViewWithContract.MyViewModel>),
                                FakeViewWithContract.ContractB);

            var window = Fixture.App?.WpfTestWindowFactory();
            var vm = new FakeViewWithContract.MyViewModel();
            var host = new ViewModelViewHost
            {
                ViewModel = vm,
                ViewContract = FakeViewWithContract.ContractB,
                ContractFallbackByPass = false,
            };

            window!.RootGrid.Children.Clear();
            window.RootGrid.Children.Add(host);

            var loaded = new RoutedEventArgs { RoutedEvent = FrameworkElement.LoadedEvent };
            window.RaiseEvent(loaded);
            host.RaiseEvent(loaded);

            await Assert.That(host.Content).IsNotNull();
            await Assert.That(host.Content).IsAssignableTo<FakeViewWithContract.View0>();
            window.Close();
        }

        async Task ResolveNoneWithFallbackBypass(ModernDependencyResolver r)
        {
            r.UnregisterCurrent(
                                typeof(IViewFor<FakeViewWithContract.MyViewModel>),
                                FakeViewWithContract.ContractB);

            var window = Fixture.App?.WpfTestWindowFactory();
            var vm = new FakeViewWithContract.MyViewModel();
            var host = new ViewModelViewHost
            {
                ViewModel = vm,
                ViewContract = FakeViewWithContract.ContractB,
                ContractFallbackByPass = true,
            };

            window!.RootGrid.Children.Clear();
            window.RootGrid.Children.Add(host);

            var loaded = new RoutedEventArgs { RoutedEvent = FrameworkElement.LoadedEvent };
            window.RaiseEvent(loaded);
            host.RaiseEvent(loaded);

            await Assert.That(host.Content).IsNull();
            window.Close();
        }
    }

    /// <summary>
    /// Exercises TransitioningContentControl with all directions and transitions.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<STAThreadExecutor>]
    [Skip("Flaky test - needs investigation")]
    public async Task TransitioningContentControlTest()
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
                await Assert.That(transitioning).IsTrue();
                while (transitioning)
                {
                    await Task.Delay(5).ConfigureAwait(true);
                }

                using (Assert.Multiple())
                {
                    await Assert.That(window.TransitioningContent.Content).IsEqualTo(v1);
                    await Assert.That(transitioning).IsFalse();
                }

                var v2 = new View2();
                window.TransitioningContent.Content = v2;
                await Assert.That(transitioning).IsTrue();
                while (transitioning)
                {
                    await Task.Delay(5).ConfigureAwait(true);
                }

                using (Assert.Multiple())
                {
                    await Assert.That(window.TransitioningContent.Content).IsEqualTo(v2);
                    await Assert.That(transitioning).IsFalse();
                }
            }

            async Task RunCycle(
                TransitioningContentControl.TransitionDirection dir,
                TransitioningContentControl.TransitionType type)
            {
                window.TransitioningContent.Direction = dir;
                window.TransitioningContent.Transition = type;
                using (Assert.Multiple())
                {
                    await Assert.That(window.TransitioningContent.Direction).IsEqualTo(dir);
                    await Assert.That(window.TransitioningContent.Transition).IsEqualTo(type);
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
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task DummySuspensionDriverTest()
    {
        var dsd = new DummySuspensionDriver();
        dsd.LoadState().Select(static _ => 1).Subscribe(async static v => await Assert.That(v).IsEqualTo(1));
        dsd.SaveState("Save Me").Select(static _ => 2).Subscribe(async static v => await Assert.That(v).IsEqualTo(2));
        dsd.InvalidateState().Select(static _ => 3).Subscribe(async static v => await Assert.That(v).IsEqualTo(3));
    }

    /// <summary>
    /// DPI override scenarios for TransitioningContentControl.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<STAThreadExecutor>]
    [Skip("Flaky test - needs investigation")]
    public async Task TransitioninContentControlDpiTest()
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
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<STAThreadExecutor>]
    public async Task ReactiveCommandRunningOnTaskThreadAllowsCanExecuteAndExecutingToFire()
    {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
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
                    testSequencer.AdvancePhaseAsync();
                });

                view.ViewModel.Command3.Execute();
                testSequencer.AdvancePhaseAsync();

                using (Assert.Multiple())
                {
                    await Assert.That(result).IsEqualTo(100);
                    await Assert.That(sawExecuting).IsTrue();
                }
            }
            finally
            {
                window?.Close();
                LiveModeDetector.UseDefaultModeDetector();
            }
        });

        window!.ShowDialog();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
    }
}
