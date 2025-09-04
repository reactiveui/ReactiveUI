// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows;
using DynamicData;
using ReactiveUI.Testing;

namespace ReactiveUI.Tests.Wpf;

/// <summary>
/// Wpf Active Content Tests.
/// NOTE: Only one Test can create an AppDomain, all Active content tests must go in this class.
/// Add to WpfActiveContentApp to add any additional mock windows.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="WpfActiveContentTests"/> class.
/// </remarks>
/// <param name="fixture">The fixture.</param>
[TestFixture]
public class WpfActiveContentTests(WpfActiveContentFixture fixture) : IClassFixture<WpfActiveContentFixture>
{
    /// <summary>
    /// Gets the fixture.
    /// </summary>
    /// <value>
    /// The fixture.
    /// </value>
    public WpfActiveContentFixture Fixture { get; } = fixture;

    [StaFact]
    public void BindListFunctionalTest()
    {
        var window = Fixture?.App?.WpfTestWindowFactory();
        var view = new MockBindListView();
        window!.RootGrid.Children.Add(view);

        var loaded = new RoutedEventArgs
        {
            RoutedEvent = FrameworkElement.LoadedEvent
        };

        window.RaiseEvent(loaded);
        view.RaiseEvent(loaded);
        var test1 = new MockBindListItemViewModel("Test1");
        view.ViewModel?.ActiveListItem.Add(test1);
        Assert.That(view.ItemList.Items.Count, Is.EqualTo(1));
        Assert.That(view.ViewModel!.ActiveItem, Is.EqualTo(test1));

        var test2 = new MockBindListItemViewModel("Test2");
        view.ViewModel?.ActiveListItem.Add(test2);
        Assert.That(view.ItemList.Items.Count, Is.EqualTo(2));
        Assert.That(view.ViewModel!.ActiveItem, Is.EqualTo(test2));

        var test3 = new MockBindListItemViewModel("Test3");
        view.ViewModel?.ActiveListItem.Add(test3);
        Assert.That(view.ItemList.Items.Count, Is.EqualTo(3));
        Assert.That(view.ViewModel!.ActiveItem, Is.EqualTo(test3));

        view.ItemList.SelectedItem = view.ItemList.Items.GetItemAt(0);
        Assert.That(view.ItemList.Items.Count, Is.EqualTo(1));
        Assert.That(view.ViewModel!.ActiveItem, Is.EqualTo(test1));

        window.Close();
    }

