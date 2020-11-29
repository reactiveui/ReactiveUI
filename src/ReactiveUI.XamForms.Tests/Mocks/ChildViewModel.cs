using Splat;

namespace ReactiveUI.XamForms.Tests.Mocks
{
    public class ChildViewModel : ReactiveObject, IRoutableViewModel
    {
        public ChildViewModel() => HostScreen = Locator.Current.GetService<IScreen>();

        public ChildViewModel(string value)
            : this() =>
            Value = value;

        public string? UrlPathSegment => "Child view: " + Value;

        public IScreen HostScreen { get; }

        public string Value { get; } = string.Empty;
    }
}
