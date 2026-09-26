// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.PlatformWinui;

/// <summary>The one alert banner the dashboard shows, hosted through the generic <c>RoutedViewHost&lt;TViewModel&gt;</c>.</summary>
[System.Diagnostics.DebuggerDisplay("{Message}")]
public sealed class AlertViewModel : ReactiveObject, IRoutableViewModel
{
    /// <summary>Initializes a new instance of the <see cref="AlertViewModel"/> class.</summary>
    /// <param name="hostScreen">The window the alert is shown in.</param>
    public AlertViewModel(IScreen hostScreen) => HostScreen = hostScreen;

    /// <inheritdoc/>
    public string UrlPathSegment => "alert";

    /// <inheritdoc/>
    public IScreen HostScreen { get; }

    /// <summary>Gets or sets the current alert text.</summary>
    public string Message
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = "No alerts";
}
