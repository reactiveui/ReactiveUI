// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MS-PL license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;
using Genesis.Ensure;

namespace Utility
{
    public abstract class DisposableBase : object, IDisposable
    {
        private const int DisposalNotStarted = 0;
        private const int DisposalStarted = 1;
        private const int DisposalComplete = 2;

        // see the constants defined above for valid values
        private int _disposeStage;

        protected bool IsDisposing
            => Interlocked.CompareExchange(ref _disposeStage, DisposalStarted, DisposalStarted) == DisposalStarted;

        protected bool IsDisposed
            => Interlocked.CompareExchange(ref _disposeStage, DisposalComplete, DisposalComplete) == DisposalComplete;

        protected bool IsDisposedOrDisposing
            =>
                Interlocked.CompareExchange(ref _disposeStage, DisposalNotStarted, DisposalNotStarted) !=
                DisposalNotStarted;

        protected virtual string ObjectName => GetType().FullName;

        public void Dispose()
        {
            if (Interlocked.CompareExchange(ref _disposeStage, DisposalStarted, DisposalNotStarted) != DisposalNotStarted)
            {
                return;
            }

            OnDisposing();
            Disposing = null;

            Dispose(true);
            MarkAsDisposed();
        }

#if DEBUG

        ~DisposableBase()
        {
            //System.Diagnostics.Debug.WriteLine("Failed to proactively dispose of object, so it is being finalized: {0}.", this.ObjectName);
            Dispose(false);
        }

#endif

        public event EventHandler Disposing;

        protected void VerifyNotDisposing() =>
            Ensure.Condition(!IsDisposing, () => new ObjectDisposedException(ObjectName));

        protected void VerifyNotDisposed() =>
            Ensure.Condition(!IsDisposed, () => new ObjectDisposedException(ObjectName));

        protected void VerifyNotDisposedOrDisposing() =>
            Ensure.Condition(!IsDisposedOrDisposing, () => new ObjectDisposedException(ObjectName));

        protected virtual void Dispose(bool disposing)
        {
        }

        protected virtual void OnDisposing() =>
            Disposing?.Invoke(this, EventArgs.Empty);

        protected void MarkAsDisposed()
        {
            GC.SuppressFinalize(this);
            Interlocked.Exchange(ref _disposeStage, DisposalComplete);
        }
    }
}
