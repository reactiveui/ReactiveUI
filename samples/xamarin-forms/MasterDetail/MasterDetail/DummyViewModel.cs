using System.Reactive;
using System.Reactive.Linq;
using ReactiveUI;
using Splat;

namespace MasterDetail
{
    public class DummyViewModel : ReactiveObject, IRoutableViewModel
    {
        public DummyViewModel(IScreen hostScreen = null)
        {
            HostScreen = hostScreen ?? Locator.Current.GetService<IScreen>();

            NavigateToDummyPage = ReactiveCommand.CreateFromObservable(
                () => HostScreen.Router.Navigate.Execute(new DummyViewModel()).Select(_ => Unit.Default));
        }

        public ReactiveCommand<Unit, Unit> NavigateToDummyPage { get; }

        public ReactiveCommand<Unit, Unit> NavigateBack => HostScreen.Router.NavigateBack;

        public string UrlPathSegment => "Dummy Page";

        public IScreen HostScreen { get; }
    }
}
