// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Reactive.Disposables;
using Xunit;

namespace ReactiveUI.Tests
{
    public class RoutableViewModelMixinTests
    {
        [Fact]
        public void WhenNavigatedToCallsOnNavigatedToWhenViewModelIsFirstAdded()
        {
            var count = 0;

            var screen = new TestScreen();
            var vm = new RoutableViewModel(screen);

            vm.WhenNavigatedTo(() =>
            {
                count++;

                return Disposable.Empty;
            });

            screen.Router.Navigate.Execute(vm);

            Assert.Equal(1, count);
        }

        [Fact]
        public void WhenNavigatedToCallsOnNavigatedToWhenViewModelReturnsToTopOfStack()
        {
            var count = 0;

            var screen = new TestScreen();
            var vm = new RoutableViewModel(screen);
            var vm2 = new RoutableViewModel(screen);

            vm.WhenNavigatedTo(() =>
            {
                count++;

                return Disposable.Empty;
            });

            screen.Router.Navigate.Execute(vm);
            screen.Router.Navigate.Execute(vm2);
            screen.Router.Navigate.Execute(vm);

            Assert.Equal(2, count);
        }

        [Fact]
        public void WhenNavigatedToCallsDisposeWhenViewModelLosesFocus()
        {
            var count = 0;

            var screen = new TestScreen();
            var vm = new RoutableViewModel(screen);
            var vm2 = new RoutableViewModel(screen);

            vm.WhenNavigatedTo(() =>
            {
                return Disposable.Create(() => count++);
            });

            screen.Router.Navigate.Execute(vm);

            Assert.Equal(0, count);

            screen.Router.Navigate.Execute(vm2);

            Assert.Equal(1, count);
        }

        [Fact]
        public void WhenNavigatedToCallsDisposeWhenNavigationStackIsReset()
        {
            var count = 0;

            var screen = new TestScreen();
            var vm1 = new RoutableViewModel(screen);
            var vm2 = new RoutableViewModel(screen);

            vm1.WhenNavigatedTo(() =>
            {
                return Disposable.Create(() => count++);
            });

            screen.Router.Navigate.Execute(vm1);

            Assert.Equal(0, count);

            screen.Router.NavigateAndReset.Execute(vm2);

            Assert.Equal(1, count);
        }

        [Fact]
        public void WhenNavigatedToObservableFiresWhenViewModelAddedToNavigationStack()
        {
            var count = 0;

            var screen = new TestScreen();
            var vm = new RoutableViewModel(screen);

            vm.WhenNavigatedToObservable().Subscribe(_ =>
            {
                count++;
            });

            screen.Router.Navigate.Execute(vm);

            Assert.Equal(1, count);
        }

        [Fact]
        public void WhenNavigatedToObservableFiresWhenViewModelReturnsToNavigationStack()
        {
            var count = 0;

            var screen = new TestScreen();
            var vm = new RoutableViewModel(screen);
            var vm2 = new RoutableViewModel(screen);

            vm.WhenNavigatedToObservable().Subscribe(_ =>
            {
                count++;
            });

            screen.Router.Navigate.Execute(vm);
            screen.Router.Navigate.Execute(vm2);
            screen.Router.Navigate.Execute(vm);

            Assert.Equal(2, count);
        }

        [Fact]
        public void WhenNavigatedToObservableCompletesWhenViewModelIsRemovedFromNavigationStack()
        {
            var count = 0;

            var screen = new TestScreen();
            var vm = new RoutableViewModel(screen);

            vm.WhenNavigatedToObservable().Subscribe(
                _ => { },
                () => { count++; });

            screen.Router.Navigate.Execute(vm);
            screen.Router.NavigateBack.Execute();

            Assert.Equal(1, count);
        }

        [Fact]
        public void WhenNavigatedToObservableCompletesWhenNavigationStackIsReset()
        {
            var count = 0;

            var screen = new TestScreen();
            var vm1 = new RoutableViewModel(screen);
            var vm2 = new RoutableViewModel(screen);

            vm1.WhenNavigatedToObservable().Subscribe(
                _ => { },
                () => { count++; });

            screen.Router.Navigate.Execute(vm1);
            screen.Router.NavigateAndReset.Execute(vm2);

            Assert.Equal(1, count);
        }

        [Fact]
        public void WhenNavigatingFromObservableFiresWhenViewModelLosesFocus()
        {
            var count = 0;
            var screen = new TestScreen();
            var vm = new RoutableViewModel(screen);
            var vm2 = new RoutableViewModel(screen);

            vm.WhenNavigatingFromObservable().Subscribe(_ =>
            {
                count++;
            });

            screen.Router.Navigate.Execute(vm);
            screen.Router.Navigate.Execute(vm2);

            Assert.Equal(1, count);
        }

        [Fact]
        public void WhenNavigatingFromObservableCompletesWhenViewModelIsRemovedFromNavigationStack()
        {
            var count = 0;

            var screen = new TestScreen();
            var vm = new RoutableViewModel(screen);

            vm.WhenNavigatingFromObservable().Subscribe(
                _ => { },
                () => { count++; });

            screen.Router.Navigate.Execute(vm);
            screen.Router.NavigateBack.Execute();

            Assert.Equal(1, count);
        }

        [Fact]
        public void WhenNavigatingFromObservableCompletesWhenNavigationStackIsReset()
        {
            var count = 0;

            var screen = new TestScreen();
            var vm1 = new RoutableViewModel(screen);
            var vm2 = new RoutableViewModel(screen);

            vm1.WhenNavigatingFromObservable().Subscribe(
                _ => { },
                () => { count++; });

            screen.Router.Navigate.Execute(vm1);
            screen.Router.NavigateAndReset.Execute(vm2);

            Assert.Equal(1, count);
        }

        private class TestScreen : IScreen
        {
            public RoutingState Router { get; } = new();
        }

        private class RoutableViewModel : ReactiveObject, IRoutableViewModel
        {
            public RoutableViewModel(IScreen screen) => HostScreen = screen;

            public string UrlPathSegment => "Test";

            public IScreen HostScreen { get; }
        }
    }
}
