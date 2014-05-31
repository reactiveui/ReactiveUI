using System;
using ReactiveUI;

namespace PlaygroundXamForms
{
    public class MainPageViewModel : ReactiveObject
    {
        public MainPageViewModel()
        {
            SavedGuid = Guid.NewGuid();
        }

        Guid savedGuid;
        public Guid SavedGuid {
            get { return savedGuid; }
            set { this.RaiseAndSetIfChanged(ref savedGuid, value); }
        }
    }
}

