using System;
using Foundation;

namespace ReactiveUI
{
    public class DateTimeNSDateConverter :
        IBindingTypeConverter
    {
        internal static Lazy<DateTimeNSDateConverter> Instance = new Lazy<DateTimeNSDateConverter>();

        public int GetAffinityForObjects(Type fromType, Type toType)
        {
            return (fromType == typeof(DateTime) && toType == typeof(NSDate)) ||
                (toType == typeof(DateTime) && fromType == typeof(NSDate)) ? 4 : -1;
        }

        public bool TryConvert(object val, Type toType, object conversionHint, out object result)
        {
            result = null;

            if (val.GetType() == typeof(DateTime) && toType == typeof(NSDate)) {
                var dt = (DateTime)val;
                result = (NSDate)dt;
                return true;
            } else if (val.GetType() == typeof(NSDate) && toType == typeof(DateTime)) {
                var dt = (NSDate)val;
                result = (DateTime)dt;
                return true;
            }

            return false;
        }
    }
}
