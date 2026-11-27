// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Runtime.CompilerServices;
using System.Windows;
using ReactiveUI.Primitives.Disposables;
using ReactiveUI.Tests.Xaml.Utilities;
using Splat;
using Splat.Builder;
using TUnit.Core.Executors;

namespace ReactiveUI.Tests.Wpf;

/// <summary>Tests for <see cref="WpfReactiveUIBuilderExtensions"/>.</summary>
[NotInParallel]
[TestExecutor<DispatcherThreadExecutor>]
public class WpfReactiveUiBuilderExtensionsTest
{
    /// <summary>The number of work items the deferred scheduler test schedules.</summary>
#if REACTIVE_SHIM
    private const int ExpectedDeferredRuns = 3;
#else
    private const int ExpectedDeferredRuns = 2;
#endif

    /// <summary>The longest wait for the deferred scheduler's work to run.</summary>
    private static readonly TimeSpan DeferredWorkTimeout = TimeSpan.FromSeconds(10);

    /// <summary>The wait between dispatcher pumps while the deferred scheduler's work runs.</summary>
    private static readonly TimeSpan DeferredWorkPollInterval = TimeSpan.FromMilliseconds(10);

    /// <summary>Tests that WpfMainThreadScheduler is not null.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task WpfMainThreadScheduler_IsNotNull() =>
        await Assert.That(WpfReactiveUIBuilderExtensions.WpfMainThreadScheduler).IsNotNull();

    /// <summary>Tests that WpfMainThreadScheduler throws on a thread without a dispatcher before an application exists.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task WpfMainThreadScheduler_WithoutDispatcher_Throws()
    {
        if (Application.Current is not null)
        {
            return;
        }

        await Assert.That(static () => Task.Factory.StartNew<ISequencer?>(
                static () => WpfReactiveUIBuilderExtensions.WpfMainThreadScheduler,
                CancellationToken.None,
                TaskCreationOptions.LongRunning,
                TaskScheduler.Default))
            .ThrowsExactly<InvalidOperationException>();
    }

    /// <summary>Tests that WithWpf throws when builder is null.</summary>
    [Test]
    public void WithWpf_ThrowsArgumentNullException_WhenBuilderIsNull() =>
        _ = Assert.Throws<ArgumentNullException>(static () =>
            WpfReactiveUIBuilderExtensions.WithWpf(null!));

    /// <summary>Tests that WithWpf configures builder correctly.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task WithWpf_ConfiguresBuilder()
    {
        AppBuilder.ResetBuilderStateForTests();
        var resolver = new ModernDependencyResolver();
        resolver.InitializeSplat();

        using (resolver.WithResolver())
        {
            var builder = resolver.CreateReactiveUIBuilder();

            var result = builder.WithWpf();

            await Assert.That(result).IsNotNull();
            await Assert.That(result).IsSameReferenceAs(builder);
        }
    }

    /// <summary>Tests that WithWpfScheduler throws when builder is null.</summary>
    [Test]
    public void WithWpfScheduler_ThrowsArgumentNullException_WhenBuilderIsNull() =>
        _ = Assert.Throws<ArgumentNullException>(static () =>
            WpfReactiveUIBuilderExtensions.WithWpfScheduler(null!));

    /// <summary>Tests that WithWpfScheduler configures scheduler correctly.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task WithWpfScheduler_ConfiguresScheduler()
    {
        AppBuilder.ResetBuilderStateForTests();
        var resolver = new ModernDependencyResolver();
        resolver.InitializeSplat();

        using (resolver.WithResolver())
        {
            var builder = resolver.CreateReactiveUIBuilder();

            var result = builder.WithWpfScheduler();

            await Assert.That(result).IsNotNull();
            await Assert.That(result).IsSameReferenceAs(builder);
        }
    }

    /// <summary>Tests that WithWpfScheduler registers the WPF dispatcher sequencer, or the deferred one before an application exists.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task WithWpfScheduler_RegistersDispatcherSequencer()
    {
        AppBuilder.ResetBuilderStateForTests();
        var resolver = new ModernDependencyResolver();
        resolver.InitializeSplat();

        using (resolver.WithResolver())
        {
            var builder = resolver.CreateReactiveUIBuilder();

            _ = builder.WithWpfScheduler();

            await Assert.That(builder.MainThreadScheduler).IsNotNull();
            if (Application.Current is null)
            {
                await Assert.That(builder.MainThreadScheduler!).IsSameReferenceAs(DeferredWpfMainThreadSequencer.Instance);
            }
            else
            {
                await Assert.That(builder.MainThreadScheduler!).IsTypeOf<DispatcherSequencer>();
            }
        }
    }

    /// <summary>Tests that WithWpf can build on a thread without a dispatcher before an application exists, as a generic host does.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task WithWpf_BuildApp_WithoutDispatcherBeforeApplication_UsesDeferredScheduler()
    {
        if (Application.Current is not null)
        {
            return;
        }

        AppBuilder.ResetBuilderStateForTests();
        var resolver = new ModernDependencyResolver();
        resolver.InitializeSplat();

        using (resolver.WithResolver())
        {
            var scheduler = await Task.Factory.StartNew(
                static state =>
                {
                    var builder = ((ModernDependencyResolver)state!).CreateReactiveUIBuilder();
                    _ = builder.WithWpf().BuildApp();
                    return builder.MainThreadScheduler;
                },
                resolver,
                CancellationToken.None,
                TaskCreationOptions.LongRunning,
                TaskScheduler.Default);

            await Assert.That(scheduler).IsSameReferenceAs(DeferredWpfMainThreadSequencer.Instance);
            await Assert.That(DeferredWpfMainThreadSequencer.Instance.CheckAccess()).IsFalse();
        }
    }

