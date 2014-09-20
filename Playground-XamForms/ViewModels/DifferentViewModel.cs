using System;
using ReactiveUI;
using Splat;

namespace PlaygroundXamForms
{
    public class DifferentViewModel : ReactiveObject, IRoutableViewModel
    {
        public string UrlPathSegment {
            get { return "Just a different screen"; }
        }

        public IScreen HostScreen { get; protected set; }

        public DifferentViewModel() : this(null) {}

        public DifferentViewModel(IScreen hostScreen = null)
        {
            HostScreen = hostScreen ?? Locator.Current.GetService<IScreen>();
        }
    }
}
