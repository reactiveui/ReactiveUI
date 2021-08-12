// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Reactive.Concurrency;
using System.Reactive.PlatformServices;

namespace ReactiveUI.Uno
{
    /// <summary>
    /// UWP platform registrations.
    /// </summary>
    /// <seealso cref="ReactiveUI.IWantsToRegisterStuff" />
    public class Registrations : IWantsToRegisterStuff
    {
        /// <inheritdoc/>
        public void Register(Action<Func<object>, Type> registerFunction)
        {
            if (registerFunction == null)
            {
                throw new ArgumentNullException(nameof(registerFunction));
            }

            registerFunction(() => new PlatformOperations(), typeof(IPlatformOperations));
            registerFunction(() => new ActivationForViewFetcher(), typeof(IActivationForViewFetcher));
            registerFunction(() => new DependencyObjectObservableForProperty(), typeof(ICreatesObservableForProperty));
            registerFunction(() => new StringConverter(), typeof(IBindingTypeConverter));
            registerFunction(() => new SingleToStringTypeConverter(), typeof(IBindingTypeConverter));
            registerFunction(() => new DoubleToStringTypeConverter(), typeof(IBindingTypeConverter));
            registerFunction(() => new DecimalToStringTypeConverter(), typeof(IBindingTypeConverter));
            registerFunction(() => new BooleanToVisibilityTypeConverter(), typeof(IBindingTypeConverter));
            registerFunction(() => new AutoDataTemplateBindingHook(), typeof(IPropertyBindingHook));

            // Re-enable once the obsolete code in Uno has been worked out.
            ////registerFunction(() => new WinRTAppDataDriver(), typeof(ISuspensionDriver));

#if NETSTANDARD
            if (WasmPlatformEnlightenmentProvider.IsWasm)
            {
                RxApp.TaskpoolScheduler = WasmScheduler.Default;
                RxApp.MainThreadScheduler = WasmScheduler.Default;
            }
            else
#endif
            {
                RxApp.TaskpoolScheduler = TaskPoolScheduler.Default;
                RxApp.MainThreadScheduler = new WaitForDispatcherScheduler(() => CoreDispatcherScheduler.Current);
            }
        }
    }
}
