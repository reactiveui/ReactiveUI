using System;
using System.Reactive;
using System.Reactive.Concurrency;
using ReactiveUI;

namespace Cinephile.ViewModels
{
    public class AboutViewModel : ViewModelBase
    {
        public AboutViewModel(IScheduler mainThreadScheduler = null, IScheduler taskPoolScheduler = null, IScreen hostScreen = null)
            : base("About", mainThreadScheduler, taskPoolScheduler, hostScreen)
        {
            OpenBrowser = ReactiveCommand.CreateFromObservable<string, Unit>(count => _movieService.LoadUpcomingMovies(count));
        }
    }
}
