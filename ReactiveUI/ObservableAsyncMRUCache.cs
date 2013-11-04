using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;

namespace ReactiveUI
{
    /// <summary>
    /// ObservableAsyncMRUCache implements memoization for asynchronous or
    /// expensive to compute methods. This memoization is an MRU-based cache
    /// with a fixed limit for the number of items in the cache.     
    ///
    /// This class guarantees that only one calculation for any given key is
    /// in-flight at a time, subsequent requests will wait for the first one and
    /// return its results (for example, an empty web image cache that receives
    /// two concurrent requests for "Foo.jpg" will only issue one WebRequest -
    /// this does not mean that a request for "Bar.jpg" will wait on "Foo.jpg").
    ///
    /// Concurrency is also limited by the maxConcurrent parameter - when too
    /// many in-flight operations are in progress, further operations will be
    /// queued until a slot is available.
    /// </summary>
    /// <typeparam name="TParam">The key type.</typeparam>
    /// <typeparam name="TVal">The type of the value to return from the cache.</typeparam>
    public sealed class ObservableAsyncMRUCache<TParam, TVal> : IEnableLogger
    {
        readonly MemoizingMRUCache<TParam, IObservable<TVal>> _innerCache;
        readonly SemaphoreSubject<int> _callQueue;
        readonly Func<TParam, IObservable<TVal>> _fetcher;
        int currentCall = 0;

        /// <summary>
        /// Constructs an ObservableAsyncMRUCache object.
        /// </summary>
        /// <param name="calculationFunc">The function that performs the
        /// expensive or asyncronous calculation and returns an async result -
        /// for CPU-based operations, Observable.Return may be used to return
        /// the result.
        ///
        /// Note that this function *must* return an equivalently-same result given a
        /// specific input - because the function is being memoized, if the
        /// calculationFunc depends on other varables other than the input
        /// value, the results will be unpredictable.
        /// </param>
        /// <param name="maxSize">The number of items to cache. When this limit
        /// is reached, not recently used items will be discarded.</param>
        /// <param name="maxConcurrent">The maximum number of concurrent
        /// asynchronous operations regardless of key - this is important for
        /// web-based caches to limit the number of concurrent requests to a
        /// server. The default is 5.</param>
        /// <param name="onRelease">This optional method is called when an item
        /// is evicted from the cache - this can be used to clean up / manage an
        /// on-disk cache; the calculationFunc can download a file and save it
        /// to a temporary folder, and the onRelease action will delete the
        /// file.</param>
        /// <param name="sched">The scheduler to run asynchronous operations on
        /// - defaults to TaskpoolScheduler</param>
        public ObservableAsyncMRUCache(
            Func<TParam, IObservable<TVal>> calculationFunc, 
            int maxSize, 
            int maxConcurrent = 5, 
            Action<TVal> onRelease = null, 
            IScheduler sched = null)
        {
            sched = sched ?? RxApp.TaskpoolScheduler;
            _callQueue = new SemaphoreSubject<int>(maxConcurrent, sched);
            _fetcher = calculationFunc;

            Action<IObservable<TVal>> release = null;
            if (onRelease != null) {
                release = new Action<IObservable<TVal>>(x => x.Subscribe(onRelease));
            }

            _innerCache = new MemoizingMRUCache<TParam, IObservable<TVal>>((x, val) => {
                var ret = (IObservable<TVal>)val;
                return ret;
            }, maxSize, release);
        }

