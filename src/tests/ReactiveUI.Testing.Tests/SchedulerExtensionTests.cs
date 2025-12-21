// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Microsoft.Reactive.Testing;

namespace ReactiveUI.Testing.Tests;

/// <summary>
/// Tests for <see cref="SchedulerExtensions"/>.
/// </summary>
[TestFixture]
public sealed class SchedulerExtensionTests
{
    /// <summary>
    /// Tests that WithScheduler sets both RxApp and RxSchedulers schedulers.
    /// </summary>
    [Test]
    public void WithScheduler_ShouldSetBothRxAppAndRxSchedulersSchedulers()
    {
        var testScheduler = new TestScheduler();
        var originalMainThread = RxApp.MainThreadScheduler;
        var originalTaskpool = RxApp.TaskpoolScheduler;
        var originalRxSchedulersMain = RxSchedulers.MainThreadScheduler;
        var originalRxSchedulersTask = RxSchedulers.TaskpoolScheduler;

        using (SchedulerExtensions.WithScheduler(testScheduler))
        {
            // Verify schedulers
            using (Assert.EnterMultipleScope())
            {
                // Verify RxApp schedulers are set
                Assert.That(RxApp.MainThreadScheduler, Is.EqualTo(testScheduler));
                Assert.That(RxApp.TaskpoolScheduler, Is.EqualTo(testScheduler));

                // Verify RxSchedulers are set
                Assert.That(RxSchedulers.MainThreadScheduler, Is.EqualTo(testScheduler));
                Assert.That(RxSchedulers.TaskpoolScheduler, Is.EqualTo(testScheduler));
            }
        }

        // Verify schedulers are restored after disposal
        using (Assert.EnterMultipleScope())
        {
            Assert.That(RxApp.MainThreadScheduler, Is.EqualTo(originalMainThread));
            Assert.That(RxApp.TaskpoolScheduler, Is.EqualTo(originalTaskpool));
            Assert.That(RxSchedulers.MainThreadScheduler, Is.EqualTo(originalRxSchedulersMain));
            Assert.That(RxSchedulers.TaskpoolScheduler, Is.EqualTo(originalRxSchedulersTask));
        }
    }
}