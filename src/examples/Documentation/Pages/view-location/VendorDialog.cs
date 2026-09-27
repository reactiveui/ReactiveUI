// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.ViewLocation;

/// <summary>
/// A stand-in for a base class a third-party dialog library ships: an app must derive from it to show a dialog, and
/// it comes with its own loosely typed context object, the way a host framework's <c>DataContext</c> or
/// <c>BindingContext</c> works.
/// </summary>
[System.Diagnostics.DebuggerDisplay("VendorDialog DialogContext = {DialogContext}")]
public class VendorDialog
{
    /// <summary>Raised when <see cref="DialogContext"/> changes.</summary>
    public event EventHandler? DialogContextChanged;

    /// <summary>Raised when the dialog is shown.</summary>
    public event EventHandler? Opened;

    /// <summary>Raised when the dialog is closed.</summary>
    public event EventHandler? Closed;

    /// <summary>Gets or sets the context object the dialog's controls bind against.</summary>
    public object? DialogContext
    {
        get;
        set
        {
            if (ReferenceEquals(field, value))
            {
                return;
            }

            field = value;
            DialogContextChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    /// <summary>Shows the dialog, raising <see cref="Opened"/>.</summary>
    public void Show() => Opened?.Invoke(this, EventArgs.Empty);

    /// <summary>Closes the dialog, raising <see cref="Closed"/>.</summary>
    public void Close() => Closed?.Invoke(this, EventArgs.Empty);
}
