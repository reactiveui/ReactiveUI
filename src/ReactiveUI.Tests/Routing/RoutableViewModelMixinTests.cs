// Copyright (c) 2023 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests;

/// <summary>
/// Routable ViewModel MixinTests.
/// </summary>
public class RoutableViewModelMixinTests
{
    /// <summary>
    /// Whens the navigated to calls on navigated to when view model is first added.
    /// </summary>
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

    /// <summary>
    /// Whens the navigated to calls on navigated to when view model returns to top of stack.
    /// </summary>
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

    /// <summary>
    /// Whens the navigated to calls dispose when view model loses focus.
    /// </summary>
    [Fact]
    public void WhenNavigatedToCallsDisposeWhenViewModelLosesFocus()
    {
        var count = 0;

        var screen = new TestScreen();
        var vm = new RoutableViewModel(screen);
        var vm2 = new RoutableViewModel(screen);

        vm.WhenNavigatedTo(() => Disposable.Create(() => count++));

        screen.Router.Navigate.Execute(vm);

        Assert.Equal(0, count);

        screen.Router.Navigate.Execute(vm2);

        Assert.Equal(1, count);
    }

    /// <summary>
    /// Whens the navigated to calls dispose when navigation stack is reset.
    /// </summary>
    [Fact]
    public void WhenNavigatedToCallsDisposeWhenNavigationStackIsReset()
    {
        var count = 0;

        var screen = new TestScreen();
        var vm1 = new RoutableViewModel(screen);
        var vm2 = new RoutableViewModel(screen);

        vm1.WhenNavigatedTo(() => Disposable.Create(() => count++));

        screen.Router.Navigate.Execute(vm1);

        Assert.Equal(0, count);

        screen.Router.NavigateAndReset.Execute(vm2);

        Assert.Equal(1, count);
    }

    /// <summary>
    /// Whens the navigated to observable fires when view model added to navigation stack.
    /// </summary>
    [Fact]
    public void WhenNavigatedToObservableFiresWhenViewModelAddedToNavigationStack()
    {
        var count = 0;

        var screen = new TestScreen();
        var vm = new RoutableViewModel(screen);

        vm.WhenNavigatedToObservable().Subscribe(_ => count++);

        screen.Router.Navigate.Execute(vm);

        Assert.Equal(1, count);
    }

    /// <summary>
    /// Whens the navigated to observable fires when view model returns to navigation stack.
    /// </summary>
    [Fact]
    public void WhenNavigatedToObservableFiresWhenViewModelReturnsToNavigationStack()
    {
        var count = 0;

        var screen = new TestScreen();
        var vm = new RoutableViewModel(screen);
        var vm2 = new RoutableViewModel(screen);

        vm.WhenNavigatedToObservable().Subscribe(_ => count++);

        screen.Router.Navigate.Execute(vm);
        screen.Router.Navigate.Execute(vm2);
        screen.Router.Navigate.Execute(vm);

        Assert.Equal(2, count);
    }

    /// <summary>
    /// Whens the navigated to observable completes when view model is removed from navigation stack.
    /// </summary>
    [Fact]
    public void WhenNavigatedToObservableCompletesWhenViewModelIsRemovedFromNavigationStack()
    {
        var count = 0;

        var screen = new TestScreen();
        var vm = new RoutableViewModel(screen);

        vm.WhenNavigatedToObservable().Subscribe(
            _ => { },
            () => count++);

        screen.Router.Navigate.Execute(vm);
        screen.Router.NavigateBack.Execute();

        Assert.Equal(1, count);
    }

    /// <summary>
    /// Whens the navigated to observable completes when navigation stack is reset.
    /// </summary>
    [Fact]
    public void WhenNavigatedToObservableCompletesWhenNavigationStackIsReset()
    {
        var count = 0;

        var screen = new TestScreen();
        var vm1 = new RoutableViewModel(screen);
        var vm2 = new RoutableViewModel(screen);

        vm1.WhenNavigatedToObservable().Subscribe(
            _ => { },
            () => count++);

        screen.Router.Navigate.Execute(vm1);
        screen.Router.NavigateAndReset.Execute(vm2);

        Assert.Equal(1, count);
    }

    /// <summary>
    /// Whens the navigating from observable fires when view model loses focus.
    /// </summary>
    [Fact]
    public void WhenNavigatingFromObservableFiresWhenViewModelLosesFocus()
    {
        var count = 0;
        var screen = new TestScreen();
        var vm = new RoutableViewModel(screen);
        var vm2 = new RoutableViewModel(screen);

        vm.WhenNavigatingFromObservable().Subscribe(_ => count++);

        screen.Router.Navigate.Execute(vm);
        screen.Router.Navigate.Execute(vm2);

        Assert.Equal(1, count);
    }

    /// <summary>
    /// Whens the navigating from observable completes when view model is removed from navigation stack.
    /// </summary>
    [Fact]
    public void WhenNavigatingFromObservableCompletesWhenViewModelIsRemovedFromNavigationStack()
    {
        var count = 0;

        var screen = new TestScreen();
        var vm = new RoutableViewModel(screen);

        vm.WhenNavigatingFromObservable().Subscribe(
            _ => { },
            () => count++);

        screen.Router.Navigate.Execute(vm);
        screen.Router.NavigateBack.Execute();

        Assert.Equal(1, count);
    }

    /// <summary>
    /// Whens the navigating from observable completes when navigation stack is reset.
    /// </summary>
    [Fact]
    public void WhenNavigatingFromObservableCompletesWhenNavigationStackIsReset()
    {
        var count = 0;

        var screen = new TestScreen();
        var vm1 = new RoutableViewModel(screen);
        var vm2 = new RoutableViewModel(screen);

        vm1.WhenNavigatingFromObservable().Subscribe(
            _ => { },
            () => count++);

        screen.Router.Navigate.Execute(vm1);
        screen.Router.NavigateAndReset.Execute(vm2);

        Assert.Equal(1, count);
    }

    private class TestScreen : IScreen
    {
        public RoutingState Router { get; } = new();
    }

    private class RoutableViewModel(IScreen screen) : ReactiveObject, IRoutableViewModel
    {
        public string UrlPathSegment => "Test";

        public IScreen HostScreen { get; } = screen;
    }
}
