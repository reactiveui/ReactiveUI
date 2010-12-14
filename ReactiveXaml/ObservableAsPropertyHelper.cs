using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Concurrency;
using System.Windows.Threading;
using System.Diagnostics.Contracts;
using System.Linq.Expressions;

#if WINDOWS_PHONE
using Microsoft.Phone.Reactive;
#endif

namespace ReactiveXaml
{
    public class ObservableAsPropertyHelper<T> : IEnableLogger, IObservable<T>
    {
        T lastValue;
        Exception lastException;
        IObservable<T> source;

        public ObservableAsPropertyHelper(IObservable<T> observable, Action<T> on_changed, T initial_value = default(T), IScheduler scheduler = null)
        {
            Contract.Requires(observable != null);
            Contract.Requires(on_changed != null);

            scheduler = scheduler ?? RxApp.DeferredScheduler;
            lastValue = initial_value;

            source = observable.DistinctUntilChanged().ObserveOn(scheduler);
            source.Subscribe(x => {
                this.Log().InfoFormat("Property helper {0:X} changed", this.GetHashCode());
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

        public IDisposable Subscribe(IObserver<T> observer)
        {
            return source.Subscribe(observer);
        }
    }

    public static class OAPHCreationHelperMixin
    {
        public static ObservableAsPropertyHelper<TRet> ObservableToProperty<TObj, TRet>(
            this TObj This,
            IObservable<TRet> Observable,
            Expression<Func<TObj, TRet>> Property,
            TRet InitialValue = default(TRet),
            IScheduler Scheduler = null)
            where TObj : ReactiveObject
        {
            Contract.Requires(This != null);
            Contract.Requires(Observable != null);
            Contract.Requires(Property != null);

            string prop_name = RxApp.expressionToPropertyName(Property);
	        var ret = new ObservableAsPropertyHelper<TRet>(Observable, _ => This.RaisePropertyChanged(prop_name), InitialValue, Scheduler);
	        This.Log().InfoFormat("OAPH {0:X} is for {1}", ret, prop_name);
	        return ret;
        }

        public static ObservableAsPropertyHelper<TRet> ToProperty<TObj, TRet>(
            this IObservable<TRet> Observable,
            TObj Object,
            Expression<Func<TObj, TRet>> Property,
            TRet InitialValue = default(TRet),
            IScheduler Scheduler = null)
            where TObj : ReactiveObject
        {
            return Object.ObservableToProperty(Observable, Property, InitialValue, Scheduler);
        }
    }
}

// vim: tw=120 ts=4 sw=4 et :
