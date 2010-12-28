using System;
using System.Collections.Generic;
using System.Linq;
using System.Concurrency;
using System.Diagnostics.Contracts;

namespace ReactiveXaml
{
    /// <summary>
    /// 
    /// </summary>
    public class ReactiveAsyncCommand : ReactiveCommand
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="canExecute"></param>
        /// <param name="maximumConcurrent"></param>
        /// <param name="scheduler"></param>
        public ReactiveAsyncCommand(IObservable<bool> canExecute = null, int maximumConcurrent = 1, IScheduler scheduler = null)
            : base(canExecute, scheduler)
        {
            commonCtor(maximumConcurrent, scheduler);
        }

        protected ReactiveAsyncCommand(Func<object, bool> canExecute, int maximumConcurrent = 1, IScheduler scheduler = null)
            : base(canExecute, scheduler)
        {
            Contract.Requires(maximumConcurrent > 0);

            this._normalSched = scheduler;
            commonCtor(maximumConcurrent, scheduler);
        }

        public static ReactiveAsyncCommand Create<TRet>(
            Func<object, TRet> calculationFunc,
            Action<TRet> callbackFunc,
            Func<object, bool> canExecute = null, 
            int maximumConcurrent = 0,
            IScheduler scheduler = null)
        {
            var ret = new ReactiveAsyncCommand(canExecute, maximumConcurrent, scheduler);
            ret.RegisterAsyncFunction(calculationFunc).Subscribe(callbackFunc);
            return ret;
        }

        void commonCtor(int maximumConcurrent, IScheduler scheduler)
        {
            AsyncCompletedNotification = new Subject<Unit>();
            this._normalSched = scheduler ?? RxApp.DeferredScheduler;

            ItemsInflight = Observable.Merge(
                this.Select(_ => 1),
                AsyncCompletedNotification.Select(_ => -1)
            ).Scan0(0, (acc, x) => {
                var ret = acc + x;
                if (ret < 0)
                    throw new OverflowException("Reference count dropped below zero");
                return ret;
            });

            ItemsInflight
                .Subscribe(x => {
                    this.Log().InfoFormat("0x{0:X} - {1} items in flight", this.GetHashCode(), x);
                    this._tooManyItems = (x >= maximumConcurrent && maximumConcurrent > 0);
                    canExecuteSubject.OnNext(!this._tooManyItems);
                });
        }

        IScheduler _normalSched;

        public IObservable<int> ItemsInflight { get; protected set; }

        public Subject<Unit> AsyncCompletedNotification { get; protected set; }

        bool _tooManyItems = false;
        public override bool CanExecute(object parameter)
        {
            // HACK: Normally we shouldn't need this, but due to the way that
            // ReactiveCommand.CanExecute works when you provide an explicit
            // Func<T>, it can "trump" the ItemsInFlight selector.
            if (this._tooManyItems)
                return false;

            return base.CanExecute(parameter);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="calculationFunc"></param>
        /// <param name="scheduler"></param>
        /// <returns></returns>
        public IObservable<TResult> RegisterAsyncFunction<TResult>(Func<object, TResult> calculationFunc, IScheduler scheduler = null)
        {
            Contract.Requires(calculationFunc != null);

            scheduler = scheduler ?? RxApp.TaskpoolScheduler;
            var rebroadcast = new Subject<TResult>();

            this.ObserveOn(scheduler)
                .Select(calculationFunc)
                .Do(_ => AsyncCompletedNotification.OnNext(new Unit()), _ => AsyncCompletedNotification.OnNext(new Unit()))
                .Subscribe(rebroadcast.OnNext, rebroadcast.OnError, rebroadcast.OnCompleted);

            return rebroadcast.ObserveOn(this._normalSched);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="calculationFunc"></param>
        /// <returns></returns>
        public IObservable<TResult> RegisterAsyncObservable<TResult>(Func<object, IObservable<TResult>> calculationFunc)
        {
            Contract.Requires(calculationFunc != null);

            var rebroadcast = new Subject<TResult>();

            this.SelectMany(calculationFunc)
                .Do(_ => AsyncCompletedNotification.OnNext(new Unit()), _ => AsyncCompletedNotification.OnNext(new Unit()))
                .Subscribe(rebroadcast.OnNext, rebroadcast.OnError, rebroadcast.OnCompleted);

            return rebroadcast.ObserveOn(this._normalSched);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="asyncAction"></param>
        /// <returns></returns>
        public IDisposable RegisterAsyncAction(Action<object> asyncAction)
        {
            Contract.Requires(asyncAction != null);

            return RegisterAsyncFunction(x => { asyncAction(x); return new Unit(); })
                .Subscribe();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="calculationFunc"></param>
        /// <param name="maxConcurrent"></param>
        /// <param name="cacheSize"></param>
        /// <param name="onRelease"></param>
        /// <param name="scheduler"></param>
        /// <returns></returns>
        public IObservable<TResult> RegisterMemoizedObservable<TResult>(
            Func<object, IObservable<TResult>> calculationFunc, 
            int maxConcurrent = 1,
            int cacheSize = 50,
            Action<TResult> onRelease = null,  
            IScheduler scheduler = null)
        {
            Contract.Requires(calculationFunc != null);
            Contract.Requires(cacheSize > 0);

            scheduler = scheduler ?? RxApp.TaskpoolScheduler;
            var cache = new ObservableAsyncMRUCache<object, TResult>(calculationFunc, cacheSize, maxConcurrent, scheduler, onRelease);
            return this.RegisterAsyncObservable(cache.AsyncGet);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="calculationFunc"></param>
        /// <param name="cacheSize"></param>
        /// <param name="onRelease"></param>
        /// <param name="scheduler"></param>
        /// <returns></returns>
        public IObservable<TResult> RegisterMemoizedFunction<TResult>(Func<object, TResult> calculationFunc, int cacheSize = 50, Action<TResult> onRelease = null, IScheduler scheduler = null)
        {
            Contract.Requires(calculationFunc != null);
            Contract.Requires(cacheSize > 0);

            scheduler = scheduler ?? RxApp.TaskpoolScheduler;
            return RegisterMemoizedObservable(x => Observable.Return(calculationFunc(x), scheduler), 1, cacheSize, onRelease, scheduler);
        }
    }
}

// vim: tw=120 ts=4 sw=4 et :
