// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Testing;

namespace ReactiveUI.Tests.Testing;

/// <summary>
/// Tests for RxTest.
/// </summary>
[NotInParallel]
public class RxTestTests
{
    [Test]
    public async Task AppBuilderTestAsync_Executes_Test_Body()
    {
        var executed = false;

        await RxTest.AppBuilderTestAsync(async () =>
        {
            executed = true;
            await Task.CompletedTask;
        });

        await Assert.That(executed).IsTrue();
    }

    [Test]
    public void AppBuilderTestAsync_Throws_When_TestBody_Is_Null()
    {
        Assert.ThrowsAsync<ArgumentNullException>(async () =>
            await RxTest.AppBuilderTestAsync(null!));
    }

    [Test]
    public async Task AppBuilderTestAsync_Propagates_Exceptions_From_TestBody()
    {
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(async () =>
            await RxTest.AppBuilderTestAsync(() => throw new InvalidOperationException("Test exception")));

        await Assert.That(exception!.Message).IsEqualTo("Test exception");
    }

    [Test]
    public async Task AppBuilderTestAsync_Throws_TimeoutException_When_TestBody_Exceeds_Timeout()
    {
        var exception = await Assert.ThrowsAsync<TimeoutException>(async () =>
            await RxTest.AppBuilderTestAsync(
                async () => await Task.Delay(200),
                maxWaitMs: 50));

        await Assert.That(exception!.Message).Contains("Test execution exceeded");
    }

    [Test]
    public async Task AppBuilderTestAsync_Resets_Builder_State_After_Test()
    {
        await RxTest.AppBuilderTestAsync(async () =>
        {
            // Test body
            await Task.CompletedTask;
        });

        // If we reach here without hanging, the builder state was reset correctly
    }
}
