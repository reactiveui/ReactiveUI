// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Tests.Infrastructure.StaticState;

namespace ReactiveUI.Tests;

/// <summary>
/// Tests the awaiters.
/// </summary>
/// <remarks>
/// This test fixture is marked as NonParallelizable because it accesses RxApp.TaskpoolScheduler,
/// which is global static state. While this test only reads the scheduler, marking it as
/// NonParallelizable ensures no interference with other tests that might modify scheduler state.
/// </remarks>
[TestFixture]
[NonParallelizable]
public class AwaiterTest
{
    private RxAppSchedulersScope? _schedulersScope;

    [SetUp]
    public void SetUp()
    {
        _schedulersScope = new RxAppSchedulersScope();
    }

    [TearDown]
    public void TearDown()
    {
        _schedulersScope?.Dispose();
    }
    /// <summary>
    /// A smoke test for Awaiters.
    /// </summary>
    [Test]
    [SuppressMessage("Usage", "xUnit1031:Do not use blocking task operations in test method", Justification = "Intentional")]
    public void AwaiterSmokeTest()
    {
        var fixture = AwaitAnObservable();
        fixture.Wait();

        Assert.That(fixture.Result, Is.EqualTo(42));
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

        var ret = await o;
        return ret;
    }
}
