using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace ReactiveUI
{
    public class EqualityTypeConverter : IBindingTypeConverter
    {
        public int GetAffinityForObjects(Type lhs, Type rhs)
        {
            if (rhs.IsAssignableFrom(lhs)) {
                return 100;
            }

            // NB: WPF is terrible.
            if (lhs == typeof (object)) {
                return 100;
            }

            return 0;
        }

        public object ConvertObjectToType(object from, Type toType, object conversionHint)
        {
            return Convert.ChangeType(from, toType);
        }
    }

    public class ComponentModelTypeConverter : IBindingTypeConverter
    {
        readonly MemoizingMRUCache<Tuple<Type, Type>, TypeConverter> typeConverterCache = new MemoizingMRUCache<Tuple<Type, Type>, TypeConverter>((types, _) => {
            var converter = TypeDescriptor.GetConverter(types.Item1);
            return converter.CanConvertTo(types.Item2) ? converter : null;
        }, 25);

        public int GetAffinityForObjects(Type lhs, Type rhs)
        {
            return typeConverterCache.Get(Tuple.Create(lhs, rhs)) != null ? 10 : 0;
        }

        public object ConvertObjectToType(object from, Type toType, object conversionHint)
        {
            var fromType = from.GetType();
            var converter = typeConverterCache.Get(Tuple.Create(fromType, toType));

            if (converter == null) {
                throw new ArgumentException(String.Format("Can't convert {0} to {1}. To fix this, register a IBindingTypeConverter", fromType, toType));
            }

            // TODO: This should use conversionHint to determine whether this is locale-aware or not
            return converter.ConvertTo(from, toType);
        }
    }
}