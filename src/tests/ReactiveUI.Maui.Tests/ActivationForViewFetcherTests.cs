// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Maui.Tests;

/// <summary>
/// Tests for ActivationForViewFetcher.
/// </summary>
public class ActivationForViewFetcherTests
{
    /// <summary>
    /// Tests that GetAffinityForView returns correct affinity for Page types.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task GetAffinityForView_PageType_ReturnsCorrectAffinity()
    {
        var fetcher = new ActivationForViewFetcher();
        var affinity = fetcher.GetAffinityForView(typeof(ContentPage));

        await Assert.That(affinity).IsEqualTo(10);
    }

    /// <summary>
    /// Tests that GetAffinityForView returns correct affinity for View types.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task GetAffinityForView_ViewType_ReturnsCorrectAffinity()
    {
        var fetcher = new ActivationForViewFetcher();
        var affinity = fetcher.GetAffinityForView(typeof(ContentView));

        await Assert.That(affinity).IsEqualTo(10);
    }

    /// <summary>
    /// Tests that GetAffinityForView returns zero for non-MAUI types.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task GetAffinityForView_NonMauiType_ReturnsZero()
    {
        var fetcher = new ActivationForViewFetcher();
        var affinity = fetcher.GetAffinityForView(typeof(string));

        await Assert.That(affinity).IsEqualTo(0);
    }

    /// <summary>
    /// Tests that GetActivationForView returns observable for ICanActivate views.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task GetActivationForView_ICanActivateView_ReturnsObservable()
    {
        var fetcher = new ActivationForViewFetcher();
        var view = new TestActivatableView();

        var activation = fetcher.GetActivationForView(view);

        await Assert.That(activation).IsNotNull();
    }

    /// <summary>
    /// Test view that implements ICanActivate.
    /// </summary>
    private class TestActivatableView : IViewFor, IActivatableView, ICanActivate
    {
        /// <inheritdoc/>
        public IObservable<Unit> Activated { get; } = Observable.Never<Unit>();

        /// <inheritdoc/>
        public IObservable<Unit> Deactivated { get; } = Observable.Never<Unit>();

        /// <inheritdoc/>
        public object? ViewModel { get; set; }
    }
}
