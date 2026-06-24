// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using Microsoft.Maui.Controls;
using ReactiveUI.Builder;
using ReactiveUI.Primitives;
using ReactiveUI.Tests.Utilities.AppBuilder;
using Splat;
using TUnit.Core.Executors;

namespace ReactiveUI.Maui.Tests;

/// <summary>Tests for <see cref="RoutedViewHost"/>.</summary>
public class RoutedViewHostTest
{
    /// <summary>The delay in milliseconds used to allow the scheduler to process title updates.</summary>
    private const int SchedulerProcessingDelayMs = 100;

    /// <summary>The title used for navigation title tests.</summary>
    private const string TestTitle = "TestTitle";

    /// <summary>Tests that RouterProperty is registered.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task RouterProperty_IsRegistered() => await Assert.That(RoutedViewHost.RouterProperty).IsNotNull();

    /// <summary>Tests that SetTitleOnNavigateProperty is registered.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    public async Task SetTitleOnNavigateProperty_IsRegistered() =>
        await Assert.That(RoutedViewHost.SetTitleOnNavigateProperty).IsNotNull();

    /// <summary>Tests that Router property can be set and retrieved.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<MauiRoutedViewHostTestExecutor>]
    public async Task Router_SetAndGet_WorksCorrectly()
    {
        var router = new RoutingState(Sequencer.Immediate);
        var host = new RoutedViewHost { Router = router };

        await Assert.That(host.Router).IsEqualTo(router);
    }

    /// <summary>Tests that SetTitleOnNavigate property can be set and retrieved.</summary>
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

    /// <summary>Tests that PagesForViewModel returns empty observable for null view model.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<MauiRoutedViewHostTestExecutor>]
    public async Task PagesForViewModel_NullViewModel_ReturnsEmpty()
    {
        var host = new TestableRoutedViewHost();
        var pages = await host.PublicPagesForViewModel(null).ToList();

        await Assert.That(pages).IsEmpty();
    }

    /// <summary>Tests that PagesForViewModel throws when view is not found.</summary>
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

    /// <summary>Tests that PagesForViewModel returns page with view model set.</summary>
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

    /// <summary>Tests that PagesForViewModel sets page title when SetTitleOnNavigate is true.</summary>
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

    /// <summary>Tests that PagesForViewModel does not set page title when SetTitleOnNavigate is false.</summary>
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

    /// <summary>Tests that PageForViewModel throws for null view model.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<MauiRoutedViewHostTestExecutor>]
    public async Task PageForViewModel_NullViewModel_Throws()
    {
        var host = new TestableRoutedViewHost();

        await Assert.That(() => host.PublicPageForViewModel(null!))
            .Throws<ArgumentNullException>();
    }

    /// <summary>Tests that PageForViewModel throws when view is not found.</summary>
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

    /// <summary>Tests that PageForViewModel returns page with view model set.</summary>
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

    /// <summary>Tests that PageForViewModel sets page title when SetTitleOnNavigate is true.</summary>
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

    /// <summary>Invalidating the current view model is a safe no-op when there is no current view model or page.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<MauiRoutedViewHostTestExecutor>]
    public async Task InvalidateCurrentViewModel_NoCurrentViewModelOrPage_ReturnsEarly()
    {
        var host = new TestableRoutedViewHost();

        // The router has an empty navigation stack (no current view model) and there is no current page,
        // so the call must short-circuit without throwing.
        host.PublicInvalidateCurrentViewModel();

        await Assert.That(host.Router?.GetCurrentViewModel()).IsNull();
    }

    /// <summary>The constructor throws when no <see cref="IScreen"/> is registered.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<MauiRoutedViewHostNoScreenExecutor>]
    public async Task Constructor_NoScreenRegistered_Throws() =>
        await Assert.That(static () => new RoutedViewHost())
            .Throws<InvalidOperationException>();

