using System;
using System.Reactive;
using System.Reactive.Concurrency;
using ReactiveUI;

namespace Cinephile.ViewModels
{
    public class AboutViewModel : ViewModelBase
    {
        public ReactiveCommand<string, Unit> ShowIconCredits
        {
            get;
        }

        public AboutViewModel(  IScheduler mainThreadScheduler = null, 
                                IScheduler taskPoolScheduler = null, 
                                IScreen hostScreen = null)
            : base("About", mainThreadScheduler, taskPoolScheduler, hostScreen)
        {
            ShowIconCredits = ReactiveCommand.CreateFromObservable<string, Unit>(url => OpenBrowser.Handle(url));
            ShowIconCredits.Subscribe();
        }
    }
}
