using System;
using System.Reactive.Concurrency;
using System.Threading;

namespace App
{
    public abstract partial class CompositionRoot
    {
        protected readonly Lazy<IScheduler> _mainScheduler;
        protected readonly Lazy<IScheduler> _taskPoolScheduler;

        private IScheduler CreateMainScheduler() => new SynchronizationContextScheduler(SynchronizationContext.Current);

        private IScheduler CreateTaskPoolScheduler() => TaskPoolScheduler.Default;
    }
}
