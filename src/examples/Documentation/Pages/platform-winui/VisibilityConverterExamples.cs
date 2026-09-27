// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using Microsoft.UI.Xaml;

namespace ReactiveUI.Documentation.PlatformWinui;

/// <summary>
/// Shows the parts of <see cref="BooleanToVisibilityTypeConverter"/>, <see cref="VisibilityToBooleanTypeConverter"/>
/// and <see cref="BooleanToVisibilityHint"/> <see cref="StationDetailPageView"/> does not already exercise through a
/// binding: the affinity score ReactiveUI's binding resolution asks each converter for, and the one hint WinUI's
/// <see cref="Visibility"/> has no member for.
/// </summary>
public static class VisibilityConverterExamples
{
    /// <summary>
    /// <see cref="BooleanToVisibilityHint.UseHidden"/> asks for <c>Visibility.Hidden</c> instead of
    /// <c>Visibility.Collapsed</c> for the non-visible value. WinUI's <see cref="Visibility"/> enum has no
    /// <c>Hidden</c> member, so the converter ignores the hint there and always returns <c>Collapsed</c>.
    /// </summary>
    public static void UseHiddenHasNoEffectOnWinUI()
    {
        BooleanToVisibilityTypeConverter converter = new();

        _ = converter.TryConvert(false, BooleanToVisibilityHint.None, out Visibility none);
        _ = converter.TryConvert(false, BooleanToVisibilityHint.UseHidden, out Visibility useHidden);

        Console.WriteLine(none);
        Console.WriteLine(useHidden);
        Console.WriteLine(none == useHidden);

        // Output:
        // Collapsed
        // Collapsed
        // True
    }

    /// <summary>
    /// <c>GetAffinityForObjects</c> reports how well a converter matches a binding; ReactiveUI's binding resolution
    /// calls it on every registered converter and picks the one with the highest score.
    /// </summary>
    public static void GetAffinityForObjectsReportsTheBuiltInScore()
    {
        BooleanToVisibilityTypeConverter toVisibility = new();
        VisibilityToBooleanTypeConverter toBoolean = new();

        Console.WriteLine(toVisibility.GetAffinityForObjects());
        Console.WriteLine(toBoolean.GetAffinityForObjects());

        // Output:
        // 2
        // 2
    }
}
