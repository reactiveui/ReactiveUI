using System;
using System.Reactive;
using ReactiveUI;
using ReactiveCommand = ReactiveUI.ReactiveCommand;

namespace PlaygroundXamForms
{
    public class MainPageViewModel : ReactiveObject
    {
        public MainPageViewModel()
        {
            SavedGuid = Guid.NewGuid();

            DoIt = ReactiveCommand.Create(() => { });
        }

        Guid savedGuid;
        public Guid SavedGuid {
            get { return savedGuid; }
            set { this.RaiseAndSetIfChanged(ref savedGuid, value); }
        }

        public ReactiveCommand<Unit,Unit> DoIt { get; protected set; }
    }
}

