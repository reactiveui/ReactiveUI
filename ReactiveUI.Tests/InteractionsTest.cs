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
            var source1 = Interaction.CreateSource();
            Assert.Throws<UnhandledInteractionException>(() => source1.Raise(new Interaction<string>()).FirstAsync().Wait());

            source1.RegisterHandler<Interaction<string>>(_ => { });
            source1.RegisterHandler<Interaction<string>>(_ => { });
            Assert.Throws<UnhandledInteractionException>(() => source1.Raise(new Interaction<string>()).FirstAsync().Wait());

            var source2 = Interaction<bool>.CreateSource();
            Assert.Throws<UnhandledInteractionException>(() => source2.Raise(new Interaction<bool>()).FirstAsync().Wait());

            source2.RegisterHandler(_ => { });
            source2.RegisterHandler(_ => { });
            Assert.Throws<UnhandledInteractionException>(() => source2.Raise(new Interaction<bool>()).FirstAsync().Wait());
        }

        [Fact]
        public void HandledInteractionsShouldNotCauseException()
        {
            var source1 = Interaction.CreateSource();
            source1.RegisterHandler<Interaction<bool>>(interaction => interaction.SetResult(true));

            var interaction1 = new Interaction<bool>();
            Assert.True(source1.Raise(interaction1).FirstAsync().Wait());

            var source2 = Interaction<bool>.CreateSource();
            source2.RegisterHandler(interaction => interaction.SetResult(true));

            var interaction2 = new Interaction<bool>();
            Assert.True(source2.Raise(interaction2).FirstAsync().Wait());
        }

        [Fact]
        public void NestedHandlersAreExecutedInReverseOrderOfSubscription()
        {
            var source1 = Interaction.CreateSource();

            using (source1.RegisterHandler<Interaction<string>>(x => x.SetResult("A"))) {
                Assert.Equal("A", source1.Raise(new Interaction<string>()).FirstAsync().Wait());

                using (source1.RegisterHandler<Interaction<string>>(x => x.SetResult("B"))) {
                    Assert.Equal("B", source1.Raise(new Interaction<string>()).FirstAsync().Wait());

                    using (source1.RegisterHandler<Interaction<string>>(x => x.SetResult("C"))) {
                        Assert.Equal("C", source1.Raise(new Interaction<string>()).FirstAsync().Wait());
                    }

                    Assert.Equal("B", source1.Raise(new Interaction<string>()).FirstAsync().Wait());
                }

                Assert.Equal("A", source1.Raise(new Interaction<string>()).FirstAsync().Wait());
            }

            var source2 = Interaction<string>.CreateSource();

            using (source2.RegisterHandler(x => x.SetResult("A"))) {
                Assert.Equal("A", source2.Raise(new Interaction<string>()).FirstAsync().Wait());

                using (source2.RegisterHandler(x => x.SetResult("B"))) {
                    Assert.Equal("B", source2.Raise(new Interaction<string>()).FirstAsync().Wait());

                    using (source2.RegisterHandler(x => x.SetResult("C"))) {
                        Assert.Equal("C", source2.Raise(new Interaction<string>()).FirstAsync().Wait());
                    }

                    Assert.Equal("B", source2.Raise(new Interaction<string>()).FirstAsync().Wait());
                }

                Assert.Equal("A", source2.Raise(new Interaction<string>()).FirstAsync().Wait());
            }
        }

        [Fact]
        public void HandlersCanOptNotToHandleTheInteraction()
        {
            var source1 = Interaction.CreateSource();

            var handler1A = source1.RegisterHandler<CustomInteraction>(x => x.SetResult("A"));
            var handler1B = source1.RegisterHandler<CustomInteraction>(
                x => {
                    // only handle if the interaction is Super Important
                    if (x.IsSuperImportant) {
                        x.SetResult("B");
                    }
                });
            var handler1C = source1.RegisterHandler<CustomInteraction>(x => x.SetResult("C"));

            using (handler1A) {
                using (handler1B) {
                    using (handler1C) {
                        Assert.Equal("C", source1.Raise(new CustomInteraction(false)).FirstAsync().Wait());
                        Assert.Equal("C", source1.Raise(new CustomInteraction(true)).FirstAsync().Wait());
                    }

                    Assert.Equal("A", source1.Raise(new CustomInteraction(false)).FirstAsync().Wait());
                    Assert.Equal("B", source1.Raise(new CustomInteraction(true)).FirstAsync().Wait());
                }

                Assert.Equal("A", source1.Raise(new CustomInteraction(false)).FirstAsync().Wait());
                Assert.Equal("A", source1.Raise(new CustomInteraction(true)).FirstAsync().Wait());
            }

            var source2 = CustomInteraction.CreateSource();

            var handler2A = source2.RegisterHandler(x => x.SetResult("A"));
            var handler2B = source2.RegisterHandler(
                x => {
                    // only handle if the interaction is Super Important
                    if (x.IsSuperImportant) {
                        x.SetResult("B");
                    }
                });
            var handler2C = source2.RegisterHandler(x => x.SetResult("C"));

            using (handler2A) {
                using (handler2B) {
                    using (handler2C) {
                        Assert.Equal("C", source2.Raise(new CustomInteraction(false)).FirstAsync().Wait());
                        Assert.Equal("C", source2.Raise(new CustomInteraction(true)).FirstAsync().Wait());
                    }

                    Assert.Equal("A", source2.Raise(new CustomInteraction(false)).FirstAsync().Wait());
                    Assert.Equal("B", source2.Raise(new CustomInteraction(true)).FirstAsync().Wait());
                }

                Assert.Equal("A", source2.Raise(new CustomInteraction(false)).FirstAsync().Wait());
                Assert.Equal("A", source2.Raise(new CustomInteraction(true)).FirstAsync().Wait());
            }
        }

        [Fact]
        public void HandlersCanContainAsynchronousCode()
        {
            var scheduler = new TestScheduler();

            var source1 = Interaction.CreateSource();

            // even though handler B is "slow" (i.e. mimicks waiting for the user), it takes precedence over A, so we expect A to never even be called
            var handler1AWasCalled = false;
            var handler1A = source1.RegisterHandler<Interaction<string>>(
                x => {
                    x.SetResult("A");
                    handler1AWasCalled = true;
                });
            var handler1B = source1.RegisterHandler<Interaction<string>>(
                x =>
                    Observable
                        .Return(Unit.Default)
                        .Delay(TimeSpan.FromSeconds(1), scheduler)
                        .Do(_ => x.SetResult("B")));

            using (handler1A)
            using (handler1B) {
                string result = null;
                source1.Raise(new Interaction<string>()).Subscribe(x => result = x);

                Assert.Null(result);
                scheduler.AdvanceBy(TimeSpan.FromSeconds(0.5).Ticks);
                Assert.Null(result);
                scheduler.AdvanceBy(TimeSpan.FromSeconds(0.6).Ticks);
                Assert.Equal("B", result);
            }

            Assert.False(handler1AWasCalled);

            var source2 = Interaction<string>.CreateSource();

            // even though handler B is "slow" (i.e. mimicks waiting for the user), it takes precedence over A, so we expect A to never even be called
            var handler2AWasCalled = false;
            var handler2A = source2.RegisterHandler(
                x => {
                    x.SetResult("A");
                    handler1AWasCalled = true;
                });
            var handler2B = source2.RegisterHandler(
                x =>
                    Observable
                        .Return(Unit.Default)
                        .Delay(TimeSpan.FromSeconds(1), scheduler)
                        .Do(_ => x.SetResult("B")));

            using (handler2A)
            using (handler2B) {
                string result = null;
                source2.Raise(new Interaction<string>()).Subscribe(x => result = x);

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

            public new static InteractionSource<CustomInteraction, string> CreateSource()
            {
                return new InteractionSource<CustomInteraction, string>();
            }
        }
    }
}