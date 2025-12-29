// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using System.Reactive.Concurrency;
using System.Reactive.Subjects;

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
    /// Tests that GetAffinityForView returns correct affinity for Cell types.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task GetAffinityForView_CellType_ReturnsCorrectAffinity()
    {
        var fetcher = new ActivationForViewFetcher();
#pragma warning disable CS0618 // Type or member is obsolete
        var affinity = fetcher.GetAffinityForView(typeof(TextCell));
#pragma warning restore CS0618 // Type or member is obsolete

        await Assert.That(affinity).IsEqualTo(10);
    }

    /// <summary>
    /// Tests that GetActivationForView works for Page views.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task GetActivationForView_PageView_ReturnsDistinctObservable()
    {
        var fetcher = new ActivationForViewFetcher();
        var page = new TestPage();

        var activation = fetcher.GetActivationForView(page);

        await Assert.That(activation).IsNotNull();
    }

    /// <summary>
    /// Tests that GetActivationForView works for View views.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task GetActivationForView_ContentView_ReturnsDistinctObservable()
    {
        var fetcher = new ActivationForViewFetcher();
        var view = new TestView();

        var activation = fetcher.GetActivationForView(view);

        await Assert.That(activation).IsNotNull();
    }

    /// <summary>
    /// Tests that GetActivationForView works for Cell views.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task GetActivationForView_CellView_ReturnsObservable()
    {
        var fetcher = new ActivationForViewFetcher();
        var cell = new TestCell();

        var activation = fetcher.GetActivationForView(cell);

        await Assert.That(activation).IsNotNull();
    }

    /// <summary>
    /// Tests that activation observable emits true/false for ICanActivate.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task GetActivationForView_ICanActivate_EmitsActivationStates()
    {
        var fetcher = new ActivationForViewFetcher();
        var activatedSubject = new Subject<Unit>();
        var deactivatedSubject = new Subject<Unit>();
        var view = new TestCanActivateView(activatedSubject, deactivatedSubject);

        var activation = fetcher.GetActivationForView(view);
        var values = new List<bool>();
        activation.ObserveOn(ImmediateScheduler.Instance).Subscribe(values.Add);

        activatedSubject.OnNext(Unit.Default);
        deactivatedSubject.OnNext(Unit.Default);
        activatedSubject.OnNext(Unit.Default);

        await Assert.That(values).Count().IsEqualTo(3);
        await Assert.That(values[0]).IsTrue();
        await Assert.That(values[1]).IsFalse();
        await Assert.That(values[2]).IsTrue();
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

    /// <summary>
    /// Test view that implements ICanActivate with controllable subjects.
    /// </summary>
    private class TestCanActivateView : IViewFor, IActivatableView, ICanActivate
    {
        public TestCanActivateView(IObservable<Unit> activated, IObservable<Unit> deactivated)
        {
            Activated = activated;
            Deactivated = deactivated;
        }

        /// <inheritdoc/>
        public IObservable<Unit> Activated { get; }

        /// <inheritdoc/>
        public IObservable<Unit> Deactivated { get; }

        /// <inheritdoc/>
        public object? ViewModel { get; set; }
    }

    /// <summary>
    /// Test page that implements IActivatableView.
    /// </summary>
    private class TestPage : ContentPage, IActivatableView
    {
    }

    /// <summary>
    /// Test view that implements IActivatableView.
    /// </summary>
    private class TestView : ContentView, IActivatableView
    {
    }

    /// <summary>
    /// Test cell that implements IActivatableView.
    /// </summary>
#pragma warning disable CS0618 // Type or member is obsolete
    private class TestCell : TextCell, IActivatableView
#pragma warning restore CS0618 // Type or member is obsolete
    {
    }
}
