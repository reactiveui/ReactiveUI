// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Microsoft.UI.Xaml.Controls;

namespace ReactiveUI.Documentation.PlatformWinui;

/// <summary>
/// A shorter view of a <see cref="WeatherReading"/>, for a host whose <c>ViewContract</c> asks for the "Compact"
/// layout. <see cref="ExcludeFromViewRegistrationAttribute"/> keeps it out of the generated view lookup, so it only
/// shows up where an example maps it to the "Compact" contract explicitly.
/// </summary>
[ExcludeFromViewRegistration]
[System.Diagnostics.DebuggerDisplay("WeatherReadingCompactRowView")]
public sealed class WeatherReadingCompactRowView : ReactiveUserControl<WeatherReading>
{
    /// <summary>The label showing the compact reading.</summary>
    private readonly TextBlock _label = new();

    /// <summary>Initializes a new instance of the <see cref="WeatherReadingCompactRowView"/> class.</summary>
    public WeatherReadingCompactRowView()
    {
        Content = _label;

        _ = RegisterPropertyChangedCallback(ViewModelProperty, static (sender, _) =>
        {
            WeatherReadingCompactRowView view = (WeatherReadingCompactRowView)sender;
            view._label.Text = view.BindingRoot is WeatherReading reading
                ? $"{reading.StationName}: {reading.TemperatureCelsius:0.0} C"
                : "(no reading)";
        });
    }
}
