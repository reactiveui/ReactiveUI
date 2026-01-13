// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests;

/// <summary>
///     Tests for <see cref="ScheduledSubject{T}" />.
/// </summary>
public class ScheduledSubjectTest
{
    /// <summary>
    ///     Tests that constructor with default observer sends values to it.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task Constructor_WithDefaultObserver_SendsValuesToIt()
    {
        var scheduler = ImmediateScheduler.Instance;
        var results = new List<int>();
        var defaultObserver = Observer.Create<int>(results.Add);

        var subject = new ScheduledSubject<int>(scheduler, defaultObserver);
        subject.OnNext(1);
        subject.OnNext(2);

        await Assert.That(results).Count().IsEqualTo(2);
    }

    /// <summary>
    ///     Tests that Dispose cleans up resources.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]

    [TestExecutor<WithVirtualTimeSchedulerExecutor>]
    public async Task Dispose_CleansUpResources()
    {
        var scheduler = TestContext.Current.GetVirtualTimeScheduler();
        var subject = new ScheduledSubject<int>(scheduler);

        subject.Dispose();

        await Task.CompletedTask;
    }

    /// <summary>
    ///     Tests that OnCompleted completes the observable.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]

    [TestExecutor<WithVirtualTimeSchedulerExecutor>]
    public async Task OnCompleted_CompletesObservable()
    {
        var scheduler = TestContext.Current.GetVirtualTimeScheduler();
        var subject = new ScheduledSubject<int>(scheduler);
        var completed = false;

        subject.Subscribe(_ => { }, () => completed = true);
        subject.OnCompleted();
        scheduler.Start();

        await Assert.That(completed).IsTrue();
    }

    /// <summary>
    ///     Tests that OnError sends error to observers.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]

    [TestExecutor<WithVirtualTimeSchedulerExecutor>]
    public async Task OnError_SendsErrorToObservers()
    {
        var scheduler = TestContext.Current.GetVirtualTimeScheduler();
        var subject = new ScheduledSubject<int>(scheduler);
        Exception? receivedError = null;

        subject.Subscribe(_ => { }, ex => receivedError = ex);
        var error = new InvalidOperationException("Test error");
        subject.OnError(error);
        scheduler.Start();

        await Assert.That(receivedError).IsEqualTo(error);
    }

    /// <summary>
    ///     Tests that OnNext emits values.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]

    [TestExecutor<WithVirtualTimeSchedulerExecutor>]
    public async Task OnNext_EmitsValues()
    {
        var scheduler = TestContext.Current.GetVirtualTimeScheduler();
        var subject = new ScheduledSubject<int>(scheduler);
        var results = new List<int>();

        subject.Subscribe(results.Add);
        subject.OnNext(1);
        subject.OnNext(2);
        scheduler.Start();

        await Assert.That(results).Count().IsEqualTo(2);
        await Assert.That(results[0]).IsEqualTo(1);
        await Assert.That(results[1]).IsEqualTo(2);
    }

    /// <summary>
    ///     Tests that Subscribe returns a disposable.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]

    [TestExecutor<WithVirtualTimeSchedulerExecutor>]
    public async Task Subscribe_ReturnsDisposable()
    {
        var scheduler = TestContext.Current.GetVirtualTimeScheduler();
        var subject = new ScheduledSubject<int>(scheduler);

        var subscription = subject.Subscribe(_ => { });

        await Assert.That(subscription).IsNotNull();
        subscription.Dispose();
    }

    /// <summary>
    ///     Tests that values are scheduled on the specified scheduler.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]

    [TestExecutor<WithVirtualTimeSchedulerExecutor>]
    public async Task Subscribe_SchedulesOnSpecifiedScheduler()
    {
        var scheduler = TestContext.Current.GetVirtualTimeScheduler();
        var subject = new ScheduledSubject<int>(scheduler);
        var results = new List<int>();

        subject.Subscribe(results.Add);
        subject.OnNext(1);

        // Before advancing scheduler, nothing should be received
        await Assert.That(results).Count().IsEqualTo(0);

        scheduler.Start();

        // After advancing scheduler, value should be received
        await Assert.That(results).Count().IsEqualTo(1);
    }

    /// <summary>
    ///     Tests that subscription disposal stops receiving values.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task Subscription_WhenDisposed_StopsReceivingValues()
    {
        var scheduler = ImmediateScheduler.Instance;
        var subject = new ScheduledSubject<int>(scheduler);
        var results = new List<int>();

        var subscription = subject.Subscribe(results.Add);
        subject.OnNext(1);
        subscription.Dispose();
        subject.OnNext(2);

        await Assert.That(results).Count().IsEqualTo(1);
    }
}
