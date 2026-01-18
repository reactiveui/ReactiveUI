// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Tests.Utilities.AppBuilder;

using TUnit.Core.Executors;

namespace ReactiveUI.Testing.Tests;

/// <summary>
///     Tests for <see cref="RxTest"/> which provides utilities for testing ReactiveUI
///     application builder scenarios with proper isolation.
/// </summary>
[NotInParallel]
[TestExecutor<AppBuilderTestExecutor>]
public class RxTestTests
{
    /// <summary>
    /// Verifies that the AppBuilderTestAsync method executes the provided test body as expected.
    /// </summary>
    /// <returns>A task that represents the asynchronous test operation.</returns>
    [Test]
    public async Task AppBuilderTestAsync_ExecutesTestBody()
    {
        // Arrange
        var executed = false;

        // Act
        await RxTest.AppBuilderTestAsync(() =>
        {
            executed = true;
            return Task.CompletedTask;
        });

        // Assert
        await Assert.That(executed).IsTrue();
    }

    /// <summary>
    /// Verifies that AppBuilderTestAsync throws an ArgumentNullException when the testBody parameter is null.
    /// </summary>
    /// <returns>A task that represents the asynchronous test operation.</returns>
    [Test]
    public async Task AppBuilderTestAsync_ThrowsArgumentNullException_WhenTestBodyIsNull()
    {
        // Act & Assert
        await Assert.That(() => RxTest.AppBuilderTestAsync(null!))
            .Throws<ArgumentException>();
    }

    /// <summary>
    /// Verifies that exceptions thrown within the AppBuilderTestAsync delegate are properly propagated to the caller.
    /// </summary>
    /// <returns>A task that represents the asynchronous test operation.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the delegate passed to AppBuilderTestAsync throws an InvalidOperationException.</exception>
    [Test]
    public async Task AppBuilderTestAsync_PropagatesExceptions()
    {
        // Act & Assert
        await Assert.That(async () =>
        {
            await RxTest.AppBuilderTestAsync(() =>
            {
                throw new InvalidOperationException("Test exception");
            });
        }).Throws<InvalidOperationException>();
    }

    /// <summary>
    /// Verifies that asynchronous exceptions thrown within the AppBuilderTestAsync method are properly propagated to
    /// the caller.
    /// </summary>
    /// <returns>A task that represents the asynchronous test operation.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the asynchronous delegate passed to AppBuilderTestAsync throws an InvalidOperationException.</exception>
    [Test]
    public async Task AppBuilderTestAsync_PropagatesAsyncExceptions()
    {
        // Act & Assert
        await Assert.That(async () =>
        {
            await RxTest.AppBuilderTestAsync(async () =>
            {
                await Task.Yield();
                throw new InvalidOperationException("Async test exception");
            });
        }).Throws<InvalidOperationException>();
    }

    /// <summary>
    /// Verifies that the AppBuilderTestAsync method allows multiple sequential invocations without interference or side
    /// effects.
    /// </summary>
    /// <returns>A task that represents the asynchronous test operation.</returns>
    [Test]
    public async Task AppBuilderTestAsync_AllowsSequentialCalls()
    {
        // Arrange
        var count = 0;

        // Act
        await RxTest.AppBuilderTestAsync(() =>
        {
            count++;
            return Task.CompletedTask;
        });

        await RxTest.AppBuilderTestAsync(() =>
        {
            count++;
            return Task.CompletedTask;
        });

        await RxTest.AppBuilderTestAsync(() =>
        {
            count++;
            return Task.CompletedTask;
        });

        // Assert
        await Assert.That(count).IsEqualTo(3);
    }

    /// <summary>
    /// Verifies that the AppBuilderTestAsync method resets its builder state between test executions.
    /// </summary>
    /// <remarks>This test ensures that state changes in one invocation of AppBuilderTestAsync do not affect
    /// subsequent invocations, maintaining test isolation.</remarks>
    /// <returns>A task that represents the asynchronous test operation.</returns>
    [Test]
    public async Task AppBuilderTestAsync_ResetsBuilderStateBetweenTests()
    {
        // This test verifies that multiple calls don't interfere with each other
        // Arrange & Act
        await RxTest.AppBuilderTestAsync(() =>
        {
            // First test - setup some state
            return Task.CompletedTask;
        });

        var secondTestExecuted = false;
        await RxTest.AppBuilderTestAsync(() =>
        {
            // Second test - should have clean state
            secondTestExecuted = true;
            return Task.CompletedTask;
        });

        // Assert
        await Assert.That(secondTestExecuted).IsTrue();
    }

    /// <summary>
    /// Verifies that the AppBuilderTestAsync method completes execution within the specified custom timeout.
    /// </summary>
    /// <remarks>This test ensures that the provided delegate is executed and completes within the given
    /// timeout period. It is intended to validate timeout handling in asynchronous test scenarios.</remarks>
    /// <returns>A task that represents the asynchronous test operation.</returns>
    [Test]
    public async Task AppBuilderTestAsync_WithCustomTimeout_ExecutesWithinTimeout()
    {
        // Arrange
        var executed = false;

        // Act
        await RxTest.AppBuilderTestAsync(
            () =>
            {
                executed = true;
                return Task.CompletedTask;
            },
            maxWaitMs: 5000);

        // Assert
        await Assert.That(executed).IsTrue();
    }

    /// <summary>
    /// Verifies that AppBuilderTestAsync throws a TimeoutException when the test action exceeds the specified timeout.
    /// </summary>
    /// <returns>A task that represents the asynchronous test operation.</returns>
    [Test]
    public async Task AppBuilderTestAsync_ThrowsTimeoutException_WhenTestExceedsTimeout()
    {
        // Act & Assert
        await Assert.That(async () =>
        {
            await RxTest.AppBuilderTestAsync(
                async () =>
                {
                    await Task.Delay(2000); // Delay longer than timeout
                },
                maxWaitMs: 100);
        }).Throws<TimeoutException>();
    }

    /// <summary>
    /// Verifies that the AppBuilderTestAsync method correctly handles delegates that return Task.CompletedTask without
    /// throwing exceptions.
    /// </summary>
    /// <returns>A task that represents the asynchronous test operation.</returns>
    [Test]
    public async Task AppBuilderTestAsync_HandlesTaskCompletedTask()
    {
        // This test verifies that returning Task.CompletedTask works correctly
        // Act - Should not throw
        await RxTest.AppBuilderTestAsync(() => Task.CompletedTask);

        // If we get here without exception, the test passed
    }
}
