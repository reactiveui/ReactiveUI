using System;
using Xamarin.Forms;
using ReactiveUI;
using System.Reactive.Subjects;
using System.Reactive;

namespace ReactiveUI.XamForms
{
    public class ReactiveNavigationPage<TViewModel> : NavigationPage, IViewFor<TViewModel>
        where TViewModel : class
    {
        /// <summary>
        /// The ViewModel to display
        /// </summary>
        public TViewModel ViewModel {
            get { return (TViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }
        public static readonly BindableProperty ViewModelProperty = 
            BindableProperty.Create<ReactiveNavigationPage<TViewModel>, TViewModel>(x => x.ViewModel, default(TViewModel), BindingMode.OneWay);

        object IViewFor.ViewModel {
            get { return ViewModel; }
            set { ViewModel = (TViewModel)value; }
        }
    }
}