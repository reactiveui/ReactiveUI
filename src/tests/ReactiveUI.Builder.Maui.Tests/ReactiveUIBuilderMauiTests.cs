// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive.Concurrency;
using System.Reactive.Disposables;

using Microsoft.Maui.Dispatching;

using ReactiveUI.Maui.Tests;

using Splat.Builder;

namespace ReactiveUI.Builder.Maui.Tests;

/// <summary>
/// Tests for ReactiveUI Builder MAUI extensions.
/// </summary>
[TestExecutor<MauiTestExecutor>]
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
               .BuildApp();

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

        await Assert.That(executed).IsTrue();
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
}
