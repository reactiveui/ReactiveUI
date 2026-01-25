// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.ComponentModel;

namespace ReactiveUI.WinForms.Tests.Winforms.Mocks;

/// <summary>
/// A custom clickable component with Enabled property for testing non-generic EventHandler binding.
/// </summary>
public class CustomClickableComponentWithEnabled : Component
{
    /// <summary>
    /// Occurs when the component is clicked.
    /// </summary>
    public event EventHandler? Click;

    /// <summary>
    /// Gets or sets a value indicating whether the component is enabled.
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool Enabled { get; set; } = true;

    /// <summary>
    /// Performs a click.
    /// </summary>
    public void PerformClick() => Click?.Invoke(this, EventArgs.Empty);
}
