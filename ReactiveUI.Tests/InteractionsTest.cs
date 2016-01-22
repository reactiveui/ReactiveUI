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
            var broker1 = Interaction.CreateBroker();
            Assert.Throws<UnhandledInteractionException>(() => broker1.Raise(new Interaction<string>()).FirstAsync().Wait());

            broker1.RegisterHandler<Interaction<string>>(_ => { });
            broker1.RegisterHandler<Interaction<string>>(_ => { });
            Assert.Throws<UnhandledInteractionException>(() => broker1.Raise(new Interaction<string>()).FirstAsync().Wait());

            var broker2 = Interaction<bool>.CreateBroker();
            Assert.Throws<UnhandledInteractionException>(() => broker2.Raise(new Interaction<bool>()).FirstAsync().Wait());

            broker2.RegisterHandler(_ => { });
            broker2.RegisterHandler(_ => { });
            Assert.Throws<UnhandledInteractionException>(() => broker2.Raise(new Interaction<bool>()).FirstAsync().Wait());
        }

        [Fact]
        public void HandledInteractionsShouldNotCauseException()
        {
            var broker1 = Interaction.CreateBroker();
            broker1.RegisterHandler<Interaction<bool>>(interaction => interaction.SetResult(true));

            var interaction1 = new Interaction<bool>();
            Assert.True(broker1.Raise(interaction1).FirstAsync().Wait());

            var broker2 = Interaction<bool>.CreateBroker();
            broker2.RegisterHandler(interaction => interaction.SetResult(true));

            var interaction2 = new Interaction<bool>();
            Assert.True(broker2.Raise(interaction2).FirstAsync().Wait());
        }

        [Fact]
        public void NestedHandlersAreExecutedInReverseOrderOfSubscription()
        {
            var broker1 = Interaction.CreateBroker();

            using (broker1.RegisterHandler<Interaction<string>>(x => x.SetResult("A"))) {
                Assert.Equal("A", broker1.Raise(new Interaction<string>()).FirstAsync().Wait());

                using (broker1.RegisterHandler<Interaction<string>>(x => x.SetResult("B"))) {
                    Assert.Equal("B", broker1.Raise(new Interaction<string>()).FirstAsync().Wait());

                    using (broker1.RegisterHandler<Interaction<string>>(x => x.SetResult("C"))) {
                        Assert.Equal("C", broker1.Raise(new Interaction<string>()).FirstAsync().Wait());
                    }

                    Assert.Equal("B", broker1.Raise(new Interaction<string>()).FirstAsync().Wait());
                }

                Assert.Equal("A", broker1.Raise(new Interaction<string>()).FirstAsync().Wait());
            }

            var broker2 = Interaction<string>.CreateBroker();

            using (broker2.RegisterHandler(x => x.SetResult("A"))) {
                Assert.Equal("A", broker2.Raise(new Interaction<string>()).FirstAsync().Wait());

                using (broker2.RegisterHandler(x => x.SetResult("B"))) {
                    Assert.Equal("B", broker2.Raise(new Interaction<string>()).FirstAsync().Wait());

                    using (broker2.RegisterHandler(x => x.SetResult("C"))) {
                        Assert.Equal("C", broker2.Raise(new Interaction<string>()).FirstAsync().Wait());
                    }

                    Assert.Equal("B", broker2.Raise(new Interaction<string>()).FirstAsync().Wait());
                }

                Assert.Equal("A", broker2.Raise(new Interaction<string>()).FirstAsync().Wait());
            }
        }

        [Fact]
        public void HandlersCanOptNotToHandleTheInteraction()
        {
            var broker1 = Interaction.CreateBroker();

            var handler1A = broker1.RegisterHandler<CustomInteraction>(x => x.SetResult("A"));
            var handler1B = broker1.RegisterHandler<CustomInteraction>(
                x => {
                    // only handle if the interaction is Super Important
                    if (x.IsSuperImportant) {
                        x.SetResult("B");
                    }
                });
            var handler1C = broker1.RegisterHandler<CustomInteraction>(x => x.SetResult("C"));

            using (handler1A) {
                using (handler1B) {
                    using (handler1C) {
                        Assert.Equal("C", broker1.Raise(new CustomInteraction(false)).FirstAsync().Wait());
                        Assert.Equal("C", broker1.Raise(new CustomInteraction(true)).FirstAsync().Wait());
                    }

                    Assert.Equal("A", broker1.Raise(new CustomInteraction(false)).FirstAsync().Wait());
                    Assert.Equal("B", broker1.Raise(new CustomInteraction(true)).FirstAsync().Wait());
                }

                Assert.Equal("A", broker1.Raise(new CustomInteraction(false)).FirstAsync().Wait());
                Assert.Equal("A", broker1.Raise(new CustomInteraction(true)).FirstAsync().Wait());
            }

            var broker2 = CustomInteraction.CreateBroker();

            var handler2A = broker2.RegisterHandler(x => x.SetResult("A"));
            var handler2B = broker2.RegisterHandler(
                x => {
                    // only handle if the interaction is Super Important
                    if (x.IsSuperImportant) {
                        x.SetResult("B");
                    }
                });
            var handler2C = broker2.RegisterHandler(x => x.SetResult("C"));

            using (handler2A) {
                using (handler2B) {
                    using (handler2C) {
                        Assert.Equal("C", broker2.Raise(new CustomInteraction(false)).FirstAsync().Wait());
                        Assert.Equal("C", broker2.Raise(new CustomInteraction(true)).FirstAsync().Wait());
                    }

                    Assert.Equal("A", broker2.Raise(new CustomInteraction(false)).FirstAsync().Wait());
                    Assert.Equal("B", broker2.Raise(new CustomInteraction(true)).FirstAsync().Wait());
                }

                Assert.Equal("A", broker2.Raise(new CustomInteraction(false)).FirstAsync().Wait());
                Assert.Equal("A", broker2.Raise(new CustomInteraction(true)).FirstAsync().Wait());
            }
        }

        [Fact]
        public void HandlersCanContainAsynchronousCode()
        {
            var scheduler = new TestScheduler();

            var broker1 = Interaction.CreateBroker();

            // even though handler B is "slow" (i.e. mimicks waiting for the user), it takes precedence over A, so we expect A to never even be called
            var handler1AWasCalled = false;
            var handler1A = broker1.RegisterHandler<Interaction<string>>(
                x => {
                    x.SetResult("A");
                    handler1AWasCalled = true;
                });
            var handler1B = broker1.RegisterHandler<Interaction<string>>(
                x =>
                    Observable
                        .Return(Unit.Default)
                        .Delay(TimeSpan.FromSeconds(1), scheduler)
                        .Do(_ => x.SetResult("B")));

            using (handler1A)
            using (handler1B) {
                string result = null;
                broker1.Raise(new Interaction<string>()).Subscribe(x => result = x);

                Assert.Null(result);
                scheduler.AdvanceBy(TimeSpan.FromSeconds(0.5).Ticks);
                Assert.Null(result);
                scheduler.AdvanceBy(TimeSpan.FromSeconds(0.6).Ticks);
                Assert.Equal("B", result);
            }

            Assert.False(handler1AWasCalled);

            var broker2 = Interaction<string>.CreateBroker();

            // even though handler B is "slow" (i.e. mimicks waiting for the user), it takes precedence over A, so we expect A to never even be called
            var handler2AWasCalled = false;
            var handler2A = broker2.RegisterHandler(
                x => {
                    x.SetResult("A");
                    handler1AWasCalled = true;
                });
            var handler2B = broker2.RegisterHandler(
                x =>
                    Observable
                        .Return(Unit.Default)
                        .Delay(TimeSpan.FromSeconds(1), scheduler)
                        .Do(_ => x.SetResult("B")));

            using (handler2A)
            using (handler2B) {
                string result = null;
                broker2.Raise(new Interaction<string>()).Subscribe(x => result = x);

                Assert.Null(result);
                scheduler.AdvanceBy(TimeSpan.FromSeconds(0.5).Ticks);
                Assert.Null(result);
                scheduler.AdvanceBy(TimeSpan.FromSeconds(0.6).Ticks);
                Assert.Equal("B", result);
            }

            Assert.False(handler2AWasCalled);
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

            public new static InteractionBroker<CustomInteraction, string> CreateBroker()
            {
                return new InteractionBroker<CustomInteraction, string>();
            }
        }
    }
}