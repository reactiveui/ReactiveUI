using Splat;

namespace ReactiveUI.XamForms.Tests.Mocks
{
    public class MainViewModel : ReactiveObject, IRoutableViewModel
    {
        public MainViewModel()
        {
            HostScreen = Locator.Current.GetService<IScreen>();
        }

        public string? UrlPathSegment => "Main view";

        public IScreen HostScreen { get; }
    }
}
