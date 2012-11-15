using System;
#if WINRT
using Windows.UI.Xaml;
#else
using System.Windows;
#endif

namespace ReactiveUI.Xaml
{
    [Flags]
    public enum BooleanToVisibilityHint
    {
        None = 0,
        Inverse = 1 << 1,
#if !SILVERLIGHT && !WINRT
        UseHidden = 1 << 2,
#endif
    }

    public class BooleanToVisibilityTypeConverter : IBindingTypeConverter
    {
        public int GetAffinityForObjects(Type lhs, Type rhs)
        {
            if (lhs == typeof (bool) && rhs == typeof (Visibility)) return 10;
            if (lhs == typeof (Visibility) && rhs == typeof (bool)) return 10;
            return 0;
        }

        public object Convert(object from, Type toType, object conversionHint)
        {
            var hint = conversionHint is BooleanToVisibilityHint ? 
                (BooleanToVisibilityHint) conversionHint : 
                BooleanToVisibilityHint.None;

            if (toType == typeof (Visibility)) {
                var fromAsBool = hint.HasFlag(BooleanToVisibilityHint.Inverse) ? !((bool) from) : (bool) from;
#if !SILVERLIGHT && !WINRT
                var notVisible = hint.HasFlag(BooleanToVisibilityHint.UseHidden) ? Visibility.Hidden : Visibility.Collapsed;
#else
                var notVisible = Visibility.Collapsed;
#endif
                return fromAsBool ? Visibility.Visible : notVisible;
            } else {
                var fromAsVis = (Visibility) from;
                return (fromAsVis == Visibility.Visible) ^ (!hint.HasFlag(BooleanToVisibilityHint.Inverse));
            }
        }
    }
}