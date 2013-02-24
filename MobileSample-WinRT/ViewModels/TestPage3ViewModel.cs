using System;
using System.Runtime.Serialization;
using ReactiveUI;
using ReactiveUI.Routing;

namespace MobileSample_WinRT.ViewModels
{
    [DataContract]
    public class TestPage3ViewModel : ReactiveObject, IRoutableViewModel
    {
        public string UrlPathSegment { get { return "test3"; } }
        public IScreen HostScreen { get; private set; }

        [DataMember]
        Guid _RandomGuid;
        public Guid RandomGuid {
            get { return _RandomGuid; }
            set { this.RaiseAndSetIfChanged(ref _RandomGuid, value); }
        }

        public TestPage3ViewModel(IScreen screen = null)
        {
            HostScreen = screen;
            RandomGuid = Guid.NewGuid();
        }
    }
}