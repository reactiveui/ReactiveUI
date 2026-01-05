// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Builder;
using Splat.Builder;
using TUnit.Core.Executors;

namespace ReactiveUI.Tests.Winforms;

/// <summary>
/// Tests for <see cref="WinFormsReactiveUIBuilderExtensions"/>.
/// </summary>
[NotInParallel]
public class WinFormsReactiveUIBuilderExtensionsTest
{
    /// <summary>
    /// Tests that WinFormsMainThreadScheduler is not null.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<STAThreadExecutor>]
    public async Task WinFormsMainThreadScheduler_IsNotNull()
    {
        await Assert.That(WinFormsReactiveUIBuilderExtensions.WinFormsMainThreadScheduler).IsNotNull();
    }

    /// <summary>
    /// Tests that WithWinForms throws when builder is null.
    /// </summary>
    [Test]
    [TestExecutor<STAThreadExecutor>]
    public void WithWinForms_ThrowsArgumentNullException_WhenBuilderIsNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            WinFormsReactiveUIBuilderExtensions.WithWinForms(null!));
    }

    /// <summary>
    /// Tests that WithWinForms configures builder correctly.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<STAThreadExecutor>]
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

    /// <summary>
    /// Tests that WithWinFormsScheduler throws when builder is null.
    /// </summary>
    [Test]
    [TestExecutor<STAThreadExecutor>]
    public void WithWinFormsScheduler_ThrowsArgumentNullException_WhenBuilderIsNull()
    {
        Assert.Throws<ArgumentNullException>(() =>
            WinFormsReactiveUIBuilderExtensions.WithWinFormsScheduler(null!));
    }

    /// <summary>
    /// Tests that WithWinFormsScheduler configures scheduler correctly.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<STAThreadExecutor>]
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
}
