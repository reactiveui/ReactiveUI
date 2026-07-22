// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if NETFRAMEWORK || (NET5_0_OR_GREATER && WINDOWS)
using Splat;
#endif

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive.Drawing;
#else
namespace ReactiveUI.Drawing;
#endif

/// <summary>Splat Drawing platform registrations.</summary>
/// <seealso cref="IWantsToRegisterStuff" />
public class Registrations : IWantsToRegisterStuff
{
    /// <inheritdoc/>
#if NETFRAMEWORK || (NET5_0_OR_GREATER && WINDOWS)
    public void Register(IRegistrar registrar)
    {
        ArgumentExceptionHelper.ThrowIfNull(registrar);
        registrar.RegisterConstant<IBitmapLoader>(static () => new PlatformBitmapLoader());
    }
#else
    public void Register(IRegistrar registrar) => ArgumentExceptionHelper.ThrowIfNull(registrar);
#endif
}
