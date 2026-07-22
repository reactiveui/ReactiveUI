// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Runtime.CompilerServices;
using ReactiveUI.Builder;
using Splat;
using TUnit.Core.Executors;

namespace ReactiveUI.Maui.Tests;

/// <summary>Tests for <see cref="MauiReactiveUIBuilderExtensions"/>.</summary>
public class MauiReactiveUIBuilderExtensionsTest
{
    /// <summary>The runtime type name of the dispatcher-backed sequencer used by the MAUI main-thread scheduler.</summary>
    private const string MauiDispatcherSequencerTypeName = "MauiDispatcherSequencer";

    /// <summary>The scheduling delay, in milliseconds, used by the delayed-dispatch tests.</summary>
    private const int ScheduleDelayMilliseconds = 100;

    /// <summary>Tests that MauiMainThreadScheduler is not null.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task MauiMainThreadScheduler_IsNotNull() =>
        await Assert.That(MauiReactiveUIBuilderExtensions.MauiMainThreadScheduler).IsNotNull();

    /// <summary>Tests that MauiMainThreadScheduler resolves to a dispatcher-backed sequencer when a current-thread dispatcher is available.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<MauiTestExecutor>]
    public async Task MauiMainThreadScheduler_WithCurrentDispatcher_ResolvesDispatcherSequencer()
    {
        // The MauiTestExecutor installs a TestDispatcherProvider whose GetForCurrentThread() returns a dispatcher,
        // so the property takes the dispatcher.ToSequencer() branch instead of falling back to Sequencer.Default.
        var scheduler = MauiReactiveUIBuilderExtensions.MauiMainThreadScheduler;

        await Assert.That(scheduler.GetType().Name).IsEqualTo(MauiDispatcherSequencerTypeName);
    }

    /// <summary>Tests that WithMauiScheduler (no dispatcher) resolves the MAUI main-thread scheduler when not in a unit test runner.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<MauiTestExecutor>]
    public async Task WithMauiScheduler_NotInUnitTestRunner_UsesMauiMainThreadScheduler()
    {
        var resolver = new ModernDependencyResolver();
        var builder = new ReactiveUIBuilder(resolver, resolver);

        using (ForceNonUnitTestMode())
        {
            // With no dispatcher and outside a unit test runner, ResolveMainThreadScheduler returns the
            // MauiMainThreadScheduler (the TestDispatcherProvider supplies a current-thread dispatcher).
            _ = builder.WithMauiScheduler();

            await Assert.That(builder.MainThreadScheduler!.GetType().Name).IsEqualTo(MauiDispatcherSequencerTypeName);
        }
    }

    /// <summary>Tests that UseReactiveUI with action does not throw.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task UseReactiveUI_WithAction_DoesNotThrow()
    {
        var builder = Microsoft.Maui.Hosting.MauiApp.CreateBuilder();

        var result = builder.UseReactiveUI(static rxBuilder => { });

        await Assert.That(result).IsNotNull();
        await Assert.That(result).IsEqualTo(builder);
    }

    /// <summary>Tests that UseReactiveUI throws for null builder.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task UseReactiveUI_NullBuilder_Throws()
    {
        const MauiAppBuilder builder = null!;

        await Assert.That(static () => builder.UseReactiveUI(static rxBuilder => { }))
            .Throws<ArgumentNullException>();
    }

    /// <summary>Tests that WithMauiScheduler throws for null builder.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task WithMauiScheduler_NullBuilder_Throws()
    {
        const IReactiveUIBuilder builder = null!;

        await Assert.That(static () => builder.WithMauiScheduler())
            .Throws<ArgumentNullException>();
    }

    /// <summary>Tests that WithMaui throws for null builder.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task WithMaui_NullBuilder_Throws()
    {
        const IReactiveUIBuilder builder = null!;

        await Assert.That(static () => builder.WithMaui())
            .Throws<ArgumentNullException>();
    }

    /// <summary>Tests that UseReactiveUI with dispatcher does not throw.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task UseReactiveUI_WithDispatcher_DoesNotThrow()
    {
        var builder = Microsoft.Maui.Hosting.MauiApp.CreateBuilder();
        var dispatcher = new MockDispatcher();

        var result = builder.UseReactiveUI(dispatcher);

        await Assert.That(result).IsNotNull();
        await Assert.That(result).IsEqualTo(builder);
    }

    /// <summary>Tests that UseReactiveUI with dispatcher throws for null builder.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task UseReactiveUI_WithDispatcher_NullBuilder_Throws()
    {
        const MauiAppBuilder builder = null!;
        var dispatcher = new MockDispatcher();

        await Assert.That(() => builder.UseReactiveUI(dispatcher))
            .Throws<ArgumentNullException>();
    }

