// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.TestGuiMocks.CommonGuiMocks.Mocks;
using ReactiveUI.Tests.Utilities.Schedulers;
using ReactiveUI.Tests.Wpf;
using ReactiveUI.Tests.Xaml.Mocks;
using TUnit.Core.Executors;

namespace ReactiveUI.Tests.Xaml;

/// <summary>Tests for <see cref="RoutingState"/>.</summary>
[NotInParallel]
[TestExecutor<WpfTestExecutor>]
public class RoutingStateTests
{
    /// <summary>The expected emission count of two.</summary>
    private const int ExpectedCountTwo = 2;

    /// <summary>The expected emission count of three.</summary>
    private const int ExpectedCountThree = 3;

    /// <summary>The expected emission count of four.</summary>
    private const int ExpectedCountFour = 4;

    /// <summary>The expected emission count of five.</summary>
    private const int ExpectedCountFive = 5;

    /// <summary>The expected emission count of six.</summary>
    private const int ExpectedCountSix = 6;

    /// <summary>The expected emission count of seven.</summary>
    private const int ExpectedCountSeven = 7;

    /// <summary>The expected emission count of eight.</summary>
    private const int ExpectedCountEight = 8;

    /// <summary>The expected emission count of nine.</summary>
    private const int ExpectedCountNine = 9;

    /// <summary>The timeout, in seconds, awaited for a thrown exception.</summary>
    private const int ThrownExceptionTimeoutSeconds = 5;

    /// <summary>Navigations the push pop test.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task NavigationPushPopTest()
    {
        var input = new TestViewModel { SomeProp = "Foo" };
        var fixture = new RoutingState(Sequencer.Immediate);

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
            await Assert.That(fixture.NavigationStack).Count().IsEqualTo(ExpectedCountTwo);
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

    /// <summary>Currents the view model observable is accurate.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task CurrentViewModelObservableIsAccurate()
    {
        var fixture = new RoutingState(Sequencer.Immediate);
        var output = new List<IRoutableViewModel?>();
        fixture.CurrentViewModel.Subscribe(vm => output.Add(vm));

        await Assert.That(output).Count().IsEqualTo(1);

        fixture.Navigate.Execute(new TestViewModel { SomeProp = "A" }).Subscribe();
        await Assert.That(output).Count().IsEqualTo(ExpectedCountTwo);

        await NavigateForwardAndAssert(fixture, output, "B", ExpectedCountThree);

        await NavigateBackAndAssert(fixture, output, ExpectedCountFour, "A");

        await NavigateForwardAndAssert(fixture, output, "B", ExpectedCountFive);
        await NavigateForwardAndAssert(fixture, output, "C", ExpectedCountSix);

        await NavigateBackAndAssert(fixture, output, ExpectedCountSeven, "B");
        await NavigateBackAndAssert(fixture, output, ExpectedCountEight, "A");
        await NavigateBackAndAssert(fixture, output, ExpectedCountNine, null);
    }

    /// <summary>Currents the view model observable is accurate via when any observable.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task CurrentViewModelObservableIsAccurateViaWhenAnyObservable()
    {
        // TestScreen seeds a default Router in its constructor, so subscribing observes that router's
        // CurrentViewModel seed (empty stack -> null) immediately, and swapping Router emits the new router's
        // seed too. CurrentViewModel emits a value on subscribe for an empty stack by design (the change layer's
        // initial batch), so both seeds count. This matches the released System.Reactive build's behaviour.
        var fixture = new TestScreen();
        var output = new List<IRoutableViewModel?>();
        fixture.WhenAnyObservable(static x => x.Router!.CurrentViewModel)
               .Subscribe(vm => output.Add(vm));

        // One seed from the constructor's default Router.
        await Assert.That(output).Count().IsEqualTo(1);

        fixture.Router = new(Sequencer.Immediate);

        // Plus one seed from the swapped-in Router.
        await Assert.That(output).Count().IsEqualTo(ExpectedCountTwo);

        fixture.Router.Navigate.Execute(new TestViewModel { SomeProp = "A" }).Subscribe();
        await Assert.That(output).Count().IsEqualTo(ExpectedCountThree);

        fixture.Router.Navigate.Execute(new TestViewModel { SomeProp = "B" }).Subscribe();
        using (Assert.Multiple())
        {
            await Assert.That(output).Count().IsEqualTo(ExpectedCountFour);
            await Assert.That((output[^1] as TestViewModel)?.SomeProp).IsEqualTo("B");
        }

        fixture.Router.NavigateBack.Execute().Subscribe();
        using (Assert.Multiple())
        {
            await Assert.That(output).Count().IsEqualTo(ExpectedCountFive);
            await Assert.That((output[^1] as TestViewModel)?.SomeProp).IsEqualTo("A");
        }
    }

    /// <summary>Navigates the and reset check navigation stack.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task NavigateAndResetCheckNavigationStack()
    {
        var fixture = new TestScreen
        {
            Router = new(Sequencer.Immediate)
        };
        var viewModel = new TestViewModel();

        await Assert.That(fixture.Router.NavigationStack).Count().IsLessThanOrEqualTo(0);

        fixture.Router.NavigateAndReset.Execute(viewModel).Subscribe();

        using (Assert.Multiple())
        {
            await Assert.That(fixture.Router.NavigationStack).Count().IsEqualTo(1);
            await Assert.That(ReferenceEquals(fixture.Router.NavigationStack[0], viewModel)).IsTrue();
        }
    }

    /// <summary>Schedulers the is used for all commands.</summary>
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
        await Assert.That(fixture.NavigationStack).Count().IsEqualTo(ExpectedCountTwo);

        // NavigateBack should execute synchronously on ImmediateScheduler
        fixture.NavigateBack.Execute().Subscribe();
        await Assert.That(fixture.NavigationStack).Count().IsEqualTo(1);

        // NavigateAndReset should execute synchronously on ImmediateScheduler
        fixture.NavigateAndReset.Execute(new TestViewModel()).Subscribe();
        await Assert.That(fixture.NavigationStack).Count().IsEqualTo(1);
    }

