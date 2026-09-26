// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.PlatformBlendDrawing;

/// <summary>
/// The weather station's state. <see cref="StateChanges"/> drives <see cref="Blend.FollowObservableStateBehavior"/>,
/// and <see cref="RaiseAlert"/> drives <see cref="Blend.ObservableTrigger"/>.
/// </summary>
[System.Diagnostics.DebuggerDisplay("{State}")]
public sealed class WeatherStateViewModel : ReactiveObject
{
    /// <summary>Initializes a new instance of the <see cref="WeatherStateViewModel"/> class.</summary>
    public WeatherStateViewModel()
    {
        RaiseAlert = ReactiveCommand.Create<string, object>(static message => message);
        StateChanges = this.WhenAnyValue(viewModel => viewModel.State);
    }

    /// <summary>Gets or sets the current condition: "Calm", "Windy" or "Stormy". Named after a WPF <c>VisualState</c>.</summary>
    public string State
    {
        get;
        set => this.RaiseAndSetIfChanged(ref field, value);
    } = "Calm";

    /// <summary>Gets the stream <see cref="Blend.FollowObservableStateBehavior"/> follows.</summary>
    public IObservable<string> StateChanges { get; }

    /// <summary>Gets the command whose output stream <see cref="Blend.ObservableTrigger"/> watches; each execution is one alert.</summary>
    public ReactiveCommand<string, object> RaiseAlert { get; }
}