    [StaFact]
    public void ViewModelHostViewTestFallback()
    {
        var oldLocator = Locator.GetLocator();

        var resolver = new ModernDependencyResolver();
        resolver.InitializeSplat();
        resolver.InitializeReactiveUI();

        // test the resolving behavior
        using (resolver.WithResolver())
        {
            ResolveViewBIfViewBIsRegistered(resolver);
            ResolveView0WithFallbck(resolver);
            ResolveNoneWithFallbckByPass(resolver);
        }

        void ResolveViewBIfViewBIsRegistered(ModernDependencyResolver resolver)
        {
            resolver.Register(() => new FakeViewWithContract.View0(), typeof(IViewFor<FakeViewWithContract.MyViewModel>));
            resolver.Register(() => new FakeViewWithContract.ViewA(), typeof(IViewFor<FakeViewWithContract.MyViewModel>), FakeViewWithContract.ContractA);
            resolver.Register(() => new FakeViewWithContract.ViewB(), typeof(IViewFor<FakeViewWithContract.MyViewModel>), FakeViewWithContract.ContractB);

            var window = Fixture?.App?.WpfTestWindowFactory();

            var viewmodel = new FakeViewWithContract.MyViewModel();
            var vmvhost = new ViewModelViewHost()
            {
                ViewModel = viewmodel,
                ViewContract = FakeViewWithContract.ContractB,
            };
            window!.RootGrid.Children.Clear();
            window!.RootGrid.Children.Add(vmvhost);

            var loaded = new RoutedEventArgs
            {
                RoutedEvent = FrameworkElement.LoadedEvent
            };
            window.RaiseEvent(loaded);
            vmvhost.RaiseEvent(loaded);

            Assert.That(vmvhost.Content, Is.Not.Null);
            Assert.IsType(typeof(FakeViewWithContract.ViewB), vmvhost.Content);
            window.Close();
        }

        void ResolveView0WithFallbck(ModernDependencyResolver resolver)
        {
            resolver.UnregisterCurrent(typeof(IViewFor<FakeViewWithContract.MyViewModel>), FakeViewWithContract.ContractB);

            var window = Fixture?.App?.WpfTestWindowFactory();

            var viewmodel = new FakeViewWithContract.MyViewModel();
            var vmvhost = new ViewModelViewHost()
            {
                ViewModel = viewmodel,
                ViewContract = FakeViewWithContract.ContractB,
                ContractFallbackByPass = false,
            };
            window!.RootGrid.Children.Clear();
            window!.RootGrid.Children.Add(vmvhost);

            var loaded = new RoutedEventArgs
            {
                RoutedEvent = FrameworkElement.LoadedEvent
            };
            window.RaiseEvent(loaded);
            vmvhost.RaiseEvent(loaded);

            Assert.That(vmvhost.Content, Is.Not.Null);
            Assert.IsType(typeof(FakeViewWithContract.View0), vmvhost.Content);
            window.Close();
        }

        void ResolveNoneWithFallbckByPass(ModernDependencyResolver resolver)
        {
            resolver.UnregisterCurrent(typeof(IViewFor<FakeViewWithContract.MyViewModel>), FakeViewWithContract.ContractB);

            var window = Fixture?.App?.WpfTestWindowFactory();

            var viewmodel = new FakeViewWithContract.MyViewModel();
            var vmvhost = new ViewModelViewHost()
            {
                ViewModel = viewmodel,
                ViewContract = FakeViewWithContract.ContractB,
                ContractFallbackByPass = true,
            };
            window!.RootGrid.Children.Clear();
            window!.RootGrid.Children.Add(vmvhost);

            var loaded = new RoutedEventArgs
            {
                RoutedEvent = FrameworkElement.LoadedEvent
            };
            window.RaiseEvent(loaded);
            vmvhost.RaiseEvent(loaded);

            Assert.That(vmvhost.Content, Is.Null);
            window.Close();
        }
    }

    [StaFact]
    public void TransitioningContentControlTest()
    {
        var window = Fixture?.App?.MockWindowFactory();
        window!.WhenActivated(async _ =>
        {
            window!.TransitioningContent.Duration = TimeSpan.FromMilliseconds(200);
            var transitioning = false;
            window.TransitioningContent.TransitionStarted += (s, e) => transitioning = true;

            window.TransitioningContent.TransitionCompleted += (s, e) => transitioning = false;

            await TestCyle(TransitioningContentControl.TransitionDirection.Down, TransitioningContentControl.TransitionType.Bounce).ConfigureAwait(true);
            await TestCyle(TransitioningContentControl.TransitionDirection.Left, TransitioningContentControl.TransitionType.Bounce).ConfigureAwait(true);
            await TestCyle(TransitioningContentControl.TransitionDirection.Right, TransitioningContentControl.TransitionType.Bounce).ConfigureAwait(true);
            await TestCyle(TransitioningContentControl.TransitionDirection.Up, TransitioningContentControl.TransitionType.Bounce).ConfigureAwait(true);
            await TestCyle(TransitioningContentControl.TransitionDirection.Down, TransitioningContentControl.TransitionType.Drop).ConfigureAwait(true);
            await TestCyle(TransitioningContentControl.TransitionDirection.Left, TransitioningContentControl.TransitionType.Drop).ConfigureAwait(true);
            await TestCyle(TransitioningContentControl.TransitionDirection.Right, TransitioningContentControl.TransitionType.Drop).ConfigureAwait(true);
            await TestCyle(TransitioningContentControl.TransitionDirection.Up, TransitioningContentControl.TransitionType.Drop).ConfigureAwait(true);
            await TestCyle(TransitioningContentControl.TransitionDirection.Down, TransitioningContentControl.TransitionType.Fade).ConfigureAwait(true);
            await TestCyle(TransitioningContentControl.TransitionDirection.Left, TransitioningContentControl.TransitionType.Fade).ConfigureAwait(true);
            await TestCyle(TransitioningContentControl.TransitionDirection.Right, TransitioningContentControl.TransitionType.Fade).ConfigureAwait(true);
            await TestCyle(TransitioningContentControl.TransitionDirection.Up, TransitioningContentControl.TransitionType.Fade).ConfigureAwait(true);
            await TestCyle(TransitioningContentControl.TransitionDirection.Down, TransitioningContentControl.TransitionType.Move).ConfigureAwait(true);
            await TestCyle(TransitioningContentControl.TransitionDirection.Left, TransitioningContentControl.TransitionType.Move).ConfigureAwait(true);
            await TestCyle(TransitioningContentControl.TransitionDirection.Right, TransitioningContentControl.TransitionType.Move).ConfigureAwait(true);
            await TestCyle(TransitioningContentControl.TransitionDirection.Up, TransitioningContentControl.TransitionType.Move).ConfigureAwait(true);
            await TestCyle(TransitioningContentControl.TransitionDirection.Down, TransitioningContentControl.TransitionType.Slide).ConfigureAwait(true);
            await TestCyle(TransitioningContentControl.TransitionDirection.Left, TransitioningContentControl.TransitionType.Slide).ConfigureAwait(true);
            await TestCyle(TransitioningContentControl.TransitionDirection.Right, TransitioningContentControl.TransitionType.Slide).ConfigureAwait(true);
            await TestCyle(TransitioningContentControl.TransitionDirection.Up, TransitioningContentControl.TransitionType.Slide).ConfigureAwait(true);

            async Task TestTransiton()
            {
                var view = new View1();
                window.TransitioningContent.Content = view;
                Assert.That(transitioning, Is.True);
                while (transitioning)
                {
                    await Task.Delay(5).ConfigureAwait(true);
                }

                Assert.That(view, Is.EqualTo(window.TransitioningContent.Content));
                Assert.That(transitioning, Is.False);

                var view2 = new View2();
                window.TransitioningContent.Content = view2;
                Assert.That(transitioning, Is.True);
                while (transitioning)
                {
                    await Task.Delay(5).ConfigureAwait(true);
                }

                Assert.That(view2, Is.EqualTo(window.TransitioningContent.Content));
                Assert.That(transitioning, Is.False);
            }

            async Task TestCyle(TransitioningContentControl.TransitionDirection direction, TransitioningContentControl.TransitionType transition)
            {
                window.TransitioningContent.Direction = direction;
                window.TransitioningContent.Transition = transition;
                Assert.That(direction, Is.EqualTo(window.TransitioningContent.Direction));
                Assert.That(transition, Is.EqualTo(window.TransitioningContent.Transition));
                await TestTransiton().ConfigureAwait(true);
                await TestTransiton().ConfigureAwait(true);
            }

            window.Close();
        });
        window!.ShowDialog();
    }

