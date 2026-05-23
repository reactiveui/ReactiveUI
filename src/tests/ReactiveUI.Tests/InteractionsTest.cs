// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using DynamicData;
using ReactiveUI.Internal;
using ReactiveUI.Tests.Utilities.Schedulers;
using TUnit.Core.Executors;

namespace ReactiveUI.Tests;

/// <summary>
///     Tests interactions.
/// </summary>
public class InteractionsTest
{
    private const string OutputA = "A";
    private const string OutputB = "B";
    private const string OutputC = "C";
    private const string ResultOutput = "result";

    /// <summary>
    ///     Test that attempting to get interaction output before it has been set should cause exception.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task AttemptingToGetInteractionOutputBeforeItHasBeenSetShouldCauseException()
    {
        var interaction = new Interaction<Unit, Unit>(ImmediateScheduler.Instance);

        interaction.RegisterHandler(context => _ = ((InteractionContext<Unit, Unit>)context).GetOutput());

        var ex = Assert.Throws<InvalidOperationException>(() => interaction.Handle(Unit.Default).Subscribe());
        await Assert.That(ex.Message).IsEqualTo("Output has not been set.");
    }

    /// <summary>
    ///     Test that attempting to set interaction output more than once should cause exception.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task AttemptingToSetInteractionOutputMoreThanOnceShouldCauseException()
    {
        var interaction = new Interaction<Unit, Unit>(ImmediateScheduler.Instance);

        interaction.RegisterHandler(context =>
        {
            context.SetOutput(Unit.Default);
            context.SetOutput(Unit.Default);
        });

        var ex = Assert.Throws<InvalidOperationException>(() => interaction.Handle(Unit.Default).Subscribe());
        await Assert.That(ex.Message).IsEqualTo("Output has already been set.");
    }

    /// <summary>
    ///     Tests that Handled interactions should not cause exception.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task HandledInteractionsShouldNotCauseException()
    {
        var interaction = new Interaction<Unit, bool>(ImmediateScheduler.Instance);
        interaction.RegisterHandler(static c => c.SetOutput(true));

        // Await rather than block: blocking (.Wait()) on a CurrentThreadScheduler-scheduled interaction can deadlock
        // when a scheduler trampoline is already active on the test thread.
        _ = await interaction.Handle(Unit.Default).FirstAsync();
    }

    /// <summary>
    ///     Tests that Handlers are executed on handler scheduler.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<WithSchedulerExecutor>]
    public async Task HandlersAreExecutedOnHandlerScheduler()
    {
        var scheduler = TestContext.Current!.GetScheduler();
        var interaction = new Interaction<Unit, string>(scheduler);

        using (interaction.RegisterHandler(x => x.SetOutput("done")))
        {
            var handled = false;
            interaction
                    .Handle(Unit.Default).Subscribe(_ => handled = true);

            // With ImmediateScheduler, handlers execute immediately
            await Assert.That(handled).IsTrue();
        }
    }

    /// <summary>
    ///     Test that handlers can contain asynchronous code.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<WithVirtualTimeSchedulerExecutor>]
    public async Task HandlersCanContainAsynchronousCode()
    {
        var scheduler = TestContext.Current.GetVirtualTimeScheduler();
        var interaction = new Interaction<Unit, string>();

        // even though handler B is "slow" (i.e. mimicks waiting for the user), it takes precedence over A, so we expect A to never even be called
        var handler1AWasCalled = false;
        var handler1A = interaction.RegisterHandler(x =>
        {
            x.SetOutput(OutputA);
            handler1AWasCalled = true;
        });
        var handler1B = interaction.RegisterHandler(x =>
            SingleValueObservable.Unit.Delay(TimeSpan.FromSeconds(1), scheduler).Do(_ => x.SetOutput(OutputB)));

        using (handler1A)
        using (handler1B)
        {
            interaction
                .Handle(Unit.Default).ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var result).Subscribe();

            await Assert.That(result).IsEmpty();
            scheduler.AdvanceBy(TimeSpan.FromSeconds(0.5));
            await Assert.That(result).IsEmpty();
            scheduler.AdvanceBy(TimeSpan.FromSeconds(0.6));
            await Assert.That(result).Count().IsEqualTo(1);
            await Assert.That(result[0]).IsEqualTo(OutputB);
        }

