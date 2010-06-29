using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Concurrency;
using System.Windows.Threading;

namespace ReactiveXaml
{
    public class ObservableAsPropertyHelper<T> : IEnableLogger
    {
        T lastValue;
        Exception lastException;

        public ObservableAsPropertyHelper(IObservable<T> observable, Action<T> on_changed, T initial_value = default(T), IScheduler scheduler = null)
        {
            scheduler = scheduler ?? ReactiveXaml.DefaultScheduler;
            lastValue = initial_value;

            observable.ObserveOn(scheduler)
                      .DistinctUntilChanged()
                      .Subscribe(x => {
                this.Log().DebugFormat("Property helper {0:X} changed", this.GetHashCode());
                lastValue = x;
                on_changed(x);
            }, ex => lastException = ex);
        }

        public T Value {
            get {
                if (lastException != null) {
                    this.Log().Error("Observable ended with OnError", lastException);
                    throw lastException;
                }
                return lastValue;
            }
        }
    }
}