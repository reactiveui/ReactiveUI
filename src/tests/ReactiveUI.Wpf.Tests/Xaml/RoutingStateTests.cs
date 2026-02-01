// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.TestGuiMocks.CommonGuiMocks.Mocks;
using ReactiveUI.Tests.Utilities.Schedulers;
using ReactiveUI.Tests.Wpf;
using ReactiveUI.Tests.Xaml.Mocks;

namespace ReactiveUI.Tests.Xaml;

[NotInParallel]
[TestExecutor<WpfTestExecutor>]
public class RoutingStateTests
{

    /// <summary>
    /// Navigations the push pop test.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task NavigationPushPopTest()
    {
        var input = new TestViewModel { SomeProp = "Foo" };
        var fixture = new RoutingState(ImmediateScheduler.Instance);

        await Assert.That(await fixture.NavigateBack.CanExecute.FirstAsync()).IsFalse();
        fixture.Navigate.Execute(input).Subscribe();

        using (Assert.Multiple())
        {
            await Assert.That(fixture.NavigationStack).Count().IsEqualTo(1);
            await Assert.That(await fixture.NavigateBack.CanExecute.FirstAsync()).IsFalse();
        }

        await fixture.Navigate.Execute(new TestViewModel());

        using (Assert.Multiple())
        {
            await Assert.That(fixture.NavigationStack).Count().IsEqualTo(2);
            await Assert.That(await fixture.NavigateBack.CanExecute.FirstAsync()).IsTrue();
        }

        IRoutableViewModel? navigatedTo = null;
        fixture.NavigateBack.Execute().Subscribe(vm => navigatedTo = vm);
        using (Assert.Multiple())
        {
            await Assert.That(navigatedTo).IsSameReferenceAs(input);
            await Assert.That(fixture.NavigationStack).Count().IsEqualTo(1);
        }
    }

    /// <summary>
    /// Currents the view model observable is accurate.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task CurrentViewModelObservableIsAccurate()
    {
        var fixture = new RoutingState(ImmediateScheduler.Instance);
        var output = new List<IRoutableViewModel?>();
        fixture.CurrentViewModel.Subscribe(vm => output.Add(vm));

        await Assert.That(output).Count().IsEqualTo(1);

        fixture.Navigate.Execute(new TestViewModel { SomeProp = "A" }).Subscribe();
        await Assert.That(output).Count().IsEqualTo(2);

        fixture.Navigate.Execute(new TestViewModel { SomeProp = "B" }).Subscribe();
        using (Assert.Multiple())
        {
            await Assert.That(output).Count().IsEqualTo(3);
            await Assert.That((output.Last() as TestViewModel)?.SomeProp).IsEqualTo("B");
        }

        var navigatedTo = await fixture.NavigateBack.Execute();
        using (Assert.Multiple())
        {
            await Assert.That(output.Last()?.GetType()).IsEqualTo(navigatedTo?.GetType());
            await Assert.That(output).Count().IsEqualTo(4);
            await Assert.That((output.Last() as TestViewModel)?.SomeProp).IsEqualTo("A");
        }

        await Assert.That((output.Last() as TestViewModel)?.SomeProp).IsEqualTo((navigatedTo as TestViewModel)?.SomeProp);

        fixture.Navigate.Execute(new TestViewModel { SomeProp = "B" }).Subscribe();
        using (Assert.Multiple())
        {
            await Assert.That(output).Count().IsEqualTo(5);
            await Assert.That((output.Last() as TestViewModel)?.SomeProp).IsEqualTo("B");
        }

        fixture.Navigate.Execute(new TestViewModel { SomeProp = "C" }).Subscribe();
        using (Assert.Multiple())
        {
            await Assert.That(output).Count().IsEqualTo(6);
            await Assert.That((output.Last() as TestViewModel)?.SomeProp).IsEqualTo("C");
        }

        navigatedTo = await fixture.NavigateBack.Execute();
        using (Assert.Multiple())
        {
            await Assert.That(output.Last()?.GetType()).IsEqualTo(navigatedTo?.GetType());
            await Assert.That(output).Count().IsEqualTo(7);
            await Assert.That((output.Last() as TestViewModel)?.SomeProp).IsEqualTo("B");
        }

        await Assert.That((output.Last() as TestViewModel)?.SomeProp).IsEqualTo((navigatedTo as TestViewModel)?.SomeProp);

        navigatedTo = await fixture.NavigateBack.Execute();
        using (Assert.Multiple())
        {
            await Assert.That(output.Last()?.GetType()).IsEqualTo(navigatedTo?.GetType());
            await Assert.That(output).Count().IsEqualTo(8);
            await Assert.That((output.Last() as TestViewModel)?.SomeProp).IsEqualTo("A");
        }

        await Assert.That((output.Last() as TestViewModel)?.SomeProp).IsEqualTo((navigatedTo as TestViewModel)?.SomeProp);

        navigatedTo = await fixture.NavigateBack.Execute();
        using (Assert.Multiple())
        {
            await Assert.That(output.Last()?.GetType()).IsEqualTo(navigatedTo?.GetType());
            await Assert.That(output).Count().IsEqualTo(9);
            await Assert.That((output.Last() as TestViewModel)?.SomeProp).IsNull();
            await Assert.That(navigatedTo as TestViewModel).IsNull();
        }
    }

