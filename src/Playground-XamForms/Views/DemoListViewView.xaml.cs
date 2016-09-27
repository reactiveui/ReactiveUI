using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using Xamarin.Forms;
using ReactiveUI;

namespace PlaygroundXamForms
{
    public partial class DemoListViewView
    {
        public DemoListViewView ()
        {

            InitializeComponent ();

            this.WhenActivated(d => 
            {
                this.OneWayBind(ViewModel, vm => vm.DogViewModelList, v => v.DogListView.ItemsSource)
                    .DisposeWith(d);
            });
        }

    }
}
