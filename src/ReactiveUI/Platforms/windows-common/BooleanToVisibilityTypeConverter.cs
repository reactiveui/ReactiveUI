// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
#if NETFX_CORE
using Windows.UI.Xaml;
#else
using System.Windows;
#endif

namespace ReactiveUI
{
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
        public bool TryConvert(object from, Type toType, object conversionHint, out object result)
        {
            var hint = conversionHint is BooleanToVisibilityHint ?
                (BooleanToVisibilityHint)conversionHint :
                BooleanToVisibilityHint.None;

            if (toType == typeof(Visibility))
            {
                var fromAsBool = hint.HasFlag(BooleanToVisibilityHint.Inverse) ? !(bool)@from : (bool)from;
#if !NETFX_CORE
                var notVisible = hint.HasFlag(BooleanToVisibilityHint.UseHidden) ? Visibility.Hidden : Visibility.Collapsed;
#else
                var notVisible = Visibility.Collapsed;
#endif
                result = fromAsBool ? Visibility.Visible : notVisible;
                return true;
            }

            var fromAsVis = (Visibility)from;
            result = fromAsVis == Visibility.Visible ^ !hint.HasFlag(BooleanToVisibilityHint.Inverse);

            return true;
        }
    }
}
