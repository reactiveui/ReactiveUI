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

        context.StateBag.Items["Scheduler"] = scheduler;
        context.StateBag.Items["MessageBus"] = messageBus;

        var prevMain = RxSchedulers.MainThreadScheduler;
        var prevTask = RxSchedulers.TaskpoolScheduler;
        var prevBus = ReactiveUI.MessageBus.Current;

        try
        {
            RxSchedulers.MainThreadScheduler = scheduler;
            RxSchedulers.TaskpoolScheduler = scheduler;
            ReactiveUI.MessageBus.Current = messageBus;
            await testAction();
        }
        finally
        {
            RxSchedulers.MainThreadScheduler = prevMain;
            RxSchedulers.TaskpoolScheduler = prevTask;
            ReactiveUI.MessageBus.Current = prevBus;
        }
    }
}
