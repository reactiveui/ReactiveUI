using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactiveUI.Xaml
{
    public class ComponentModelTypeConverter : IBindingTypeConverter
    {
        readonly MemoizingMRUCache<Tuple<Type, Type>, TypeConverter> typeConverterCache = new MemoizingMRUCache<Tuple<Type, Type>, TypeConverter>((types, _) =>
        {
            // NB: String is a Magical Type(tm) to TypeConverters. If we are
            // converting from string => int, we need the Int converter, not
            // the string converter :-/
            if (types.Item1 == typeof(string))
            {
                types = Tuple.Create(types.Item2, types.Item1);
            }

            var converter = TypeDescriptor.GetConverter(types.Item1);
            return converter.CanConvertTo(types.Item2) ? converter : null;
        }, 25);

        public int GetAffinityForObjects(Type lhs, Type rhs)
        {
            var converter = typeConverterCache.Get(Tuple.Create(lhs, rhs));
            return converter != null ? 10 : 0;
        }

        public bool TryConvert(object from, Type toType, object conversionHint, out object result)
        {
            if (from == null)
            {
                result = null;
                return true;
            }

            var fromType = from.GetType();
            var converter = typeConverterCache.Get(Tuple.Create(fromType, toType));

            if (converter == null)
            {
                throw new ArgumentException(String.Format("Can't convert {0} to {1}. To fix this, register a IBindingTypeConverter", fromType, toType));
            }

            try
            {
                // TODO: This should use conversionHint to determine whether this is locale-aware or not
                result = (fromType == typeof(string)) ? converter.ConvertFrom(from) : converter.ConvertTo(from, toType);
                return true;
            }
            catch (FormatException)
            {
                result = null;
                return false;
            }
            catch (Exception e)
            {
                // Errors from ConvertFrom end up here but wrapped in 
                // outer exception. Add more types here as required.
                // IndexOutOfRangeException is given when trying to
                // convert empty strings with some/all? converters
                if (e.InnerException is IndexOutOfRangeException ||
                    e.InnerException is FormatException)
                {
                    result = null;
                    return false;
                }
                else
                {
                    throw;
                }
            }
        }
    }
}
