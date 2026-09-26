// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace ReactiveUI.Documentation.PlatformWinui;

/// <summary>
/// The detail view for one station. Its storm badge and override panel are shown or hidden with
/// <see cref="BooleanToVisibilityTypeConverter"/> and <see cref="VisibilityToBooleanTypeConverter"/>, the panel is
/// wrapped in a <see cref="TransitioningContentControl"/>, and it prints what <see cref="PlatformOperations"/>
/// reports for the device orientation.
/// </summary>
[System.Diagnostics.DebuggerDisplay("StationDetailPageView")]
public sealed class StationDetailPageView : ReactivePage<StationDetailPageViewModel>
{
    /// <summary>Converts a bool to a <see cref="Visibility"/>, the direction the storm badge and override panel need.</summary>
    private static readonly BooleanToVisibilityTypeConverter ToVisibility = new();

    /// <summary>Converts a <see cref="Visibility"/> back to a bool, the direction reading a control's state needs.</summary>
    private static readonly VisibilityToBooleanTypeConverter ToBoolean = new();

    /// <summary>The label showing the reading and the platform's reported orientation.</summary>
    private readonly TextBlock _readingLabel = new();

    /// <summary>The badge shown only while the station reports a storm.</summary>
    private readonly TextBlock _stormBadge = new() { Text = "STORM" };

    /// <summary>The panel an operator can show manually, wrapped in a transition so showing it animates.</summary>
    private readonly TransitioningContentControl _overridePanel = new() { Content = new TextBlock { Text = "Manual override active" } };

    /// <summary>Initializes a new instance of the <see cref="StationDetailPageView"/> class.</summary>
    public StationDetailPageView()
    {
        StackPanel panel = new();
        panel.Children.Add(_readingLabel);
        panel.Children.Add(_stormBadge);
        panel.Children.Add(_overridePanel);
        Content = panel;

        this.WhenActivated(disposables =>
        {
            StationDetailPageViewModel? viewModel = ViewModel;
            if (viewModel is null)
            {
                return;
            }

            string? orientation = new PlatformOperations().GetOrientation();
            _readingLabel.Text = $"{viewModel.Reading.StationName}: {viewModel.Reading.TemperatureCelsius:0.0} C "
                + $"(orientation: {orientation ?? "unknown"})";

            _ = ToVisibility.TryConvert(viewModel.Reading.IsStormy, BooleanToVisibilityHint.None, out Visibility stormVisibility);
            _stormBadge.Visibility = stormVisibility;

            IDisposable subscription = viewModel.WhenAnyValue(vm => vm.ManualOverrideVisible)
                .Subscribe(visible =>
                {
                    // Inverse: the panel is shown when the flag is false (nothing overridden yet needs no extra chrome)
                    // and hidden once an operator sets it, exercising the hint alongside the default conversion.
                    _ = ToVisibility.TryConvert(visible, BooleanToVisibilityHint.Inverse, out Visibility panelVisibility);
                    _overridePanel.Visibility = panelVisibility;

                    _ = ToBoolean.TryConvert(_overridePanel.Visibility, BooleanToVisibilityHint.Inverse, out bool roundTripped);
                    System.Diagnostics.Debug.Assert(roundTripped == visible, "The hint should round-trip the original flag.");
                });
            disposables(subscription);
        });
    }
}
