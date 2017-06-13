using System;
using System.Reactive.Linq;
using Splat;
using Xamarin.Forms;

namespace ReactiveUI.XamForms
{
    /// <summary>
    /// This content view will automatically load and host the view for the given view model. The view model whose view is
    /// to be displayed should be assigned to the <see cref="ViewModel"/> property. Optionally, the chosen view can be
    /// customized by specifying a contract via <see cref="ViewContractObservable"/> or <see cref="ViewContract"/>.
    /// </summary>
    public class ViewModelViewHost : ContentView, IViewFor
    {
        /// <summary>
        /// The view model whose associated view is to be displayed.
        /// </summary>
        public object ViewModel
        {
            get { return GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="ViewModel"/> property.
        /// </summary>
        public static readonly BindableProperty ViewModelProperty = BindableProperty.Create(
            nameof(ViewModel),
            typeof(object),
            typeof(ViewModelViewHost),
            default(object),
            BindingMode.OneWay);

        /// <summary>
        /// The content to display when <see cref="ViewModel"/> is <see langword="null"/>.
        /// </summary>
        public View DefaultContent
        {
            get { return (View)GetValue(DefaultContentProperty); }
            set { SetValue(DefaultContentProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="DefaultContent"/> property.
        /// </summary>
        public static readonly BindableProperty DefaultContentProperty = BindableProperty.Create(
            nameof(DefaultContent),
            typeof(View),
            typeof(ViewModelViewHost),
            default(View),
            BindingMode.OneWay);

        /// <summary>
        /// The contract to use when resolving the view for the given view model.
        /// </summary>
        public IObservable<string> ViewContractObservable
        {
            get { return (IObservable<string>)GetValue(ViewContractObservableProperty); }
            set { SetValue(ViewContractObservableProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="ViewContractObservable"/> property.
        /// </summary>
        public static readonly BindableProperty ViewContractObservableProperty = BindableProperty.Create(
            nameof(ViewContractObservable),
            typeof(IObservable<string>),
            typeof(ViewModelViewHost),
            Observable<string>.Never,
            BindingMode.OneWay);

        private string viewContract;

        /// <summary>
        /// A fixed contract to use when resolving the view for the given view model.
        /// </summary>
        /// <remarks>
        /// This property is a mere convenience so that a fixed contract can be assigned directly in XAML.
        /// </remarks>
        public string ViewContract
        {
            get { return this.viewContract; }
            set { ViewContractObservable = Observable.Return(value); }
        }

        /// <summary>
        /// Can be used to override the view locator to use when resolving the view. If unspecified, <see cref="ViewLocator.Current"/> will be used.
        /// </summary>
        public IViewLocator ViewLocator { get; set; }

        public ViewModelViewHost()
        {
            // NB: InUnitTestRunner also returns true in Design Mode
            if (ModeDetector.InUnitTestRunner()) {
                ViewContractObservable = Observable<string>.Never;
                return;
            }

            ViewContractObservable = Observable<string>.Default;

            var vmAndContract = Observable.CombineLatest(
                this.WhenAnyValue(x => x.ViewModel),
                this.WhenAnyObservable(x => x.ViewContractObservable),
                (vm, contract) => new { ViewModel = vm, Contract = contract, });

            this.WhenActivated(() => {
                return new[] {
                    vmAndContract.Subscribe(x => {
                        this.viewContract = x.Contract;

                        if (x.ViewModel == null) {
                            this.Content = this.DefaultContent;
                            return;
                        }

                        var viewLocator = ViewLocator ?? ReactiveUI.ViewLocator.Current;
                        var view = viewLocator.ResolveView(x.ViewModel, x.Contract) ?? viewLocator.ResolveView(x.ViewModel, null);

                        if (view == null) {
                            throw new Exception(String.Format("Couldn't find view for '{0}'.", x.ViewModel));
                        }

                        var castView = view as View;

                        if (castView == null) {
                            throw new Exception(String.Format("View '{0}' is not a subclass of '{1}'.", view.GetType().FullName, typeof(View).FullName));
                        }

                        view.ViewModel = x.ViewModel;
                        this.Content = castView;
                    })};
            });
        }
    }
}