// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Microsoft.Reactive.Testing;

namespace ReactiveUI.Testing.Tests;

/// <summary>
/// Tests for SchedulerExtensions.
/// </summary>
public sealed class SchedulerExtensionTests
{
    /// <summary>
    /// Tests that WithScheduler sets both RxApp and RxSchedulers schedulers.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task WithScheduler_ShouldSetBothRxAppAndRxSchedulersSchedulers()
    {
        var testScheduler = new TestScheduler();
        var originalMainThread = RxApp.MainThreadScheduler;
        var originalTaskpool = RxApp.TaskpoolScheduler;
        var originalRxSchedulersMain = RxSchedulers.MainThreadScheduler;
        var originalRxSchedulersTask = RxSchedulers.TaskpoolScheduler;

        using (SchedulerExtensions.WithScheduler(testScheduler))
        {
            // Verify schedulers
            using (Assert.Multiple())
            {
                // Verify RxApp schedulers are set
                await Assert.That(RxApp.MainThreadScheduler).IsEqualTo(testScheduler);
                await Assert.That(RxApp.TaskpoolScheduler).IsEqualTo(testScheduler);

                // Verify RxSchedulers are set
                await Assert.That(RxSchedulers.MainThreadScheduler).IsEqualTo(testScheduler);
                await Assert.That(RxSchedulers.TaskpoolScheduler).IsEqualTo(testScheduler);
            }
        }

        // Verify schedulers are restored after disposal
        using (Assert.Multiple())
        {
            await Assert.That(RxApp.MainThreadScheduler).IsEqualTo(originalMainThread);
            await Assert.That(RxApp.TaskpoolScheduler).IsEqualTo(originalTaskpool);
            await Assert.That(RxSchedulers.MainThreadScheduler).IsEqualTo(originalRxSchedulersMain);
            await Assert.That(RxSchedulers.TaskpoolScheduler).IsEqualTo(originalRxSchedulersTask);
        }
    }
}
