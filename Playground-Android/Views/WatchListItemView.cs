using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.Content;
using Android.Views;
using Android.Widget;
using MobileSample_Android.ViewModels;
using ReactiveUI;

namespace MobileSample_Android.Views
{
    public class WatchListItemView : ReactiveViewHost<WatchListItemViewModel>
    {
        public WatchListItemView(WatchListItemViewModel viewModel, Context ctx, ViewGroup parent) : base(ctx, Resource.Layout.WatchListItem, parent)
        {
            ViewModel = viewModel;
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