using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using ReactiveUI.UwpRouting.ViewModels;

namespace ReactiveUI.UwpRouting.Views
{
    public sealed partial class MainView : Page, IViewFor<MainViewModel>
    {
        public static readonly DependencyProperty ViewModelProperty = DependencyProperty
            .Register(nameof(ViewModel), typeof(MainViewModel), typeof(MainView), null);

        public MainView()
        {
            InitializeComponent();
            ViewModel = new MainViewModel();
            this.WhenActivated(disposables => { });
        }

        public MainViewModel ViewModel
        {
            get => (MainViewModel)GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }

        object IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (MainViewModel)value;
        }
    }
}
