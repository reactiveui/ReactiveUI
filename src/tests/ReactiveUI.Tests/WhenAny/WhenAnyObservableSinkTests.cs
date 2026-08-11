// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.WhenAny;

/// <summary>
/// Direct tests for the internal WhenAnyObservable sinks (the merge and switch shapes of the shared generation
/// engine), covering the re-entrancy, grammar-violation and multi-threaded paths the public
/// <c>WhenAnyObservable</c> API cannot reach.
/// </summary>
/// <remarks>
/// The behaviours pinned here are the ones whose failure mode is a lost notification rather than an exception:
/// an older generation tearing down a newer one's subscriptions, a generation completing downstream while the
/// outer stream is still live, and a terminal notification arriving twice. Each of those leaves a consumer
/// waiting forever, so every test asserts the exact downstream completion count instead of merely observing that
/// values arrived.
/// </remarks>
public class WhenAnyObservableSinkTests
{
    /// <summary>The number of producer threads used by the multi-threaded tests.</summary>
    private const int ProducerCount = 4;

    /// <summary>The number of values each producer thread pushes into its own inner observable.</summary>
    private const int ValuesPerProducer = 2_000;

    /// <summary>The number of generations the churn test pushes through the outer stream.</summary>
    private const int ChurnGenerationCount = 500;

    /// <summary>The value the first generation emits while it is being subscribed.</summary>
    private const int FirstGenerationValue = 1;

    /// <summary>The first value the second generation emits.</summary>
    private const int SecondGenerationValue = 2;

    /// <summary>The second value the second generation emits.</summary>
    private const int ThirdGenerationValue = 3;

    /// <summary>The value emitted by an inner that also completes during subscription.</summary>
    private const int SynchronousInnerValue = 7;

    /// <summary>The value emitted by a generation that arrives after an earlier one ran dry.</summary>
    private const int LateGenerationValue = 11;

    /// <summary>The value emitted by the sibling of a doubly-completing inner.</summary>
    private const int SiblingValue = 5;

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
    /// The same downstream re-entrancy against the merge shape: the nested generation's whole set of inners must
    /// survive the outer frame unwinding.
    /// </summary>
    /// <param name="cancellationToken">The token that aborts the test when its timeout elapses.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [Timeout(30_000)]
    public async Task MergeSink_OuterReenteredFromDownstream_KeepsNewestGeneration(CancellationToken cancellationToken)
    {
        var outer = new Signal<IObservable<int>[]>();
        var left = new Signal<int>();
        var right = new Signal<int>();
        var trackedLeft = new DisposalCountingObservable<int>(left);
        var trackedRight = new DisposalCountingObservable<int>(right);
        var recorder = new Recorder<int>();

        using var subscription = new WhenAnyObservableMergeSink<int>(outer).Subscribe(recorder);

        recorder.OnNextHandler = _ =>
        {
            recorder.OnNextHandler = null;
            outer.OnNext([trackedLeft, trackedRight]);
        };

        outer.OnNext([new ScriptedObservable<int>(static observer => observer.OnNext(FirstGenerationValue))]);

        left.OnNext(SecondGenerationValue);
        right.OnNext(ThirdGenerationValue);
        left.OnCompleted();
        right.OnCompleted();
        outer.OnCompleted();

        cancellationToken.ThrowIfCancellationRequested();

        int[] expected = [FirstGenerationValue, SecondGenerationValue, ThirdGenerationValue];
        await Assert.That(trackedLeft.DisposeCount).IsEqualTo(0);
        await Assert.That(trackedRight.DisposeCount).IsEqualTo(0);
        await Assert.That(recorder.Values).IsEquivalentTo(expected);
        await Assert.That(recorder.Completed).IsEqualTo(1);
    }

