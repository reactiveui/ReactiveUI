using System;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using ReactiveUI;
using ReactiveUI.XamForms;
using Xamarin.Forms;

namespace MasterDetail
{
    public partial class MainPage : ReactiveMasterDetailPage<MainViewModel>
    {
        public MainPage()
        {
            InitializeComponent();

            ViewModel = new MainViewModel();
            Detail = new NavigationPage(new MyDetailPage(ViewModel.Detail));

            this.WhenActivated(
                disposables =>
                {
                    this
                        .OneWayBind(ViewModel, vm => vm.MyList, v => v.MyListView.ItemsSource)
                        .DisposeWith(disposables);
                    this
                        .Bind(ViewModel, vm => vm.Selected, v => v.MyListView.SelectedItem)
                        .DisposeWith(disposables);
                    this
                        .WhenAnyValue(x => x.ViewModel.Selected)
                        .Where(x => x != null)
                        .Subscribe(
                            model =>
                            {
                                MyListView.SelectedItem = null;
                                IsPresented = false;
                            })
                        .DisposeWith(disposables);
                });
        }
    }
}
