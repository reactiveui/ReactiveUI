// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.Rxappbuilder;

/// <summary>The view model behind the pantry screen; the app keeps one instance for its whole lifetime.</summary>
[System.Diagnostics.DebuggerDisplay("PantryViewModel ItemsInStock = {ItemsInStock}")]
public sealed class PantryViewModel : ReactiveObject
{
    /// <summary>Gets or sets how many items are in stock.</summary>
    public int ItemsInStock
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = 12;
}
