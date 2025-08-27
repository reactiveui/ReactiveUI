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
    /// <param name="maxWaitMs">The maximum wait ms.</param>
    /// <exception cref="System.ArgumentNullException">testBody.</exception>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public static async Task AppBuilderTestAsync(Func<Task> testBody, int maxWaitMs = 5000)
    {
        if (testBody is null)
        {
            throw new ArgumentNullException(nameof(testBody));
        }

        await TestGate.WaitAsync().ConfigureAwait(false);
        try
        {
            // Force-reset any previous builder state to avoid waiting deadlocks.
            AppBuilder.ResetBuilderStateForTests();

            try
            {
                // Execute actual test
                await testBody().ConfigureAwait(false);
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
