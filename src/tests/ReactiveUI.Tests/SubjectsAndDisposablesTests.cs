// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Internal;

namespace ReactiveUI.Tests;

/// <summary>
/// Tests for the internal disposable helpers (<c>DisposableBag</c>, <c>OnceDisposable</c>, <c>MutableDisposable</c>,
/// <c>SwapDisposable</c>) and the internal broadcast subjects that replace System.Reactive's subjects.
/// </summary>
public class SubjectsAndDisposablesTests
{
    /// <summary>Number of entries added to a bag to exercise both inline slots and the growing overflow array.</summary>
    private const int FiveEntries = 5;

    /// <summary>Replay buffer size used to verify only the most recent values are replayed.</summary>
    private const int BufferSize = 2;

    /// <summary>Verifies the bag disposes every entry (including the overflow array) exactly once and is idempotent.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task DisposableBag_DisposesAllEntriesOnce()
    {
        var bag = new DisposableBag();
        var items = new List<Counter>();
        for (var i = 0; i < FiveEntries; i++)
        {
            var counter = new Counter();
            items.Add(counter);
            bag.Add(counter);
        }

        bag.Dispose();
        bag.Dispose();

        await Assert.That(items.TrueForAll(static c => c.Count == 1)).IsTrue();
    }

    /// <summary>Verifies the multi-slot constructors populate and dispose their entries.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task DisposableBag_Constructors_DisposeSeededEntries()
    {
        var a = new Counter();
        var b = new Counter();
        new DisposableBag(a, b).Dispose();

        var c = new Counter();
        var d = new Counter();
        var e = new Counter();
        new DisposableBag(c, d, e).Dispose();

        await Assert.That(a.Count).IsEqualTo(1);
        await Assert.That(e.Count).IsEqualTo(1);
    }

    /// <summary>Verifies adding after disposal disposes the entry immediately, and null is ignored.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task DisposableBag_AddAfterDispose_DisposesImmediately()
    {
        var bag = new DisposableBag();
        bag.Dispose();
        var late = new Counter();
        bag.Add(late);
        bag.Add(null!);

        await Assert.That(late.Count).IsEqualTo(1);
    }

    /// <summary>Verifies a once-disposable accepts a single assignment and disposes it on disposal.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task OnceDisposable_AssignThenDispose()
    {
        var inner = new Counter();
        var holder = new OnceDisposable { Disposable = inner };

        await Assert.That(holder.IsAssigned).IsTrue();
        await Assert.That(holder.Disposable).IsEqualTo(inner);

        holder.Dispose();

        await Assert.That(holder.IsDisposed).IsTrue();
        await Assert.That(holder.Disposable).IsNull();
        await Assert.That(inner.Count).IsEqualTo(1);
    }

    /// <summary>Verifies a second assignment to a once-disposable throws.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task OnceDisposable_DoubleAssign_Throws()
    {
        var holder = new OnceDisposable { Disposable = new Counter() };

        await Assert.That(() => holder.Disposable = new Counter()).Throws<InvalidOperationException>();
    }

    /// <summary>Verifies assigning after disposal disposes the value immediately without throwing.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task OnceDisposable_AssignAfterDispose_DisposesImmediately()
    {
        var holder = new OnceDisposable();
        holder.Dispose();
        var late = new Counter();
        holder.Disposable = late;

        await Assert.That(late.Count).IsEqualTo(1);
    }

    /// <summary>Verifies the mutable disposable keeps replaced values alive and disposes the current one.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task MutableDisposable_DoesNotDisposePreviousOnReassign()
    {
        var first = new Counter();
        var second = new Counter();
        var holder = new MutableDisposable { Disposable = first };
        holder.Disposable = second;
        holder.Dispose();

        var late = new Counter();
        holder.Disposable = late;

        await Assert.That(first.Count).IsEqualTo(0);
        await Assert.That(second.Count).IsEqualTo(1);
        await Assert.That(late.Count).IsEqualTo(1);
    }

    /// <summary>Verifies the swap disposable disposes the replaced value and the latest on disposal.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task SwapDisposable_DisposesPreviousOnReassign()
    {
        var first = new Counter();
        var second = new Counter();
        var holder = new SwapDisposable { Disposable = first };
        holder.Disposable = second;
        holder.Dispose();

        var late = new Counter();
        holder.Disposable = late;

        await Assert.That(first.Count).IsEqualTo(1);
        await Assert.That(second.Count).IsEqualTo(1);
        await Assert.That(late.Count).IsEqualTo(1);
    }

    /// <summary>Verifies the broadcast subject delivers to multiple observers, honours unsubscribe, and completes.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task BroadcastSubject_MultiObserverDeliveryAndUnsubscribe()
    {
        var subject = new BroadcastSubject<string>();
        var first = new Recorder<string>();
        var second = new Recorder<string>();

        var sub1 = subject.Subscribe(first);
        using var sub2 = subject.Subscribe(second);
        subject.OnNext("a");
        sub1.Dispose();
        subject.OnNext("b");
        subject.OnCompleted();

        string[] firstExpected = ["a"];
        string[] secondExpected = ["a", "b"];
        await Assert.That(first.Values).IsEquivalentTo(firstExpected);
        await Assert.That(second.Values).IsEquivalentTo(secondExpected);
        await Assert.That(second.Completed).IsEqualTo(1);
    }

    /// <summary>Verifies the broadcast subject forwards errors to its observers.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task BroadcastSubject_ForwardsError()
    {
        var subject = new BroadcastSubject<string>();
        var rec = new Recorder<string>();
        var ex = new InvalidOperationException("boom");

        using var sub = subject.Subscribe(rec);
        subject.OnError(ex);

        Exception[] expected = [ex];
        await Assert.That(rec.Errors).IsEquivalentTo(expected);
    }

