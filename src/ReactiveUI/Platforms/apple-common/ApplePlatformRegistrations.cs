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
/// <summary>The service registrations shared by every Apple platform.</summary>
internal static class ApplePlatformRegistrations
{
    /// <summary>Registers the services common to every Apple platform.</summary>
    /// <param name="registrar">The registrar the services are registered with.</param>
    internal static void Register(IRegistrar registrar)
    {
        ArgumentExceptionHelper.ThrowIfNull(registrar);

        registrar.RegisterConstant<IPlatformOperations>(static () => new PlatformOperations());

        if (!ModeDetector.InUnitTestRunner())
        {
            RxSchedulers.TaskpoolScheduler = Sequencer.Default;
            RxSchedulers.MainThreadScheduler = NSRunloopSequencer.Main;
        }

        registrar.RegisterConstant<ISuspensionDriver>(static () => new AppSupportJsonSuspensionDriver());
    }
}
