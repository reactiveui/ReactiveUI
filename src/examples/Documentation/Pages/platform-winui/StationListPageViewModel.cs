// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.PlatformWinui;

/// <summary>The page that lists every station's latest reading. Opening one navigates to its detail page.</summary>
[System.Diagnostics.DebuggerDisplay("{UrlPathSegment}")]
public sealed class StationListPageViewModel : ReactiveObject, IRoutableViewModel
{
    /// <summary>Initializes a new instance of the <see cref="StationListPageViewModel"/> class.</summary>
    /// <param name="hostScreen">The window the page is shown in.</param>
    /// <param name="readings">The readings the page lists, one per station.</param>
    public StationListPageViewModel(IScreen hostScreen, IReadOnlyList<WeatherReading> readings)
    {
        HostScreen = hostScreen;
        Readings = readings;
        OpenDetail = ReactiveCommand.CreateFromObservable<WeatherReading, IRoutableViewModel>(
            reading => HostScreen.Router.Navigate.Execute(new StationDetailPageViewModel(HostScreen, reading)));
    }

    /// <inheritdoc/>
    public string UrlPathSegment => "stations";

    /// <inheritdoc/>
    public IScreen HostScreen { get; }

    /// <summary>Gets the latest reading from every station.</summary>
    public IReadOnlyList<WeatherReading> Readings { get; }

    /// <summary>Gets the command that opens a station's detail page.</summary>
    public ReactiveCommand<WeatherReading, IRoutableViewModel> OpenDetail { get; }
}
