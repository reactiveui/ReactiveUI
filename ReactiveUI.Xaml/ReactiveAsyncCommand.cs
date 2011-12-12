using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Diagnostics.Contracts;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using ReactiveUI;

namespace ReactiveUI.Xaml
{
    /// <summary>
    /// ReactiveAsyncCommand represents commands that run an asynchronous
    /// operation in the background when invoked. The main benefit of this
    /// command is that it will keep track of in-flight operations and
    /// disable/enable CanExecute when there are too many of them (i.e. a
    /// "Search" button shouldn't have many concurrent requests running if the
    /// user clicks the button many times quickly)
    /// </summary>
    public class ReactiveAsyncCommand : IReactiveAsyncCommand, IEnableLogger, IDisposable
    {
        /// <summary>
        /// Constructs a new ReactiveAsyncCommand.
        /// </summary>
        /// <param name="canExecute">An Observable representing when the command
        /// can execute. If null, the Command can always execute.</param>
        /// <param name="maximumConcurrent">The maximum number of in-flight
        /// operations at a time - defaults to one.</param>
        /// <param name="scheduler">The scheduler to run the asynchronous
        /// operations on - defaults to the Taskpool scheduler.</param>
        public ReactiveAsyncCommand(
            IObservable<bool> canExecute = null, 
            int maximumConcurrent = 1, 
            IScheduler scheduler = null)
        {
            commonCtor(maximumConcurrent, scheduler, canExecute);
        }

        protected ReactiveAsyncCommand(
            Func<object, bool> canExecute, 
            int maximumConcurrent = 1, 
            IScheduler scheduler = null)
        {
            Contract.Requires(maximumConcurrent > 0);

            _canExecuteExplicitFunc = canExecute;
            commonCtor(maximumConcurrent, scheduler);
        }

        /// <summary>
        /// Create is a helper method to create a basic ReactiveAsyncCommand
        /// in a non-Rx way, closer to how BackgroundWorker works.
        /// </summary>
        /// <param name="calculationFunc">The function that will calculate
        /// results in the background</param>
        /// <param name="callbackFunc">The method to be called once the
        /// calculation function completes. This method is guaranteed to be
        /// called on the UI thread.</param>
        /// <param name="maximumConcurrent">The maximum number of in-flight
        /// operations at a time - defaults to one.</param>
        /// <param name="scheduler">The scheduler to run the asynchronous
        /// operations on - defaults to the Taskpool scheduler.</param>
        public static ReactiveAsyncCommand Create<TRet>(
            Func<object, TRet> calculationFunc,
            Action<TRet> callbackFunc,
            Func<object, bool> canExecute = null, 
            int maximumConcurrent = 1,
            IScheduler scheduler = null)
        {
            var ret = new ReactiveAsyncCommand(canExecute, maximumConcurrent, scheduler);
            ret.RegisterAsyncFunction(calculationFunc).Subscribe(callbackFunc);
            return ret;
        }

        void commonCtor(int maximumConcurrent, IScheduler scheduler, IObservable<bool> canExecute = null)
        {
            _canExecuteSubject = new ScheduledSubject<bool>(Scheduler.Immediate);
            _executeSubject = new ScheduledSubject<object>(Scheduler.Immediate);
            _normalSched = scheduler ?? RxApp.DeferredScheduler;

            AsyncStartedNotification = new ScheduledSubject<Unit>(RxApp.DeferredScheduler);
            AsyncCompletedNotification = new ScheduledSubject<Unit>(RxApp.DeferredScheduler);
            AsyncErrorNotification = new ScheduledSubject<Exception>(RxApp.DeferredScheduler);

            ItemsInflight = Observable.Merge(
                AsyncStartedNotification.Select(_ => 1),
                AsyncCompletedNotification.Select(_ => -1)
            ).Scan(0, (acc, x) => {
                var ret = acc + x;
                if (ret < 0) {
                    this.Log().Fatal("Reference count dropped below zero");
                }
                return ret;
            }).Multicast(new BehaviorSubject<int>(0)).PermaRef().ObserveOn(RxApp.DeferredScheduler);

            bool startCE = (_canExecuteExplicitFunc != null ? _canExecuteExplicitFunc(null) : true);

            CanExecuteObservable = Observable.CombineLatest(
                    _canExecuteSubject.StartWith(startCE), ItemsInflight.Select(x => x < maximumConcurrent).StartWith(true),
                    (canEx, slotsAvail) => canEx && slotsAvail)
                .DistinctUntilChanged();

            CanExecuteObservable.Subscribe(x => {
                this.Log().InfoFormat("Setting canExecuteLatest to {0}", x);
                _canExecuteLatest = x;
                if (CanExecuteChanged != null) {
                    CanExecuteChanged(this, new EventArgs());
                }
            });

            if (canExecute != null) {
                var ce = canExecute.Multicast(_canExecuteSubject);
                _inner = ce.Connect();
            }

            _maximumConcurrent = maximumConcurrent;
        }

