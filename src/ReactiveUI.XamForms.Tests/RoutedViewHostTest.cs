// Copyright (c) 2023 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using ReactiveUI.XamForms.Tests.Mocks;
using Splat;
using Xunit;

namespace ReactiveUI.XamForms.Tests
{
    /// <summary>
    /// Tests the RoutedView hosting.
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public sealed class RoutedViewHostTest : IDisposable
    {
        private readonly NavigationViewModel _navigationViewModel = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="RoutedViewHostTest"/> class.
        /// </summary>
        public RoutedViewHostTest()
        {
            Locator.CurrentMutable.Register<IViewFor<MainViewModel>>(() => new MainView());
            Locator.CurrentMutable.Register<IViewFor<ChildViewModel>>(() => new ChildView());

            Locator.CurrentMutable.Register<IScreen>(() => _navigationViewModel);
            Locator.CurrentMutable.Register<IRoutableViewModel>(() => new MainViewModel(), nameof(MainViewModel));
            Locator.CurrentMutable.Register<IRoutableViewModel>(() => new ChildViewModel(), nameof(ChildViewModel));
        }

        /// <summary>
        /// Tests that creating a new RoutedView without a view model that it has no current page.
        /// </summary>
        [Fact]
        public void NewRoutedViewHostHasNoCurrentPage()
        {
            var fixture = CreateRoutedViewHost(initialViewModel: null);

            Assert.Null(fixture.CurrentPage);
            Assert.Equal(0, fixture.StackDepth);
        }

        /// <summary>
        /// Tests that creating a new RoutedView with a view model that it has the main view as the current page.
        /// </summary>
        [Fact]
        public void NewRoutedViewHostHasMainViewCurrentPage()
        {
            var fixture = CreateRoutedViewHost();

            var currentPage = fixture.CurrentPage;
            Assert.NotNull(currentPage);
            Assert.IsType<MainView>(currentPage);
            Assert.IsType<MainViewModel>(currentPage.BindingContext);
            Assert.NotNull(currentPage.BindingContext);
            Assert.Equal("Main view", currentPage.Title);

            Assert.Equal(1, fixture.StackDepth);
        }

        /// <summary>
        /// Tests that creating a new RoutedView can navigate to a page from a initial no page.
        /// </summary>
        /// <returns>A task to monitor the progress.</returns>
        [Fact]
        public async Task NavigateToMainViewFromNoPage()
        {
            var fixture = CreateRoutedViewHost(initialViewModel: null);

            var viewModel = await _navigationViewModel.Navigate(nameof(MainViewModel));

            Assert.NotNull(viewModel);

            var currentPage = fixture.CurrentPage;
            Assert.NotNull(currentPage);
            Assert.IsType<MainView>(currentPage);
            Assert.IsType<MainViewModel>(currentPage.BindingContext);
            Assert.Equal(viewModel, currentPage.BindingContext);
            Assert.Equal("Main view", currentPage.Title);

            Assert.Equal(1, fixture.StackDepth);
        }

        /// <summary>
        /// Test that makes sure that you can navigate to child views.
        /// </summary>
        /// <returns>A task to monitor the progress.</returns>
        [Fact]
        public async Task NavigateToChildView()
        {
            var fixture = CreateRoutedViewHost();

            var viewModel = await _navigationViewModel.Navigate(nameof(ChildViewModel));

            Assert.NotNull(viewModel);

            var currentPage = fixture.CurrentPage;
            Assert.NotNull(currentPage);
            Assert.IsType<ChildView>(currentPage);
            Assert.IsType<ChildViewModel>(currentPage.BindingContext);
            Assert.Equal(viewModel, currentPage.BindingContext);
            Assert.Equal("Child view: ", currentPage.Title);

            Assert.Equal(2, fixture.StackDepth);
        }

        /// <summary>
        /// Test that makes sure that you can navigate to second child view.
        /// </summary>
        /// <returns>A task to monitor the progress.</returns>
        [Fact]
        public async Task NavigateToSecondChildView()
        {
            var fixture = CreateRoutedViewHost();

            await _navigationViewModel.Navigate(nameof(ChildViewModel));
            var viewModel = await _navigationViewModel.NavigateToChild("Testing");

            Assert.NotNull(viewModel);

            var currentPage = fixture.CurrentPage;
            Assert.NotNull(currentPage);
            Assert.IsType<ChildView>(currentPage);
            Assert.IsType<ChildViewModel>(currentPage.BindingContext);
            Assert.Equal(viewModel, currentPage.BindingContext);
            Assert.Equal("Child view: Testing", currentPage.Title);

            Assert.Equal(3, fixture.StackDepth);
        }

        /// <summary>
        /// Test that makes sure that you can navigate back from child view.
        /// </summary>
        /// <returns>A task to monitor the progress.</returns>
        [Fact]
        public async Task NavigateBackFromChildView()
        {
            var fixture = CreateRoutedViewHost();
            var mainPage = fixture.CurrentPage;

            await _navigationViewModel.Navigate(nameof(ChildViewModel));
            await _navigationViewModel.NavigateBack();

            var currentPage = fixture.CurrentPage;
            Assert.Equal(mainPage, currentPage);
            Assert.IsType<MainView>(currentPage);
            Assert.IsType<MainViewModel>(currentPage.BindingContext);
            Assert.NotNull(currentPage.BindingContext);
            Assert.Equal("Main view", currentPage.Title);

            Assert.Equal(1, fixture.StackDepth);
        }

        /// <summary>
        /// Test that makes sure that you can navigate back from second child view.
        /// </summary>
        /// <returns>A task to monitor the progress.</returns>
        [Fact]
        public async Task NavigateBackFromSecondChildView()
        {
            var fixture = CreateRoutedViewHost();

            var childViewModel = await _navigationViewModel.Navigate(nameof(ChildViewModel));
            var childPage = fixture.CurrentPage;

            await _navigationViewModel.NavigateToChild("Testing");
            await _navigationViewModel.NavigateBack();

            var currentPage = fixture.CurrentPage;
            Assert.Equal(childPage, currentPage);
            Assert.IsType<ChildView>(currentPage);
            Assert.IsType<ChildViewModel>(currentPage.BindingContext);
            Assert.Equal(childViewModel, currentPage.BindingContext);
            Assert.Equal("Child view: ", currentPage.Title);

            Assert.Equal(2, fixture.StackDepth);
        }

        /// <summary>
        /// Test that makes sure that you can navigate back twice from a child view.
        /// </summary>
        /// <returns>A task to monitor the progress.</returns>
        [Fact]
        public async Task NavigateBackFromChildView2Times()
        {
            var fixture = CreateRoutedViewHost();
            var mainPage = fixture.CurrentPage;

            await _navigationViewModel.Navigate(nameof(ChildViewModel));
            await _navigationViewModel.NavigateToChild("Testing");
            await _navigationViewModel.NavigateBack();
            await _navigationViewModel.NavigateBack();

            var currentPage = fixture.CurrentPage;
            Assert.Equal(mainPage, currentPage);
            Assert.IsType<MainView>(currentPage);
            Assert.IsType<MainViewModel>(currentPage.BindingContext);
            Assert.NotNull(currentPage.BindingContext);
            Assert.Equal("Main view", currentPage.Title);

            Assert.Equal(1, fixture.StackDepth);
        }

        /// <summary>
        /// Test that makes sure that you can navigate back twice from the main view.
        /// </summary>
        /// <returns>A task to monitor the progress.</returns>
        [Fact]
        public async Task NavigateBackFromMainView()
        {
            var fixture = CreateRoutedViewHost();
            var mainPage = fixture.CurrentPage;

            await _navigationViewModel.NavigateBack();

            var currentPage = fixture.CurrentPage;
            Assert.Equal(mainPage, currentPage);
            Assert.IsType<MainView>(currentPage);
            Assert.IsType<MainViewModel>(currentPage.BindingContext);
            Assert.NotNull(currentPage.BindingContext);
            Assert.Equal("Main view", currentPage.Title);

            Assert.Equal(1, fixture.StackDepth);
        }

        /// <summary>
        /// Test that makes sure that you can navigate back from the main view then to the child view.
        /// </summary>
        /// <returns>A task to monitor the progress.</returns>
        [Fact]
        public async Task NavigateBackFromMainViewAndThenToChildView()
        {
            var fixture = CreateRoutedViewHost();

            await _navigationViewModel.NavigateBack();
            var viewModel = await _navigationViewModel.Navigate(nameof(ChildViewModel));

            Assert.NotNull(viewModel);

            var currentPage = fixture.CurrentPage;
            Assert.NotNull(currentPage);
            Assert.IsType<ChildView>(currentPage);
            Assert.IsType<ChildViewModel>(currentPage.BindingContext);
            Assert.Equal(viewModel, currentPage.BindingContext);
            Assert.Equal("Child view: ", currentPage.Title);

            Assert.Equal(1, fixture.StackDepth);
        }

        /// <summary>
        /// Test that makes sure that you can navigate to a child view then reset.
        /// </summary>
        /// <param name="stackDepthBefore">The stack depth before the reset.</param>
        /// <returns>A task to monitor the progress.</returns>
        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        public async Task NavigateToChildViewAndReset(int stackDepthBefore)
        {
            var fixture = CreateRoutedViewHost(stackDepthBefore > 0 ? nameof(MainViewModel) : null);

            if (stackDepthBefore > 1)
            {
                await _navigationViewModel.Navigate(nameof(ChildViewModel));
            }

            var viewModel = await _navigationViewModel.NavigateAndResetToChild("Reset test");

            Assert.NotNull(viewModel);

            var currentPage = fixture.CurrentPage;
            Assert.NotNull(currentPage);
            Assert.IsType<ChildView>(currentPage);
            Assert.IsType<ChildViewModel>(currentPage.BindingContext);
            Assert.Equal(viewModel, currentPage.BindingContext);
            Assert.Equal("Child view: Reset test", currentPage.Title);

            Assert.Equal(1, fixture.StackDepth);
        }

        /// <summary>
        /// Test that makes sure that you can navigate back from a child view.
        /// </summary>
        /// <param name="animated">If we should navigated animated.</param>
        /// <param name="fast">If we should navigate fast.</param>
        /// <returns>A task to monitor the progress.</returns>
        [Theory]
        [InlineData(false, false)]
        [InlineData(false, true)]
        [InlineData(true, false)]
        [InlineData(true, true)]
        public async Task GoBackFromChildView(bool animated, bool fast)
        {
            var fixture = CreateRoutedViewHost();
            var viewModel = _navigationViewModel.Router.GetCurrentViewModel();

            await _navigationViewModel.Navigate(nameof(ChildViewModel));
            await fixture.PopAsyncInner(animated, fast).ConfigureAwait(true);

            var viewModelSearch = _navigationViewModel.Router.FindViewModelInStack<MainViewModel>();
            Assert.NotNull(viewModelSearch);

            Assert.Equal(1, fixture.StackDepth);
            var navigationStack = _navigationViewModel.Router.NavigationStack;
            Assert.Equal(1, navigationStack.Count);
            Assert.Equal(viewModel, navigationStack[0]);

            var currentPage = fixture.CurrentPage;
            Assert.NotNull(currentPage);
            Assert.IsType<MainView>(currentPage);
            Assert.IsType<MainViewModel>(currentPage.BindingContext);
            Assert.Equal(viewModel, currentPage.BindingContext);
            Assert.Equal("Main view", currentPage.Title);
        }

        /// <summary>
        /// Test that makes sure that you can navigate back from a second child view.
        /// </summary>
        /// <param name="animated">If we should navigated animated.</param>
        /// <param name="fast">If we should navigate fast.</param>
        /// <returns>A task to monitor the progress.</returns>
        [Theory]
        [InlineData(false, false)]
        [InlineData(false, true)]
        [InlineData(true, false)]
        [InlineData(true, true)]
        public async Task GoBackFromSecondChildView(bool animated, bool fast)
        {
            var fixture = CreateRoutedViewHost();
            var rootViewModel = _navigationViewModel.Router.GetCurrentViewModel();

            var viewModel = await _navigationViewModel.NavigateToChild("C1");
            await _navigationViewModel.NavigateToChild("C2");
            await fixture.PopAsyncInner(animated, fast).ConfigureAwait(true);

            Assert.Equal(2, fixture.StackDepth);
            var navigationStack = _navigationViewModel.Router.NavigationStack;
            Assert.Equal(2, navigationStack.Count);
            Assert.Equal(rootViewModel, navigationStack[0]);
            Assert.Equal(viewModel, navigationStack[1]);

            var currentPage = fixture.CurrentPage;
            Assert.NotNull(currentPage);
            Assert.IsType<ChildView>(currentPage);
            Assert.IsType<ChildViewModel>(currentPage.BindingContext);
            Assert.Equal(viewModel, currentPage.BindingContext);
            Assert.Equal("Child view: C1", currentPage.Title);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        public void Dispose() => Locator.SetLocator(new ModernDependencyResolver());

        private RoutedViewHost CreateRoutedViewHost(string? initialViewModel = nameof(MainViewModel))
        {
            if (initialViewModel is not null)
            {
                var mainViewModel = Locator.Current.GetService<IRoutableViewModel>(initialViewModel);

                if (mainViewModel is null)
                {
                    throw new InvalidOperationException("There should be a valid view model.");
                }

                _navigationViewModel.Router.NavigationStack.Add(mainViewModel);
            }

            var routedViewHost = new RoutedViewHost();
            routedViewHost.SendAppearing();
            return routedViewHost;
        }
    }
}
