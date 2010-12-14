using System;
using System.Collections.Generic;
using System.Concurrency;
using System.Linq;
using System.Text;
using System.Diagnostics.Contracts;

#if WINDOWS_PHONE
using Microsoft.Phone.Reactive;
#endif

#if !SILVERLIGHT
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;
#endif

namespace ReactiveXaml
{
    public class ObservableAsyncMRUCache<TParam, TVal> : IEnableLogger
    {
        readonly MemoizingMRUCache<TParam, IObservable<TVal>> _innerCache;
        readonly SemaphoreSubject<long> _callQueue;
        readonly Func<TParam, IObservable<TVal>> _fetcher;
        long currentCall = 0;

        public ObservableAsyncMRUCache(Func<TParam, IObservable<TVal>> func, int maxSize, int maxConcurrent = 1, IScheduler sched = null, Action<TVal> onRelease = null)
        {
            _callQueue = new SemaphoreSubject<long>(maxConcurrent, sched);
            _fetcher = func;

            Action<IObservable<TVal>> release = null;
            if (onRelease != null) {
                release = new Action<IObservable<TVal>>(x => onRelease(x.First()));
            }

            _innerCache = new MemoizingMRUCache<TParam, IObservable<TVal>>((x, val) => {
                var ret = (IObservable<TVal>)val;
                return ret;
            }, maxSize, release);
        }

        public IObservable<TVal> AsyncGet(TParam key)
        {
            IObservable<TVal> result;
            if (_innerCache.TryGet(key, out result)) {
                this.Log().DebugFormat("Cache hit: '{0}'", key);
                return result;
            }

            var myCall = Interlocked.Increment(ref currentCall);

            var rs = new ReplaySubject<TVal>();
            _callQueue.Where(x => x == myCall).Subscribe(_ => {
                this.Log().DebugFormat("Dispatching '{0}'", key);
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

            lock(_innerCache) {
                _innerCache.Get(key, rs);
            }

            _callQueue.OnNext(myCall);
            return rs;
        }

        public TVal Get(TParam key)
        {
            return AsyncGet(key).First();
        }
    }

    public class SemaphoreSubject<T> : ISubject<T>, IEnableLogger
    {
        readonly Subject<T> _inner;
        Queue<T> _nextItems = new Queue<T>();
        long _count;
        readonly long _maxCount;

        public SemaphoreSubject(int maxCount, IScheduler sched = null)
        {
            this.Log().DebugFormat("maxCount is '{0}'", maxCount);
            _inner = (sched != null ? new Subject<T>(sched) : new Subject<T>());
            _maxCount = maxCount;
        }

        public void OnNext(T value)
        {
            var queue = Interlocked.CompareExchange(ref _nextItems, null, null);
            if (queue == null)
                return;

            lock (queue) {
                this.Log().DebugFormat("OnNext called for '{0}', count is '{1}'", value, _count);
                queue.Enqueue(value);
            }
            yieldUntilEmptyOrBlocked();
        }

        public void Release()
        {
            Interlocked.Decrement(ref _count);

            this.Log().DebugFormat("Releasing, count is now {0}", _count);
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

            while(Interlocked.Read(ref _count) < _maxCount) {
                T next;
                lock(queue) {
                    if (queue.Count == 0) {
                        break;
                    }
                    next = queue.Dequeue();
                }

                this.Log().DebugFormat("Yielding '{0}', _count = {1}, _maxCount = {2}", next, _count, _maxCount);
                _inner.OnNext(next);

                if (Interlocked.Increment(ref _count) >= _maxCount) {
                    break;
                }
            }
        }
    }

    public class LockedDictionary<TKey, TVal> : IDictionary<TKey, TVal>
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
