using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReactiveUI.Serialization;
using Xunit;

namespace ReactiveUI.Routing.Tests
{
    public class TestViewModel : ModelBase, IRoutableViewModel
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
        public void RoutingStateSerializableRoundTripTest()
        {
            var engine = new DictionaryStorageEngine();

            using(var _ = engine.AsPrimaryEngine()) {
                var input = new TestViewModel() {SomeProp = "Foo"};
                var fixture = new RoutingState();
                fixture.NavigationStack.Add(input);

                RxStorage.Engine.CreateSyncPoint(fixture);

                var output = RxStorage.Engine.Load<RoutingState>(fixture.ContentHash);

                Assert.True(output.NavigationStack.Count == 1);
                Assert.True(output.NavigationStack[0].ContentHash == input.ContentHash);
                Assert.Equal(input.SomeProp, ((TestViewModel) output.NavigationStack[0]).SomeProp);
            }
        }

        [Fact]
        public void NavigationPushPopTest()
        {
            var input = new TestViewModel() {SomeProp = "Foo"};
            var fixture = new RoutingState();

            Assert.False(fixture.NavigateBack.CanExecute(input));
            fixture.NavigateForward.Execute(input);

            Assert.Equal(1, fixture.NavigationStack.Count);
            Assert.Equal(input.ContentHash, fixture.NavigationStack[0].ContentHash);
            Assert.True(fixture.NavigateBack.CanExecute(null));

            fixture.NavigateBack.Execute(null);

            Assert.Equal(0, fixture.NavigationStack.Count);
        }

        [Fact]
        public void NavigationPushPopSerializationTest()
        {
            var engine = new DictionaryStorageEngine();

            using (var _ = engine.AsPrimaryEngine()) {
                var input = new TestViewModel() {SomeProp = "Foo"};
                var fixture = new RoutingState();
                fixture.NavigateForward.Execute(input);

                RxStorage.Engine.CreateSyncPoint(fixture);

                var output = RxStorage.Engine.GetLatestRootObject<RoutingState>();

                Assert.True(output.NavigateBack.CanExecute(null));

                output.NavigateBack.Execute(null);
                RxStorage.Engine.CreateSyncPoint(output);
                output = RxStorage.Engine.GetLatestRootObject<RoutingState>();

                Assert.False(output.NavigateBack.CanExecute(null));
            }
        }
    }
}
