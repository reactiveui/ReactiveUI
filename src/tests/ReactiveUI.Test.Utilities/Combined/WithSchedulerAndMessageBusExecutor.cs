// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Utilities.Combined;

/// <summary>
///     Test executor that sets up both scheduler and message bus overrides.
/// </summary>
public class WithSchedulerAndMessageBusExecutor : ITestExecutor
{
    /// <inheritdoc />
    public async ValueTask ExecuteTest(TestContext context, Func<ValueTask> testAction)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(testAction);

        var scheduler = ImmediateScheduler.Instance;
        var messageBus = new ReactiveUI.MessageBus();
        var previousBus = ReactiveUI.MessageBus.Current;

        context.StateBag.Items["Scheduler"] = scheduler;
        context.StateBag.Items["MessageBus"] = messageBus;

        RxAppBuilder.ResetForTesting();

        _ = RxAppBuilder.CreateReactiveUIBuilder()
            .WithMainThreadScheduler(scheduler)
            .WithTaskPoolScheduler(scheduler)
            .WithMessageBus(messageBus)
            .WithCoreServices()
            .BuildApp();

        try
        {
            context.RestoreExecutionContext();
            await testAction();
        }
        finally
        {
            ReactiveUI.MessageBus.Current = previousBus;
            RxAppBuilder.ResetForTesting();
        }
    }
}
