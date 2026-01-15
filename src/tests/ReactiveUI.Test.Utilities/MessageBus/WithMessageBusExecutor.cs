// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Tests.Utilities.Schedulers;

namespace ReactiveUI.Tests.Utilities.MessageBus;

/// <summary>
///     Test executor that sets up an isolated MessageBus for test duration.
/// </summary>
public class WithMessageBusExecutor : ITestExecutor
{
    /// <inheritdoc />
    public async ValueTask ExecuteTest(TestContext context, Func<ValueTask> testAction)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(testAction);

        var scheduler = ImmediateScheduler.Instance;
        var virtualTimeScheduler = new VirtualTimeScheduler();
        var testBus = new ReactiveUI.MessageBus();
        var previousBus = ReactiveUI.MessageBus.Current;

        context.StateBag.Items["Scheduler"] = scheduler;
        context.StateBag.Items["VirtualTimeScheduler"] = virtualTimeScheduler;
        context.StateBag.Items["MessageBus"] = testBus;

        // Force-reset any previous builder state to avoid waiting deadlocks.
        RxAppBuilder.ResetForTesting();

        _ = RxAppBuilder.CreateReactiveUIBuilder()
            .WithMainThreadScheduler(scheduler)
            .WithTaskPoolScheduler(scheduler)
            .WithMessageBus(testBus)
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