    /// <summary>Invalidating with a matching current page view model type reassigns the current view model onto the page.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<MauiRoutedViewHostTestExecutor>]
    public async Task InvalidateCurrentViewModel_MatchingViewModelType_UpdatesViewModel()
    {
        var host = new TestableRoutedViewHost();

        // Make the current page an IViewFor whose view model is the same type as the router's current view model.
        var routerViewModel = new TestRoutableViewModel();
        host.Router.NavigationStack.Add(routerViewModel);

        var pageViewModel = new TestRoutableViewModel();
        var page = new TestRoutableView { ViewModel = pageViewModel };
        await host.PushAsync(page);

        host.PublicInvalidateCurrentViewModel();

        // The page's view model was replaced with the router's current view model (same type, so it is reassigned).
        await Assert.That(page.ViewModel).IsEqualTo(routerViewModel);
    }

    /// <summary>Invalidating with an incompatible current page view model type leaves the page view model untouched.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<MauiRoutedViewHostTestExecutor>]
    public async Task InvalidateCurrentViewModel_IncompatibleViewModelType_DoesNotUpdate()
    {
        var host = new TestableRoutedViewHost();

        // The router's current view model is a different type than the page's view model.
        host.Router.NavigationStack.Add(new SecondRoutableViewModel());

        var originalViewModel = new TestRoutableViewModel();
        var page = new TestRoutableView { ViewModel = originalViewModel };
        await host.PushAsync(page);

        host.PublicInvalidateCurrentViewModel();

        // The types are incompatible, so the page's view model is not replaced.
        await Assert.That(page.ViewModel).IsEqualTo(originalViewModel);
    }

    /// <summary>Invalidating when the current page view model is <see langword="null"/> takes the incompatible-type branch and does not assign.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<MauiRoutedViewHostTestExecutor>]
    public async Task InvalidateCurrentViewModel_NullPageViewModel_DoesNotUpdate()
    {
        var host = new TestableRoutedViewHost();

        host.Router.NavigationStack.Add(new TestRoutableViewModel());

        // The current page is an IViewFor whose ViewModel is null, exercising the null-conditional type comparison.
        var page = new TestRoutableView();
        await host.PushAsync(page);

        host.PublicInvalidateCurrentViewModel();

        await Assert.That(page.ViewModel).IsNull();
    }

    /// <summary>A navigate request whose stacks differ resolves the current page and pushes it via the navigate pipeline.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<MauiRoutedViewHostTestExecutor>]
    public async Task Navigate_StacksDiffer_ResolvesAndPushesCurrentPage()
    {
        var host = new TestableRoutedViewHost();

        // Pre-seed the navigation stack larger than the router stack so StacksAreDifferent enumerates the
        // navigation stack without indexing past it once Navigate appends the new view model.
        var seededView = CreateRoutableView();
        await host.PushAsync(seededView);
        await host.PushAsync(CreateRoutableView());

        host.Router.NavigationStack.Add(seededView.ViewModel!);

        // Navigate -> OnNavigateRequested -> StacksAreDifferent() == true -> schedules OnNavigateAsync ->
        // ResolveCurrentPage resolves and pushes the current view model's page.
        var navigateTarget = new TestRoutableViewModel();
        _ = host.Router.Navigate.Execute(navigateTarget).Subscribe(_ => { });
        await Task.Delay(SchedulerProcessingDelayMs);

        await Assert.That(host.Navigation.NavigationStack).IsNotEmpty();
    }

    /// <summary>A navigate request whose stacks already reference-match short-circuits in <c>OnNavigateRequested</c>.</summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Test]
    [TestExecutor<MauiRoutedViewHostTestExecutor>]
    public async Task Navigate_StacksAlreadyMatch_ShortCircuits()
    {
        var host = new TestableRoutedViewHost();

        // Make the pushed page reference-match the router view model that Navigate is about to append, so that the
        // navigate pipeline observes StacksAreDifferent() == false and returns early.
        var viewModel = new TestRoutableViewModel();
        var page = new TestRoutableView { ViewModel = viewModel };
        await host.PushAsync(page);

        var initialCount = host.Navigation.NavigationStack.Count;

        _ = host.Router.Navigate.Execute(viewModel).Subscribe(_ => { });
        await Task.Delay(SchedulerProcessingDelayMs);

        await Assert.That(host.Navigation.NavigationStack.Count).IsEqualTo(initialCount);
    }

