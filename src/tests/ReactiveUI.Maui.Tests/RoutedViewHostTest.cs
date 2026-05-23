// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using System.Reactive.Concurrency;
using Microsoft.Maui.Controls;
using ReactiveUI.Builder;
using ReactiveUI.Tests.Utilities.AppBuilder;

namespace ReactiveUI.Maui.Tests;

/// <summary>
/// Tests for <see cref="RoutedViewHost"/>.
/// </summary>
public class RoutedViewHostTest
{
    /// <summary>
    /// The delay in milliseconds used to allow the scheduler to process title updates.
    /// </summary>
    private const int SchedulerProcessingDelayMs = 100;

    /// <summary>
    /// The title used for navigation title tests.
    /// </summary>
    private const string TestTitle = "TestTitle";

    /// <summary>
    /// Tests that RouterProperty is registered.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task RouterProperty_IsRegistered() => await Assert.That(RoutedViewHost.RouterProperty).IsNotNull();

    /// <summary>
    /// Tests that SetTitleOnNavigateProperty is registered.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task SetTitleOnNavigateProperty_IsRegistered() =>
        await Assert.That(RoutedViewHost.SetTitleOnNavigateProperty).IsNotNull();

    /// <summary>
    /// Tests that Router property can be set and retrieved.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<MauiRoutedViewHostTestExecutor>]
    public async Task Router_SetAndGet_WorksCorrectly()
    {
        var router = new RoutingState(ImmediateScheduler.Instance);
        var host = new RoutedViewHost { Router = router };

        await Assert.That(host.Router).IsEqualTo(router);
    }

    /// <summary>
    /// Tests that SetTitleOnNavigate property can be set and retrieved.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<MauiRoutedViewHostTestExecutor>]
    public async Task SetTitleOnNavigate_SetAndGet_WorksCorrectly()
    {
        var host = new RoutedViewHost { SetTitleOnNavigate = true };

        await Assert.That(host.SetTitleOnNavigate).IsTrue();

        host.SetTitleOnNavigate = false;

        await Assert.That(host.SetTitleOnNavigate).IsFalse();
    }

    /// <summary>
    /// Tests that PagesForViewModel returns empty observable for null view model.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<MauiRoutedViewHostTestExecutor>]
    public async Task PagesForViewModel_NullViewModel_ReturnsEmpty()
    {
        var host = new TestableRoutedViewHost();
        var pages = await host.PublicPagesForViewModel(null).ToList();

        await Assert.That(pages).IsEmpty();
    }

    /// <summary>
    /// Tests that PagesForViewModel throws when view is not found.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<MauiRoutedViewHostTestExecutor>]
    public async Task PagesForViewModel_ViewNotFound_Throws()
    {
        var host = new TestableRoutedViewHost();
        var viewModel = new UnregisteredViewModel();

        await Assert.That(async () => await host.PublicPagesForViewModel(viewModel).ToList())
            .Throws<Exception>();
    }

    /// <summary>
    /// Tests that PagesForViewModel returns page with view model set.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<MauiRoutedViewHostTestExecutor>]
    public async Task PagesForViewModel_ValidViewModel_ReturnsPageWithViewModel()
    {
        var host = new TestableRoutedViewHost();
        var viewModel = new TestRoutableViewModel();
        var pages = await host.PublicPagesForViewModel(viewModel).ToList();

        await Assert.That(pages).Count().IsEqualTo(1);
        await Assert.That(pages[0]).IsAssignableTo<TestRoutableView>();
        var view = (TestRoutableView)pages[0];
        await Assert.That(view.ViewModel).IsEqualTo(viewModel);
    }

    /// <summary>
    /// Tests that PagesForViewModel sets page title when SetTitleOnNavigate is true.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<MauiRoutedViewHostTestExecutor>]
    public async Task PagesForViewModel_SetTitleOnNavigateTrue_SetsPageTitle()
    {
        var host = new TestableRoutedViewHost { SetTitleOnNavigate = true };
        var viewModel = new TestRoutableViewModel { UrlPathSegment = TestTitle };
        var pages = await host.PublicPagesForViewModel(viewModel).ToList();

        await Assert.That(pages[0].Title).IsEqualTo(TestTitle);
    }

    /// <summary>
    /// Tests that PagesForViewModel does not set page title when SetTitleOnNavigate is false.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<MauiRoutedViewHostTestExecutor>]
    public async Task PagesForViewModel_SetTitleOnNavigateFalse_DoesNotSetPageTitle()
    {
        var host = new TestableRoutedViewHost { SetTitleOnNavigate = false };
        var viewModel = new TestRoutableViewModel { UrlPathSegment = TestTitle };
        var pages = await host.PublicPagesForViewModel(viewModel).ToList();

        await Assert.That(pages[0].Title).IsNotEqualTo(TestTitle);
    }

    /// <summary>
    /// Tests that PageForViewModel throws for null view model.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<MauiRoutedViewHostTestExecutor>]
    public async Task PageForViewModel_NullViewModel_Throws()
    {
        var host = new TestableRoutedViewHost();

        await Assert.That(() => host.PublicPageForViewModel(null!))
            .Throws<ArgumentNullException>();
    }

