using System;
using System.Collections.Generic;
using System.Concurrency;
using System.Linq;
using System.Text;

#if !SILVERLIGHT
using System.Threading.Tasks;
using System.Collections.Concurrent;
#endif

namespace ReactiveXaml
{
    public interface IMemoizingMRUCacheBase { }
    public interface IMemoizingMRUCache<TParam, TVal> : IMemoizingMRUCacheBase
    {
        IEnumerable<TVal> CachedValues();
        TVal Get(TParam key);
        TVal Get(TParam key, object context);
        void InvalidateAll();
    }

    /// <summary>
    /// This data structure is a representation of a memoizing cache - i.e. a
    /// class that will evaluate a function, but keep a cache of recently
    /// evaluated parameters.
    ///
    /// Since this is a memoizing cache, it is important that this function be a
    /// "pure" function in the mathematical sense - that a key *always* maps to
    /// a corresponding return value.
    /// </summary>
    /// <typeparam name="TParam">The type of the parameter to the calculation function.</typeparam>
    /// <typeparam name="TVal">The type of the value returned by the calculation
    /// function.</typeparam>
    public class MemoizingMRUCache<TParam, TVal> : IMemoizingMRUCache<TParam,TVal>, IEnableLogger
    {
        private readonly Func<TParam, object, TVal> calculationFunction;
        private readonly Action<TVal> releaseFunction;
        private readonly int maxCacheSize;

        private LinkedList<TParam> cacheMRUList;
        private Dictionary<TParam, Tuple<LinkedListNode<TParam>, TVal>> cacheEntries;

        public MemoizingMRUCache(Func<TParam, object, TVal> func, int max_size) : this(func, max_size, null) { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="func">The function whose results you want to cache,
        /// which is provided the key value, and an Tag object that is
        /// user-defined</param>
        /// <param name="max_size">The size of the cache to maintain.</param>
        /// <param name="on_release">A function to call when a result gets
        /// evicted from the cache (i.e. because Invalidate was called or the
        /// cache is full)</param>
        public MemoizingMRUCache(Func<TParam, object, TVal> func, int max_size, Action<TVal> on_release)
        {
            calculationFunction = func;
            releaseFunction = on_release;
            maxCacheSize = max_size;
            InvalidateAll();
        }

        public TVal Get(TParam key) { return Get(key, null); }

        /// <summary>
        /// Evaluates the function provided, returning the cached value if possible
        /// </summary>
        /// <param name="key"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public TVal Get(TParam key, object context)
        {
            if (cacheEntries.ContainsKey(key)) {
                var found = cacheEntries[key];
                this.Log().DebugFormat("Cache hit: {0}", key);
                cacheMRUList.Remove(found.Item1);
                cacheMRUList.AddFirst(found.Item1);
                return found.Item2;
            }

            this.Log().DebugFormat("Cache miss: {0}", key);
            var result = calculationFunction(key, context);

            var node = new LinkedListNode<TParam>(key);
            cacheMRUList.AddFirst(node);
            cacheEntries[key] = new Tuple<LinkedListNode<TParam>, TVal>(node, result);
            maintainCache();
            return result;
        }

        public bool TryGet(TParam key, out TVal val)
        {
            Tuple<LinkedListNode<TParam>, TVal> output;
            var ret = cacheEntries.TryGetValue(key, out output);
            if (ret && output != null) {
                val = output.Item2;
            } else {
                val = default(TVal);
            }
            return ret;
        }

        /// <summary>
        /// Ensure that the next time this key is queried, the calculation
        /// function will be called.
        /// </summary>
        public void Invalidate(TParam key)
        {
            if (!cacheEntries.ContainsKey(key))
                return;

            var to_remove = cacheEntries[key];
            if (releaseFunction != null)
                releaseFunction(to_remove.Item2);

            cacheMRUList.Remove(to_remove.Item1);
            cacheEntries.Remove(key);
        }

        /// <summary>
        /// Invalidate all items in the cache
        /// </summary>
        public void InvalidateAll()
        {
            if (releaseFunction == null || cacheEntries == null) {
                cacheMRUList = new LinkedList<TParam>();
                cacheEntries = new Dictionary<TParam, Tuple<LinkedListNode<TParam>, TVal>>();
                return;
            }

            if (cacheEntries.Count == 0)
                return;

            /* We have to remove them one-by-one to call the release function
             * We ToArray() this so we don't get a "modifying collection while
             * enumerating" exception. */
            foreach (var v in cacheEntries.Keys.ToArray()) { Invalidate(v); }
        }

        /// <summary>
        /// Returns all values currently in the cache
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TVal> CachedValues()
        {
            return cacheEntries.Select(x => x.Value.Item2);
        }

        private void maintainCache()
        {
            while (cacheMRUList.Count > maxCacheSize) {
                var to_remove = cacheMRUList.Last.Value;
                if (releaseFunction != null)
                    releaseFunction(cacheEntries[to_remove].Item2);

                this.Log().DebugFormat("Evicting {0}", to_remove);
                cacheEntries.Remove(cacheMRUList.Last.Value);
                cacheMRUList.RemoveLast();
            }
        }
    }

#if !SILVERLIGHT
    public sealed class QueuedAsyncMRUCache<TParam, TVal> : IEnableLogger
    {
        readonly MemoizingMRUCache<TParam, TVal> innerCache;
        readonly BlockingCollection<TParam> concurrentOps;
        readonly ConcurrentDictionary<TParam, IObservable<TVal>> concurrentOpsDict = new ConcurrentDictionary<TParam, IObservable<TVal>>();
        readonly Func<TParam, TVal> func;

