// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics;
using ReactiveUI.Tests.Infrastructure.StaticState;

namespace ReactiveUI.Tests;

/// <summary>
/// Tests the RxApp class.
/// </summary>
/// <remarks>
/// This test fixture is marked as NonParallelizable because it accesses
/// RxApp.MainThreadScheduler static property, which is global state that
/// must not be accessed concurrently by parallel tests.
/// </remarks>
[TestFixture]
[NonParallelizable]
public class RxAppTest
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
    /// Tests that schedulers should be current thread in test runner.
    /// </summary>
    [Test]
    public void SchedulerShouldBeCurrentThreadInTestRunner()
    {
        Debug.WriteLine(RxApp.MainThreadScheduler.GetType().FullName);
        Assert.That(RxApp.MainThreadScheduler, Is.EqualTo(CurrentThreadScheduler.Instance));
    }
}
