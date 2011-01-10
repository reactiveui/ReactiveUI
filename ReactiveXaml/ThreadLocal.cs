using System;
using System.Collections.Generic;
using System.Threading;

namespace ReactiveXaml
{
    public class ThreadLocal<T>
    {
        Func<T> _factory;
        Dictionary<int, T> _cache = new Dictionary<int,T>();

        public ThreadLocal() : this(() => default(T))
        {
        }

        public ThreadLocal(Func<T> valueFactory)
        {
            _factory = valueFactory;
        }

        public T Value {
            get {
                lock(_cache) {
                    int key = Thread.CurrentThread.ManagedThreadId;
                    if (_cache.ContainsKey(key)) {
                        return _cache[key];
                    }

                    return (_cache[key] = _factory());
                }
            }
            set {
                lock(_cache) {
                    int key = Thread.CurrentThread.ManagedThreadId;
                    _cache[key] = value;
                }
            }
        }
    }
}

// vim: tw=120 ts=4 sw=4 et :
