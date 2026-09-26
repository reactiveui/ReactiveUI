// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ReactiveUI.Device.Tests.Runner;

/// <summary>
/// Lets TUnit subscribe to <see cref="Console.CancelKeyPress"/> on Android and iOS, where the runtime has no console
/// signals and the subscription throws <see cref="PlatformNotSupportedException"/>.
/// </summary>
/// <remarks>
/// <para>
/// TUnit 1.69 subscribes to <see cref="Console.CancelKeyPress"/> when a run starts and skips it only in the browser, so
/// every run on a device fails before the first test. <see cref="Console"/> registers its signal handler once, when
/// its registration field is still empty. Filling that field with an inert registration makes the subscription a
/// no-op: a device app never receives Ctrl+C, and Microsoft.Testing.Platform still cancels the run through its own token.
/// </para>
/// <para>Remove this shim once TUnit skips the subscription on Android and iOS.</para>
/// </remarks>
internal static class ConsoleCancelKeyShim
{
    /// <summary>Fills the console's signal registrations with inert ones so subscribing to Ctrl+C cannot throw.</summary>
    internal static void Install()
    {
        if (!OperatingSystem.IsAndroid() && !OperatingSystem.IsIOS() && !OperatingSystem.IsTvOS())
        {
            return;
        }

        ref var sigInt = ref SigIntRegistration(null);
        ref var sigQuit = ref SigQuitRegistration(null);
        sigInt ??= (PosixSignalRegistration)RuntimeHelpers.GetUninitializedObject(typeof(PosixSignalRegistration));
        sigQuit ??= (PosixSignalRegistration)RuntimeHelpers.GetUninitializedObject(typeof(PosixSignalRegistration));
    }

    /// <summary>Reads <c>Console.s_sigIntRegistration</c>.</summary>
    /// <param name="console">Unused; identifies the owning type.</param>
    /// <returns>A reference to the field.</returns>
    [UnsafeAccessor(UnsafeAccessorKind.StaticField, Name = "s_sigIntRegistration")]
    private static extern ref PosixSignalRegistration? SigIntRegistration([UnsafeAccessorType("System.Console, System.Console")] object? console);

    /// <summary>Reads <c>Console.s_sigQuitRegistration</c>.</summary>
    /// <param name="console">Unused; identifies the owning type.</param>
    /// <returns>A reference to the field.</returns>
    [UnsafeAccessor(UnsafeAccessorKind.StaticField, Name = "s_sigQuitRegistration")]
    private static extern ref PosixSignalRegistration? SigQuitRegistration([UnsafeAccessorType("System.Console, System.Console")] object? console);
}
