using Android.OS;
using Android.Widget;
using MobileSample_Android.ViewModels;
using MobileSample_Android.Views;
using ReactiveUI;
using Android.Support.V7.Widget;
using Android.App;

namespace MobileSample_Android
{
    //[Activity(Label = "AndroidPlayground", MainLauncher = true)]
    public class WatchListActivity : ReactiveActivity<WatchListViewModel>
    {
        public Button OpenMarket { get; private set; }
        public Button CloseMarket { get; private set; }
        public Button Reset { get; private set; }

        public RecyclerView WatchList { get; private set; }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.WatchList);
            ViewModel = new WatchListViewModel();

            this.WireUpControls();

            WatchList.SetLayoutManager(new LinearLayoutManager(this));

            var adapter = new WatchListItemViewAdapter(this.ViewModel.Stocks); 

            WatchList.SetAdapter(adapter);

            this.BindCommand(ViewModel, vm => vm.OpenMarketCommand, c => c.OpenMarket);
            this.BindCommand(ViewModel, vm => vm.CloseMarketCommand, c => c.CloseMarket);
            this.BindCommand(ViewModel, vm => vm.ResetCommand, c => c.Reset);
        }
    }
}