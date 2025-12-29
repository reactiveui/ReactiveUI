// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Tests.Infrastructure.StaticState;

namespace ReactiveUI.Tests.Core;

[NotInParallel]
public class AwaiterTest : IDisposable
{
    private RxAppSchedulersScope? _schedulersScope;

    [Before(Test)]
    public void SetUp()
    {
        _schedulersScope = new RxAppSchedulersScope();
    }

    [After(Test)]
    public void TearDown()
    {
        _schedulersScope?.Dispose();
    }

    /// <summary>
    /// A smoke test for Awaiters.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task AwaiterSmokeTest()
    {
        var fixture = AwaitAnObservable();
        fixture.Wait();

        await Assert.That(fixture.Result).IsEqualTo(42);
    }

    public void Dispose()
    {
        _schedulersScope?.Dispose();
        _schedulersScope = null;
    }

    private static async Task<int> AwaitAnObservable()
    {
        var o = Observable.Start(
            static () =>
            {
                Thread.Sleep(1000);
                return 42;
            },
            RxApp.TaskpoolScheduler);

        return await o;
    }
}