    /// <summary>Verifies that navigation exceptions are surfaced through <c>ThrownExceptions</c>.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<WithSchedulerExecutor>]
    public async Task RoutingStateThrows()
    {
        var scheduler = TestContext.Current!.GetScheduler();
        var fixture = new RoutingState(scheduler);

        // Set up observable to capture the thrown exception
        var exceptionTask = fixture.Navigate.ThrownExceptions
            .Timeout(TimeSpan.FromSeconds(ThrownExceptionTimeoutSeconds))
            .FirstAsync();

        // Execute with null to trigger the exception - subscribe with error handler to catch it
        fixture.Navigate.Execute(null!).Subscribe(_ => { }, ex => { });

        // Wait for the exception to be captured through ThrownExceptions
        var thrownException = await exceptionTask;

        await Assert.That(thrownException).IsNotNull();
        await Assert.That(thrownException!.Message).Contains("Navigate must be called on an IRoutableViewModel");
    }

    /// <summary>Test FindViewModelInStack finds the correct view model.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task FindViewModelInStackFindsCorrectViewModel()
    {
        var fixture = new RoutingState(Sequencer.Immediate);
        var vm1 = new TestViewModel { SomeProp = "First" };
        var vm2 = new TestViewModel { SomeProp = "Second" };
        var vm3 = new TestViewModel { SomeProp = "Third" };

        fixture.Navigate.Execute(vm1).Subscribe();
        fixture.Navigate.Execute(vm2).Subscribe();
        fixture.Navigate.Execute(vm3).Subscribe();

        var found = fixture.FindViewModelInStack<TestViewModel>();

        await Assert.That(found).IsEqualTo(vm3); // Should find the last one (topmost)
    }

    /// <summary>Test FindViewModelInStack returns null when not found.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task FindViewModelInStackReturnsNullWhenNotFound()
    {
        var fixture = new RoutingState(Sequencer.Immediate);
        var found = fixture.FindViewModelInStack<TestViewModel>();

        await Assert.That(found).IsNull();
    }

