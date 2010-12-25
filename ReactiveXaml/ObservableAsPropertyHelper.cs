using System;
using System.Linq;
using System.Concurrency;
using System.Diagnostics.Contracts;
using System.Linq.Expressions;

#if WINDOWS_PHONE
using Microsoft.Phone.Reactive;
#endif

namespace ReactiveXaml
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ObservableAsPropertyHelper<T> : IEnableLogger, IObservable<T>
    {
        T lastValue;
        Exception lastException;
        IObservable<T> source;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="observable"></param>
        /// <param name="onChanged"></param>
        /// <param name="initialValue"></param>
        /// <param name="scheduler"></param>
        public ObservableAsPropertyHelper(IObservable<T> observable, Action<T> onChanged, T initialValue = default(T), IScheduler scheduler = null)
        {
            Contract.Requires(observable != null);
            Contract.Requires(onChanged != null);

            scheduler = scheduler ?? RxApp.DeferredScheduler;
            lastValue = initialValue;

            source = observable.DistinctUntilChanged().ObserveOn(scheduler);
            source.Subscribe(x => {
                this.Log().InfoFormat("Property helper {0:X} changed", this.GetHashCode());
                lastValue = x;
                onChanged(x);
            }, ex => lastException = ex);
        }

        /// <summary>
        /// 
        /// </summary>
        public T Value {
            get {
                if (lastException != null) {
                    this.Log().Error("Observable ended with OnError", lastException);
                    throw lastException;
                }
                return lastValue;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="observer"></param>
        /// <returns></returns>
        public IDisposable Subscribe(IObserver<T> observer)
        {
            return source.Subscribe(observer);
        }
    }

    public static class OAPHCreationHelperMixin
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TObj"></typeparam>
        /// <typeparam name="TRet"></typeparam>
        /// <param name="This"></param>
        /// <param name="observable"></param>
        /// <param name="property"></param>
        /// <param name="initialValue"></param>
        /// <param name="scheduler"></param>
        /// <returns></returns>
        public static ObservableAsPropertyHelper<TRet> ObservableToProperty<TObj, TRet>(
            this TObj This,
            IObservable<TRet> observable,
            Expression<Func<TObj, TRet>> property,
            TRet initialValue = default(TRet),
            IScheduler scheduler = null)
            where TObj : ReactiveObject
        {
            Contract.Requires(This != null);
            Contract.Requires(observable != null);
            Contract.Requires(property != null);

            string prop_name = RxApp.expressionToPropertyName(property);
	        var ret = new ObservableAsPropertyHelper<TRet>(observable, _ => This.raisePropertyChanged(prop_name), initialValue, scheduler);
	        This.Log().InfoFormat("OAPH {0:X} is for {1}", ret, prop_name);
	        return ret;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TObj"></typeparam>
        /// <typeparam name="TRet"></typeparam>
        /// <param name="This"></param>
        /// <param name="source"></param>
        /// <param name="property"></param>
        /// <param name="initialValue"></param>
        /// <param name="scheduler"></param>
        /// <returns></returns>
        public static ObservableAsPropertyHelper<TRet> ToProperty<TObj, TRet>(
            this IObservable<TRet> This,
            TObj source,
            Expression<Func<TObj, TRet>> property,
            TRet initialValue = default(TRet),
            IScheduler scheduler = null)
            where TObj : ReactiveObject
        {
            return source.ObservableToProperty(This, property, initialValue, scheduler);
        }
    }
}

// vim: tw=120 ts=4 sw=4 et :
