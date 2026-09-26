// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.Rxappbuilder;

/// <summary>The view model behind an ingredient list; the app resolves a fresh one for every recipe it opens.</summary>
[System.Diagnostics.DebuggerDisplay("IngredientListViewModel Count = {Count}")]
public sealed class IngredientListViewModel : ReactiveObject
{
    /// <summary>Gets or sets how many ingredients the recipe needs.</summary>
    public int Count
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }
}
