using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using ReactiveUI.XamForms.Tests.Mocks;
using Splat;
using Xunit;

namespace ReactiveUI.XamForms.Tests
{
    public sealed class RoutedViewHostTest : IDisposable
    {
        private readonly NavigationViewModel _navigationViewModel = new NavigationViewModel();

        public RoutedViewHostTest()
        {
            Locator.CurrentMutable.Register<IViewFor<MainViewModel>>(() => new MainView());
            Locator.CurrentMutable.Register<IViewFor<ChildViewModel>>(() => new ChildView());

            Locator.CurrentMutable.Register<IScreen>(() => _navigationViewModel);
            Locator.CurrentMutable.Register<IRoutableViewModel>(() => new MainViewModel(), nameof(MainViewModel));
            Locator.CurrentMutable.Register<IRoutableViewModel>(() => new ChildViewModel(), nameof(ChildViewModel));
        }

        [Fact]
        public void NewRoutedViewHostHasNoCurrentPage()
        {
            var fixture = CreateRoutedViewHost(initialViewModel: null);

            Assert.Null(fixture.CurrentPage);
            Assert.Equal(0, fixture.StackDepth);
        }

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

        [Fact]
        public async Task NavigateToMainViewFromNoPage()
        {
            var fixture = CreateRoutedViewHost(initialViewModel: null);

            var viewModel = await _navigationViewModel.Navigate(nameof(MainViewModel)).GetAwaiter();

            Assert.NotNull(viewModel);

            var currentPage = fixture.CurrentPage;
            Assert.NotNull(currentPage);
            Assert.IsType<MainView>(currentPage);
            Assert.IsType<MainViewModel>(currentPage.BindingContext);
            Assert.Equal(viewModel, currentPage.BindingContext);
            Assert.Equal("Main view", currentPage.Title);

            Assert.Equal(1, fixture.StackDepth);
        }

        [Fact]
        public async Task NavigateToChildView()
        {
            var fixture = CreateRoutedViewHost();

            var viewModel = await _navigationViewModel.Navigate(nameof(ChildViewModel)).GetAwaiter();

            Assert.NotNull(viewModel);

            var currentPage = fixture.CurrentPage;
            Assert.NotNull(currentPage);
            Assert.IsType<ChildView>(currentPage);
            Assert.IsType<ChildViewModel>(currentPage.BindingContext);
            Assert.Equal(viewModel, currentPage.BindingContext);
            Assert.Equal("Child view: ", currentPage.Title);

            Assert.Equal(2, fixture.StackDepth);
        }

        [Fact]
        public async Task NavigateToSecondChildView()
        {
            var fixture = CreateRoutedViewHost();

            await _navigationViewModel.Navigate(nameof(ChildViewModel)).GetAwaiter();
            var viewModel = await _navigationViewModel.NavigateToChild("Testing").GetAwaiter();

            Assert.NotNull(viewModel);

            var currentPage = fixture.CurrentPage;
            Assert.NotNull(currentPage);
            Assert.IsType<ChildView>(currentPage);
            Assert.IsType<ChildViewModel>(currentPage.BindingContext);
            Assert.Equal(viewModel, currentPage.BindingContext);
            Assert.Equal("Child view: Testing", currentPage.Title);

            Assert.Equal(3, fixture.StackDepth);
        }

        [Fact]
        public async Task NavigateBackFromChildView()
        {
            var fixture = CreateRoutedViewHost();
            var mainPage = fixture.CurrentPage;

            await _navigationViewModel.Navigate(nameof(ChildViewModel)).GetAwaiter();
            await _navigationViewModel.NavigateBack().GetAwaiter();

            var currentPage = fixture.CurrentPage;
            Assert.Equal(mainPage, currentPage);
            Assert.IsType<MainView>(currentPage);
            Assert.IsType<MainViewModel>(currentPage.BindingContext);
            Assert.NotNull(currentPage.BindingContext);
            Assert.Equal("Main view", currentPage.Title);

            Assert.Equal(1, fixture.StackDepth);
        }

        [Fact]
        public async Task NavigateBackFromSecondChildView()
        {
            var fixture = CreateRoutedViewHost();

            var childViewModel = await _navigationViewModel.Navigate(nameof(ChildViewModel)).GetAwaiter();
            var childPage = fixture.CurrentPage;

            var viewModel = await _navigationViewModel.NavigateToChild("Testing").GetAwaiter();
            await _navigationViewModel.NavigateBack().GetAwaiter();

            var currentPage = fixture.CurrentPage;
            Assert.Equal(childPage, currentPage);
            Assert.IsType<ChildView>(currentPage);
            Assert.IsType<ChildViewModel>(currentPage.BindingContext);
            Assert.Equal(childViewModel, currentPage.BindingContext);
            Assert.Equal("Child view: ", currentPage.Title);

            Assert.Equal(2, fixture.StackDepth);
        }

