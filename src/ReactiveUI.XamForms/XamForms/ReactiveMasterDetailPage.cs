using System;
using ReactiveUI;
using Xamarin.Forms;

namespace ReactiveUI.XamForms
{
    public class ReactiveMasterDetailPage<TViewModel> : MasterDetailPage, IViewFor<TViewModel>
        where TViewModel : class
    {
        /// <summary>
        /// The ViewModel to display
        /// </summary>
        public TViewModel ViewModel {
            get { return (TViewModel)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }
        public static readonly BindableProperty ViewModelProperty = BindableProperty.Create(
            nameof(ViewModel),
            typeof(TViewModel),
            typeof(ReactiveMasterDetailPage<TViewModel>),
            default(TViewModel),
            BindingMode.OneWay,
            propertyChanged: OnViewModelChanged);

        object IViewFor.ViewModel {
            get { return ViewModel; }
            set { ViewModel = (TViewModel)value; }
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            this.ViewModel = this.BindingContext as TViewModel;
        }

        private static void OnViewModelChanged(BindableObject bindableObject, object oldValue, object newValue)
        {
            bindableObject.BindingContext = newValue;
        }
    }
}