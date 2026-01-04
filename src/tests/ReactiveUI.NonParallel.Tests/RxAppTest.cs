// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics;

using ReactiveUI.Tests.Infrastructure.StaticState;

namespace ReactiveUI.Tests.Core;

[NotInParallel]
public class RxAppTest : IDisposable
{
    private RxSchedulersSchedulersScope? _schedulersScope;

    [Before(Test)]
    public void SetUp()
    {
        _schedulersScope = new RxSchedulersSchedulersScope();
    }

    [After(Test)]
    public void TearDown()
    {
        _schedulersScope?.Dispose();
    }

    /// <summary>
    /// Tests that schedulers should be current thread in test runner.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task SchedulerShouldBeCurrentThreadInTestRunner()
    {
        await Assert.That(RxSchedulers.MainThreadScheduler).IsEqualTo(CurrentThreadScheduler.Instance);
    }

    public void Dispose()
    {
        TearDown();
    }
}
