// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.PlatformWinui;

/// <summary>
/// A maintenance visit booked for one station's sensors. Its view, <see cref="SensorMaintenanceView"/>, is added to
/// the view locator with <c>Map</c>, and the default hosts find it there.
/// </summary>
[System.Diagnostics.DebuggerDisplay("{StationName}: {Task}")]
public sealed class SensorMaintenanceViewModel : ReactiveObject, IRoutableViewModel
{
    /// <summary>Initializes a new instance of the <see cref="SensorMaintenanceViewModel"/> class.</summary>
    /// <param name="hostScreen">The screen whose router shows the visit.</param>
    /// <param name="stationName">The station the visit is booked for.</param>
    /// <param name="task">What the technician does on the visit.</param>
    public SensorMaintenanceViewModel(IScreen hostScreen, string stationName, string task)
    {
        HostScreen = hostScreen;
        StationName = stationName;
        Task = task;
    }

    /// <inheritdoc/>
    public string UrlPathSegment => $"maintenance/{StationName}";

    /// <inheritdoc/>
    public IScreen HostScreen { get; }

    /// <summary>Gets the station the visit is booked for.</summary>
    public string StationName { get; }

    /// <summary>Gets what the technician does on the visit.</summary>
    public string Task { get; }
}