        await Assert.That(handler1AWasCalled).IsFalse();
    }

    /// <summary>
    ///     Test that handlers can contain asynchronous code via tasks.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task HandlersCanContainAsynchronousCodeViaTasks()
    {
        var interaction = new Interaction<Unit, string>(ImmediateScheduler.Instance);

        interaction.RegisterHandler(context =>
        {
            context.SetOutput(ResultOutput);
            return Task.FromResult(true);
        });

        // The Task-based handler yields before running (see #4351), so it completes asynchronously; await the
        // interaction result rather than reading it synchronously after Subscribe.
        var result = await interaction.Handle(Unit.Default);

        await Assert.That(result).IsEqualTo(ResultOutput);
    }

    /// <summary>
    ///     Tests that handlers can opt not to handle the interaction.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task HandlersCanOptNotToHandleTheInteraction()
    {
        var interaction = new Interaction<bool, string>(ImmediateScheduler.Instance);

        var handler1A = interaction.RegisterHandler(static x => x.SetOutput(OutputA));
        var handler1B = interaction.RegisterHandler(static x =>
        {
            // only handle if the input is true
            if (!x.Input)
            {
                return;
            }

            x.SetOutput(OutputB);
        });
        var handler1C = interaction.RegisterHandler(static x => x.SetOutput(OutputC));

        using (handler1A)
        {
            using (handler1B)
            {
                using (handler1C)
                using (Assert.Multiple())
                {
                    await Assert.That(await interaction.Handle(false).FirstAsync()).IsEqualTo(OutputC);
                    await Assert.That(await interaction.Handle(true).FirstAsync()).IsEqualTo(OutputC);
                }

                using (Assert.Multiple())
                {
                    await Assert.That(await interaction.Handle(false).FirstAsync()).IsEqualTo(OutputA);
                    await Assert.That(await interaction.Handle(true).FirstAsync()).IsEqualTo(OutputB);
                }
            }

            using (Assert.Multiple())
            {
                await Assert.That(await interaction.Handle(false).FirstAsync()).IsEqualTo(OutputA);
                await Assert.That(await interaction.Handle(true).FirstAsync()).IsEqualTo(OutputA);
            }
        }
    }

    /// <summary>
    ///     Tests that handlers returning observables can return any kind of observable.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task HandlersReturningObservablesCanReturnAnyKindOfObservable()
    {
        var interaction = new Interaction<Unit, string>(ImmediateScheduler.Instance);

        const int SampleValue = 42;
        _ = interaction.RegisterHandler(x =>
            Observable
                    .Return(SampleValue).Do(_ => x.SetOutput(ResultOutput)));

        var result = await interaction.Handle(Unit.Default).FirstAsync();
        await Assert.That(result).IsEqualTo(ResultOutput);
    }

    /// <summary>
    ///     Test that Nested handlers are executed in reverse order of subscription.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task NestedHandlersAreExecutedInReverseOrderOfSubscription()
    {
        var interaction = new Interaction<Unit, string>(ImmediateScheduler.Instance);

        using (interaction.RegisterHandler(static x => x.SetOutput(OutputA)))
        {
            await Assert.That(await interaction.Handle(Unit.Default).FirstAsync()).IsEqualTo(OutputA);
            using (interaction.RegisterHandler(static x => x.SetOutput(OutputB)))
            {
                await Assert.That(await interaction.Handle(Unit.Default).FirstAsync()).IsEqualTo(OutputB);
                using (interaction.RegisterHandler(static x => x.SetOutput(OutputC)))
                {
                    await Assert.That(await interaction.Handle(Unit.Default).FirstAsync()).IsEqualTo(OutputC);
                }

                await Assert.That(await interaction.Handle(Unit.Default).FirstAsync()).IsEqualTo(OutputB);
            }

            await Assert.That(await interaction.Handle(Unit.Default).FirstAsync()).IsEqualTo(OutputA);
        }
    }

    /// <summary>
    ///     Tests that registers null handler should cause exception.
    /// </summary>
    [Test]
    public void RegisterNullHandlerShouldCauseException()
    {
        var interaction = new Interaction<Unit, Unit>(ImmediateScheduler.Instance);

        Assert.Throws<ArgumentNullException>(() =>
            interaction.RegisterHandler((Action<IInteractionContext<Unit, Unit>>)null!));
        Assert.Throws<ArgumentNullException>(() => interaction.RegisterHandler(null!));
        Assert.Throws<ArgumentNullException>(() =>
            interaction.RegisterHandler((Func<IInteractionContext<Unit, Unit>, IObservable<Unit>>)null!));
    }

    /// <summary>
    ///     Tests that unhandled interactions should cause exception.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task UnhandledInteractionsShouldCauseException()
    {
        var interaction = new Interaction<string, Unit>(ImmediateScheduler.Instance);
        var ex = await Assert.That(async () => await interaction.Handle("foo").FirstAsync())
            .Throws<UnhandledInteractionException<string, Unit>>();
        using (Assert.Multiple())
        {
            await Assert.That(ex!.Interaction).IsSameReferenceAs(interaction);
            await Assert.That(ex.Input).IsEqualTo("foo");
        }

        interaction.RegisterHandler(_ => { });
        interaction.RegisterHandler(_ => { });
        ex = await Assert.That(async () => await interaction.Handle("bar").FirstAsync())
            .Throws<UnhandledInteractionException<string, Unit>>();
        using (Assert.Multiple())
        {
            await Assert.That(ex!.Interaction).IsSameReferenceAs(interaction);
            await Assert.That(ex.Input).IsEqualTo("bar");
        }
    }
}
