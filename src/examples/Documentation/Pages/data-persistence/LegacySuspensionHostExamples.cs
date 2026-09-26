// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.DataPersistence;

/// <summary>
/// Shows the legacy, untyped <see cref="ISuspensionHost"/> members on <see cref="SuspensionHost{TAppState}"/>: code
/// written against the older, object-based contract still works against a typed host, because the typed host
/// projects <see cref="ISuspensionHost.AppState"/> and <see cref="ISuspensionHost.CreateNewAppState"/> to and from
/// its own typed members.
/// </summary>
public static class LegacySuspensionHostExamples
{
    /// <summary>Setting <c>AppState</c> and <c>CreateNewAppState</c> through the untyped interface reads back through <c>AppStateValue</c> and <c>CreateNewAppStateTyped</c>.</summary>
    public static void SetStateThroughTheUntypedInterface()
    {
        using SuspensionHost<GameSaveState> host = new();
        ISuspensionHost untypedHost = host;

        untypedHost.CreateNewAppState = static () => new GameSaveState(Level: 1, Score: 0);
        untypedHost.AppState = new GameSaveState(Level: 3, Score: 500);

        Console.WriteLine(host.AppStateValue!.Level);
        Console.WriteLine(untypedHost.AppState is GameSaveState state ? state.Score : -1);
        Console.WriteLine(((GameSaveState)untypedHost.CreateNewAppState!()).Level);

        // Output:
        // 3
        // 500
        // 1
    }
}