    /// <summary>
    /// Currents the view model observable is accurate via when any observable.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task CurrentViewModelObservableIsAccurateViaWhenAnyObservable()
    {
        var fixture = new TestScreen();
        var output = new List<IRoutableViewModel?>();
        fixture.WhenAnyObservable(static x => x.Router!.CurrentViewModel)
               .Subscribe(vm => output.Add(vm));

        fixture.Router = new RoutingState(ImmediateScheduler.Instance);

        await Assert.That(output).Count().IsEqualTo(1);

        fixture.Router.Navigate.Execute(new TestViewModel { SomeProp = "A" }).Subscribe();
        await Assert.That(output).Count().IsEqualTo(2);

        fixture.Router.Navigate.Execute(new TestViewModel { SomeProp = "B" }).Subscribe();
        using (Assert.Multiple())
        {
            await Assert.That(output).Count().IsEqualTo(3);
            await Assert.That((output.Last() as TestViewModel)?.SomeProp).IsEqualTo("B");
        }

        fixture.Router.NavigateBack.Execute().Subscribe();
        using (Assert.Multiple())
        {
            await Assert.That(output).Count().IsEqualTo(4);
            await Assert.That((output.Last() as TestViewModel)?.SomeProp).IsEqualTo("A");
        }
    }

    /// <summary>
    /// Navigates the and reset check navigation stack.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task NavigateAndResetCheckNavigationStack()
    {
        var fixture = new TestScreen
        {
            Router = new RoutingState(ImmediateScheduler.Instance)
        };
        var viewModel = new TestViewModel();

        await Assert.That(fixture.Router.NavigationStack).Count().IsLessThanOrEqualTo(0);

        fixture.Router.NavigateAndReset.Execute(viewModel).Subscribe();

        using (Assert.Multiple())
        {
            await Assert.That(fixture.Router.NavigationStack).Count().IsEqualTo(1);
            await Assert.That(ReferenceEquals(fixture.Router.NavigationStack.First(), viewModel)).IsTrue();
        }
    }

    /// <summary>
    /// Schedulers the is used for all commands.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<WithSchedulerExecutor>]
    public async Task SchedulerIsUsedForAllCommands()
    {
        var scheduler = TestContext.Current!.GetScheduler();
        var fixture = new RoutingState(scheduler);

        // Navigate should execute synchronously on ImmediateScheduler
        fixture.Navigate.Execute(new TestViewModel()).Subscribe();
        await Assert.That(fixture.NavigationStack).Count().IsEqualTo(1);

        // Navigate again
        fixture.Navigate.Execute(new TestViewModel()).Subscribe();
        await Assert.That(fixture.NavigationStack).Count().IsEqualTo(2);

        // NavigateBack should execute synchronously on ImmediateScheduler
        fixture.NavigateBack.Execute().Subscribe();
        await Assert.That(fixture.NavigationStack).Count().IsEqualTo(1);

        // NavigateAndReset should execute synchronously on ImmediateScheduler
        fixture.NavigateAndReset.Execute(new TestViewModel()).Subscribe();
        await Assert.That(fixture.NavigationStack).Count().IsEqualTo(1);
    }

