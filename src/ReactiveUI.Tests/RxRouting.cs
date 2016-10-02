using System;
using ReactiveUI;
using Splat;
using Xunit;

namespace Foobar.ViewModels
{
    public interface IBazViewModel : IRoutableViewModel { }

    public interface IFooBarViewModel : IRoutableViewModel { }

    public class BazViewModel : ReactiveObject, IBazViewModel
    {
        public BazViewModel(IScreen hostScreen)
        {
            HostScreen = hostScreen;
        }

        public IScreen HostScreen { get; private set; }

        public string UrlPathSegment { get { return "foo"; } }
    }

    public class FooBarViewModel : ReactiveObject, IFooBarViewModel
    {
        public FooBarViewModel(IScreen hostScreen)
        {
            HostScreen = hostScreen;
        }

        public IScreen HostScreen { get; private set; }

        public string UrlPathSegment { get { return "foo"; } }
    }
}

namespace Foobar.Views
{
    using ViewModels;

    public interface IBazView : IViewFor<IBazViewModel> { }

    public class BazView : IBazView
    {
        object IViewFor.ViewModel { get { return ViewModel; } set { ViewModel = (IBazViewModel)value; } }

        public IBazViewModel ViewModel { get; set; }
    }

    public class FooBarView : IViewFor<IFooBarViewModel>
    {
        object IViewFor.ViewModel { get { return ViewModel; } set { ViewModel = (IFooBarViewModel)value; } }

        public IFooBarViewModel ViewModel { get; set; }
    }
}

namespace ReactiveUI.Routing.Tests
{
    using Foobar.ViewModels;
    using Foobar.Views;

    public class RxRoutingTests : IEnableLogger
    {
        [Fact]
        public void ResolveExplicitViewType()
        {
            var resolver = new ModernDependencyResolver();

            resolver.InitializeSplat();
            resolver.InitializeReactiveUI();
            resolver.Register(() => new BazView(), typeof(IBazView));

            using (resolver.WithResolver()) {
                var fixture = new DefaultViewLocator();
                IBazViewModel vm = new BazViewModel(null);

                var result = fixture.ResolveView(vm);
                this.Log().Info(result.GetType().FullName);
                Assert.True(result is BazView);
            }
        }
    }
}