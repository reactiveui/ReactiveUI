// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Microsoft.Maui.Controls;

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive.Maui.Internal;
#else
namespace ReactiveUI.Maui.Internal;
#endif

/// <summary>Shared layout builders for MAUI text and image item views.</summary>
internal static class ItemViewLayoutHelper
{
    /// <summary>The padding around the item content layout.</summary>
    internal const double ContentPadding = 16;

    /// <summary>The font size of the primary text label.</summary>
    private const double PrimaryFontSize = 16;

    /// <summary>The font size of the detail text label.</summary>
    private const double DetailFontSize = 12;

    /// <summary>The opacity applied to the detail text label.</summary>
    private const double DetailOpacity = 0.7;

    /// <summary>Creates the primary and detail text labels and initializes their text values.</summary>
    /// <param name="textLabel">Receives the created primary label.</param>
    /// <param name="detailLabel">Receives the created detail label.</param>
    /// <param name="text">The initial primary text.</param>
    /// <param name="detail">The initial detail text.</param>
    internal static void CreateTextLabels(out Label textLabel, out Label detailLabel, string? text, string? detail)
    {
        textLabel = new() { FontSize = PrimaryFontSize, VerticalOptions = LayoutOptions.Center, Text = text };
        detailLabel = new() { FontSize = DetailFontSize, VerticalOptions = LayoutOptions.Center, Opacity = DetailOpacity, Text = detail };
    }

    /// <summary>Creates the vertical stack layout containing the primary and detail labels.</summary>
    /// <param name="textLabel">The primary label.</param>
    /// <param name="detailLabel">The detail label.</param>
    /// <param name="padding">The padding around the stack.</param>
    /// <param name="fillHorizontal">Whether the stack fills horizontal space.</param>
    /// <returns>The configured <see cref="StackLayout"/>.</returns>
    internal static StackLayout CreateTextStack(Label textLabel, Label detailLabel, double padding, bool fillHorizontal)
    {
        var stack = new StackLayout { Orientation = StackOrientation.Vertical, VerticalOptions = LayoutOptions.Center, Padding = padding, Children = { textLabel, detailLabel } };

        if (fillHorizontal)
        {
            stack.HorizontalOptions = LayoutOptions.Fill;
        }

        return stack;
    }
}