        IScheduler _normalSched;
        Func<object, bool> _canExecuteExplicitFunc = null;
        ISubject<bool> _canExecuteSubject;
        bool _canExecuteLatest;
        ISubject<object> _executeSubject;
        int _maximumConcurrent;
        IDisposable _inner = null;

        public IObservable<int> ItemsInflight { get; protected set; }

        public ISubject<Unit> AsyncStartedNotification { get; protected set; }

        public ISubject<Unit> AsyncCompletedNotification { get; protected set; }

        public ISubject<Exception> AsyncErrorNotification { get; protected set; }

        public IObservable<bool> CanExecuteObservable { get; protected set; }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            if (_canExecuteExplicitFunc != null) {
                _canExecuteSubject.OnNext(_canExecuteExplicitFunc(parameter));
            }
            this.Log().InfoFormat("CanExecute: returning {0}", _canExecuteLatest);
            return _canExecuteLatest;
        }

        public void Execute(object parameter)
        {
            if (!CanExecute(parameter)) {
                this.Log().Error("Attempted to call Execute when CanExecute is False!");
                return;
            }
            _executeSubject.OnNext(parameter);
        }

        public IDisposable Subscribe(IObserver<object> observer)
        {
            return _executeSubject.Subscribe(observer);
        }

        public void Dispose()
        {
            if (_inner != null) {
                _inner.Dispose();
            }
        }

        /// <summary>
        /// RegisterAsyncFunction registers an asynchronous method that returns a result
        /// to be called whenever the Command's Execute method is called.
        /// </summary>
        /// <param name="calculationFunc">The function to be run in the
        /// background.</param>
        /// <param name="scheduler"></param>
        /// <returns>An Observable that will fire on the UI thread once per
        /// invoecation of Execute, once the async method completes. Subscribe to
        /// this to retrieve the result of the calculationFunc.</returns>
        public IObservable<TResult> RegisterAsyncFunction<TResult>(
            Func<object, TResult> calculationFunc, 
            IScheduler scheduler = null)
        {
            Contract.Requires(calculationFunc != null);

            var asyncFunc = calculationFunc.ToAsync(scheduler ?? RxApp.TaskpoolScheduler);
            return RegisterAsyncObservable(asyncFunc);
        }

        /// <summary>
        /// RegisterAsyncAction registers an asynchronous method that runs
        /// whenever the Command's Execute method is called and doesn't return a
        /// result.
        /// </summary>
        /// <param name="calculationFunc">The function to be run in the
        /// background.</param>
        public void RegisterAsyncAction(Action<object> calculationFunc, 
            IScheduler scheduler = null)
        {
            Contract.Requires(calculationFunc != null);
            RegisterAsyncFunction(x => { calculationFunc(x); return new Unit(); }, scheduler);
        }

        /// <summary>
        /// RegisterAsyncObservable registers an Rx-based async method whose
        /// results will be returned on the UI thread.
        /// </summary>
        /// <param name="calculationFunc">A calculation method that returns a
        /// future result, such as a method returned via
        /// Observable.FromAsyncPattern.</param>
        /// <returns>An Observable representing the items returned by the
        /// calculation result. Note that with this method it is possible with a
        /// calculationFunc to return multiple items per invocation of Execute.</returns>
        public IObservable<TResult> RegisterAsyncObservable<TResult>(Func<object, IObservable<TResult>> calculationFunc)
        {
            Contract.Requires(calculationFunc != null);

            var ret = _executeSubject
                .Select(x => {
                    AsyncStartedNotification.OnNext(Unit.Default);

                    return calculationFunc(x)
                        .Catch<TResult, Exception>(ex => {
                            AsyncErrorNotification.OnNext(ex);
                            return Observable.Empty<TResult>();
                        })
                        .Finally(() => AsyncCompletedNotification.OnNext(Unit.Default));
                });

            return ret.Merge().Multicast(new ScheduledSubject<TResult>(RxApp.DeferredScheduler)).PermaRef();
        }

