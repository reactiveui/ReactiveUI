using ReactiveUI;
using Splat;
using Xunit;

namespace Foobar.ViewModels
{
    public interface IFooBarViewModel : IRoutableViewModel { }

    public interface IBazViewModel : IRoutableViewModel { }

    public interface IQuxViewModel : IRoutableViewModel { }

    public class FooBarViewModel : ReactiveObject, IFooBarViewModel
    {
        public string UrlPathSegment => "foo";
        public IScreen HostScreen { get; }
    }

    public class BazViewModel : ReactiveObject, IBazViewModel
    {
        public string UrlPathSegment => "foo";
        public IScreen HostScreen { get; }
    }

    public class QuxViewModel : ReactiveObject, IQuxViewModel
    {
        public string UrlPathSegment => "foo";
        public IScreen HostScreen { get; }
    }
}

namespace Foobar.Views
{
    using ViewModels;

    public class FooBarView : IViewFor<IFooBarViewModel>
    {
        object IViewFor.ViewModel { get { return ViewModel; } set { ViewModel = (IFooBarViewModel)value; } }
        public IFooBarViewModel ViewModel { get; set; }
    }

    public interface IBazView : IViewFor<IBazViewModel> { }

    public class BazView : IBazView
    {
        object IViewFor.ViewModel { get { return ViewModel; } set { ViewModel = (IBazViewModel)value; } }
        public IBazViewModel ViewModel { get; set; }
    }

    public class QuxView : IViewFor<QuxViewModel>
    {
        object IViewFor.ViewModel { get { return ViewModel; } set { ViewModel = (QuxViewModel)value; } }
        public QuxViewModel ViewModel { get; set; }
    }
}

namespace ReactiveUI.Tests
{
    using Foobar.ViewModels;
    using Foobar.Views;

    public class DefaultViewLocatorTests
    {
        [Fact]
        public void ResolveByInterfaceName()
        {
            var resolver = new ModernDependencyResolver();
            resolver.Register(() => new BazView(), typeof(IBazView));

            using (resolver.WithResolver())
            {
                var fixture = new DefaultViewLocator();
                var vm = new BazViewModel();

                var result = fixture.ResolveView(vm);
                Assert.IsType<BazView>(result);
            }
        }

        [Fact]
        public void ResolveByInterfaceType()
        {
            var resolver = new ModernDependencyResolver();
            resolver.Register(() => new FooBarView(), typeof(IViewFor<IFooBarViewModel>));

            using (resolver.WithResolver())
            {
                var fixture = new DefaultViewLocator();
                var vm = new FooBarViewModel();

                var result = fixture.ResolveView(vm);
                Assert.IsType<FooBarView>(result);
            }
        }

        [Fact]
        public void ResolveByConcreteViewFor()
        {
            var resolver = new ModernDependencyResolver();
            resolver.Register(() => new QuxView(), typeof(IViewFor<QuxViewModel>));

            using (resolver.WithResolver())
            {
                var fixture = new DefaultViewLocator();
                var vm = new QuxViewModel();

                var result = fixture.ResolveView(vm);
                Assert.IsType<QuxView>(result);
            }
        }

        [Fact]
        public void ResolveViewForInterfaceViewModel()
        {
            var resolver = new ModernDependencyResolver();
            resolver.Register(() => new BazView(), typeof(IBazView));

            using (resolver.WithResolver())
            {
                var fixture = new DefaultViewLocator();
                IBazViewModel vm = new BazViewModel();

                var result = fixture.ResolveView(vm);
                Assert.IsType<BazView>(result);
            }
        }

        [Fact]
        public void ResolveUsesSpecifiedContract()
        {
            const string contract = "Contract";

            var resolver = new ModernDependencyResolver();
            resolver.Register(() => new BazView(), typeof(IBazView), contract);

            using (resolver.WithResolver())
            {
                var fixture = new DefaultViewLocator();
                var vm = new BazViewModel();

                var result = fixture.ResolveView(vm, contract);
                Assert.IsType<BazView>(result);
            }
        }

        [Fact]
        public void ResolveReturnsNullWhenContractHasNoMatchingRegistration()
        {
            const string contract = "Contract";

            var resolver = new ModernDependencyResolver();
            resolver.Register(() => new BazView(), typeof(IBazView));

            using (resolver.WithResolver())
            {
                var fixture = new DefaultViewLocator();
                var vm = new BazViewModel();

                var result = fixture.ResolveView(vm, contract);
                Assert.Null(result);
            }
        }
    }
}