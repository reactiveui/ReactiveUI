// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Commands;

/// <summary>Tests for the fused sinks that <see cref="CombinedReactiveCommand{TParam, TResult}"/> combines its child commands with.</summary>
public class CombinedReactiveCommandSinkTests
{
    /// <summary>Verifies that the can-execute sink completes once every source has completed.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task AllTrueCanExecuteObservable_AllSourcesComplete_CompletesDownstream()
    {
        var first = new Signal<bool>();
        var second = new Signal<bool>();
        var observer = new RecordingObserver<bool>();

        using var subscription = new CombinedReactiveCommand<RxVoid, RxVoid>.AllTrueCanExecuteObservable([first, second]).Subscribe(observer);

        first.OnCompleted();
        var completedAfterFirst = observer.CompletedCount;
        second.OnCompleted();

        await Assert.That(completedAfterFirst).IsEqualTo(0);
        await Assert.That(observer.CompletedCount).IsEqualTo(1);
    }

    /// <summary>Verifies that the can-execute sink forwards only the first error and ignores later terminal signals.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task AllTrueCanExecuteObservable_ErrorThenFurtherTerminalSignals_ForwardsFirstErrorOnly()
    {
        var first = new Signal<bool>();
        var second = new Signal<bool>();
        var third = new Signal<bool>();
        var observer = new RecordingObserver<bool>();
        var failure = new InvalidOperationException("first failure");

        using var subscription = new CombinedReactiveCommand<RxVoid, RxVoid>.AllTrueCanExecuteObservable([first, second, third]).Subscribe(observer);

        first.OnError(failure);
        second.OnError(new InvalidOperationException("second failure"));
        third.OnCompleted();

        await Assert.That(observer.Errors).Count().IsEqualTo(1);
        await Assert.That(observer.Errors[0]).IsSameReferenceAs(failure);
        await Assert.That(observer.CompletedCount).IsEqualTo(0);
    }

    /// <summary>Verifies that the merged-exceptions sink forwards values and completes once every source has completed.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task MergedExceptionsObservable_ValuesThenAllSourcesComplete_ForwardsValuesAndCompletes()
    {
        var first = new Signal<Exception>();
        var second = new Signal<Exception>();
        var observer = new RecordingObserver<Exception>();
        var value = new InvalidOperationException("value");

        using var subscription = new CombinedReactiveCommand<RxVoid, RxVoid>.MergedExceptionsObservable([first, second]).Subscribe(observer);

        first.OnNext(value);
        first.OnCompleted();
        var completedAfterFirst = observer.CompletedCount;
        second.OnCompleted();

        await Assert.That(observer.Values).Count().IsEqualTo(1);
        await Assert.That(observer.Values[0]).IsSameReferenceAs(value);
        await Assert.That(completedAfterFirst).IsEqualTo(0);
        await Assert.That(observer.CompletedCount).IsEqualTo(1);
    }

    /// <summary>Verifies that the merged-exceptions sink forwards a source error once and then stops forwarding values.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task MergedExceptionsObservable_SourceErrors_ForwardsErrorAndStops()
    {
        var first = new Signal<Exception>();
        var second = new Signal<Exception>();
        var observer = new RecordingObserver<Exception>();
        var failure = new InvalidOperationException("failure");

        using var subscription = new CombinedReactiveCommand<RxVoid, RxVoid>.MergedExceptionsObservable([first, second]).Subscribe(observer);

        first.OnError(failure);
        second.OnNext(new InvalidOperationException("late value"));

        await Assert.That(observer.Errors).Count().IsEqualTo(1);
        await Assert.That(observer.Errors[0]).IsSameReferenceAs(failure);
        await Assert.That(observer.Values).IsEmpty();
    }

    /// <summary>Verifies that disposing the merged-exceptions subscription stops forwarding values.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task MergedExceptionsObservable_Disposed_StopsForwardingValues()
    {
        var source = new Signal<Exception>();
        var observer = new RecordingObserver<Exception>();

        var subscription = new CombinedReactiveCommand<RxVoid, RxVoid>.MergedExceptionsObservable([source]).Subscribe(observer);
        subscription.Dispose();

        source.OnNext(new InvalidOperationException("late value"));

        await Assert.That(observer.Values).IsEmpty();
    }

    /// <summary>Verifies that a sink disposed before it subscribes to its sources disposes nothing and does not throw.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task MergedExceptionsObservable_SinkDisposedBeforeRun_DoesNotThrow()
    {
        var observer = new RecordingObserver<Exception>();
        var sink = new CombinedReactiveCommand<RxVoid, RxVoid>.MergedExceptionsObservable.Sink(observer, [new Signal<Exception>()]);

        await Assert.That(sink.Dispose).ThrowsNothing();
        await Assert.That(observer.CompletedCount).IsEqualTo(0);
    }

    /// <summary>An observer that records every notification it receives.</summary>
    /// <typeparam name="T">The value type.</typeparam>
    private sealed class RecordingObserver<T> : IObserver<T>
    {
        /// <summary>Gets the values received.</summary>
        public List<T> Values { get; } = [];

        /// <summary>Gets the errors received.</summary>
        public List<Exception> Errors { get; } = [];

        /// <summary>Gets the number of completions received.</summary>
        public int CompletedCount { get; private set; }

        /// <inheritdoc/>
        public void OnNext(T value) => Values.Add(value);

        /// <inheritdoc/>
        public void OnError(Exception error) => Errors.Add(error);

        /// <inheritdoc/>
        public void OnCompleted() => CompletedCount++;
    }
}
