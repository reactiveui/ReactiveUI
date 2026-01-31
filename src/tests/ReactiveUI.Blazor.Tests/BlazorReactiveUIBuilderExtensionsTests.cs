// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive.Concurrency;
using ReactiveUI.Builder;

namespace ReactiveUI.Blazor.Tests;

/// <summary>
/// Tests for the <see cref="BlazorReactiveUIBuilderExtensions"/> class.
/// These tests verify the Blazor-specific builder extensions for configuring ReactiveUI.
/// </summary>
public class BlazorReactiveUIBuilderExtensionsTests
{
    /// <summary>
    /// Verifies that BlazorMainThreadScheduler returns CurrentThreadScheduler.Instance.
    /// </summary>
    /// <returns>A Task representing the asynchronous test operation.</returns>
    [Test]
    public async Task BlazorMainThreadScheduler_ReturnsCurrentThreadScheduler()
    {
        var scheduler = BlazorReactiveUIBuilderExtensions.BlazorMainThreadScheduler;

        await Assert.That(scheduler).IsSameReferenceAs(CurrentThreadScheduler.Instance);
    }

    /// <summary>
    /// Verifies that BlazorWasmScheduler returns WasmScheduler.Default.
    /// </summary>
    /// <returns>A Task representing the asynchronous test operation.</returns>
    [Test]
    public async Task BlazorWasmScheduler_ReturnsWasmScheduler()
    {
        var scheduler = BlazorReactiveUIBuilderExtensions.BlazorWasmScheduler;

        await Assert.That(scheduler).IsSameReferenceAs(WasmScheduler.Default);
    }

    /// <summary>
    /// Verifies that WithBlazor calls WithBlazorScheduler and WithPlatformModule.
    /// </summary>
    /// <returns>A Task representing the asynchronous test operation.</returns>
    [Test]
    public async Task WithBlazor_ConfiguresBlazorSchedulerAndPlatformModule()
    {
        var builder = new TestReactiveUIBuilder();

        var result = builder.WithBlazor();

        await Assert.That(result).IsSameReferenceAs(builder);
        await Assert.That(builder.MainThreadSchedulerSet).IsTrue();
        await Assert.That(builder.MainThreadScheduler).IsSameReferenceAs(CurrentThreadScheduler.Instance);
        await Assert.That(builder.PlatformModuleCalled).IsTrue();
    }

    /// <summary>
    /// Verifies that WithBlazor throws ArgumentNullException when builder is null.
    /// </summary>
    /// <returns>A Task representing the asynchronous test operation.</returns>
    [Test]
    public async Task WithBlazor_ThrowsArgumentNullException_WhenBuilderIsNull()
    {
        IReactiveUIBuilder? builder = null;

        var exception = await Assert.That(() => builder!.WithBlazor()).Throws<ArgumentNullException>();
        await Assert.That(exception).IsNotNull();
        await Assert.That(exception.ParamName).IsEqualTo("builder");
    }

    /// <summary>
    /// Verifies that WithBlazorScheduler sets the main thread scheduler to CurrentThreadScheduler.
    /// </summary>
    /// <returns>A Task representing the asynchronous test operation.</returns>
    [Test]
    public async Task WithBlazorScheduler_SetsMainThreadScheduler()
    {
        var builder = new TestReactiveUIBuilder();

        var result = builder.WithBlazorScheduler();

        await Assert.That(result).IsSameReferenceAs(builder);
        await Assert.That(builder.MainThreadSchedulerSet).IsTrue();
        await Assert.That(builder.MainThreadScheduler).IsSameReferenceAs(CurrentThreadScheduler.Instance);
    }

    /// <summary>
    /// Verifies that WithBlazorScheduler throws ArgumentNullException when builder is null.
    /// </summary>
    /// <returns>A Task representing the asynchronous test operation.</returns>
    [Test]
    public async Task WithBlazorScheduler_ThrowsArgumentNullException_WhenBuilderIsNull()
    {
        IReactiveUIBuilder? builder = null;

        var exception = await Assert.That(() => builder!.WithBlazorScheduler()).Throws<ArgumentNullException>();
        await Assert.That(exception).IsNotNull();
        await Assert.That(exception.ParamName).IsEqualTo("builder");
    }

