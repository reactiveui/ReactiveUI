// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive.Concurrency;

using Splat.Builder;

namespace ReactiveUI.Builder.Tests;

[NotInParallel]
public class BuilderSchedulerMixinsTests
{
    [Before(HookType.Test)]
    public void SetUp() => AppBuilder.ResetBuilderStateForTests();

    [Test]
    public void WithTaskPoolScheduler_Throws_When_Builder_Null()
    {
        var scheduler = ImmediateScheduler.Instance;
        Assert.Throws<ArgumentNullException>(() => BuilderMixins.WithTaskPoolScheduler(null!, scheduler));
    }

    [Test]
    public void WithMainThreadScheduler_Throws_When_Builder_Null()
    {
        var scheduler = ImmediateScheduler.Instance;
        Assert.Throws<ArgumentNullException>(() => BuilderMixins.WithMainThreadScheduler(null!, scheduler));
    }

    [Test]
    public async Task WithTaskPoolScheduler_Sets_Scheduler_And_Rx_Schedulers()
    {
        var original = RxSchedulers.TaskpoolScheduler;
        try
        {
            using var resolver = new ModernDependencyResolver();
            var builder = resolver.CreateReactiveUIBuilder();
            var scheduler = ImmediateScheduler.Instance;

            builder.WithTaskPoolScheduler(scheduler);
            builder.WithCoreServices().Build();

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

    [Test]
    public async Task WithMainThreadScheduler_Sets_Scheduler_And_Rx_Schedulers()
    {
        var original = RxSchedulers.MainThreadScheduler;
        try
        {
            using var resolver = new ModernDependencyResolver();
            var builder = resolver.CreateReactiveUIBuilder();
            var scheduler = ImmediateScheduler.Instance;

            builder.WithMainThreadScheduler(scheduler);
            builder.WithCoreServices().Build();

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

    [Test]
    public async Task WithTaskPoolScheduler_Extension_Method_Returns_Builder()
    {
        using var resolver = new ModernDependencyResolver();
        var builder = resolver.CreateReactiveUIBuilder();
        var scheduler = ImmediateScheduler.Instance;

        var result = BuilderMixins.WithTaskPoolScheduler(builder, scheduler);

        await Assert.That(result).IsSameReferenceAs(builder);
    }

    [Test]
    public async Task WithMainThreadScheduler_Extension_Method_Returns_Builder()
    {
        using var resolver = new ModernDependencyResolver();
        var builder = resolver.CreateReactiveUIBuilder();
        var scheduler = ImmediateScheduler.Instance;

        var result = BuilderMixins.WithMainThreadScheduler(builder, scheduler);

        await Assert.That(result).IsSameReferenceAs(builder);
    }
}
