// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.PlatformWinui;

/// <summary>The detail page for one station's reading.</summary>
[System.Diagnostics.DebuggerDisplay("{UrlPathSegment}: {Reading}")]
public sealed class StationDetailPageViewModel : ReactiveObject, IRoutableViewModel
{
    /// <summary>Initializes a new instance of the <see cref="StationDetailPageViewModel"/> class.</summary>
    /// <param name="hostScreen">The window the page is shown in.</param>
    /// <param name="reading">The reading the page shows.</param>
    public StationDetailPageViewModel(IScreen hostScreen, WeatherReading reading)
    {
        HostScreen = hostScreen;
        Reading = reading;
        GoBack = ReactiveCommand.CreateFromObservable(() => HostScreen.Router.NavigateBack.Execute());
    }

    /// <inheritdoc/>
    public string UrlPathSegment => $"stations/{Reading.StationName}";

    /// <inheritdoc/>
    public IScreen HostScreen { get; }

    /// <summary>Gets the reading the page shows.</summary>
    public WeatherReading Reading { get; }

    /// <summary>Gets the command that returns to the station list.</summary>
    public ReactiveCommand<RxVoid, IRoutableViewModel> GoBack { get; }

    /// <summary>Gets or sets a value indicating whether the operator has manually shown the override panel.</summary>
    public bool ManualOverrideVisible
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    }
}
