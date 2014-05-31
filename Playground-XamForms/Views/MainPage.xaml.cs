using System;
using System.Collections.Generic;
using ReactiveUI;
using Xamarin.Forms;

namespace PlaygroundXamForms
{    
    public partial class MainPage : ContentPage, IViewFor<MainPageViewModel>
    {    
        public MainPage ()
        {
            InitializeComponent ();
            ViewModel = new MainPageViewModel();

            this.OneWayBind(ViewModel, x => x.SavedGuid, x => x.savedGuid.Text);
        }

        public static readonly BindableProperty ViewModelProperty = BindableProperty.Create<MainPage, MainPageViewModel>(
            x => x.ViewModel, null, BindingMode.OneWay);

        public MainPageViewModel ViewModel {
            get { return (MainPageViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        object IViewFor.ViewModel {
            get { return ViewModel; }
            set { ViewModel = (MainPageViewModel)value; }
        }
    }
}

