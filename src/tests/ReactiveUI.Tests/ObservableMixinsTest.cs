// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVE_SHIM
using ReactiveUI.Reactive.Builder;
#else
using ReactiveUI.Builder;
#endif

namespace ReactiveUI.Tests;

/// <summary>Tests for <see cref="ObservableMixins" />.</summary>
public class ObservableMixinsTest
{
    /// <summary>
    /// Verifies that <see cref="ObservableMixins.WhereNotNull{T}(IObservable{T})" /> can be used before ReactiveUI has
    /// been initialized through the builder. The helper is pure and must not force runtime initialization.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task WhereNotNullDoesNotRequireBuilderInitialization()
    {
        RxAppBuilder.ResetForTesting();

        var subject = new Signal<int?>();
        var results = new List<int?>();

        const int SecondValue = 2;
        const int ExpectedCount = 2;

        _ = ObservableMixins.WhereNotNull(subject).Subscribe(results.Add);

        subject.OnNext(1);
        subject.OnNext(null);
        subject.OnNext(SecondValue);

        await Assert.That(results).Count().IsEqualTo(ExpectedCount);
    }

    /// <summary>Tests that WhereNotNull emits all non-null values.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task WhereNotNull_EmitsAllNonNullValues()
    {
        var subject = new Signal<int?>();
        var results = new List<int?>();

        const int SecondValue = 2;
        const int ThirdValue = 3;
        const int ExpectedCount = 3;
        const int ThirdIndex = 2;

        _ = ObservableMixins.WhereNotNull(subject).ObserveOn(Sequencer.Immediate).Subscribe(results.Add);

        subject.OnNext(1);
        subject.OnNext(SecondValue);
        subject.OnNext(ThirdValue);

        await Assert.That(results).Count().IsEqualTo(ExpectedCount);
        await Assert.That(results[0]).IsEqualTo(1);
        await Assert.That(results[1]).IsEqualTo(SecondValue);
        await Assert.That(results[ThirdIndex]).IsEqualTo(ThirdValue);
    }

    /// <summary>Tests that WhereNotNull filters out null values.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task WhereNotNull_FiltersNullValues()
    {
        var subject = new Signal<string?>();
        var results = new List<string>();

        _ = ObservableMixins.WhereNotNull(subject).ObserveOn(Sequencer.Immediate).Subscribe(results.Add);

        subject.OnNext("value1");
        subject.OnNext(null);
        subject.OnNext("value2");
        subject.OnNext(null);
        subject.OnNext("value3");

        const int ExpectedCount = 3;
        const int ThirdIndex = 2;
        await Assert.That(results).Count().IsEqualTo(ExpectedCount);
        await Assert.That(results[0]).IsEqualTo("value1");
        await Assert.That(results[1]).IsEqualTo("value2");
        await Assert.That(results[ThirdIndex]).IsEqualTo("value3");
    }

    /// <summary>Tests that WhereNotNull emits nothing when only nulls are sent.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task WhereNotNull_WithOnlyNulls_EmitsNothing()
    {
        var subject = new Signal<string?>();
        var results = new List<string>();

        _ = ObservableMixins.WhereNotNull(subject).ObserveOn(Sequencer.Immediate).Subscribe(results.Add);

        subject.OnNext(null);
        subject.OnNext(null);
        subject.OnNext(null);

        await Assert.That(results).Count().IsEqualTo(0);
    }

    /// <summary>Tests that WhereNotNull works with reference types.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task WhereNotNull_WorksWithReferenceTypes()
    {
        var subject = new Signal<TestClass?>();
        var results = new List<TestClass>();
        var obj1 = new TestClass { Value = "test1" };
        var obj2 = new TestClass { Value = "test2" };

        _ = ObservableMixins.WhereNotNull(subject).ObserveOn(Sequencer.Immediate).Subscribe(results.Add);

        subject.OnNext(obj1);
        subject.OnNext(null);
        subject.OnNext(obj2);

        const int ExpectedCount = 2;
        await Assert.That(results).Count().IsEqualTo(ExpectedCount);
        await Assert.That(results[0]).IsEqualTo(obj1);
        await Assert.That(results[1]).IsEqualTo(obj2);
    }

    /// <summary>Tests that WhereNotNull forwards completion from the source.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task WhereNotNullForwardsCompletion()
    {
        var subject = new Signal<int?>();
        var completed = false;

        using var subscription = ObservableMixins.WhereNotNull(subject)
            .Subscribe(static _ => { }, static _ => { }, () => completed = true);

        subject.OnCompleted();

        await Assert.That(completed).IsTrue();
    }

    /// <summary>Tests that WhereNotNull forwards errors from the source.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task WhereNotNullForwardsError()
    {
        var subject = new Signal<int?>();
        var expected = new InvalidOperationException("source failure");
        Exception? captured = null;

        using var subscription = ObservableMixins.WhereNotNull(subject)
            .Subscribe(static _ => { }, ex => captured = ex, static () => { });

        subject.OnError(expected);

        await Assert.That(captured).IsSameReferenceAs(expected);
    }

    /// <summary>Tests that the deferred-value helper surfaces a factory exception as an <c>OnError</c> notification.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task DeferredValueObservableForwardsFactoryExceptionAsOnError()
    {
        var expected = new InvalidOperationException("factory failure");
        var observable = new ObservableMixins.DeferredValueObservable<int>(() => throw expected);
        Exception? captured = null;

        using var subscription = observable.Subscribe(static _ => { }, ex => captured = ex, static () => { });

        await Assert.That(captured).IsSameReferenceAs(expected);
    }

