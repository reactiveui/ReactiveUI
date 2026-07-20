// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.Concurrent;
using ReactiveUI.Tests.Utilities.Schedulers;
using TUnit.Core.Executors;

namespace ReactiveUI.Tests;

/// <summary>Tests interactions.</summary>
public class InteractionsTest
{
    /// <summary>The output produced by the first interaction handler.</summary>
    private const string OutputA = "A";

    /// <summary>The output produced by the second interaction handler.</summary>
    private const string OutputB = "B";

    /// <summary>The output produced by the third interaction handler.</summary>
    private const string OutputC = "C";

    /// <summary>The output used by handlers that return a single result.</summary>
    private const string ResultOutput = "result";

    /// <summary>Test that attempting to get interaction output before it has been set should cause exception.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task AttemptingToGetInteractionOutputBeforeItHasBeenSetShouldCauseException()
    {
        var interaction = new Interaction<RxVoid, RxVoid>(Sequencer.Immediate);

        _ = interaction.RegisterHandler(context => _ = ((InteractionContext<RxVoid, RxVoid>)context).GetOutput());

        var ex = Assert.Throws<InvalidOperationException>(() => interaction.Handle(RxVoid.Default).Subscribe());
        await Assert.That(ex.Message).IsEqualTo("Output has not been set.");
    }

    /// <summary>Test that attempting to set interaction output more than once should cause exception.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task AttemptingToSetInteractionOutputMoreThanOnceShouldCauseException()
    {
        var interaction = new Interaction<RxVoid, RxVoid>(Sequencer.Immediate);

        _ = interaction.RegisterHandler(context =>
        {
            context.SetOutput(RxVoid.Default);
            context.SetOutput(RxVoid.Default);
        });

        var ex = Assert.Throws<InvalidOperationException>(() => interaction.Handle(RxVoid.Default).Subscribe());
        await Assert.That(ex.Message).IsEqualTo("Output has already been set.");
    }

    /// <summary>Tests that Handled interactions should not cause exception.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task HandledInteractionsShouldNotCauseException()
    {
        var interaction = new Interaction<RxVoid, bool>(Sequencer.Immediate);
        _ = interaction.RegisterHandler(static c => c.SetOutput(true));

        // Await rather than block: blocking (.Wait()) on a CurrentThreadScheduler-scheduled interaction can deadlock
        // when a scheduler trampoline is already active on the test thread.
        _ = await interaction.Handle(RxVoid.Default).FirstAsync();
    }

    /// <summary>Tests that Handlers are executed on handler scheduler.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<WithSchedulerExecutor>]
    public async Task HandlersAreExecutedOnHandlerScheduler()
    {
        var scheduler = TestContext.Current!.GetScheduler();
        var interaction = new Interaction<RxVoid, string>(scheduler);

        using (interaction.RegisterHandler(x => x.SetOutput("done")))
        {
            var handled = false;
            _ = interaction
                .Handle(RxVoid.Default).Subscribe(_ => handled = true);

            // With ImmediateScheduler, handlers execute immediately
            await Assert.That(handled).IsTrue();
        }
    }

    /// <summary>Test that handlers can contain asynchronous code.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<WithVirtualTimeSchedulerExecutor>]
    public async Task HandlersCanContainAsynchronousCode()
    {
        var scheduler = TestContext.Current.GetVirtualTimeScheduler();
        var interaction = new Interaction<RxVoid, string>();

        // even though handler B is "slow" (i.e. mimicks waiting for the user), it takes precedence over A, so we expect A to never even be called
        var handler1AWasCalled = false;
        var handler1A = interaction.RegisterHandler(x =>
        {
            x.SetOutput(OutputA);
            handler1AWasCalled = true;
        });
        var handler1B = interaction.RegisterHandler(x =>
            ImmutableReturnRxVoidSignal.Instance.Delay(TimeSpan.FromSeconds(1), scheduler)
                .Do(_ => x.SetOutput(OutputB)));

        using (handler1A)
        using (handler1B)
        {
            var result = interaction
                .Handle(RxVoid.Default).Collect();

            await Assert.That(result).IsEmpty();
            scheduler.AdvanceBy(TimeSpan.FromSeconds(0.5));
            await Assert.That(result).IsEmpty();
            scheduler.AdvanceBy(TimeSpan.FromSeconds(0.6));
            await Assert.That(result).Count().IsEqualTo(1);
            await Assert.That(result[0]).IsEqualTo(OutputB);
        }

        await Assert.That(handler1AWasCalled).IsFalse();
    }

