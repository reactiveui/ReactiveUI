// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Splat;
using TUnit.Core.Executors;

namespace ReactiveUI.WinForms.Tests.Winforms;

/// <summary>Tests for <see cref="WinFormsReactiveUIBuilderExtensions"/>.</summary>
[NotInParallel]
[TestExecutor<WinFormsTestExecutor>]

public class WinFormsReactiveUIBuilderExtensionsTest
{
    /// <summary>Tests that WinFormsMainThreadScheduler is not null.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task WinFormsMainThreadScheduler_IsNotNull() =>
        await Assert.That(WinFormsReactiveUIBuilderExtensions.WinFormsMainThreadScheduler).IsNotNull();

    /// <summary>Tests that WithWinForms throws when builder is null.</summary>
    [Test]
    public void WithWinForms_ThrowsArgumentNullException_WhenBuilderIsNull() =>
        _ = Assert.Throws<ArgumentNullException>(static () =>
            WinFormsReactiveUIBuilderExtensions.WithWinForms(null!));

    /// <summary>Tests that WithWinForms configures builder correctly.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task WithWinForms_ConfiguresBuilder()
    {
        var resolver = new ModernDependencyResolver();
        resolver.InitializeSplat();

        using (resolver.WithResolver())
        {
            var builder = resolver.CreateReactiveUIBuilder();

            var result = builder.WithWinForms();

            await Assert.That(result).IsNotNull();
            await Assert.That(result).IsSameReferenceAs(builder);
        }
    }

    /// <summary>Tests that WithWinFormsScheduler throws when builder is null.</summary>
    [Test]
    public void WithWinFormsScheduler_ThrowsArgumentNullException_WhenBuilderIsNull() =>
        _ = Assert.Throws<ArgumentNullException>(static () =>
            WinFormsReactiveUIBuilderExtensions.WithWinFormsScheduler(null!));

    /// <summary>Tests that WithWinFormsScheduler configures scheduler correctly.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task WithWinFormsScheduler_ConfiguresScheduler()
    {
        var resolver = new ModernDependencyResolver();
        resolver.InitializeSplat();

        using (resolver.WithResolver())
        {
            var builder = resolver.CreateReactiveUIBuilder();

            var result = builder.WithWinFormsScheduler();

            await Assert.That(result).IsNotNull();
            await Assert.That(result).IsSameReferenceAs(builder);
        }
    }

    /// <summary>Tests that WithWinFormsScheduler registers the WinForms control sequencer.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task WithWinFormsScheduler_RegistersControlSequencer()
    {
        var resolver = new ModernDependencyResolver();
        resolver.InitializeSplat();

        using (resolver.WithResolver())
        {
            var builder = (ReactiveUIBuilder)resolver.CreateReactiveUIBuilder();

            _ = builder.WithWinFormsScheduler();

            await Assert.That(builder.MainThreadScheduler).IsNotNull();
            await Assert.That(builder.MainThreadScheduler!).IsTypeOf<ControlSequencer>();
        }
    }
}
