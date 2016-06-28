using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.OS;
using Android.Widget;
using MobileSample_Android.ViewModels;
using MobileSample_Android.Views;
using ReactiveUI;
using Android.Views;

namespace MobileSample_Android
{
    //[Activity(Label = "AndroidPlayground", MainLauncher = true)]
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

            var adapter = new ReactiveListAdapter<WatchListItemViewModel>(
                ViewModel.Stocks,
                (viewModel, parent) => new WatchListItemView(viewModel, this, parent));

            WatchList.Adapter = adapter;

            this.BindCommand(ViewModel, vm => vm.OpenMarketCommand, c => c.OpenMarket);
            this.BindCommand(ViewModel, vm => vm.CloseMarketCommand, c => c.CloseMarket);
            this.BindCommand(ViewModel, vm => vm.ResetCommand, c => c.Reset);
        }
    }
}