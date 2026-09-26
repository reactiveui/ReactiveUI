// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.Collections;

/// <summary>One line of stock in the shop's inventory.</summary>
/// <param name="name">The product's name.</param>
/// <param name="stock">The number of units on the shelf.</param>
[System.Diagnostics.DebuggerDisplay("{Name}: {Stock}")]
public sealed class Product(string name, int stock) : ReactiveObject
{
    /// <summary>Gets or sets the product's name.</summary>
    public string Name
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = name;

    /// <summary>Gets or sets the number of units on the shelf.</summary>
    public int Stock
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = stock;
}
