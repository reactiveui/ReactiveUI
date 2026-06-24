// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Builder.Tests.Executors;
using TUnit.Core.Executors;

namespace ReactiveUI.Builder.Tests;

/// <summary>Tests for RxApp migration functionality including WithExceptionHandler, WithSuspensionHost, and WithCacheSizes.</summary>
[NotInParallel]
[TestExecutor<RxAppMigrationTestExecutor>]
public class ReactiveUIBuilderRxAppMigrationTests
{
    /// <summary>The custom small-cache size used by the cache-size tests.</summary>
    private const int CustomSmallCacheSize = 128;

    /// <summary>The custom big-cache size used by the cache-size tests.</summary>
    private const int CustomBigCacheSize = 512;

    /// <summary>The small-cache size used by the chained cache-size tests.</summary>
    private const int ChainedSmallCacheSize = 100;

    /// <summary>The big-cache size used by the chained cache-size tests.</summary>
    private const int ChainedBigCacheSize = 400;

    /// <summary>The small-cache size applied by the first call in the override-order tests.</summary>
    private const int FirstSmallCacheSize = 100;

    /// <summary>The big-cache size applied by the first call in the override-order tests.</summary>
    private const int FirstBigCacheSize = 200;

    /// <summary>The small-cache size applied by the last call in the override-order tests.</summary>
    private const int LastSmallCacheSize = 300;

    /// <summary>The big-cache size applied by the last call in the override-order tests.</summary>
    private const int LastBigCacheSize = 600;

    /// <summary>An invalid (zero) cache size used to verify validation.</summary>
    private const int InvalidCacheSize = 0;

    /// <summary>A negative cache size used to verify validation.</summary>
    private const int NegativeCacheSize = -1;

    /// <summary>A valid cache size used to verify validation succeeds.</summary>
    private const int ValidCacheSize = 100;
#if ANDROID || IOS
    /// <summary>The default small-cache size expected on mobile platforms.</summary>
    private const int MobileSmallCacheDefault = 32;

    /// <summary>The default big-cache size expected on mobile platforms.</summary>
    private const int MobileBigCacheDefault = 64;
#else
    /// <summary>The default small-cache size expected on desktop platforms.</summary>
    private const int DesktopSmallCacheDefault = 64;

    /// <summary>The default big-cache size expected on desktop platforms.</summary>
    private const int DesktopBigCacheDefault = 256;
#endif

    /// <summary>Verifies that a custom exception handler receives exceptions from the default handler.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    [TestExecutor<WithExceptionHandlerExecutor>]
    public async Task WithExceptionHandler_Should_Set_Custom_Exception_Handler()
    {
        var testException = new InvalidOperationException("Test exception");
        RxState.DefaultExceptionHandler.OnNext(testException);

        await Assert.That(WithExceptionHandlerExecutor.CapturedEx).IsEqualTo(testException);
    }

    /// <summary>Verifies that a null exception handler throws <see cref="ArgumentNullException"/>.</summary>
    [Test]
    public void WithExceptionHandler_With_Null_Handler_Should_Throw()
    {
        var builder = RxAppBuilder.CreateReactiveUIBuilder();

        _ = Assert.Throws<ArgumentNullException>(() => builder.WithExceptionHandler(null!));
    }

    /// <summary>Verifies that the non-generic suspension host creates a default <see cref="SuspensionHost"/>.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    [TestExecutor<WithSuspensionHostNonGenericExecutor>]
    public async Task WithSuspensionHost_NonGeneric_Should_Create_Default_Host()
    {
        var host = RxSuspension.SuspensionHost;
        await Assert.That(host).IsNotNull();
        await Assert.That(host).IsTypeOf<SuspensionHost>();
    }

    /// <summary>Verifies that the generic suspension host creates a typed <see cref="SuspensionHost{T}"/>.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    [TestExecutor<WithSuspensionHostGenericExecutor>]
    public async Task WithSuspensionHost_Generic_Should_Create_Typed_Host()
    {
        var host = RxSuspension.SuspensionHost;
        await Assert.That(host).IsNotNull();
        await Assert.That(host).IsTypeOf<SuspensionHost<TestAppState>>();
    }

    /// <summary>Verifies that custom cache sizes are applied to <see cref="RxCacheSize"/>.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    [TestExecutor<WithCacheSizesExecutor>]
    public async Task WithCacheSizes_Should_Set_Custom_Cache_Sizes()
    {
        await Assert.That(RxCacheSize.SmallCacheLimit).IsEqualTo(CustomSmallCacheSize);
        await Assert.That(RxCacheSize.BigCacheLimit).IsEqualTo(CustomBigCacheSize);
    }

