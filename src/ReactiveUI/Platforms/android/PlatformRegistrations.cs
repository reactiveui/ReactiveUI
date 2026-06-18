// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Splat;

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive;
#else
namespace ReactiveUI;
#endif
/// <summary>Android platform registrations.</summary>
/// <seealso cref="IWantsToRegisterStuff" />
public class PlatformRegistrations : IWantsToRegisterStuff
{
    /// <inheritdoc/>
    public void Register(IRegistrar registrar)
    {
        ArgumentExceptionHelper.ThrowIfNull(registrar);

        registrar.RegisterConstant<IPlatformOperations>(static () => new PlatformOperations());
        registrar.RegisterConstant<IBindingFallbackConverter>(static () => new ComponentModelFallbackConverter());
        registrar.RegisterConstant<ICreatesObservableForProperty>(static () => new AndroidObservableForWidgets());
        registrar.RegisterConstant<ICreatesCommandBinding>(static () => new AndroidCommandBinders());

        if (!ModeDetector.InUnitTestRunner())
        {
            RxSchedulers.TaskpoolScheduler = TaskPoolSequencer.Default;
            RxSchedulers.MainThreadScheduler = HandlerSequencer.Main;
        }

        registrar.RegisterConstant<ISuspensionDriver>(static () => new BundleSuspensionDriver());
    }
}
