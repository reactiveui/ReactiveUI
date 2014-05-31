using System;
using ReactiveUI;

namespace PlaygroundXamForms
{
    public class MainPageViewModel : ReactiveObject
    {
        public MainPageViewModel()
        {
            SavedGuid = Guid.NewGuid();

            DoIt = ReactiveCommand.Create();
        }

        Guid savedGuid;
        public Guid SavedGuid {
            get { return savedGuid; }
            set { this.RaiseAndSetIfChanged(ref savedGuid, value); }
        }

        public ReactiveCommand<Object> DoIt { get; protected set; }
    }
}

