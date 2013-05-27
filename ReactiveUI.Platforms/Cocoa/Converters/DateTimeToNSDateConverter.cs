using System;
using System.Globalization;
using System.Linq;
using MonoTouch.Foundation;
using ReactiveUI;

namespace ReactiveUI.Cocoa
{
	public class DateTimeNSDateConverter :
		IImplicitBindingTypeConverter
    {
		public static Lazy<DateTimeNSDateConverter> Instance = new Lazy<DateTimeNSDateConverter>();

		#region IBindingTypeConverter implementation

		public int GetAffinityForObjects(Type fromType, Type toType)
		{
			return (fromType == typeof(DateTime) && toType == typeof(NSDate)) ||
				(toType == typeof(DateTime) && fromType == typeof(NSDate)) ? 4 : -1;
		}

		public bool TryConvert(object value, Type toType, object conversionHint, out object result)
		{
			result = null;

			if(value.GetType() == typeof(DateTime) && toType == typeof(NSDate))
			{
				var dt = (DateTime) value;
				result = (NSDate) dt;
				return true;
			}

			else if(value.GetType() == typeof(NSDate) && toType == typeof(DateTime))
			{
				var dt = (NSDate) value;
				result = (DateTime) dt;
				return true;
			}

			return false;
		}

		#endregion
    }
}
