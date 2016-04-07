using Android.Views;
using Android.Widget;
using MobileSample_Android.ViewModels;
using ReactiveUI;
using ReactiveUI.Android.Support;
using Android.Support.V7.Widget;

namespace MobileSample_Android.Views
{
    public class WatchListItemViewAdapter : ReactiveRecyclerViewAdapter<WatchListItemViewModel>
    {
        public WatchListItemViewAdapter(IReadOnlyReactiveList<WatchListItemViewModel> backingList)
            : base(backingList)
        {
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View itemView = LayoutInflater.FromContext(parent.Context).Inflate(Resource.Layout.WatchListItem, parent, false);

            return new WatchListItemViewHolder(itemView);
        }
    }

    public class WatchListItemViewHolder : ReactiveRecyclerViewViewHolder<WatchListItemViewModel>
    {
        public WatchListItemViewHolder(View view) : base(view)
        {
            this.WireUpControls();

            this.Bind(ViewModel, vm => vm.Symbol, v => v.Symbol.Text);
            this.OneWayBind(ViewModel, vm => vm.Price, v => v.Price.Text, v => string.Format("{0:0.00}", v));
            this.OneWayBind(ViewModel, vm => vm.LastChange, v => v.LastChange.Text, v => string.Format("{0:0.00}", v));
            this.OneWayBind(ViewModel, vm => vm.PercentChange, v => v.PercentChange.Text, v => string.Format("{0:P2}", v));
            this.OneWayBind(ViewModel, vm => vm.DayOpen, v => v.Open.Text, v => string.Format("{0:0.00}", v));
            this.OneWayBind(ViewModel, vm => vm.DayHigh, v => v.High.Text, v => string.Format("{0:0.00}", v));
            this.OneWayBind(ViewModel, vm => vm.DayLow, v => v.Low.Text, v => string.Format("{0:0.00}", v));
        }

        public TextView Symbol { get; private set; }
        public TextView Price { get; private set; }
        public TextView LastChange { get; private set; }
        public TextView PercentChange { get; private set; }
        public TextView Open { get; private set; }
        public TextView High { get; private set; }
        public TextView Low { get; private set; }
    }
}