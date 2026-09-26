// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using TUnit.Core.Interfaces;

namespace ReactiveUI.WinForms.Tests.Winforms;

/// <summary>Tests for <see cref="WinFormsThreadExecutor"/>.</summary>
public sealed class WinFormsThreadExecutorTests
{
    /// <summary>Verifies a synchronous test only reports finished once the executor has torn it down.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task SynchronousTestFinishesAfterTearDown()
    {
        var executor = new RecordingExecutor();

        await ((ITestExecutor)executor).ExecuteTest(TestContext.Current!, static () => default);

        await Assert.That(executor.TornDown).IsTrue();
    }

    /// <summary>Verifies an asynchronous test only reports finished once the executor has torn it down.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task AsynchronousTestFinishesAfterTearDown()
    {
        var executor = new RecordingExecutor();

        await ((ITestExecutor)executor).ExecuteTest(TestContext.Current!, static async () => await Task.Yield());

        await Assert.That(executor.TornDown).IsTrue();
    }

    /// <summary>An executor with a slow teardown that records when the teardown has finished.</summary>
    /// <remarks>
    /// The delay stands in for a teardown that resets the app builder and the service locator. It gives a test that
    /// reports finished too early the time to be seen doing so; it cannot make a correct executor fail.
    /// </remarks>
    private sealed class RecordingExecutor : WinFormsThreadExecutor
    {
        /// <summary>How long the teardown takes.</summary>
        private static readonly TimeSpan TearDownDuration = TimeSpan.FromMilliseconds(200);

        /// <summary>One once the teardown has finished.</summary>
        private int _tornDown;

        /// <summary>Gets a value indicating whether the teardown has finished.</summary>
        public bool TornDown => Volatile.Read(ref _tornDown) == 1;

        /// <inheritdoc/>
        protected override void TearDown()
        {
            Thread.Sleep(TearDownDuration);
            Volatile.Write(ref _tornDown, 1);
            base.TearDown();
        }
    }
}
