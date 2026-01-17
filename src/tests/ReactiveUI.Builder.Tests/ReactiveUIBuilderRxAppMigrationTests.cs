// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive;
using Splat.Builder;

namespace ReactiveUI.Builder.Tests;

/// <summary>
/// Tests for RxApp migration functionality including WithExceptionHandler, WithSuspensionHost, and WithCacheSizes.
/// </summary>
[NotInParallel]
public class ReactiveUIBuilderRxAppMigrationTests
{
    /// <summary>
    /// Resets ReactiveUI static state before each test.
    /// </summary>
    [Before(Test)]
    public void SetUp()
    {
        RxAppBuilder.ResetForTesting();
        RxCacheSize.ResetForTesting();
        RxState.ResetForTesting();
        RxSuspension.ResetForTesting();
    }

    [Test]
    public async Task WithExceptionHandler_Should_Set_Custom_Exception_Handler()
    {
        AppBuilder.ResetBuilderStateForTests();
        using var locator = new ModernDependencyResolver();

        Exception? capturedEx = null;
        var customHandler = Observer.Create<Exception>(ex => capturedEx = ex);

        locator.CreateReactiveUIBuilder()
            .WithExceptionHandler(customHandler)
            .WithCoreServices()
            .BuildApp();

        var testException = new InvalidOperationException("Test exception");
        RxState.DefaultExceptionHandler.OnNext(testException);

        await Assert.That(capturedEx).IsEqualTo(testException);
    }

    [Test]
    public void WithExceptionHandler_With_Null_Handler_Should_Throw()
    {
        AppBuilder.ResetBuilderStateForTests();
        using var locator = new ModernDependencyResolver();
        var builder = locator.CreateReactiveUIBuilder();

        Assert.Throws<ArgumentNullException>(() => builder.WithExceptionHandler(null!));
    }

    [Test]
    public async Task WithSuspensionHost_NonGeneric_Should_Create_Default_Host()
    {
        AppBuilder.ResetBuilderStateForTests();
        using var locator = new ModernDependencyResolver();

        locator.CreateReactiveUIBuilder()
            .WithSuspensionHost()
            .WithCoreServices()
            .BuildApp();

        var host = RxSuspension.SuspensionHost;
        await Assert.That(host).IsNotNull();
        await Assert.That(host).IsTypeOf<SuspensionHost>();
    }

    [Test]
    public async Task WithSuspensionHost_Generic_Should_Create_Typed_Host()
    {
        AppBuilder.ResetBuilderStateForTests();
        using var locator = new ModernDependencyResolver();

        locator.CreateReactiveUIBuilder()
            .WithSuspensionHost<TestAppState>()
            .WithCoreServices()
            .BuildApp();

        var host = RxSuspension.SuspensionHost;
        await Assert.That(host).IsNotNull();
        await Assert.That(host).IsTypeOf<SuspensionHost<TestAppState>>();
    }

    [Test]
    public async Task WithCacheSizes_Should_Set_Custom_Cache_Sizes()
    {
        AppBuilder.ResetBuilderStateForTests();
        using var locator = new ModernDependencyResolver();

        locator.CreateReactiveUIBuilder()
            .WithCacheSizes(smallCacheLimit: 128, bigCacheLimit: 512)
            .WithCoreServices()
            .BuildApp();

        await Assert.That(RxCacheSize.SmallCacheLimit).IsEqualTo(128);
        await Assert.That(RxCacheSize.BigCacheLimit).IsEqualTo(512);
    }

    [Test]
    public void WithCacheSizes_With_Zero_Or_Negative_Values_Should_Throw()
    {
        AppBuilder.ResetBuilderStateForTests();
        using var locator = new ModernDependencyResolver();
        var builder = locator.CreateReactiveUIBuilder();

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
        AppBuilder.ResetBuilderStateForTests();
        using var locator = new ModernDependencyResolver();

        locator.CreateReactiveUIBuilder()
            .WithCoreServices()
            .BuildApp();

#if ANDROID || IOS
        await Assert.That(RxCacheSize.SmallCacheLimit).IsEqualTo(32);
        await Assert.That(RxCacheSize.BigCacheLimit).IsEqualTo(64);
#else
        await Assert.That(RxCacheSize.SmallCacheLimit).IsEqualTo(64);
        await Assert.That(RxCacheSize.BigCacheLimit).IsEqualTo(256);
#endif
    }

