using System.Runtime.Serialization;

namespace ReactiveUI.Benchmarks
{
    /// <summary>
    /// A mock for a ReactiveObject which is routable.
    /// </summary>
    [DataContract]
    public class MockViewModel : ReactiveObject, IRoutableViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MockViewModel"/> class.
        /// </summary>
        public MockViewModel() => HostScreen = new MockHostScreen();

        /// <summary>
        /// Gets the main host screen.
        /// </summary>
        public IScreen HostScreen { get; }

        /// <summary>
        /// Gets the url path. This is not being used in this version.
        /// </summary>
        public string UrlPathSegment { get; }
    }
}
