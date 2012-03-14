using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReactiveUI.Serialization;
using Xunit;

namespace ReactiveUI.Routing.Tests
{
    public class TestViewModel : ReactiveObject, IRoutableViewModel
    {
        string _SomeProp;
        public string SomeProp {
            get { return _SomeProp; }
            set { this.RaiseAndSetIfChanged(x => x.SomeProp, value); }
        }

        public string FriendlyUrlName {
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
            fixture.NavigateForward.Execute(input);

            Assert.Equal(1, fixture.NavigationStack.Count);
            Assert.True(fixture.NavigateBack.CanExecute(null));

            fixture.NavigateBack.Execute(null);

            Assert.Equal(0, fixture.NavigationStack.Count);
        }
    }
}
