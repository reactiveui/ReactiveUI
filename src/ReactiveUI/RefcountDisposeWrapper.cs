using System;
using System.Threading;

namespace ReactiveUI
{
    sealed class RefcountDisposeWrapper
    {
        IDisposable _inner;
        int refCount = 1;

        public RefcountDisposeWrapper(IDisposable inner) { this._inner = inner; }

        public void AddRef()
        {
            Interlocked.Increment(ref this.refCount);
        }

        public void Release()
        {
            if (Interlocked.Decrement(ref this.refCount) == 0) {
                var inner = Interlocked.Exchange(ref this._inner, null);
                inner.Dispose();
            }
        }
    }
}