    /// <summary>
    /// Verifies that WithBlazorWasmScheduler sets the main thread scheduler to WasmScheduler.
    /// </summary>
    /// <returns>A Task representing the asynchronous test operation.</returns>
    [Test]
    public async Task WithBlazorWasmScheduler_SetsMainThreadScheduler()
    {
        var builder = new TestReactiveUIBuilder();

        var result = builder.WithBlazorWasmScheduler();

        await Assert.That(result).IsSameReferenceAs(builder);
        await Assert.That(builder.MainThreadSchedulerSet).IsTrue();
        await Assert.That(builder.MainThreadScheduler).IsSameReferenceAs(WasmScheduler.Default);
    }

    /// <summary>
    /// Verifies that WithBlazorWasmScheduler throws ArgumentNullException when builder is null.
    /// </summary>
    /// <returns>A Task representing the asynchronous test operation.</returns>
    [Test]
    public async Task WithBlazorWasmScheduler_ThrowsArgumentNullException_WhenBuilderIsNull()
    {
        IReactiveUIBuilder? builder = null;

        var exception = await Assert.That(() => builder!.WithBlazorWasmScheduler()).Throws<ArgumentNullException>();
        await Assert.That(exception).IsNotNull();
        await Assert.That(exception.ParamName).IsEqualTo("builder");
    }

    /// <summary>
    /// A test implementation of IReactiveUIBuilder for testing purposes.
    /// </summary>
    private class TestReactiveUIBuilder : IReactiveUIBuilder
    {
        public IScheduler? MainThreadScheduler { get; private set; }

        public IScheduler? TaskpoolScheduler { get; private set; }

        public bool MainThreadSchedulerSet { get; private set; }

        public bool TaskpoolSchedulerSet { get; private set; }

        public bool PlatformModuleCalled { get; private set; }

        public IReactiveUIBuilder WithMainThreadScheduler(IScheduler scheduler, bool setRxApp = true)
        {
            MainThreadScheduler = scheduler;
            MainThreadSchedulerSet = true;
            return this;
        }

        public IReactiveUIBuilder WithPlatformModule<T>()
            where T : IWantsToRegisterStuff, new()
        {
            PlatformModuleCalled = true;
            return this;
        }

        public Splat.Builder.IAppInstance Build() => throw new NotImplementedException();

        public Splat.Builder.IAppBuilder UseCurrentSplatLocator() => throw new NotImplementedException();

        public Splat.Builder.IAppBuilder UsingModule<T>(T registrationModule)
            where T : Splat.Builder.IModule => throw new NotImplementedException();

        public Splat.Builder.IAppBuilder WithCoreServices() => throw new NotImplementedException();

        public Splat.Builder.IAppBuilder WithCustomRegistration(Action<IMutableDependencyResolver> configureAction) => throw new NotImplementedException();

        public IReactiveUIBuilder WithMessageBus() => throw new NotImplementedException();

        public IReactiveUIBuilder WithMessageBus(Action<IMessageBus> configure) => throw new NotImplementedException();

        public IReactiveUIBuilder WithMessageBus(IMessageBus messageBus) => throw new NotImplementedException();

        public IReactiveUIBuilder ConfigureSuspensionDriver(Action<ISuspensionDriver> configure) => throw new NotImplementedException();

        public IReactiveUIBuilder ConfigureViewLocator(Action<DefaultViewLocator> configure) => throw new NotImplementedException();

        public IReactiveUIBuilder ForCustomPlatform(IScheduler mainThreadScheduler, Action<IMutableDependencyResolver> platformServices) => throw new NotImplementedException();

        public IReactiveUIBuilder ForPlatforms(params Action<IReactiveUIBuilder>[] platformConfigurations) => throw new NotImplementedException();

        public IReactiveUIBuilder RegisterSingletonView<TView, TViewModel>()
            where TView : class, IViewFor<TViewModel>, new()
            where TViewModel : class, IReactiveObject => throw new NotImplementedException();

        public IReactiveUIBuilder RegisterSingletonViewModel<TViewModel>()
            where TViewModel : class, IReactiveObject, new() => throw new NotImplementedException();

        public IReactiveUIBuilder RegisterView<TView, TViewModel>()
            where TView : class, IViewFor<TViewModel>, new()
            where TViewModel : class, IReactiveObject => throw new NotImplementedException();

        public IReactiveUIBuilder RegisterViewModel<TViewModel>()
            where TViewModel : class, IReactiveObject, new() => throw new NotImplementedException();

        public IReactiveUIBuilder RegisterConstantViewModel<TViewModel>()
            where TViewModel : class, IReactiveObject, new() => throw new NotImplementedException();

        public IReactiveUIBuilder WithPlatformServices() => throw new NotImplementedException();

