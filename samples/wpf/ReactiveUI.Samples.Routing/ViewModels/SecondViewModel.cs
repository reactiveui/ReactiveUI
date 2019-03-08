using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;

namespace ReactiveUI.Samples.Routing.ViewModels
{
    public class SecondViewModel : ReactiveObject, IRoutableViewModel
    {
        public SecondViewModel(IScreen hostScreen)
        {
            HostScreen = hostScreen;

            this.Back = HostScreen.Router.NavigateBack;
        }

        public string UrlPathSegment => "Second";
        public IScreen HostScreen { get; }

        public ReactiveCommand<Unit, Unit> Back { get; }
    }
}
