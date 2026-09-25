// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.Controls;

/// <summary>A console stand-in for a read-only piece of text on a screen.</summary>
[System.Diagnostics.DebuggerDisplay("Label Text = {Text}, IsVisible = {IsVisible}")]
public sealed class Label : ReactiveObject
{
    /// <summary>Gets or sets the text the label shows.</summary>
    public string Text
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = string.Empty;

    /// <summary>Gets or sets a value indicating whether the label is shown.</summary>
    public bool IsVisible
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = true;
}
