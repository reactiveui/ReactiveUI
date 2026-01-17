// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;

namespace ReactiveUI.Drawing;

/// <summary>
/// Splat Drawing platform registrations.
/// </summary>
/// <seealso cref="IWantsToRegisterStuff" />
public class Registrations : IWantsToRegisterStuff
{
    /// <inheritdoc/>
    public void Register(IRegistrar registrar)
    {
        ArgumentExceptionHelper.ThrowIfNull(registrar);

#if NETFRAMEWORK || (NET5_0_OR_GREATER && WINDOWS)
        registrar.RegisterConstant<IBitmapLoader>(static () => new PlatformBitmapLoader());
#endif
    }
}
