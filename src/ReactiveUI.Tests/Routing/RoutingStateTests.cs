// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using DynamicData;

using Microsoft.Reactive.Testing;

using ReactiveUI.Testing;

namespace ReactiveUI.Tests;

[TestFixture]
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
        var fixture = new RoutingState();

        Assert.That(await fixture.NavigateBack.CanExecute.FirstAsync(, Is.False));
        await fixture.Navigate.Execute(new TestViewModel());

        Assert.That(fixture.NavigationStack.Count, Is.EqualTo(1));
        Assert.That(await fixture.NavigateBack.CanExecute.FirstAsync(, Is.False));

        await fixture.Navigate.Execute(new TestViewModel());

        Assert.That(fixture.NavigationStack.Count, Is.EqualTo(2));
        Assert.That(await fixture.NavigateBack.CanExecute.FirstAsync(, Is.True));

        var navigatedTo = await fixture.NavigateBack.Execute() ?? throw new InvalidOperationException("Should have valid navigated to screen");
        Assert.That(input.GetType(, Is.EqualTo(navigatedTo.GetType())));
        Assert.That(fixture.NavigationStack.Count, Is.EqualTo(1));
    }

    /// <summary>
    /// Currents the view model observable is accurate.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task CurrentViewModelObservableIsAccurate()
    {
        var fixture = new RoutingState();
        fixture.CurrentViewModel.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var output).Subscribe();

        Assert.That(output.Count, Is.EqualTo(1));

        await fixture.Navigate.Execute(new TestViewModel { SomeProp = "A" });
        Assert.That(output.Count, Is.EqualTo(2));

        await fixture.Navigate.Execute(new TestViewModel { SomeProp = "B" });
        Assert.That(output.Count, Is.EqualTo(3));
        Assert.That((output.Last(, Is.EqualTo("B")) as TestViewModel)?.SomeProp);

        var navigatedTo = await fixture.NavigateBack.Execute();
        Assert.That(output.Last(, Is.EqualTo(navigatedTo?.GetType()))?.GetType());
        Assert.That(output.Count, Is.EqualTo(4));
        Assert.That((output.Last(, Is.EqualTo("A")) as TestViewModel)?.SomeProp);
        Assert.That((output.Last(, Is.EqualTo((navigatedTo as TestViewModel)?.SomeProp)) as TestViewModel)?.SomeProp);

        await fixture.Navigate.Execute(new TestViewModel { SomeProp = "B" });
        Assert.That(output.Count, Is.EqualTo(5));
        Assert.That((output.Last(, Is.EqualTo("B")) as TestViewModel)?.SomeProp);

        await fixture.Navigate.Execute(new TestViewModel { SomeProp = "C" });
        Assert.That(output.Count, Is.EqualTo(6));
        Assert.That((output.Last(, Is.EqualTo("C")) as TestViewModel)?.SomeProp);

        navigatedTo = await fixture.NavigateBack.Execute();
        Assert.That(output.Last(, Is.EqualTo(navigatedTo?.GetType()))?.GetType());
        Assert.That(output.Count, Is.EqualTo(7));
        Assert.That((output.Last(, Is.EqualTo("B")) as TestViewModel)?.SomeProp);
        Assert.That((output.Last(, Is.EqualTo((navigatedTo as TestViewModel)?.SomeProp)) as TestViewModel)?.SomeProp);

        navigatedTo = await fixture.NavigateBack.Execute();
        Assert.That(output.Last(, Is.EqualTo(navigatedTo?.GetType()))?.GetType());
        Assert.That(output.Count, Is.EqualTo(8));
        Assert.That((output.Last(, Is.EqualTo("A")) as TestViewModel)?.SomeProp);
        Assert.That((output.Last(, Is.EqualTo((navigatedTo as TestViewModel)?.SomeProp)) as TestViewModel)?.SomeProp);

        navigatedTo = await fixture.NavigateBack.Execute();
        Assert.That(output.Last(, Is.EqualTo(navigatedTo?.GetType()))?.GetType());
        Assert.That(output.Count, Is.EqualTo(9));
        Assert.That((output.Last(, Is.EqualTo(null)) as TestViewModel)?.SomeProp);
        Assert.That((navigatedTo as TestViewModel, Is.EqualTo(null))?.SomeProp);
    }

    /// <summary>
    /// Currents the view model observable is accurate via when any observable.
    /// </summary>
    [Test]
    public void CurrentViewModelObservableIsAccurateViaWhenAnyObservable()
    {
        var fixture = new TestScreen();
        fixture.WhenAnyObservable(x => x.Router!.CurrentViewModel)
               .ToObservableChangeSet(ImmediateScheduler.Instance)
               .Bind(out var output)
               .Subscribe();

        fixture.Router = new RoutingState();

        Assert.That(output.Count, Is.EqualTo(1));

        fixture.Router.Navigate.Execute(new TestViewModel { SomeProp = "A" });
        Assert.That(output.Count, Is.EqualTo(2));

        fixture.Router.Navigate.Execute(new TestViewModel { SomeProp = "B" });
        Assert.That(output.Count, Is.EqualTo(3));
        Assert.That((output.Last(, Is.EqualTo("B")) as TestViewModel)?.SomeProp);

        fixture.Router.NavigateBack.Execute();
        Assert.That(output.Count, Is.EqualTo(4));
        Assert.That((output.Last(, Is.EqualTo("A")) as TestViewModel)?.SomeProp);
    }

    /// <summary>
    /// Navigates the and reset check navigation stack.
    /// </summary>
    [Test]
    public void NavigateAndResetCheckNavigationStack()
    {
        var fixture = new TestScreen
        {
            Router = new RoutingState()
        };
        var viewModel = new TestViewModel();

        Assert.That(fixture.Router.NavigationStack.Count > 0, Is.False);

        fixture.Router.NavigateAndReset.Execute(viewModel);

        Assert.That(fixture.Router.NavigationStack.Count == 1, Is.True);
        Assert.That(ReferenceEquals(fixture.Router.NavigationStack.First(, Is.True), viewModel));
    }

    /// <summary>
    /// Schedulers the is used for all commands.
    /// </summary>
    [Test]
    public void SchedulerIsUsedForAllCommands() =>
        new TestScheduler().With(
            scheduler =>
            {
                var fixture = new RoutingState(scheduler);

                fixture
                    .Navigate
                    .ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var navigate).Subscribe();
                fixture
                    .NavigateBack
                    .ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var navigateBack).Subscribe();
                fixture
                    .NavigateAndReset
                    .ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var navigateAndReset).Subscribe();

                fixture.Navigate.Execute(new TestViewModel()).Subscribe();
                Assert.That(navigate, Is.Empty);
                scheduler.Start();
                Assert.That(navigate, Is.Not.Empty);

                fixture.NavigateBack.Execute().Subscribe();
                Assert.That(navigateBack, Is.Empty);
                scheduler.Start();
                Assert.That(navigateBack, Is.Not.Empty);

                fixture.NavigateAndReset.Execute(new TestViewModel()).Subscribe();
                Assert.That(navigateAndReset, Is.Empty);
                scheduler.Start();
                Assert.That(navigateAndReset, Is.Not.Empty);
            });

    [Test]
    public void RoutingStateThrows() =>
        new TestScheduler().With(
            scheduler =>
            {
                var fixture = new RoutingState(scheduler);

                fixture
                    .Navigate
                    .ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var navigate).Subscribe();
                fixture
                    .NavigateBack
                    .ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var navigateBack).Subscribe();
                fixture
                    .NavigateAndReset
                    .ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var navigateAndReset).Subscribe();

                Assert.Throws<Exception>(() => fixture.Navigate.Execute(default(TestViewModel)!).Subscribe());
            });
}
