// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.WhenAny;

/// <summary>Exercises the switch sink retained by suspension through reentrancy and completion races.</summary>
/// <remarks>
/// The behaviours pinned here are the ones whose failure mode is a lost notification rather than an exception:
/// an older generation tearing down a newer one's subscriptions, a generation completing downstream while the
/// outer stream is still live, and a terminal notification arriving twice. Each of those leaves a consumer
/// waiting forever, so every test asserts the exact downstream completion count instead of merely observing that
/// values arrived.
/// </remarks>
public class WhenAnyObservableSwitchSinkTests
{
    /// <summary>The number of generations the churn test pushes through the outer stream.</summary>
    private const int ChurnGenerationCount = 500;

    /// <summary>The value the first generation emits while it is being subscribed.</summary>
    private const int FirstGenerationValue = 1;

    /// <summary>The first value the second generation emits.</summary>
    private const int SecondGenerationValue = 2;

    /// <summary>The value emitted by a generation that arrives after an earlier one ran dry.</summary>
    private const int LateGenerationValue = 11;

    /// <summary>
    /// A downstream handler that pushes a new outer value re-enters the switch sink while the first generation's
    /// subscribe frame is still on the stack. The nested generation must survive: its inner keeps delivering and
    /// the sink still completes once both it and the outer stream are done.
    /// </summary>
    /// <param name="cancellationToken">The token that aborts the test when its timeout elapses.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    /// <remarks>
    /// Catches the stale-generation clobber: if the outer (older) frame publishes its subscription unconditionally
    /// after unwinding, it disposes the nested generation's inner while the generation counter already names the
    /// nested one. Every later notification is then dropped by the id check and downstream never terminates.
    /// </remarks>
    [Test]
    [Timeout(30_000)]
    public async Task SwitchSink_OuterReenteredFromDownstream_KeepsNewestGeneration(CancellationToken cancellationToken)
    {
        var outer = new Signal<IObservable<int>>();
        var second = new Signal<int>();
        var trackedSecond = new DisposalCountingObservable<int>(second);
        var recorder = new Recorder<int>();

        using var subscription = new WhenAnyObservableSwitchSink<int>(outer).Subscribe(recorder);

        // The first inner emits as it is subscribed; the downstream handler answers by pushing a second
        // generation, so OnNextOuter runs re-entrantly underneath the first generation's own subscribe call.
        recorder.OnNextHandler = _ =>
        {
            recorder.OnNextHandler = null;
            outer.OnNext(trackedSecond);
        };

        outer.OnNext(new ScriptedObservable<int>(static observer => observer.OnNext(FirstGenerationValue)));

        second.OnNext(SecondGenerationValue);
        second.OnCompleted();
        outer.OnCompleted();

        cancellationToken.ThrowIfCancellationRequested();

        int[] expected = [FirstGenerationValue, SecondGenerationValue];
        await Assert.That(trackedSecond.DisposeCount).IsEqualTo(0);
        await Assert.That(recorder.Values).IsEquivalentTo(expected);
        await Assert.That(recorder.Completed).IsEqualTo(1);
    }

    /// <summary>
    /// A generation that runs dry before the outer stream finishes must not terminate the sink, and the inners of
    /// the generation that follows must still be delivered.
    /// </summary>
    /// <param name="cancellationToken">The token that aborts the test when its timeout elapses.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [Timeout(30_000)]
    public async Task SwitchSink_GenerationCompletesBeforeOuter_KeepsRunning(CancellationToken cancellationToken)
    {
        var outer = new Signal<IObservable<int>>();
        var later = new Signal<int>();
        var recorder = new Recorder<int>();

        using var subscription = new WhenAnyObservableSwitchSink<int>(outer).Subscribe(recorder);

        outer.OnNext(new ScriptedObservable<int>(static observer => observer.OnCompleted()));

        await Assert.That(recorder.Completed).IsEqualTo(0);

        outer.OnNext(later);
        later.OnNext(LateGenerationValue);

        cancellationToken.ThrowIfCancellationRequested();

        int[] expected = [LateGenerationValue];
        await Assert.That(recorder.Values).IsEquivalentTo(expected);
        await Assert.That(recorder.Completed).IsEqualTo(0);

        later.OnCompleted();
        outer.OnCompleted();

        await Assert.That(recorder.Completed).IsEqualTo(1);
    }

