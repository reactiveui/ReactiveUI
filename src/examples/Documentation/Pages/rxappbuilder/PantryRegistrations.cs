// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.Rxappbuilder;

/// <summary>Registers the suspension driver the recipe book uses to remember what was on screen.</summary>
[System.Diagnostics.DebuggerDisplay("PantryRegistrations")]
public sealed class PantryRegistrations : IWantsToRegisterStuff
{
    /// <inheritdoc/>
    public void Register(IRegistrar registrar) =>
        registrar.RegisterConstant<ISuspensionDriver>(static () => new InMemorySuspensionDriver());
}