        [Fact]
        public async Task NavigateBackFromChildView2Times()
        {
            var fixture = CreateRoutedViewHost();
            var mainPage = fixture.CurrentPage;

            await _navigationViewModel.Navigate(nameof(ChildViewModel)).GetAwaiter();
            await _navigationViewModel.NavigateToChild("Testing").GetAwaiter();
            await _navigationViewModel.NavigateBack().GetAwaiter();
            await _navigationViewModel.NavigateBack().GetAwaiter();

            var currentPage = fixture.CurrentPage;
            Assert.Equal(mainPage, currentPage);
            Assert.IsType<MainView>(currentPage);
            Assert.IsType<MainViewModel>(currentPage.BindingContext);
            Assert.NotNull(currentPage.BindingContext);
            Assert.Equal("Main view", currentPage.Title);

            Assert.Equal(1, fixture.StackDepth);
        }

        [Fact]
        public async Task NavigateBackFromMainView()
        {
            var fixture = CreateRoutedViewHost();
            var mainPage = fixture.CurrentPage;

            await _navigationViewModel.NavigateBack().GetAwaiter();

            var currentPage = fixture.CurrentPage;
            Assert.Equal(mainPage, currentPage);
            Assert.IsType<MainView>(currentPage);
            Assert.IsType<MainViewModel>(currentPage.BindingContext);
            Assert.NotNull(currentPage.BindingContext);
            Assert.Equal("Main view", currentPage.Title);

            Assert.Equal(1, fixture.StackDepth);
        }

        [Fact]
        public async Task NavigateBackFromMainViewAndThenToChildView()
        {
            var fixture = CreateRoutedViewHost();

            await _navigationViewModel.NavigateBack().GetAwaiter();
            var viewModel = await _navigationViewModel.Navigate(nameof(ChildViewModel)).GetAwaiter();

            Assert.NotNull(viewModel);

            var currentPage = fixture.CurrentPage;
            Assert.NotNull(currentPage);
            Assert.IsType<ChildView>(currentPage);
            Assert.IsType<ChildViewModel>(currentPage.BindingContext);
            Assert.Equal(viewModel, currentPage.BindingContext);
            Assert.Equal("Child view: ", currentPage.Title);

            Assert.Equal(1, fixture.StackDepth);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        public async Task NavigateToChildViewAndReset(int stackDepthBefore)
        {
            var fixture = CreateRoutedViewHost(stackDepthBefore > 0 ? nameof(MainViewModel) : null);

            if (stackDepthBefore > 1)
            {
                await _navigationViewModel.Navigate(nameof(ChildViewModel)).GetAwaiter();
            }

            var viewModel = await _navigationViewModel.NavigateAndResetToChild("Reset test").GetAwaiter();

            Assert.NotNull(viewModel);

            var currentPage = fixture.CurrentPage;
            Assert.NotNull(currentPage);
            Assert.IsType<ChildView>(currentPage);
            Assert.IsType<ChildViewModel>(currentPage.BindingContext);
            Assert.Equal(viewModel, currentPage.BindingContext);
            Assert.Equal("Child view: Reset test", currentPage.Title);

            Assert.Equal(1, fixture.StackDepth);
        }

        [Theory]
        [InlineData(false, false)]
        [InlineData(false, true)]
        [InlineData(true, false)]
        [InlineData(true, true)]
        public async Task GoBackFromChildView(bool animated, bool fast)
        {
            var fixture = CreateRoutedViewHost();
            var viewModel = _navigationViewModel.Router.GetCurrentViewModel();

            await _navigationViewModel.Navigate(nameof(ChildViewModel)).GetAwaiter();
            await fixture.PopAsyncInner(animated, fast).ConfigureAwait(true);

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

        [Theory]
        [InlineData(false, false)]
        [InlineData(false, true)]
        [InlineData(true, false)]
        [InlineData(true, true)]
        public async Task GoBackFromSecondChildView(bool animated, bool fast)
        {
            var fixture = CreateRoutedViewHost();
            var rootViewModel = _navigationViewModel.Router.GetCurrentViewModel();

            var viewModel = await _navigationViewModel.NavigateToChild("C1").GetAwaiter();
            await _navigationViewModel.NavigateToChild("C2").GetAwaiter();
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

        public void Dispose()
        {
            Locator.SetLocator(new ModernDependencyResolver());
        }

        private RoutedViewHost CreateRoutedViewHost(string? initialViewModel = nameof(MainViewModel))
        {
            if (initialViewModel != null)
            {
                var mainViewModel = Locator.Current.GetService<IRoutableViewModel>(initialViewModel);
                _navigationViewModel.Router.NavigationStack.Add(mainViewModel);
            }

            var routedViewHost = new RoutedViewHost();
            routedViewHost.SendAppearing();
            return routedViewHost;
        }
    }
}
