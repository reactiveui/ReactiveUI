// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Microsoft.UI.Xaml.Controls;

namespace ReactiveUI.Documentation.PlatformWinui;

/// <summary>
/// Shows a <see cref="SensorMaintenanceViewModel"/>. It implements only the non-generic <see cref="IViewFor"/>, so the
/// source generator writes no lookup entry for it. The app adds it to the view locator with <c>Map</c> instead.
/// </summary>
[System.Diagnostics.DebuggerDisplay("SensorMaintenanceView")]
public sealed class SensorMaintenanceView : UserControl, IViewFor
{
    /// <summary>The label showing the visit.</summary>
    private readonly TextBlock _label = new();

    /// <summary>Initializes a new instance of the <see cref="SensorMaintenanceView"/> class.</summary>
    public SensorMaintenanceView() => Content = _label;

    /// <inheritdoc/>
    public object? ViewModel
    {
        get;
        set
        {
            field = value;
            _label.Text = value is SensorMaintenanceViewModel visit
                ? $"{visit.StationName}: {visit.Task}"
                : "(no visit)";
        }
    }

    /// <summary>Gets the text the view shows for its visit.</summary>
    public string Summary => _label.Text;
}