    /// <summary>An inner that completes twice must not terminate downstream twice.</summary>
    /// <param name="cancellationToken">The token that aborts the test when its timeout elapses.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    /// <remarks>
    /// Without a per-inner terminal latch the second completion re-runs the switch sink's completion path with
    /// the outer already done, delivering a second <c>OnCompleted</c> to a downstream that has already been told
    /// the sequence ended.
    /// </remarks>
    [Test]
    [Timeout(30_000)]
    public async Task SwitchSink_InnerCompletesTwiceAfterOuter_CompletesDownstreamOnce(CancellationToken cancellationToken)
    {
        var outer = new Signal<IObservable<int>>();
        var recorder = new Recorder<int>();

        using var subscription = new WhenAnyObservableSwitchSink<int>(outer).Subscribe(recorder);

        var capture = new ObserverCapture<int>();
        outer.OnNext(capture);
        outer.OnCompleted();

        cancellationToken.ThrowIfCancellationRequested();

        await Assert.That(capture.Observer).IsNotNull();
        capture.Observer!.OnCompleted();
        capture.Observer.OnCompleted();

        await Assert.That(recorder.Completed).IsEqualTo(1);
    }

    /// <summary>
    /// Whichever of the outer stream and the current generation finishes last terminates the sink, and it does so
    /// exactly once in either order.
    /// </summary>
    /// <param name="outerCompletesFirst">Whether the outer stream completes before the inner does.</param>
    /// <param name="cancellationToken">The token that aborts the test when its timeout elapses.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [Arguments(true)]
    [Arguments(false)]
    [Timeout(30_000)]
    public async Task SwitchSink_OuterAndInnerCompleteInEitherOrder_CompletesDownstreamOnce(bool outerCompletesFirst, CancellationToken cancellationToken)
    {
        var outer = new Signal<IObservable<int>>();
        var inner = new Signal<int>();
        var recorder = new Recorder<int>();

        using var subscription = new WhenAnyObservableSwitchSink<int>(outer).Subscribe(recorder);
        outer.OnNext(inner);

        cancellationToken.ThrowIfCancellationRequested();

        if (outerCompletesFirst)
        {
            outer.OnCompleted();
            await Assert.That(recorder.Completed).IsEqualTo(0);
            inner.OnCompleted();
        }
        else
        {
            inner.OnCompleted();
            await Assert.That(recorder.Completed).IsEqualTo(0);
            outer.OnCompleted();
        }

        await Assert.That(recorder.Completed).IsEqualTo(1);
    }

    /// <summary>
    /// A generation churn racing against a concurrent producer still terminates exactly once: the last generation
    /// pushed is the only one that can hold the sink open, and superseded generations are ignored rather than
    /// double-counted.
    /// </summary>
    /// <param name="cancellationToken">The token that aborts the test when its timeout elapses.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [Timeout(120_000)]
    public async Task SwitchSink_GenerationChurnAgainstConcurrentProducer_CompletesOnce(CancellationToken cancellationToken)
    {
        var outer = new Signal<IObservable<int>>();
        var recorder = new Recorder<int>();
        using var subscription = new WhenAnyObservableSwitchSink<int>(outer).Subscribe(recorder);

        var produced = new Signal<int>[ChurnGenerationCount];
        for (var i = 0; i < ChurnGenerationCount; i++)
        {
            produced[i] = new();
        }

        using var start = new ManualResetEventSlim(false);
        var pump = new Thread(() =>
        {
            start.Wait(cancellationToken);
            for (var i = 0; i < ChurnGenerationCount; i++)
            {
                produced[i].OnNext(i);
            }
        })
        { IsBackground = true };

        pump.Start();
        start.Set();

        for (var i = 0; i < ChurnGenerationCount; i++)
        {
            outer.OnNext(produced[i]);
        }

        pump.Join();

        // The final generation is the only one that can hold the sink open; retire it, then the outer stream.
        produced[ChurnGenerationCount - 1].OnCompleted();
        outer.OnCompleted();

        await Assert.That(recorder.OverlappingDeliveries).IsEqualTo(0);
        await Assert.That(recorder.Completed).IsEqualTo(1);
    }

