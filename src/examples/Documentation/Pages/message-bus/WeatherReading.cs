// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Documentation.Messaging;

/// <summary>A single reading from the school's weather station.</summary>
/// <param name="TemperatureCelsius">The outside temperature at the time of the reading.</param>
[System.Diagnostics.DebuggerDisplay("{TemperatureCelsius}")]
public sealed record WeatherReading(double TemperatureCelsius);
