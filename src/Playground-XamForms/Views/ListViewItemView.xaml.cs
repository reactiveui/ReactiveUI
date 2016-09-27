using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using Xamarin.Forms;
using ReactiveUI;

namespace PlaygroundXamForms
{
    public partial class ListViewItemView 
    {
        public ListViewItemView()
        {
            InitializeComponent();

            this.WhenActivated(d => {
                this.OneWayBind(ViewModel, vm => vm.Name, v => v.Name.Text).DisposeWith(d);
                this.OneWayBind(ViewModel, vm => vm.Race, v => v.Race.Text).DisposeWith(d);
             });


        }

        
    }
}