    /// <summary>Test FindViewModelInStack searches from top of stack.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task FindViewModelInStackSearchesFromTop()
    {
        var fixture = new RoutingState(Sequencer.Immediate);
        var vm1 = new TestViewModel { SomeProp = "First" };
        var vm2 = new AlternateViewModel();
        var vm3 = new TestViewModel { SomeProp = "Third" };

        fixture.Navigate.Execute(vm1).Subscribe();
        fixture.Navigate.Execute(vm2).Subscribe();
        fixture.Navigate.Execute(vm3).Subscribe();

        var found = fixture.FindViewModelInStack<TestViewModel>();

        await Assert.That(found?.SomeProp).IsEqualTo("Third");
    }

    /// <summary>Test FindViewModelInStack throws on null.</summary>
    [Test]
    public void FindViewModelInStackThrowsOnNull()
    {
        RoutingState? fixture = null;
        Assert.Throws<ArgumentNullException>(() => fixture!.FindViewModelInStack<TestViewModel>());
    }

    /// <summary>Test GetCurrentViewModel returns the top view model.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task GetCurrentViewModelReturnsTopViewModel()
    {
        var fixture = new RoutingState(Sequencer.Immediate);
        var vm1 = new TestViewModel { SomeProp = "First" };
        var vm2 = new TestViewModel { SomeProp = "Second" };

        fixture.Navigate.Execute(vm1).Subscribe();
        fixture.Navigate.Execute(vm2).Subscribe();

        var current = fixture.GetCurrentViewModel();

        await Assert.That(current).IsEqualTo(vm2);
    }

    /// <summary>Test GetCurrentViewModel returns null for empty stack.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task GetCurrentViewModelReturnsNullForEmptyStack()
    {
        var fixture = new RoutingState(Sequencer.Immediate);
        var current = fixture.GetCurrentViewModel();

        await Assert.That(current).IsNull();
    }

    /// <summary>Test GetCurrentViewModel throws on null.</summary>
    [Test]
    public void GetCurrentViewModelThrowsOnNull()
    {
        RoutingState? fixture = null;
        Assert.Throws<ArgumentNullException>(() => fixture!.GetCurrentViewModel());
    }

    /// <summary>Test WhenNavigatedToObservable fires when navigated to.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task WhenNavigatedToObservableFires()
    {
        var screen = new TestScreen { Router = new(Sequencer.Immediate) };
        var vm = new TestViewModel { HostScreen = screen };

        var fired = false;
        vm.WhenNavigatedToObservable().Subscribe(_ => fired = true);

        screen.Router.Navigate.Execute(vm).Subscribe();

        await Assert.That(fired).IsTrue();
    }

    /// <summary>Test WhenNavigatedToObservable completes when removed.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task WhenNavigatedToObservableCompletesWhenRemoved()
    {
        var screen = new TestScreen { Router = new(Sequencer.Immediate) };
        var vm = new TestViewModel { HostScreen = screen };

        var completed = false;
        vm.WhenNavigatedToObservable().Subscribe(_ => { }, () => completed = true);

        screen.Router.Navigate.Execute(vm).Subscribe();
        screen.Router.NavigateAndReset.Execute(new TestViewModel { HostScreen = screen }).Subscribe();

        await Assert.That(completed).IsTrue();
    }

    /// <summary>Test WhenNavigatingFromObservable fires when navigating away.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task WhenNavigatingFromObservableFires()
    {
        var screen = new TestScreen { Router = new(Sequencer.Immediate) };
        var vm1 = new TestViewModel { HostScreen = screen };
        var vm2 = new TestViewModel { HostScreen = screen };

        var fired = false;
        vm1.WhenNavigatingFromObservable().Subscribe(_ => fired = true);

        screen.Router.Navigate.Execute(vm1).Subscribe();
        screen.Router.Navigate.Execute(vm2).Subscribe();

        await Assert.That(fired).IsTrue();
    }

