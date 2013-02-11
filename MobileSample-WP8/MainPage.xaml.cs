using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using MobileSample_WP8.Resources;
using MobileSample_WP8.ViewModels;
using ReactiveUI;

namespace MobileSample_WP8
{
    public partial class MainPage : PhoneApplicationPage, IViewFor<AppBootstrapper>
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            this.OneWayBind(ViewModel, x => x.Router, x => x.Router.Router);
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