// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive;
using ReactiveUI.Builder.Tests.Executors;
using TUnit.Core.Executors;

namespace ReactiveUI.Builder.Tests;

/// <summary>
/// Tests for RxApp migration functionality including WithExceptionHandler, WithSuspensionHost, and WithCacheSizes.
/// </summary>
[NotInParallel]
[TestExecutor<RxAppMigrationTestExecutor>]
public class ReactiveUIBuilderRxAppMigrationTests
{
    [Test]
    [TestExecutor<WithExceptionHandlerExecutor>]
    public async Task WithExceptionHandler_Should_Set_Custom_Exception_Handler()
    {
        var testException = new InvalidOperationException("Test exception");
        RxState.DefaultExceptionHandler.OnNext(testException);

        await Assert.That(WithExceptionHandlerExecutor.CapturedEx).IsEqualTo(testException);
    }

    [Test]
    public void WithExceptionHandler_With_Null_Handler_Should_Throw()
    {
        var builder = RxAppBuilder.CreateReactiveUIBuilder();

        Assert.Throws<ArgumentNullException>(() => builder.WithExceptionHandler(null!));
    }

    [Test]
    [TestExecutor<WithSuspensionHostNonGenericExecutor>]
    public async Task WithSuspensionHost_NonGeneric_Should_Create_Default_Host()
    {
        var host = RxSuspension.SuspensionHost;
        await Assert.That(host).IsNotNull();
        await Assert.That(host).IsTypeOf<SuspensionHost>();
    }

    [Test]
    [TestExecutor<WithSuspensionHostGenericExecutor>]
    public async Task WithSuspensionHost_Generic_Should_Create_Typed_Host()
    {
        var host = RxSuspension.SuspensionHost;
        await Assert.That(host).IsNotNull();
        await Assert.That(host).IsTypeOf<SuspensionHost<TestAppState>>();
    }

    [Test]
    [TestExecutor<WithCacheSizesExecutor>]
    public async Task WithCacheSizes_Should_Set_Custom_Cache_Sizes()
    {
        await Assert.That(RxCacheSize.SmallCacheLimit).IsEqualTo(128);
        await Assert.That(RxCacheSize.BigCacheLimit).IsEqualTo(512);
    }

    [Test]
    public void WithCacheSizes_With_Zero_Or_Negative_Values_Should_Throw()
    {
        var builder = RxAppBuilder.CreateReactiveUIBuilder();

        Assert.Throws<ArgumentOutOfRangeException>(() =>
            builder.WithCacheSizes(smallCacheLimit: 0, bigCacheLimit: 100));

        Assert.Throws<ArgumentOutOfRangeException>(() =>
            builder.WithCacheSizes(smallCacheLimit: 100, bigCacheLimit: 0));

        Assert.Throws<ArgumentOutOfRangeException>(() =>
            builder.WithCacheSizes(smallCacheLimit: -1, bigCacheLimit: 100));

        Assert.Throws<ArgumentOutOfRangeException>(() =>
            builder.WithCacheSizes(smallCacheLimit: 100, bigCacheLimit: -1));
    }

    [Test]
    public async Task RxCacheSize_Should_Use_Platform_Defaults_When_Not_Configured()
    {
#if ANDROID || IOS
        await Assert.That(RxCacheSize.SmallCacheLimit).IsEqualTo(32);
        await Assert.That(RxCacheSize.BigCacheLimit).IsEqualTo(64);
#else
        await Assert.That(RxCacheSize.SmallCacheLimit).IsEqualTo(64);
        await Assert.That(RxCacheSize.BigCacheLimit).IsEqualTo(256);
#endif
    }

    [Test]
    [TestExecutor<WithAllMigrationMethodsExecutor>]
    public async Task Builder_Should_Support_Chaining_All_RxApp_Migration_Methods()
    {
        // Verify exception handler
        var testException = new InvalidOperationException("Test");
        RxState.DefaultExceptionHandler.OnNext(testException);
        await Assert.That(WithAllMigrationMethodsExecutor.CapturedEx).IsEqualTo(testException);

        // Verify suspension host
        var host = RxSuspension.SuspensionHost;
        await Assert.That(host).IsTypeOf<SuspensionHost<TestAppState>>();

        // Verify cache sizes
        await Assert.That(RxCacheSize.SmallCacheLimit).IsEqualTo(100);
        await Assert.That(RxCacheSize.BigCacheLimit).IsEqualTo(400);
    }

    [Test]
    public async Task RxSchedulers_DefaultExceptionHandler_Should_Not_Be_Null()
    {
        var handler = RxState.DefaultExceptionHandler;
        await Assert.That(handler).IsNotNull();
    }

    [Test]
    public async Task RxSchedulers_SuspensionHost_Should_Not_Be_Null()
    {
        var host = RxSuspension.SuspensionHost;
        await Assert.That(host).IsNotNull();
    }