    /// <summary>
    /// An inner that emits and completes synchronously while it is being subscribed is fully accounted for: its
    /// value reaches downstream and its completion retires the generation, so the later outer completion
    /// terminates the sink exactly once.
    /// </summary>
    /// <param name="cancellationToken">The token that aborts the test when its timeout elapses.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [Timeout(30_000)]
    public async Task MergeSink_InnerCompletesDuringSubscribe_CompletesOnceWhenOuterFinishes(CancellationToken cancellationToken)
    {
        var outer = new Signal<IObservable<int>[]>();
        var recorder = new Recorder<int>();

        using var subscription = new WhenAnyObservableMergeSink<int>(outer).Subscribe(recorder);

        outer.OnNext([new ScriptedObservable<int>(static observer =>
        {
            observer.OnNext(SynchronousInnerValue);
            observer.OnCompleted();
        })]);

        cancellationToken.ThrowIfCancellationRequested();

        int[] expected = [SynchronousInnerValue];
        await Assert.That(recorder.Values).IsEquivalentTo(expected);
        await Assert.That(recorder.Completed).IsEqualTo(0);

        outer.OnCompleted();

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
    /// An inner that completes twice must not consume another inner's share of the generation's outstanding
    /// count, so the merge sink still waits for the sibling that is genuinely still running.
    /// </summary>
    /// <param name="cancellationToken">The token that aborts the test when its timeout elapses.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    /// <remarks>
    /// Without a per-inner terminal latch the double completion drives the outstanding count to zero while a
    /// sibling is live, so the sink terminates early and the sibling's remaining values are silently discarded.
    /// </remarks>
    [Test]
    [Timeout(30_000)]
    public async Task MergeSink_InnerCompletesTwice_StillWaitsForSibling(CancellationToken cancellationToken)
    {
        var outer = new Signal<IObservable<int>[]>();
        var sibling = new Signal<int>();
        var recorder = new Recorder<int>();

        using var subscription = new WhenAnyObservableMergeSink<int>(outer).Subscribe(recorder);

        var doubleCompleter = new ScriptedObservable<int>(static observer =>
        {
            observer.OnCompleted();
            observer.OnCompleted();
        });

        outer.OnNext([doubleCompleter, sibling]);
        outer.OnCompleted();

        cancellationToken.ThrowIfCancellationRequested();

        await Assert.That(recorder.Completed).IsEqualTo(0);

        sibling.OnNext(SiblingValue);

        int[] expected = [SiblingValue];
        await Assert.That(recorder.Values).IsEquivalentTo(expected);

        sibling.OnCompleted();

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

    /// <summary>An outer stream that completes without ever producing a generation terminates the sink once.</summary>
    /// <param name="cancellationToken">The token that aborts the test when its timeout elapses.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [Timeout(30_000)]
    public async Task MergeSink_OuterCompletesWithoutGeneration_CompletesDownstreamOnce(CancellationToken cancellationToken)
    {
        var outer = new Signal<IObservable<int>[]>();
        var recorder = new Recorder<int>();

        using var subscription = new WhenAnyObservableMergeSink<int>(outer).Subscribe(recorder);
        outer.OnCompleted();

        cancellationToken.ThrowIfCancellationRequested();

        await Assert.That(recorder.Completed).IsEqualTo(1);
    }

    /// <summary>An empty generation leaves nothing outstanding, so the outer completion terminates the sink once.</summary>
    /// <param name="cancellationToken">The token that aborts the test when its timeout elapses.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [Timeout(30_000)]
    public async Task MergeSink_EmptyGeneration_CompletesDownstreamOnceWhenOuterFinishes(CancellationToken cancellationToken)
    {
        var outer = new Signal<IObservable<int>[]>();
        var recorder = new Recorder<int>();

        using var subscription = new WhenAnyObservableMergeSink<int>(outer).Subscribe(recorder);
        outer.OnNext([]);

        cancellationToken.ThrowIfCancellationRequested();

        await Assert.That(recorder.Completed).IsEqualTo(0);

        outer.OnCompleted();

        await Assert.That(recorder.Completed).IsEqualTo(1);
    }

    /// <summary>Superseding a generation disposes the generation it replaced, and only that one.</summary>
    /// <param name="cancellationToken">The token that aborts the test when its timeout elapses.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [Timeout(30_000)]
    public async Task MergeSink_NewGeneration_DisposesOnlyTheSupersededGeneration(CancellationToken cancellationToken)
    {
        var outer = new Signal<IObservable<int>[]>();
        var firstGeneration = new DisposalCountingObservable<int>(new Signal<int>());
        var secondGeneration = new DisposalCountingObservable<int>(new Signal<int>());
        var recorder = new Recorder<int>();

        var subscription = new WhenAnyObservableMergeSink<int>(outer).Subscribe(recorder);

        outer.OnNext([firstGeneration]);
        outer.OnNext([secondGeneration]);

        cancellationToken.ThrowIfCancellationRequested();

        await Assert.That(firstGeneration.DisposeCount).IsEqualTo(1);
        await Assert.That(secondGeneration.DisposeCount).IsEqualTo(0);

        subscription.Dispose();

        await Assert.That(secondGeneration.DisposeCount).IsEqualTo(1);
        await Assert.That(recorder.Completed).IsEqualTo(0);
    }

    /// <summary>
    /// Producers on separate threads push into their own inners while the outer stream terminates from the test
    /// thread. Downstream must see every value, never see two deliveries overlap, and be completed exactly once
    /// regardless of which side finishes last.
    /// </summary>
    /// <param name="outerCompletesFirst">Whether the outer stream completes before the producers finish.</param>
    /// <param name="cancellationToken">The token that aborts the test when its timeout elapses.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    /// <remarks>
    /// Overlapping deliveries would mean the gate no longer serialises the observer contract; a completion count
    /// other than one would mean a race between the outer completion and the last inner completion either lost
    /// the terminal notification or delivered it twice.
    /// </remarks>
    [Test]
    [Arguments(true)]
    [Arguments(false)]
    [Timeout(120_000)]
    public async Task MergeSink_ConcurrentProducers_DeliversEveryValueAndCompletesOnce(bool outerCompletesFirst, CancellationToken cancellationToken)
    {
        var outer = new Signal<IObservable<int>[]>();
        var inners = new Signal<int>[ProducerCount];
        var generation = new IObservable<int>[ProducerCount];
        for (var i = 0; i < ProducerCount; i++)
        {
            inners[i] = new();
            generation[i] = inners[i];
        }

        var recorder = new Recorder<int>();
        using var subscription = new WhenAnyObservableMergeSink<int>(outer).Subscribe(recorder);
        outer.OnNext(generation);

        using var start = new ManualResetEventSlim(false);
        var threads = new Thread[ProducerCount];
        for (var i = 0; i < ProducerCount; i++)
        {
            var producer = inners[i];
            threads[i] = new(() =>
            {
                start.Wait(cancellationToken);
                for (var value = 0; value < ValuesPerProducer; value++)
                {
                    producer.OnNext(value);
                }

                producer.OnCompleted();
            })
            { IsBackground = true };
            threads[i].Start();
        }

        start.Set();

        if (outerCompletesFirst)
        {
            outer.OnCompleted();
        }

        for (var i = 0; i < ProducerCount; i++)
        {
            threads[i].Join();
        }

        if (!outerCompletesFirst)
        {
            outer.OnCompleted();
        }

        await Assert.That(recorder.OverlappingDeliveries).IsEqualTo(0);
        await Assert.That(recorder.Values.Count).IsEqualTo(ProducerCount * ValuesPerProducer);
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
