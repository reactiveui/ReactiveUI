// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics;
using System.Runtime.CompilerServices;

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive;
#else
namespace ReactiveUI;
#endif
/// <summary>UIKit platform registrations.</summary>
/// <seealso cref="IWantsToRegisterStuff" />
[Preserve(AllMembers = true)]
public class PlatformRegistrations : IWantsToRegisterStuff
{
    /// <inheritdoc/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Register(IRegistrar registrar) =>
        ApplePlatformRegistrations.Register(
            registrar,
            static () => new UIKitObservableForProperty(),
            static () => new UIKitCommandBinders());
}
