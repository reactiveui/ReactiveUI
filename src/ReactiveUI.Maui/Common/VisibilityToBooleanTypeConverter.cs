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

/// <summary>Converts <see cref="Visibility"/> to <see cref="bool"/>.</summary>
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
    public override int GetAffinityForObjects() => BindingAffinity.DefaultInternalTypeConverter;

    /// <inheritdoc/>
    public override bool TryConvert(Visibility from, object? conversionHint, out bool result)
    {
        var hint = conversionHint is BooleanToVisibilityHint visibilityHint
            ? visibilityHint
            : BooleanToVisibilityHint.None;

        var isVisible = from == Visibility.Visible;
        result = (hint & BooleanToVisibilityHint.Inverse) != 0 ? !isVisible : isVisible;
        return true;
    }
}
