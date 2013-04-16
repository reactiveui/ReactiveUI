using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Diagnostics.Contracts;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using ReactiveUI;
using System.Threading.Tasks;

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
    public class ReactiveAsyncCommand : IReactiveAsyncCommand, IDisposable, IEnableLogger
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
        /// <param name="initialCondition">Initial CanExecute state</param>
        public ReactiveAsyncCommand(
            IObservable<bool> canExecute = null, 
            int maximumConcurrent = 1, 
            IScheduler scheduler = null,
            bool initialCondition = true)
        {
            commonCtor(maximumConcurrent, scheduler, canExecute, initialCondition);
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

        void commonCtor(int maximumConcurrent, IScheduler scheduler, IObservable<bool> canExecute = null, bool initialCondition = true)
        {
            _normalSched = scheduler ?? RxApp.DeferredScheduler;
            _canExecuteSubject = new ScheduledSubject<bool>(_normalSched);
            _executeSubject = new ScheduledSubject<object>(Scheduler.Immediate);
            _exSubject = new ScheduledSubject<Exception>(_normalSched, RxApp.DefaultExceptionHandler);

            AsyncStartedNotification = new ScheduledSubject<Unit>(RxApp.DeferredScheduler);
            AsyncCompletedNotification = new ScheduledSubject<Unit>(RxApp.DeferredScheduler);

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

            bool startCE = (_canExecuteExplicitFunc != null ? _canExecuteExplicitFunc(null) : initialCondition);

            CanExecuteObservable = Observable.CombineLatest(
                    _canExecuteSubject.StartWith(startCE), ItemsInflight.Select(x => x < maximumConcurrent).StartWith(true),
                    (canEx, slotsAvail) => canEx && slotsAvail)
                .DistinctUntilChanged();

            CanExecuteObservable.Subscribe(x => {
                this.Log().Debug("Setting canExecuteLatest to {0}", x);
                _canExecuteLatest = x;
                if (CanExecuteChanged != null) {
                    CanExecuteChanged(this, new EventArgs());
                }
            });

            if (canExecute != null) {
                _inner = canExecute.Subscribe(_canExecuteSubject.OnNext, _exSubject.OnNext);
            }

            MaximumConcurrent = maximumConcurrent;

            ThrownExceptions = _exSubject;
        }

        IScheduler _normalSched;
        Func<object, bool> _canExecuteExplicitFunc = null;
        ISubject<bool> _canExecuteSubject;
        bool _canExecuteLatest;
        ISubject<object> _executeSubject;
        IDisposable _inner = null;
        ScheduledSubject<Exception> _exSubject;

        public int MaximumConcurrent { get; protected set; }

        public IObservable<int> ItemsInflight { get; protected set; }

        public ISubject<Unit> AsyncStartedNotification { get; protected set; }

        public ISubject<Unit> AsyncCompletedNotification { get; protected set; }

        public IObservable<bool> CanExecuteObservable { get; protected set; }

        public IObservable<Exception> ThrownExceptions { get; protected set; }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            if (_canExecuteExplicitFunc != null) {
                _canExecuteSubject.OnNext(_canExecuteExplicitFunc(parameter));
            }
            this.Log().Debug("CanExecute: returning {0}", _canExecuteLatest);
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
            return _executeSubject.Subscribe(
                Observer.Create<object>(
                    x => marshalFailures(observer.OnNext, x),
                    ex => marshalFailures(observer.OnError, ex),
                    () => marshalFailures(observer.OnCompleted)));
        }

        public void Dispose()
        {
            if (_inner != null) {
                _inner.Dispose();
            }
        }

        public IObservable<TResult> RegisterAsyncObservable<TResult>(Func<object, IObservable<TResult>> calculationFunc)
        {
            Contract.Requires(calculationFunc != null);

            var ret = _executeSubject
                .Select(x => {
                    AsyncStartedNotification.OnNext(Unit.Default);

                    return calculationFunc(x)
                        .Catch<TResult, Exception>(ex => {
                            _exSubject.OnNext(ex);
                            return Observable.Empty<TResult>();
                        })
                        .Finally(() => AsyncCompletedNotification.OnNext(Unit.Default));
                });

            return ret.Merge().Multicast(new ScheduledSubject<TResult>(RxApp.DeferredScheduler)).PermaRef();
        }

        void marshalFailures<T>(Action<T> block, T param)
        {
            try {
                block(param);
            } catch (Exception ex) {
                _exSubject.OnNext(ex);
            }
        }

        void marshalFailures(Action block)
        {
            marshalFailures(_ => block(), Unit.Default);
        }
    }

    public static class ReactiveAsyncCommandMixins
    {
        /// <summary>
        /// This method returns the current number of items in flight.
        /// </summary>
        public static int CurrentItemsInFlight(this IReactiveAsyncCommand This)
        {
            return This.ItemsInflight.First();
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
        public static IObservable<TResult> RegisterAsyncFunction<TResult>(this IReactiveAsyncCommand This,
            Func<object, TResult> calculationFunc,
            IScheduler scheduler = null)
        {
            Contract.Requires(calculationFunc != null);

            var asyncFunc = calculationFunc.ToAsync(scheduler ?? RxApp.TaskpoolScheduler);
            return This.RegisterAsyncObservable(asyncFunc);
        }

        /// <summary>
        /// RegisterAsyncAction registers an asynchronous method that runs
        /// whenever the Command's Execute method is called and doesn't return a
        /// result.
        /// </summary>
        /// <param name="calculationFunc">The function to be run in the
        /// background.</param>
        public static IObservable<Unit> RegisterAsyncAction(this IReactiveAsyncCommand This, 
            Action<object> calculationFunc,
            IScheduler scheduler = null)
        {
            Contract.Requires(calculationFunc != null);
            return This.RegisterAsyncFunction(x => { calculationFunc(x); return new Unit(); }, scheduler);
        }

        /// <summary>
        /// RegisterAsyncTask registers an TPL/Async method that runs when a 
        /// Command gets executed and returns the result
        /// </summary>
        /// <returns>An Observable that will fire on the UI thread once per
        /// invoecation of Execute, once the async method completes. Subscribe to
        /// this to retrieve the result of the calculationFunc.</returns>
        public static IObservable<TResult> RegisterAsyncTask<TResult>(this IReactiveAsyncCommand This, Func<object, Task<TResult>> calculationFunc)
        {
            Contract.Requires(calculationFunc != null);
            return This.RegisterAsyncObservable(x => calculationFunc(x).ToObservable());
        }

        /// <summary>
        /// RegisterAsyncTask registers an TPL/Async method that runs when a 
        /// Command gets executed and returns no result. 
        /// </summary>
        /// <param name="calculationFunc">The function to be run in the
        /// background.</param>
        /// <returns>An Observable that signals when the Task completes, on
        /// the UI thread.</returns>
        public static IObservable<Unit> RegisterAsyncTask<TResult>(this IReactiveAsyncCommand This, Func<object, Task> calculationFunc)
        {
            Contract.Requires(calculationFunc != null);
            return This.RegisterAsyncObservable(x => calculationFunc(x).ToObservable());
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
        public static IObservable<TResult> RegisterMemoizedFunction<TResult>(
            this IReactiveAsyncCommand This,
            Func<object, TResult> calculationFunc,
            int maxSize = 50,
            Action<TResult> onRelease = null,
            IScheduler sched = null)
        {
            Contract.Requires(calculationFunc != null);
            Contract.Requires(maxSize > 0);

            sched = sched ?? RxApp.TaskpoolScheduler;
            return RegisterMemoizedObservable(This, x => Observable.Return(calculationFunc(x), sched), maxSize, onRelease, sched);
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
        public static IObservable<TResult> RegisterMemoizedObservable<TResult>(
            this IReactiveAsyncCommand This,
            Func<object, IObservable<TResult>> calculationFunc,
            int maxSize = 50,
            Action<TResult> onRelease = null,
            IScheduler sched = null)
        {
            Contract.Requires(calculationFunc != null);
            Contract.Requires(maxSize > 0);

            sched = sched ?? RxApp.TaskpoolScheduler;
            var cache = new ObservableAsyncMRUCache<object, TResult>(
                calculationFunc, maxSize, This.MaximumConcurrent, onRelease, sched);
            return This.RegisterAsyncObservable(cache.AsyncGet);
        }
    }
}

// vim: tw=120 ts=4 sw=4 et :
