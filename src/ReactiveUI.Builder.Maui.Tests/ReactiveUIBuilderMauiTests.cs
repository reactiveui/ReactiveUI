// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using Microsoft.Maui.Dispatching;
using Splat;
using Splat.Builder;

namespace ReactiveUI.Builder.Maui.Tests;

[TestFixture]
public class ReactiveUIBuilderMauiTests
{
    [Test]
    public void WithMaui_Should_Register_Services()
    {
        AppBuilder.ResetBuilderStateForTests();
        using var locator = new ModernDependencyResolver();

        locator.CreateReactiveUIBuilder()
               .WithMaui()
               .Build();

        var observableProperty = locator.GetService<ICreatesObservableForProperty>();
        Assert.That(observableProperty, Is.Not.Null);

        var typeConverters = locator.GetServices<IBindingTypeConverter>();
        Assert.That(typeConverters, Is.Not.Null);
    }

    [Test]
    public void WithMauiScheduler_Should_Use_Custom_Dispatcher_When_Provided()
    {
        AppBuilder.ResetBuilderStateForTests();
        using var locator = new ModernDependencyResolver();
        var dispatcher = new TestDispatcher();
        var builder = locator.CreateReactiveUIBuilder();

        builder.WithMauiScheduler(dispatcher);

        Assert.That(builder.MainThreadScheduler, Is.Not.Null);

        var executed = false;
        builder.MainThreadScheduler!.Schedule(0, (_, _) =>
        {
            executed = true;
            return Disposable.Empty;
        });

        using (Assert.EnterMultipleScope())
        {
            Assert.That(dispatcher.DispatchCallCount, Is.GreaterThan(0));
            Assert.That(executed, Is.True);
        }
    }

    [Test]
    public void WithMauiScheduler_Should_Use_CurrentThread_When_In_Unit_Test_Runner()
    {
        AppBuilder.ResetBuilderStateForTests();
        using var locator = new ModernDependencyResolver();
        var builder = locator.CreateReactiveUIBuilder();

        using (ForceUnitTestMode())
        {
            builder.WithMauiScheduler();
            Assert.That(builder.MainThreadScheduler, Is.EqualTo(CurrentThreadScheduler.Instance));
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

#pragma warning disable CA1822 // Mark members as static
        public bool? InDesignMode() => false;
#pragma warning restore CA1822 // Mark members as static
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

    private sealed class TestDispatcherTimer(ReactiveUIBuilderMauiTests.TestDispatcher dispatcher) : IDispatcherTimer
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
