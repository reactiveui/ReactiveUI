// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows;
using DynamicData;
using Splat;
using Xunit;

namespace ReactiveUI.Tests.Wpf
{
    /// <summary>
    /// Wpf Active Content Tests.
    /// NOTE: Only one Test can create an AppDomain, all Active content tests must go in this class.
    /// Add to WpfActiveContentApp to add any additional mock windows.
    /// </summary>
    public class WpfActiveContentTests : IClassFixture<WpfActiveContentFixture>
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private static bool isExecutingExecuted = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="WpfActiveContentTests"/> class.
        /// </summary>
        /// <param name="fixture">The fixture.</param>
        public WpfActiveContentTests(WpfActiveContentFixture fixture) => Fixture = fixture;

        /// <summary>
        /// Gets the fixture.
        /// </summary>
        /// <value>
        /// The fixture.
        /// </value>
        public WpfActiveContentFixture Fixture { get; }

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
            Assert.Equal(1, view.ItemList.Items.Count);
            Assert.Equal(test1, view.ViewModel!.ActiveItem);

            var test2 = new MockBindListItemViewModel("Test2");
            view.ViewModel?.ActiveListItem.Add(test2);
            Assert.Equal(2, view.ItemList.Items.Count);
            Assert.Equal(test2, view.ViewModel!.ActiveItem);

            var test3 = new MockBindListItemViewModel("Test3");
            view.ViewModel?.ActiveListItem.Add(test3);
            Assert.Equal(3, view.ItemList.Items.Count);
            Assert.Equal(test3, view.ViewModel!.ActiveItem);

            view.ItemList.SelectedItem = view.ItemList.Items.GetItemAt(0);
            Assert.Equal(1, view.ItemList.Items.Count);
            Assert.Equal(test1, view.ViewModel!.ActiveItem);

            window.Close();
        }

        [StaFact]
        public void TransitioningContentControlTest()
        {
            var window = Fixture?.App?.MockWindowFactory();
            window!.WhenActivated(async d =>
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
                    Assert.True(transitioning);
                    while (transitioning)
                    {
                        await Task.Delay(5).ConfigureAwait(true);
                    }

                    Assert.Equal(window.TransitioningContent.Content, view);
                    Assert.False(transitioning);

                    var view2 = new View2();
                    window.TransitioningContent.Content = view2;
                    Assert.True(transitioning);
                    while (transitioning)
                    {
                        await Task.Delay(5).ConfigureAwait(true);
                    }

                    Assert.Equal(window.TransitioningContent.Content, view2);
                    Assert.False(transitioning);
                }

                async Task TestCyle(TransitioningContentControl.TransitionDirection direction, TransitioningContentControl.TransitionType transition)
                {
                    window.TransitioningContent.Direction = direction;
                    window.TransitioningContent.Transition = transition;
                    Assert.Equal(window.TransitioningContent.Direction, direction);
                    Assert.Equal(window.TransitioningContent.Transition, transition);
                    await TestTransiton().ConfigureAwait(true);
                    await TestTransiton().ConfigureAwait(true);
                }

                window.Close();
            });
            window!.ShowDialog();
        }

        [Fact]
        public void DummySuspensionDriverTest()
        {
            var dsd = new DummySuspensionDriver();
            dsd.LoadState().Select(_ => 1).Subscribe(_ => Assert.Equal(1, _));
            dsd.SaveState("Save Me").Select(_ => 2).Subscribe(_ => Assert.Equal(2, _));
            dsd.InvalidateState().Select(_ => 3).Subscribe(_ => Assert.Equal(3, _));
        }

        [StaFact]
        public void TransitioninContentControlDpiTest()
        {
            var window = Fixture?.App?.TCMockWindowFactory();

            window!.WhenActivated(async d =>
            {
                TransitioningContentControl.OverrideDpi = true;
                window!.TransitioningContent.Height = 500;
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
                window.Close();
            });
            window!.ShowDialog();
        }

        [StaFact]
        public void ReactiveCommandRunningOnTaskThreadAllowsCanExecuteAndExecutingToFire()
        {
            var modeDetector = new LiveTestModeDetector();
            ModeDetector.OverrideModeDetector(modeDetector);
            var window = Fixture?.App?.MockWindowFactory();
            window!.WhenActivated(async d =>
            {
                var view = new CanExecuteExecutingView();
                view!.ViewModel = new();
                isExecutingExecuted = false;
                window!.TransitioningContent.VerticalContentAlignment = VerticalAlignment.Stretch;
                window!.TransitioningContent.HorizontalContentAlignment = HorizontalAlignment.Stretch;
                window!.TransitioningContent.Content = view;
                await Task.Delay(5000).ConfigureAwait(true);
                int? result = null;
                view!.ViewModel!.Command3.IsExecuting
                .Subscribe(static value =>
                {
                    if (value)
                    {
                        isExecutingExecuted = true;
                    }
                });
                view!.ViewModel!.Command3.Subscribe(r => result = r);
                await view!.ViewModel!.Command3.Execute();
                await Task.Delay(5000).ConfigureAwait(true);
                Assert.True(isExecutingExecuted);

                Assert.Equal(100, result);

                window.Close();
            });
            window!.ShowDialog();
        }

#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    }
}
