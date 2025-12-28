// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Splat.Builder;

namespace ReactiveUI.Testing;

/// <summary>
/// RxTest.
/// </summary>
public static class RxTest
{
    private static readonly SemaphoreSlim TestGate = new(1, 1);

    /// <summary>
    /// Applications the builder test asynchronous.
    /// </summary>
    /// <param name="testBody">The test body.</param>
    /// <param name="maxWaitMs">The maximum wait in milliseconds for both acquiring the test gate and running the test body.</param>
    /// <exception cref="System.ArgumentNullException">testBody.</exception>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public static async Task AppBuilderTestAsync(Func<Task> testBody, int maxWaitMs = 60000)
    {
        ArgumentExceptionHelper.ThrowIfNull(testBody);

        // Try to acquire the global test gate with timeout to avoid deadlocks in CI.
        if (!await TestGate.WaitAsync(maxWaitMs).ConfigureAwait(false))
        {
            throw new TimeoutException($"Timed out waiting for AppBuilder test gate after {maxWaitMs}ms.");
        }

        try
        {
            // Force-reset any previous builder state to avoid waiting deadlocks.
            AppBuilder.ResetBuilderStateForTests();

            try
            {
                // Execute actual test with timeout so it doesn't hang forever on CI.
                var testTask = testBody();
                var timeoutTask = Task.Delay(maxWaitMs);
                var completed = await Task.WhenAny(testTask, timeoutTask).ConfigureAwait(false);
                if (completed == timeoutTask)
                {
                    throw new TimeoutException($"Test execution exceeded {maxWaitMs}ms.");
                }

                // Propagate exceptions from the test body.
                await testTask.ConfigureAwait(false);
            }
            finally
            {
                // Final reset after test
                AppBuilder.ResetBuilderStateForTests();
            }
        }
        finally
        {
            TestGate.Release();
        }
    }
}
