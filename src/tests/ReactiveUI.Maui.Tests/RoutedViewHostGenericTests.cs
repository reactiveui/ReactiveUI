// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive.Concurrency;
using Microsoft.Maui.Controls;
using ReactiveUI.Builder;
using ReactiveUI.Tests.Utilities.AppBuilder;
using TUnit.Core.Interfaces;

namespace ReactiveUI.Maui.Tests;

/// <summary>
/// Tests for the generic <see cref="RoutedViewHost{TViewModel}"/>.
/// </summary>
[NotInParallel]
[TestExecutor<MauiTestExecutor>]
public class RoutedViewHostGenericTests
{
    /// <summary>
    /// Tests that RouterProperty is registered for the generic type.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task RouterProperty_IsRegistered()
    {
        await Assert.That(RoutedViewHost<TestRoutableViewModel>.RouterProperty).IsNotNull();
    }

    /// <summary>
    /// Tests that SetTitleOnNavigateProperty is registered for the generic type.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task SetTitleOnNavigateProperty_IsRegistered()
    {
        await Assert.That(RoutedViewHost<TestRoutableViewModel>.SetTitleOnNavigateProperty).IsNotNull();
    }

    /// <summary>
    /// Tests that the generic RoutedViewHost type can be referenced and used in type constraints.
    /// Instance creation requires IScreen registration and MAUI infrastructure.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task GenericType_CanBeReferenced()
    {
        // Verify the generic type is properly defined
        var type = typeof(RoutedViewHost<TestRoutableViewModel>);

        await Assert.That(type).IsNotNull();
        await Assert.That(type.IsGenericType).IsTrue();
    }

    /// <summary>
    /// Tests that Router property can be set and retrieved.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<MauiGenericRoutedViewHostTestExecutor>]
    public async Task Router_SetAndGet_WorksCorrectly()
    {
        var router = new RoutingState(ImmediateScheduler.Instance);
        var host = new RoutedViewHost<TestRoutableViewModel> { Router = router };

        await Assert.That(host.Router).IsEqualTo(router);
    }

    /// <summary>
    /// Tests that SetTitleOnNavigate property can be set and retrieved.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<MauiGenericRoutedViewHostTestExecutor>]
    public async Task SetTitleOnNavigate_SetAndGet_WorksCorrectly()
    {
        var host = new RoutedViewHost<TestRoutableViewModel> { SetTitleOnNavigate = true };

        await Assert.That(host.SetTitleOnNavigate).IsTrue();

        host.SetTitleOnNavigate = false;

        await Assert.That(host.SetTitleOnNavigate).IsFalse();
    }

    /// <summary>
    /// Tests that PagesForViewModel returns empty observable for null view model.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<MauiGenericRoutedViewHostTestExecutor>]
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
    [TestExecutor<MauiUnregisteredViewTestExecutor>]
    public async Task PagesForViewModel_ViewNotFound_Throws()
    {
        var host = new TestableRoutedViewHostUnregistered();

        await Assert.That(async () => await host.PublicPagesForViewModel(new UnregisteredViewModel()).ToList())
            .Throws<Exception>();
    }

    /// <summary>
    /// Tests that PagesForViewModel returns page with view model set.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<MauiGenericRoutedViewHostTestExecutor>]
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
    [TestExecutor<MauiGenericRoutedViewHostTestExecutor>]
    public async Task PagesForViewModel_SetTitleOnNavigateTrue_SetsPageTitle()
    {
        var host = new TestableRoutedViewHost { SetTitleOnNavigate = true };
        var viewModel = new TestRoutableViewModel { UrlPathSegment = "TestTitle" };
        var pages = await host.PublicPagesForViewModel(viewModel).ToList();

        await Assert.That(pages[0].Title).IsEqualTo("TestTitle");
    }

    /// <summary>
    /// Tests that PagesForViewModel does not set page title when SetTitleOnNavigate is false.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<MauiGenericRoutedViewHostTestExecutor>]
    public async Task PagesForViewModel_SetTitleOnNavigateFalse_DoesNotSetPageTitle()
    {
        var host = new TestableRoutedViewHost { SetTitleOnNavigate = false };
        var viewModel = new TestRoutableViewModel { UrlPathSegment = "TestTitle" };
        var pages = await host.PublicPagesForViewModel(viewModel).ToList();

        await Assert.That(pages[0].Title).IsNotEqualTo("TestTitle");
    }