        /// <summary>
        /// Issues an request to fetch the value for the specified key as an
        /// async operation. The Observable returned will fire one time when the
        /// async operation finishes. If the operation is cached, an Observable
        /// that immediately fires upon subscribing will be returned.
        /// </summary>
        /// <param name="key">The key to provide to the calculation function.</param>
        /// <returns>Returns an Observable representing the future result.</returns>
        public IObservable<TVal> AsyncGet(TParam key)
        {
            IObservable<TVal> result;
            int myCall;
            var rs = new ReplaySubject<TVal>();

            lock (_innerCache) {
                if (_innerCache.TryGet(key, out result)) {
                    this.Log().Debug("Cache hit: '{0}'", key);
                    return result;
                }

                myCall = Interlocked.Increment(ref currentCall);

                _callQueue.Where(x => x == myCall).Take(1).Subscribe(_ => {
                    this.Log().Debug("Dispatching '{0}'", key);
                    IObservable<TVal> fetched = null;
                    try {
                        fetched = _fetcher(key);
                    } catch (Exception ex) {
                        _callQueue.Release();
                        rs.OnError(ex);
                        return;
                    }

                    fetched.Subscribe(x => {
                        rs.OnNext(x);
                    }, ex => {
                        _callQueue.Release();
                        rs.OnError(ex);
                    }, () => {
                        _callQueue.Release();
                        rs.OnCompleted();
                    });
                });

                _innerCache.Get(key, rs);
            }

            _callQueue.OnNext(myCall);
            return rs;
        }

        /// <summary>
        /// The synchronous version of AsyncGet - it will issue a request for
        /// the value of a specific key and wait until the value can be
        /// provided.
        /// </summary>
        /// <param name="key">The key to provide to the calculation function.</param>
        /// <returns>The resulting value.</returns>
        public TVal Get(TParam key)
        {
            return AsyncGet(key).First();
        }
    }

    public static class ObservableCacheMixin
    {
        /// <summary>
        /// Works like SelectMany, but memoizes selector calls. In addition, it 
        /// guarantees that no more than 'maxConcurrent' selectors are running 
        /// concurrently and queues the rest. This is very important when using
        /// web services to avoid potentially spamming the server with hundreds 
        /// of requests.
        /// </summary>
        /// <param name="selector">A selector similar to one you would pass as a 
        /// parameter passed to SelectMany. Note that similarly to 
        /// ObservableAsyncMRUCache.AsyncGet, a selector must return semantically
        /// identical results given the same key - i.e. it must be a 'function' in
        /// the mathematical sense.</param>
        /// <param name="maxCached">The number of items to cache. When this limit
        /// is reached, not recently used items will be discarded.</param>
        /// <param name="maxConcurrent">The maximum number of concurrent
        /// asynchronous operations regardless of key - this is important for
        /// web-based caches to limit the number of concurrent requests to a
        /// server. The default is 5.</param>
        /// <param name="scheduler"></param>
        /// <returns>An Observable representing the flattened results of the 
        /// selector.</returns>
        public static IObservable<TRet> CachedSelectMany<T, TRet>(this IObservable<T> This, Func<T, IObservable<TRet>> selector, int maxCached = 50, int maxConcurrent = 5, IScheduler scheduler = null)
        {
            var cache = new ObservableAsyncMRUCache<T, TRet>(selector, maxCached, maxConcurrent, null, scheduler);
            return This.SelectMany(cache.AsyncGet);
        }

        /// <summary>
        /// Works like SelectMany, but memoizes selector calls. In addition, it 
        /// guarantees that no more than 'maxConcurrent' selectors are running 
        /// concurrently and queues the rest. This is very important when using
        /// web services to avoid potentially spamming the server with hundreds 
        /// of requests.
        ///
        /// This overload is useful when making the same web service call in
        /// several places in the code, to ensure that all of the code paths are
        /// using the same cache.
        /// </summary>
        /// <param name="existingCache">An already-configured ObservableAsyncMRUCache.</param>
        /// <returns>An Observable representing the flattened results of the 
        /// cache selector.</returns>
        public static IObservable<TRet> CachedSelectMany<T, TRet>(this IObservable<T> This, ObservableAsyncMRUCache<T, TRet> existingCache)
        {
            return This.SelectMany(existingCache.AsyncGet);
        }
    }

    internal class SemaphoreSubject<T> : ISubject<T>, IEnableLogger
    {        
        readonly ISubject<T> _inner;
        Queue<T> _nextItems = new Queue<T>();
        int _count;
        readonly int _maxCount;

        public SemaphoreSubject(int maxCount, IScheduler sched = null)
        {
            this.Log().Debug("maxCount is '{0}'", maxCount);
            _inner = (sched != null ? (ISubject<T>)new ScheduledSubject<T>(sched) : new Subject<T>());
            _maxCount = maxCount;
        }