    /// <summary>Tests that the parameterless action overload runs the action and completes.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task FromAsyncWithAllNotificationsRunsAction()
    {
        var source = ObservableMixins.FromAsyncWithAllNotifications(static _ => Task.CompletedTask);

        var result = await RunToCompletionAsync(source);

        await Assert.That(result).IsEqualTo(RxVoid.Default);
    }

    /// <summary>Tests that the parameterized action overload passes the parameter and completes.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task FromAsyncWithAllNotificationsWithParameterRunsAction()
    {
        const int Parameter = 7;
        var observed = 0;

        var source = ObservableMixins.FromAsyncWithAllNotifications(
            (param, _) =>
            {
                observed = param;
                return Task.CompletedTask;
            },
            Parameter);

        _ = await RunToCompletionAsync(source);

        await Assert.That(observed).IsEqualTo(Parameter);
    }

    /// <summary>Tests that the result-producing overload emits the produced value.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task FromAsyncWithAllNotificationsProducesResult()
    {
        const int Expected = 42;

        var source = ObservableMixins.FromAsyncWithAllNotifications(static _ => Task.FromResult(Expected));

        var result = await RunToCompletionAsync(source);

        await Assert.That(result).IsEqualTo(Expected);
    }

    /// <summary>Tests that the parameterized result-producing overload emits the produced value.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task FromAsyncWithAllNotificationsWithParameterProducesResult()
    {
        const int Parameter = 21;
        const int Multiplier = 2;
        const int Expected = 42;

        var source = ObservableMixins.FromAsyncWithAllNotifications(
            static (param, _) => Task.FromResult(param * Multiplier),
            Parameter);

        var result = await RunToCompletionAsync(source);

        await Assert.That(result).IsEqualTo(Expected);
    }

    /// <summary>Tests that a faulted asynchronous action surfaces its exception as an <c>OnError</c> notification.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task FromAsyncWithAllNotificationsPropagatesError()
    {
        var expected = new InvalidOperationException("async failure");

        var source = ObservableMixins.FromAsyncWithAllNotifications(
            static (ex, _) => Task.FromException<int>(ex),
            expected);

        var (result, _) = Materialize(source);

        var completion = new TaskCompletionSource<Exception>(TaskCreationOptions.RunContinuationsAsynchronously);
        using var subscription = result.Subscribe(
            static _ => { },
            ex => completion.TrySetResult(ex),
            static () => { });

        var actual = await completion.Task;

        await Assert.That(actual).IsSameReferenceAs(expected);
    }

    /// <summary>Tests that the cancel delegate returned alongside the result can be invoked without throwing.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task FromAsyncWithAllNotificationsCancelDelegateIsInvokable()
    {
        var started = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var release = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

        var source = ObservableMixins.FromAsyncWithAllNotifications(
            static async (state, ct) =>
            {
                var (start, rel) = state;
                _ = start.TrySetResult();
                await rel.Task.ConfigureAwait(false);
                ct.ThrowIfCancellationRequested();
                return 0;
            },
            (started, release));

        var (result, cancel) = Materialize(source);

        var completion = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        using var subscription = result.Subscribe(
            static _ => { },
            _ => completion.TrySetResult(),
            () => completion.TrySetResult());

        await started.Task;
        cancel();
        _ = release.TrySetResult();

        await completion.Task;

        await Assert.That(completion.Task.IsCompletedSuccessfully).IsTrue();
    }

    /// <summary>Subscribes to a deferred source and returns the single tuple it emits synchronously on subscription.</summary>
    /// <typeparam name="TResult">The result type produced by the inner observable.</typeparam>
    /// <param name="source">The deferred source to materialize.</param>
    /// <returns>The result observable and its cancellation delegate.</returns>
    private static (IObservable<TResult> Result, Action Cancel) Materialize<TResult>(
        IObservable<(IObservable<TResult> Result, Action Cancel)> source)
    {
        (IObservable<TResult> Result, Action Cancel)? captured = null;
        using var subscription = source.Subscribe(tuple => captured = tuple);

        return captured ?? throw new InvalidOperationException("The deferred source did not emit a value synchronously.");
    }

    /// <summary>Runs a deferred asynchronous source to completion and returns the single emitted result.</summary>
    /// <typeparam name="TResult">The result type produced by the inner observable.</typeparam>
    /// <param name="source">The deferred source to run.</param>
    /// <returns>The single value emitted by the inner observable.</returns>
    private static async Task<TResult> RunToCompletionAsync<TResult>(
        IObservable<(IObservable<TResult> Result, Action Cancel)> source)
    {
        var (result, _) = Materialize(source);

        var completion = new TaskCompletionSource<TResult>(TaskCreationOptions.RunContinuationsAsynchronously);
        var last = default(TResult)!;
        using var subscription = result.Subscribe(
            value => last = value,
            ex => completion.TrySetException(ex),
            () => completion.TrySetResult(last));

        return await completion.Task;
    }

    /// <summary>Test class for reference type testing.</summary>
    private sealed class TestClass
    {
        /// <summary>Gets or sets the test value.</summary>
        public string? Value { get; set; }
    }
}