    [Test]
    public void DummySuspensionDriverTest()
    {
        var dsd = new DummySuspensionDriver();
        dsd.LoadState().Select(_ => 1).Subscribe(_ => Assert.That(_, Is.EqualTo(1)));
        dsd.SaveState("Save Me").Select(_ => 2).Subscribe(_ => Assert.That(_, Is.EqualTo(2)));
        dsd.InvalidateState().Select(_ => 3).Subscribe(_ => Assert.That(_, Is.EqualTo(3)));
    }

    [StaFact]
    public void TransitioninContentControlDpiTest()
    {
        var window = Fixture?.App?.TCMockWindowFactory();
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

    [StaFact]
    public void ReactiveCommandRunningOnTaskThreadAllowsCanExecuteAndExecutingToFire()
    {
        LiveModeDetector.UseRuntimeThreads();
        var window = Fixture?.App?.MockWindowFactory();
        window!.WhenActivated(async d =>
        {
            try
            {
                using var testSequencer = new TestSequencer();
                window!.TransitioningContent.VerticalContentAlignment = VerticalAlignment.Stretch;
                window!.TransitioningContent.HorizontalContentAlignment = HorizontalAlignment.Stretch;
                var view = new CanExecuteExecutingView();
                window!.TransitioningContent.Content = view;
                await Task.Delay(2000).ConfigureAwait(true);

                var isExecutingExecuted = false;
                view!.ViewModel!.Command3.IsExecuting
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(value =>
                {
                    if (value)
                    {
                        isExecutingExecuted = true;
                    }
                }).DisposeWith(d);

                int? result = null;
                view!.ViewModel!.Command3.Subscribe(async r =>
                {
                    result = r;
                    await testSequencer.AdvancePhaseAsync();
                });
                await view!.ViewModel!.Command3.Execute();
                await testSequencer.AdvancePhaseAsync();
                Assert.That(result, Is.EqualTo(100));
                Assert.That(isExecutingExecuted, Is.True);
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
