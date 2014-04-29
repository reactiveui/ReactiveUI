using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using Microsoft.Reactive.Testing;
using ReactiveUI.Testing;
using ReactiveUI.Tests;
using Xunit;

namespace ReactiveUI.Routing.Tests
{
    public class TestViewModel : ReactiveObject, IRoutableViewModel
    {
        string _SomeProp;
        public string SomeProp {
            get { return _SomeProp; }
            set { this.RaiseAndSetIfChanged(ref _SomeProp, value); }
        }

        public string UrlPathSegment {
            get { return "Test"; }
        }

        public IScreen HostScreen
        {
            get { return null; }
        }
    }

    public class TestScreen : ReactiveObject, IScreen
    {
        RoutingState _Router;
        public RoutingState Router {
            get { return _Router; }
            set { this.RaiseAndSetIfChanged(ref _Router, value); }
        }
    }

    public class CustomRoutingParams : IRoutingParams
    {
        public bool NotInNavigationStack { get; set; }
        public string Contract { get; set; }

        public bool AsDialog { get; set; }
    }

    public class RoutingStateTests
    {
        [Fact]
        public void NavigationPushPopTest()
        {
            var input = new TestViewModel() {SomeProp = "Foo"};
            var fixture = new RoutingState();

            Assert.False(fixture.NavigateBack.CanExecute(input));
            fixture.Navigate.Execute(new TestViewModel());

            Assert.Equal(1, fixture.NavigationStack.Count);
            Assert.False(fixture.NavigateBack.CanExecute(null));

            fixture.Navigate.Execute(new TestViewModel());

            Assert.Equal(2, fixture.NavigationStack.Count);
            Assert.True(fixture.NavigateBack.CanExecute(null));

            fixture.NavigateBack.Execute(null);

            Assert.Equal(1, fixture.NavigationStack.Count);
        }

        [Fact]
        public void CurrentViewModelObservableIsAccurate()
        {
            var fixture = new RoutingState();
            var output = fixture.CurrentViewModel.CreateCollection();

            Assert.Equal(1, output.Count);

            fixture.Navigate.Execute(new TestViewModel() { SomeProp = "A" });
            Assert.Equal(2, output.Count);

            fixture.Navigate.Execute(new TestViewModel() { SomeProp = "B" });
            Assert.Equal(3, output.Count);
            Assert.Equal("B", ((TestViewModel)output.Last()).SomeProp);

            fixture.NavigateBack.Execute(null);
            Assert.Equal(4, output.Count);
            Assert.Equal("A", ((TestViewModel)output.Last()).SomeProp);
        }

        [Fact]
        public void CurrentViewModelObservableIsAccurateViaWhenAnyObservable()
        {
            var fixture = new TestScreen();
            var output = fixture.WhenAnyObservable(x => x.Router.CurrentViewModel).CreateCollection();
            fixture.Router = new RoutingState();

            Assert.Equal(1, output.Count);

            fixture.Router.Navigate.Execute(new TestViewModel() { SomeProp = "A" });
            Assert.Equal(2, output.Count);

            fixture.Router.Navigate.Execute(new TestViewModel() { SomeProp = "B" });
            Assert.Equal(3, output.Count);
            Assert.Equal("B", ((TestViewModel)output.Last()).SomeProp);

            fixture.Router.NavigateBack.Execute(null);
            Assert.Equal(4, output.Count);
            Assert.Equal("A", ((TestViewModel)output.Last()).SomeProp);
        }
<<<<<<< HEAD

        [Fact]
        public void NavigateWithoutBackStack()
        {
            var input = new TestViewModel() { SomeProp = "Foo" };
            var fixture = new RoutingState();

            // check default navigation works
            fixture.Navigate.Execute(new TestViewModel());
            Assert.Equal(1, fixture.NavigationStack.Count);

            // navigate (via ExtensionMethod) without putting the call to the NavigationStack
            bool notInNavigationStack = true;
            fixture.Navigate(input, notInNavigationStack);
            Assert.Equal(1, fixture.NavigationStack.Count);

            // navigate (via ExtensionMethod) with putting the call to the NavigationStack
            notInNavigationStack = false;
            fixture.Navigate(input, notInNavigationStack);
            Assert.Equal(2, fixture.NavigationStack.Count);

        }