        /// <summary>
        /// RegisterMemoizedFunction is similar to RegisterAsyncFunction, but
        /// caches its results so that subsequent Execute calls with the same
        /// CommandParameter will not need to be run in the background.         
        /// </summary>
        /// <param name="calculationFunc">The function that performs the
        /// expensive or asyncronous calculation and returns the result.
        ///
        /// Note that this function *must* return an equivalently-same result given a
        /// specific input - because the function is being memoized, if the
        /// calculationFunc depends on other varables other than the input
        /// value, the results will be unpredictable.</param>
        /// <param name="maxSize">The number of items to cache. When this limit
        /// is reached, not recently used items will be discarded.</param>
        /// <param name="onRelease">This optional method is called when an item
        /// is evicted from the cache - this can be used to clean up / manage an
        /// on-disk cache; the calculationFunc can download a file and save it
        /// to a temporary folder, and the onRelease action will delete the
        /// file.</param>
        /// <param name="sched">The scheduler to run asynchronous operations on
        /// - defaults to TaskpoolScheduler</param>
        /// <returns>An Observable that will fire on the UI thread once per
        /// invocation of Execute, once the async method completes. Subscribe to
        /// this to retrieve the result of the calculationFunc.</returns>
        public IObservable<TResult> RegisterMemoizedFunction<TResult>(
            Func<object, TResult> calculationFunc, 
            int maxSize = 50, 
            Action<TResult> onRelease = null, 
            IScheduler sched = null)
        {
            Contract.Requires(calculationFunc != null);
            Contract.Requires(maxSize > 0);

            sched = sched ?? RxApp.TaskpoolScheduler;
            return RegisterMemoizedObservable(x => Observable.Return(calculationFunc(x), sched), maxSize, onRelease, sched);
        }

        /// <summary>
        /// RegisterMemoizedObservable is similar to RegisterAsyncObservable, but
        /// caches its results so that subsequent Execute calls with the same
        /// CommandParameter will not need to be run in the background.         
        /// </summary>
        /// <param name="calculationFunc">The function that performs the
        /// expensive or asyncronous calculation and returns the result.
        ///
        /// Note that this function *must* return an equivalently-same result given a
        /// specific input - because the function is being memoized, if the
        /// calculationFunc depends on other varables other than the input
        /// value, the results will be unpredictable. 
        /// </param>
        /// <param name="maxSize">The number of items to cache. When this limit
        /// is reached, not recently used items will be discarded.</param>
        /// <param name="onRelease">This optional method is called when an item
        /// is evicted from the cache - this can be used to clean up / manage an
        /// on-disk cache; the calculationFunc can download a file and save it
        /// to a temporary folder, and the onRelease action will delete the
        /// file.</param>
        /// <param name="sched">The scheduler to run asynchronous operations on
        /// - defaults to TaskpoolScheduler</param>
        /// <returns>An Observable representing the items returned by the
        /// calculation result. Note that with this method it is possible with a
        /// calculationFunc to return multiple items per invocation of Execute.</returns>
        public IObservable<TResult> RegisterMemoizedObservable<TResult>(
            Func<object, IObservable<TResult>> calculationFunc, 
            int maxSize = 50,
            Action<TResult> onRelease = null,  
            IScheduler sched = null)
        {
            Contract.Requires(calculationFunc != null);
            Contract.Requires(maxSize > 0);

            sched = sched ?? RxApp.TaskpoolScheduler;
            var cache = new ObservableAsyncMRUCache<object, TResult>(
                calculationFunc, maxSize, _maximumConcurrent, onRelease, sched);
            return this.RegisterAsyncObservable(cache.AsyncGet);
        }
    }

    public static class ReactiveAsyncCommandMixins
    {
        public static int CurrentItemsInFlight(this IReactiveAsyncCommand This)
        {
            return This.ItemsInflight.First();
        }
    }
}

// vim: tw=120 ts=4 sw=4 et :
