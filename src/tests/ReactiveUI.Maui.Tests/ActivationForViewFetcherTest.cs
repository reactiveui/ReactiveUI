// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using System.Reactive.Subjects;

namespace ReactiveUI.Maui.Tests;

/// <summary>
/// Tests for <see cref="ActivationForViewFetcher"/>.
/// </summary>
public class ActivationForViewFetcherTest
{
    private const int ExpectedMauiAffinity = 10;
    private const int ExpectedNonMauiAffinity = 0;
    private const int ActivationDelayMilliseconds = 50;

    /// <summary>
    /// Tests that GetAffinityForView returns 10 for Page types.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task GetAffinityForView_PageType_Returns10()
    {
        var fetcher = new ActivationForViewFetcher();

        var affinity = fetcher.GetAffinityForView(typeof(Page));

        await Assert.That(affinity).IsEqualTo(ExpectedMauiAffinity);
    }

    /// <summary>
    /// Tests that GetAffinityForView returns 10 for ContentPage types.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task GetAffinityForView_ContentPageType_Returns10()
    {
        var fetcher = new ActivationForViewFetcher();

        var affinity = fetcher.GetAffinityForView(typeof(ContentPage));

        await Assert.That(affinity).IsEqualTo(ExpectedMauiAffinity);
    }

    /// <summary>
    /// Tests that GetAffinityForView returns 10 for View types.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task GetAffinityForView_ViewType_Returns10()
    {
        var fetcher = new ActivationForViewFetcher();

        var affinity = fetcher.GetAffinityForView(typeof(View));

        await Assert.That(affinity).IsEqualTo(ExpectedMauiAffinity);
    }

    /// <summary>
    /// Tests that GetAffinityForView returns 10 for Label types.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task GetAffinityForView_LabelType_Returns10()
    {
        var fetcher = new ActivationForViewFetcher();

        var affinity = fetcher.GetAffinityForView(typeof(Label));

        await Assert.That(affinity).IsEqualTo(ExpectedMauiAffinity);
    }

    /// <summary>
    /// Tests that GetAffinityForView returns 10 for Cell types.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task GetAffinityForView_CellType_Returns10()
    {
        var fetcher = new ActivationForViewFetcher();

        var affinity = fetcher.GetAffinityForView(typeof(Cell));

        await Assert.That(affinity).IsEqualTo(ExpectedMauiAffinity);
    }

    /// <summary>
    /// Tests that GetAffinityForView returns 10 for ViewCell types.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task GetAffinityForView_ViewCellType_Returns10()
    {
        var fetcher = new ActivationForViewFetcher();

#pragma warning disable CS0618 // Type or member is obsolete
        var affinity = fetcher.GetAffinityForView(typeof(ViewCell));
#pragma warning restore CS0618 // Type or member is obsolete

        await Assert.That(affinity).IsEqualTo(ExpectedMauiAffinity);
    }

    /// <summary>
    /// Tests that GetAffinityForView returns 0 for non-MAUI types.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task GetAffinityForView_NonMauiType_Returns0()
    {
        var fetcher = new ActivationForViewFetcher();

        var affinity = fetcher.GetAffinityForView(typeof(string));

        await Assert.That(affinity).IsEqualTo(ExpectedNonMauiAffinity);
    }

    /// <summary>
    /// Tests that GetActivationForView returns observable for ICanActivate views.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task GetActivationForView_ICanActivateView_ReturnsObservable()
    {
        var fetcher = new ActivationForViewFetcher();
        var view = new TestCanActivateView();

        var activation = fetcher.GetActivationForView(view);

        await Assert.That(activation).IsNotNull();
    }

    /// <summary>
    /// Tests that GetActivationForView with ICanActivate emits activation changes.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task GetActivationForView_ICanActivate_EmitsActivationChanges()
    {
        var fetcher = new ActivationForViewFetcher();
        var view = new TestCanActivateView();
        var results = new List<bool>();

        fetcher.GetActivationForView(view).Subscribe(results.Add);

        view.ActivateSubject.OnNext(Unit.Default);
        await Task.Delay(ActivationDelayMilliseconds);

        await Assert.That(results).Contains(true);

        view.DeactivateSubject.OnNext(Unit.Default);
        await Task.Delay(ActivationDelayMilliseconds);

        await Assert.That(results).Contains(false);
    }

    /// <summary>
    /// Tests that GetActivationForView returns observable for non-activatable views.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task GetActivationForView_NonActivatableView_ReturnsObservable()
    {
        var fetcher = new ActivationForViewFetcher();
        var view = new TestNonActivatableView();

        var activation = fetcher.GetActivationForView(view);

        await Assert.That(activation).IsNotNull();
    }

    /// <summary>
    /// Test view that implements ICanActivate for testing.
    /// </summary>
    private sealed class TestCanActivateView : IActivatableView, ICanActivate
    {
        /// <summary>
        /// Gets the subject used to trigger activation.
        /// </summary>
        public Subject<Unit> ActivateSubject { get; } = new();

        /// <summary>
        /// Gets the subject used to trigger deactivation.
        /// </summary>
        public Subject<Unit> DeactivateSubject { get; } = new();

        /// <inheritdoc/>
        public IObservable<Unit> Activated => ActivateSubject;

        /// <inheritdoc/>
        public IObservable<Unit> Deactivated => DeactivateSubject;
    }

    /// <summary>
    /// Test non-activatable view for testing.
    /// </summary>
    [SuppressMessage("Minor Code Smell", "S2094:Classes should not be empty", Justification = "Marker type for tests.")]
    private sealed class TestNonActivatableView : IActivatableView;
}
