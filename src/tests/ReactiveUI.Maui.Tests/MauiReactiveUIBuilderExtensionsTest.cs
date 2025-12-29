// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Microsoft.Maui.Dispatching;
using ReactiveUI.Builder;

namespace ReactiveUI.Tests.Maui;

/// <summary>
/// Tests for <see cref="MauiReactiveUIBuilderExtensions"/>.
/// </summary>
public class MauiReactiveUIBuilderExtensionsTest
{
    /// <summary>
    /// Tests that MauiMainThreadScheduler is not null.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task MauiMainThreadScheduler_IsNotNull()
    {
        await Assert.That(MauiReactiveUIBuilderExtensions.MauiMainThreadScheduler).IsNotNull();
    }

#if ANDROID
    /// <summary>
    /// Tests that AndroidMainThreadScheduler is not null.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task AndroidMainThreadScheduler_IsNotNull()
    {
        await Assert.That(MauiReactiveUIBuilderExtensions.AndroidMainThreadScheduler).IsNotNull();
    }
#endif

#if MACCATALYST || IOS || MACOS || TVOS
    /// <summary>
    /// Tests that AppleMainThreadScheduler is not null.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task AppleMainThreadScheduler_IsNotNull()
    {
        await Assert.That(MauiReactiveUIBuilderExtensions.AppleMainThreadScheduler).IsNotNull();
    }
#endif

#if WINUI_TARGET
    /// <summary>
    /// Tests that WinUIMauiMainThreadScheduler is not null.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task WinUIMauiMainThreadScheduler_IsNotNull()
    {
        await Assert.That(MauiReactiveUIBuilderExtensions.WinUIMauiMainThreadScheduler).IsNotNull();
    }
#endif

    /// <summary>
    /// Tests that UseReactiveUI with action does not throw.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task UseReactiveUI_WithAction_DoesNotThrow()
    {
        var builder = Microsoft.Maui.Hosting.MauiApp.CreateBuilder();

        var result = builder.UseReactiveUI(rxBuilder => { });

        await Assert.That(result).IsNotNull();
        await Assert.That(result).IsEqualTo(builder);
    }

    /// <summary>
    /// Tests that UseReactiveUI throws for null builder.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task UseReactiveUI_NullBuilder_Throws()
    {
        Microsoft.Maui.Hosting.MauiAppBuilder builder = null!;

        await Assert.That(() => builder.UseReactiveUI(rxBuilder => { }))
            .Throws<ArgumentNullException>();
    }

    /// <summary>
    /// Tests that WithMauiScheduler throws for null builder.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task WithMauiScheduler_NullBuilder_Throws()
    {
        IReactiveUIBuilder builder = null!;

        await Assert.That(() => builder.WithMauiScheduler())
            .Throws<ArgumentNullException>();
    }

    /// <summary>
    /// Tests that WithMaui throws for null builder.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task WithMaui_NullBuilder_Throws()
    {
        IReactiveUIBuilder builder = null!;

        await Assert.That(() => builder.WithMaui())
            .Throws<ArgumentNullException>();
    }
}
