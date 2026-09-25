// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.Controls;

/// <summary>A console stand-in for a check box. Setting <see cref="IsChecked"/> is what ticking it does.</summary>
[System.Diagnostics.DebuggerDisplay("CheckBox IsChecked = {IsChecked}")]
public sealed class CheckBox : ReactiveObject
{
    /// <summary>Gets or sets a value indicating whether the box is ticked.</summary>
    public bool IsChecked
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }
}
