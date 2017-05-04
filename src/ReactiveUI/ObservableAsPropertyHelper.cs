using System;
using System.Diagnostics.Contracts;
using System.Linq.Expressions;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using Splat;

namespace ReactiveUI
{
    /// <summary>
    /// ObservableAsPropertyHelper is a class to help ViewModels implement
    /// "output properties", that is, a property that is backed by an
    /// Observable. The property will be read-only, but will still fire change
    /// notifications. This class can be created directly, but is more often created 
    /// via the <see cref="OAPHCreationHelperMixin" /> extension methods.    
    /// </summary>
    public sealed class ObservableAsPropertyHelper<T> : IHandleObservableErrors, IDisposable, IEnableLogger
    {
        T _lastValue;
        readonly IConnectableObservable<T> _source;
        IDisposable _inner;
        private int _activated;

        /// <summary>
        /// Constructs an ObservableAsPropertyHelper object.
        /// </summary>
        /// <param name="observable">
        /// The Observable to base the property on.
        /// </param>
        /// <param name="onChanged">
        /// The action to take when the property changes, typically this will call the 
        /// ViewModel's RaisePropertyChanged method.
        /// </param>
        /// <param name="initialValue">
        /// The initial value of the property.
        /// </param>
        /// <param name="deferSubscription">
        /// A value indicating whether the <see cref="ObservableAsPropertyHelper{T}"/> 
        /// should defer the subscription to the <paramref name="observable"/> source 
        /// until the first call to <see cref="Value"/>, or if it should immediately 
        /// subscribe to the the <paramref name="observable"/> source.
        /// </param>
        /// <param name="scheduler">
        /// The scheduler that the notifications will be provided on - 
        /// this should normally be a Dispatcher-based scheduler
        /// </param>
        public ObservableAsPropertyHelper(
            IObservable<T> observable,
            Action<T> onChanged,
            T initialValue = default(T),
            bool deferSubscription = false,
            IScheduler scheduler = null) : this(observable, onChanged, null, initialValue, deferSubscription, scheduler) { }

        /// <summary>
        /// Constructs an ObservableAsPropertyHelper object.
        /// </summary>
        /// <param name="observable">
        /// The Observable to base the property on.
        /// </param>
        /// <param name="onChanged">
        /// The action to take when the property changes, typically this will call 
        /// the ViewModel's RaisePropertyChanged method.
        /// </param>
        /// <param name="onChanging">
        /// The action to take when the property changes, typically this will call 
        /// the ViewModel's RaisePropertyChanging method.
        /// </param>
        /// <param name="initialValue">
        /// The initial value of the property.
        /// </param>
        /// <param name="deferSubscription">
        /// A value indicating whether the <see cref="ObservableAsPropertyHelper{T}"/> 
        /// should defer the subscription to the <paramref name="observable"/> source 
        /// until the first call to <see cref="Value"/>, or if it should immediately 
        /// subscribe to the the <paramref name="observable"/> source.
        /// </param>
        /// <param name="scheduler">
        /// The scheduler that the notifications will provided on - this 
        /// should normally be a Dispatcher-based scheduler
        /// </param>
        public ObservableAsPropertyHelper(
            IObservable<T> observable,
            Action<T> onChanged,
            Action<T> onChanging = null,
            T initialValue = default(T),
            bool deferSubscription = false,
            IScheduler scheduler = null)
        {
            Contract.Requires(observable != null);
            Contract.Requires(onChanged != null);

            scheduler = scheduler ?? CurrentThreadScheduler.Instance;
            onChanging = onChanging ?? (_ => { });

            var subj = new ScheduledSubject<T>(scheduler);
            var exSubject = new ScheduledSubject<Exception>(CurrentThreadScheduler.Instance, RxApp.DefaultExceptionHandler);

            subj.Subscribe(x => {
                onChanging(x);
                _lastValue = x;
                onChanged(x);
            }, exSubject.OnNext);

            ThrownExceptions = exSubject;

            _lastValue = initialValue;
            _source = observable.StartWith(initialValue).DistinctUntilChanged().Multicast(subj);
            if (!deferSubscription) {
                _inner = _source.Connect();
                _activated = 1;
            }
        }

        /// <summary>
        /// The last provided value from the Observable. 
        /// </summary>
        public T Value
        {
            get
            {
                if (Interlocked.CompareExchange(ref _activated, 1, 0) == 0) {
                    _inner = _source.Connect();
                }

                return _lastValue;
            }
        }

        /// <summary>
        /// Fires whenever an exception would normally terminate ReactiveUI 
        /// internal state.
        /// </summary>
        public IObservable<Exception> ThrownExceptions { get; private set; }

        /// <summary>
        /// Disposes this ObservableAsPropertyHelper
        /// </summary>
        public void Dispose()
        {
            (_inner ?? Disposable.Empty).Dispose();
            _inner = null;
        }

