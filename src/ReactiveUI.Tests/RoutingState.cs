using System;
using System.Linq;
using System.Reactive.Linq;
using Xunit;

namespace ReactiveUI.Routing.Tests
{
    using System.Threading.Tasks;
    using Microsoft.Reactive.Testing;
    public class TestViewModel : ReactiveObject, IRoutableViewModel
    {
        string _SomeProp;
        public string SomeProp {
            get { return _SomeProp; }
            set { this.RaiseAndSetIfChanged(ref _SomeProp, value); }
        }

        public string UrlPathSegment {
            get { return "Test"; }
        }

        public IScreen HostScreen
        {
            get { return null; }
        }
    }

    public class TestScreen : ReactiveObject, IScreen
    {
        RoutingState _Router;
        public RoutingState Router {
            get { return _Router; }
            set { this.RaiseAndSetIfChanged(ref _Router, value); }
        }
    }

    public class RoutingStateTests
    {
        [Fact]
        public async Task NavigationPushPopTest()
        {
            var input = new TestViewModel() {SomeProp = "Foo"};
            var fixture = new RoutingState();

            Assert.False(await fixture.NavigateBack.CanExecute.FirstAsync());
            await fixture.Navigate.Execute(new TestViewModel());

            Assert.Equal(1, fixture.NavigationStack.Count);
            Assert.False(await fixture.NavigateBack.CanExecute.FirstAsync());

            await fixture.Navigate.Execute(new TestViewModel());

            Assert.Equal(2, fixture.NavigationStack.Count);
            Assert.True(await fixture.NavigateBack.CanExecute.FirstAsync());

            await fixture.NavigateBack.Execute();

            Assert.Equal(1, fixture.NavigationStack.Count);
        }

        [Fact]
        public void CurrentViewModelObservableIsAccurate()
        {
            var fixture = new RoutingState();
            var output = fixture.CurrentViewModel.CreateCollection();

            Assert.Equal(1, output.Count);

            fixture.Navigate.Execute(new TestViewModel() { SomeProp = "A" });
            Assert.Equal(2, output.Count);

            fixture.Navigate.Execute(new TestViewModel() { SomeProp = "B" });
            Assert.Equal(3, output.Count);
            Assert.Equal("B", ((TestViewModel)output.Last()).SomeProp);

            fixture.NavigateBack.Execute();
            Assert.Equal(4, output.Count);
            Assert.Equal("A", ((TestViewModel)output.Last()).SomeProp);
        }

        [Fact]
        public void CurrentViewModelObservableIsAccurateViaWhenAnyObservable()
        {
            var fixture = new TestScreen();
            var output = fixture.WhenAnyObservable(x => x.Router.CurrentViewModel).CreateCollection();
            fixture.Router = new RoutingState();

            Assert.Equal(1, output.Count);

            fixture.Router.Navigate.Execute(new TestViewModel() { SomeProp = "A" });
            Assert.Equal(2, output.Count);

            fixture.Router.Navigate.Execute(new TestViewModel() { SomeProp = "B" });
            Assert.Equal(3, output.Count);
            Assert.Equal("B", ((TestViewModel)output.Last()).SomeProp);

            fixture.Router.NavigateBack.Execute();
            Assert.Equal(4, output.Count);
            Assert.Equal("A", ((TestViewModel)output.Last()).SomeProp);
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
            Assert.True(object.ReferenceEquals(fixture.Router.NavigationStack.First(), viewModel));
        }

        [Fact]
        public void SchedulerIsUsedForAllCommands()
        {
            var scheduler = new TestScheduler();
            var fixture = new RoutingState
            {
                Scheduler = scheduler
            };
            var navigate = fixture
                .Navigate
                .CreateCollection();
            var navigateBack = fixture
                .NavigateBack
                .CreateCollection();
            var navigateAndReset = fixture
                .NavigateAndReset
                .CreateCollection();

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
