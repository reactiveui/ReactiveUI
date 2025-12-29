// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows;
using System.Windows.Controls;

using TUnit.Core.Executors;

namespace ReactiveUI.Tests.Wpf;

/// <summary>
/// Tests for <see cref="ActivationForViewFetcher"/>.
/// </summary>
[NotInParallel]
public class ActivationForViewFetcherTest
{
    /// <summary>
    /// Tests that GetAffinityForView returns 10 for FrameworkElement types.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<STAThreadExecutor>]
    public async Task GetAffinityForView_FrameworkElementType_Returns10()
    {
        var fetcher = new ActivationForViewFetcher();

        var affinity = fetcher.GetAffinityForView(typeof(FrameworkElement));

        await Assert.That(affinity).IsEqualTo(10);
    }

    /// <summary>
    /// Tests that GetAffinityForView returns 10 for derived FrameworkElement types.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<STAThreadExecutor>]
    public async Task GetAffinityForView_DerivedFrameworkElementType_Returns10()
    {
        var fetcher = new ActivationForViewFetcher();

        var affinity = fetcher.GetAffinityForView(typeof(Button));

        await Assert.That(affinity).IsEqualTo(10);
    }

    /// <summary>
    /// Tests that GetAffinityForView returns 10 for Window types.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<STAThreadExecutor>]
    public async Task GetAffinityForView_WindowType_Returns10()
    {
        var fetcher = new ActivationForViewFetcher();

        var affinity = fetcher.GetAffinityForView(typeof(Window));

        await Assert.That(affinity).IsEqualTo(10);
    }

    /// <summary>
    /// Tests that GetAffinityForView returns 0 for non-FrameworkElement types.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<STAThreadExecutor>]
    public async Task GetAffinityForView_NonFrameworkElementType_Returns0()
    {
        var fetcher = new ActivationForViewFetcher();

        var affinity = fetcher.GetAffinityForView(typeof(string));

        await Assert.That(affinity).IsEqualTo(0);
    }

    /// <summary>
    /// Tests that GetActivationForView returns empty for non-FrameworkElement view.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<STAThreadExecutor>]
    public async Task GetActivationForView_NonFrameworkElementView_ReturnsEmpty()
    {
        var fetcher = new ActivationForViewFetcher();
        var view = new TestNonFrameworkElementView();

        var activation = fetcher.GetActivationForView(view);

        await Assert.That(activation).IsNotNull();
    }

    /// <summary>
    /// Test non-FrameworkElement view for testing.
    /// </summary>
    private class TestNonFrameworkElementView : IActivatableView
    {
    }
}
