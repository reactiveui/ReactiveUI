// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Android.OS;

namespace ReactiveUI.AndroidX;

/// <summary>
/// AndroidX platform registrations.
/// </summary>
/// <seealso cref="ReactiveUI.IWantsToRegisterStuff" />
public class Registrations : IWantsToRegisterStuff
{
    /// <inheritdoc/>
#if NET6_0_OR_GREATER
    [RequiresUnreferencedCode("Uses reflection to create instances of types.")]
    [RequiresDynamicCode("Uses reflection to create instances of types.")]
#endif
    public void Register(Action<Func<object>, Type> registerFunction)
    {
        ArgumentExceptionHelper.ThrowIfNull(registerFunction);

        // Leverage core Android platform registrations already present in ReactiveUI.Platforms android.
        // This ensures IPlatformOperations, binding converters, and schedulers are configured.
        new PlatformRegistrations().Register(registerFunction);

        // AndroidX specific registrations could be added here if needed in the future.

        // Ensure a SynchronizationContext exists on Android when not in unit tests.
        if (!ModeDetector.InUnitTestRunner() && Looper.MyLooper() is null)
        {
            Looper.Prepare();
        }
    }
}
