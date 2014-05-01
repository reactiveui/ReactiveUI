using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MobileSample_WinRT.ViewModels;
using ReactiveUI;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace MobileSample_WinRT
{
    public sealed partial class MainPage : Page, IViewFor<AppBootstrapper>
    {
        public MainPage()
        {
            this.InitializeComponent();
            this.OneWayBind(ViewModel, x => x.Router, x => x.Router.Router);

            // XXX: Why can't I dot into this with BindCommand
            this.OneWayBind(ViewModel, x => x.Router.NavigateBack, x => x.BackButton.Command);
        }

        public AppBootstrapper ViewModel {
            get { return (AppBootstrapper)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register("ViewModel", typeof(AppBootstrapper), typeof(MainPage), new PropertyMetadata(null));

        object IViewFor.ViewModel {
            get { return ViewModel; }
            set { ViewModel = (AppBootstrapper)value; }
        }
    }
}
