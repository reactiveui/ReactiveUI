using System.Runtime.Serialization;

namespace ReactiveUI.Benchmarks
{
    [DataContract]
    public class MockViewModel : ReactiveObject, IRoutableViewModel
    {
        public IScreen HostScreen { get; }
        public string UrlPathSegment { get; }

        public MockViewModel() => HostScreen = new MockHostScreen();
    }
}
