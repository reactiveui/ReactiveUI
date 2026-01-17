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
/// Converts <see cref="Visibility"/> to <see cref="bool"/>.
/// </summary>
/// <remarks>
/// <para>
/// The conversion supports a <see cref="BooleanToVisibilityHint"/> as the conversion hint parameter:
/// </para>
/// <list type="bullet">
/// <item><description><see cref="BooleanToVisibilityHint.None"/> - Visible maps to True, other values map to False.</description></item>
/// <item><description><see cref="BooleanToVisibilityHint.Inverse"/> - Inverts the result (Visible → False, other → True).</description></item>
/// </list>
/// <para>
/// This converter enables two-way binding between boolean properties and visibility.
/// </para>
/// </remarks>
public sealed class VisibilityToBooleanTypeConverter : BindingTypeConverter<Visibility, bool>
{
    /// <inheritdoc/>
    public override int GetAffinityForObjects() => 2;

    /// <inheritdoc/>
    public override bool TryConvert(Visibility from, object? conversionHint, [NotNullWhen(true)] out bool result)
    {
        var hint = conversionHint is BooleanToVisibilityHint visibilityHint
            ? visibilityHint
            : BooleanToVisibilityHint.None;

        var isVisible = from == Visibility.Visible;
        result = (hint & BooleanToVisibilityHint.Inverse) != 0 ? !isVisible : isVisible;
        return true;
    }
}