    /// <summary>Verifies the behavior subject seeds new subscribers with the latest value.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task BehaviorBroadcastSubject_SeedsLatestValue()
    {
        var subject = new BehaviorBroadcastSubject<string>("initial");
        var early = new Recorder<string>();
        using var earlySub = subject.Subscribe(early);
        subject.OnNext("updated");

        var late = new Recorder<string>();
        using var lateSub = subject.Subscribe(late);

        await Assert.That(early.Values).Contains("initial");
        await Assert.That(early.Values).Contains("updated");
        await Assert.That(late.Values).Contains("updated");
    }

    /// <summary>Verifies the replay subject replays its buffered values to a later subscriber.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ReplayBroadcastSubject_ReplaysBuffer()
    {
        var subject = new ReplayBroadcastSubject<string>(BufferSize);
        subject.OnNext("a");
        subject.OnNext("b");
        subject.OnNext("c");

        var late = new Recorder<string>();
        using var sub = subject.Subscribe(late);

        string[] expected = ["b", "c"];
        await Assert.That(late.Values).IsEquivalentTo(expected);
    }

    /// <summary>Verifies the delayable subject buffers while delayed and flushes on demand.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task DelayableNotificationSubject_BuffersThenFlushes()
    {
        var delayed = true;
        var subject = new DelayableNotificationSubject<string>(() => delayed, static items => items);
        var rec = new Recorder<string>();
        using var sub = subject.Subscribe(rec);

        subject.OnNext("buffered");
        await Assert.That(rec.Values).IsEmpty();

        delayed = false;
        subject.Flush();

        await Assert.That(rec.Values).Contains("buffered");
    }

    /// <summary>Verifies the behavior subject's terminal paths: completion, error, replay-of-terminal, and ignore-after-stop.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task BehaviorBroadcastSubject_TerminalPaths()
    {
        var completed = new BehaviorBroadcastSubject<string>("seed");
        completed.OnCompleted();
        completed.OnNext("ignored");
        var afterComplete = new Recorder<string>();
        using (completed.Subscribe(afterComplete))
        {
            completed.Dispose();
        }

        var errored = new BehaviorBroadcastSubject<string>("seed");
        var ex = new InvalidOperationException("behavior-error");
        var early = new Recorder<string>();
        using var earlySub = errored.Subscribe(early);
        errored.OnError(ex);
        var afterError = new Recorder<string>();
        using var lateSub = errored.Subscribe(afterError);

        await Assert.That(afterComplete.Completed).IsEqualTo(1);
        await Assert.That(early.Errors).Contains(ex);
        await Assert.That(afterError.Errors).Contains(ex);
    }

    /// <summary>Verifies the replay subject replays its buffer then forwards the terminal completion to a late subscriber.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task ReplayBroadcastSubject_TerminalPaths()
    {
        var completed = new ReplayBroadcastSubject<string>(BufferSize);
        completed.OnNext("a");
        completed.OnCompleted();
        completed.OnNext("ignored");
        var afterComplete = new Recorder<string>();
        completed.Subscribe(afterComplete);
        completed.Dispose();

        var errored = new ReplayBroadcastSubject<string>(BufferSize);
        var ex = new InvalidOperationException("replay-error");
        errored.OnError(ex);
        var afterError = new Recorder<string>();
        errored.Subscribe(afterError);

        await Assert.That(afterComplete.Values).Contains("a");
        await Assert.That(afterComplete.Completed).IsEqualTo(1);
        await Assert.That(afterError.Errors).Contains(ex);
    }

    /// <summary>Verifies the delayable subject delivers immediately when not delayed and forwards terminal notifications.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task DelayableNotificationSubject_ImmediateAndTerminal()
    {
        var immediate = new DelayableNotificationSubject<string>(static () => false, static items => items);
        var rec = new Recorder<string>();
        using var sub = immediate.Subscribe(rec);
        immediate.OnNext("now");
        immediate.OnCompleted();
        immediate.OnNext("ignored");

        var errored = new DelayableNotificationSubject<string>(static () => false, static items => items);
        var ex = new InvalidOperationException("delayable-error");
        errored.OnError(ex);
        var afterError = new Recorder<string>();
        errored.Subscribe(afterError);

        await Assert.That(rec.Values).Contains("now");
        await Assert.That(rec.Completed).IsEqualTo(1);
        await Assert.That(afterError.Errors).Contains(ex);
    }

    /// <summary>A disposable that counts how many times it was disposed.</summary>
    private sealed class Counter : IDisposable
    {
        /// <summary>Gets the number of times <see cref="Dispose"/> was called.</summary>
        public int Count { get; private set; }

        /// <inheritdoc/>
        public void Dispose() => Count++;
    }

    /// <summary>Records the notifications delivered to an observer for assertion.</summary>
    /// <typeparam name="T">The notification value type.</typeparam>
    private sealed class Recorder<T> : IObserver<T>
    {
        /// <summary>Gets the values delivered via <see cref="OnNext"/>.</summary>
        public List<T> Values { get; } = [];

        /// <summary>Gets the errors delivered via <see cref="OnError"/>.</summary>
        public List<Exception> Errors { get; } = [];

        /// <summary>Gets the number of times <see cref="OnCompleted"/> was called.</summary>
        public int Completed { get; private set; }

        /// <inheritdoc/>
        public void OnNext(T value) => Values.Add(value);

        /// <inheritdoc/>
        public void OnError(Exception error) => Errors.Add(error);

        /// <inheritdoc/>
        public void OnCompleted() => Completed++;
    }
}
