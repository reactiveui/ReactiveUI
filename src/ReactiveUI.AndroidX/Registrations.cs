// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Android.OS;

namespace ReactiveUI.AndroidX;

/// <summary>
/// AndroidX platform registrations.
/// </summary>
/// <seealso cref="IWantsToRegisterStuff" />
public class Registrations : IWantsToRegisterStuff
{
    /// <inheritdoc/>
    public void Register(IRegistrar registrar)
    {
        ArgumentExceptionHelper.ThrowIfNull(registrar);

        // Leverage core Android platform registrations already present in ReactiveUI.Platforms android.
        // This ensures IPlatformOperations, binding converters, and schedulers are configured.
        new PlatformRegistrations().Register(registrar);

        // AndroidX specific registrations could be added here if needed in the future.

        // Ensure a SynchronizationContext exists on Android when not in unit tests.
        if (ModeDetector.InUnitTestRunner() || Looper.MyLooper() is not null)
        {
            return;
        }

        Looper.Prepare();
    }
}
