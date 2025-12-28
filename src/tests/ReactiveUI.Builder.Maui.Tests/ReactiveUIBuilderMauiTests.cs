// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive.Concurrency;
using System.Reactive.Disposables;

using Microsoft.Maui.Dispatching;

using Splat.Builder;

namespace ReactiveUI.Builder.Maui.Tests;

/// <summary>
/// Tests for ReactiveUI Builder MAUI extensions.
/// </summary>
public class ReactiveUIBuilderMauiTests
{
    /// <summary>
    /// Verifies that the WithMaui builder extension registers required MAUI services.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous test operation.</returns>
    [Test]
    public async Task WithMaui_Should_Register_Services()
    {
        AppBuilder.ResetBuilderStateForTests();
        using var locator = new ModernDependencyResolver();

        locator.CreateReactiveUIBuilder()
               .WithMaui()
               .Build();

        var observableProperty = locator.GetService<ICreatesObservableForProperty>();
        await Assert.That(observableProperty).IsNotNull();

        var typeConverters = locator.GetServices<IBindingTypeConverter>();
        await Assert.That(typeConverters).IsNotNull();
    }

    /// <summary>
    /// Verifies that WithMauiScheduler uses a custom dispatcher when one is provided.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous test operation.</returns>
    [Test]
    public async Task WithMauiScheduler_Should_Use_Custom_Dispatcher_When_Provided()
    {
        AppBuilder.ResetBuilderStateForTests();
        using var locator = new ModernDependencyResolver();
        var dispatcher = new TestDispatcher();
        var builder = locator.CreateReactiveUIBuilder();

        builder.WithMauiScheduler(dispatcher);

        await Assert.That(builder.MainThreadScheduler).IsNotNull();

        var executed = false;
        builder.MainThreadScheduler!.Schedule(0, (_, _) =>
        {
            executed = true;
            return Disposable.Empty;
        });

        using (Assert.Multiple())
        {
            await Assert.That(dispatcher.DispatchCallCount).IsGreaterThan(0);
            await Assert.That(executed).IsTrue();
        }
    }

    /// <summary>
    /// Verifies that WithMauiScheduler falls back to CurrentThreadScheduler when running in unit test mode.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous test operation.</returns>
    [Test]
    public async Task WithMauiScheduler_Should_Use_CurrentThread_When_In_Unit_Test_Runner()
    {
        AppBuilder.ResetBuilderStateForTests();
        using var locator = new ModernDependencyResolver();
        var builder = locator.CreateReactiveUIBuilder();

        using (ForceUnitTestMode())
        {
            builder.WithMauiScheduler();
            await Assert.That(builder.MainThreadScheduler).IsEqualTo(CurrentThreadScheduler.Instance);
        }
    }

    /// <summary>
    /// Temporarily overrides the mode detector to indicate the code is running in a unit test.
    /// </summary>
    /// <returns>A disposable that restores the default mode detector when disposed.</returns>
    private static IDisposable ForceUnitTestMode()
    {
        var detector = new AlwaysTrueModeDetector();
        ModeDetector.OverrideModeDetector(detector);
        return Disposable.Create(static () => ModeDetector.OverrideModeDetector(new DefaultModeDetector()));
    }

    /// <summary>
    /// Mode detector implementation that always reports being in a unit test runner.
    /// </summary>
    private sealed class AlwaysTrueModeDetector : IModeDetector
    {
        /// <summary>
        /// Indicates whether the code is running in a unit test runner.
        /// </summary>
        /// <returns>Always returns <see langword="true"/>.</returns>
        public bool? InUnitTestRunner() => true;
    }

    /// <summary>
    /// Test dispatcher implementation that tracks how many times dispatch methods are called.
    /// </summary>
    private sealed class TestDispatcher : IDispatcher
    {
        /// <summary>
        /// Gets the number of times <see cref="Dispatch"/> or <see cref="DispatchDelayed"/> was called.
        /// </summary>
        public int DispatchCallCount { get; private set; }

        /// <summary>
        /// Gets a value indicating whether dispatching is required.
        /// </summary>
        /// <remarks>
        /// Always returns <see langword="true"/> to force the scheduler to call <see cref="Dispatch"/>.
        /// </remarks>
        public bool IsDispatchRequired => true;

        /// <summary>
        /// Dispatches an action immediately and increments the call counter.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        /// <returns>Always returns <see langword="true"/>.</returns>
        public bool Dispatch(Action action)
        {
            DispatchCallCount++;
            action();
            return true;
        }

        /// <summary>
        /// Dispatches an action immediately, ignoring the delay, and increments the call counter.
        /// </summary>
        /// <param name="delay">The delay to ignore (executed immediately).</param>
        /// <param name="action">The action to execute.</param>
        /// <returns>Always returns <see langword="true"/>.</returns>
        public bool DispatchDelayed(TimeSpan delay, Action action)
        {
            DispatchCallCount++;
            action();
            return true;
        }

        /// <summary>
        /// Creates a test dispatcher timer.
        /// </summary>
        /// <returns>A new <see cref="TestDispatcherTimer"/> instance.</returns>
        public IDispatcherTimer CreateTimer() => new TestDispatcherTimer(this);
    }

    /// <summary>
    /// Test timer implementation that fires immediately when started.
    /// </summary>
    /// <param name="dispatcher">The dispatcher used to execute the timer callback.</param>
    private sealed class TestDispatcherTimer(TestDispatcher dispatcher) : IDispatcherTimer
    {
        /// <summary>
        /// Occurs when the timer interval has elapsed.
        /// </summary>
        public event EventHandler? Tick;

        /// <summary>
        /// Gets or sets the interval between timer ticks.
        /// </summary>
        /// <remarks>
        /// This value is ignored; the timer fires immediately when started.
        /// </remarks>
        public TimeSpan Interval { get; set; }

        /// <summary>
        /// Gets a value indicating whether the timer is currently running.
        /// </summary>
        public bool IsRunning { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether the timer should fire repeatedly.
        /// </summary>
        /// <remarks>
        /// If <see langword="false"/>, the timer stops automatically after firing once.
        /// </remarks>
        public bool IsRepeating { get; set; }

        /// <summary>
        /// Starts the timer and immediately fires the Tick event.
        /// </summary>
        /// <remarks>
        /// If <see cref="IsRepeating"/> is <see langword="false"/>, the timer stops after firing.
        /// </remarks>
        public void Start()
        {
            IsRunning = true;
            dispatcher.Dispatch(() => Tick?.Invoke(this, EventArgs.Empty));

            if (!IsRepeating)
            {
                Stop();
            }
        }

        /// <summary>
        /// Stops the timer.
        /// </summary>
        public void Stop() => IsRunning = false;
    }
}
