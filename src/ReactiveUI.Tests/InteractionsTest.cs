using System;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Microsoft.Reactive.Testing;
using ReactiveUI.Testing;
using Xunit;

namespace ReactiveUI.Tests
{
    public class InteractionsTest
    {
        [Fact]
        public void RegisterNullHandlerShouldCauseException()
        {
            var interaction = new Interaction<Unit, Unit>();

            Assert.Throws<ArgumentNullException>(() => interaction.RegisterHandler((Action<InteractionContext<Unit, Unit>>)null));
            Assert.Throws<ArgumentNullException>(() => interaction.RegisterHandler((Func<InteractionContext<Unit, Unit>, Task>)null));
            Assert.Throws<ArgumentNullException>(() => interaction.RegisterHandler((Func<InteractionContext<Unit, Unit>, IObservable<Unit>>)null));
        }

        [Fact]
        public void UnhandledInteractionsShouldCauseException()
        {
            var interaction = new Interaction<string, Unit>();
            var ex = Assert.Throws<UnhandledInteractionException<string, Unit>>(() => interaction.Handle("foo").FirstAsync().Wait());
            Assert.Same(interaction, ex.Interaction);
            Assert.Equal("foo", ex.Input);

            interaction.RegisterHandler(_ => { });
            interaction.RegisterHandler(_ => { });
            ex = Assert.Throws<UnhandledInteractionException<string, Unit>>(() => interaction.Handle("bar").FirstAsync().Wait());
            Assert.Same(interaction, ex.Interaction);
            Assert.Equal("bar", ex.Input);
        }

        [Fact]
        public void AttemptingToSetInteractionOutputMoreThanOnceShouldCauseException()
        {
            var interaction = new Interaction<Unit, Unit>();

            interaction.RegisterHandler(context => {
                context.SetOutput(Unit.Default);
                context.SetOutput(Unit.Default);
            });

            var ex = Assert.Throws<InvalidOperationException>(() => interaction.Handle(Unit.Default).Subscribe());
            Assert.Equal("Output has already been set.", ex.Message);
        }

        [Fact]
        public void AttemptingToGetInteractionOutputBeforeItHasBeenSetShouldCauseException()
        {
            var interaction = new Interaction<Unit, Unit>();

            interaction.RegisterHandler(context => {
                var output = context.GetOutput();
            });

            var ex = Assert.Throws<InvalidOperationException>(() => interaction.Handle(Unit.Default).Subscribe());
            Assert.Equal("Output has not been set.", ex.Message);
        }

        [Fact]
        public void HandledInteractionsShouldNotCauseException()
        {
            var interaction = new Interaction<Unit, bool>();
            interaction.RegisterHandler(c => c.SetOutput(true));

            interaction.Handle(Unit.Default).FirstAsync().Wait();
        }

        [Fact]
        public void HandlersAreExecutedOnHandlerScheduler()
        {
            (new TestScheduler()).With(sched => {
                var interaction = new Interaction<Unit, string>(sched);

                using (interaction.RegisterHandler(x => x.SetOutput("done"))) {
                    var handled = false;
                    interaction
                        .Handle(Unit.Default)
                        .Subscribe(_ => handled = true);

                    Assert.False(handled);

                    sched.Start();
                    Assert.True(handled);
                }
            });
        }

        [Fact]
        public void NestedHandlersAreExecutedInReverseOrderOfSubscription()
        {
            var interaction = new Interaction<Unit, string>();

            using (interaction.RegisterHandler(x => x.SetOutput("A"))) {
                Assert.Equal("A", interaction.Handle(Unit.Default).FirstAsync().Wait());

                using (interaction.RegisterHandler(x => x.SetOutput("B"))) {
                    Assert.Equal("B", interaction.Handle(Unit.Default).FirstAsync().Wait());

                    using (interaction.RegisterHandler(x => x.SetOutput("C"))) {
                        Assert.Equal("C", interaction.Handle(Unit.Default).FirstAsync().Wait());
                    }

                    Assert.Equal("B", interaction.Handle(Unit.Default).FirstAsync().Wait());
                }

                Assert.Equal("A", interaction.Handle(Unit.Default).FirstAsync().Wait());
            }
        }

        [Fact]
        public void HandlersCanOptNotToHandleTheInteraction()
        {
            var interaction = new Interaction<bool, string>();

            var handler1A = interaction.RegisterHandler(x => x.SetOutput("A"));
            var handler1B = interaction.RegisterHandler(
                x => {
                    // only handle if the input is true
                    if (x.Input) {
                        x.SetOutput("B");
                    }
                });
            var handler1C = interaction.RegisterHandler(x => x.SetOutput("C"));

            using (handler1A) {
                using (handler1B) {
                    using (handler1C) {
                        Assert.Equal("C", interaction.Handle(false).FirstAsync().Wait());
                        Assert.Equal("C", interaction.Handle(true).FirstAsync().Wait());
                    }

                    Assert.Equal("A", interaction.Handle(false).FirstAsync().Wait());
                    Assert.Equal("B", interaction.Handle(true).FirstAsync().Wait());
                }

                Assert.Equal("A", interaction.Handle(false).FirstAsync().Wait());
                Assert.Equal("A", interaction.Handle(true).FirstAsync().Wait());
            }
        }

        [Fact]
        public void HandlersCanContainAsynchronousCode()
        {
            var scheduler = new TestScheduler();
            var interaction = new Interaction<Unit, string>();

            // even though handler B is "slow" (i.e. mimicks waiting for the user), it takes precedence over A, so we expect A to never even be called
            var handler1AWasCalled = false;
            var handler1A = interaction.RegisterHandler(
                x => {
                    x.SetOutput("A");
                    handler1AWasCalled = true;
                });
            var handler1B = interaction.RegisterHandler(
                x =>
                    Observables
                        .Unit
                        .Delay(TimeSpan.FromSeconds(1), scheduler)
                        .Do(_ => x.SetOutput("B")));

            using (handler1A)
            using (handler1B) {
                var result = interaction
                    .Handle(Unit.Default)
                    .CreateCollection(ImmediateScheduler.Instance);

                Assert.Equal(0, result.Count);
                scheduler.AdvanceBy(TimeSpan.FromSeconds(0.5).Ticks);
                Assert.Equal(0, result.Count);
                scheduler.AdvanceBy(TimeSpan.FromSeconds(0.6).Ticks);
                Assert.Equal(1, result.Count);
                Assert.Equal("B", result[0]);
            }

            Assert.False(handler1AWasCalled);
        }

        [Fact]
        public void HandlersCanContainAsynchronousCodeViaTasks()
        {
            var interaction = new Interaction<Unit, string>();

            interaction.RegisterHandler(context => {
                context.SetOutput("result");
                return Task.FromResult(true);
            });

            string result = null;
            interaction
                .Handle(Unit.Default)
                .Subscribe(r => result = r);
        }

        [Fact]
        public void HandlersReturningObservablesCanReturnAnyKindOfObservable()
        {
            var interaction = new Interaction<Unit, string>();

            var handler = interaction.RegisterHandler(
                x =>
                    Observable
                        .Return(42)
                        .Do(_ => x.SetOutput("result")));

            var result = interaction.Handle(Unit.Default).FirstAsync().Wait();
            Assert.Equal("result", result);
        }
    }
}