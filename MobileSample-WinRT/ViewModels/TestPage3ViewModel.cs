using System;
using System.Linq;
using System.Runtime.Serialization;
using ReactiveUI;
using Splat;

namespace MobileSample_WinRT.ViewModels
{
    public class StringTileViewModel : ReactiveObject
    {
        public string Model { get; protected set; }

        public StringTileViewModel(string someString)
        {
            Model = someString;
        }
    }

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

        public ReactiveList<string> ListOfStrings { get; protected set; }
        public IReactiveDerivedList<StringTileViewModel> ListOfTiles { get; protected set; }

        public ReactiveCommand PopulateList { get; protected set; }
        public ReactiveCommand ClearList { get; protected set; }

        public TestPage3ViewModel(IScreen screen = null)
        {
            HostScreen = screen ?? Locator.Current.GetService<IScreen>();
            RandomGuid = Guid.NewGuid();

            ListOfStrings = new ReactiveList<string>();
            ListOfTiles = ListOfStrings.CreateDerivedCollection(x => new StringTileViewModel(x));

            PopulateList = new ReactiveCommand();
            PopulateList.Subscribe(_ => ListOfStrings.AddRange(Enumerable.Range(0, 50).Select(__ => Guid.NewGuid().ToString())));

            ClearList = new ReactiveCommand();
            ClearList.Subscribe(_ => {
                ListOfStrings.Clear();
                GC.Collect(10, GCCollectionMode.Forced, true);
            });
        }
    }
}