    /// <summary>An observable that runs a scripted, synchronous sequence against every observer as it subscribes.</summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <param name="script">The notifications to deliver inline during <see cref="Subscribe"/>.</param>
    private sealed class ScriptedObservable<T>(Action<IObserver<T>> script) : IObservable<T>
    {
        /// <inheritdoc/>
        public IDisposable Subscribe(IObserver<T> observer)
        {
            script(observer);
            return EmptyDisposable.Instance;
        }
    }

    /// <summary>An observable that hands its subscriber straight back to the test so it can be driven by hand.</summary>
    /// <typeparam name="T">The element type.</typeparam>
    private sealed class ObserverCapture<T> : IObservable<T>
    {
        /// <summary>Gets the observer handed to the most recent subscription.</summary>
        public IObserver<T>? Observer { get; private set; }

        /// <inheritdoc/>
        public IDisposable Subscribe(IObserver<T> observer)
        {
            Observer = observer;
            return EmptyDisposable.Instance;
        }
    }

    /// <summary>An observable that counts how many times a subscription handed out by it was disposed.</summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <param name="source">The observable being wrapped.</param>
    private sealed class DisposalCountingObservable<T>(IObservable<T> source) : IObservable<T>
    {
        /// <summary>The number of subscriptions disposed so far.</summary>
        private int _disposeCount;

        /// <summary>Gets the number of subscriptions handed out by this observable that have been disposed.</summary>
        public int DisposeCount => Volatile.Read(ref _disposeCount);

        /// <inheritdoc/>
        public IDisposable Subscribe(IObserver<T> observer)
        {
            var subscription = source.Subscribe(observer);
            return new ActionDisposable(() =>
            {
                _ = Interlocked.Increment(ref _disposeCount);
                subscription.Dispose();
            });
        }
    }

    /// <summary>
    /// Records the notifications delivered downstream, detects overlapping deliveries, and optionally re-enters
    /// the sink from the value callback.
    /// </summary>
    /// <typeparam name="T">The notification value type.</typeparam>
    private sealed class Recorder<T> : IObserver<T>
    {
        /// <summary>Tracks how many deliveries are inside the callback at once, to detect lost serialisation.</summary>
        private int _inFlight;

        /// <summary>The number of times two deliveries were observed inside the callback simultaneously.</summary>
        private int _overlappingDeliveries;

        /// <summary>Gets the values delivered downstream.</summary>
        public List<T> Values { get; } = [];

        /// <summary>Gets the errors delivered downstream.</summary>
        public List<Exception> Errors { get; } = [];

        /// <summary>Gets the number of times downstream was completed.</summary>
        public int Completed { get; private set; }

        /// <summary>Gets the number of deliveries that overlapped another delivery.</summary>
        public int OverlappingDeliveries => Volatile.Read(ref _overlappingDeliveries);

        /// <summary>Gets or sets a callback run inside <see cref="OnNext"/>, used to re-enter the sink.</summary>
        public Action<T>? OnNextHandler { get; set; }

        /// <inheritdoc/>
        public void OnNext(T value)
        {
            if (Interlocked.Increment(ref _inFlight) != 1)
            {
                _ = Interlocked.Increment(ref _overlappingDeliveries);
            }

            Values.Add(value);
            OnNextHandler?.Invoke(value);
            _ = Interlocked.Decrement(ref _inFlight);
        }

        /// <inheritdoc/>
        public void OnError(Exception error) => Errors.Add(error);

        /// <inheritdoc/>
        public void OnCompleted() => Completed++;
    }
}
