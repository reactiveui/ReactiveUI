# Design Time

Views/AboutView.xaml:


    <Page
        x:Class="MyCoolApp.UWP.Views.AboutView"

        xmlns:designTime="using:MyCoolApp.UWP.DesignTime"
        d:DataContext="{d:DesignInstance designTime:DesignTimeAboutViewModel,
                                                  IsDesignTimeCreatable=True}"
        d:DesignHeight="600"
        d:DesignWidth="600"



DesignTime/DesignTimeAboutViewModel.cs:

    using System.Collections.ObjectModel;
    using System.Threading.Tasks;
    using MyCoolApp.Core.ViewModels;
    using ReactiveUI;
    using MyCoolApp.Core.ServiceModel;

    namespace MyCoolApp.UWP.DesignTime
    {
        public class DesignTimeAboutViewModel : AboutViewModel
        {
            public DesignTimeAboutViewModel()
            {
                AboutSections = new ObservableCollection<AboutSectionViewModel>(new Collection<AboutSectionViewModel>
                {
                    new AboutSectionViewModel {Title = "Title 1", Body = "Lorum Ipsum"},
                    new AboutSectionViewModel {Title = "Title 2", Body = "Lorum Ipsum"},
                    new AboutSectionViewModel {Title = "Title 3", Body = "Lorum Ipsum"}
                });

                RefreshCommand = ReactiveCommand.CreateAsyncTask(o => Task.FromResult(new AboutFeed()));
            }
        }
    }
    
  ViewModels/IAboutViewModel.cs:

    public interface IAboutViewModel : IRoutableViewModel
    {
        ObservableCollection<AboutSectionViewModel> AboutSections { get; set; }

        ReactiveCommand<AboutFeed> RefreshCommand { get; set; }
    }

ViewModels/IAboutSectionViewModel.cs:

    public interface IAboutSectionViewModel
    {
        string Title { get; set; }
        string Body { get; set; }
    }
