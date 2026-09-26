// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.Rxappbuilder;

/// <summary>The view model behind the shopping list screen; the app keeps a single one for the whole session.</summary>
[System.Diagnostics.DebuggerDisplay("ShoppingListViewModel Items = {Items.Count}")]
public sealed class ShoppingListViewModel : ReactiveObject
{
    /// <summary>Gets the ingredients still to buy.</summary>
    public List<string> Items { get; } = ["Tomatoes", "Basil"];
}