    /// <summary>Verifies that zero or negative cache sizes throw <see cref="ArgumentOutOfRangeException"/>.</summary>
    [Test]
    public void WithCacheSizes_With_Zero_Or_Negative_Values_Should_Throw()
    {
        var builder = RxAppBuilder.CreateReactiveUIBuilder();

        _ = Assert.Throws<ArgumentOutOfRangeException>(() =>
            builder.WithCacheSizes(InvalidCacheSize, ValidCacheSize));

        _ = Assert.Throws<ArgumentOutOfRangeException>(() =>
            builder.WithCacheSizes(ValidCacheSize, InvalidCacheSize));

        _ = Assert.Throws<ArgumentOutOfRangeException>(() =>
            builder.WithCacheSizes(NegativeCacheSize, ValidCacheSize));

        _ = Assert.Throws<ArgumentOutOfRangeException>(() =>
            builder.WithCacheSizes(ValidCacheSize, NegativeCacheSize));
    }

    /// <summary>Verifies that cache sizes fall back to platform defaults when not configured.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task RxCacheSize_Should_Use_Platform_Defaults_When_Not_Configured()
    {
#if ANDROID || IOS
        await Assert.That(RxCacheSize.SmallCacheLimit).IsEqualTo(MobileSmallCacheDefault);
        await Assert.That(RxCacheSize.BigCacheLimit).IsEqualTo(MobileBigCacheDefault);
#else
        await Assert.That(RxCacheSize.SmallCacheLimit).IsEqualTo(DesktopSmallCacheDefault);
        await Assert.That(RxCacheSize.BigCacheLimit).IsEqualTo(DesktopBigCacheDefault);
#endif
    }

