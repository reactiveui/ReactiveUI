// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.Registration;

/// <summary>The player feature's own registrations: a new engine every time one is asked for.</summary>
public sealed class PlayerModule : IWantsToRegisterStuff
{
    /// <inheritdoc/>
    public void Register(IRegistrar registrar) =>
        registrar.Register<IPlaybackEngine>(static () => new PlaybackEngine());
}
