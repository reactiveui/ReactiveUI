using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using ReactiveUI.UwpRouting.ViewModels;

namespace ReactiveUI.UwpRouting.Views
{
    public sealed partial class FirstView : UserControl, IViewFor<FirstViewModel>
    {
        public static readonly DependencyProperty ViewModelProperty = DependencyProperty
            .Register(nameof(ViewModel), typeof(FirstViewModel), typeof(FirstView), null);

        public FirstView()
        {
            InitializeComponent();
            this.WhenActivated(disposables => { });
        }

        public FirstViewModel ViewModel
        {
            get => (FirstViewModel)GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }

        object IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (FirstViewModel)value;
        }
    }
}
