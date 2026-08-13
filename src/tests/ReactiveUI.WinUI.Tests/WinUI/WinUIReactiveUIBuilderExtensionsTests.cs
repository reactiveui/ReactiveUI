// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Microsoft.UI.Xaml;
using Splat;
using Splat.Builder;
using TUnit.Core.Executors;

namespace ReactiveUI.Tests.WinUI;

/// <summary>Tests for <see cref="WinUIReactiveUIBuilderExtensions"/>.</summary>
[NotInParallel]
[TestExecutor<WinUIThreadExecutor>]
public class WinUIReactiveUIBuilderExtensionsTests
{
    /// <summary>Verifies the WinUI main thread scheduler marshals onto the dispatcher queue.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task WinUIMainThreadScheduler_MarshalsOntoTheDispatcherQueue() =>
        _ = await Assert.That(WinUIReactiveUIBuilderExtensions.WinUIMainThreadScheduler).IsTypeOf<DispatcherQueueSequencer>();

    /// <summary>Verifies a missing builder is rejected.</summary>
    [Test]
    public void WithWinUI_WithNoBuilder_Throws() =>
        _ = Assert.Throws<ArgumentNullException>(static () => WinUIReactiveUIBuilderExtensions.WithWinUI(null!));

    /// <summary>Verifies a missing builder is rejected by the scheduler configuration.</summary>
    [Test]
    public void WithWinUIScheduler_WithNoBuilder_Throws() =>
        _ = Assert.Throws<ArgumentNullException>(static () => WinUIReactiveUIBuilderExtensions.WithWinUIScheduler(null!));

    /// <summary>Verifies a missing builder is rejected by the converter registration.</summary>
    [Test]
    public void WithWinUIConverters_WithNoBuilder_Throws() =>
        _ = Assert.Throws<ArgumentNullException>(static () => WinUIReactiveUIBuilderExtensions.WithWinUIConverters(null!));

    /// <summary>Verifies configuring WinUI keeps the fluent chain on the same builder.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task WithWinUI_ReturnsTheSameBuilder()
    {
        AppBuilder.ResetBuilderStateForTests();
        var resolver = new ModernDependencyResolver();
        resolver.InitializeSplat();

        using (resolver.WithResolver())
        {
            var builder = resolver.CreateReactiveUIBuilder();

            await Assert.That(builder.WithWinUI()).IsSameReferenceAs(builder);
        }
    }

    /// <summary>Verifies configuring the WinUI scheduler installs the dispatcher queue sequencer.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task WithWinUIScheduler_InstallsTheDispatcherQueueSequencer()
    {
        AppBuilder.ResetBuilderStateForTests();
        var resolver = new ModernDependencyResolver();
        resolver.InitializeSplat();

        using (resolver.WithResolver())
        {
            var builder = (ReactiveUIBuilder)resolver.CreateReactiveUIBuilder();

            _ = builder.WithWinUIScheduler();

            _ = await Assert.That(builder.MainThreadScheduler!).IsTypeOf<DispatcherQueueSequencer>();
        }
    }

    /// <summary>Verifies a binding between a boolean and a visibility can be converted in both directions.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task WithWinUIConverters_MakesTheVisibilityConversionsResolvable()
    {
        AppBuilder.ResetBuilderStateForTests();
        var resolver = new ModernDependencyResolver();
        resolver.InitializeSplat();

        using (resolver.WithResolver())
        {
            var builder = (ReactiveUIBuilder)resolver.CreateReactiveUIBuilder();

            _ = builder.WithWinUIConverters().BuildApp();

            using (Assert.Multiple())
            {
                _ = await Assert.That(builder.ConverterService.ResolveConverter(typeof(bool), typeof(Visibility)))
                    .IsTypeOf<BooleanToVisibilityTypeConverter>();
                _ = await Assert.That(builder.ConverterService.ResolveConverter(typeof(Visibility), typeof(bool)))
                    .IsTypeOf<VisibilityToBooleanTypeConverter>();
            }
        }
    }
}
