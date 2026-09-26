// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Microsoft.Maui;
using ReactiveUI.Maui;

namespace ReactiveUI.Documentation.PlatformMaui;

/// <summary>
/// Shows <see cref="BooleanToVisibilityTypeConverter"/> and <see cref="VisibilityToBooleanTypeConverter"/>, which
/// <c>Bind</c> and <c>OneWayBind</c> use to convert between a <see cref="bool"/> view model property and a MAUI
/// <see cref="Visibility"/> property, and <see cref="BooleanToVisibilityHint"/>, the conversion hint they both read.
/// </summary>
public static class VisibilityConverterExamples
{
    /// <summary><see cref="BooleanToVisibilityHint.None"/> maps true to visible and false to collapsed; the other hints change that mapping.</summary>
    public static void BooleanToVisibilityAppliesEachHint()
    {
        BooleanToVisibilityTypeConverter converter = new();

        _ = converter.TryConvert(true, BooleanToVisibilityHint.None, out Visibility visible);
        _ = converter.TryConvert(false, BooleanToVisibilityHint.None, out Visibility collapsed);
        _ = converter.TryConvert(false, BooleanToVisibilityHint.UseHidden, out Visibility hidden);
        _ = converter.TryConvert(true, BooleanToVisibilityHint.Inverse, out Visibility invertedTrue);

        Console.WriteLine(visible);
        Console.WriteLine(collapsed);
        Console.WriteLine(hidden);
        Console.WriteLine(invertedTrue);

        // Output:
        // Visible
        // Collapsed
        // Hidden
        // Collapsed
    }

    /// <summary><see cref="VisibilityToBooleanTypeConverter"/> converts back, for a two-way bind.</summary>
    public static void VisibilityToBooleanConvertsBack()
    {
        VisibilityToBooleanTypeConverter converter = new();

        _ = converter.TryConvert(Visibility.Visible, BooleanToVisibilityHint.None, out bool visibleIsTrue);
        _ = converter.TryConvert(Visibility.Collapsed, BooleanToVisibilityHint.None, out bool collapsedIsFalse);
        _ = converter.TryConvert(Visibility.Visible, BooleanToVisibilityHint.Inverse, out bool invertedVisibleIsFalse);

        Console.WriteLine(visibleIsTrue);
        Console.WriteLine(collapsedIsFalse);
        Console.WriteLine(invertedVisibleIsFalse);

        // Output:
        // True
        // False
        // False
    }
}