        [Fact]
        public void NavigateBackViewModelObservableIsAccurate()
        {
            var fixture = new RoutingState();
            var output = fixture.NavigateBackViewModel.CreateCollection();

            Assert.Equal(0, output.Count);

            fixture.Navigate.Execute(new TestViewModel() { SomeProp = "A" });
            Assert.Equal(0, output.Count);

            fixture.Navigate.Execute(new TestViewModel() { SomeProp = "B" });
            Assert.Equal(0, output.Count);

            fixture.NavigateBack.Execute(null);
            Assert.Equal(1, output.Count);
            Assert.Equal("A", ((TestViewModel)output.Last()).SomeProp);

        }

        [Fact]
        public void NavigateCommandWithRoutingParams()
        {
            var fixture = new RoutingState();
            var output = fixture.Navigate.CreateCollection();

            Assert.Equal(0, output.Count);

            fixture.Navigate.Execute(new RoutableViewModelWithParams(new TestViewModel{ SomeProp = "A"}, new RoutingParams{Contract = "A"}));
            Assert.Equal(1, output.Count);

            var navigateValue = output.Last();
            Assert.NotEqual(null, navigateValue);

            var viewModelWithParams = navigateValue.AsRoutableViewModel<IRoutableViewModel>();
            Assert.NotEqual(null, viewModelWithParams);

            Assert.Equal("A", ((TestViewModel)viewModelWithParams.Item1).SomeProp);
            Assert.Equal("A", ((RoutingParams)viewModelWithParams.Item2).Contract);

        }
        
        [Fact]
        public void NavigateCommandWithCustomRoutingParams()
        {
            var fixture = new RoutingState();
            var output = fixture.Navigate.CreateCollection();

            Assert.Equal(0, output.Count);

            fixture.Navigate.Execute(new RoutableViewModelWithParams(new TestViewModel { SomeProp = "A" }, new CustomRoutingParams { AsDialog = true }));
            Assert.Equal(1, output.Count);

            var navigateValue = output.Last();
            Assert.NotEqual(null, navigateValue);

            var viewModelWithParams = navigateValue.AsRoutableViewModel<IRoutableViewModel>();
            Assert.NotEqual(null, viewModelWithParams);

            Assert.Equal("A", ((TestViewModel)viewModelWithParams.Item1).SomeProp);
            Assert.True(((CustomRoutingParams)viewModelWithParams.Item2).AsDialog);

        }

        [Fact]
        public void RoutingStateExcecuteNavigateWithParamsUsingTuple()
        {
            var fixture = new RoutingState();
            var output = fixture.Navigate.CreateCollection();

            Assert.Equal(0, output.Count);

            fixture.ExcecuteNavigateWithParams(
                Tuple.Create(
                    new TestViewModel { SomeProp = "A" } as IRoutableViewModel, 
                    new RoutingParams { Contract = "A" } as IRoutingParams
                ));
            Assert.Equal(1, output.Count);

            var navigateValue = output.Last();
            Assert.NotEqual(null, navigateValue);

            var viewModelWithParams = navigateValue.AsRoutableViewModel<IRoutableViewModel>();
            Assert.NotEqual(null, viewModelWithParams);

            Assert.Equal("A", ((TestViewModel)viewModelWithParams.Item1).SomeProp);
            Assert.Equal("A", ((RoutingParams)viewModelWithParams.Item2).Contract);

        }

        [Fact]
        public void RoutingStateExcecuteNavigateWithParamsUsingViewModelAndParams()
        {
            var fixture = new RoutingState();
            var output = fixture.Navigate.CreateCollection();

            Assert.Equal(0, output.Count);

            fixture.ExcecuteNavigateWithParams(
                    new TestViewModel { SomeProp = "A" },
                    new RoutingParams { Contract = "A" }
                );
            Assert.Equal(1, output.Count);

            var navigateValue = output.Last();
            Assert.NotEqual(null, navigateValue);

            var viewModelWithParams = navigateValue.AsRoutableViewModel<IRoutableViewModel>();
            Assert.NotEqual(null, viewModelWithParams);

            Assert.Equal("A", ((TestViewModel)viewModelWithParams.Item1).SomeProp);
            Assert.Equal("A", ((RoutingParams)viewModelWithParams.Item2).Contract);
        }

        
=======
        
        [Fact]
        public void NavigateAndResetCheckNavigationStack()
        {
            var fixture = new TestScreen();
            fixture.Router = new RoutingState();
            var viewModel = new TestViewModel();

            Assert.False(fixture.Router.NavigationStack.Any());

            fixture.Router.NavigateAndReset.Execute(viewModel);

            Assert.True(fixture.Router.NavigationStack.Count == 1);
            Assert.True(object.ReferenceEquals(fixture.Router.NavigationStack.First(), viewModel));
        }
>>>>>>> upstream/rxui6-master
    }
}