    /// <summary>Tests that the deferred scheduler runs immediate and delayed work on the dispatcher it finds when first used.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task DeferredScheduler_RunsWorkOnTheDispatcherFoundWhenFirstUsed()
    {
        // Resolve this test thread's dispatcher rather than DispatcherSequencer.Main, which an Application created by
        // another test would bind to a thread nobody pumps.
        var sequencer = new DeferredWpfMainThreadSequencer(static () => DispatcherSequencer.Current);
        var runs = new StrongBox<int>();
        var before = TimeProvider.System.GetUtcNow();

        _ = sequencer.Schedule(runs, static (_, box) => CountRun(box));
        _ = sequencer.Schedule(runs, TimeSpan.Zero, static (_, box) => CountRun(box));
#if REACTIVE_SHIM
        _ = sequencer.Schedule(runs, before, static (_, box) => CountRun(box));
#endif

        using var timeout = new CancellationTokenSource(DeferredWorkTimeout);
        while (runs.Value < ExpectedDeferredRuns && !timeout.IsCancellationRequested)
        {
            DispatcherUtilities.DoEvents();
            await Task.Delay(DeferredWorkPollInterval, CancellationToken.None);
        }

        using (Assert.Multiple())
        {
            await Assert.That(runs.Value).IsEqualTo(ExpectedDeferredRuns);
            await Assert.That(sequencer.Now).IsGreaterThanOrEqualTo(before);
        }
    }

    /// <summary>Tests that WithWpfConverters throws when builder is null.</summary>
    [Test]
    public void WithWpfConverters_ThrowsArgumentNullException_WhenBuilderIsNull() =>
        _ = Assert.Throws<ArgumentNullException>(static () =>
            WpfReactiveUIBuilderExtensions.WithWpfConverters(null!));

    /// <summary>Tests that WithWpfConverters registers WPF-specific converters in the ConverterService.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task WithWpfConverters_RegistersWpfSpecificConverters()
    {
        AppBuilder.ResetBuilderStateForTests();
        var resolver = new ModernDependencyResolver();
        resolver.InitializeSplat();

        using (resolver.WithResolver())
        {
            var builder = resolver.CreateReactiveUIBuilder();

            _ = builder.WithWpfConverters();

            // Verify BooleanToVisibilityTypeConverter is registered
            var boolToVisibility = builder.ConverterService.TypedConverters.TryGetConverter(typeof(bool), typeof(Visibility));
            await Assert.That(boolToVisibility).IsNotNull();
            await Assert.That(boolToVisibility).IsTypeOf<BooleanToVisibilityTypeConverter>();

            // Verify VisibilityToBooleanTypeConverter is registered
            var visibilityToBool = builder.ConverterService.TypedConverters.TryGetConverter(typeof(Visibility), typeof(bool));
            await Assert.That(visibilityToBool).IsNotNull();
            await Assert.That(visibilityToBool).IsTypeOf<VisibilityToBooleanTypeConverter>();
        }
    }

    /// <summary>Tests that WithWpf registers all required converters to the ConverterService via BuildApp.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task WithWpf_BuildApp_RegistersAllConvertersToConverterService()
    {
        AppBuilder.ResetBuilderStateForTests();
        var resolver = new ModernDependencyResolver();
        resolver.InitializeSplat();

        using (resolver.WithResolver())
        {
            var builder = AppLocator.CurrentMutable.CreateReactiveUIBuilder();
            _ = builder.WithWpf().BuildApp();

            var converterService = builder.ConverterService;

            // WPF-specific converters
            await Assert.That(converterService.TypedConverters.TryGetConverter(typeof(bool), typeof(Visibility))).IsNotNull();
            await Assert.That(converterService.TypedConverters.TryGetConverter(typeof(Visibility), typeof(bool))).IsNotNull();

            // Standard converters from WithCoreServices
            await Assert.That(converterService.TypedConverters.TryGetConverter(typeof(int), typeof(string))).IsNotNull();
            await Assert.That(converterService.TypedConverters.TryGetConverter(typeof(string), typeof(int))).IsNotNull();
            await Assert.That(converterService.TypedConverters.TryGetConverter(typeof(bool), typeof(string))).IsNotNull();
        }
    }

    /// <summary>Tests that after WithWpf, the BooleanToVisibilityTypeConverter correctly converts values.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task WithWpf_BoolToVisibilityConverter_ConvertsCorrectly()
    {
        AppBuilder.ResetBuilderStateForTests();
        var resolver = new ModernDependencyResolver();
        resolver.InitializeSplat();

        using (resolver.WithResolver())
        {
            var builder = AppLocator.CurrentMutable.CreateReactiveUIBuilder();
            _ = builder.WithWpf().BuildApp();

            var converter = builder.ConverterService.TypedConverters.TryGetConverter(typeof(bool), typeof(Visibility));
            await Assert.That(converter).IsNotNull();

            var success = converter!.TryConvertTyped(true, null, out var result);
            await Assert.That(success).IsTrue();
            await Assert.That(result).IsEqualTo(Visibility.Visible);
        }
    }

    /// <summary>Counts one run of scheduled work.</summary>
    /// <param name="runs">The run counter.</param>
    /// <returns>A disposable with nothing to release.</returns>
    private static EmptyDisposable CountRun(StrongBox<int> runs)
    {
        runs.Value++;
        return EmptyDisposable.Instance;
    }
}
