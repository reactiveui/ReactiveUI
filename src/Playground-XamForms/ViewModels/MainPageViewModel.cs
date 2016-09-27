using System;
using System.Reactive;
using System.Reactive.Disposables;
using ReactiveUI;
using Splat;
using ReactiveCommand = ReactiveUI.ReactiveCommand;

namespace PlaygroundXamForms
{
    public class MainPageViewModel : ReactiveObject, IRoutableViewModel, ISupportsActivation
    {

        public string UrlPathSegment { get; }
        public IScreen HostScreen { get; }


        public MainPageViewModel(IScreen screen = null)
        {
            HostScreen = screen ?? Locator.Current.GetService<IScreen>();

            SavedGuid = Guid.NewGuid();

            NavigateToListView = ReactiveCommand.CreateFromObservable(
                () => HostScreen.Router.Navigate.Execute(new DemoListViewViewModel(HostScreen)));


            //this.WhenActivated(d => 
            //{
            //    d(NavigateToListView = ReactiveCommand.CreateFromObservable(
            //        () => HostScreen.Router.Navigate.Execute(new DemoListViewViewModel(HostScreen))));
            //});

        }


        public ReactiveCommand NavigateToListView { get; protected set; }


        Guid savedGuid;
        private readonly ViewModelActivator activator = new ViewModelActivator();

        public Guid SavedGuid {
            get { return savedGuid; }
            set { this.RaiseAndSetIfChanged(ref savedGuid, value); }
        }


        ViewModelActivator ISupportsActivation.Activator
        {
            get { return activator; }
        }
    }
}

