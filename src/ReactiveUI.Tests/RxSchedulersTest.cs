// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Microsoft.Reactive.Testing;
using NUnit.Framework;

namespace ReactiveUI.Tests;

/// <summary>
/// Tests the RxSchedulers class to ensure it works without RequiresUnreferencedCode attributes.
/// </summary>
public class RxSchedulersTest
{
    /// <summary>
    /// Tests that schedulers can be accessed without attributes.
    /// </summary>
    [Test]
    public void SchedulersCanBeAccessedWithoutAttributes()
    {
        // This test method itself should not require RequiresUnreferencedCode
        // because it uses RxSchedulers instead of RxApp
        var mainScheduler = RxSchedulers.MainThreadScheduler;
        var taskpoolScheduler = RxSchedulers.TaskpoolScheduler;

        using (Assert.EnterMultipleScope())
        {
            Assert.That(mainScheduler, Is.Not.Null);
            Assert.That(taskpoolScheduler, Is.Not.Null);
        }
    }

    /// <summary>
    /// Tests that schedulers can be set and retrieved.
    /// </summary>
    [Test]
    public void SchedulersCanBeSetAndRetrieved()
    {
        // Store original schedulers to ensure test isolation
        var originalMainScheduler = RxSchedulers.MainThreadScheduler;
        var originalTaskpoolScheduler = RxSchedulers.TaskpoolScheduler;

        try
        {
            var testScheduler = new TestScheduler();

            // Set schedulers
            RxSchedulers.MainThreadScheduler = testScheduler;
            RxSchedulers.TaskpoolScheduler = testScheduler;

            using (Assert.EnterMultipleScope())
            {
                // Verify they were set
                Assert.That(RxSchedulers.MainThreadScheduler, Is.EqualTo(testScheduler));
                Assert.That(RxSchedulers.TaskpoolScheduler, Is.EqualTo(testScheduler));
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
    /// Tests that RxSchedulers provides basic scheduler functionality.
    /// </summary>
    [Test]
    public void SchedulersProvideBasicFunctionality()
    {
        var mainScheduler = RxSchedulers.MainThreadScheduler;
        var taskpoolScheduler = RxSchedulers.TaskpoolScheduler;

        using (Assert.EnterMultipleScope())
        {
            // Verify they implement IScheduler
            Assert.That(mainScheduler, Is.AssignableTo<IScheduler>());
            Assert.That(taskpoolScheduler, Is.AssignableTo<IScheduler>());

            // Verify they have Now property - only check if not using TestScheduler
            // TestScheduler.Now returns DateTimeOffset.MinValue by design
            if (mainScheduler is not TestScheduler)
            {
                Assert.That(mainScheduler.Now, Is.GreaterThan(DateTimeOffset.MinValue));
            }

            if (taskpoolScheduler is not TestScheduler)
            {
                Assert.That(taskpoolScheduler.Now, Is.GreaterThan(DateTimeOffset.MinValue));
            }
        }
    }
}
