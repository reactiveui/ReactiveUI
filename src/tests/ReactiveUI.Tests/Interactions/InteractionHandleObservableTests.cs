// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Tests.Utilities.Schedulers;

namespace ReactiveUI.Tests.Interactions;

/// <summary>Tests for the sequential interaction runner behind <see cref="Interaction{TInput, TOutput}.Handle"/>.</summary>
public class InteractionHandleObservableTests
{
    /// <summary>The output produced by handlers that succeed.</summary>
    private const string ResultOutput = "result";

    /// <summary>A handler that throws synchronously surfaces its exception to the observer.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task HandlerThrowingSynchronouslyForwardsError()
    {
        var interaction = new Interaction<RxVoid, string>(Sequencer.Immediate);
        var boom = new InvalidOperationException("handler boom");
        _ = interaction.RegisterHandler((Action<IInteractionContext<RxVoid, string>>)(_ => throw boom));

        Exception? received = null;
        using var subscription = interaction.Handle(RxVoid.Default).Subscribe(static _ => { }, ex => received = ex);

        await Assert.That(received).IsSameReferenceAs(boom);
    }

    /// <summary>An error from a handler's observable surfaces to the observer.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task HandlerObservableErrorForwardsError()
    {
        var interaction = new Interaction<RxVoid, string>(Sequencer.Immediate);
        var boom = new InvalidOperationException("observable boom");
        _ = interaction.RegisterHandler(_ => new ThrowingObservable(boom));

        Exception? received = null;
        using var subscription = interaction.Handle(RxVoid.Default).Subscribe(static _ => { }, ex => received = ex);

        await Assert.That(received).IsSameReferenceAs(boom);
    }

    /// <summary>With a deferring scheduler the handler step is queued and only runs once the scheduler advances.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task DeferredHandlerStepRunsWhenSchedulerStarts()
    {
        var scheduler = new VirtualTimeScheduler();
        var interaction = new Interaction<RxVoid, string>(scheduler);
        _ = interaction.RegisterHandler((Action<IInteractionContext<RxVoid, string>>)(static context => context.SetOutput(ResultOutput)));

        string? result = null;
        using var subscription = interaction.Handle(RxVoid.Default).Subscribe(r => result = r);

        // The step is queued on the scheduler, not run inline.
        await Assert.That(result).IsNull();

        scheduler.Start();

        await Assert.That(result).IsEqualTo(ResultOutput);
    }

    /// <summary>Disposing before the deferred step runs cancels the run so no output is produced.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task DisposingBeforeDeferredStepCancelsRun()
    {
        var scheduler = new VirtualTimeScheduler();
        var interaction = new Interaction<RxVoid, string>(scheduler);
        _ = interaction.RegisterHandler((Action<IInteractionContext<RxVoid, string>>)(static context => context.SetOutput(ResultOutput)));

        string? result = null;
        var subscription = interaction.Handle(RxVoid.Default).Subscribe(r => result = r);
        subscription.Dispose();

        scheduler.Start();

        await Assert.That(result).IsNull();
    }

    /// <summary>If a context reports handled but its output retrieval throws, the error is forwarded to the observer.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task HandledContextWithThrowingOutputForwardsError()
    {
        var interaction = new ThrowingOutputInteraction(Sequencer.Immediate);

        Exception? received = null;
        using var subscription = interaction.Handle(RxVoid.Default).Subscribe(static _ => { }, ex => received = ex);

        await Assert.That(received).IsNotNull();
    }

    /// <summary>A step running after the sink has been disposed is a no-op: the observer receives nothing.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task StepAfterDisposeIsIgnored()
    {
        var observer = new RecordingObserver();
        var context = new ThrowingOutputContext(RxVoid.Default);
        var interaction = new Interaction<RxVoid, string>(Sequencer.Immediate);
        var sink = new InteractionHandleObservable<RxVoid, string>.Sink(
            observer,
            [],
            context,
            Sequencer.Immediate,
            interaction,
            RxVoid.Default);

        sink.Dispose();
        sink.Step(0);

        using (Assert.Multiple())
        {
            await Assert.That(observer.Values).IsEmpty();
            await Assert.That(observer.Error).IsNull();
            await Assert.That(observer.Completed).IsFalse();
        }
    }

    /// <summary>An observable that errors any subscriber synchronously.</summary>
    /// <param name="error">The error to deliver on subscribe.</param>
    private sealed class ThrowingObservable(Exception error) : IObservable<RxVoid>
    {
        /// <inheritdoc/>
        public IDisposable Subscribe(IObserver<RxVoid> observer)
        {
            observer.OnError(error);
            return EmptyDisposable.Instance;
        }
    }

    /// <summary>An interaction whose context reports handled but throws when its output is read.</summary>
    /// <param name="scheduler">The handler scheduler.</param>
    private sealed class ThrowingOutputInteraction(ISequencer scheduler) : Interaction<RxVoid, string>(scheduler)
    {
        /// <inheritdoc/>
        protected override IOutputContext<RxVoid, string> GenerateContext(RxVoid input) => new ThrowingOutputContext(input);
    }

    /// <summary>A context that always reports handled but throws when its output is retrieved.</summary>
    /// <param name="input">The interaction input.</param>
    private sealed class ThrowingOutputContext(RxVoid input) : IOutputContext<RxVoid, string>
    {
        /// <inheritdoc/>
        public RxVoid Input => input;

        /// <inheritdoc/>
        public bool IsHandled => true;

        /// <inheritdoc/>
        public void SetOutput(string output)
        {
            // Intentionally empty: this stub forces the handled-but-no-output path.
        }

        /// <inheritdoc/>
        public string GetOutput() => throw new InvalidOperationException("output boom");
    }

    /// <summary>Records every notification an observer receives.</summary>
    private sealed class RecordingObserver : IObserver<string>
    {
        /// <summary>Gets the values delivered via <see cref="OnNext"/>.</summary>
        public List<string> Values { get; } = [];

        /// <summary>Gets the error delivered via <see cref="OnError"/>, if any.</summary>
        public Exception? Error { get; private set; }

        /// <summary>Gets a value indicating whether <see cref="OnCompleted"/> was invoked.</summary>
        public bool Completed { get; private set; }

        /// <inheritdoc/>
        public void OnNext(string value) => Values.Add(value);

        /// <inheritdoc/>
        public void OnError(Exception error) => Error = error;

        /// <inheritdoc/>
        public void OnCompleted() => Completed = true;
    }
}
