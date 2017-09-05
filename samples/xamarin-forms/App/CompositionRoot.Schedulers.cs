// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MS-PL license.
// See the LICENSE file in the project root for more information.

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
