// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Utilities.Schedulers;

/// <summary>
///     Extensions for accessing test utilities from TestContext.
/// </summary>
public static class TestContextExtensions
{
    /// <summary>
    ///     Gets the scheduler configured for this test (ImmediateScheduler).
    /// </summary>
    /// <param name="context">The test context.</param>
    /// <returns>The scheduler instance.</returns>
    public static IScheduler GetScheduler(this TestContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        return (IScheduler)(context.StateBag.Items["Scheduler"] ?? ImmediateScheduler.Instance);
    }

    /// <summary>
    ///     Gets the VirtualTimeScheduler configured for this test.
    ///     Only available when using WithVirtualTimeSchedulerExecutor.
    /// </summary>
    /// <param name="context">The test context.</param>
    /// <returns>The VirtualTimeScheduler instance.</returns>
    /// <exception cref="InvalidOperationException">Thrown when VirtualTimeScheduler is not configured.</exception>
    public static VirtualTimeScheduler GetVirtualTimeScheduler(this TestContext? context)
    {
        ArgumentNullException.ThrowIfNull(context);
        return (VirtualTimeScheduler)(context.StateBag.Items["VirtualTimeScheduler"]
            ?? throw new InvalidOperationException("VirtualTimeScheduler not configured. Use [TestExecutor<WithVirtualTimeSchedulerExecutor>] on the test method or class."));
    }
}
