using System;
using System.Reactive.Linq;
using System.Runtime.Serialization;
using ReactiveUI;
using ReactiveUI.Routing;
using ReactiveUI.Xaml;

namespace MobileSample_WinRT.ViewModels
{
    [DataContract]
    public class TestPage1ViewModel : ReactiveObject, IRoutableViewModel
    {
        public string UrlPathSegment { get { return "test1"; } }
        public IScreen HostScreen { get; private set; }

        public ReactiveCommand NavPage2 { get; protected set; }
        public ReactiveCommand NavPage3 { get; protected set; }

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

            // XXX: This is hella jank
            NavPage2 = new ReactiveCommand(screen.Router.Navigate.CanExecuteObservable);
            NavPage2.Select(_ => new TestPage2ViewModel(screen)).InvokeCommand(screen.Router.Navigate);

            NavPage3 = new ReactiveCommand(screen.Router.Navigate.CanExecuteObservable);
            NavPage3.Select(_ => new TestPage3ViewModel(screen)).InvokeCommand(screen.Router.Navigate);
        }
    }
}