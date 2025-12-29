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
    [Test]
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
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task UnhandledInteractionsShouldCauseException()
    {
        var interaction = new Interaction<string, Unit>();
        var ex = Assert.Throws<UnhandledInteractionException<string, Unit>>(() => interaction.Handle("foo").FirstAsync().Wait());
        using (Assert.Multiple())
        {
            await Assert.That(ex.Interaction).IsSameReferenceAs(interaction);
            await Assert.That(ex.Input).IsEqualTo("foo");
        }

        interaction.RegisterHandler(_ => { });
        interaction.RegisterHandler(_ => { });
        ex = Assert.Throws<UnhandledInteractionException<string, Unit>>(() => interaction.Handle("bar").FirstAsync().Wait());
        using (Assert.Multiple())
        {
            await Assert.That(ex.Interaction).IsSameReferenceAs(interaction);
            await Assert.That(ex.Input).IsEqualTo("bar");
        }
    }

    /// <summary>
    /// Test that attempting to set interaction output more than once should cause exception.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task AttemptingToSetInteractionOutputMoreThanOnceShouldCauseException()
    {
        var interaction = new Interaction<Unit, Unit>();

        interaction.RegisterHandler(context =>
        {
            context.SetOutput(Unit.Default);
            context.SetOutput(Unit.Default);
        });

        var ex = Assert.Throws<InvalidOperationException>(() => interaction.Handle(Unit.Default).Subscribe());
        await Assert.That(ex.Message).IsEqualTo("Output has already been set.");
    }

    /// <summary>
    /// Test that attempting to get interaction output before it has been set should cause exception.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task AttemptingToGetInteractionOutputBeforeItHasBeenSetShouldCauseException()
    {
        var interaction = new Interaction<Unit, Unit>();

        interaction.RegisterHandler(context =>
        {
            var output = ((InteractionContext<Unit, Unit>)context).GetOutput();
        });

        var ex = Assert.Throws<InvalidOperationException>(() => interaction.Handle(Unit.Default).Subscribe());
        await Assert.That(ex.Message).IsEqualTo("Output has not been set.");
    }

    /// <summary>
    /// Tests that Handled interactions should not cause exception.
    /// </summary>
    [Test]
    public void HandledInteractionsShouldNotCauseException()
    {
        var interaction = new Interaction<Unit, bool>();
        interaction.RegisterHandler(static c => c.SetOutput(true));

        interaction.Handle(Unit.Default).FirstAsync().Wait();
    }

    /// <summary>
    /// Tests that Handlers are executed on handler scheduler.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task HandlersAreExecutedOnHandlerScheduler() =>
        await new TestScheduler().With(async scheduler =>
        {
            var interaction = new Interaction<Unit, string>(scheduler);

            using (interaction.RegisterHandler(x => x.SetOutput("done")))
            {
                var handled = false;
                interaction
                    .Handle(Unit.Default)
                    .Subscribe(_ => handled = true);

                await Assert.That(handled).IsFalse();

                scheduler.Start();
                await Assert.That(handled).IsTrue();
            }
        });

    /// <summary>
    /// Test that Nested handlers are executed in reverse order of subscription.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task NestedHandlersAreExecutedInReverseOrderOfSubscription()
    {
        var interaction = new Interaction<Unit, string>();

        using (interaction.RegisterHandler(static x => x.SetOutput("A")))
        {
            await Assert.That(interaction.Handle(Unit.Default).FirstAsync().Wait()).IsEqualTo("A");
            using (interaction.RegisterHandler(static x => x.SetOutput("B")))
            {
                await Assert.That(interaction.Handle(Unit.Default).FirstAsync().Wait()).IsEqualTo("B");
                using (interaction.RegisterHandler(static x => x.SetOutput("C")))
                {
                    await Assert.That(interaction.Handle(Unit.Default).FirstAsync().Wait()).IsEqualTo("C");
                }

                await Assert.That(interaction.Handle(Unit.Default).FirstAsync().Wait()).IsEqualTo("B");
            }

            await Assert.That(interaction.Handle(Unit.Default).FirstAsync().Wait()).IsEqualTo("A");
        }
    }

    /// <summary>
    /// Tests that handlers can opt not to handle the interaction.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task HandlersCanOptNotToHandleTheInteraction()
    {
        var interaction = new Interaction<bool, string>();

        var handler1A = interaction.RegisterHandler(static x => x.SetOutput("A"));
        var handler1B = interaction.RegisterHandler(
            static x =>
            {
                // only handle if the input is true
                if (x.Input)
                {
                    x.SetOutput("B");
                }
            });
        var handler1C = interaction.RegisterHandler(static x => x.SetOutput("C"));

        using (handler1A)
        {
            using (handler1B)
            {
                using (handler1C)
                using (Assert.Multiple())
                {
                    await Assert.That(interaction.Handle(false).FirstAsync().Wait()).IsEqualTo("C");
                    await Assert.That(interaction.Handle(true).FirstAsync().Wait()).IsEqualTo("C");
                }

                using (Assert.Multiple())
                {
                    await Assert.That(interaction.Handle(false).FirstAsync().Wait()).IsEqualTo("A");
                    await Assert.That(interaction.Handle(true).FirstAsync().Wait()).IsEqualTo("B");
                }
            }

            using (Assert.Multiple())
            {
                await Assert.That(interaction.Handle(false).FirstAsync().Wait()).IsEqualTo("A");
                await Assert.That(interaction.Handle(true).FirstAsync().Wait()).IsEqualTo("A");
            }
        }
    }

    /// <summary>
    /// Test that handlers can contain asynchronous code.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task HandlersCanContainAsynchronousCode()
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

            await Assert.That(result).IsEmpty();
            scheduler.AdvanceBy(TimeSpan.FromSeconds(0.5).Ticks);
            await Assert.That(result).IsEmpty();
            scheduler.AdvanceBy(TimeSpan.FromSeconds(0.6).Ticks);
            await Assert.That(result).Count().IsEqualTo(1);
            await Assert.That(result[0]).IsEqualTo("B");
        }

        await Assert.That(handler1AWasCalled).IsFalse();
    }

    /// <summary>
    /// Test that handlers can contain asynchronous code via tasks.
    /// </summary>
    [Test]
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
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task HandlersReturningObservablesCanReturnAnyKindOfObservable()
    {
        var interaction = new Interaction<Unit, string>();

        var handler = interaction.RegisterHandler(
            x =>
                Observable
                    .Return(42)
                    .Do(_ => x.SetOutput("result")));

        var result = interaction.Handle(Unit.Default).FirstAsync().Wait();
        await Assert.That(result).IsEqualTo("result");
    }
}
