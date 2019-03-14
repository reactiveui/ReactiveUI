using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ReactiveUI.Samples.Routing.ViewModels;
using System.Threading.Tasks;

namespace ReactiveUI.Samples.Routing.Views
{
    /// <summary>
    /// Interaction logic for WelcomeView.xaml
    /// </summary>
    public partial class WelcomeView : IViewFor<IWelcomeViewModel>
    {
        public WelcomeView()
        {
            InitializeComponent();

            this.WhenActivated(d =>
            {
                d(this.WhenAnyValue(x => x.ViewModel).BindTo(this, x => x.DataContext));
                d(this.BindCommand(ViewModel, vm => vm.HelloWorld, view => view.helloWorldButton));
                d(this.BindCommand(ViewModel, vm => vm.NavigateToSecond, view => view.navigateButton));
            });
        }

        public IWelcomeViewModel ViewModel
        {
            get { return (IWelcomeViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(IWelcomeViewModel), typeof(WelcomeView), new PropertyMetadata(null));

        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (IWelcomeViewModel)value; }
        }
    }
}