    /// <summary>Test WhenNavigatedTo sets up and tears down correctly.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task WhenNavigatedToSetsUpAndTearsDown()
    {
        var screen = new TestScreen { Router = new(Sequencer.Immediate) };
        var vm = new TestViewModel { HostScreen = screen };

        var setupCount = 0;
        var teardownCount = 0;

        vm.WhenNavigatedTo(() =>
        {
            setupCount++;
            return Scope.Create(() => teardownCount++);
        });

        screen.Router.Navigate.Execute(vm).Subscribe();
        await Assert.That(setupCount).IsEqualTo(1);

        screen.Router.Navigate.Execute(new TestViewModel { HostScreen = screen }).Subscribe();
        await Assert.That(teardownCount).IsEqualTo(1);
    }

    /// <summary>Test WhenNavigatedTo throws on null.</summary>
    [Test]
    public void WhenNavigatedToThrowsOnNull()
    {
        TestViewModel? vm = null;
        Assert.Throws<ArgumentNullException>(() => vm!.WhenNavigatedTo(() => Scope.Empty));
    }

    /// <summary>Test WhenNavigatedToObservable throws on null.</summary>
    [Test]
    public void WhenNavigatedToObservableThrowsOnNull()
    {
        TestViewModel? vm = null;
        Assert.Throws<ArgumentNullException>(() => vm!.WhenNavigatedToObservable());
    }

    /// <summary>Test WhenNavigatingFromObservable throws on null.</summary>
    [Test]
    public void WhenNavigatingFromObservableThrowsOnNull()
    {
        TestViewModel? vm = null;
        Assert.Throws<ArgumentNullException>(() => vm!.WhenNavigatingFromObservable());
    }

    /// <summary>Navigates forward to a new <see cref="TestViewModel"/> and asserts the observed output count and the top-of-stack property value.</summary>
    /// <param name="fixture">The routing state under test.</param>
    /// <param name="output">The captured sequence of current view models.</param>
    /// <param name="someProp">The property value to navigate to and expect on top.</param>
    /// <param name="expectedCount">The expected number of observed view models.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    private static async Task NavigateForwardAndAssert(RoutingState fixture, List<IRoutableViewModel?> output, string someProp, int expectedCount)
    {
        fixture.Navigate.Execute(new TestViewModel { SomeProp = someProp }).Subscribe();
        using (Assert.Multiple())
        {
            await Assert.That(output).Count().IsEqualTo(expectedCount);
            await Assert.That((output[^1] as TestViewModel)?.SomeProp).IsEqualTo(someProp);
        }
    }

    /// <summary>
    /// Navigates back and asserts the observed output count, that the top-of-stack type
    /// matches the navigated-to view model, and the expected top property value.
    /// </summary>
    /// <param name="fixture">The routing state under test.</param>
    /// <param name="output">The captured sequence of current view models.</param>
    /// <param name="expectedCount">The expected number of observed view models.</param>
    /// <param name="expectedProp">The expected top property value, or <see langword="null"/> when the stack is empty.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    private static async Task NavigateBackAndAssert(RoutingState fixture, List<IRoutableViewModel?> output, int expectedCount, string? expectedProp)
    {
        var navigatedTo = await fixture.NavigateBack.Execute();
        using (Assert.Multiple())
        {
            await Assert.That(output[^1]?.GetType()).IsEqualTo(navigatedTo?.GetType());
            await Assert.That(output).Count().IsEqualTo(expectedCount);
            await Assert.That((output[^1] as TestViewModel)?.SomeProp).IsEqualTo(expectedProp);
        }

        await Assert.That((output[^1] as TestViewModel)?.SomeProp).IsEqualTo((navigatedTo as TestViewModel)?.SomeProp);

        if (expectedProp is not null)
        {
            return;
        }

        await Assert.That(navigatedTo as TestViewModel).IsNull();
    }

    /// <summary>Alternate view model for testing.</summary>
    private sealed class AlternateViewModel : ReactiveObject, IRoutableViewModel
    {
        /// <inheritdoc/>
        public string? UrlPathSegment { get; set; }

        /// <inheritdoc/>
        public IScreen HostScreen { get; set; } = null!;
    }
}