    [Test]
    [TestExecutor<WithSchedulerExecutor>]
    public async Task RoutingStateThrows()
    {
        var scheduler = TestContext.Current!.GetScheduler();
        var fixture = new RoutingState(scheduler);

        // Set up observable to capture the thrown exception
        var exceptionTask = fixture.Navigate.ThrownExceptions
            .FirstAsync()
            .Timeout(TimeSpan.FromSeconds(5))
            .ToTask();

        // Execute with null to trigger the exception - subscribe with error handler to catch it
        fixture.Navigate.Execute(null!).Subscribe(_ => { }, ex => { });

        // Wait for the exception to be captured through ThrownExceptions
        var thrownException = await exceptionTask;

        await Assert.That(thrownException).IsNotNull();
        await Assert.That(thrownException!.Message).Contains("Navigate must be called on an IRoutableViewModel");
    }

    /// <summary>
    /// Test FindViewModelInStack finds the correct view model.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task FindViewModelInStackFindsCorrectViewModel()
    {
        var fixture = new RoutingState(ImmediateScheduler.Instance);
        var vm1 = new TestViewModel { SomeProp = "First" };
        var vm2 = new TestViewModel { SomeProp = "Second" };
        var vm3 = new TestViewModel { SomeProp = "Third" };

        fixture.Navigate.Execute(vm1).Subscribe();
        fixture.Navigate.Execute(vm2).Subscribe();
        fixture.Navigate.Execute(vm3).Subscribe();

        var found = fixture.FindViewModelInStack<TestViewModel>();

        await Assert.That(found).IsEqualTo(vm3); // Should find the last one (topmost)
    }

    /// <summary>
    /// Test FindViewModelInStack returns null when not found.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task FindViewModelInStackReturnsNullWhenNotFound()
    {
        var fixture = new RoutingState(ImmediateScheduler.Instance);
        var found = fixture.FindViewModelInStack<TestViewModel>();

        await Assert.That(found).IsNull();
    }

    /// <summary>
    /// Test FindViewModelInStack searches from top of stack.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task FindViewModelInStackSearchesFromTop()
    {
        var fixture = new RoutingState(ImmediateScheduler.Instance);
        var vm1 = new TestViewModel { SomeProp = "First" };
        var vm2 = new AlternateViewModel();
        var vm3 = new TestViewModel { SomeProp = "Third" };

        fixture.Navigate.Execute(vm1).Subscribe();
        fixture.Navigate.Execute(vm2).Subscribe();
        fixture.Navigate.Execute(vm3).Subscribe();

        var found = fixture.FindViewModelInStack<TestViewModel>();

        await Assert.That(found?.SomeProp).IsEqualTo("Third");
    }

    /// <summary>
    /// Test FindViewModelInStack throws on null.
    /// </summary>
    [Test]
    public void FindViewModelInStackThrowsOnNull()
    {
        RoutingState? fixture = null;
        Assert.Throws<ArgumentNullException>(() => fixture!.FindViewModelInStack<TestViewModel>());
    }

    /// <summary>
    /// Test GetCurrentViewModel returns the top view model.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task GetCurrentViewModelReturnsTopViewModel()
    {
        var fixture = new RoutingState(ImmediateScheduler.Instance);
        var vm1 = new TestViewModel { SomeProp = "First" };
        var vm2 = new TestViewModel { SomeProp = "Second" };

        fixture.Navigate.Execute(vm1).Subscribe();
        fixture.Navigate.Execute(vm2).Subscribe();

        var current = fixture.GetCurrentViewModel();

        await Assert.That(current).IsEqualTo(vm2);
    }

    /// <summary>
    /// Test GetCurrentViewModel returns null for empty stack.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task GetCurrentViewModelReturnsNullForEmptyStack()
    {
        var fixture = new RoutingState(ImmediateScheduler.Instance);
        var current = fixture.GetCurrentViewModel();

        await Assert.That(current).IsNull();
    }

