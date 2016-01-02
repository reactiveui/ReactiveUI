﻿using ReactiveUI;
using Xamarin.Forms;

namespace ReactiveUI.XamForms
{
    public class ReactiveEntryCell<TViewModel> : EntryCell, IViewFor<TViewModel>
        where TViewModel : class
    {
        public static readonly BindableProperty ViewModelProperty = BindableProperty.Create<ReactiveEntryCell<TViewModel>, TViewModel>(
            x => x.ViewModel,
            null,
            BindingMode.OneWay);

        public TViewModel ViewModel
        {
            get { return (TViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        object IViewFor.ViewModel
        {
            get { return this.ViewModel; }
            set { this.ViewModel = (TViewModel)value; }
        }
    }
}