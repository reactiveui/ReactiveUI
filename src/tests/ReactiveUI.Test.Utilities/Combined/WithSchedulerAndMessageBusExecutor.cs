// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if REACTIVE_SHIM
using ReactiveUI.Reactive.Builder;
using MessageBusType = ReactiveUI.Reactive.MessageBus;
#else
using ReactiveUI.Builder;
using MessageBusType = ReactiveUI.MessageBus;
#endif
using ReactiveUI.Tests.Utilities.AppBuilder;

namespace ReactiveUI.Tests.Utilities.Combined;

/// <summary>Test executor that sets up both scheduler and message bus overrides.</summary>
public class WithSchedulerAndMessageBusExecutor : BaseAppBuilderTestExecutor
{
    /// <summary>The message bus captured before the test, restored during cleanup.</summary>
    private IMessageBus? _previousBus;

    /// <inheritdoc />
    public override async ValueTask ExecuteTest(TestContext context, Func<ValueTask> action)
    {
        try
        {
            await base.ExecuteTest(context, action);
        }
        finally
        {
            // Restore previous message bus
            if (_previousBus is not null)
            {
                MessageBusType.Current = _previousBus;
            }
        }
    }

    /// <inheritdoc />
    protected override void ConfigureAppBuilder(IReactiveUIBuilder builder, TestContext context)
    {
        ArgumentNullException.ThrowIfNull(builder);
        ArgumentNullException.ThrowIfNull(context);

        var scheduler = Sequencer.Immediate;
        var messageBus = new MessageBusType();

        // Save previous bus for restoration
        _previousBus = MessageBusType.Current;

        // Store test utilities in context
        context.StateBag.Items["Scheduler"] = scheduler;
        context.StateBag.Items["MessageBus"] = messageBus;

        // Ensure TestContext.Current is set
        context.RestoreExecutionContext();

        // Configure builder with schedulers, message bus, and core services
        _ = builder
            .WithMainThreadScheduler(scheduler)
            .WithTaskPoolScheduler(scheduler)
            .WithMessageBus(messageBus)
            .WithCoreServices();
    }
}
