using System;
using System.Collections.Generic;
using Xamarin.Forms;
using ReactiveUI.XamForms;
using ReactiveUI;
using System.Reactive.Linq;

namespace PlaygroundXamForms
{
    public partial class TestView : ContentPage, IViewFor<TestViewModel>
    {
        public TestView ()
        {
            InitializeComponent();

            this.OneWayBind(ViewModel, x => x.TheGuid, x => x.TheGuid.Text);

            this.WhenAnyValue(x => x.ViewModel.HostScreen.Router)
                .Select(x => x.NavigateCommandFor<DifferentViewModel>())
                .BindTo(this, x => x.NavigateToDifferentView.Command);
        }

        /// <summary>
        /// The ViewModel to display
        /// </summary>
        public TestViewModel ViewModel {
            get { return (TestViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }
        public static readonly BindableProperty ViewModelProperty =
            BindableProperty.Create<TestView, TestViewModel>(x => x.ViewModel, default(TestViewModel), BindingMode.OneWay);

        object IViewFor.ViewModel {
            get { return ViewModel; }
            set { ViewModel = (TestViewModel)value; }
        }
    }
}