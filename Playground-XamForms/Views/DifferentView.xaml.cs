using System;
using System.Collections.Generic;
using Xamarin.Forms;
using ReactiveUI;

namespace PlaygroundXamForms
{
    public partial class DifferentView : ContentPage, IViewFor<DifferentViewModel>
    {
        public DifferentView ()
        {
            InitializeComponent ();

            this.BindCommand(ViewModel, x => x.HostScreen.Router.NavigateBack, x => x.NavigateBack);
        }

        /// <summary>
        /// The ViewModel to display
        /// </summary>
        public DifferentViewModel ViewModel {
            get { return (DifferentViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }
        public static readonly BindableProperty ViewModelProperty =
            BindableProperty.Create<DifferentView, DifferentViewModel>(x => x.ViewModel, default(DifferentViewModel), BindingMode.OneWay);

        object IViewFor.ViewModel {
            get { return ViewModel; }
            set { ViewModel = (DifferentViewModel)value; }
        }
    }
}
