// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Maui;
using ReactiveUI.Maui.Tests;
using Splat;
using Splat.Builder;
using TUnit.Core.Executors;

namespace ReactiveUI.Builder.Maui.Tests;

/// <summary>Tests for ReactiveUI Builder MAUI extensions.</summary>
[TestExecutor<MauiTestExecutor>]
public class ReactiveUIBuilderMauiTests
{
    /// <summary>Verifies that the WithMaui builder extension registers required MAUI services.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous test operation.</returns>
    [Test]
    public async Task WithMaui_Should_Register_Services()
    {
        AppBuilder.ResetBuilderStateForTests();
        using var locator = new ModernDependencyResolver();
        _ = locator.CreateReactiveUIBuilder()
            .WithMaui()
            .BuildApp();

        await Assert.That(locator.GetService<IViewLocator>()).IsNotNull();
        await Assert.That(locator.GetService<IActivationForViewFetcher>()).IsNotNull();
    }

    /// <summary>Verifies that WithMaui registers the MAUI activation fetcher rather than only the core registrations.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous test operation.</returns>
    [Test]
    public async Task WithMaui_Should_Register_Maui_ActivationForViewFetcher()
    {
        AppBuilder.ResetBuilderStateForTests();
        using var locator = new ModernDependencyResolver();
        _ = locator.CreateReactiveUIBuilder()
            .WithMaui()
            .BuildApp();

        var fetchers = locator.GetServices<IActivationForViewFetcher>();

        await Assert.That(fetchers.OfType<ActivationForViewFetcher>().Count()).IsEqualTo(1);
    }

    /// <summary>Verifies that a page calling WhenActivated resolves activation after WithMaui without throwing.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous test operation.</returns>
    [Test]
    public async Task WithMaui_Should_Allow_WhenActivated_On_ReactiveContentPage()
    {
        AppBuilder.ResetBuilderStateForTests();
        using var locator = new ModernDependencyResolver();
        _ = locator.CreateReactiveUIBuilder()
            .WithMaui()
            .BuildApp();

        using (locator.WithResolver())
        {
            WhenActivatedPage page = new();

            await Assert.That(() => page.WhenActivated(static (MultipleDisposable _) => { }).Dispose()).ThrowsNothing();
        }
    }

    /// <summary>Verifies that WithMauiScheduler uses a custom dispatcher when one is provided.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous test operation.</returns>
    [Test]
    public async Task WithMauiScheduler_Should_Use_Custom_Dispatcher_When_Provided()
    {
        AppBuilder.ResetBuilderStateForTests();
        using var locator = new ModernDependencyResolver();
        var dispatcher = new TestDispatcher();
        var builder = locator.CreateReactiveUIBuilder();

        _ = builder.WithMauiScheduler(dispatcher);

        await Assert.That(builder.MainThreadScheduler).IsNotNull();

        var executed = false;
        _ = builder.MainThreadScheduler!.Schedule(0, (_, _) =>
        {
            executed = true;
            return EmptyDisposable.Instance;
        });

        await Assert.That(executed).IsTrue();
    }

    /// <summary>Verifies that WithMauiScheduler falls back to CurrentThreadScheduler when running in unit test mode.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous test operation.</returns>
    [Test]
    public async Task WithMauiScheduler_Should_Use_CurrentThread_When_In_Unit_Test_Runner()
    {
        AppBuilder.ResetBuilderStateForTests();
        using var locator = new ModernDependencyResolver();
        var builder = locator.CreateReactiveUIBuilder();

        using (ForceUnitTestMode())
        {
            _ = builder.WithMauiScheduler();
            await Assert.That(builder.MainThreadScheduler).IsEqualTo(Sequencer.CurrentThread);
        }
    }

    /// <summary>Temporarily overrides the mode detector to indicate the code is running in a unit test.</summary>
    /// <returns>A disposable that restores the default mode detector when disposed.</returns>
    private static ActionDisposable ForceUnitTestMode()
    {
        var detector = new AlwaysTrueModeDetector();
        ModeDetector.OverrideModeDetector(detector);
        return new(static () => ModeDetector.OverrideModeDetector(new DefaultModeDetector()));
    }

    /// <summary>View model for <see cref="WhenActivatedPage"/>.</summary>
    private sealed class WhenActivatedViewModel : ReactiveObject;

    /// <summary>Page derived from <see cref="ReactiveContentPage{TViewModel}"/>, mirroring the reproduction in #4468.</summary>
    private sealed class WhenActivatedPage : ReactiveContentPage<WhenActivatedViewModel>;

    /// <summary>Mode detector implementation that always reports being in a unit test runner.</summary>
    private sealed class AlwaysTrueModeDetector : IModeDetector
    {
        /// <summary>Indicates whether the code is running in a unit test runner.</summary>
        /// <returns>Always returns <see langword="true"/>.</returns>
        public bool? InUnitTestRunner() => true;
    }
}
