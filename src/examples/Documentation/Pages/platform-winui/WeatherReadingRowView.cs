// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Microsoft.UI.Xaml.Controls;

namespace ReactiveUI.Documentation.PlatformWinui;

/// <summary>
/// Shows one <see cref="WeatherReading"/>. It is a <see cref="ReactiveUserControl{TViewModel}"/>, so the view
/// locator can find it whenever something needs to host a reading, such as <see cref="AutoDataTemplateBindingHook"/>
/// hosting each row of a station's readings list.
/// </summary>
[System.Diagnostics.DebuggerDisplay("WeatherReadingRowView")]
public sealed class WeatherReadingRowView : ReactiveUserControl<WeatherReading>
{
    /// <summary>The label showing the current reading.</summary>
    private readonly TextBlock _label = new();

    /// <summary>Initializes a new instance of the <see cref="WeatherReadingRowView"/> class.</summary>
    public WeatherReadingRowView()
    {
        Content = _label;

        // ViewModel is a dependency property, not a CLR property that raises PropertyChanged, so the change is
        // observed the way any WinUI dependency property is: RegisterPropertyChangedCallback. ViewModelProperty
        // is the field the base class registers it under.
        _ = RegisterPropertyChangedCallback(ViewModelProperty, static (sender, _) =>
        {
            WeatherReadingRowView view = (WeatherReadingRowView)sender;
            WeatherReading? reading = view.BindingRoot;
            view._label.Text = reading is null
                ? "(no reading)"
                : $"{reading.StationName}: {reading.TemperatureCelsius:0.0} C";
        });
    }
}