    /// <summary>
    /// Tests that PageForViewModel throws for null view model.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<MauiGenericRoutedViewHostTestExecutor>]
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
    [TestExecutor<MauiUnregisteredViewTestExecutor>]
    public async Task PageForViewModel_ViewNotFound_Throws()
    {
        var host = new TestableRoutedViewHostUnregistered();

        await Assert.That(() => host.PublicPageForViewModel(new UnregisteredViewModel()))
            .Throws<Exception>();
    }

    /// <summary>
    /// Tests that PageForViewModel returns page with view model set.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<MauiGenericRoutedViewHostTestExecutor>]
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
    [TestExecutor<MauiGenericRoutedViewHostTestExecutor>]
    public async Task PageForViewModel_SetTitleOnNavigateTrue_SetsPageTitle()
    {
        var host = new TestableRoutedViewHost { SetTitleOnNavigate = true };
        var viewModel = new TestRoutableViewModel { UrlPathSegment = "TestTitle" };

        // Allow scheduler to process the title update
        await Task.Delay(100);

        var page = host.PublicPageForViewModel(viewModel);

        // The title is set via scheduler, so we need to wait
        await Task.Delay(100);

        await Assert.That(page.Title).IsEqualTo("TestTitle");
    }

    /// <summary>
    /// Test executor that sets up MAUI environment with view registration.
    /// </summary>
    [NotInParallel]
    public sealed class MauiGenericRoutedViewHostTestExecutor : MauiTestExecutor
    {
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
    /// Test executor that sets up MAUI environment without view registration for testing error cases.
    /// </summary>
    [NotInParallel]
    public sealed class MauiUnregisteredViewTestExecutor : MauiTestExecutor
    {
        private readonly AppBuilderTestHelper _helper = new();

        /// <inheritdoc/>
        protected override void Initialize()
        {
            base.Initialize();

            _helper.Initialize(builder =>
            {
                builder
                    .WithMaui()
                    .WithCoreServices();

                // Register IScreen for constructor, but don't register the view
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
    private sealed class TestableRoutedViewHost : RoutedViewHost<TestRoutableViewModel>
    {
        public IObservable<Page> PublicPagesForViewModel(IRoutableViewModel? vm) =>
            PagesForViewModel(vm);

        public Page PublicPageForViewModel(IRoutableViewModel vm) =>
            PageForViewModel(vm);

        public void PublicInvalidateCurrentViewModel() =>
            InvalidateCurrentViewModel();

        public Task PublicSyncNavigationStacksAsync() =>
            SyncNavigationStacksAsync();
    }

    /// <summary>
    /// Testable RoutedViewHost with unregistered view model for error testing.
    /// </summary>
    private sealed class TestableRoutedViewHostUnregistered : RoutedViewHost<UnregisteredViewModel>
    {
        public IObservable<Page> PublicPagesForViewModel(IRoutableViewModel? vm) =>
            PagesForViewModel(vm);

        public Page PublicPageForViewModel(IRoutableViewModel vm) =>
            PageForViewModel(vm);
    }

    /// <summary>
    /// Test routable view model.
    /// </summary>
    private sealed class TestRoutableViewModel : ReactiveObject, IRoutableViewModel
    {
        public string? UrlPathSegment { get; set; } = "test";

        public IScreen HostScreen { get; } = null!;
    }

    /// <summary>
    /// Test routable view.
    /// </summary>
    private sealed class TestRoutableView : ContentPage, IViewFor<TestRoutableViewModel>
    {
        public TestRoutableViewModel? ViewModel { get; set; }

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
        public string? UrlPathSegment { get; set; } = "unregistered";

        public IScreen HostScreen { get; } = null!;
    }

    /// <summary>
    /// Test screen implementation.
    /// </summary>
    private sealed class TestScreen : ReactiveObject, IScreen
    {
        public RoutingState Router { get; } = new(ImmediateScheduler.Instance);
    }
}
