// Copyright (c) 2019 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Reactive.Concurrency;
using Splat;

namespace ReactiveUI
{
    /// <summary>
    /// Mac platform registrations.
    /// </summary>
    /// <seealso cref="ReactiveUI.IWantsToRegisterStuff" />
    public class PlatformRegistrations : IWantsToRegisterStuff
    {
        /// <inheritdoc/>
        public void Register(Action<Func<object>, Type> registerFunction)
        {
            if (registerFunction == null)
            {
                throw new ArgumentNullException(nameof(registerFunction));
            }

            registerFunction(() => new PlatformOperations(), typeof(IPlatformOperations));
            registerFunction(() => new ComponentModelTypeConverter(), typeof(IBindingTypeConverter));
            registerFunction(() => new AppKitObservableForProperty(), typeof(ICreatesObservableForProperty));
            registerFunction(() => new TargetActionCommandBinder(), typeof(ICreatesCommandBinding));
            registerFunction(() => new DateTimeNSDateConverter(), typeof(IBindingTypeConverter));
            registerFunction(() => new KVOObservableForProperty(), typeof(ICreatesObservableForProperty));
            RxApp.TaskpoolScheduler = TaskPoolScheduler.Default;
            RxApp.MainThreadScheduler = new WaitForDispatcherScheduler(() => new NSRunloopScheduler());
            registerFunction(() => new AppSupportJsonSuspensionDriver(), typeof(ISuspensionDriver));
       }
    }
}
