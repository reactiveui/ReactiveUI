using System;
using System.Linq;
using System.Reactive.Linq;
using Xunit;

namespace ReactiveUI.Tests
{
    public class InteractionsTest
    {
        [Fact]
        public void UnhandledInteractionsShouldDie()
        {
            var sut = new UserInteraction<bool>();
            Assert.Throws<UnhandledUserInteractionException>(() => sut.Propagate().First());
        }

        [Fact]
        public void HandledUserInteractionsShouldNotThrow()
        {
            var sut = new UserInteraction<bool>();

            using (UserInteraction.PropagatedInteractions.OfType<UserInteraction<bool>>().Subscribe(x => x.SetResult(true)))
            {
                var result = sut.Propagate().First();
                Assert.True(result);
            }

            Assert.Throws<UnhandledUserInteractionException>(() => sut.Propagate().First());
        }

        [Fact]
        public void NestedHandlersAreExecutedInReverseOrderOfSubscription()
        {
            var interactions = UserInteraction
                .PropagatedInteractions
                .OfType<UserInteraction<string>>();

            using (interactions.Subscribe(x => x.SetResult("A")))
            {
                Assert.Equal("A", new UserInteraction<string>().Propagate().First());

                using (interactions.Subscribe(x => x.SetResult("B")))
                {
                    Assert.Equal("B", new UserInteraction<string>().Propagate().First());

                    using (interactions.Subscribe(x => x.SetResult("C")))
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
            var interactions = UserInteraction
                .PropagatedInteractions
                .OfType<CustomInteraction>();
            var handlerA = interactions
                .Subscribe(x => x.SetResult("A"));
            var handlerB = interactions
                .Subscribe(
                    x =>
                    {
                        // only handle if the interaction is Super Important
                        if (x.IsSuperImportant)
                        {
                            x.SetResult("B");
                        }
                    });
            var handlerC = interactions
                .Subscribe(x => x.SetResult("C"));

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