// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Testing;

namespace ReactiveUI.Tests.Testing;

/// <summary>
/// Tests for AppBuilderTestBase.
/// </summary>
[NotInParallel]
public class AppBuilderTestBaseTests
{
    /// <summary>
    /// Tests that RunAppBuilderTestAsync with Func&lt;Task&gt; executes the test body.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task RunAppBuilderTestAsync_WithTaskFunc_ExecutesTestBody()
    {
        var executed = false;

        await TestAppBuilderTest.TestWithTaskFunc(async () =>
        {
            executed = true;
            await Task.CompletedTask;
        });

        await Assert.That(executed).IsTrue();
    }

    /// <summary>
    /// Tests that RunAppBuilderTestAsync with Action executes the test body.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task RunAppBuilderTestAsync_WithAction_ExecutesTestBody()
    {
        var executed = false;

        await TestAppBuilderTest.TestWithAction(() => executed = true);

        await Assert.That(executed).IsTrue();
    }

    /// <summary>
    /// Tests that RunAppBuilderTestAsync with Action converts to Task correctly.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task RunAppBuilderTestAsync_WithAction_ConvertsToTask()
    {
        var counter = 0;

        var task = TestAppBuilderTest.TestWithAction(() => counter++);

        await task;

        await Assert.That(counter).IsEqualTo(1);
    }

    /// <summary>
    /// Test implementation that inherits from AppBuilderTestBase.
    /// </summary>
    private sealed class TestAppBuilderTest : AppBuilderTestBase
    {
        /// <summary>
        /// Test method that calls RunAppBuilderTestAsync with a Task-returning function.
        /// </summary>
        /// <param name="testBody">The test body to execute.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public static Task TestWithTaskFunc(Func<Task> testBody) =>
            RunAppBuilderTestAsync(testBody);

        /// <summary>
        /// Test method that calls RunAppBuilderTestAsync with an Action.
        /// </summary>
        /// <param name="testBody">The test body to execute.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public static Task TestWithAction(Action testBody) =>
            RunAppBuilderTestAsync(testBody);
    }
}
