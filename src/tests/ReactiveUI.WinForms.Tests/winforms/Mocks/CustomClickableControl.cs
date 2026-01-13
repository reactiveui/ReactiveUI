// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Forms;

namespace ReactiveUI.WinForms.Tests.Winforms.Mocks;

/// <summary>
/// Custom clickable control.
/// </summary>
public class CustomClickableControl : Control
{
    /// <summary>
    /// Performs the click.
    /// </summary>
    public void PerformClick() => InvokeOnClick(this, EventArgs.Empty);

    /// <summary>
    /// Raises the mouse click event.
    /// </summary>
    /// <param name="args">The <see cref="MouseEventArgs"/> instance containing the event data.</param>
    public void RaiseMouseClickEvent(MouseEventArgs args) => OnMouseClick(args);

    /// <summary>
    /// Raises the mouse up event.
    /// </summary>
    /// <param name="args">The <see cref="MouseEventArgs"/> instance containing the event data.</param>
    public void RaiseMouseUpEvent(MouseEventArgs args) => OnMouseUp(args);
}
