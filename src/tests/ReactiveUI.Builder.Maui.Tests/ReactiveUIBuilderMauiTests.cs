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

    private static IDisposable ForceUnitTestMode()
    {
        var detector = new AlwaysTrueModeDetector();
        ModeDetector.OverrideModeDetector(detector);
        return Disposable.Create(static () => ModeDetector.OverrideModeDetector(new DefaultModeDetector()));
    }

    private sealed class AlwaysTrueModeDetector : IModeDetector
    {
        public bool? InUnitTestRunner() => true;
    }

    private sealed class TestDispatcher : IDispatcher
    {
        public int DispatchCallCount { get; private set; }

        public bool IsDispatchRequired => true;

        public bool Dispatch(Action action)
        {
            DispatchCallCount++;
            action();
            return true;
        }

        public bool DispatchDelayed(TimeSpan delay, Action action)
        {
            DispatchCallCount++;
            action();
            return true;
        }

        public IDispatcherTimer CreateTimer() => new TestDispatcherTimer(this);
    }

    private sealed class TestDispatcherTimer(TestDispatcher dispatcher) : IDispatcherTimer
    {
        public event EventHandler? Tick;

        public TimeSpan Interval { get; set; }

        public bool IsRunning { get; private set; }

        public bool IsRepeating { get; set; }

        public void Start()
        {
            IsRunning = true;
            dispatcher.Dispatch(() => Tick?.Invoke(this, EventArgs.Empty));

            if (!IsRepeating)
            {
                Stop();
            }
        }

        public void Stop() => IsRunning = false;
    }
}
