// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Splat;
using Splat.Builder;

namespace ReactiveUI.Documentation.Rxappbuilder;

/// <summary>A plain Splat module, the kind written before the builder's own registration methods existed.</summary>
[System.Diagnostics.DebuggerDisplay("PantryModule")]
public sealed class PantryModule : IModule
{
    /// <inheritdoc/>
    public void Configure(IMutableDependencyResolver resolver) =>
        resolver.RegisterConstant<IIngredientCatalog>(new IngredientCatalog());
}
