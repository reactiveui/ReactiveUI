using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics.Contracts;
using System.Linq.Expressions;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace ReactiveUI
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
    public sealed class ObservableAsPropertyHelper<T> : IObservable<T>, IHandleObservableErrors, IDisposable, IEnableLogger
    {
        T _lastValue;
        readonly IObservable<T> _source;
        IDisposable _inner;

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
        public ObservableAsPropertyHelper(
            IObservable<T> observable, 
            Action<T> onChanged, 
            T initialValue = default(T), 
            IScheduler scheduler = null)
        {
            Contract.Requires(observable != null);
            Contract.Requires(onChanged != null);

            scheduler = scheduler ?? RxApp.DeferredScheduler;
            _lastValue = initialValue;

            var subj = new ScheduledSubject<T>(scheduler);
            var exSubject = new ScheduledSubject<Exception>(scheduler, RxApp.DefaultExceptionHandler);

            subj.Subscribe(x => {
                this.Log().Debug("Property helper {0:X} changed", this.GetHashCode());
                _lastValue = x;
                onChanged(x);
            }, exSubject.OnNext);

            ThrownExceptions = exSubject;

            // Fire off an initial RaisePropertyChanged to make sure bindings
            // have a value
            subj.OnNext(initialValue);

            var src = observable.DistinctUntilChanged().Multicast(subj);
            _inner = src.Connect();
            _source = src;
        }

        /// <summary>
        /// The last provided value from the Observable. 
        /// </summary>
        public T Value {
            get { return _lastValue; }
        }

        /// <summary>
        /// Fires whenever an exception would normally terminate ReactiveUI 
        /// internal state.
        /// </summary>
        public IObservable<Exception> ThrownExceptions { get; protected set; }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            return _source.Subscribe(observer);
        }

        public void Dispose()
        {
            _inner.Dispose();
            _inner = null;
        }

        /// <summary>
        /// Constructs a "default" ObservableAsPropertyHelper object. This is
        /// useful for when you will initialize the OAPH later, but don't want
        /// bindings to access a null OAPH at startup.
        /// </summary>
        /// <param name="initialValue">The initial (and only) value of the property.</param>
        /// <param name="scheduler">The scheduler that the notifications will be
        /// provided on - this should normally be a Dispatcher-based scheduler
        /// (and is by default)</param>
        public static ObservableAsPropertyHelper<T> Default(T initialValue = default(T), IScheduler scheduler = null)
        {
            return new ObservableAsPropertyHelper<T>(Observable.Never<T>(), _ => {}, initialValue, scheduler);
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

            string prop_name = Reflection.SimpleExpressionToPropertyName(property);
            var ret = new ObservableAsPropertyHelper<TRet>(observable, 
                _ => This.raisePropertyChanged(prop_name), 
                initialValue, scheduler);

            LogHost.Default.Debug("OAPH {0:X} is for {1}", ret, prop_name);
            return ret;
        }

        /// <summary>
        /// Converts an Observable to an ObservableAsPropertyHelper and
        /// automatically provides the onChanged method to raise the property
        /// changed notification.         
        /// </summary>
        /// <param name="source">The ReactiveObject that has the property</param>
        /// <param name="property">An Expression representing the property (i.e.
        /// 'x => x.SomeProperty'</param>
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
            IScheduler scheduler = null,
            bool setViaReflection = true)
            where TObj : ReactiveObject
        {
            var ret = source.ObservableToProperty(This, property, initialValue, scheduler);

            string propName = Reflection.SimpleExpressionToPropertyName(property);

            if (setViaReflection) {
                var fi = Reflection.GetBackingFieldInfoForProperty<TObj>(propName, true);
                if (fi != null) fi.SetValue(source, ret);
            }

            return ret;
        }
    }
}

// vim: tw=120 ts=4 sw=4 et :
