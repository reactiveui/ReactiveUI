// Licensed to the .NET Foundation under one or more agreements. 
// The .NET Foundation licenses this file to you under the MIT license. 
// See the LICENSE file in the project root for more information. 

using System;
using System.Reactive.Concurrency;

namespace ReactiveUI
{
    public class PlatformRegistrations : IWantsToRegisterStuff
    {
        public void Register(Action<Func<object>, Type> registerFunction)
        {
            registerFunction(() => new PlatformOperations(), typeof(IPlatformOperations));
            registerFunction(() => new ComponentModelTypeConverter(), typeof(IBindingTypeConverter));
            registerFunction(() => new AndroidObservableForWidgets(), typeof(ICreatesObservableForProperty));
            registerFunction(() => new AndroidCommandBinders(), typeof(ICreatesCommandBinding));
            RxApp.TaskpoolScheduler = TaskPoolScheduler.Default;
            RxApp.MainThreadScheduler = HandlerScheduler.MainThreadScheduler;
            registerFunction(() => new BundleSuspensionDriver(), typeof(ISuspensionDriver));
        }
    }
}
