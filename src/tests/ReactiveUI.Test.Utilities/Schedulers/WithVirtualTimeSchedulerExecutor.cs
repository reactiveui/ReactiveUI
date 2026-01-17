// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Tests.Utilities.AppBuilder;

namespace ReactiveUI.Tests.Utilities.Schedulers;

/// <summary>
///     Test executor that wraps test execution with VirtualTimeScheduler.
///     Provides deterministic time control for testing time-dependent behavior.
/// </summary>
public class WithVirtualTimeSchedulerExecutor : BaseAppBuilderTestExecutor
{
    /// <inheritdoc />
    protected override void ConfigureAppBuilder(IReactiveUIBuilder builder, TestContext context)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(context);

        // Ensure TestContext.Current is set in case of async flow issues
        context.RestoreExecutionContext();

        var scheduler = new VirtualTimeScheduler();

        // Store both in StateBag for retrieval by tests
        context.StateBag.Items["VirtualTimeScheduler"] = scheduler;
        context.StateBag.Items["Scheduler"] = scheduler;

        // Configure builder with virtual time scheduler and core services
        builder
            .WithMainThreadScheduler(scheduler)
            .WithTaskPoolScheduler(scheduler)
            .WithCoreServices();
    }
}
