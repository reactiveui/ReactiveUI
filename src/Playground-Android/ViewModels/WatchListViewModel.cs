using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Windows.Input;
using ReactiveUI;

namespace MobileSample_Android.ViewModels
{
    // borrowed from https://github.com/SignalR/SignalR-StockTicker/blob/master/SignalR.StockTicker/SignalR.StockTicker/SignalR.StockTicker/StockTicker.cs

    public class WatchListViewModel : ReactiveObject
    {
        private readonly object marketStateLock = new object();
        private readonly double rangePercent = 0.002;
        private readonly ReactiveList<WatchListItemViewModel> stocks = new ReactiveList<WatchListItemViewModel>();


        private readonly TimeSpan updateInterval = TimeSpan.FromMilliseconds(250);
        private readonly Random updateOrNotRandom = new Random();
        private readonly object updateStockPricesLock = new object();
        private volatile MarketState marketState = MarketState.Closed;

        private IDisposable timer;
        private volatile bool updatingStockPrices;

        public ICommand OpenMarketCommand { get; private set; }
        public ICommand CloseMarketCommand { get; private set; }
        public ICommand ResetCommand { get; private set; }

        public WatchListViewModel()
        {
            var openCmd = ReactiveCommand.CreateAsyncObservable(this.WhenAnyValue(vm => vm.MarketState, m => m == MarketState.Closed),
                _ => Observable.Start(OpenMarket), RxApp.MainThreadScheduler);
            OpenMarketCommand = openCmd;

            var closeCmd = ReactiveCommand.CreateAsyncObservable(
                this.WhenAnyValue(vm => vm.MarketState, m => m == MarketState.Open),
                _ => Observable.Start(CloseMarket), RxApp.MainThreadScheduler);
            CloseMarketCommand = closeCmd;

            var resetCmd = ReactiveCommand.CreateAsyncObservable(
                this.WhenAnyValue(vm => vm.MarketState, m => m == MarketState.Closed),
                _ => Observable.Start(Reset), RxApp.MainThreadScheduler);
            ResetCommand = resetCmd;

            LoadDefaultStocks();
        }

        public IReadOnlyReactiveList<WatchListItemViewModel> Stocks
        {
            get { return stocks; }
        }

        public MarketState MarketState
        {
            get { return marketState; }
            private set
            {
                marketState = value;
                this.RaisePropertyChanged(); // can't use ref as it's volatile
            }
        }

        private bool TryUpdateStockPrice(WatchListItemViewModel stock)
        {
            // Randomly choose whether to udpate this stock or not
            var r = updateOrNotRandom.NextDouble();
            if (r > 0.1)
            {
                return false;
            }

            // Update the stock price by a random factor of the range percent
            var random = new Random((int)Math.Floor(stock.Price));
            var percentChange = random.NextDouble()*rangePercent;
            var pos = random.NextDouble() > 0.51;
            var change = Math.Round(stock.Price*(decimal)percentChange, 2);
            change = pos ? change : -change;

            stock.Price += change;
            return true;
        }

        private void UpdateStockPrices()
        {
            // This function must be re-entrant as it's running as a timer interval handler
            lock (updateStockPricesLock)
            {
                if (!updatingStockPrices)
                {
                    updatingStockPrices = true;


                    foreach (var stock in stocks)
                    {
                        if (TryUpdateStockPrice(stock))
                        {
                            //BroadcastStockPrice(stock);
                        }
                    }


                    updatingStockPrices = false;
                }
            }
        }

        public void OpenMarket()
        {
            lock (marketStateLock)
            {
                if (MarketState != MarketState.Open)
                {
                    timer = Observable.Timer(updateInterval, updateInterval, RxApp.MainThreadScheduler)
                                      .Subscribe(_ => UpdateStockPrices());
                    MarketState = MarketState.Open;
                }
            }
        }

        public void CloseMarket()
        {
            lock (marketStateLock)
            {
                if (MarketState == MarketState.Open)
                {
                    if (timer != null)
                    {
                        timer.Dispose();
                    }


                    MarketState = MarketState.Closed;
                }
            }
        }

        public void Reset()
        {
            lock (marketStateLock)
            {
                if (MarketState != MarketState.Closed)
                {
                    throw new InvalidOperationException("Market must be closed before it can be reset.");
                }

                LoadDefaultStocks();
            }
        }

        private void LoadDefaultStocks()
        {
            using (stocks.SuppressChangeNotifications())
            {
                stocks.Clear();
                stocks.Add(new WatchListItemViewModel("MSFT") {Price = 36.91m});
                stocks.Add(new WatchListItemViewModel("AAPL") {Price = 545.09m});
                stocks.Add(new WatchListItemViewModel("GOOG") {Price = 1107.32m});
                stocks.Add(new WatchListItemViewModel("FB") {Price = 54.77m});
            }
        }
    }

    public enum MarketState
    {
        Closed,
        Open
    }
}