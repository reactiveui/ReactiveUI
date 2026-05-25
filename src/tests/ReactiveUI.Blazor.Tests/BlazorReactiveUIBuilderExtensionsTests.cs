// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using System.Reactive.Concurrency;
using ReactiveUI.Builder;
using Splat;

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
    [SuppressMessage(
        "Major Code Smell",
        "S4018:Generic methods should provide type parameters",
        Justification = "Mock must match interface generic methods that take no argument of the type parameter.")]
    private sealed class TestReactiveUIBuilder : IReactiveUIBuilder
    {
        /// <summary>
        /// Gets the main thread scheduler that was set on the builder.
        /// </summary>
        public IScheduler? MainThreadScheduler { get; private set; }

        /// <summary>
        /// Gets the task pool scheduler that was set on the builder.
        /// </summary>
        public IScheduler? TaskpoolScheduler { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the main thread scheduler was set.
        /// </summary>
        public bool MainThreadSchedulerSet { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the task pool scheduler was set.
        /// </summary>
        public bool TaskpoolSchedulerSet { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the platform module registration was called.
        /// </summary>
        public bool PlatformModuleCalled { get; private set; }

        /// <inheritdoc/>
        public IReactiveUIBuilder WithMainThreadScheduler(IScheduler scheduler) => WithMainThreadScheduler(scheduler, true);

        /// <inheritdoc/>
        public IReactiveUIBuilder WithMainThreadScheduler(IScheduler scheduler, bool setRxApp)
        {
            MainThreadScheduler = scheduler;
            MainThreadSchedulerSet = true;
            return this;
        }

        /// <inheritdoc/>
        public IReactiveUIBuilder WithPlatformModule<T>()
            where T : IWantsToRegisterStuff, new()
        {
            PlatformModuleCalled = true;
            return this;
        }

        /// <inheritdoc/>
        public Splat.Builder.IAppInstance Build() => throw new NotSupportedException();

        IReactiveUIInstance IReactiveUIBuilder.Build() => throw new NotSupportedException();

        /// <inheritdoc/>
        public Splat.Builder.IAppBuilder UseCurrentSplatLocator() => throw new NotSupportedException();

        /// <inheritdoc/>
        public Splat.Builder.IAppBuilder UsingModule<T>(T registrationModule)
            where T : Splat.Builder.IModule => throw new NotSupportedException();

        /// <inheritdoc/>
        public Splat.Builder.IAppBuilder WithCoreServices() => this;

        /// <inheritdoc/>
        public Splat.Builder.IAppBuilder WithCustomRegistration(Action<IMutableDependencyResolver> configureAction) =>
            throw new NotSupportedException();

        /// <inheritdoc/>
        public IReactiveUIBuilder WithMessageBus() => throw new NotSupportedException();

        /// <inheritdoc/>
        public IReactiveUIBuilder WithMessageBus(Action<IMessageBus> configure) => throw new NotSupportedException();

        /// <inheritdoc/>
        public IReactiveUIBuilder WithMessageBus(IMessageBus messageBus) => throw new NotSupportedException();

        /// <inheritdoc/>
        public IReactiveUIBuilder ConfigureSuspensionDriver(Action<ISuspensionDriver> configure) =>
            throw new NotSupportedException();

        /// <inheritdoc/>
        public IReactiveUIBuilder ConfigureViewLocator(Action<DefaultViewLocator> configure) =>
            throw new NotSupportedException();

        /// <inheritdoc/>
        public IReactiveUIBuilder ForCustomPlatform(
            IScheduler mainThreadScheduler,
            Action<IMutableDependencyResolver> platformServices) => throw new NotSupportedException();

        /// <inheritdoc/>
        public IReactiveUIBuilder ForPlatforms(params Action<IReactiveUIBuilder>[] platformConfigurations) =>
            throw new NotSupportedException();

        /// <inheritdoc/>
        public IReactiveUIBuilder RegisterSingletonView<TView, TViewModel>()
            where TView : class, IViewFor<TViewModel>, new()
            where TViewModel : class, IReactiveObject => throw new NotSupportedException();

        /// <inheritdoc/>
        public IReactiveUIBuilder RegisterSingletonViewModel<TViewModel>()
            where TViewModel : class, IReactiveObject, new() => throw new NotSupportedException();

        /// <inheritdoc/>
        public IReactiveUIBuilder RegisterView<TView, TViewModel>()
            where TView : class, IViewFor<TViewModel>, new()
            where TViewModel : class, IReactiveObject => throw new NotSupportedException();

        /// <inheritdoc/>
        public IReactiveUIBuilder RegisterViewModel<TViewModel>()
            where TViewModel : class, IReactiveObject, new() => throw new NotSupportedException();

        /// <inheritdoc/>
        public IReactiveUIBuilder RegisterConstantViewModel<TViewModel>()
            where TViewModel : class, IReactiveObject, new() => throw new NotSupportedException();

        /// <inheritdoc/>
        public IReactiveUIBuilder WithPlatformServices() => throw new NotSupportedException();

        /// <inheritdoc/>
        public IReactiveUIBuilder WithRegistration(Action<IMutableDependencyResolver> configureAction) =>
            throw new NotSupportedException();

        /// <inheritdoc/>
        public IReactiveUIBuilder WithRegistrationOnBuild(Action<IMutableDependencyResolver> configureAction) =>
            throw new NotSupportedException();

        /// <inheritdoc/>
        public IReactiveUIBuilder WithTaskPoolScheduler(IScheduler scheduler) => WithTaskPoolScheduler(scheduler, true);

        /// <inheritdoc/>
        public IReactiveUIBuilder WithTaskPoolScheduler(IScheduler scheduler, bool setRxApp)
        {
            TaskpoolScheduler = scheduler;
            TaskpoolSchedulerSet = true;
            return this;
        }

        /// <inheritdoc/>
        public IReactiveUIBuilder WithViewsFromAssembly(System.Reflection.Assembly assembly) =>
            throw new NotSupportedException();

        /// <inheritdoc/>
        public IReactiveUIBuilder UsingSplatModule<T>(T registrationModule)
            where T : Splat.Builder.IModule => throw new NotSupportedException();

        /// <inheritdoc/>
        public IReactiveUIInstance BuildApp() => throw new NotSupportedException();

        /// <inheritdoc/>
        public IReactiveUIInstance WithInstance<T>(Action<T?> action) => throw new NotSupportedException();

        /// <inheritdoc/>
        public IReactiveUIInstance WithInstance<T1, T2>(Action<T1?, T2?> action) => throw new NotSupportedException();

        /// <inheritdoc/>
        public IReactiveUIInstance WithInstance<T1, T2, T3>(Action<T1?, T2?, T3?> action) =>
            throw new NotSupportedException();

        /// <inheritdoc/>
        public IReactiveUIInstance WithInstance<T1, T2, T3, T4>(Action<T1?, T2?, T3?, T4?> action) =>
            throw new NotSupportedException();

        /// <inheritdoc/>
        public IReactiveUIInstance WithInstance<T1, T2, T3, T4, T5>(Action<T1?, T2?, T3?, T4?, T5?> action) =>
            throw new NotSupportedException();

        /// <inheritdoc/>
        public IReactiveUIInstance WithInstance<T1, T2, T3, T4, T5, T6>(Action<T1?, T2?, T3?, T4?, T5?, T6?> action) =>
            throw new NotSupportedException();

        /// <inheritdoc/>
        public IReactiveUIInstance WithInstance<T1, T2, T3, T4, T5, T6, T7>(
            Action<T1?, T2?, T3?, T4?, T5?, T6?, T7?> action) => throw new NotSupportedException();

        /// <inheritdoc/>
        public IReactiveUIInstance WithInstance<T1, T2, T3, T4, T5, T6, T7, T8>(
            Action<T1?, T2?, T3?, T4?, T5?, T6?, T7?, T8?> action) => throw new NotSupportedException();

        /// <inheritdoc/>
        public IReactiveUIInstance WithInstance<T1, T2, T3, T4, T5, T6, T7, T8, T9>(
            Action<T1?, T2?, T3?, T4?, T5?, T6?, T7?, T8?, T9?> action) => throw new NotSupportedException();

        /// <inheritdoc/>
        public IReactiveUIInstance WithInstance<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(
            Action<T1?, T2?, T3?, T4?, T5?, T6?, T7?, T8?, T9?, T10?> action) => throw new NotSupportedException();

        /// <inheritdoc/>
        public IReactiveUIInstance WithInstance<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(
            Action<T1?, T2?, T3?, T4?, T5?, T6?, T7?, T8?, T9?, T10?, T11?> action) =>
            throw new NotSupportedException();

        /// <inheritdoc/>
        public IReactiveUIInstance WithInstance<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(
            Action<T1?, T2?, T3?, T4?, T5?, T6?, T7?, T8?, T9?, T10?, T11?, T12?> action) =>
            throw new NotSupportedException();

        /// <inheritdoc/>
        public IReactiveUIInstance WithInstance<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(
            Action<T1?, T2?, T3?, T4?, T5?, T6?, T7?, T8?, T9?, T10?, T11?, T12?, T13?> action) =>
            throw new NotSupportedException();

        /// <inheritdoc/>
        public IReactiveUIInstance WithInstance<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(
            Action<T1?, T2?, T3?, T4?, T5?, T6?, T7?, T8?, T9?, T10?, T11?, T12?, T13?, T14?> action) =>
            throw new NotSupportedException();

        /// <inheritdoc/>
        public IReactiveUIInstance WithInstance<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(
            Action<T1?, T2?, T3?, T4?, T5?, T6?, T7?, T8?, T9?, T10?, T11?, T12?, T13?, T14?, T15?> action) =>
            throw new NotSupportedException();

        /// <inheritdoc/>
        public IReactiveUIInstance WithInstance<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(
            Action<T1?, T2?, T3?, T4?, T5?, T6?, T7?, T8?, T9?, T10?, T11?, T12?, T13?, T14?, T15?, T16?> action) =>
            throw new NotSupportedException();

        /// <inheritdoc/>
        public IReactiveUIBuilder WithExceptionHandler(IObserver<Exception> exceptionHandler) =>
            throw new NotSupportedException();

        /// <inheritdoc/>
        public IReactiveUIBuilder WithSuspensionHost() => throw new NotSupportedException();

        /// <inheritdoc/>
        public IReactiveUIBuilder WithSuspensionHost<TAppState>() => throw new NotSupportedException();

        /// <inheritdoc/>
        public IReactiveUIBuilder WithCacheSizes(int smallCacheLimit, int bigCacheLimit) =>
            throw new NotSupportedException();

        /// <inheritdoc/>
        public IReactiveUIBuilder WithConverter<TFrom, TTo>(BindingTypeConverter<TFrom, TTo> converter) =>
            throw new NotSupportedException();

        /// <inheritdoc/>
        public IReactiveUIBuilder WithConverter(IBindingTypeConverter converter) => throw new NotSupportedException();

        /// <inheritdoc/>
        public IReactiveUIBuilder WithConverter<TFrom, TTo>(Func<BindingTypeConverter<TFrom, TTo>> factory) =>
            throw new NotSupportedException();

        /// <inheritdoc/>
        public IReactiveUIBuilder WithConverter(Func<IBindingTypeConverter> factory) =>
            throw new NotSupportedException();

        /// <inheritdoc/>
        public IReactiveUIBuilder WithFallbackConverter(IBindingFallbackConverter converter) =>
            throw new NotSupportedException();

        /// <inheritdoc/>
        public IReactiveUIBuilder WithFallbackConverter(Func<IBindingFallbackConverter> factory) =>
            throw new NotSupportedException();

        /// <inheritdoc/>
        public IReactiveUIBuilder WithSetMethodConverter(ISetMethodBindingConverter converter) =>
            throw new NotSupportedException();

        /// <inheritdoc/>
        public IReactiveUIBuilder WithSetMethodConverter(Func<ISetMethodBindingConverter> factory) =>
            throw new NotSupportedException();

        /// <inheritdoc/>
        public IReactiveUIBuilder WithConvertersFrom(IReadonlyDependencyResolver resolver) =>
            throw new NotSupportedException();
    }
}
