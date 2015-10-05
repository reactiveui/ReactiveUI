using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ReactiveUI.Tests
{
    public class InteractionsTest
    {
        [Fact]
        public void UnhandledPropagatedInteractionsShouldCauseException()
        {
            var sut = new UserInteraction<bool>();
            Assert.Throws<UnhandledUserInteractionException>(() => sut.Propagate().First());
        }

        [Fact]
        public void HandledPropagatedInteractionsShouldNotCauseException()
        {
            var sut = new UserInteraction<bool>();

            using (UserInteraction.RegisterHandler<UserInteraction<bool>>(x => x.SetResult(true)))
            {
                var result = sut.Propagate().First();
                Assert.True(result);
            }

            Assert.Throws<UnhandledUserInteractionException>(() => sut.Propagate().First());
        }

        [Fact]
        public void NestedHandlersAreExecutedInReverseOrderOfSubscription()
        {
            using (UserInteraction.RegisterHandler<UserInteraction<string>>(x => x.SetResult("A")))
            {
                Assert.Equal("A", new UserInteraction<string>().Propagate().First());

                using (UserInteraction.RegisterHandler<UserInteraction<string>>(x => x.SetResult("B")))
                {
                    Assert.Equal("B", new UserInteraction<string>().Propagate().First());

                    using (UserInteraction.RegisterHandler<UserInteraction<string>>(x => x.SetResult("C")))
                    {
                        Assert.Equal("C", new UserInteraction<string>().Propagate().First());
                    }

                    Assert.Equal("B", new UserInteraction<string>().Propagate().First());
                }

                Assert.Equal("A", new UserInteraction<string>().Propagate().First());
            }
        }

        [Fact]
        public void HandlersCanOptNotToHandleTheInteraction()
        {
            var handlerA = UserInteraction.RegisterHandler<CustomInteraction>(x => x.SetResult("A"));
            var handlerB = UserInteraction.RegisterHandler<CustomInteraction>(
                x =>
                {
                    // only handle if the interaction is Super Important
                    if (x.IsSuperImportant)
                    {
                        x.SetResult("B");
                    }
                });
            var handlerC = UserInteraction.RegisterHandler<CustomInteraction>(x => x.SetResult("C"));

            using (handlerA)
            {
                using (handlerB)
                {
                    using (handlerC)
                    {
                        Assert.Equal("C", new CustomInteraction(false).Propagate().First());
                        Assert.Equal("C", new CustomInteraction(true).Propagate().First());
                    }

                    Assert.Equal("A", new CustomInteraction(false).Propagate().First());
                    Assert.Equal("B", new CustomInteraction(true).Propagate().First());
                }

                Assert.Equal("A", new CustomInteraction(false).Propagate().First());
                Assert.Equal("A", new CustomInteraction(true).Propagate().First());
            }
        }

        [Fact]
        public void HandlersCanContainAsynchronousCode()
        {
            // even though handler B is "slow" (i.e. mimicks waiting for the user), it takes precedence over A, so we expect A to never even be called
            var handlerAWasCalled = false;
            var handlerA = UserInteraction.RegisterHandler<UserInteraction<string>>(
                x =>
                {
                    x.SetResult("A");
                    handlerAWasCalled = true;
                });
            var handlerB = UserInteraction.RegisterHandler<UserInteraction<string>>(
                async x =>
                {
                    await Task.Delay(10);
                    x.SetResult("B");
                });

            using (handlerA)
            using (handlerB)
            {
                Assert.Equal("B", new UserInteraction<string>().Propagate().First());
            }

            Assert.False(handlerAWasCalled);
        }

        private class CustomInteraction : UserInteraction<string>
        {
            public CustomInteraction(bool isSuperImportant)
            {
                IsSuperImportant = isSuperImportant;
            }

            public bool IsSuperImportant
            {
                get;
                set;
            }
        }
    }
}