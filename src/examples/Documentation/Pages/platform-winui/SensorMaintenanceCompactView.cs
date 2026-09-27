// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Microsoft.UI.Xaml.Controls;

namespace ReactiveUI.Documentation.PlatformWinui;

/// <summary>
/// A shorter view of a <see cref="SensorMaintenanceViewModel"/>, for a host whose <c>ViewContract</c> asks for the
/// "Compact" layout. It implements only the non-generic <see cref="IViewFor"/>, the same way
/// <see cref="SensorMaintenanceView"/> does, so the source generator writes no lookup entry for it either; an
/// example maps it to the "Compact" contract explicitly.
/// </summary>
[System.Diagnostics.DebuggerDisplay("SensorMaintenanceCompactView")]
public sealed class SensorMaintenanceCompactView : UserControl, IViewFor
{
    /// <summary>The label showing the visit, in one shorter line.</summary>
    private readonly TextBlock _label = new();

    /// <summary>Initializes a new instance of the <see cref="SensorMaintenanceCompactView"/> class.</summary>
    public SensorMaintenanceCompactView() => Content = _label;

    /// <inheritdoc/>
    public object? ViewModel
    {
        get;
        set
        {
            field = value;
            _label.Text = value is SensorMaintenanceViewModel visit ? visit.StationName : "(no visit)";
        }
    }

    /// <summary>Gets the text the view shows for its visit.</summary>
    public string Summary => _label.Text;
}
