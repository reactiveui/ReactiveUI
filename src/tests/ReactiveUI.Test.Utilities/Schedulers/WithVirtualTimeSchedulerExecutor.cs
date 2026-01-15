// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Utilities.Schedulers;

/// <summary>
///     Test executor that wraps test execution with VirtualTimeScheduler.
///     Provides deterministic time control for testing time-dependent behavior.
/// </summary>
public class WithVirtualTimeSchedulerExecutor : ITestExecutor
{
    /// <inheritdoc />
    public async ValueTask ExecuteTest(TestContext context, Func<ValueTask> testAction)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(testAction);

        IScheduler? originalMainThreadScheduler = null;
        IScheduler? originalTaskpoolScheduler = null;

        try
        {
            // Ensure TestContext.Current is set in case of async flow issues
            context.RestoreExecutionContext();

            var scheduler = new VirtualTimeScheduler();
            originalMainThreadScheduler = RxSchedulers.MainThreadScheduler;
            originalTaskpoolScheduler = RxSchedulers.TaskpoolScheduler;

            // Store both in StateBag for retrieval by tests
            context.StateBag.Items["VirtualTimeScheduler"] = scheduler;
            context.StateBag.Items["Scheduler"] = scheduler;

            RxAppBuilder.CreateReactiveUIBuilder()
                .WithMainThreadScheduler(scheduler)
                .WithTaskPoolScheduler(scheduler)
                .WithCoreServices()
                .BuildApp();

            RxSchedulers.MainThreadScheduler = scheduler;
            RxSchedulers.TaskpoolScheduler = scheduler;

            await testAction();
        }
        finally
        {
            if (originalMainThreadScheduler is not null)
            {
                RxSchedulers.MainThreadScheduler = originalMainThreadScheduler;
            }

            if (originalTaskpoolScheduler is not null)
            {
                RxSchedulers.TaskpoolScheduler = originalTaskpoolScheduler;
            }

            RxAppBuilder.ResetForTesting();
        }
    }
}
