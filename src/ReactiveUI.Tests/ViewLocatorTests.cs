using System;
using ReactiveUI;
using Splat;
using Xunit;

namespace ReactiveUI.Tests
{
    public interface IFooViewModel { }

    public interface IBarViewModel { }

    public class FooViewModel : ReactiveObject, IFooViewModel
    {
    }

    public class FooViewModelWithWeirdName : ReactiveObject, IFooViewModel
    {
    }

    public class BarViewModel : ReactiveObject, IBarViewModel
    {
    }

    public interface IFooView : IViewFor<IFooViewModel> { }

    public class FooView : IFooView
    {
        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (IFooViewModel)value; }
        }

        public IFooViewModel ViewModel { get; set; }
    }

    public class BarView : IViewFor<IBarViewModel>
    {
        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (IBarViewModel)value; }
        }

        public IBarViewModel ViewModel { get; set; }
    }

    public class FooWithWeirdConvention : IFooView
    {
        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (IFooViewModel)value; }
        }

        public IFooViewModel ViewModel { get; set; }
    }

    public class FooThatThrowsView : IFooView
    {
        public FooThatThrowsView()
        {
            throw new InvalidOperationException("This is a test failure.");
        }

        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (IFooViewModel)value; }
        }

        public IFooViewModel ViewModel { get; set; }
    }

    public class DefaultViewLocatorTests
    {
        [Fact]
        public void ByDefaultViewModelIsReplacedWithViewWhenDeterminingTheServiceName()
        {
            var resolver = new ModernDependencyResolver();

            resolver.InitializeSplat();
            resolver.InitializeReactiveUI();
            resolver.Register(() => new FooView(), typeof(IViewFor<FooViewModel>));

            using (resolver.WithResolver()) {
                var fixture = new DefaultViewLocator();
                FooViewModel vm = new FooViewModel();

                var result = fixture.ResolveView(vm);
                Assert.IsType<FooView>(result);
            }
        }

        [Fact]
        public void TheRuntimeTypeOfTheViewModelIsUsedToResolveTheView()
        {
            var resolver = new ModernDependencyResolver();

            resolver.InitializeSplat();
            resolver.InitializeReactiveUI();
            resolver.Register(() => new FooView(), typeof(FooView));

            using (resolver.WithResolver()) {
                var fixture = new DefaultViewLocator();
                object vm = new FooViewModel();

                var result = fixture.ResolveView(vm);
                Assert.IsType<FooView>(result );
            }
        }

        [Fact]
        public void ViewModelToViewNamingConventionCanBeCustomized()
        {
            var resolver = new ModernDependencyResolver();

            resolver.InitializeSplat();
            resolver.InitializeReactiveUI();
            resolver.Register(() => new FooWithWeirdConvention(), typeof(FooWithWeirdConvention));

            using (resolver.WithResolver()) {
                var fixture = new DefaultViewLocator();
                fixture.ViewModelToViewFunc = viewModelName => viewModelName.Replace("ViewModel", "WithWeirdConvention");
                FooViewModel vm = new FooViewModel();

                var result = fixture.ResolveView(vm);
                Assert.IsType<FooWithWeirdConvention>(result);
            }
        }

        [Fact]
        public void CanResolveViewFromViewModelClassUsingClassRegistration()
        {
            var resolver = new ModernDependencyResolver();

            resolver.InitializeSplat();
            resolver.InitializeReactiveUI();
            resolver.Register(() => new FooView(), typeof(FooView));

            using (resolver.WithResolver()) {
                var fixture = new DefaultViewLocator();
                FooViewModel vm = new FooViewModel();

                var result = fixture.ResolveView(vm);
                Assert.IsType<FooView>(result);
            }
        }

        [Fact]
        public void CanResolveViewFromViewModelClassUsingInterfaceRegistration()
        {
            var resolver = new ModernDependencyResolver();

            resolver.InitializeSplat();
            resolver.InitializeReactiveUI();
            resolver.Register(() => new FooView(), typeof(IFooView));

            using (resolver.WithResolver()) {
                var fixture = new DefaultViewLocator();
                FooViewModel vm = new FooViewModel();

                var result = fixture.ResolveView(vm);
                Assert.IsType<FooView>(result);
            }
        }

        [Fact]
        public void CanResolveViewFromViewModelClassUsingIViewForRegistration()
        {
            var resolver = new ModernDependencyResolver();

            resolver.InitializeSplat();
            resolver.InitializeReactiveUI();
            resolver.Register(() => new FooView(), typeof(IViewFor<FooViewModel>));

            using (resolver.WithResolver()) {
                var fixture = new DefaultViewLocator();
                FooViewModel vm = new FooViewModel();

                var result = fixture.ResolveView(vm);
                Assert.IsType<FooView>(result);
            }
        }

        [Fact]
        public void CanResolveViewFromViewModelInterfaceUsingClassRegistration()
        {
            var resolver = new ModernDependencyResolver();

            resolver.InitializeSplat();
            resolver.InitializeReactiveUI();
            resolver.Register(() => new FooView(), typeof(FooView));

            using (resolver.WithResolver()) {
                var fixture = new DefaultViewLocator();
                IFooViewModel vm = new FooViewModelWithWeirdName();

                var result = fixture.ResolveView(vm);
                Assert.IsType<FooView>(result);
            }
        }

        [Fact]
        public void CanResolveViewFromViewModelInterfaceUsingInterfaceRegistration()
        {
            var resolver = new ModernDependencyResolver();

            resolver.InitializeSplat();
            resolver.InitializeReactiveUI();
            resolver.Register(() => new FooView(), typeof(IFooView));

            using (resolver.WithResolver()) {
                var fixture = new DefaultViewLocator();
                IFooViewModel vm = new FooViewModel();

                var result = fixture.ResolveView(vm);
                Assert.IsType<FooView>(result);
            }
        }

        [Fact]
        public void CanResolveViewFromViewModelInterfaceUsingIViewForRegistration()
        {
            var resolver = new ModernDependencyResolver();

            resolver.InitializeSplat();
            resolver.InitializeReactiveUI();
            resolver.Register(() => new FooView(), typeof(IViewFor<IFooViewModel>));

            using (resolver.WithResolver()) {
                var fixture = new DefaultViewLocator();
                IFooViewModel vm = new FooViewModel();

                var result = fixture.ResolveView(vm);
                Assert.IsType<FooView>(result);
            }
        }

        [Fact]
        public void ContractIsUsedWhenResolvingView()
        {
            var resolver = new ModernDependencyResolver();

            resolver.InitializeSplat();
            resolver.InitializeReactiveUI();
            resolver.Register(() => new FooView(), typeof(IViewFor<IFooViewModel>), "first");
            resolver.Register(() => new FooWithWeirdConvention(), typeof(IViewFor<IFooViewModel>), "second");

            using (resolver.WithResolver()) {
                var fixture = new DefaultViewLocator();
                var vm = new FooViewModel();

                var result = fixture.ResolveView(vm);
                Assert.Null(result);

                result = fixture.ResolveView(vm, "first");
                Assert.IsType<FooView>(result);

                result = fixture.ResolveView(vm, "second");
                Assert.IsType<FooWithWeirdConvention>(result);
            }
        }

        [Fact]
        public void NoErrorIsRaisedIfATypeCannotBeFound()
        {
            var resolver = new ModernDependencyResolver();

            resolver.InitializeSplat();
            resolver.InitializeReactiveUI();

            using (resolver.WithResolver()) {
                var fixture = new DefaultViewLocator();
                fixture.ViewModelToViewFunc = viewModelName => "DoesNotExist";
                var vm = new FooViewModel();

                var result = fixture.ResolveView(vm);
                Assert.Null(result);
            }
        }

        [Fact]
        public void NoErrorIsRaisedIfAServiceCannotBeFound()
        {
            var resolver = new ModernDependencyResolver();

            resolver.InitializeSplat();
            resolver.InitializeReactiveUI();

            using (resolver.WithResolver()) {
                var fixture = new DefaultViewLocator();
                var vm = new FooViewModel();

                var result = fixture.ResolveView(vm);
                Assert.Null(result);
            }
        }

        [Fact]
        public void NoErrorIsRaisedIfTheServiceDoesNotImplementIViewFor()
        {
            var resolver = new ModernDependencyResolver();

            resolver.InitializeSplat();
            resolver.InitializeReactiveUI();
            resolver.Register(() => "this string does not implement IViewFor", typeof(IViewFor<IFooViewModel>));

            using (resolver.WithResolver()) {
                var fixture = new DefaultViewLocator();
                var vm = new FooViewModel();

                var result = fixture.ResolveView(vm);
                Assert.Null(result);
            }
        }

        [Fact]
        public void AnErrorIsRaisedIfTheCreationOfTheViewFails()
        {
            var resolver = new ModernDependencyResolver();

            resolver.InitializeSplat();
            resolver.InitializeReactiveUI();
            resolver.Register(() => new FooThatThrowsView(), typeof(IViewFor<IFooViewModel>));

            using (resolver.WithResolver()) {
                var fixture = new DefaultViewLocator();
                var vm = new FooViewModel();

                var ex = Assert.Throws<InvalidOperationException>(() => fixture.ResolveView(vm));
                Assert.Equal("This is a test failure.", ex.Message);
            }
        }
    }
}
