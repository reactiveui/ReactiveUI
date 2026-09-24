// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows;
using System.Windows.Threading;
using Splat;
using Splat.Builder;
using TUnit.Core.Executors;

namespace ReactiveUI.Tests.Wpf;

/// <summary>Tests for <see cref="WpfReactiveUIBuilderExtensions"/>.</summary>
[NotInParallel]
public class WpfReactiveUiBuilderExtensionsTest
{
    /// <summary>Upper bound on pumping the dispatcher while waiting for scheduled work.</summary>
    private static readonly TimeSpan ScheduleTimeout = TimeSpan.FromSeconds(10);

    /// <summary>Tests that WpfMainThreadScheduler is not null.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task WpfMainThreadScheduler_IsNotNull() =>
        await Assert.That(WpfReactiveUIBuilderExtensions.WpfMainThreadScheduler).IsNotNull();

    /// <summary>Tests that WpfMainThreadScheduler marshals through a dispatcher rather than running inline (#4456).</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task WpfMainThreadScheduler_IsDispatcherSequencer() =>
        await Assert.That(WpfReactiveUIBuilderExtensions.WpfMainThreadScheduler).IsTypeOf<DispatcherSequencer>();

    /// <summary>Tests that the main thread scheduler binds to the calling thread's dispatcher when no application exists.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<STAThreadExecutor>]
    public async Task CreateMainThreadScheduler_WithoutApplication_BindsCurrentDispatcher()
    {
        var scheduler = WpfReactiveUIBuilderExtensions.CreateMainThreadScheduler(null);

        await Assert.That(scheduler.Dispatcher).IsSameReferenceAs(Dispatcher.CurrentDispatcher);
    }

    /// <summary>Tests that the main thread scheduler binds to the application's dispatcher when an application exists.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<STAThreadExecutor>]
    public async Task CreateMainThreadScheduler_WithApplication_BindsApplicationDispatcher()
    {
        var application = Application.Current ?? new Application();

        var scheduler = WpfReactiveUIBuilderExtensions.CreateMainThreadScheduler(application);

        await Assert.That(scheduler.Dispatcher).IsSameReferenceAs(application.Dispatcher);
    }

    /// <summary>Tests that work scheduled from a background thread runs on the dispatcher thread (#4456).</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<STAThreadExecutor>]
    public async Task CreateMainThreadScheduler_ScheduledFromBackgroundThread_RunsOnDispatcherThread()
    {
        var scheduler = WpfReactiveUIBuilderExtensions.CreateMainThreadScheduler(null);
        var dispatcherThreadId = Environment.CurrentManagedThreadId;
        ScheduleProbe probe = new();

        // Give up after a bounded wait so a scheduling failure fails the test instead of hanging the pump.
        DispatcherTimer timeout = new(ScheduleTimeout, DispatcherPriority.Normal, (_, _) => probe.Frame.Continue = false, Dispatcher.CurrentDispatcher);

        _ = Task.Run(() =>
        {
            probe.SchedulingThreadId = Environment.CurrentManagedThreadId;
            _ = scheduler.Schedule(probe, static (_, p) => p.Record());
        });

        // Pump this thread's dispatcher until the scheduled work runs.
        Dispatcher.PushFrame(probe.Frame);
        timeout.Stop();

        using (Assert.Multiple())
        {
            await Assert.That(probe.SchedulingThreadId).IsNotEqualTo(dispatcherThreadId);
            await Assert.That(probe.ExecutedThreadId).IsEqualTo(dispatcherThreadId);
        }
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

    /// <summary>Tests that WithWpfScheduler registers the WPF dispatcher sequencer.</summary>
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
            await Assert.That(builder.MainThreadScheduler!).IsTypeOf<DispatcherSequencer>();
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

            // Verify ComponentModelFallbackConverter is registered as a fallback converter
            var fallbackConverters = builder.ConverterService.FallbackConverters.GetAllConverters().ToList();
            await Assert.That(fallbackConverters).IsNotEmpty();
            await Assert.That(fallbackConverters.OfType<ComponentModelFallbackConverter>().Any()).IsTrue();
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

            // Fallback converter
            var fallbackConverters = converterService.FallbackConverters.GetAllConverters().ToList();
            await Assert.That(fallbackConverters).IsNotEmpty();
            await Assert.That(fallbackConverters.OfType<ComponentModelFallbackConverter>().Any()).IsTrue();
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

    /// <summary>Records which threads scheduled and ran a work item, and releases the pumping frame once it runs.</summary>
    private sealed class ScheduleProbe : IDisposable
    {
        /// <summary>Gets the frame pumped until the scheduled work runs.</summary>
        public DispatcherFrame Frame { get; } = new();

        /// <summary>Gets or sets the managed thread id that scheduled the work.</summary>
        public int SchedulingThreadId { get; set; }

        /// <summary>Gets the managed thread id the scheduled work ran on.</summary>
        public int ExecutedThreadId { get; private set; }

        /// <summary>Records the executing thread and stops the pump.</summary>
        /// <returns>This probe, as the work item's disposable.</returns>
        public ScheduleProbe Record()
        {
            ExecutedThreadId = Environment.CurrentManagedThreadId;
            Frame.Continue = false;
            return this;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            // Nothing to release; the probe only records thread ids.
        }
    }
}
