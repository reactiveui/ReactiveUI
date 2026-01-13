// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

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

        var testBus = new ReactiveUI.MessageBus();
        context.StateBag.Items["MessageBus"] = testBus;

        // Force-reset any previous builder state to avoid waiting deadlocks.
        Splat.Builder.AppBuilder.ResetBuilderStateForTests();
        RxAppBuilder.ResetForTesting();

        // Re-initialize ReactiveUI with core services after reset
        // This ensures tests have a clean, properly initialized environment
        _ = RxAppBuilder.CreateReactiveUIBuilder()
            .WithCoreServices()
            .BuildApp();

        try
        {
            // Execute actual test with timeout so it doesn't hang forever on CI.
            await testAction();
        }
        finally
        {
            // Final reset after test
            Splat.Builder.AppBuilder.ResetBuilderStateForTests();
            RxAppBuilder.ResetForTesting();
        }

        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(testAction);

        var prevBus = ReactiveUI.MessageBus.Current;
        try
        {
            ReactiveUI.MessageBus.Current = testBus;
            await testAction();
        }
        finally
        {
            ReactiveUI.MessageBus.Current = prevBus;
        }
    }
}
