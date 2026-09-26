// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.PlatformWinui;

/// <summary>
/// The latest rain-radar image for one station. Its view, <see cref="RadarImageView"/>, is registered only with the
/// service locator, so the default hosts cannot find it.
/// </summary>
[System.Diagnostics.DebuggerDisplay("{StationName} radar")]
public sealed class RadarImageViewModel : ReactiveObject, IRoutableViewModel
{
    /// <summary>Initializes a new instance of the <see cref="RadarImageViewModel"/> class.</summary>
    /// <param name="hostScreen">The screen whose router shows the image.</param>
    /// <param name="stationName">The station the radar image is centered on.</param>
    public RadarImageViewModel(IScreen hostScreen, string stationName)
    {
        HostScreen = hostScreen;
        StationName = stationName;
    }

    /// <inheritdoc/>
    public string UrlPathSegment => $"radar/{StationName}";

    /// <inheritdoc/>
    public IScreen HostScreen { get; }

    /// <summary>Gets the station the radar image is centered on.</summary>
    public string StationName { get; }
}