    /// <summary>Tests that WithMauiScheduler registers the correct scheduler.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task WithMauiScheduler_RegistersScheduler()
    {
        var resolver = new ModernDependencyResolver();
        var builder = new ReactiveUIBuilder(resolver, resolver);
        var dispatcher = new MockDispatcher();

        _ = builder.WithMauiScheduler(dispatcher);

        await Assert.That(builder.MainThreadScheduler).IsNotNull();
        await Assert.That(builder.MainThreadScheduler!.GetType().Name).IsEqualTo(MauiDispatcherSequencerTypeName);
    }

    /// <summary>Tests that MauiDispatcherSequencer schedules actions on the dispatcher.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task MauiDispatcherSequencer_Schedule_DispatchesAction()
    {
        var resolver = new ModernDependencyResolver();
        var builder = new ReactiveUIBuilder(resolver, resolver);
        var dispatcher = new MockDispatcher();
        _ = builder.WithMauiScheduler(dispatcher);
        var scheduler = builder.MainThreadScheduler!;

        var executed = new StrongBox<bool>();
        _ = scheduler.Schedule(executed, static state => state.Value = true);

        // MockDispatcher executes immediately if Dispatch is called
        await Assert.That(executed.Value).IsTrue();
        await Assert.That(dispatcher.DispatchCount).IsEqualTo(1);
    }

    /// <summary>Tests that MauiDispatcherSequencer schedules delayed actions through the dispatcher's native DispatchDelayed.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task MauiDispatcherSequencer_ScheduleWithDelay_UsesDispatchDelayed()
    {
        var resolver = new ModernDependencyResolver();
        var builder = new ReactiveUIBuilder(resolver, resolver);
        var dispatcher = new MockDispatcher();
        _ = builder.WithMauiScheduler(dispatcher);
        var scheduler = builder.MainThreadScheduler!;

        var executed = new StrongBox<bool>();
        _ = scheduler.Schedule(executed, TimeSpan.FromMilliseconds(ScheduleDelayMilliseconds), static state => state.Value = true);

        // Delays are routed through IDispatcher.DispatchDelayed (the dispatcher's native delayed dispatch),
        // not a created timer, and the forwarded delay is the remaining time until the due timestamp.
        await Assert.That(dispatcher.DelayedDispatches.Count).IsEqualTo(1);
        var (delay, callback) = dispatcher.DelayedDispatches[0];
        using (Assert.Multiple())
        {
            await Assert.That(delay).IsGreaterThan(TimeSpan.Zero);
            await Assert.That(delay).IsLessThanOrEqualTo(TimeSpan.FromMilliseconds(ScheduleDelayMilliseconds));
        }

        // Invoking the delayed callback runs the scheduled work.
        callback();
        await Assert.That(executed.Value).IsTrue();
    }

    /// <summary>Temporarily overrides the mode detector so the code believes it is not running in a unit test.</summary>
    /// <returns>A disposable that restores the default mode detector when disposed.</returns>
    private static ActionDisposable ForceNonUnitTestMode()
    {
        ModeDetector.OverrideModeDetector(new AlwaysFalseModeDetector());
        return new(static () => ModeDetector.OverrideModeDetector(new DefaultModeDetector()));
    }

    /// <summary>Mode detector implementation that always reports it is not running in a unit test runner.</summary>
    private sealed class AlwaysFalseModeDetector : IModeDetector
    {
        /// <summary>Indicates whether the code is running in a unit test runner.</summary>
        /// <returns>Always returns <see langword="false"/>.</returns>
        public bool? InUnitTestRunner() => false;
    }

    /// <summary>Mock dispatcher that records immediate and delayed dispatch calls for testing.</summary>
    private sealed class MockDispatcher : IDispatcher
    {
        /// <summary>Gets the number of times <see cref="Dispatch"/> has been called.</summary>
        public int DispatchCount { get; private set; }

        /// <summary>Gets the delayed dispatches recorded by <see cref="DispatchDelayed"/>.</summary>
        public List<(TimeSpan Delay, Action Action)> DelayedDispatches { get; } = [];

        /// <inheritdoc/>
        public bool IsDispatchRequired => true; // Force Dispatch call

        /// <inheritdoc/>
        public bool Dispatch(Action action)
        {
            DispatchCount++;
            action();
            return true;
        }

        /// <inheritdoc/>
        public bool DispatchDelayed(TimeSpan delay, Action action)
        {
            DelayedDispatches.Add((delay, action));
            return true;
        }

        /// <inheritdoc/>
        public IDispatcherTimer CreateTimer() =>
            throw new NotSupportedException("MauiDispatcherSequencer schedules delays via DispatchDelayed, not CreateTimer.");
    }
}
