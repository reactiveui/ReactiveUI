// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Utilities.Schedulers;

/// <summary>Extensions for accessing test utilities from TestContext.</summary>
public static class TestContextExtensions
{
    /// <summary>Extension methods for the TestContext.</summary>
    /// <param name="context">The test context.</param>
    extension(TestContext? context)
    {
        /// <summary>Gets the scheduler configured for this test (ImmediateScheduler).</summary>
        /// <returns>The scheduler instance.</returns>
        public ISequencer GetScheduler()
        {
            ArgumentNullException.ThrowIfNull(context);
            return (ISequencer)(context.StateBag.Items["Scheduler"] ?? Sequencer.Immediate);
        }

        /// <summary>Gets the VirtualTimeScheduler configured for this test. Only available when using WithVirtualTimeSchedulerExecutor.</summary>
        /// <returns>The VirtualTimeScheduler instance.</returns>
        /// <exception cref="InvalidOperationException">Thrown when VirtualTimeScheduler is not configured.</exception>
        public VirtualTimeScheduler GetVirtualTimeScheduler()
        {
            ArgumentNullException.ThrowIfNull(context);
            return (VirtualTimeScheduler)(context.StateBag.Items["VirtualTimeScheduler"]
                                          ?? throw new InvalidOperationException(
                                              "VirtualTimeScheduler not configured. Use [TestExecutor<WithVirtualTimeSchedulerExecutor>] on the test method or class."));
        }
    }
}
