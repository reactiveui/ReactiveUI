// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Tests.Utilities.AppBuilder;
using ReactiveUI.Tests.Utilities.Schedulers;

namespace ReactiveUI.Tests.Utilities.MessageBus;

/// <summary>
///     Test executor that sets up an isolated MessageBus for test duration.
/// </summary>
public class WithMessageBusExecutor : BaseAppBuilderTestExecutor
{
    private IMessageBus? _previousBus;

    /// <inheritdoc />
    public override async ValueTask ExecuteTest(TestContext context, Func<ValueTask> testAction)
    {
        try
        {
            await base.ExecuteTest(context, testAction);
        }
        finally
        {
            // Restore previous message bus
            if (_previousBus is not null)
            {
                ReactiveUI.MessageBus.Current = _previousBus;
            }
        }
    }

    /// <inheritdoc />
    protected override void ConfigureAppBuilder(IReactiveUIBuilder builder, TestContext context)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(context);

        var scheduler = ImmediateScheduler.Instance;
        var virtualTimeScheduler = new VirtualTimeScheduler();
        var testBus = new ReactiveUI.MessageBus();

        // Save previous bus for restoration
        _previousBus = ReactiveUI.MessageBus.Current;

        // Store test utilities in context
        context.StateBag.Items["Scheduler"] = scheduler;
        context.StateBag.Items["VirtualTimeScheduler"] = virtualTimeScheduler;
        context.StateBag.Items["MessageBus"] = testBus;

        // Ensure TestContext.Current is set
        context.RestoreExecutionContext();

        // Configure builder with schedulers, message bus, and core services
        builder
            .WithMainThreadScheduler(scheduler)
            .WithTaskPoolScheduler(scheduler)
            .WithMessageBus(testBus)
            .WithCoreServices();
    }
}
