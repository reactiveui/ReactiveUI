﻿// Copyright (c) 2024 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if WINUI_TARGET
using Microsoft.UI.Xaml;
#else
using Microsoft.Maui;
#endif

#if IS_MAUI
namespace ReactiveUI.Maui;
#else
namespace ReactiveUI;
#endif

/// <summary>
/// This type convert converts between Boolean and XAML Visibility - the
/// conversionHint is a BooleanToVisibilityHint.
/// </summary>
public class BooleanToVisibilityTypeConverter : IBindingTypeConverter
{
    /// <inheritdoc/>
    public int GetAffinityForObjects(Type fromType, Type toType)
    {
        if (fromType == typeof(bool) && toType == typeof(Visibility))
        {
            return 10;
        }

        if (fromType == typeof(Visibility) && toType == typeof(bool))
        {
            return 10;
        }

        return 0;
    }

    /// <inheritdoc/>
    public bool TryConvert(object? from, Type toType, object? conversionHint, out object result)
    {
        var hint = conversionHint is BooleanToVisibilityHint visibilityHint ?
            visibilityHint :
            BooleanToVisibilityHint.None;

        if (toType == typeof(Visibility) && from is bool fromBool)
        {
            var fromAsBool = (hint & BooleanToVisibilityHint.Inverse) != 0 ? !fromBool : fromBool;

#if !WINUI_TARGET
            var notVisible = (hint & BooleanToVisibilityHint.UseHidden) != 0 ? Visibility.Hidden : Visibility.Collapsed;
#else
            const Visibility notVisible = Visibility.Collapsed;
#endif
            result = fromAsBool ? Visibility.Visible : notVisible;
            return true;
        }

        if (from is Visibility fromAsVis)
        {
            result = fromAsVis == Visibility.Visible ^ (hint & BooleanToVisibilityHint.Inverse) == 0;
        }
        else
        {
            result = Visibility.Visible;
        }

        return true;
    }
}
