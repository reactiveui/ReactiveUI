﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics.Contracts;
using System.Linq.Expressions;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Splat;
using System.Reactive.Disposables;

namespace ReactiveUI
{
    public delegate void RefAction<T1>(ref T1 backingField, T1 value);

    /// <summary>
    /// ObservableAsPropertyHelper is a class to help ViewModels implement
    /// "output properties", that is, a property that is backed by an
    /// Observable. The property will be read-only, but will still fire change
    /// notifications. This class can be created directly, but is more often created via the
    /// ToProperty and ObservableToProperty extension methods.
    /// </summary>
    public sealed class ObservableAsPropertyHelper<T> : IHandleObservableErrors, IDisposable, IEnableLogger
    {
        T _lastValue;
        readonly IConnectableObservable<T> _source;
        IDisposable _inner;

        /// <summary>
        /// Constructs an ObservableAsPropertyHelper object.
        /// </summary>
        /// <param name="observable">The Observable to base the property on.</param>
        /// <param name="onNewValue">The action to take when the property
        /// changes, typically this will call the ViewModel's
        /// RaisePropertyChanged method.</param>
        /// <param name="initialValue">The initial value of the property.</param>
        /// <param name="scheduler">The scheduler that the notifications will be
        /// provided on - this is by default the CurrentThreadScheduler.</param>
        public ObservableAsPropertyHelper(
            IObservable<T> observable,
            RefAction<T> onNewValue = null, 
            T initialValue = default(T), 
            IScheduler scheduler = null)
        {
            Contract.Requires(observable != null);

            scheduler = scheduler ?? CurrentThreadScheduler.Instance;
            onNewValue = onNewValue ?? new RefAction<T>((ref T field, T value) => field = value);

            var subj = new ScheduledSubject<T>(scheduler);
            var exSubject = new ScheduledSubject<Exception>(CurrentThreadScheduler.Instance, RxApp.DefaultExceptionHandler);

            subj.Subscribe(x => onNewValue(ref _lastValue, x), exSubject.OnNext);

            ThrownExceptions = exSubject;

            // Push the initial value
            subj.OnNext(initialValue);
            _source = observable.DistinctUntilChanged().Multicast(subj);
        }

        /// <summary>
        /// The last provided value from the Observable. 
        /// </summary>
        public T Value {
            get { 
                _inner = _inner ?? _source.Connect();
                return _lastValue; 
            }
        }

        /// <summary>
        /// Fires whenever an exception would normally terminate ReactiveUI 
        /// internal state.
        /// </summary>
        public IObservable<Exception> ThrownExceptions { get; private set; }

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
        /// <param name="initialValue">The initial (and only) value of the property.</param>
        /// <param name="scheduler">The scheduler that the notifications will be
        /// provided on - this should normally be a Dispatcher-based scheduler
        /// (and is by default)</param>
        public static ObservableAsPropertyHelper<T> Default(T initialValue = default(T), IScheduler scheduler = null)
        {
            return new ObservableAsPropertyHelper<T>(Observable.Never<T>(), null, initialValue, scheduler);
        }
    }

    public static class OAPHCreationHelperMixin
    {
        static ObservableAsPropertyHelper<TRet> observableToProperty<TObj, TRet>(
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

            Expression expression = Reflection.Rewrite(property.Body);

            if (expression.GetParent().NodeType != ExpressionType.Parameter) {
                throw new ArgumentException("Property expression must be of the form 'x => x.SomeProperty'");
            }

            var ret = new ObservableAsPropertyHelper<TRet>(observable, 
                (ref TRet field, TRet value) => This.RaiseAndSetIfChanged(ref field, value, expression.GetMemberInfo().Name), 
                initialValue, scheduler);

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
            IScheduler scheduler = null)
            where TObj : ReactiveObject
        {
            return source.observableToProperty(This, property, initialValue, scheduler);
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
            out ObservableAsPropertyHelper<TRet> result,
            TRet initialValue = default(TRet),
            IScheduler scheduler = null)
            where TObj : ReactiveObject
        {
            var ret = source.observableToProperty(This, property, initialValue, scheduler);

            result = ret;
            return ret;
        }
    }
}

// vim: tw=120 ts=4 sw=4 et :
