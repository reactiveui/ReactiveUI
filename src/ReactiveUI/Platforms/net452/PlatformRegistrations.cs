﻿// Licensed to the .NET Foundation under one or more agreements. 
// The .NET Foundation licenses this file to you under the MS-PL license. 
// See the LICENSE file in the project root for more information. 

using System;
using System.Reactive.Concurrency;

namespace ReactiveUI
{
    public class PlatformRegistrations : IWantsToRegisterStuff
    {
        public void Register(Action<Func<object>, Type> registerFunction)
        {
            registerFunction(() => new ComponentModelTypeConverter(), typeof(IBindingTypeConverter));
            RxApp.TaskpoolScheduler = TaskPoolScheduler.Default;
            RxApp.MainThreadScheduler = new WaitForDispatcherScheduler(() => DispatcherScheduler.Current);
        }
    }
}
