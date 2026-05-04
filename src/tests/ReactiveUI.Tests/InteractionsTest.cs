// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using DynamicData;

namespace ReactiveUI.Tests;

/// <summary>
///     Tests interactions.
/// </summary>
[NotInParallel]
public class InteractionsTest
{
    /// <summary>
    ///     Test that attempting to get interaction output before it has been set should cause exception.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task AttemptingToGetInteractionOutputBeforeItHasBeenSetShouldCauseException()
    {
        var interaction = new Interaction<Unit, Unit>();

        interaction.RegisterHandler(context => { _ = ((InteractionContext<Unit, Unit>)context).GetOutput(); });

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => interaction.Handle(Unit.Default).ToTask());
        await Assert.That(ex!.Message).IsEqualTo("Output has not been set.");
    }

    /// <summary>
    ///     Test that attempting to set interaction output more than once should cause exception.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task AttemptingToSetInteractionOutputMoreThanOnceShouldCauseException()
    {
        var interaction = new Interaction<Unit, Unit>();

        interaction.RegisterHandler(context =>
        {
            context.SetOutput(Unit.Default);
            context.SetOutput(Unit.Default);
        });

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => interaction.Handle(Unit.Default).ToTask());
        await Assert.That(ex!.Message).IsEqualTo("Output has already been set.");
    }

    /// <summary>
    ///     Tests that Handled interactions should not cause exception.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task HandledInteractionsShouldNotCauseException()
    {
        var interaction = new Interaction<Unit, bool>();
        interaction.RegisterHandler(static c => c.SetOutput(true));

        await interaction.Handle(Unit.Default);
    }

    /// <summary>
    ///     Tests that Handlers are executed on handler scheduler.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task HandlersAreExecutedOnHandlerScheduler()
    {
        var schedulerThreadId = -1;
        using var scheduler = new EventLoopScheduler(
            threadStart =>
            {
                var thread = new Thread(threadStart) { IsBackground = true };
                schedulerThreadId = thread.ManagedThreadId;
                return thread;
            });
        var interaction = new Interaction<Unit, string>(scheduler);
        var handlerThreadId = -1;

        using (interaction.RegisterHandler(x =>
        {
            handlerThreadId = Environment.CurrentManagedThreadId;
            x.SetOutput("done");
        }))
        {
            var result = await interaction.Handle(Unit.Default).ToTask().WaitAsync(TimeSpan.FromSeconds(5));

            using (Assert.Multiple())
            {
                await Assert.That(result).IsEqualTo("done");
                await Assert.That(handlerThreadId).IsEqualTo(schedulerThreadId);
            }
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
        var handler1BWasSubscribed = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var handler1A = interaction.RegisterHandler(x =>
        {
            x.SetOutput("A");
            handler1AWasCalled = true;
        });
        var handler1B = interaction.RegisterHandler(x =>
        {
            return Observable.Create<Unit>(
                observer =>
                {
                    var subscription = Observables
                        .Unit
                        .Delay(TimeSpan.FromSeconds(1), scheduler)
                        .Do(_ => x.SetOutput("B"))
                        .Subscribe(observer);

                    handler1BWasSubscribed.TrySetResult();
                    return subscription;
                });
        });

        using (handler1A)
        using (handler1B)
        {
            interaction
                .Handle(Unit.Default)
                .ToObservableChangeSet(ImmediateScheduler.Instance).Bind(out var result).Subscribe();

            await handler1BWasSubscribed.Task.WaitAsync(TimeSpan.FromSeconds(5));
            await Assert.That(result).IsEmpty();
            scheduler.AdvanceBy(TimeSpan.FromSeconds(0.5));
            await Assert.That(result).IsEmpty();
            scheduler.AdvanceBy(TimeSpan.FromSeconds(0.6));
            await Assert.That(result).Count().IsEqualTo(1);
            await Assert.That(result[0]).IsEqualTo("B");
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
        var interaction = new Interaction<Unit, string>();

        interaction.RegisterHandler(context =>
        {
            context.SetOutput("result");
            return Task.FromResult(true);
        });

        var result = await interaction.Handle(Unit.Default);

        await Assert.That(result).IsEqualTo("result");
    }

    /// <summary>
    ///     Tests that task handlers release the current scheduler before invoking user code.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task TaskHandlersShouldNotBlockNestedInteractionsBeforeReturningTask()
    {
        var parent = new Interaction<Unit, Unit>();
        var nested = new Interaction<Unit, string>();
        var nestedHandledBeforeParentReturned = false;
        string? nestedOutput = null;

        nested.RegisterHandler(context => context.SetOutput("nested"));

        parent.RegisterHandler(context =>
        {
            using var nestedSubscription = nested.Handle(Unit.Default).Subscribe(output => nestedOutput = output);
            nestedHandledBeforeParentReturned = nestedOutput == "nested";

            context.SetOutput(Unit.Default);
            return Task.CompletedTask;
        });

        await parent.Handle(Unit.Default);

        await Assert.That(nestedHandledBeforeParentReturned).IsTrue();
    }

    /// <summary>
    ///     Tests that observable handlers release the current scheduler before invoking user code.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task ObservableHandlersShouldNotBlockNestedInteractionsBeforeReturningObservable()
    {
        var parent = new Interaction<Unit, Unit>();
        var nested = new Interaction<Unit, string>();
        var nestedHandledBeforeParentReturned = false;
        string? nestedOutput = null;

        nested.RegisterHandler(context => context.SetOutput("nested"));

        parent.RegisterHandler(context =>
        {
            using var nestedSubscription = nested.Handle(Unit.Default).Subscribe(output => nestedOutput = output);
            nestedHandledBeforeParentReturned = nestedOutput == "nested";

            context.SetOutput(Unit.Default);
            return Observables.Unit;
        });

        await parent.Handle(Unit.Default);

        await Assert.That(nestedHandledBeforeParentReturned).IsTrue();
    }

    /// <summary>
    ///     Tests that handlers can opt not to handle the interaction.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task HandlersCanOptNotToHandleTheInteraction()
    {
        var interaction = new Interaction<bool, string>();

        var handler1A = interaction.RegisterHandler(static x => x.SetOutput("A"));
        var handler1B = interaction.RegisterHandler(static x =>
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
                    await Assert.That(await interaction.Handle(false)).IsEqualTo("C");
                    await Assert.That(await interaction.Handle(true)).IsEqualTo("C");
                }

                using (Assert.Multiple())
                {
                    await Assert.That(await interaction.Handle(false)).IsEqualTo("A");
                    await Assert.That(await interaction.Handle(true)).IsEqualTo("B");
                }
            }

            using (Assert.Multiple())
            {
                await Assert.That(await interaction.Handle(false)).IsEqualTo("A");
                await Assert.That(await interaction.Handle(true)).IsEqualTo("A");
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
        var interaction = new Interaction<Unit, string>();

        _ = interaction.RegisterHandler(x =>
            Observable
                .Return(42)
                .Do(_ => x.SetOutput("result")));

        var result = await interaction.Handle(Unit.Default);
        await Assert.That(result).IsEqualTo("result");
    }

    /// <summary>
    ///     Test that Nested handlers are executed in reverse order of subscription.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task NestedHandlersAreExecutedInReverseOrderOfSubscription()
    {
        var interaction = new Interaction<Unit, string>();

        using (interaction.RegisterHandler(static x => x.SetOutput("A")))
        {
            await Assert.That(await interaction.Handle(Unit.Default)).IsEqualTo("A");
            using (interaction.RegisterHandler(static x => x.SetOutput("B")))
            {
                await Assert.That(await interaction.Handle(Unit.Default)).IsEqualTo("B");
                using (interaction.RegisterHandler(static x => x.SetOutput("C")))
                {
                    await Assert.That(await interaction.Handle(Unit.Default)).IsEqualTo("C");
                }

                await Assert.That(await interaction.Handle(Unit.Default)).IsEqualTo("B");
            }

            await Assert.That(await interaction.Handle(Unit.Default)).IsEqualTo("A");
        }
    }

    /// <summary>
    ///     Tests that registers null handler should cause exception.
    /// </summary>
    [Test]
    public void RegisterNullHandlerShouldCauseException()
    {
        var interaction = new Interaction<Unit, Unit>();

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
        var interaction = new Interaction<string, Unit>();
        var ex = await Assert.ThrowsAsync<UnhandledInteractionException<string, Unit>>(() =>
            interaction.Handle("foo").ToTask());
        using (Assert.Multiple())
        {
            await Assert.That(ex!.Interaction).IsSameReferenceAs(interaction);
            await Assert.That(ex.Input).IsEqualTo("foo");
        }

        interaction.RegisterHandler(_ => { });
        interaction.RegisterHandler(_ => { });
        ex = await Assert.ThrowsAsync<UnhandledInteractionException<string, Unit>>(() =>
            interaction.Handle("bar").ToTask());
        using (Assert.Multiple())
        {
            await Assert.That(ex!.Interaction).IsSameReferenceAs(interaction);
            await Assert.That(ex.Input).IsEqualTo("bar");
        }
    }
}