        public QueuedAsyncMRUCache(Func<TParam, TVal> func, int max_size, int max_concurrent = 1, Action<TVal> on_release = null)
        {
            this.func = func;

            concurrentOps = new BlockingCollection<TParam>(max_concurrent);

            innerCache = new MemoizingMRUCache<TParam, TVal>((x, _) => { 
                IObservable<TVal> ret;
                while (!concurrentOpsDict.TryGetValue(x, out ret)) { }
                this.Log().DebugFormat("Stashing {0} in cache", x);
                return ret.Take(1).First(); 
            }, max_size, on_release);
        }

        public IEnumerable<TVal> CachedValues()
        {
            lock (innerCache) {
                return innerCache.CachedValues().ToArray();
            }
        }

        public TVal Get(TParam key)
        {
            this.Log().DebugFormat("Blocking Get: {0}", key);
            return AsyncGet(key).Take(1).First();
        }

        public IObservable<TVal> AsyncGet(TParam key)
        {
            TVal ret;
            this.Log().DebugFormat("Async Get: {0}", key);
            lock (innerCache) {
                if (innerCache.TryGet(key, out ret)) {
                    this.Log().DebugFormat("Found key in cache: {0}", key);
                    return Observable.Return(ret);
                }
            }

            var t = new Task<TVal>(() => func(key));
            var new_item = t.ToObservable();
            t.Start();

            IObservable<TVal> observable;
            if (concurrentOpsDict.TryGetValue(key, out observable)) {
                this.Log().DebugFormat("Found pending item in cache: {0}", key);
                return observable;
            }

            concurrentOps.Add(key);
            concurrentOpsDict.GetOrAdd(key, new_item);

            this.Log().DebugFormat("Not in cache or pending, adding new item: {0}", key);
            new_item.Subscribe(x => {
                concurrentOpsDict[key] = Observable.Return(x);
                lock (innerCache) { innerCache.Get(key); }
                concurrentOpsDict.TryRemove(key, out observable);
                concurrentOps.Take();
            }, ex => {
                var dontcare = t.Exception;
                concurrentOpsDict.TryRemove(key, out observable);
                concurrentOps.Take();
            });
            return new_item;
        }

        public void InvalidateAll()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            concurrentOps.CompleteAdding();
        }
    }
#endif

#if DOTNETISOLDANDSAD
    public class Tuple<T1, T2>
    {
        public Tuple(T1 item1, T2 item2) { Item1 = item1; Item2 = item2; }
        public Tuple() {} 

        public T1 Item1 {get; set;}
        public T2 Item2 {get; set;}
    }
#endif
}