using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ReactiveUI;
using ReactiveUI.Mobile;
using MobileSample_WinRT.Views;

namespace MobileSample_WinRT.ViewModels
{
    [DataContract]
    public class AppBootstrapper : ReactiveObject, IApplicationRootState
    {
        [DataMember] RoutingState _Router;

        public IRoutingState Router {
            get { return _Router; }
            set { _Router = (RoutingState) value; } // XXX: This is dumb.
        }

        public AppBootstrapper()
        {
            Router = new RoutingState();

            var resolver = RxApp.MutableResolver;

            resolver.Register(() => new TestPage1View(), typeof(IViewFor<TestPage1ViewModel>), "FullScreenLandscape");
            resolver.Register(() => new TestPage2View(), typeof(IViewFor<TestPage2ViewModel>), "FullScreenLandscape");
            resolver.Register(() => new TestPage3View(), typeof(IViewFor<TestPage3ViewModel>), "FullScreenLandscape");

            resolver.Register(() => new TestPage1ViewModel(), typeof(TestPage1ViewModel));
            resolver.Register(() => new TestPage2ViewModel(), typeof(TestPage2ViewModel));
            resolver.Register(() => new TestPage3ViewModel(), typeof(TestPage3ViewModel));

            resolver.RegisterConstant(this, typeof(IApplicationRootState));
            resolver.RegisterConstant(this, typeof(IScreen));
            resolver.RegisterConstant(new MainPage(), typeof(IViewFor), "InitialPage");

            Router.Navigate.Execute(new TestPage1ViewModel(this));
        }
    }
}
