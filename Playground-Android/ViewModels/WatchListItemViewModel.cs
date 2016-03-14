using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using ReactiveUI;

namespace MobileSample_Android.ViewModels
{
    public class WatchListItemViewModel : ReactiveObject
    {
        private decimal price;
        private decimal dayOpen;
        private decimal dayLow;
        private decimal dayHigh;
        private decimal lastChange;

        private ObservableAsPropertyHelper<decimal> change;
        private ObservableAsPropertyHelper<double> percentChange; 

        public WatchListItemViewModel(string symbol)
        {
            Symbol = symbol;

            this.WhenAnyValue(v => v.Price, v => v.DayOpen, (p, o) => p - o)
                .ToProperty(this, v => v.Change, out change);

            this.WhenAnyValue(v => v.Change, v => v.Price, (c, p) => p != 0 ? (double)Math.Round(c / p, 4) : 0)
                .ToProperty(this, v => v.PercentChange, out percentChange);
        }

        public string Symbol { get; private set; }


        public decimal Change
        {
            get { return change.Value; }
        }

        public double PercentChange
        {
            get { return percentChange.Value; }
        }

        public decimal DayOpen
        {
            get { return dayOpen; }
            private set { this.RaiseAndSetIfChanged(ref dayOpen, value); }
        }

        public decimal DayLow
        {
            get { return dayLow; }
            private set { this.RaiseAndSetIfChanged(ref dayLow, value); }
        }

        public decimal DayHigh
        {
            get { return dayHigh; }
            private set { this.RaiseAndSetIfChanged(ref dayHigh, value); }
        }


        public decimal LastChange
        {
            get { return lastChange; }
            private set { this.RaiseAndSetIfChanged(ref lastChange, value); }
        }

        public decimal Price
        {
            get
            {
                return price;
            }
            set
            {
                if (price == value)
                {
                    return;
                }

                LastChange = value - price;
                price = value;

                if (DayOpen == 0)
                {
                    DayOpen = price;
                }
                if (price < DayLow || DayLow == 0)
                {
                    DayLow = price;
                }
                if (price > DayHigh)
                {
                    DayHigh = price;
                }

                this.RaisePropertyChanged();
            }
        }


    }
}