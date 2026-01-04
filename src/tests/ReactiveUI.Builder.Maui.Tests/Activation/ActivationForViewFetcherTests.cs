// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Subjects;
using ReactiveUI.Maui;

using Splat.Builder;

using TUnit.Core.Executors;

namespace ReactiveUI.Builder.Maui.Tests.Activation;

/// <summary>
/// Tests for the activation for view fetcher.
/// </summary>
public sealed partial class ActivationForViewFetcherTests
{
    /// <summary>
    /// Verifies that a page and its child view activate and deactivate via the fetcher.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task PageAndChildViewActivateAndDeactivate()
    {
        AppBuilder.ResetBuilderStateForTests();
        var resolver = new ModernDependencyResolver();
        resolver.InitializeSplat();
        resolver.CreateReactiveUIBuilder()
            .WithPlatformServices()
            .BuildApp();
        resolver.RegisterConstant<IActivationForViewFetcher>(new ActivationForViewFetcher());

        using (resolver.WithResolver())
        {
            var page = new TestPage();
            var child = new TestView();
            var pageViewModel = new TestActivatableViewModel();
            var childViewModel = new TestActivatableViewModel();

            page.Content = child;
            page.ViewModel = pageViewModel;
            child.ViewModel = childViewModel;

            using (Assert.Multiple())
            {
                await Assert.That(page.ActivationCount).IsEqualTo(0);
                await Assert.That(child.ActivationCount).IsEqualTo(0);
            }

            // Manually activate the views
            page.Activate();
            child.Activate();

            // Give the activation time to propagate
            await Task.Delay(50);

            using (Assert.Multiple())
            {
                await Assert.That(page.ActivationCount).IsEqualTo(1);
                await Assert.That(child.ActivationCount).IsEqualTo(1);
                await Assert.That(pageViewModel.ActivationCount).IsEqualTo(1);
                await Assert.That(childViewModel.ActivationCount).IsEqualTo(1);
            }

            // Manually deactivate the views
            page.Deactivate();
            child.Deactivate();

            // Give the deactivation time to propagate
            await Task.Delay(50);

            using (Assert.Multiple())
            {
                await Assert.That(page.ActivationCount).IsEqualTo(0);
                await Assert.That(child.ActivationCount).IsEqualTo(0);
                await Assert.That(pageViewModel.ActivationCount).IsEqualTo(0);
                await Assert.That(childViewModel.ActivationCount).IsEqualTo(0);
            }
        }
    }

    /// <summary>
    /// Test page that tracks activation count.
    /// </summary>
    private sealed class TestPage : ReactiveContentPage<TestActivatableViewModel>, IActivatableView, ICanActivate
    {
        private readonly Subject<Unit> _activated = new();
        private readonly Subject<Unit> _deactivated = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="TestPage"/> class.
        /// </summary>
        public TestPage() => this.WhenActivated(d =>
        {
            ActivationCount++;
            d(Disposable.Create(() => ActivationCount--));
        });

        /// <inheritdoc/>
        public IObservable<Unit> Activated => _activated;

        /// <inheritdoc/>
        public IObservable<Unit> Deactivated => _deactivated;

        /// <summary>
        /// Gets the current activation count.
        /// </summary>
        public int ActivationCount { get; private set; }

        /// <summary>
        /// Manually trigger activation for testing.
        /// </summary>
        public void Activate() => _activated.OnNext(Unit.Default);

        /// <summary>
        /// Manually trigger deactivation for testing.
        /// </summary>
        public void Deactivate() => _deactivated.OnNext(Unit.Default);
    }

    /// <summary>
    /// Test view that tracks activation count.
    /// </summary>
    private sealed class TestView : ReactiveContentView<TestActivatableViewModel>, IActivatableView, ICanActivate
    {
        private readonly Subject<Unit> _activated = new();
        private readonly Subject<Unit> _deactivated = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="TestView"/> class.
        /// </summary>
        public TestView() => this.WhenActivated(d =>
        {
            ActivationCount++;
            d(Disposable.Create(() => ActivationCount--));
        });

        /// <inheritdoc/>
        public IObservable<Unit> Activated => _activated;

        /// <inheritdoc/>
        public IObservable<Unit> Deactivated => _deactivated;

        /// <summary>
        /// Gets the current activation count.
        /// </summary>
        public int ActivationCount { get; private set; }

        /// <summary>
        /// Manually trigger activation for testing.
        /// </summary>
        public void Activate() => _activated.OnNext(Unit.Default);

        /// <summary>
        /// Manually trigger deactivation for testing.
        /// </summary>
        public void Deactivate() => _deactivated.OnNext(Unit.Default);
    }

    /// <summary>
    /// Test view model that tracks activation count.
    /// </summary>
    private sealed class TestActivatableViewModel : ReactiveObject, IActivatableViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TestActivatableViewModel"/> class.
        /// </summary>
        public TestActivatableViewModel() => this.WhenActivated(d =>
        {
            ActivationCount++;
            d(Disposable.Create(() => ActivationCount--));
        });

        /// <summary>
        /// Gets the view model activator.
        /// </summary>
        public ViewModelActivator Activator { get; } = new();

        /// <summary>
        /// Gets the current activation count.
        /// </summary>
        public int ActivationCount { get; private set; }
    }
}
