// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive.Concurrency;

namespace ReactiveUI;

/// <summary>
/// .NET Framework platform registrations.
/// </summary>
/// <seealso cref="ReactiveUI.IWantsToRegisterStuff" />
public class PlatformRegistrations : IWantsToRegisterStuff
{
    /// <inheritdoc/>
    public void Register(IRegistrar registrar)
    {
        ArgumentExceptionHelper.ThrowIfNull(registrar);

        registrar.RegisterConstant<IBindingFallbackConverter>(static () => new ComponentModelFallbackConverter());

        if (ModeDetector.InUnitTestRunner())
        {
            return;
        }

        RxSchedulers.TaskpoolScheduler = TaskPoolScheduler.Default;
        RxSchedulers.MainThreadScheduler = DefaultScheduler.Instance;
    }
}
