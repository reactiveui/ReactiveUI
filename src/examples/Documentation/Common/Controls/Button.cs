// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.Controls;

/// <summary>
/// A console stand-in for a push button. It raises <see cref="Click"/> when <see cref="PerformClick"/> is called,
/// the way a real button does when the user presses it, and reports whether it is enabled.
/// </summary>
[System.Diagnostics.DebuggerDisplay("Button IsEnabled = {IsEnabled}")]
public sealed class Button : ReactiveObject
{
    /// <summary>Raised when the button is pressed.</summary>
    public event EventHandler? Click;

    /// <summary>Gets or sets a value indicating whether the button accepts presses.</summary>
    public bool IsEnabled
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = true;

    /// <summary>Presses the button, as a user would, when it is enabled.</summary>
    public void PerformClick()
    {
        if (!IsEnabled)
        {
            return;
        }

        Click?.Invoke(this, EventArgs.Empty);
    }
}
