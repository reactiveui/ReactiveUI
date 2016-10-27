using ReactiveUI;
using Xamarin.Forms;

namespace ReactiveUI.XamForms
{
    public class ReactiveImageCell<TViewModel> : ImageCell, IViewFor<TViewModel>
        where TViewModel : class
    {
        public static readonly BindableProperty ViewModelProperty = BindableProperty.Create(
            nameof(ViewModel),
            typeof(TViewModel),
            typeof(ReactiveImageCell<TViewModel>),
            default(TViewModel),
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