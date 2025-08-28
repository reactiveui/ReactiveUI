// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using DynamicData;

using Microsoft.Reactive.Testing;

using ReactiveUI.Testing;

namespace ReactiveUI.Tests;

/// <summary>
/// Tests interactions.
/// </summary>
public class InteractionsTest
{
    /// <summary>
    /// Tests that registers null handler should cause exception.
    /// </summary>
    [Fact]
    public void RegisterNullHandlerShouldCauseException()
    {
        var interaction = new Interaction<Unit, Unit>();

        Assert.Throws<ArgumentNullException>(() => interaction.RegisterHandler((Action<IInteractionContext<Unit, Unit>>)null!));
        Assert.Throws<ArgumentNullException>(() => interaction.RegisterHandler(null!));
        Assert.Throws<ArgumentNullException>(() => interaction.RegisterHandler((Func<IInteractionContext<Unit, Unit>, IObservable<Unit>>)null!));
    }

    /// <summary>
    /// Tests that unhandled interactions should cause exception.
    /// </summary>
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

    /// <summary>
    /// Test that attempting to set interaction output more than once should cause exception.
    /// </summary>
    [Fact]
    public void AttemptingToSetInteractionOutputMoreThanOnceShouldCauseException()
    {
        var interaction = new Interaction<Unit, Unit>();

        interaction.RegisterHandler(context =>
        {
            context.SetOutput(Unit.Default);
            context.SetOutput(Unit.Default);
        });

        var ex = Assert.Throws<InvalidOperationException>(() => interaction.Handle(Unit.Default).Subscribe());
        Assert.Equal("Output has already been set.", ex.Message);
    }

    /// <summary>
    /// Test that attempting to get interaction output before it has been set should cause exception.
    /// </summary>
    [Fact]
    public void AttemptingToGetInteractionOutputBeforeItHasBeenSetShouldCauseException()
    {
        var interaction = new Interaction<Unit, Unit>();

        interaction.RegisterHandler(context =>
        {
            var output = ((InteractionContext<Unit, Unit>)context).GetOutput();
        });

        var ex = Assert.Throws<InvalidOperationException>(() => interaction.Handle(Unit.Default).Subscribe());
        Assert.Equal("Output has not been set.", ex.Message);
    }

    /// <summary>
    /// Tests that Handled interactions should not cause exception.
    /// </summary>
    [Fact]
    public void HandledInteractionsShouldNotCauseException()
    {
        var interaction = new Interaction<Unit, bool>();
        interaction.RegisterHandler(c => c.SetOutput(true));

        interaction.Handle(Unit.Default).FirstAsync().Wait();
    }

    /// <summary>
    /// Tests that Handlers are executed on handler scheduler.
    /// </summary>
    [Fact]
    public void HandlersAreExecutedOnHandlerScheduler() =>
        new TestScheduler().With(scheduler =>
        {
            var interaction = new Interaction<Unit, string>(scheduler);

            using (interaction.RegisterHandler(x => x.SetOutput("done")))
            {
                var handled = false;
                interaction
                    .Handle(Unit.Default)
                    .Subscribe(_ => handled = true);

                Assert.False(handled);

                scheduler.Start();
                Assert.True(handled);
            }
        });

    /// <summary>
    /// Test that Nested handlers are executed in reverse order of subscription.
    /// </summary>
    [Fact]
    public void NestedHandlersAreExecutedInReverseOrderOfSubscription()
    {
        var interaction = new Interaction<Unit, string>();

        using (interaction.RegisterHandler(x => x.SetOutput("A")))
        {
            Assert.Equal("A", interaction.Handle(Unit.Default).FirstAsync().Wait());

            using (interaction.RegisterHandler(x => x.SetOutput("B")))
            {
                Assert.Equal("B", interaction.Handle(Unit.Default).FirstAsync().Wait());

                using (interaction.RegisterHandler(x => x.SetOutput("C")))
                {
                    Assert.Equal("C", interaction.Handle(Unit.Default).FirstAsync().Wait());
                }

                Assert.Equal("B", interaction.Handle(Unit.Default).FirstAsync().Wait());
            }

            Assert.Equal("A", interaction.Handle(Unit.Default).FirstAsync().Wait());
        }
    }

    /// <summary>
    /// Tests that handlers can opt not to handle the interaction.
    /// </summary>
    [Fact]
    public void HandlersCanOptNotToHandleTheInteraction()
    {
        var interaction = new Interaction<bool, string>();

        var handler1A = interaction.RegisterHandler(x => x.SetOutput("A"));
        var handler1B = interaction.RegisterHandler(
            x =>
            {
                // only handle if the input is true
                if (x.Input)
                {
                    x.SetOutput("B");
                }
            });
        var handler1C = interaction.RegisterHandler(x => x.SetOutput("C"));

        using (handler1A)
        {
            using (handler1B)
            {
                using (handler1C)
                {
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
    public void UnhandledInteractionExceptionTests()
    {
        var uie = new UnhandledInteractionException<Unit, string>();
        Assert.NotNull(uie);
#pragma warning disable SYSLIB0051 // Type or member is obsolete
#pragma warning disable SYSLIB0050 // Type or member is obsolete
        uie.GetObjectData(new(typeof(string), new System.Runtime.Serialization.FormatterConverter()), default);
#pragma warning restore SYSLIB0050 // Type or member is obsolete
        var uieme = new UnhandledInteractionException<Unit, string>("exception", new Exception("inner exception"));
        Assert.NotNull(uieme);
        Assert.Throws<ArgumentNullException>(() => uieme.GetObjectData(default!, default));
#pragma warning restore SYSLIB0051 // Type or member is obsolete
    }

    /// <summary>
    /// Test that handlers can contain asynchronous code.
    /// </summary>
    [Fact]
    public void HandlersCanContainAsynchronousCode()
    {
        var scheduler = new TestScheduler();
        var interaction = new Interaction<Unit, string>();

        // even though handler B is "slow" (i.e. mimicks waiting for the user), it takes precedence over A, so we expect A to never even be called
        var handler1AWasCalled = false;
        var handler1A = interaction.RegisterHandler(
            x =>
            {
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
        using (handler1B)
        {
            interaction
                .Handle(Unit.Default)
                .ToObservableChangeSet(scheduler: ImmediateScheduler.Instance).Bind(out var result).Subscribe();

            Assert.Equal(0, result.Count);
            scheduler.AdvanceBy(TimeSpan.FromSeconds(0.5).Ticks);
            Assert.Equal(0, result.Count);
            scheduler.AdvanceBy(TimeSpan.FromSeconds(0.6).Ticks);
            Assert.Equal(1, result.Count);
            Assert.Equal("B", result[0]);
        }

        Assert.False(handler1AWasCalled);
    }

    /// <summary>
    /// Test that handlers can contain asynchronous code via tasks.
    /// </summary>
    [Fact]
    public void HandlersCanContainAsynchronousCodeViaTasks()
    {
        var interaction = new Interaction<Unit, string>();

        interaction.RegisterHandler(context =>
        {
            context.SetOutput("result");
            return Task.FromResult(true);
        });

        string? result = null;
        interaction
            .Handle(Unit.Default)
            .Subscribe(r => result = r);
    }

    /// <summary>
    /// Tests that handlers returning observables can return any kind of observable.
    /// </summary>
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
