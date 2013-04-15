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
    }
}
