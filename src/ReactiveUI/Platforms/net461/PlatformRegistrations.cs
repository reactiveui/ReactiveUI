// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Reactive.Concurrency;

namespace ReactiveUI
{
    /// <summary>
    /// .NET Framework platform registrations.
    /// </summary>
    /// <seealso cref="ReactiveUI.IWantsToRegisterStuff" />
    public class PlatformRegistrations : IWantsToRegisterStuff
    {
        /// <inheritdoc/>
        public void Register(Action<Func<object>, Type> registerFunction)
        {
            registerFunction(() => new ComponentModelTypeConverter(), typeof(IBindingTypeConverter));
            RxApp.TaskpoolScheduler = TaskPoolScheduler.Default;
            RxApp.MainThreadScheduler = DefaultScheduler.Instance;
        }
    }
}
