// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.PlatformWinui;

/// <summary>A single reading from a weather station.</summary>
[System.Diagnostics.DebuggerDisplay("{StationName}: {TemperatureCelsius}")]
public sealed class WeatherReading
{
    /// <summary>Initializes a new instance of the <see cref="WeatherReading"/> class.</summary>
    /// <param name="stationName">The name of the station that took the reading.</param>
    /// <param name="temperatureCelsius">The temperature, in degrees Celsius.</param>
    /// <param name="isStormy">Whether the station is currently reporting a storm.</param>
    public WeatherReading(string stationName, double temperatureCelsius, bool isStormy)
    {
        StationName = stationName;
        TemperatureCelsius = temperatureCelsius;
        IsStormy = isStormy;
    }

    /// <summary>Gets the name of the station that took the reading.</summary>
    public string StationName { get; }

    /// <summary>Gets the temperature, in degrees Celsius.</summary>
    public double TemperatureCelsius { get; }

    /// <summary>Gets a value indicating whether the station is currently reporting a storm.</summary>
    public bool IsStormy { get; }
}
