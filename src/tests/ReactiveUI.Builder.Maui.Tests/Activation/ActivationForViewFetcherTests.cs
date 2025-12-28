// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive.Disposables;
using ReactiveUI.Builder.Maui.Tests.Infrastructure;
using ReactiveUI.Maui;

using Splat.Builder;

using TUnit.Core.Executors;

namespace ReactiveUI.Builder.Maui.Tests.Activation;

/// <summary>
/// Tests for the activation for view fetcher.
/// </summary>
[STAThreadExecutor]
public sealed partial class ActivationForViewFetcherTests
{
    /// <summary>
    /// Verifies that a page and its child view activate and deactivate via the fetcher.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    /// <remarks>
    /// MAUI 9+ lifecycle events (Appearing/Disappearing, Loaded/Unloaded) require a real window manager
    /// and UI thread to fire properly. These events use custom storage mechanisms that cannot be triggered
    /// through reflection in a unit test environment.
    /// This test is skipped because proper testing requires a full MAUI integration test with actual
    /// window/page navigation, which belongs in a separate integration test project.
    /// The activation mechanism itself is tested in other platform-specific tests.
    /// </remarks>
    [Test]
    [Skip("MAUI lifecycle events require integration testing with real window/page navigation")]
    public async Task PageAndChildViewActivateAndDeactivate()
    {
        AppBuilder.ResetBuilderStateForTests();
        var resolver = new ModernDependencyResolver();
        resolver.InitializeSplat();
        resolver.InitializeReactiveUI();
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

            await Task.Delay(50);

            page.TriggerAppearing();
            child.TriggerLoaded();

            // Give the activation time to propagate
            await Task.Delay(50);

            using (Assert.Multiple())
            {
                await Assert.That(page.ActivationCount).IsEqualTo(1);
                await Assert.That(child.ActivationCount).IsEqualTo(1);
                await Assert.That(pageViewModel.ActivationCount).IsEqualTo(1);
                await Assert.That(childViewModel.ActivationCount).IsEqualTo(1);
            }

            child.TriggerUnloaded();
            page.TriggerDisappearing();

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
    private sealed class TestPage : ReactiveContentPage<TestActivatableViewModel>, IActivatableView
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TestPage"/> class.
        /// </summary>
        public TestPage() => this.WhenActivated(d =>
        {
            ActivationCount++;
            d(Disposable.Create(() => ActivationCount--));
        });

        /// <summary>
        /// Gets the current activation count.
        /// </summary>
        public int ActivationCount { get; private set; }

        /// <summary>
        /// Triggers the page appearing lifecycle.
        /// </summary>
        /// <remarks>
        /// Uses <see cref="MauiLifecycleHelpers"/> to trigger the Appearing event.
        /// </remarks>
        public void TriggerAppearing() => MauiLifecycleHelpers.TriggerAppearing(this);

        /// <summary>
        /// Triggers the page disappearing lifecycle.
        /// </summary>
        /// <remarks>
        /// Uses <see cref="MauiLifecycleHelpers"/> to trigger the Disappearing event.
        /// </remarks>
        public void TriggerDisappearing() => MauiLifecycleHelpers.TriggerDisappearing(this);
    }

    /// <summary>
    /// Test view that tracks activation count.
    /// </summary>
    private sealed class TestView : ReactiveContentView<TestActivatableViewModel>, IActivatableView
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TestView"/> class.
        /// </summary>
        public TestView() => this.WhenActivated(d =>
        {
            ActivationCount++;
            d(Disposable.Create(() => ActivationCount--));
        });

        /// <summary>
        /// Gets the current activation count.
        /// </summary>
        public int ActivationCount { get; private set; }

        /// <summary>
        /// Triggers the view loaded lifecycle.
        /// </summary>
        /// <remarks>
        /// Uses <see cref="MauiLifecycleHelpers"/> to trigger the Loaded event.
        /// </remarks>
        public void TriggerLoaded() => MauiLifecycleHelpers.TriggerLoaded(this);

        /// <summary>
        /// Triggers the view unloaded lifecycle.
        /// </summary>
        /// <remarks>
        /// Uses <see cref="MauiLifecycleHelpers"/> to trigger the Unloaded event.
        /// </remarks>
        public void TriggerUnloaded() => MauiLifecycleHelpers.TriggerUnloaded(this);
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
