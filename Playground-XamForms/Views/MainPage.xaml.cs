using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using ReactiveUI;
using Xamarin.Forms;
using System.Diagnostics;

namespace PlaygroundXamForms
{    
    public partial class MainPage : ContentPage, IViewFor<MainPageViewModel>
    {    
        public MainPage ()
        {
            InitializeComponent ();
            ViewModel = new MainPageViewModel();

            this.OneWayBind(ViewModel, x => x.SavedGuid, x => x.savedGuid.Text);
            this.BindCommand(ViewModel, x => x.DoIt, x => x.doIt);

            this.WhenAnyObservable(x => x.ViewModel.DoIt)
                .Subscribe(_ => {
                    Debug.WriteLine("Doin' it.");
                });
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

