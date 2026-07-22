// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Tests.Utilities.Schedulers;
using TUnit.Core.Executors;

namespace ReactiveUI.Tests;

/// <summary>Tests for <see cref="ScheduledSubject{T}" />.</summary>
public class ScheduledSubjectTest
{
    /// <summary>Tests that constructor with default observer sends values to it.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task Constructor_WithDefaultObserver_SendsValuesToIt()
    {
        var scheduler = Sequencer.Immediate;
        var results = new List<int>();
        var defaultObserver = TestObserver.Create<int>(results.Add);

        const int SecondValue = 2;
        const int ExpectedCount = 2;
        var subject = new ScheduledSubject<int>(scheduler, defaultObserver);
        subject.OnNext(1);
        subject.OnNext(SecondValue);

        await Assert.That(results).Count().IsEqualTo(ExpectedCount);
    }

    /// <summary>Tests that Dispose cleans up resources.</summary>
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

    /// <summary>Tests that OnCompleted completes the observable.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<WithVirtualTimeSchedulerExecutor>]
    public async Task OnCompleted_CompletesObservable()
    {
        var scheduler = TestContext.Current.GetVirtualTimeScheduler();
        var subject = new ScheduledSubject<int>(scheduler);
        var completed = false;

        _ = subject.Subscribe(static _ => { }, () => completed = true);
        subject.OnCompleted();
        scheduler.Start();

        await Assert.That(completed).IsTrue();
    }

    /// <summary>Tests that OnError sends error to observers.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<WithVirtualTimeSchedulerExecutor>]
    public async Task OnError_SendsErrorToObservers()
    {
        var scheduler = TestContext.Current.GetVirtualTimeScheduler();
        var subject = new ScheduledSubject<int>(scheduler);
        Exception? receivedError = null;

        _ = subject.Subscribe(static _ => { }, ex => receivedError = ex);
        var error = new InvalidOperationException("Test error");
        subject.OnError(error);
        scheduler.Start();

        await Assert.That(receivedError).IsEqualTo(error);
    }

    /// <summary>Tests that OnNext emits values.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<WithVirtualTimeSchedulerExecutor>]
    public async Task OnNext_EmitsValues()
    {
        var scheduler = TestContext.Current.GetVirtualTimeScheduler();
        var subject = new ScheduledSubject<int>(scheduler);
        var results = new List<int>();

        const int SecondValue = 2;
        const int ExpectedCount = 2;
        _ = subject.Subscribe(results.Add);
        subject.OnNext(1);
        subject.OnNext(SecondValue);
        scheduler.Start();

        await Assert.That(results).Count().IsEqualTo(ExpectedCount);
        await Assert.That(results[0]).IsEqualTo(1);
        await Assert.That(results[1]).IsEqualTo(SecondValue);
    }

    /// <summary>Tests that Subscribe returns a disposable.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<WithVirtualTimeSchedulerExecutor>]
    public async Task Subscribe_ReturnsDisposable()
    {
        var scheduler = TestContext.Current.GetVirtualTimeScheduler();
        var subject = new ScheduledSubject<int>(scheduler);

        var subscription = subject.Subscribe(static _ => { });

        await Assert.That(subscription).IsNotNull();
        subscription.Dispose();
    }

    /// <summary>Tests that values are scheduled on the specified scheduler.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task Subscribe_SchedulesOnSpecifiedScheduler()
    {
        var scheduler = new VirtualTimeScheduler();
        var subject = new ScheduledSubject<int>(scheduler);
        var results = new List<int>();

        _ = subject.Subscribe(results.Add);
        subject.OnNext(1);
        await Assert.That(results).IsEmpty();

        scheduler.Start();

        using (Assert.Multiple())
        {
            await Assert.That(results).Count().IsEqualTo(1);
        }
    }

    /// <summary>Tests that subscription disposal stops receiving values.</summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    public async Task Subscription_WhenDisposed_StopsReceivingValues()
    {
        var scheduler = Sequencer.Immediate;
        var subject = new ScheduledSubject<int>(scheduler);
        var results = new List<int>();

        const int ValueAfterDispose = 2;
        var subscription = subject.Subscribe(results.Add);
        subject.OnNext(1);
        subscription.Dispose();
        subject.OnNext(ValueAfterDispose);

        await Assert.That(results).Count().IsEqualTo(1);
    }
}
