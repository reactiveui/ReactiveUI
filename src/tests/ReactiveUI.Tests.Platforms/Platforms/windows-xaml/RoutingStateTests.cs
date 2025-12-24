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
    [Test]
    public async Task NavigationPushPopTest()
    {
        var input = new TestViewModel { SomeProp = "Foo" };
        var fixture = new RoutingState();

        await Assert.That(await fixture.NavigateBack.CanExecute.FirstAsync()).IsFalse();
        await fixture.Navigate.Execute(new TestViewModel());

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

        var navigatedTo = await fixture.NavigateBack.Execute() ?? throw new InvalidOperationException("Should have valid navigated to screen");
        using (Assert.Multiple())
        {
            await Assert.That(input.GetType()).IsEqualTo(navigatedTo.GetType());
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
        var fixture = new RoutingState();
        fixture.CurrentViewModel.ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var output).Subscribe();

        await Assert.That(output).Count().IsEqualTo(1);

        await fixture.Navigate.Execute(new TestViewModel { SomeProp = "A" });
        await Assert.That(output).Count().IsEqualTo(2);

        await fixture.Navigate.Execute(new TestViewModel { SomeProp = "B" });
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

        await fixture.Navigate.Execute(new TestViewModel { SomeProp = "B" });
        using (Assert.Multiple())
        {
            await Assert.That(output).Count().IsEqualTo(5);
            await Assert.That((output.Last() as TestViewModel)?.SomeProp).IsEqualTo("B");
        }

        await fixture.Navigate.Execute(new TestViewModel { SomeProp = "C" });
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
        fixture.WhenAnyObservable(static x => x.Router!.CurrentViewModel)
               .ToObservableChangeSet(ImmediateScheduler.Instance)
               .Bind(out var output)
               .Subscribe();

        fixture.Router = new RoutingState();

        await Assert.That(output).Count().IsEqualTo(1);

#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
        fixture.Router.Navigate.Execute(new TestViewModel { SomeProp = "A" });
        await Assert.That(output).Count().IsEqualTo(2);

        fixture.Router.Navigate.Execute(new TestViewModel { SomeProp = "B" });
        using (Assert.Multiple())
        {
            await Assert.That(output).Count().IsEqualTo(3);
            await Assert.That((output.Last() as TestViewModel)?.SomeProp).IsEqualTo("B");
        }

        fixture.Router.NavigateBack.Execute();
        using (Assert.Multiple())
        {
            await Assert.That(output).Count().IsEqualTo(4);
            await Assert.That((output.Last() as TestViewModel)?.SomeProp).IsEqualTo("A");
        }
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
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
            Router = new RoutingState()
        };
        var viewModel = new TestViewModel();

        await Assert.That(fixture.Router.NavigationStack).Count().IsLessThanOrEqualTo(0);

        await fixture.Router.NavigateAndReset.Execute(viewModel);

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
    public async Task SchedulerIsUsedForAllCommands() =>
        await new TestScheduler().With(
            async static scheduler =>
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
                await Assert.That(navigate).IsEmpty();
                scheduler.Start();
                await Assert.That(navigate).IsNotEmpty();

                fixture.NavigateBack.Execute().Subscribe();
                await Assert.That(navigateBack).IsEmpty();
                scheduler.Start();
                await Assert.That(navigateBack).IsNotEmpty();

                fixture.NavigateAndReset.Execute(new TestViewModel()).Subscribe();
                await Assert.That(navigateAndReset).IsEmpty();
                scheduler.Start();
                await Assert.That(navigateAndReset).IsNotEmpty();
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

                Assert.Throws<Exception>(() => fixture.Navigate.Execute(null!).Subscribe());
            });
}
