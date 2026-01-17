// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Tests.Wpf;

namespace ReactiveUI.Tests.Xaml;

/// <summary>
/// Routable ViewModel MixinTests.
/// </summary>
[NotInParallel]
[TestExecutor<WpfTestExecutor>]
public class RoutableViewModelMixinTests
{
    /// <summary>
    /// Whens the navigated to calls on navigated to when view model is first added.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task WhenNavigatedToCallsOnNavigatedToWhenViewModelIsFirstAdded()
    {
        var count = 0;

        var screen = new TestScreen();
        var vm = new RoutableViewModel(screen);

        vm.WhenNavigatedTo(() =>
        {
            count++;

            return Disposable.Empty;
        });

        screen.Router.Navigate.Execute(vm).Subscribe();

        await Assert.That(count).IsEqualTo(1);
    }

    /// <summary>
    /// Whens the navigated to calls on navigated to when view model returns to top of stack.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task WhenNavigatedToCallsOnNavigatedToWhenViewModelReturnsToTopOfStack()
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

        screen.Router.Navigate.Execute(vm).Subscribe();
        screen.Router.Navigate.Execute(vm2).Subscribe();
        screen.Router.Navigate.Execute(vm).Subscribe();

        await Assert.That(count).IsEqualTo(2);
    }

    /// <summary>
    /// Whens the navigated to calls dispose when view model loses focus.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task WhenNavigatedToCallsDisposeWhenViewModelLosesFocus()
    {
        var count = 0;

        var screen = new TestScreen();
        var vm = new RoutableViewModel(screen);
        var vm2 = new RoutableViewModel(screen);

        vm.WhenNavigatedTo(() => Disposable.Create(() => count++));

        screen.Router.Navigate.Execute(vm).Subscribe();

        await Assert.That(count).IsEqualTo(0);

        screen.Router.Navigate.Execute(vm2).Subscribe();

        await Assert.That(count).IsEqualTo(1);
    }

    /// <summary>
    /// Whens the navigated to calls dispose when navigation stack is reset.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task WhenNavigatedToCallsDisposeWhenNavigationStackIsReset()
    {
        var count = 0;

        var screen = new TestScreen();
        var vm1 = new RoutableViewModel(screen);
        var vm2 = new RoutableViewModel(screen);

        vm1.WhenNavigatedTo(() => Disposable.Create(() => count++));

        screen.Router.Navigate.Execute(vm1).Subscribe();

        await Assert.That(count).IsEqualTo(0);

        screen.Router.NavigateAndReset.Execute(vm2).Subscribe();

        await Assert.That(count).IsEqualTo(1);
    }

    /// <summary>
    /// Whens the navigated to observable fires when view model added to navigation stack.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task WhenNavigatedToObservableFiresWhenViewModelAddedToNavigationStack()
    {
        var count = 0;

        var screen = new TestScreen();
        var vm = new RoutableViewModel(screen);

        vm.WhenNavigatedToObservable().Subscribe(_ => count++);

        screen.Router.Navigate.Execute(vm).Subscribe();

        await Assert.That(count).IsEqualTo(1);
    }

    /// <summary>
    /// Whens the navigated to observable fires when view model returns to navigation stack.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task WhenNavigatedToObservableFiresWhenViewModelReturnsToNavigationStack()
    {
        var count = 0;

        var screen = new TestScreen();
        var vm = new RoutableViewModel(screen);
        var vm2 = new RoutableViewModel(screen);

        vm.WhenNavigatedToObservable().Subscribe(_ => count++);

        screen.Router.Navigate.Execute(vm).Subscribe();
        screen.Router.Navigate.Execute(vm2).Subscribe();
        screen.Router.Navigate.Execute(vm).Subscribe();

        await Assert.That(count).IsEqualTo(2);
    }

    /// <summary>
    /// Whens the navigated to observable completes when view model is removed from navigation stack.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task WhenNavigatedToObservableCompletesWhenViewModelIsRemovedFromNavigationStack()
    {
        var count = 0;

        var screen = new TestScreen();
        var vm = new RoutableViewModel(screen);

        vm.WhenNavigatedToObservable().Subscribe(
            _ => { },
            () => count++);

        screen.Router.Navigate.Execute(vm).Subscribe();
        screen.Router.NavigateBack.Execute().Subscribe();

        await Assert.That(count).IsEqualTo(1);
    }

    /// <summary>
    /// Whens the navigated to observable completes when navigation stack is reset.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task WhenNavigatedToObservableCompletesWhenNavigationStackIsReset()
    {
        var count = 0;

        var screen = new TestScreen();
        var vm1 = new RoutableViewModel(screen);
        var vm2 = new RoutableViewModel(screen);

        vm1.WhenNavigatedToObservable().Subscribe(
            _ => { },
            () => count++);

        screen.Router.Navigate.Execute(vm1).Subscribe();
        screen.Router.NavigateAndReset.Execute(vm2).Subscribe();

        await Assert.That(count).IsEqualTo(1);
    }

    /// <summary>
    /// Whens the navigating from observable fires when view model loses focus.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task WhenNavigatingFromObservableFiresWhenViewModelLosesFocus()
    {
        var count = 0;
        var screen = new TestScreen();
        var vm = new RoutableViewModel(screen);
        var vm2 = new RoutableViewModel(screen);

        vm.WhenNavigatingFromObservable().Subscribe(_ => count++);

        screen.Router.Navigate.Execute(vm).Subscribe();
        screen.Router.Navigate.Execute(vm2).Subscribe();

        await Assert.That(count).IsEqualTo(1);
    }

    /// <summary>
    /// Whens the navigating from observable completes when view model is removed from navigation stack.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task WhenNavigatingFromObservableCompletesWhenViewModelIsRemovedFromNavigationStack()
    {
        var count = 0;

        var screen = new TestScreen();
        var vm = new RoutableViewModel(screen);

        vm.WhenNavigatingFromObservable().Subscribe(
            _ => { },
            () => count++);

        screen.Router.Navigate.Execute(vm).Subscribe();
        screen.Router.NavigateBack.Execute().Subscribe();

        await Assert.That(count).IsEqualTo(1);
    }

    /// <summary>
    /// Whens the navigating from observable completes when navigation stack is reset.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task WhenNavigatingFromObservableCompletesWhenNavigationStackIsReset()
    {
        var count = 0;

        var screen = new TestScreen();
        var vm1 = new RoutableViewModel(screen);
        var vm2 = new RoutableViewModel(screen);

        vm1.WhenNavigatingFromObservable().Subscribe(
            _ => { },
            () => count++);

        screen.Router.Navigate.Execute(vm1).Subscribe();
        screen.Router.NavigateAndReset.Execute(vm2).Subscribe();

        await Assert.That(count).IsEqualTo(1);
    }

    private class TestScreen : IScreen
    {
        public RoutingState Router { get; } = new(ImmediateScheduler.Instance);
    }

    private class RoutableViewModel(IScreen screen) : ReactiveUI.ReactiveObject, IRoutableViewModel
    {
        public string UrlPathSegment => "Test";

        public IScreen HostScreen { get; } = screen;
    }
}
