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
    public sealed class QueuedAsyncMRUCache<TParam, TVal> : IEnableLogger, IDisposable
    {
        readonly MemoizingMRUCache<TParam, TVal> innerCache;
        readonly BlockingCollection<TParam> concurrentOps;
        readonly ConcurrentDictionary<TParam, IObservable<TVal>> concurrentOpsDict = new ConcurrentDictionary<TParam, IObservable<TVal>>();
        readonly Func<TParam, TVal> func;

        public QueuedAsyncMRUCache(Func<TParam, TVal> func, int max_size, int max_concurrent = 1, Action<TVal> on_release = null)
        {
            Contract.Requires(func != null);
            Contract.Requires(max_size > 0);
            Contract.Requires(max_concurrent > 0);

            this.func = func;

            concurrentOps = new BlockingCollection<TParam>(max_concurrent);

            innerCache = new MemoizingMRUCache<TParam, TVal>((x, _) => { 
                IObservable<TVal> ret;
                while (!concurrentOpsDict.TryGetValue(x, out ret)) { }
                this.Log().DebugFormat("Stashing {0} in cache", x);
                return ret.First();
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
            Contract.Requires(key != null);

            TVal ret;
            this.Log().DebugFormat("Async Get: {0}", key);
            lock (innerCache) {
                if (innerCache.TryGet(key, out ret)) {
                    this.Log().DebugFormat("Found key in cache: {0}", key);
                    return Observable.Return(ret);
                }
            }

            IObservable<TVal> observable;
            if (concurrentOpsDict.TryGetValue(key, out observable)) {
                this.Log().DebugFormat("Found pending item in cache: {0}", key);
                return observable;
            }

            var t = new Task<TVal>(() => func(key));
            var new_item = t.ToObservable();
            this.Log().Debug("Cache item not found, launching new task");
            t.Start();

            concurrentOps.Add(key);
            concurrentOpsDict.GetOrAdd(key, new_item);

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

        public void Dispose()
        {
            concurrentOps.CompleteAdding();
        }
    }
}