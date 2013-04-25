using System;
using System.Reactive.Linq;
using System.Runtime.Serialization;
using ReactiveUI;
using ReactiveUI.Xaml;

namespace MobileSample_WinRT.ViewModels
{
    [DataContract]
    public class TestPage1ViewModel : ReactiveObject, IRoutableViewModel
    {
        public string UrlPathSegment { get { return "test1"; } }
        public IScreen HostScreen { get; private set; }

        public IReactiveCommand NavPage2 { get; protected set; }
        public IReactiveCommand NavPage3 { get; protected set; }

        [DataMember]
        Guid _RandomGuid;
        public Guid RandomGuid {
            get { return _RandomGuid; }
            set { this.RaiseAndSetIfChanged(ref _RandomGuid, value); }
        }

        public TestPage1ViewModel(IScreen screen = null)
        {
            HostScreen = screen ?? RxApp.DependencyResolver.GetService<IScreen>();
            RandomGuid = Guid.NewGuid();

            NavPage2 = HostScreen.Router.NavigateCommandFor<TestPage2ViewModel>();
            NavPage3 = HostScreen.Router.NavigateCommandFor<TestPage3ViewModel>();
        }
    }
}