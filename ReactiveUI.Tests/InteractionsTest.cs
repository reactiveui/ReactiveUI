using System;
using System.Reactive;
using System.Reactive.Linq;
using Microsoft.Reactive.Testing;
using Xunit;

namespace ReactiveUI.Tests
{
    public class InteractionsTest
    {
        [Fact]
        public void UnhandledInteractionsShouldCauseException()
        {
            var broker = new InteractionBroker<Interaction<string>>();
            Assert.Throws<UnhandledInteractionException>(() => broker.Raise(new Interaction<string>()).FirstAsync().Wait());

            broker.RegisterHandler(_ => { });
            broker.RegisterHandler(_ => { });
            Assert.Throws<UnhandledInteractionException>(() => broker.Raise(new Interaction<string>()).FirstAsync().Wait());
        }

        [Fact]
        public void HandledInteractionsShouldNotCauseException()
        {
            var broker = new InteractionBroker<Interaction<bool>>();
            broker.RegisterHandler(i => i.SetResult(true));

            var interaction = new Interaction<bool>();
            broker.Raise(interaction).FirstAsync().Wait();
            Assert.True(interaction.GetResult());
        }

        [Fact]
        public void HandlersCanBeRegisteredForSubclassesOfTheInteractionType()
        {
            var broker = new InteractionBroker<Interaction<string>>();
            broker.RegisterHandler(i => i.SetResult("A"));
            broker.RegisterHandler<CustomInteraction>(i => i.SetResult("B"));

            var interaction1 = new Interaction<string>();
            var interaction2 = new CustomInteraction(false);

            broker.Raise(interaction1).FirstAsync().Wait();
            broker.Raise(interaction2).FirstAsync().Wait();

            Assert.Equal("A", interaction1.GetResult());
            Assert.Equal("B", interaction2.GetResult());
        }

        [Fact]
        public void NestedHandlersAreExecutedInReverseOrderOfSubscription()
        {
            var broker = new InteractionBroker<Interaction<string>>();
            Interaction<string> interaction;

            using (broker.RegisterHandler(x => x.SetResult("A"))) {
                interaction = new Interaction<string>();
                broker.Raise(interaction).FirstAsync().Wait();
                Assert.Equal("A", interaction.GetResult());

                using (broker.RegisterHandler(x => x.SetResult("B"))) {
                    interaction = new Interaction<string>();
                    broker.Raise(interaction).FirstAsync().Wait();
                    Assert.Equal("B", interaction.GetResult());

                    using (broker.RegisterHandler(x => x.SetResult("C"))) {
                        interaction = new Interaction<string>();
                        broker.Raise(interaction).FirstAsync().Wait();
                        Assert.Equal("C", interaction.GetResult());
                    }

                    interaction = new Interaction<string>();
                    broker.Raise(interaction).FirstAsync().Wait();
                    Assert.Equal("B", interaction.GetResult());
                }

                interaction = new Interaction<string>();
                broker.Raise(interaction).FirstAsync().Wait();
                Assert.Equal("A", interaction.GetResult());
            }
        }

        [Fact]
        public void HandlersCanOptNotToHandleTheInteraction()
        {
            var broker = new InteractionBroker<CustomInteraction>();

            var handler1A = broker.RegisterHandler(x => x.SetResult("A"));
            var handler1B = broker.RegisterHandler(
                x => {
                    // only handle if the interaction is Super Important
                    if (x.IsSuperImportant) {
                        x.SetResult("B");
                    }
                });
            var handler1C = broker.RegisterHandler(x => x.SetResult("C"));
            CustomInteraction interaction;

            using (handler1A) {
                using (handler1B) {
                    using (handler1C) {
                        interaction = new CustomInteraction(false);
                        broker.Raise(interaction).FirstAsync().Wait();
                        Assert.Equal("C", interaction.GetResult());
                        interaction = new CustomInteraction(true);
                        broker.Raise(interaction).FirstAsync().Wait();
                        Assert.Equal("C", interaction.GetResult());
                    }

                    interaction = new CustomInteraction(false);
                    broker.Raise(interaction).FirstAsync().Wait();
                    Assert.Equal("A", interaction.GetResult());
                    interaction = new CustomInteraction(true);
                    broker.Raise(interaction).FirstAsync().Wait();
                    Assert.Equal("B", interaction.GetResult());
                }

                interaction = new CustomInteraction(false);
                broker.Raise(interaction).FirstAsync().Wait();
                Assert.Equal("A", interaction.GetResult());
                interaction = new CustomInteraction(true);
                broker.Raise(interaction).FirstAsync().Wait();
                Assert.Equal("A", interaction.GetResult());
            }
        }

        [Fact]
        public void HandlersCanContainAsynchronousCode()
        {
            var scheduler = new TestScheduler();
            var broker = new InteractionBroker<Interaction<string>>();

            // even though handler B is "slow" (i.e. mimicks waiting for the user), it takes precedence over A, so we expect A to never even be called
            var handler1AWasCalled = false;
            var handler1A = broker.RegisterHandler(
                x => {
                    x.SetResult("A");
                    handler1AWasCalled = true;
                });
            var handler1B = broker.RegisterHandler(
                x =>
                    Observable
                        .Return(Unit.Default)
                        .Delay(TimeSpan.FromSeconds(1), scheduler)
                        .Do(_ => x.SetResult("B")));

            using (handler1A)
            using (handler1B) {
                var interaction = new Interaction<string>();
                broker.Raise(interaction).Subscribe();

                Assert.Throws<InvalidOperationException>(() => interaction.GetResult());
                scheduler.AdvanceBy(TimeSpan.FromSeconds(0.5).Ticks);
                Assert.Throws<InvalidOperationException>(() => interaction.GetResult());
                scheduler.AdvanceBy(TimeSpan.FromSeconds(0.6).Ticks);
                Assert.Equal("B", interaction.GetResult());
            }

            Assert.False(handler1AWasCalled);
        }

        private class CustomInteraction : Interaction<string>
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