        public void OnNext(T value)
        {
            var queue = Interlocked.CompareExchange(ref _nextItems, null, null);
            if (queue == null)
                return;

            lock (queue) {
                this.Log().Debug("OnNext called for '{0}', count is '{1}'", value, _count);
                queue.Enqueue(value);
            }
            yieldUntilEmptyOrBlocked();
        }

        public void Release()
        {
            Interlocked.Decrement(ref _count);

            this.Log().Debug("Releasing, count is now {0}", _count);
            yieldUntilEmptyOrBlocked();
        }

        public void OnCompleted()
        {
            var queue = Interlocked.Exchange(ref _nextItems, null);
            if (queue == null)
                return;

            T[] items;
            lock (queue) {
                items = queue.ToArray();
            }

            foreach(var v in items) {
                _inner.OnNext(v);
            }

            _inner.OnCompleted();
        }

        public void OnError(Exception error)
        {
            var queue = Interlocked.Exchange(ref _nextItems, null);
            _inner.OnError(error);
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            return _inner.Subscribe(observer);
        }

        void yieldUntilEmptyOrBlocked()
        {
            var queue = Interlocked.CompareExchange(ref _nextItems, null, null);

            if (queue == null) {
                return;
            }

            while(_count < _maxCount) {
                T next;
                lock(queue) {
                    if (queue.Count == 0) {
                        break;
                    }
                    next = queue.Dequeue();
                }

                this.Log().Debug("Yielding '{0}', _count = {1}, _maxCount = {2}", next, _count, _maxCount);
                _inner.OnNext(next);

                if (Interlocked.Increment(ref _count) >= _maxCount) {
                    break;
                }
            }
        }
    }

    internal class LockedDictionary<TKey, TVal> : IDictionary<TKey, TVal>
    {
        Dictionary<TKey, TVal> _inner = new Dictionary<TKey, TVal>();

        public void Add(TKey key, TVal value) {
            lock (_inner) {
                _inner.Add(key, value);
            }
        }

        public bool ContainsKey(TKey key) {
            lock (_inner) {
                return _inner.ContainsKey(key);
            }
        }

        public ICollection<TKey> Keys {
            get {
                lock (_inner) {
                    return _inner.Keys.ToArray();
                }
            }
        }

        public bool Remove(TKey key) {
            lock (_inner) {
                return _inner.Remove(key); 
            }
        }

        public bool TryGetValue(TKey key, out TVal value) {
            lock (_inner) {
                return _inner.TryGetValue(key, out value);
            }
        }

        public ICollection<TVal> Values {
            get {
                lock (_inner) {
                    return _inner.Values.ToArray();
                }
            }
        }

        public TVal this[TKey key] {
            get {
                lock (_inner) {
                    return _inner[key];
                }
            }
            set {
                lock (_inner) {
                    _inner[key] = value;
                }
            }
        }

        public void Add(KeyValuePair<TKey, TVal> item) {
            lock (_inner) {
                _inner.Add(item.Key, item.Value); 
            }
        }

        public void Clear() {
            lock (_inner) {
                _inner.Clear();
            }
        }

        public bool Contains(KeyValuePair<TKey, TVal> item) {
            lock(_inner) {
                var inner = _inner as IDictionary<TKey, TVal>;
                return (inner.Contains(item));
            }
        }

        public void CopyTo(KeyValuePair<TKey, TVal>[] array, int arrayIndex) {
            lock(_inner) {
                var inner = _inner as IDictionary<TKey, TVal>;
                inner.CopyTo(array, arrayIndex);
            }
        }

        public int Count {
            get {
                lock (_inner) {
                    return _inner.Count;
                }
            }
        }

        public bool IsReadOnly {
            get { return false; }
        }

        public bool Remove(KeyValuePair<TKey, TVal> item) {
            lock(_inner) {
                var inner = _inner as IDictionary<TKey, TVal>;
                return inner.Remove(item);
            }
        }

        public IEnumerator<KeyValuePair<TKey, TVal>> GetEnumerator() {
            lock (_inner) {
                return _inner.ToList().GetEnumerator();
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            lock(_inner) {
                return _inner.ToArray().GetEnumerator();
            }
        }
    }
}

// vim: tw=120 ts=4 sw=4 et :
