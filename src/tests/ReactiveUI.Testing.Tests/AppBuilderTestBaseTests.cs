// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Tests.Utilities.AppBuilder;

using TUnit.Core.Executors;

namespace ReactiveUI.Testing.Tests;

/// <summary>
///     Tests for <see cref="AppBuilderTestBase"/> which provides a base class for testing
///     ReactiveUI application builder scenarios.
/// </summary>
[NotInParallel]
[TestExecutor<AppBuilderTestExecutor>]
public class AppBuilderTestBaseTests
{
    /// <summary>
    /// Verifies that <see cref="AppBuilderTestBase.RunAppBuilderTestAsync(Func{Task})"/>
    /// executes an asynchronous test body successfully.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task RunAppBuilderTestAsync_WithAsyncTestBody_ExecutesTest()
    {
        // Arrange
        var executed = false;

        // Act
        await TestHelper.RunAppBuilderTestAsync(async () =>
        {
            executed = true;
            await Task.CompletedTask;
        });

        // Assert
        await Assert.That(executed).IsTrue();
    }

    /// <summary>
    /// Verifies that <see cref="AppBuilderTestBase.RunAppBuilderTestAsync(Action)"/>
    /// executes a synchronous test body successfully.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task RunAppBuilderTestAsync_WithSyncTestBody_ExecutesTest()
    {
        // Arrange
        var executed = false;

        // Act
        await TestHelper.RunAppBuilderTestAsync(() =>
        {
            executed = true;
        });

        // Assert
        await Assert.That(executed).IsTrue();
    }

    /// <summary>
    /// Verifies that <see cref="AppBuilderTestBase.RunAppBuilderTestAsync(Func{Task})"/>
    /// propagates exceptions thrown by the asynchronous test body.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task RunAppBuilderTestAsync_WithAsyncTestBody_PropagatesExceptions()
    {
        // Act & Assert
        await Assert.That(async () =>
        {
            await TestHelper.RunAppBuilderTestAsync(async () =>
            {
                await Task.Yield();
                throw new InvalidOperationException("Test exception");
            });
        }).Throws<InvalidOperationException>();
    }

    /// <summary>
    /// Verifies that <see cref="AppBuilderTestBase.RunAppBuilderTestAsync(Action)"/>
    /// propagates exceptions thrown by the synchronous test body.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task RunAppBuilderTestAsync_WithSyncTestBody_PropagatesExceptions()
    {
        // Act & Assert
        await Assert.That(async () =>
        {
            await TestHelper.RunAppBuilderTestAsync(() =>
            {
                throw new InvalidOperationException("Test exception");
            });
        }).Throws<InvalidOperationException>();
    }

    /// <summary>
    /// Verifies that <see cref="AppBuilderTestBase.RunAppBuilderTestAsync(Action)"/>
    /// can be called multiple times sequentially without interference.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous unit test.</returns>
    [Test]
    public async Task RunAppBuilderTestAsync_AllowsMultipleSequentialCalls()
    {
        // Arrange
        var executionCount = 0;

        // Act
        await TestHelper.RunAppBuilderTestAsync(() => executionCount++);
        await TestHelper.RunAppBuilderTestAsync(() => executionCount++);
        await TestHelper.RunAppBuilderTestAsync(() => executionCount++);

        // Assert
        await Assert.That(executionCount).IsEqualTo(3);
    }

    /// <summary>
    /// Test helper that inherits from AppBuilderTestBase.
    /// </summary>
    private sealed class TestHelper : AppBuilderTestBase
    {
        public static new Task RunAppBuilderTestAsync(Func<Task> testBody) =>
            AppBuilderTestBase.RunAppBuilderTestAsync(testBody);

        public static new Task RunAppBuilderTestAsync(Action testBody) =>
            AppBuilderTestBase.RunAppBuilderTestAsync(testBody);
    }
}
