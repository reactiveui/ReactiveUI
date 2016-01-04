using ReactiveUI;
using Splat;
using Xunit;

namespace Foobar.ViewModels
{
    public interface IFooBarViewModel : IRoutableViewModel {}

    public interface IBazViewModel : IRoutableViewModel {}

    public interface IQuxViewModel : IRoutableViewModel { }

    public class FooBarViewModel : ReactiveObject, IFooBarViewModel 
    {
        public string UrlPathSegment { get { return "foo"; } }
        public IScreen HostScreen { get; private set; }

        public FooBarViewModel(IScreen hostScreen)
        {
            HostScreen = hostScreen;
        }
    }

    public class BazViewModel : ReactiveObject, IBazViewModel 
    {
        public string UrlPathSegment { get { return "foo"; } }
        public IScreen HostScreen { get; private set; }

        public BazViewModel(IScreen hostScreen)
        {
            HostScreen = hostScreen;
        }
    }

    public class QuxViewModel : ReactiveObject, IQuxViewModel
    {
        public string UrlPathSegment { get { return "foo"; } }
        public IScreen HostScreen { get; private set; }

        public QuxViewModel(IScreen hostScreen)
        {
            HostScreen = hostScreen;
        }
    }
}

namespace Foobar.Views
{
    using ViewModels;

    public class FooBarView : IViewFor<IFooBarViewModel> 
    {
        object IViewFor.ViewModel { get { return ViewModel; } set { ViewModel = (IFooBarViewModel) value; } }
        public IFooBarViewModel ViewModel { get; set; }
    }

    public interface IBazView : IViewFor<IBazViewModel> {}

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

namespace ReactiveUI.Routing.Tests
{
    using Foobar.ViewModels;
    using Foobar.Views;

    public class RxRoutingTests : IEnableLogger
    {
        [Fact]
        public void ResolveByInterfaceName()
        {
            var resolver = new ModernDependencyResolver();

            resolver.InitializeSplat();
            resolver.InitializeReactiveUI();
            resolver.Register(() => new BazView(), typeof(IBazView));

            using (resolver.WithResolver()) {
                var fixture = new DefaultViewLocator();
                var vm = new BazViewModel(null);

                var result = fixture.ResolveView(vm);
                this.Log().Info(result.GetType().FullName);
                Assert.IsType<BazView>(result);
            }
        }

        [Fact]
        public void ResolveByInterfaceType()
        {
            var resolver = new ModernDependencyResolver();

            resolver.InitializeSplat();
            resolver.InitializeReactiveUI();
            resolver.Register(() => new FooBarView(), typeof(IViewFor<IFooBarViewModel>));

            using (resolver.WithResolver()) {
                var fixture = new DefaultViewLocator();
                var vm = new FooBarViewModel(null);

                var result = fixture.ResolveView(vm);
                this.Log().Info(result.GetType().FullName);
                Assert.IsType<FooBarView>(result);
            }
        }

        [Fact]
        public void ResolveByConcreteViewFor()
        {
            var resolver = new ModernDependencyResolver();

            resolver.InitializeSplat();
            resolver.InitializeReactiveUI();
            resolver.Register(() => new QuxView(), typeof(IViewFor<QuxViewModel>));

            using (resolver.WithResolver()) {
                var fixture = new DefaultViewLocator();
                var vm = new QuxViewModel(null);

                var result = fixture.ResolveView(vm);
                this.Log().Info(result.GetType().FullName);
                Assert.IsType<QuxView>(result);
            }
        }
    }
}
