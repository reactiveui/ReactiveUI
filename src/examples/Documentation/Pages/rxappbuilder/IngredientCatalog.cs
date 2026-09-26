// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.Rxappbuilder;

/// <summary>A fixed, in-memory catalog of ingredients.</summary>
[System.Diagnostics.DebuggerDisplay("IngredientCatalog Count = {Ingredients.Count}")]
public sealed class IngredientCatalog : IIngredientCatalog
{
    /// <inheritdoc/>
    public IReadOnlyList<string> Ingredients { get; } = ["Flour", "Sugar", "Eggs"];
}
