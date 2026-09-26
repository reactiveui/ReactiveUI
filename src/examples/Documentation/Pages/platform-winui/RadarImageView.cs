// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Microsoft.UI.Xaml.Controls;

namespace ReactiveUI.Documentation.PlatformWinui;

/// <summary>
/// Shows a <see cref="RadarImageViewModel"/>. It stands in for a view from a radar vendor's control library, which
/// registers its views with the service locator and is built without the source generator. The
/// <see cref="ExcludeFromViewRegistrationAttribute"/> keeps this copy out of the generated view lookup in the same way.
/// </summary>
[ExcludeFromViewRegistration]
[System.Diagnostics.DebuggerDisplay("RadarImageView")]
public sealed class RadarImageView : ReactiveUserControl<RadarImageViewModel>
{
    /// <summary>The label standing in for the radar image.</summary>
    private readonly TextBlock _label = new();

    /// <summary>Initializes a new instance of the <see cref="RadarImageView"/> class.</summary>
    public RadarImageView()
    {
        Content = _label;
        _ = RegisterPropertyChangedCallback(ViewModelProperty, static (sender, _) =>
        {
            RadarImageView view = (RadarImageView)sender;
            view._label.Text = view.BindingRoot is RadarImageViewModel radar ? $"{radar.StationName} radar" : "(no radar)";
        });
    }

    /// <summary>Gets the text the view shows for its radar image.</summary>
    public string Summary => _label.Text;
}
