// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Builder.Tests.Executors;
using TUnit.Core.Executors;

namespace ReactiveUI.Builder.Tests.Mixins;

/// <summary>Tests for the scheduler members of <see cref="IReactiveUIBuilder"/>.</summary>
[NotInParallel]
public class BuilderSchedulerMixinsTests
{
    /// <summary>Verifies that setting the task pool scheduler updates both the builder and <see cref="RxSchedulers"/>.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    [TestExecutor<ResetOnlyExecutor>]
    public async Task WithTaskPoolScheduler_Sets_Scheduler_And_Rx_Schedulers()
    {
        var original = RxSchedulers.TaskpoolScheduler;
        try
        {
            var builder = RxAppBuilder.CreateReactiveUIBuilder();
            var scheduler = Sequencer.Immediate;

            _ = builder.WithTaskPoolScheduler(scheduler);
            _ = builder.WithCoreServices().Build();

            using (Assert.Multiple())
            {
                await Assert.That(builder.TaskpoolScheduler).IsSameReferenceAs(scheduler);
                await Assert.That(RxSchedulers.TaskpoolScheduler).IsSameReferenceAs(scheduler);
            }
        }
        finally
        {
            RxSchedulers.TaskpoolScheduler = original;
        }
    }

    /// <summary>Verifies that setting the main thread scheduler updates both the builder and <see cref="RxSchedulers"/>.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    [TestExecutor<ResetOnlyExecutor>]
    public async Task WithMainThreadScheduler_Sets_Scheduler_And_Rx_Schedulers()
    {
        var original = RxSchedulers.MainThreadScheduler;
        try
        {
            var builder = RxAppBuilder.CreateReactiveUIBuilder();
            var scheduler = Sequencer.Immediate;

            _ = builder.WithMainThreadScheduler(scheduler);
            _ = builder.WithCoreServices().Build();

            using (Assert.Multiple())
            {
                await Assert.That(builder.MainThreadScheduler).IsSameReferenceAs(scheduler);
                await Assert.That(RxSchedulers.MainThreadScheduler).IsSameReferenceAs(scheduler);
            }
        }
        finally
        {
            RxSchedulers.MainThreadScheduler = original;
        }
    }

    /// <summary>Verifies that setting the task pool scheduler returns the same builder for chaining.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    [TestExecutor<ResetOnlyExecutor>]
    public async Task WithTaskPoolScheduler_Returns_Builder()
    {
        var builder = RxAppBuilder.CreateReactiveUIBuilder();

        var result = builder.WithTaskPoolScheduler(Sequencer.Immediate);

        await Assert.That(result).IsSameReferenceAs(builder);
    }

    /// <summary>Verifies that setting the main thread scheduler returns the same builder for chaining.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    [TestExecutor<ResetOnlyExecutor>]
    public async Task WithMainThreadScheduler_Returns_Builder()
    {
        var builder = RxAppBuilder.CreateReactiveUIBuilder();

        var result = builder.WithMainThreadScheduler(Sequencer.Immediate);

        await Assert.That(result).IsSameReferenceAs(builder);
    }

    /// <summary>Executor that only resets state, leaving builder configuration to each test.</summary>
    internal sealed class ResetOnlyExecutor : BuilderTestExecutorBase
    {
        /// <inheritdoc/>
        protected override void ConfigureBuilder()
        {
            // Tests in this class configure and build the builder themselves.
        }
    }
}
