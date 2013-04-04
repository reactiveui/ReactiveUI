using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    public class Lazy<T>
    {
        object _gate = 42;

        Func<T> _valueFetcher;
        public Lazy(Func<T> valueFetcher)
        {
            _valueFetcher = valueFetcher;
        }

        public bool IsValueCreated
        {
            get { return _valueFetcher == null; }
        }

        T _Value;
        public T Value
        {
            get
            {
                lock (_gate)
                {
                    if (_valueFetcher != null)
                    {
                        _Value = _valueFetcher();
                        _valueFetcher = null;
                    }

                    return _Value;
                }
            }
        }
    }
}
