using System;
using System.Runtime.Serialization;
using ReactiveUI;

namespace MobileSample_WP8.ViewModels
{
    [DataContract]
    public class TestPage1ViewModel : ReactiveObject, IRoutableViewModel
    {
        public string UrlPathSegment { get { return "test1"; } }
        public IScreen HostScreen { get; private set; }

        [DataMember]
        Guid _RandomGuid;
        public Guid RandomGuid {
            get { return _RandomGuid; }
            set { this.RaiseAndSetIfChanged(ref _RandomGuid, value); }
        }

        public TestPage1ViewModel(IScreen screen = null)
        {
            HostScreen = screen;
            RandomGuid = Guid.NewGuid();
        }
    }
}