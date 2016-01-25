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
            var interaction = new Interaction<InteractionData<string>>();
            Assert.Throws<UnhandledInteractionException<InteractionData<string>>>(() => interaction.Handle(new InteractionData<string>()).FirstAsync().Wait());

            interaction.RegisterHandler(_ => { });
            interaction.RegisterHandler(_ => { });
            Assert.Throws<UnhandledInteractionException<InteractionData<string>>>(() => interaction.Handle(new InteractionData<string>()).FirstAsync().Wait());
        }

        [Fact]
        public void HandledInteractionsShouldNotCauseException()
        {
            var interaction = new Interaction<InteractionData<bool>>();
            interaction.RegisterHandler(i => i.SetResult(true));

            var data = new InteractionData<bool>();
            interaction.Handle(data).FirstAsync().Wait();
            Assert.True(data.GetResult());
        }

        [Fact]
        public void HandlersCanBeRegisteredForSubclassesOfTheInteractionType()
        {
            var interaction = new Interaction<InteractionData<string>>();
            interaction.RegisterHandler(i => i.SetResult("A"));
            interaction.RegisterHandler<CustomInteraction>(i => i.SetResult("B"));

            var data1 = new InteractionData<string>();
            var data2 = new CustomInteraction(false);

            interaction.Handle(data1).FirstAsync().Wait();
            interaction.Handle(data2).FirstAsync().Wait();

            Assert.Equal("A", data1.GetResult());
            Assert.Equal("B", data2.GetResult());
        }

        [Fact]
        public void NestedHandlersAreExecutedInReverseOrderOfSubscription()
        {
            var interaction = new Interaction<InteractionData<string>>();
            InteractionData<string> data;

            using (interaction.RegisterHandler(x => x.SetResult("A"))) {
                data = new InteractionData<string>();
                interaction.Handle(data).FirstAsync().Wait();
                Assert.Equal("A", data.GetResult());

                using (interaction.RegisterHandler(x => x.SetResult("B"))) {
                    data = new InteractionData<string>();
                    interaction.Handle(data).FirstAsync().Wait();
                    Assert.Equal("B", data.GetResult());

                    using (interaction.RegisterHandler(x => x.SetResult("C"))) {
                        data = new InteractionData<string>();
                        interaction.Handle(data).FirstAsync().Wait();
                        Assert.Equal("C", data.GetResult());
                    }

                    data = new InteractionData<string>();
                    interaction.Handle(data).FirstAsync().Wait();
                    Assert.Equal("B", data.GetResult());
                }

                data = new InteractionData<string>();
                interaction.Handle(data).FirstAsync().Wait();
                Assert.Equal("A", data.GetResult());
            }
        }

        [Fact]
        public void HandlersCanOptNotToHandleTheInteraction()
        {
            var interaction = new Interaction<CustomInteraction>();

            var handler1A = interaction.RegisterHandler(x => x.SetResult("A"));
            var handler1B = interaction.RegisterHandler(
                x => {
                    // only handle if the interaction is Super Important
                    if (x.IsSuperImportant) {
                        x.SetResult("B");
                    }
                });
            var handler1C = interaction.RegisterHandler(x => x.SetResult("C"));
            CustomInteraction data;

            using (handler1A) {
                using (handler1B) {
                    using (handler1C) {
                        data = new CustomInteraction(false);
                        interaction.Handle(data).FirstAsync().Wait();
                        Assert.Equal("C", data.GetResult());
                        data = new CustomInteraction(true);
                        interaction.Handle(data).FirstAsync().Wait();
                        Assert.Equal("C", data.GetResult());
                    }

                    data = new CustomInteraction(false);
                    interaction.Handle(data).FirstAsync().Wait();
                    Assert.Equal("A", data.GetResult());
                    data = new CustomInteraction(true);
                    interaction.Handle(data).FirstAsync().Wait();
                    Assert.Equal("B", data.GetResult());
                }

                data = new CustomInteraction(false);
                interaction.Handle(data).FirstAsync().Wait();
                Assert.Equal("A", data.GetResult());
                data = new CustomInteraction(true);
                interaction.Handle(data).FirstAsync().Wait();
                Assert.Equal("A", data.GetResult());
            }
        }

        [Fact]
        public void HandlersCanContainAsynchronousCode()
        {
            var scheduler = new TestScheduler();
            var interaction = new Interaction<InteractionData<string>>();

            // even though handler B is "slow" (i.e. mimicks waiting for the user), it takes precedence over A, so we expect A to never even be called
            var handler1AWasCalled = false;
            var handler1A = interaction.RegisterHandler(
                x => {
                    x.SetResult("A");
                    handler1AWasCalled = true;
                });
            var handler1B = interaction.RegisterHandler(
                x =>
                    Observable
                        .Return(Unit.Default)
                        .Delay(TimeSpan.FromSeconds(1), scheduler)
                        .Do(_ => x.SetResult("B")));

            using (handler1A)
            using (handler1B) {
                var data = new InteractionData<string>();
                interaction.Handle(data).Subscribe();

                Assert.Throws<InvalidOperationException>(() => data.GetResult());
                scheduler.AdvanceBy(TimeSpan.FromSeconds(0.5).Ticks);
                Assert.Throws<InvalidOperationException>(() => data.GetResult());
                scheduler.AdvanceBy(TimeSpan.FromSeconds(0.6).Ticks);
                Assert.Equal("B", data.GetResult());
            }

            Assert.False(handler1AWasCalled);
        }

        private class CustomInteraction : InteractionData<string>
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