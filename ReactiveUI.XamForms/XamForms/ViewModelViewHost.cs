using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows;
using Splat;
using Xamarin.Forms;

namespace ReactiveUI.XamForms
{
    /// <summary>
    /// This content control will automatically load the View associated with
    /// the ViewModel property and display it. This control is very useful
    /// inside a DataTemplate to display the View associated with a ViewModel.
    /// </summary>
    public class ViewModelViewHost : StackLayout, IViewFor
    {
        /// <summary>
        /// The ViewModel to display
        /// </summary>
        public object ViewModel {
            get { return GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }
        public static readonly BindableProperty ViewModelProperty = 
            BindableProperty.Create<ViewModelViewHost, object>(x => x.ViewModel, null, BindingMode.OneWay);

        /// <summary>
        /// If no ViewModel is displayed, this content (i.e. a control) will be displayed.
        /// </summary>
        public View DefaultContent {
            get { return (View)GetValue(DefaultContentProperty); }
            set { SetValue(DefaultContentProperty, value); }
        }
        public static readonly BindableProperty DefaultContentProperty =
            BindableProperty.Create<ViewModelViewHost, View>(x => x.DefaultContent, null, BindingMode.OneWay);

        public IObservable<string> ViewContractObservable {
            get { return (IObservable<string>)GetValue(ViewContractObservableProperty); }
            set { SetValue(ViewContractObservableProperty, value); }
        }
        public static readonly BindableProperty ViewContractObservableProperty =
            BindableProperty.Create<ViewModelViewHost, IObservable<string>>(x => x.ViewContractObservable, Observable.Never<string>(), BindingMode.OneWay);

        public IViewLocator ViewLocator { get; set; }

        public ViewModelViewHost()
        {
            // NB: InUnitTestRunner also returns true in Design Mode
            if (ModeDetector.InUnitTestRunner()) {
                ViewContractObservable = Observable.Never<string>();
                return;
            }

            var vmAndContract = Observable.CombineLatest(
                this.WhenAnyValue(x => x.ViewModel),
                this.WhenAnyObservable(x => x.ViewContractObservable),
                (vm, contract) => new { ViewModel = vm, Contract = contract, });

            var platform = Locator.Current.GetService<IPlatformOperations>();
            if (platform == null) {
                throw new Exception("Couldn't find an IPlatformOperations. This should never happen, your dependency resolver is broken");
            }

            ViewContractObservable = Observable.FromEventPattern<EventHandler, EventArgs>(x => SizeChanged += x, x => SizeChanged -= x)
                .Select(_ => platform.GetOrientation())
                .DistinctUntilChanged()
                .StartWith(platform.GetOrientation())
                .Select(x => x != null ? x.ToString() : default(string));

            (this as IViewFor).WhenActivated(() => {
                return new[] { vmAndContract.Subscribe(x => {
                    if (x.ViewModel == null) {
                        foreach (View v in this.Children) this.Children.Remove(v);
                        if (DefaultContent != null) this.Children.Add(DefaultContent);
                        return;
                    }

                    var viewLocator = ViewLocator ?? ReactiveUI.ViewLocator.Current;
                    var view = viewLocator.ResolveView(x.ViewModel, x.Contract) ?? viewLocator.ResolveView(x.ViewModel, null);

                    if (view == null) {
                        throw new Exception(String.Format("Couldn't find view for '{0}'.", x.ViewModel));
                    }

                    view.ViewModel = x.ViewModel;

                    foreach (View v in this.Children) this.Children.Remove(v);
                    if (view != null) this.Children.Add(view as View);
                })};
            });
        }
    }
}
