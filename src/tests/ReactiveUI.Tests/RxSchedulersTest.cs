// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Tests.Utilities.Schedulers;

namespace ReactiveUI.Tests;

/// <summary>
///     Tests the RxSchedulers class to ensure it works without RequiresUnreferencedCode attributes.
/// </summary>
[NotInParallel]
public class RxSchedulersTest
{
    /// <summary>
    ///     Tests that schedulers can be accessed without attributes.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<WithSchedulerExecutor>]
    public async Task SchedulersCanBeAccessedWithoutAttributes()
    {
        // This test method itself should not require RequiresUnreferencedCode
        // because it uses RxSchedulers instead of RxApp
        var mainScheduler = RxSchedulers.MainThreadScheduler;
        var taskpoolScheduler = RxSchedulers.TaskpoolScheduler;

        using (Assert.Multiple())
        {
            await Assert.That(mainScheduler).IsNotNull();
            await Assert.That(taskpoolScheduler).IsNotNull();
        }
    }

    /// <summary>
    ///     Tests that schedulers can be set and retrieved.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<WithVirtualTimeSchedulerExecutor>]
    public async Task SchedulersCanBeSetAndRetrieved()
    {
        var testScheduler = TestContext.Current.GetVirtualTimeScheduler();

        // Store original schedulers to ensure test isolation
        var originalMainScheduler = RxSchedulers.MainThreadScheduler;
        var originalTaskpoolScheduler = RxSchedulers.TaskpoolScheduler;

        try
        {
            // Set schedulers
            RxSchedulers.MainThreadScheduler = testScheduler;
            RxSchedulers.TaskpoolScheduler = testScheduler;

            using (Assert.Multiple())
            {
                // Verify they were set
                await Assert.That(RxSchedulers.MainThreadScheduler).IsEqualTo(testScheduler);
                await Assert.That(RxSchedulers.TaskpoolScheduler).IsEqualTo(testScheduler);
            }
        }
        finally
        {
            // Always restore original schedulers to ensure test isolation
            RxSchedulers.MainThreadScheduler = originalMainScheduler;
            RxSchedulers.TaskpoolScheduler = originalTaskpoolScheduler;
        }
    }

    /// <summary>
    ///     Tests that RxSchedulers provides basic scheduler functionality.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<WithSchedulerExecutor>]
    public async Task SchedulersProvideBasicFunctionality()
    {
        var mainScheduler = RxSchedulers.MainThreadScheduler;
        var taskpoolScheduler = RxSchedulers.TaskpoolScheduler;

        using (Assert.Multiple())
        {
            // Verify they implement IScheduler
            await Assert.That(mainScheduler).IsAssignableTo<IScheduler>();
            await Assert.That(taskpoolScheduler).IsAssignableTo<IScheduler>();

            // Verify they have Now property - only check if not using VirtualTimeScheduler
            // VirtualTimeScheduler.Now returns DateTimeOffset.MinValue by design
            if (mainScheduler is not VirtualTimeScheduler)
            {
                await Assert.That(mainScheduler.Now).IsGreaterThan(DateTimeOffset.MinValue);
            }

            if (taskpoolScheduler is not VirtualTimeScheduler)
            {
                await Assert.That(taskpoolScheduler.Now).IsGreaterThan(DateTimeOffset.MinValue);
            }
        }
    }
}