    /// <summary>
    /// Test GetCurrentViewModel throws on null.
    /// </summary>
    [Test]
    public void GetCurrentViewModelThrowsOnNull()
    {
        RoutingState? fixture = null;
        Assert.Throws<ArgumentNullException>(() => fixture!.GetCurrentViewModel());
    }

    /// <summary>
    /// Test WhenNavigatedToObservable fires when navigated to.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task WhenNavigatedToObservableFires()
    {
        var screen = new TestScreen { Router = new RoutingState(ImmediateScheduler.Instance) };
        var vm = new TestViewModel { HostScreen = screen };

        var fired = false;
        vm.WhenNavigatedToObservable().Subscribe(_ => fired = true);

        screen.Router.Navigate.Execute(vm).Subscribe();

        await Assert.That(fired).IsTrue();
    }

    /// <summary>
    /// Test WhenNavigatedToObservable completes when removed.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task WhenNavigatedToObservableCompletesWhenRemoved()
    {
        var screen = new TestScreen { Router = new RoutingState(ImmediateScheduler.Instance) };
        var vm = new TestViewModel { HostScreen = screen };

        var completed = false;
        vm.WhenNavigatedToObservable().Subscribe(_ => { }, () => completed = true);

        screen.Router.Navigate.Execute(vm).Subscribe();
        screen.Router.NavigateAndReset.Execute(new TestViewModel { HostScreen = screen }).Subscribe();

        await Assert.That(completed).IsTrue();
    }

    /// <summary>
    /// Test WhenNavigatingFromObservable fires when navigating away.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task WhenNavigatingFromObservableFires()
    {
        var screen = new TestScreen { Router = new RoutingState(ImmediateScheduler.Instance) };
        var vm1 = new TestViewModel { HostScreen = screen };
        var vm2 = new TestViewModel { HostScreen = screen };

        var fired = false;
        vm1.WhenNavigatingFromObservable().Subscribe(_ => fired = true);

        screen.Router.Navigate.Execute(vm1).Subscribe();
        screen.Router.Navigate.Execute(vm2).Subscribe();

        await Assert.That(fired).IsTrue();
    }

    /// <summary>
    /// Test WhenNavigatedTo sets up and tears down correctly.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task WhenNavigatedToSetsUpAndTearsDown()
    {
        var screen = new TestScreen { Router = new RoutingState(ImmediateScheduler.Instance) };
        var vm = new TestViewModel { HostScreen = screen };

        var setupCount = 0;
        var teardownCount = 0;

        vm.WhenNavigatedTo(() =>
        {
            setupCount++;
            return Disposable.Create(() => teardownCount++);
        });

        screen.Router.Navigate.Execute(vm).Subscribe();
        await Assert.That(setupCount).IsEqualTo(1);

        screen.Router.Navigate.Execute(new TestViewModel { HostScreen = screen }).Subscribe();
        await Assert.That(teardownCount).IsEqualTo(1);
    }

    /// <summary>
    /// Test WhenNavigatedTo throws on null.
    /// </summary>
    [Test]
    public void WhenNavigatedToThrowsOnNull()
    {
        TestViewModel? vm = null;
        Assert.Throws<ArgumentNullException>(() => vm!.WhenNavigatedTo(() => Disposable.Empty));
    }

    /// <summary>
    /// Test WhenNavigatedToObservable throws on null.
    /// </summary>
    [Test]
    public void WhenNavigatedToObservableThrowsOnNull()
    {
        TestViewModel? vm = null;
        Assert.Throws<ArgumentNullException>(() => vm!.WhenNavigatedToObservable());
    }

    /// <summary>
    /// Test WhenNavigatingFromObservable throws on null.
    /// </summary>
    [Test]
    public void WhenNavigatingFromObservableThrowsOnNull()
    {
        TestViewModel? vm = null;
        Assert.Throws<ArgumentNullException>(() => vm!.WhenNavigatingFromObservable());
    }

    /// <summary>
    /// Alternate view model for testing.
    /// </summary>
    private class AlternateViewModel : ReactiveUI.ReactiveObject, IRoutableViewModel
    {
        public string? UrlPathSegment { get; set; }

        public IScreen HostScreen { get; set; } = null!;
    }
}
