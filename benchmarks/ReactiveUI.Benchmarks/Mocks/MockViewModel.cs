namespace ReactiveUI.Benchmarks
{
    public class MockViewModel : ReactiveObject, IRoutableViewModel
    {
        public IScreen HostScreen { get; }
        public string UrlPathSegment { get; }

        public MockViewModel() => HostScreen = new MockHostScreen();
    }
}
