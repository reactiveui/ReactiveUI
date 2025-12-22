// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics;

using ReactiveUI.Tests.Infrastructure.StaticState;

using TUnit.Assertions;
using TUnit.Assertions.Extensions;
using TUnit.Core;

using static TUnit.Assertions.Assert;

namespace ReactiveUI.Tests.Core;
[NonParallelizable]
public class RxAppTest : IDisposable
{
    private RxAppSchedulersScope? _schedulersScope;

    [Before(HookType.Test)]
    public void SetUp()
    {
        _schedulersScope = new RxAppSchedulersScope();
    }

    [After(HookType.Test)]
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

    public void Dispose()
    {
        TearDown();
    }
}