    /// <summary>Verifies that all RxApp migration methods can be chained together and applied.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
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
        await Assert.That(RxCacheSize.SmallCacheLimit).IsEqualTo(ChainedSmallCacheSize);
        await Assert.That(RxCacheSize.BigCacheLimit).IsEqualTo(ChainedBigCacheSize);
    }

    /// <summary>Verifies that the default exception handler is not null.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task RxSchedulers_DefaultExceptionHandler_Should_Not_Be_Null()
    {
        var handler = RxState.DefaultExceptionHandler;
        await Assert.That(handler).IsNotNull();
    }

    /// <summary>Verifies that the suspension host is not null.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    public async Task RxSchedulers_SuspensionHost_Should_Not_Be_Null()
    {
        var host = RxSuspension.SuspensionHost;
        await Assert.That(host).IsNotNull();
    }

    /// <summary>Verifies that the last exception handler wins when set multiple times.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    [TestExecutor<WithMultipleExceptionHandlersExecutor>]
    public async Task WithExceptionHandler_Called_Multiple_Times_Should_Use_Last_Handler()
    {
        var testException = new InvalidOperationException("Test");
        RxState.DefaultExceptionHandler.OnNext(testException);

        await Assert.That(WithMultipleExceptionHandlersExecutor.SecondCaptured).IsEqualTo(testException);
        await Assert.That(WithMultipleExceptionHandlersExecutor.FirstCaptured).IsNull();
    }

    /// <summary>Verifies that the last cache sizes win when set multiple times.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    [TestExecutor<WithMultipleCacheSizesExecutor>]
    public async Task WithCacheSizes_Called_Multiple_Times_Should_Use_Last_Values()
    {
        await Assert.That(RxCacheSize.SmallCacheLimit).IsEqualTo(LastSmallCacheSize);
        await Assert.That(RxCacheSize.BigCacheLimit).IsEqualTo(LastBigCacheSize);
    }

    /// <summary>Verifies that the generic suspension host overrides a previously set non-generic host.</summary>
    /// <returns>A task representing the asynchronous test.</returns>
    [Test]
    [TestExecutor<WithSuspensionHostOverrideExecutor>]
    public async Task WithSuspensionHost_Generic_Overrides_NonGeneric()
    {
        var host = RxSuspension.SuspensionHost;
        await Assert.That(host).IsTypeOf<SuspensionHost<TestAppState>>();
    }

    /// <summary>Executor that resets RxApp migration-related static state before and after each test.</summary>
    internal class RxAppMigrationTestExecutor : BuilderTestExecutorBase
    {
        /// <inheritdoc/>
        protected override void ResetState()
        {
            base.ResetState();
            RxCacheSize.ResetForTesting();
            RxState.ResetForTesting();
            RxSuspension.ResetForTesting();
        }
    }

    /// <summary>Executor that builds the app with a custom exception handler.</summary>
    internal sealed class WithExceptionHandlerExecutor : RxAppMigrationTestExecutor
    {
        /// <summary>Gets the exception captured by the custom handler.</summary>
        public static Exception? CapturedEx { get; private set; }

        /// <inheritdoc/>
        protected override void ConfigureBuilder()
        {
            CapturedEx = null;
            var customHandler = Witness.Create<Exception>(ex => CapturedEx = ex);

            _ = RxAppBuilder.CreateReactiveUIBuilder()
                .WithExceptionHandler(customHandler)
                .WithCoreServices()
                .BuildApp();
        }
    }

    /// <summary>Executor that builds the app with a non-generic suspension host.</summary>
    internal sealed class WithSuspensionHostNonGenericExecutor : RxAppMigrationTestExecutor
    {
        /// <inheritdoc/>
        protected override void ConfigureBuilder() =>
            RxAppBuilder.CreateReactiveUIBuilder()
                .WithSuspensionHost()
                .WithCoreServices()
                .BuildApp();
    }

    /// <summary>Executor that builds the app with a generic suspension host.</summary>
    internal sealed class WithSuspensionHostGenericExecutor : RxAppMigrationTestExecutor
    {
        /// <inheritdoc/>
        protected override void ConfigureBuilder() =>
            RxAppBuilder.CreateReactiveUIBuilder()
                .WithSuspensionHost<TestAppState>()
                .WithCoreServices()
                .BuildApp();
    }

    /// <summary>Executor that builds the app with custom cache sizes.</summary>
    internal sealed class WithCacheSizesExecutor : RxAppMigrationTestExecutor
    {
        /// <inheritdoc/>
        protected override void ConfigureBuilder() =>
            RxAppBuilder.CreateReactiveUIBuilder()
                .WithCacheSizes(CustomSmallCacheSize, CustomBigCacheSize)
                .WithCoreServices()
                .BuildApp();
    }

    /// <summary>Executor that builds the app using all RxApp migration methods chained together.</summary>
    internal sealed class WithAllMigrationMethodsExecutor : RxAppMigrationTestExecutor
    {
        /// <summary>Gets the exception captured by the custom handler.</summary>
        public static Exception? CapturedEx { get; private set; }

        /// <inheritdoc/>
        protected override void ConfigureBuilder()
        {
            CapturedEx = null;
            var customHandler = Witness.Create<Exception>(ex => CapturedEx = ex);

            _ = RxAppBuilder.CreateReactiveUIBuilder()
                .WithExceptionHandler(customHandler)
                .WithSuspensionHost<TestAppState>()
                .WithCacheSizes(ChainedSmallCacheSize, ChainedBigCacheSize)
                .WithCoreServices()
                .BuildApp();
        }
    }

    /// <summary>Executor that builds the app with two exception handlers to verify the last one wins.</summary>
    internal sealed class WithMultipleExceptionHandlersExecutor : RxAppMigrationTestExecutor
    {
        /// <summary>Gets the exception captured by the first handler.</summary>
        public static Exception? FirstCaptured { get; private set; }

        /// <summary>Gets the exception captured by the second handler.</summary>
        public static Exception? SecondCaptured { get; private set; }

        /// <inheritdoc/>
        protected override void ConfigureBuilder()
        {
            FirstCaptured = null;
            SecondCaptured = null;

            var firstHandler = Witness.Create<Exception>(ex => FirstCaptured = ex);
            var secondHandler = Witness.Create<Exception>(ex => SecondCaptured = ex);

            _ = RxAppBuilder.CreateReactiveUIBuilder()
                .WithExceptionHandler(firstHandler)
                .WithExceptionHandler(secondHandler)
                .WithCoreServices()
                .BuildApp();
        }
    }

    /// <summary>Executor that builds the app while setting cache sizes twice to verify the last call wins.</summary>
    internal sealed class WithMultipleCacheSizesExecutor : RxAppMigrationTestExecutor
    {
        /// <inheritdoc/>
        protected override void ConfigureBuilder() =>
            RxAppBuilder.CreateReactiveUIBuilder()
                .WithCacheSizes(FirstSmallCacheSize, FirstBigCacheSize)
                .WithCacheSizes(LastSmallCacheSize, LastBigCacheSize)
                .WithCoreServices()
                .BuildApp();
    }

    /// <summary>Executor that builds the app setting a non-generic then generic suspension host to verify the override.</summary>
    internal sealed class WithSuspensionHostOverrideExecutor : RxAppMigrationTestExecutor
    {
        /// <inheritdoc/>
        protected override void ConfigureBuilder() =>
            RxAppBuilder.CreateReactiveUIBuilder()
                .WithSuspensionHost()
                .WithSuspensionHost<TestAppState>()
                .WithCoreServices()
                .BuildApp();
    }

    /// <summary>Sample application state used to verify the typed suspension host.</summary>
    private sealed class TestAppState
    {
        /// <summary>Gets or sets the name.</summary>
        public string? Name { get; set; }

        /// <summary>Gets or sets the counter.</summary>
        public int Counter { get; set; }
    }
}
