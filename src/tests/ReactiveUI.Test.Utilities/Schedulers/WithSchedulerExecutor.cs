// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Utilities.Schedulers;

/// <summary>
///     Test executor that wraps test execution with ImmediateScheduler override.
///     Sets RxSchedulers.MainThreadScheduler and TaskpoolScheduler for test duration.
/// </summary>
public class WithSchedulerExecutor : ITestExecutor
{
    /// <inheritdoc />
    public async ValueTask ExecuteTest(TestContext context, Func<ValueTask> testAction)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(testAction);

        var scheduler = ImmediateScheduler.Instance;
        IScheduler? originalMainThreadScheduler = null;
        IScheduler? originalTaskpoolScheduler = null;

        try
        {
            // Ensure TestContext.Current is set in case of async flow issues
            context.RestoreExecutionContext();

            originalMainThreadScheduler = RxSchedulers.MainThreadScheduler;
            originalTaskpoolScheduler = RxSchedulers.TaskpoolScheduler;

            // Store scheduler in StateBag for retrieval by tests
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
