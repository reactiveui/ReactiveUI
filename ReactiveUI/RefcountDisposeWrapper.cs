using System;
using System.Threading;

namespace ReactiveUI
{
    sealed class RefcountDisposeWrapper
    {
        IDisposable _inner;
        int refCount = 1;

        public RefcountDisposeWrapper(IDisposable inner) { _inner = inner; }

        public void AddRef()
        {
            Interlocked.Increment(ref refCount);
        }

        public void Release()
        {
            if (Interlocked.Decrement(ref refCount) == 0) {
                var inner = Interlocked.Exchange(ref _inner, null);
                inner.Dispose();
            }
        }
    }
}