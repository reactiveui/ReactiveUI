using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace ReactiveUI.Routing.Tests
{
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
        IRoutingState _Router;
        public IRoutingState Router {
            get { return _Router; }
            set { this.RaiseAndSetIfChanged(ref _Router, value); }
        }
    }

    public class RoutingStateTests
    {
        [Fact]
        public void NavigationPushPopTest()
        {
            var input = new TestViewModel() {SomeProp = "Foo"};
            var fixture = new RoutingState();

            Assert.False(fixture.NavigateBack.CanExecute(input));
            fixture.Navigate.Execute(new TestViewModel());

            Assert.Equal(1, fixture.NavigationStack.Count);
            Assert.False(fixture.NavigateBack.CanExecute(null));

            fixture.Navigate.Execute(new TestViewModel());

            Assert.Equal(2, fixture.NavigationStack.Count);
            Assert.True(fixture.NavigateBack.CanExecute(null));

            fixture.NavigateBack.Execute(null);

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

            fixture.NavigateBack.Execute(null);
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

            fixture.Router.NavigateBack.Execute(null);
            Assert.Equal(4, output.Count);
            Assert.Equal("A", ((TestViewModel)output.Last()).SomeProp);
        }
    }
}
