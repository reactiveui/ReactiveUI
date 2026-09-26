// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.Registration;

/// <summary>Registers every service through the contract-taking overload of <see cref="IRegistrar"/>'s three registration methods.</summary>
public sealed class ContractModule : IWantsToRegisterStuff
{
    /// <inheritdoc/>
    public void Register(IRegistrar registrar)
    {
        registrar.Register<IPlaybackEngine>(static () => new PlaybackEngine(), "preview");
        registrar.RegisterConstant<IPlatformOperations>(static () => new MobileOrientationOperations(), "mobile");
        registrar.RegisterConstant<IPlatformOperations>(static () => new DesktopOrientationOperations(), "desktop");
        registrar.RegisterLazySingleton<ITrackLibrary>(static () => new TrackLibrary(), "shared");
    }
}
