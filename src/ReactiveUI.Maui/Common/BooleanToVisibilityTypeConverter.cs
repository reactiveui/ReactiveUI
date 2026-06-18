// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if WINUI_TARGET
// Alias rather than import the namespace: the Maui-windows TFM also imports Microsoft.Maui implicitly, so a bare
// Visibility would be ambiguous between Microsoft.UI.Xaml.Visibility and Microsoft.Maui.Visibility.
using Visibility = Microsoft.UI.Xaml.Visibility;
#else
using Visibility = Microsoft.Maui.Visibility;
#endif

#if IS_MAUI
#if REACTIVE_SHIM
namespace ReactiveUI.Reactive.Maui;
#else
namespace ReactiveUI.Maui;
#endif
#else
#if REACTIVE_SHIM
namespace ReactiveUI.Reactive;
#else
namespace ReactiveUI;
#endif
#endif

/// <summary>Converts <see cref="bool"/> to Visibility.</summary>
/// <remarks>
/// <para>
/// The conversion supports a <see cref="BooleanToVisibilityHint"/> as the conversion hint parameter:
/// </para>
/// <list type="bullet">
/// <item><description><see cref="BooleanToVisibilityHint.None"/> - True maps to Visible, False maps to Collapsed.</description></item>
/// <item><description><see cref="BooleanToVisibilityHint.Inverse"/> - Inverts the boolean before conversion (True → Collapsed, False → Visible).</description></item>
/// <item><description><see cref="BooleanToVisibilityHint.UseHidden"/> - Use Hidden instead of Collapsed for false values (MAUI only, ignored on WinUI).</description></item>
/// </list>
/// <para>
/// Hints can be combined using bitwise OR (e.g., <c>BooleanToVisibilityHint.Inverse | BooleanToVisibilityHint.UseHidden</c>).
/// </para>
/// </remarks>
public sealed class BooleanToVisibilityTypeConverter : BindingTypeConverter<bool, Visibility>
{
    /// <inheritdoc/>
    public override int GetAffinityForObjects() => BindingAffinity.DefaultInternalTypeConverter;

    /// <inheritdoc/>
    public override bool TryConvert(bool from, object? conversionHint, out Visibility result)
    {
        var hint = conversionHint is BooleanToVisibilityHint visibilityHint
            ? visibilityHint
            : BooleanToVisibilityHint.None;

        var value = (hint & BooleanToVisibilityHint.Inverse) != 0 ? !from : from;

#if !WINUI_TARGET
        var notVisible = (hint & BooleanToVisibilityHint.UseHidden) != 0
            ? Visibility.Hidden
            : Visibility.Collapsed;
#else
        const Visibility notVisible = Visibility.Collapsed;
#endif

        result = value ? Visibility.Visible : notVisible;
        return true;
    }
}