        /// <summary>
        /// Constructs a "default" ObservableAsPropertyHelper object. This is
        /// useful for when you will initialize the OAPH later, but don't want
        /// bindings to access a null OAPH at startup.
        /// </summary>
        /// <param name="initialValue">
        /// The initial (and only) value of the property.
        /// </param>
        /// <param name="scheduler">
        /// The scheduler that the notifications will be provided on - this should 
        /// normally be a Dispatcher-based scheduler
        /// </param>
        public static ObservableAsPropertyHelper<T> Default(T initialValue = default(T), IScheduler scheduler = null)
        {
            return new ObservableAsPropertyHelper<T>(Observable<T>.Never, _ => { }, initialValue, false, scheduler);
        }
    }

    /// <summary>
    /// A collection of helpers to aid working with observable properties
    /// </summary>
    public static class OAPHCreationHelperMixin
    {
        static ObservableAsPropertyHelper<TRet> observableToProperty<TObj, TRet>(
                this TObj This,
                IObservable<TRet> observable,
                Expression<Func<TObj, TRet>> property,
                TRet initialValue = default(TRet),
                bool deferSubscription = false,
                IScheduler scheduler = null)
            where TObj : IReactiveObject
        {
            Contract.Requires(This != null);
            Contract.Requires(observable != null);
            Contract.Requires(property != null);

            Expression expression = Reflection.Rewrite(property.Body);

            if (expression.GetParent().NodeType != ExpressionType.Parameter) {
                throw new ArgumentException("Property expression must be of the form 'x => x.SomeProperty'");
            }

            var name = expression.GetMemberInfo().Name;
            if (expression is IndexExpression)
                name += "[]";

            var ret = new ObservableAsPropertyHelper<TRet>(observable,
                _ => This.raisePropertyChanged(name),
                _ => This.raisePropertyChanging(name),
                initialValue, deferSubscription, scheduler);

            return ret;
        }

        /// <summary>
        /// Converts an Observable to an ObservableAsPropertyHelper and
        /// automatically provides the onChanged method to raise the property
        /// changed notification.         
        /// </summary>
        /// <param name="This">
        /// The observable to convert to an ObservableAsPropertyHelper
        /// </param>
        /// <param name="source">
        /// The ReactiveObject that has the property
        /// </param>
        /// <param name="property">
        /// An Expression representing the property (i.e. <c>x => x.SomeProperty</c>)
        /// </param>
        /// <param name="initialValue">
        /// The initial value of the property.
        /// </param>
        /// <param name="deferSubscription">
        /// A value indicating whether the <see cref="ObservableAsPropertyHelper{T}"/> 
        /// should defer the subscription to the <paramref name="This"/> source 
        /// until the first call to <see cref="ObservableAsPropertyHelper{T}.Value"/>,
        /// or if it should immediately subscribe to the the <paramref name="This"/> source.
        /// </param>
        /// <param name="scheduler">
        /// The scheduler that the notifications will be provided on - this should normally 
        /// be a Dispatcher-based scheduler
        /// </param>
        /// <returns>
        /// An initialized ObservableAsPropertyHelper; use this as the backing field 
        /// for your property.
        /// </returns>
        public static ObservableAsPropertyHelper<TRet> ToProperty<TObj, TRet>(
            this IObservable<TRet> This,
            TObj source,
            Expression<Func<TObj, TRet>> property,
            TRet initialValue = default(TRet),
            bool deferSubscription = false,
            IScheduler scheduler = null)
            where TObj : IReactiveObject
        {
            return source.observableToProperty(This, property, initialValue, deferSubscription, scheduler);
        }

        /// <summary>
        /// Converts an Observable to an ObservableAsPropertyHelper and
        /// automatically provides the onChanged method to raise the property
        /// changed notification.         
        /// </summary>
        /// <param name="This">
        /// The observable to convert to an ObservableAsPropertyHelper
        /// </param>
        /// <param name="source">
        /// The ReactiveObject that has the property
        /// </param>
        /// <param name="property">
        /// An Expression representing the property (i.e. <c>x => x.SomeProperty</c>)
        /// </param>
        /// <param name="result">
        /// An out param matching the return value, provided for convenience
        /// </param>
        /// <param name="initialValue">
        /// The initial value of the property.
        /// </param>
        /// <param name="deferSubscription">
        /// A value indicating whether the <see cref="ObservableAsPropertyHelper{T}"/> 
        /// should defer the subscription to the <paramref name="This"/> source 
        /// until the first call to <see cref="ObservableAsPropertyHelper{T}.Value"/>, 
        /// or if it should immediately subscribe to the the <paramref name="This"/> source.
        /// </param>
        /// <param name="scheduler">
        /// The scheduler that the notifications will be provided on - this should 
        /// normally be a Dispatcher-based scheduler
        /// </param>
        /// <returns>
        /// An initialized ObservableAsPropertyHelper; use this as the backing 
        /// field for your property.
        /// </returns>
        public static ObservableAsPropertyHelper<TRet> ToProperty<TObj, TRet>(
            this IObservable<TRet> This,
            TObj source,
            Expression<Func<TObj, TRet>> property,
            out ObservableAsPropertyHelper<TRet> result,
            TRet initialValue = default(TRet),
            bool deferSubscription = false,
            IScheduler scheduler = null)
            where TObj : IReactiveObject
        {
            var ret = source.observableToProperty(This, property, initialValue, deferSubscription, scheduler);

            result = ret;
            return ret;
        }
    }
}

// vim: tw=120 ts=4 sw=4 et :
