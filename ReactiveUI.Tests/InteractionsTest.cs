using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ReactiveUI.Tests
{
    public class InteractionsTest
    {
        [Fact]
        public void UnhandledGlobalInteractionsShouldCauseException()
        {
            var sut = new UserInteraction<bool>();
            Assert.Throws<UnhandledUserInteractionException>(() => sut.RaiseGlobal().First());
        }

        [Fact]
        public void HandledGlobalInteractionsShouldNotCauseException()
        {
            var sut = new UserInteraction<bool>();

            using (UserInteraction.RegisterGlobalHandler<UserInteraction<bool>>(x => x.SetResult(true)))
            {
                var result = sut.RaiseGlobal().First();
                Assert.True(result);
            }

            Assert.Throws<UnhandledUserInteractionException>(() => sut.RaiseGlobal().First());
        }

        [Fact]
        public void NestedGlobalHandlersAreExecutedInReverseOrderOfSubscription()
        {
            using (UserInteraction.RegisterGlobalHandler<UserInteraction<string>>(x => x.SetResult("A")))
            {
                Assert.Equal("A", new UserInteraction<string>().RaiseGlobal().First());

                using (UserInteraction.RegisterGlobalHandler<UserInteraction<string>>(x => x.SetResult("B")))
                {
                    Assert.Equal("B", new UserInteraction<string>().RaiseGlobal().First());

                    using (UserInteraction.RegisterGlobalHandler<UserInteraction<string>>(x => x.SetResult("C")))
                    {
                        Assert.Equal("C", new UserInteraction<string>().RaiseGlobal().First());
                    }

                    Assert.Equal("B", new UserInteraction<string>().RaiseGlobal().First());
                }

                Assert.Equal("A", new UserInteraction<string>().RaiseGlobal().First());
            }
        }

        [Fact]
        public void GlobalHandlersCanOptNotToHandleTheInteraction()
        {
            var handlerA = UserInteraction.RegisterGlobalHandler<CustomInteraction>(x => x.SetResult("A"));
            var handlerB = UserInteraction.RegisterGlobalHandler<CustomInteraction>(
                x =>
                {
                    // only handle if the interaction is Super Important
                    if (x.IsSuperImportant)
                    {
                        x.SetResult("B");
                    }
                });
            var handlerC = UserInteraction.RegisterGlobalHandler<CustomInteraction>(x => x.SetResult("C"));

            using (handlerA)
            {
                using (handlerB)
                {
                    using (handlerC)
                    {
                        Assert.Equal("C", new CustomInteraction(false).RaiseGlobal().First());
                        Assert.Equal("C", new CustomInteraction(true).RaiseGlobal().First());
                    }

                    Assert.Equal("A", new CustomInteraction(false).RaiseGlobal().First());
                    Assert.Equal("B", new CustomInteraction(true).RaiseGlobal().First());
                }

                Assert.Equal("A", new CustomInteraction(false).RaiseGlobal().First());
                Assert.Equal("A", new CustomInteraction(true).RaiseGlobal().First());
            }
        }

        [Fact]
        public void GlobalHandlersCanContainAsynchronousCode()
        {
            // even though handler B is "slow" (i.e. mimicks waiting for the user), it takes precedence over A, so we expect A to never even be called
            var handlerAWasCalled = false;
            var handlerA = UserInteraction.RegisterGlobalHandler<UserInteraction<string>>(
                x =>
                {
                    x.SetResult("A");
                    handlerAWasCalled = true;
                });
            var handlerB = UserInteraction.RegisterGlobalHandler<UserInteraction<string>>(
                async x =>
                {
                    await Task.Delay(10);
                    x.SetResult("B");
                });

            using (handlerA)
            using (handlerB)
            {
                Assert.Equal("B", new UserInteraction<string>().RaiseGlobal().First());
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