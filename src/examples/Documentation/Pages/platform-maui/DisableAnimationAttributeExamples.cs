// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Maui;

namespace ReactiveUI.Documentation.PlatformMaui;

/// <summary>Shows <see cref="DisableAnimationAttribute"/>, which <see cref="RoutedViewHost"/> checks before an animated push.</summary>
public static class DisableAnimationAttributeExamples
{
    /// <summary>A page marked with <see cref="DisableAnimationAttribute"/> pushes onto a <see cref="RoutedViewHost"/> without a transition.</summary>
    public static void MarksAPageToSkipItsPushAnimation()
    {
        bool marked = typeof(RecipeDetailPage).IsDefined(typeof(DisableAnimationAttribute), inherit: false);

        Console.WriteLine(marked);

        // Output:
        // True
    }
}
