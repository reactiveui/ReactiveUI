using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReactiveUI;
using ReactiveUI.Routing;
using Xunit;

namespace Foobar.ViewModels
{
    public interface IFooBarViewModel : IRoutableViewModel {}

    public interface IBazViewModel : IRoutableViewModel {}

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
}

namespace ReactiveUI.Routing.Tests
{
    using Foobar.Views;
    using Foobar.ViewModels;

    public class RxRoutingTests : IEnableLogger
    {
        [Fact]
        public void ResolveExplicitViewType()
        {
            var resolver = new ModernDependencyResolver();
            resolver.Register(() => new BazView(), typeof(IBazView));

            using (resolver.WithResolver()) {
                var vm = new BazViewModel(null);

                var result = RxRouting.ResolveView(vm);
                this.Log().Info(result.GetType().FullName);
                Assert.True(result is BazView);
            }
        }
    }
}