    [Test]
    [TestExecutor<WithMultipleExceptionHandlersExecutor>]
    public async Task WithExceptionHandler_Called_Multiple_Times_Should_Use_Last_Handler()
    {
        var testException = new InvalidOperationException("Test");
        RxState.DefaultExceptionHandler.OnNext(testException);

        await Assert.That(WithMultipleExceptionHandlersExecutor.SecondCaptured).IsEqualTo(testException);
        await Assert.That(WithMultipleExceptionHandlersExecutor.FirstCaptured).IsNull();
    }

    [Test]
    [TestExecutor<WithMultipleCacheSizesExecutor>]
    public async Task WithCacheSizes_Called_Multiple_Times_Should_Use_Last_Values()
    {
        await Assert.That(RxCacheSize.SmallCacheLimit).IsEqualTo(300);
        await Assert.That(RxCacheSize.BigCacheLimit).IsEqualTo(600);
    }

    [Test]
    [TestExecutor<WithSuspensionHostOverrideExecutor>]
    public async Task WithSuspensionHost_Generic_Overrides_NonGeneric()
    {
        var host = RxSuspension.SuspensionHost;
        await Assert.That(host).IsTypeOf<SuspensionHost<TestAppState>>();
    }

    internal class RxAppMigrationTestExecutor : BuilderTestExecutorBase
    {
        protected override void ResetState()
        {
            base.ResetState();
            RxCacheSize.ResetForTesting();
            RxState.ResetForTesting();
            RxSuspension.ResetForTesting();
        }
    }

    internal sealed class WithExceptionHandlerExecutor : RxAppMigrationTestExecutor
    {
        public static Exception? CapturedEx { get; private set; }

        protected override void ConfigureBuilder()
        {
            CapturedEx = null;
            var customHandler = Observer.Create<Exception>(ex => CapturedEx = ex);

            RxAppBuilder.CreateReactiveUIBuilder()
                .WithExceptionHandler(customHandler)
                .WithCoreServices()
                .BuildApp();
        }
    }

    internal sealed class WithSuspensionHostNonGenericExecutor : RxAppMigrationTestExecutor
    {
        protected override void ConfigureBuilder() =>
            RxAppBuilder.CreateReactiveUIBuilder()
                .WithSuspensionHost()
                .WithCoreServices()
                .BuildApp();
    }

    internal sealed class WithSuspensionHostGenericExecutor : RxAppMigrationTestExecutor
    {
        protected override void ConfigureBuilder() =>
            RxAppBuilder.CreateReactiveUIBuilder()
                .WithSuspensionHost<TestAppState>()
                .WithCoreServices()
                .BuildApp();
    }

    internal sealed class WithCacheSizesExecutor : RxAppMigrationTestExecutor
    {
        protected override void ConfigureBuilder() =>
            RxAppBuilder.CreateReactiveUIBuilder()
                .WithCacheSizes(smallCacheLimit: 128, bigCacheLimit: 512)
                .WithCoreServices()
                .BuildApp();
    }

    internal sealed class WithAllMigrationMethodsExecutor : RxAppMigrationTestExecutor
    {
        public static Exception? CapturedEx { get; private set; }

        protected override void ConfigureBuilder()
        {
            CapturedEx = null;
            var customHandler = Observer.Create<Exception>(ex => CapturedEx = ex);

            RxAppBuilder.CreateReactiveUIBuilder()
                .WithExceptionHandler(customHandler)
                .WithSuspensionHost<TestAppState>()
                .WithCacheSizes(smallCacheLimit: 100, bigCacheLimit: 400)
                .WithCoreServices()
                .BuildApp();
        }
    }

    internal sealed class WithMultipleExceptionHandlersExecutor : RxAppMigrationTestExecutor
    {
        public static Exception? FirstCaptured { get; private set; }

        public static Exception? SecondCaptured { get; private set; }

        protected override void ConfigureBuilder()
        {
            FirstCaptured = null;
            SecondCaptured = null;

            var firstHandler = Observer.Create<Exception>(ex => FirstCaptured = ex);
            var secondHandler = Observer.Create<Exception>(ex => SecondCaptured = ex);

            RxAppBuilder.CreateReactiveUIBuilder()
                .WithExceptionHandler(firstHandler)
                .WithExceptionHandler(secondHandler)
                .WithCoreServices()
                .BuildApp();
        }
    }

    internal sealed class WithMultipleCacheSizesExecutor : RxAppMigrationTestExecutor
    {
        protected override void ConfigureBuilder() =>
            RxAppBuilder.CreateReactiveUIBuilder()
                .WithCacheSizes(smallCacheLimit: 100, bigCacheLimit: 200)
                .WithCacheSizes(smallCacheLimit: 300, bigCacheLimit: 600)
                .WithCoreServices()
                .BuildApp();
    }

    internal sealed class WithSuspensionHostOverrideExecutor : RxAppMigrationTestExecutor
    {
        protected override void ConfigureBuilder() =>
            RxAppBuilder.CreateReactiveUIBuilder()
                .WithSuspensionHost()
                .WithSuspensionHost<TestAppState>()
                .WithCoreServices()
                .BuildApp();
    }

    private class TestAppState
    {
        public string? Name { get; set; }

        public int Counter { get; set; }
    }
}