    [Test]
    public async Task Builder_Should_Support_Chaining_All_RxApp_Migration_Methods()
    {
        AppBuilder.ResetBuilderStateForTests();
        using var locator = new ModernDependencyResolver();

        Exception? capturedEx = null;
        var customHandler = Observer.Create<Exception>(ex => capturedEx = ex);

        locator.CreateReactiveUIBuilder()
            .WithExceptionHandler(customHandler)
            .WithSuspensionHost<TestAppState>()
            .WithCacheSizes(smallCacheLimit: 100, bigCacheLimit: 400)
            .WithCoreServices()
            .BuildApp();

        // Verify exception handler
        var testException = new InvalidOperationException("Test");
        RxState.DefaultExceptionHandler.OnNext(testException);
        await Assert.That(capturedEx).IsEqualTo(testException);

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
        AppBuilder.ResetBuilderStateForTests();
        using var locator = new ModernDependencyResolver();

        locator.CreateReactiveUIBuilder()
            .WithCoreServices()
            .BuildApp();

        var handler = RxState.DefaultExceptionHandler;
        await Assert.That(handler).IsNotNull();
    }

    [Test]
    public async Task RxSchedulers_SuspensionHost_Should_Not_Be_Null()
    {
        AppBuilder.ResetBuilderStateForTests();
        using var locator = new ModernDependencyResolver();

        locator.CreateReactiveUIBuilder()
            .WithCoreServices()
            .BuildApp();

        var host = RxSuspension.SuspensionHost;
        await Assert.That(host).IsNotNull();
    }

    [Test]
    public async Task WithExceptionHandler_Called_Multiple_Times_Should_Use_Last_Handler()
    {
        AppBuilder.ResetBuilderStateForTests();
        using var locator = new ModernDependencyResolver();

        Exception? firstCaptured = null;
        Exception? secondCaptured = null;

        var firstHandler = Observer.Create<Exception>(ex => firstCaptured = ex);
        var secondHandler = Observer.Create<Exception>(ex => secondCaptured = ex);

        locator.CreateReactiveUIBuilder()
            .WithExceptionHandler(firstHandler)
            .WithExceptionHandler(secondHandler)
            .WithCoreServices()
            .BuildApp();

        var testException = new InvalidOperationException("Test");
        RxState.DefaultExceptionHandler.OnNext(testException);

        await Assert.That(secondCaptured).IsEqualTo(testException);
        await Assert.That(firstCaptured).IsNull();
    }

    [Test]
    public async Task WithCacheSizes_Called_Multiple_Times_Should_Use_Last_Values()
    {
        AppBuilder.ResetBuilderStateForTests();
        using var locator = new ModernDependencyResolver();

        locator.CreateReactiveUIBuilder()
            .WithCacheSizes(smallCacheLimit: 100, bigCacheLimit: 200)
            .WithCacheSizes(smallCacheLimit: 300, bigCacheLimit: 600)
            .WithCoreServices()
            .BuildApp();

        await Assert.That(RxCacheSize.SmallCacheLimit).IsEqualTo(300);
        await Assert.That(RxCacheSize.BigCacheLimit).IsEqualTo(600);
    }

    [Test]
    public async Task WithSuspensionHost_Generic_Overrides_NonGeneric()
    {
        AppBuilder.ResetBuilderStateForTests();
        using var locator = new ModernDependencyResolver();

        locator.CreateReactiveUIBuilder()
            .WithSuspensionHost()
            .WithSuspensionHost<TestAppState>()
            .WithCoreServices()
            .BuildApp();

        var host = RxSuspension.SuspensionHost;
        await Assert.That(host).IsTypeOf<SuspensionHost<TestAppState>>();
    }

    private class TestAppState
    {
        public string? Name { get; set; }

        public int Counter { get; set; }
    }
}