    /// <summary>
    /// Tests that PageForViewModel throws when view is not found.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<MauiRoutedViewHostTestExecutor>]
    public async Task PageForViewModel_ViewNotFound_Throws()
    {
        var host = new TestableRoutedViewHost();
        var viewModel = new UnregisteredViewModel();

        await Assert.That(() => host.PublicPageForViewModel(viewModel))
            .Throws<Exception>();
    }

    /// <summary>
    /// Tests that PageForViewModel returns page with view model set.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<MauiRoutedViewHostTestExecutor>]
    public async Task PageForViewModel_ValidViewModel_ReturnsPageWithViewModel()
    {
        var host = new TestableRoutedViewHost();
        var viewModel = new TestRoutableViewModel();
        var page = host.PublicPageForViewModel(viewModel);

        await Assert.That(page).IsAssignableTo<TestRoutableView>();
        var view = (TestRoutableView)page;
        await Assert.That(view.ViewModel).IsEqualTo(viewModel);
    }

    /// <summary>
    /// Tests that PageForViewModel sets page title when SetTitleOnNavigate is true.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<MauiRoutedViewHostTestExecutor>]
    public async Task PageForViewModel_SetTitleOnNavigateTrue_SetsPageTitle()
    {
        var host = new TestableRoutedViewHost { SetTitleOnNavigate = true };
        var viewModel = new TestRoutableViewModel { UrlPathSegment = TestTitle };

        // Allow scheduler to process the title update
        await Task.Delay(SchedulerProcessingDelayMs);

        var page = host.PublicPageForViewModel(viewModel);

        // The title is set via scheduler, so we need to wait
        await Task.Delay(SchedulerProcessingDelayMs);

        await Assert.That(page.Title).IsEqualTo(TestTitle);
    }

    /// <summary>
    /// Test executor that sets up MAUI environment with view registration.
    /// </summary>
    [NotInParallel]
    public sealed class MauiRoutedViewHostTestExecutor : MauiTestExecutor
    {
        /// <summary>
        /// The helper that configures and tears down the ReactiveUI app builder.
        /// </summary>
        private readonly AppBuilderTestHelper _helper = new();

        /// <inheritdoc/>
        protected override void Initialize()
        {
            base.Initialize();

            _helper.Initialize(builder =>
            {
                builder
                    .WithMaui()
                    .RegisterView<TestRoutableView, TestRoutableViewModel>()
                    .WithCoreServices();

                // Register IScreen for constructor
                AppLocator.CurrentMutable.Register<IScreen>(() => new TestScreen());
            });
        }

        /// <inheritdoc/>
        protected override void CleanUp()
        {
            _helper.CleanUp();
            base.CleanUp();
        }
    }

    /// <summary>
    /// Testable RoutedViewHost that exposes protected methods.
    /// </summary>
    [RequiresUnreferencedCode(
        "This class uses reflection to determine view model types at runtime through ViewLocator, which may be incompatible with trimming.")]
    [RequiresDynamicCode("ViewLocator.ResolveView uses reflection which is incompatible with AOT compilation.")]
    private sealed class TestableRoutedViewHost : RoutedViewHost
    {
        /// <summary>
        /// Exposes the protected PagesForViewModel method.
        /// </summary>
        /// <param name="vm">The view model.</param>
        /// <returns>An observable of pages.</returns>
        public IObservable<Page> PublicPagesForViewModel(IRoutableViewModel? vm) =>
            PagesForViewModel(vm);

        /// <summary>
        /// Exposes the protected PageForViewModel method.
        /// </summary>
        /// <param name="vm">The view model.</param>
        /// <returns>The page for the view model.</returns>
        public Page PublicPageForViewModel(IRoutableViewModel vm) =>
            PageForViewModel(vm);

        /// <summary>
        /// Exposes the protected InvalidateCurrentViewModel method.
        /// </summary>
        public void PublicInvalidateCurrentViewModel() =>
            InvalidateCurrentViewModel();

        /// <summary>
        /// Exposes the protected SyncNavigationStacksAsync method.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        public Task PublicSyncNavigationStacksAsync() =>
            SyncNavigationStacksAsync();
    }

    /// <summary>
    /// Test routable view model.
    /// </summary>
    private sealed class TestRoutableViewModel : ReactiveObject, IRoutableViewModel
    {
        /// <inheritdoc/>
        public string? UrlPathSegment { get; set; } = "test";

        /// <inheritdoc/>
        public IScreen HostScreen { get; } = null!;
    }

    /// <summary>
    /// Test routable view.
    /// </summary>
    private sealed class TestRoutableView : ContentPage, IViewFor<TestRoutableViewModel>
    {
        /// <inheritdoc/>
        public TestRoutableViewModel? ViewModel { get; set; }

        /// <inheritdoc/>
        object? IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = value as TestRoutableViewModel;
        }
    }

    /// <summary>
    /// Unregistered view model for testing error cases.
    /// </summary>
    private sealed class UnregisteredViewModel : ReactiveObject, IRoutableViewModel
    {
        /// <inheritdoc/>
        public string? UrlPathSegment { get; set; } = "unregistered";

        /// <inheritdoc/>
        public IScreen HostScreen { get; } = null!;
    }

    /// <summary>
    /// Test screen implementation.
    /// </summary>
    private sealed class TestScreen : ReactiveObject, IScreen
    {
        /// <inheritdoc/>
        public RoutingState Router { get; } = new(ImmediateScheduler.Instance);
    }
}
