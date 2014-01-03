using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.OS;
using Android.Widget;
using AndroidPlayground.ViewModels;
using AndroidPlayground.Views;
using ReactiveUI;
using ReactiveUI.Android;

namespace AndroidPlayground
{
    [Activity(Label = "AndroidPlayground", MainLauncher = true)]
    public class WatchListActivity : ReactiveActivity<WatchListViewModel>
    {
        public Button OpenMarket { get; private set; }
        public Button CloseMarket { get; private set; }
        public Button Reset { get; private set; }

        public ListView WatchList { get; private set; }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.WatchList);
            ViewModel = new WatchListViewModel();

            this.WireUpControls();

            var adapter = new ReactiveListAdapter<WatchListItemViewModel, WatchListItemView>(
                this,
                ViewModel.Stocks,
                c => new WatchListItemView(c),
                (viewModel, view) =>
                {
                    view.Bind(view.ViewModel, vm => vm.Symbol, v => v.Symbol.Text);
                    view.Bind(view.ViewModel, vm => vm.Price, v => v.Price.Text);
                    view.Bind(view.ViewModel, vm => vm.LastChange, v => v.LastChange.Text);
                    view.Bind(view.ViewModel, vm => vm.PercentChange, v => v.PercentChange.Text);
                    view.Bind(view.ViewModel, vm => vm.DayOpen, v => v.Open.Text);
                    view.Bind(view.ViewModel, vm => vm.DayHigh, v => v.High.Text);
                    view.Bind(view.ViewModel, vm => vm.DayLow, v => v.Low.Text);
                });
            WatchList.Adapter = adapter;

            this.BindCommand(ViewModel, vm => vm.OpenMarketCommand, c => c.OpenMarket);
            this.BindCommand(ViewModel, vm => vm.CloseMarketCommand, c => c.CloseMarket);
            this.BindCommand(ViewModel, vm => vm.ResetCommand, c => c.Reset);
        }
    }
}