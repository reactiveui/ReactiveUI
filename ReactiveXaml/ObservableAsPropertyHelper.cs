using System;
using System.Linq;
using System.Concurrency;
using System.Diagnostics.Contracts;
using System.Linq.Expressions;

namespace ReactiveXaml
{
    /// <summary>
    /// ObservableAsPropertyHelper is a class to help ViewModels implement
    /// "output properties", that is, a property that is backed by an
    /// Observable. The property will be read-only, but will still fire change
    /// notifications. This class can be created directly, but is more often created via the
    /// ToProperty and ObservableToProperty extension methods.
    ///
    /// This class is also an Observable itself, so that output properties can
    /// be chained - for example a "Path" property and a chained
    /// "PathFileNameOnly" property.
    /// </summary>
    public class ObservableAsPropertyHelper<T> : IEnableLogger, IObservable<T>
    {
        T lastValue;
        Exception lastException;
        IObservable<T> source;

        /// <summary>
        /// Constructs an ObservableAsPropertyHelper object.
        /// </summary>
        /// <param name="observable">The Observable to base the property on.</param>
        /// <param name="onChanged">The action to take when the property
        /// changes, typically this will call the ViewModel's
        /// RaisePropertyChanged method.</param>
        /// <param name="initialValue">The initial value of the property.</param>
        /// <param name="scheduler">The scheduler that the notifications will be
        /// provided on - this should normally be a Dispatcher-based scheduler
        /// (and is by default)</param>
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
        /// The last provided value from the Observable. 
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

        public IDisposable Subscribe(IObserver<T> observer)
        {
            return source.Subscribe(observer);
        }
    }

    public static class OAPHCreationHelperMixin
    {
        /// <summary>
        /// Converts an Observable to an ObservableAsPropertyHelper and
        /// automatically provides the onChanged method to raise the property
        /// changed notification. The ToProperty method is semantically
        /// equivalent to this method and is often more convenient.
        /// </summary>
        /// <param name="observable">The Observable to base the property on.</param>
        /// <param name="property">An Expression representing the property (i.e.
        /// 'x => x.SomeProperty'</param>
        /// <param name="initialValue">The initial value of the property.</param>
        /// <param name="scheduler">The scheduler that the notifications will be
        /// provided on - this should normally be a Dispatcher-based scheduler
        /// (and is by default)</param>
        /// <returns>An initialized ObservableAsPropertyHelper; use this as the
        /// backing field for your property.</returns>
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
        /// <param name="This"></param>
        /// <param name="source"></param>
        /// <param name="property"></param>
        /// <param name="initialValue"></param>
        /// <param name="scheduler"></param>
        /// <param name="initialValue">The initial value of the property.</param>
        /// <param name="scheduler">The scheduler that the notifications will be
        /// provided on - this should normally be a Dispatcher-based scheduler
        /// (and is by default)</param>
        /// <returns>An initialized ObservableAsPropertyHelper; use this as the
        /// backing field for your property.</returns>
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
