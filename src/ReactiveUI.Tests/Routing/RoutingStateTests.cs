// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using DynamicData;

using Microsoft.Reactive.Testing;

using ReactiveUI.Testing;

namespace ReactiveUI.Tests;

public class RoutingStateTests
{
    /// <summary>
    /// Navigations the push pop test.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    public async Task NavigationPushPopTest()
    {
        var input = new TestViewModel { SomeProp = "Foo" };
        var fixture = new RoutingState();

        Assert.False(await fixture.NavigateBack.CanExecute.FirstAsync());
        await fixture.Navigate.Execute(new TestViewModel());

        Assert.Equal(1, fixture.NavigationStack.Count);
        Assert.False(await fixture.NavigateBack.CanExecute.FirstAsync());

        await fixture.Navigate.Execute(new TestViewModel());

        Assert.Equal(2, fixture.NavigationStack.Count);
        Assert.True(await fixture.NavigateBack.CanExecute.FirstAsync());

        var navigatedTo = await fixture.NavigateBack.Execute() ?? throw new InvalidOperationException("Should have valid navigated to screen");
        Assert.Equal(navigatedTo.GetType(), input.GetType());
        Assert.Equal(1, fixture.NavigationStack.Count);
    }

    /// <summary>
    /// Currents the view model observable is accurate.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Fact]
    public async Task CurrentViewModelObservableIsAccurate()
    {
        var fixture = new RoutingState();
        fixture.CurrentViewModel.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var output).Subscribe();

        Assert.Equal(1, output.Count);

        await fixture.Navigate.Execute(new TestViewModel { SomeProp = "A" });
        Assert.Equal(2, output.Count);

        await fixture.Navigate.Execute(new TestViewModel { SomeProp = "B" });
        Assert.Equal(3, output.Count);
        Assert.Equal("B", (output.Last() as TestViewModel)?.SomeProp);

        var navigatedTo = await fixture.NavigateBack.Execute();
        Assert.Equal(navigatedTo?.GetType(), output.Last()?.GetType());
        Assert.Equal(4, output.Count);
        Assert.Equal("A", (output.Last() as TestViewModel)?.SomeProp);
        Assert.Equal((navigatedTo as TestViewModel)?.SomeProp, (output.Last() as TestViewModel)?.SomeProp);

        await fixture.Navigate.Execute(new TestViewModel { SomeProp = "B" });
        Assert.Equal(5, output.Count);
        Assert.Equal("B", (output.Last() as TestViewModel)?.SomeProp);

        await fixture.Navigate.Execute(new TestViewModel { SomeProp = "C" });
        Assert.Equal(6, output.Count);
        Assert.Equal("C", (output.Last() as TestViewModel)?.SomeProp);

        navigatedTo = await fixture.NavigateBack.Execute();
        Assert.Equal(navigatedTo?.GetType(), output.Last()?.GetType());
        Assert.Equal(7, output.Count);
        Assert.Equal("B", (output.Last() as TestViewModel)?.SomeProp);
        Assert.Equal((navigatedTo as TestViewModel)?.SomeProp, (output.Last() as TestViewModel)?.SomeProp);

        navigatedTo = await fixture.NavigateBack.Execute();
        Assert.Equal(navigatedTo?.GetType(), output.Last()?.GetType());
        Assert.Equal(8, output.Count);
        Assert.Equal("A", (output.Last() as TestViewModel)?.SomeProp);
        Assert.Equal((navigatedTo as TestViewModel)?.SomeProp, (output.Last() as TestViewModel)?.SomeProp);

        navigatedTo = await fixture.NavigateBack.Execute();
        Assert.Equal(navigatedTo?.GetType(), output.Last()?.GetType());
        Assert.Equal(9, output.Count);
        Assert.Equal(null, (output.Last() as TestViewModel)?.SomeProp);
        Assert.Equal(null, (navigatedTo as TestViewModel)?.SomeProp);
    }

    /// <summary>
    /// Currents the view model observable is accurate via when any observable.
    /// </summary>
    [Fact]
    public void CurrentViewModelObservableIsAccurateViaWhenAnyObservable()
    {
        var fixture = new TestScreen();
        fixture.WhenAnyObservable(x => x.Router!.CurrentViewModel)
               .ToObservableChangeSet(ImmediateScheduler.Instance)
               .Bind(out var output)
               .Subscribe();

        fixture.Router = new RoutingState();

        Assert.Equal(1, output.Count);

        fixture.Router.Navigate.Execute(new TestViewModel { SomeProp = "A" });
        Assert.Equal(2, output.Count);

        fixture.Router.Navigate.Execute(new TestViewModel { SomeProp = "B" });
        Assert.Equal(3, output.Count);
        Assert.Equal("B", (output.Last() as TestViewModel)?.SomeProp);

        fixture.Router.NavigateBack.Execute();
        Assert.Equal(4, output.Count);
        Assert.Equal("A", (output.Last() as TestViewModel)?.SomeProp);
    }

    /// <summary>
    /// Navigates the and reset check navigation stack.
    /// </summary>
    [Fact]
    public void NavigateAndResetCheckNavigationStack()
    {
        var fixture = new TestScreen
        {
            Router = new RoutingState()
        };
        var viewModel = new TestViewModel();

        Assert.False(fixture.Router.NavigationStack.Count > 0);

        fixture.Router.NavigateAndReset.Execute(viewModel);

        Assert.True(fixture.Router.NavigationStack.Count == 1);
        Assert.True(ReferenceEquals(fixture.Router.NavigationStack.First(), viewModel));
    }

    /// <summary>
    /// Schedulers the is used for all commands.
    /// </summary>
    [Fact]
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
                Assert.Empty(navigate);
                scheduler.Start();
                Assert.NotEmpty(navigate);

                fixture.NavigateBack.Execute().Subscribe();
                Assert.Empty(navigateBack);
                scheduler.Start();
                Assert.NotEmpty(navigateBack);

                fixture.NavigateAndReset.Execute(new TestViewModel()).Subscribe();
                Assert.Empty(navigateAndReset);
                scheduler.Start();
                Assert.NotEmpty(navigateAndReset);
            });

    [Fact]
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
