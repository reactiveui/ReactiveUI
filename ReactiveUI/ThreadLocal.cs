using System;
using System.Collections.Generic;

namespace System.Threading
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

#if WP7 || DOTNETISOLDANDSAD
namespace System
{
    

}
#endif

// vim: tw=120 ts=4 sw=4 et :
