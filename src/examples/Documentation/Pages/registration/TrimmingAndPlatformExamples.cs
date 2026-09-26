// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.Registration;

/// <summary>Covers the two small helper types a registration module reaches for now and then.</summary>
public static class TrimmingAndPlatformExamples
{
    /// <summary>
    /// The host keeps a <see cref="NotAWeakReference"/> to each module it has registered, for a diagnostics screen that
    /// lists what is loaded. Unlike a <see cref="WeakReference"/>, its target is never collected while the reference
    /// itself is held, so the diagnostics screen can read it at any time.
    /// </summary>
    public static void TrackALoadedModule()
    {
        PlayerModule playerModule = new();
        NotAWeakReference loadedModule = new(playerModule);

        Console.WriteLine(ReferenceEquals(loadedModule.Target, playerModule));
        Console.WriteLine(loadedModule.IsAlive);

        // Output:
        // True
        // True
    }
}
