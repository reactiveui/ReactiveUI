// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.Registration;

/// <summary>The library feature's own registrations: one catalog, shared by every screen that reads it.</summary>
public sealed class TrackLibraryModule : IWantsToRegisterStuff
{
    /// <inheritdoc/>
    public void Register(IRegistrar registrar) =>
        registrar.RegisterLazySingleton<ITrackLibrary>(static () => new TrackLibrary());
}