    /// <summary>Creates a routable view seeded with its own view model.</summary>
    /// <returns>A new <see cref="TestRoutableView"/> whose view model is set.</returns>
    private static TestRoutableView CreateRoutableView() =>
        new() { ViewModel = new() };

    /// <summary>Test executor that sets up MAUI environment without registering an <see cref="IScreen"/>.</summary>
    [NotInParallel]
    public sealed class MauiRoutedViewHostNoScreenExecutor : MauiTestExecutor
    {
        /// <summary>The helper that configures and tears down the ReactiveUI app builder.</summary>
        private readonly AppBuilderTestHelper _helper = new();

        /// <inheritdoc/>
        protected override void Initialize()
        {
            base.Initialize();

            // Deliberately do not register an IScreen so the RoutedViewHost constructor throws.
            _helper.Initialize(builder => _ = builder.WithMaui().WithCoreServices());
        }

        /// <inheritdoc/>
        protected override void CleanUp()
        {
            _helper.CleanUp();
            base.CleanUp();
        }
    }

    /// <summary>Test executor that sets up MAUI environment with view registration.</summary>
    [NotInParallel]
    public sealed class MauiRoutedViewHostTestExecutor : MauiTestExecutor
    {
        /// <summary>The helper that configures and tears down the ReactiveUI app builder.</summary>
        private readonly AppBuilderTestHelper _helper = new();

        /// <inheritdoc/>
        protected override void Initialize()
        {
            base.Initialize();

            _helper.Initialize(builder =>
            {
                _ = builder
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

    /// <summary>Testable RoutedViewHost that exposes protected methods.</summary>
    [RequiresUnreferencedCode(
        "This class uses reflection to determine view model types at runtime through ViewLocator, which may be incompatible with trimming.")]
    [RequiresDynamicCode("ViewLocator.ResolveView uses reflection which is incompatible with AOT compilation.")]
    private sealed class TestableRoutedViewHost : RoutedViewHost
    {
        /// <summary>Exposes the protected PagesForViewModel method.</summary>
        /// <param name="vm">The view model.</param>
        /// <returns>An observable of pages.</returns>
        public IObservable<Page> PublicPagesForViewModel(IRoutableViewModel? vm) =>
            PagesForViewModel(vm);

        /// <summary>Exposes the protected PageForViewModel method.</summary>
        /// <param name="vm">The view model.</param>
        /// <returns>The page for the view model.</returns>
        public Page PublicPageForViewModel(IRoutableViewModel vm) =>
            PageForViewModel(vm);

        /// <summary>Exposes the protected InvalidateCurrentViewModel method.</summary>
        public void PublicInvalidateCurrentViewModel() =>
            InvalidateCurrentViewModel();

        /// <summary>Exposes the protected SyncNavigationStacksAsync method.</summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        public Task PublicSyncNavigationStacksAsync() =>
            SyncNavigationStacksAsync();
    }

    /// <summary>Test routable view model.</summary>
    private sealed class TestRoutableViewModel : ReactiveObject, IRoutableViewModel
    {
        /// <inheritdoc/>
        public string? UrlPathSegment { get; set; } = "test";

        /// <inheritdoc/>
        public IScreen HostScreen { get; } = null!;
    }

    /// <summary>Test routable view.</summary>
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

    /// <summary>A second routable view model of a distinct type, used to exercise incompatible-type branches.</summary>
    private sealed class SecondRoutableViewModel : ReactiveObject, IRoutableViewModel
    {
        /// <inheritdoc/>
        public string? UrlPathSegment { get; set; } = "second";

        /// <inheritdoc/>
        public IScreen HostScreen { get; } = null!;
    }

    /// <summary>Unregistered view model for testing error cases.</summary>
    private sealed class UnregisteredViewModel : ReactiveObject, IRoutableViewModel
    {
        /// <inheritdoc/>
        public string? UrlPathSegment { get; set; } = "unregistered";

        /// <inheritdoc/>
        public IScreen HostScreen { get; } = null!;
    }

    /// <summary>Test screen implementation.</summary>
    private sealed class TestScreen : ReactiveObject, IScreen
    {
        /// <inheritdoc/>
        public RoutingState Router { get; } = new(Sequencer.Immediate);
    }
}
