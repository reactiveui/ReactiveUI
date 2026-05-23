// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Forms;

namespace ReactiveUI.WinForms.Tests.Winforms.Mocks;

/// <summary>
/// A third-party style control with a Value property and a corresponding Changed event.
/// </summary>
public class ThirdPartyControl : Control
{
    /// <summary>
    /// Backing field for the value.
    /// </summary>
    private string? _value;

    /// <summary>
    /// Occurs when the value changes.
    /// </summary>
    public event EventHandler? ValueChanged;

    /// <summary>
    /// Gets or sets the value.
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string? Value
    {
        get => _value;
        set
        {
            if (_value == value)
            {
                return;
            }

            _value = value;
            OnValueChanged();
        }
    }

    /// <summary>
    /// Raises the <see cref="ValueChanged"/> event.
    /// </summary>
    protected virtual void OnValueChanged() => ValueChanged?.Invoke(this, EventArgs.Empty);
}
