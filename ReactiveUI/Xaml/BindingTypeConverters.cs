using System;
#if NETFX_CORE
using Windows.UI.Xaml;
#else
using System.Windows;
#endif

namespace ReactiveUI
{
    [Flags]
    public enum BooleanToVisibilityHint
    {
        None = 0,
        Inverse = 1 << 1,
#if !SILVERLIGHT && !NETFX_CORE
        UseHidden = 1 << 2,
#endif
    }

    /// <summary>
    /// This type convert converts between Boolean and XAML Visibility - the 
    /// conversionHint is a BooleanToVisibilityHint
    /// </summary>
    public class BooleanToVisibilityTypeConverter : IBindingTypeConverter
    {
        public int GetAffinityForObjects(Type fromType, Type toType)
        {
            if (fromType == typeof (bool) && toType == typeof (Visibility)) return 10;
            if (fromType == typeof (Visibility) && toType == typeof (bool)) return 10;
            return 0;
        }

        public bool TryConvert(object from, Type toType, object conversionHint, out object result)
        {
            var hint = conversionHint is BooleanToVisibilityHint ? 
                (BooleanToVisibilityHint) conversionHint : 
                BooleanToVisibilityHint.None;

            if (toType == typeof (Visibility)) {
                var fromAsBool = hint.HasFlag(BooleanToVisibilityHint.Inverse) ? !((bool) from) : (bool) from;
#if !SILVERLIGHT && !NETFX_CORE
                var notVisible = hint.HasFlag(BooleanToVisibilityHint.UseHidden) ? Visibility.Hidden : Visibility.Collapsed;
#else
                var notVisible = Visibility.Collapsed;
#endif
                result = fromAsBool ? Visibility.Visible : notVisible;
                return true;
            } else {
                var fromAsVis = (Visibility) from;
                result = (fromAsVis == Visibility.Visible) ^ (!hint.HasFlag(BooleanToVisibilityHint.Inverse));
                return true;
            }
        }
    }
}