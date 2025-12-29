// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Builder;
using Splat.Builder;

namespace ReactiveUI.Tests.Wpf;

/// <summary>
/// Tests for <see cref="WpfReactiveUIBuilderExtensions"/>.
/// </summary>
[NotInParallel]
public class WpfReactiveUIBuilderExtensionsTest
{
    /// <summary>
    /// Tests that WpfMainThreadScheduler is not null.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task WpfMainThreadScheduler_IsNotNull()
    {
        await Assert.That(WpfReactiveUIBuilderExtensions.WpfMainThreadScheduler).IsNotNull();
    }

    /// <summary>
    /// Tests that WithWpf throws when builder is null.
    /// </summary>
    [Test]
    public void WithWpf_ThrowsArgumentNullException_WhenBuilderIsNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            WpfReactiveUIBuilderExtensions.WithWpf(null!));
    }

    /// <summary>
    /// Tests that WithWpf configures builder correctly.
    /// </summary>
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

    /// <summary>
    /// Tests that WithWpfScheduler throws when builder is null.
    /// </summary>
    [Test]
    public void WithWpfScheduler_ThrowsArgumentNullException_WhenBuilderIsNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            WpfReactiveUIBuilderExtensions.WithWpfScheduler(null!));
    }

    /// <summary>
    /// Tests that WithWpfScheduler configures scheduler correctly.
    /// </summary>
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
}