    /// <summary>Test that handlers can contain asynchronous code via tasks.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task HandlersCanContainAsynchronousCodeViaTasks()
    {
        var interaction = new Interaction<RxVoid, string>(Sequencer.Immediate);

        _ = interaction.RegisterHandler(context =>
        {
            context.SetOutput(ResultOutput);
            return Task.FromResult(true);
        });

        // The Task-based handler yields before running (see #4351), so it completes asynchronously; await the
        // interaction result rather than reading it synchronously after Subscribe.
        var result = await interaction.Handle(RxVoid.Default);

        await Assert.That(result).IsEqualTo(ResultOutput);
    }

    /// <summary>Tests that handlers can opt not to handle the interaction.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task HandlersCanOptNotToHandleTheInteraction()
    {
        var interaction = new Interaction<bool, string>(Sequencer.Immediate);

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

    /// <summary>Tests that handlers returning observables can return any kind of observable.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task HandlersReturningObservablesCanReturnAnyKindOfObservable()
    {
        var interaction = new Interaction<RxVoid, string>(Sequencer.Immediate);

        const int SampleValue = 42;
        _ = interaction.RegisterHandler(x =>
            Signal
                .Emit(SampleValue).Do(_ => x.SetOutput(ResultOutput)));

        var result = await interaction.Handle(RxVoid.Default).FirstAsync();
        await Assert.That(result).IsEqualTo(ResultOutput);
    }

    /// <summary>Test that Nested handlers are executed in reverse order of subscription.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task NestedHandlersAreExecutedInReverseOrderOfSubscription()
    {
        var interaction = new Interaction<RxVoid, string>(Sequencer.Immediate);

        using (interaction.RegisterHandler(static x => x.SetOutput(OutputA)))
        {
            await Assert.That(await interaction.Handle(RxVoid.Default).FirstAsync()).IsEqualTo(OutputA);
            using (interaction.RegisterHandler(static x => x.SetOutput(OutputB)))
            {
                await Assert.That(await interaction.Handle(RxVoid.Default).FirstAsync()).IsEqualTo(OutputB);
                using (interaction.RegisterHandler(static x => x.SetOutput(OutputC)))
                {
                    await Assert.That(await interaction.Handle(RxVoid.Default).FirstAsync()).IsEqualTo(OutputC);
                }

                await Assert.That(await interaction.Handle(RxVoid.Default).FirstAsync()).IsEqualTo(OutputB);
            }

            await Assert.That(await interaction.Handle(RxVoid.Default).FirstAsync()).IsEqualTo(OutputA);
        }
    }

    /// <summary>Tests that registers null handler should cause exception.</summary>
    [Test]
    public void RegisterNullHandlerShouldCauseException()
    {
        var interaction = new Interaction<RxVoid, RxVoid>(Sequencer.Immediate);

        _ = Assert.Throws<ArgumentNullException>(() =>
            interaction.RegisterHandler((Action<IInteractionContext<RxVoid, RxVoid>>)null!));
        _ = Assert.Throws<ArgumentNullException>(() => interaction.RegisterHandler(null!));
        _ = Assert.Throws<ArgumentNullException>(() =>
            interaction.RegisterHandler((Func<IInteractionContext<RxVoid, RxVoid>, IObservable<RxVoid>>)null!));
    }

    /// <summary>Tests that unhandled interactions should cause exception.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task UnhandledInteractionsShouldCauseException()
    {
        var interaction = new Interaction<string, RxVoid>(Sequencer.Immediate);
        var ex = await Assert.That(() => interaction.Handle("foo").FirstAsync())
            .Throws<UnhandledInteractionException<string, RxVoid>>();
        using (Assert.Multiple())
        {
            await Assert.That(ex!.Interaction).IsSameReferenceAs(interaction);
            await Assert.That(ex.Input).IsEqualTo("foo");
        }

        _ = interaction.RegisterHandler(_ => { });
        _ = interaction.RegisterHandler(_ => { });
        ex = await Assert.That(() => interaction.Handle("bar").FirstAsync())
            .Throws<UnhandledInteractionException<string, RxVoid>>();
        using (Assert.Multiple())
        {
            await Assert.That(ex!.Interaction).IsSameReferenceAs(interaction);
            await Assert.That(ex.Input).IsEqualTo("bar");
        }
    }

    /// <summary>
    /// A task-based handler registered while a <see cref="SynchronizationContext"/> is installed resumes on that
    /// captured context rather than on a thread-pool thread, so UI handlers run on the UI thread (see issue #4393).
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task TaskHandlerResumesOnCapturedSynchronizationContext()
    {
        using var uiContext = new SingleThreadedSynchronizationContext();
        var observedContext = new TaskCompletionSource<SynchronizationContext?>();
        var completed = new TaskCompletionSource<RxVoid>();

        uiContext.Post(() =>
        {
            var interaction = new Interaction<RxVoid, RxVoid>();
            _ = interaction.RegisterHandler(async context =>
            {
                _ = observedContext.TrySetResult(SynchronizationContext.Current);
                context.SetOutput(RxVoid.Default);
                await Task.CompletedTask;
            });

            _ = interaction.Handle(RxVoid.Default).Subscribe(
                _ => completed.TrySetResult(RxVoid.Default),
                completed.SetException);
        });

        var handlerContext = await observedContext.Task;
        _ = await completed.Task;

        await Assert.That(handlerContext).IsSameReferenceAs(uiContext);
    }

    /// <summary>
    /// A task-based handler yields before running, so it is not invoked inside the subscription that triggers the
    /// interaction (see issue #4351); the handler only runs once the triggering call has unwound.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task TaskHandlerDoesNotRunInsideTriggeringSubscription()
    {
        const string afterSubscribeMarker = "after-subscribe";
        const string handlerMarker = "handler";
        const int expectedStepCount = 2;

        using var uiContext = new SingleThreadedSynchronizationContext();
        var order = new List<string>();
        var completed = new TaskCompletionSource<IReadOnlyList<string>>();

        uiContext.Post(() =>
        {
            var interaction = new Interaction<RxVoid, string>();
            _ = interaction.RegisterHandler(async context =>
            {
                order.Add(handlerMarker);
                context.SetOutput(ResultOutput);
                await Task.CompletedTask;
            });

            _ = interaction.Handle(RxVoid.Default).Subscribe(
                _ => completed.TrySetResult(order),
                completed.SetException);

            // Recorded before the yielded handler continuation is pumped, so it must precede the handler marker.
            order.Add(afterSubscribeMarker);
        });

        var sequence = await completed.Task;

        using (Assert.Multiple())
        {
            await Assert.That(sequence).Count().IsEqualTo(expectedStepCount);
            await Assert.That(sequence[0]).IsEqualTo(afterSubscribeMarker);
            await Assert.That(sequence[1]).IsEqualTo(handlerMarker);
        }
    }

    /// <summary>A minimal single-threaded <see cref="SynchronizationContext"/> backed by one pumped worker thread.</summary>
    private sealed class SingleThreadedSynchronizationContext : SynchronizationContext, IDisposable
    {
        /// <summary>The queue of work items pumped on the worker thread in order.</summary>
        private readonly BlockingCollection<Action> _queue = new();

        /// <summary>Initializes a new instance of the <see cref="SingleThreadedSynchronizationContext"/> class.</summary>
        public SingleThreadedSynchronizationContext()
        {
            var thread = new Thread(Run) { IsBackground = true, Name = "interaction-ui" };
            thread.Start();
        }

        /// <inheritdoc/>
        public override void Post(SendOrPostCallback d, object? state) => _queue.Add(() => d(state));

        /// <summary>Queues an action to run on the worker thread.</summary>
        /// <param name="action">The action to run.</param>
        public void Post(Action action) => _queue.Add(action);

        /// <inheritdoc/>
        public void Dispose() => _queue.CompleteAdding();

        /// <summary>Pumps queued work on the worker thread with this context installed as current.</summary>
        private void Run()
        {
            SynchronizationContext.SetSynchronizationContext(this);
            foreach (var work in _queue.GetConsumingEnumerable())
            {
                work();
            }
        }
    }
}