        public IReactiveUIBuilder WithRegistration(Action<IMutableDependencyResolver> configureAction) => throw new NotImplementedException();

        public IReactiveUIBuilder WithRegistrationOnBuild(Action<IMutableDependencyResolver> configureAction) => throw new NotImplementedException();

        public IReactiveUIBuilder WithTaskPoolScheduler(IScheduler scheduler, bool setRxApp = true)
        {
            TaskpoolScheduler = scheduler;
            TaskpoolSchedulerSet = true;
            return this;
        }

        public IReactiveUIBuilder WithViewsFromAssembly(System.Reflection.Assembly assembly) => throw new NotImplementedException();

        public IReactiveUIBuilder UsingSplatModule<T>(T registrationModule)
            where T : Splat.Builder.IModule => throw new NotImplementedException();

        public IReactiveUIInstance BuildApp() => throw new NotImplementedException();

        public IReactiveUIInstance WithInstance<T>(Action<T?> action) => throw new NotImplementedException();

        public IReactiveUIInstance WithInstance<T1, T2>(Action<T1?, T2?> action) => throw new NotImplementedException();

        public IReactiveUIInstance WithInstance<T1, T2, T3>(Action<T1?, T2?, T3?> action) => throw new NotImplementedException();

        public IReactiveUIInstance WithInstance<T1, T2, T3, T4>(Action<T1?, T2?, T3?, T4?> action) => throw new NotImplementedException();

        public IReactiveUIInstance WithInstance<T1, T2, T3, T4, T5>(Action<T1?, T2?, T3?, T4?, T5?> action) => throw new NotImplementedException();

        public IReactiveUIInstance WithInstance<T1, T2, T3, T4, T5, T6>(Action<T1?, T2?, T3?, T4?, T5?, T6?> action) => throw new NotImplementedException();

        public IReactiveUIInstance WithInstance<T1, T2, T3, T4, T5, T6, T7>(Action<T1?, T2?, T3?, T4?, T5?, T6?, T7?> action) => throw new NotImplementedException();

        public IReactiveUIInstance WithInstance<T1, T2, T3, T4, T5, T6, T7, T8>(Action<T1?, T2?, T3?, T4?, T5?, T6?, T7?, T8?> action) => throw new NotImplementedException();

        public IReactiveUIInstance WithInstance<T1, T2, T3, T4, T5, T6, T7, T8, T9>(Action<T1?, T2?, T3?, T4?, T5?, T6?, T7?, T8?, T9?> action) => throw new NotImplementedException();

        public IReactiveUIInstance WithInstance<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Action<T1?, T2?, T3?, T4?, T5?, T6?, T7?, T8?, T9?, T10?> action) => throw new NotImplementedException();

        public IReactiveUIInstance WithInstance<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(Action<T1?, T2?, T3?, T4?, T5?, T6?, T7?, T8?, T9?, T10?, T11?> action) => throw new NotImplementedException();

        public IReactiveUIInstance WithInstance<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(Action<T1?, T2?, T3?, T4?, T5?, T6?, T7?, T8?, T9?, T10?, T11?, T12?> action) => throw new NotImplementedException();

        public IReactiveUIInstance WithInstance<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(Action<T1?, T2?, T3?, T4?, T5?, T6?, T7?, T8?, T9?, T10?, T11?, T12?, T13?> action) => throw new NotImplementedException();

        public IReactiveUIInstance WithInstance<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(Action<T1?, T2?, T3?, T4?, T5?, T6?, T7?, T8?, T9?, T10?, T11?, T12?, T13?, T14?> action) => throw new NotImplementedException();

        public IReactiveUIInstance WithInstance<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(Action<T1?, T2?, T3?, T4?, T5?, T6?, T7?, T8?, T9?, T10?, T11?, T12?, T13?, T14?, T15?> action) => throw new NotImplementedException();

        public IReactiveUIInstance WithInstance<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(Action<T1?, T2?, T3?, T4?, T5?, T6?, T7?, T8?, T9?, T10?, T11?, T12?, T13?, T14?, T15?, T16?> action) => throw new NotImplementedException();

        public IReactiveUIBuilder WithExceptionHandler(IObserver<Exception> exceptionHandler) => throw new NotImplementedException();

        public IReactiveUIBuilder WithSuspensionHost() => throw new NotImplementedException();

        public IReactiveUIBuilder WithSuspensionHost<TAppState>() => throw new NotImplementedException();

        public IReactiveUIBuilder WithCacheSizes(int smallCacheLimit, int bigCacheLimit) => throw new NotImplementedException();
    }
}
