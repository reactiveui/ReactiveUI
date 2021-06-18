// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading.Tasks;
using DynamicData;
using Microsoft.Reactive.Testing;
using Xunit;

namespace ReactiveUI.Tests
{
    public class RoutingStateTests
    {
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

            var navigatedTo = await fixture.NavigateBack.Execute();
            Assert.Equal(navigatedTo.GetType(), input.GetType());
            Assert.Equal(1, fixture.NavigationStack.Count);
        }

        [Fact]
        public async Task CurrentViewModelObservableIsAccurate()
        {
            var fixture = new RoutingState(RxApp.MainThreadScheduler);
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

        [Fact]
        public void NavigateAndResetCheckNavigationStack()
        {
            var fixture = new TestScreen();
            fixture.Router = new RoutingState();
            var viewModel = new TestViewModel();

            Assert.False(fixture.Router.NavigationStack.Any());

            fixture.Router.NavigateAndReset.Execute(viewModel);

            Assert.True(fixture.Router.NavigationStack.Count == 1);
            Assert.True(ReferenceEquals(fixture.Router.NavigationStack.First(), viewModel));
        }

        [Fact]
        public void SchedulerIsUsedForAllCommands()
        {
            var scheduler = new TestScheduler();
            var fixture = new RoutingState
            {
                Scheduler = scheduler
            };

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
        }
    }
}
