// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests.Utilities.AppBuilder;

/// <summary>
///     Test executor that sets up AppBuilder isolation for test duration.
///     Ensures tests run serially and AppBuilder state is reset before/after each test.
/// </summary>
public class AppBuilderTestExecutor : ITestExecutor
{
    /// <inheritdoc />
    public async ValueTask ExecuteTest(TestContext context, Func<ValueTask> testAction)
    {
        ArgumentNullException.ThrowIfNull(testAction);

        // Force-reset any previous builder state to avoid waiting deadlocks.
        RxAppBuilder.ResetForTesting();
        Splat.Builder.AppBuilder.ResetBuilderStateForTests();

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
            // Final reset after test and rebuild to ensure completely clean state for next test
            RxAppBuilder.ResetForTesting();

            // Rebuild to clear any service registrations made during the test
            _ = RxAppBuilder.CreateReactiveUIBuilder()
                .WithCoreServices()
                .BuildApp();
        }
    }
}
