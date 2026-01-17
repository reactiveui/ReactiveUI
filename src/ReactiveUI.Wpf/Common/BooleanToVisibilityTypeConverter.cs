// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;

#if HAS_MAUI
using Microsoft.Maui;

#endif
#if HAS_WINUI
using Microsoft.UI.Xaml;
#elif HAS_UNO
using Windows.UI.Xaml;
#else
using System.Windows;
#endif

#if HAS_UNO
namespace ReactiveUI.Uno
#else
namespace ReactiveUI;
#endif

/// <summary>
/// Converts <see cref="bool"/> to <see cref="Visibility"/>.
/// </summary>
/// <remarks>
/// <para>
/// The conversion supports a <see cref="BooleanToVisibilityHint"/> as the conversion hint parameter:
/// </para>
/// <list type="bullet">
/// <item><description><see cref="BooleanToVisibilityHint.None"/> - True maps to Visible, False maps to Collapsed.</description></item>
/// <item><description><see cref="BooleanToVisibilityHint.Inverse"/> - Inverts the boolean before conversion (True → Collapsed, False → Visible).</description></item>
/// <item><description><see cref="BooleanToVisibilityHint.UseHidden"/> - Use Hidden instead of Collapsed for false values (WPF only, ignored on UNO/WinUI).</description></item>
/// </list>
/// <para>
/// Hints can be combined using bitwise OR (e.g., <c>BooleanToVisibilityHint.Inverse | BooleanToVisibilityHint.UseHidden</c>).
/// </para>
/// </remarks>
public sealed class BooleanToVisibilityTypeConverter : BindingTypeConverter<bool, Visibility>
{
    /// <inheritdoc/>
    public override int GetAffinityForObjects() => 2;

    /// <inheritdoc/>
    public override bool TryConvert(bool from, object? conversionHint, [NotNullWhen(true)] out Visibility result)
    {
        var hint = conversionHint is BooleanToVisibilityHint visibilityHint
            ? visibilityHint
            : BooleanToVisibilityHint.None;

        var value = (hint & BooleanToVisibilityHint.Inverse) != 0 ? !from : from;

#if !HAS_UNO && !HAS_WINUI
        var notVisible = (hint & BooleanToVisibilityHint.UseHidden) != 0
            ? Visibility.Hidden
            : Visibility.Collapsed;
#else
        var notVisible = Visibility.Collapsed;
#endif

        result = value ? Visibility.Visible : notVisible;
        return true;
    }
}
