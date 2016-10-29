using System;
using System.Reactive.Disposables;
using ReactiveUI.Tests.RoutableViewMixinTests;
using Xunit;

namespace ReactiveUI.Tests
{
    namespace RoutableViewMixinTests
    {
        public class TestScreen : IScreen
        {
            public RoutingState Router { get; } = new RoutingState();
        }

        public class RoutableViewModel : ReactiveObject, IRoutableViewModel
        {
            public RoutableViewModel(IScreen screen)
            {
                HostScreen = screen;
            }

            public string UrlPathSegment => "Test";
            public IScreen HostScreen { get; }
        }
    }

    public class RoutableViewModelMixinTests
    {
        [Fact]
        public void WhenNavigatedToCallsOnNavigatedToWhenViewModelIsFirstAdded()
        {
            var count = 0;

            var screen = new TestScreen();
            var vm = new RoutableViewModel(screen);

            vm.WhenNavigatedTo(() => {
                count++;

                return Disposable.Empty;
            });

            screen.Router.Navigate.Execute(vm);

            Assert.Equal(1, count);
        }

        [Fact]
        public void WhenNavigatedToCallsOnNavigatedToWhenViewModelReturnsToTopOfStack()
        {
            var count = 0;

            var screen = new TestScreen();
            var vm = new RoutableViewModel(screen);
            var vm2 = new RoutableViewModel(screen);

            vm.WhenNavigatedTo(() => {
                count++;

                return Disposable.Empty;
            });

            screen.Router.Navigate.Execute(vm);
            screen.Router.Navigate.Execute(vm2);
            screen.Router.Navigate.Execute(vm);

            Assert.Equal(2, count);
        }

        [Fact]
        public void WhenNavigatedToCallsDisposeWhenViewModelLosesFocus()
        {
            var count = 0;

            var screen = new TestScreen();
            var vm = new RoutableViewModel(screen);
            var vm2 = new RoutableViewModel(screen);

            vm.WhenNavigatedTo(() => {
                return Disposable.Create(() => count++);
            });

            screen.Router.Navigate.Execute(vm);

            Assert.Equal(0, count);

            screen.Router.Navigate.Execute(vm2);

            Assert.Equal(1, count);
        }

        [Fact]
        public void WhenNavigatedToObservableFiresWhenViewModelAddedToNavigationStack()
        {
            var count = 0;

            var screen = new TestScreen();
            var vm = new RoutableViewModel(screen);

            vm.WhenNavigatedToObservable().Subscribe(_ => {
                count++;
            });

            screen.Router.Navigate.Execute(vm);

            Assert.Equal(1, count);
        }

        [Fact]
        public void WhenNavigatedToObservableFiresWhenViewModelReturnsToNavigationStack()
        {
            var count = 0;

            var screen = new TestScreen();
            var vm = new RoutableViewModel(screen);
            var vm2 = new RoutableViewModel(screen);

            vm.WhenNavigatedToObservable().Subscribe(_ => {
                count++;
            });

            screen.Router.Navigate.Execute(vm);
            screen.Router.Navigate.Execute(vm2);
            screen.Router.Navigate.Execute(vm);

            Assert.Equal(2, count);
        }

        [Fact]
        public void WhenNavigatedToObservableCompletesWhenViewModelIsRemovedFromNavigationStack()
        {
            var count = 0;

            var screen = new TestScreen();
            var vm = new RoutableViewModel(screen);

            vm.WhenNavigatedToObservable().Subscribe(
                _ => {},
                () => { count++; });

            screen.Router.Navigate.Execute(vm);
            screen.Router.NavigateBack.Execute();

            Assert.Equal(1, count);
        }

        [Fact]
        public void WhenNavigatingFromObservableFiresWhenViewModelLosesFocus()
        {
            var count = 0;
            var screen = new TestScreen();
            var vm = new RoutableViewModel(screen);
            var vm2 = new RoutableViewModel(screen);

            vm.WhenNavigatingFromObservable().Subscribe(_ => {
                count++;
            });

            screen.Router.Navigate.Execute(vm);
            screen.Router.Navigate.Execute(vm2);

            Assert.Equal(1, count);
        }

        [Fact]
        public void WhenNavigatingFromObservableCompletesWhenViewModelIsRemovedFromNavigationStack()
        {
            var count = 0;

            var screen = new TestScreen();
            var vm = new RoutableViewModel(screen);

            vm.WhenNavigatingFromObservable().Subscribe(
                _ => {},
                () => { count++; });

            screen.Router.Navigate.Execute(vm);
            screen.Router.NavigateBack.Execute();

            Assert.Equal(1, count);
        }
    }
}
