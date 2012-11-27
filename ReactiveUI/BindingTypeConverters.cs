using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
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

        static MethodInfo genericMi = null;
        static MemoizingMRUCache<Type, MethodInfo> referenceCastCache = new MemoizingMRUCache<Type, MethodInfo>((t, _) => {
            genericMi = genericMi ?? 
                typeof (EqualityTypeConverter).GetMethod("DoReferenceCast", BindingFlags.Public | BindingFlags.Static);
            return genericMi.MakeGenericMethod(new[] {t});
        }, 25);

        public bool TryConvert(object from, Type toType, object conversionHint, out object result)
        {
            Contract.Requires(toType != null);

            var mi = referenceCastCache.Get(toType);
            result = mi.Invoke(null, new[] {from});
            return true;
        }

        public static object DoReferenceCast<T>(object from)
        {
#if WINRT
            bool isValueType = typeof (T).GetTypeInfo().IsValueType;
#else
            bool isValueType = typeof (T).IsValueType;
#endif
            if (isValueType) {
                return System.Convert.ChangeType(from, typeof (T), null);
            } else {
                return (T) from;
            }
        }
    }

    public class StringConverter : IBindingTypeConverter
    {
        public int GetAffinityForObjects(Type lhs, Type rhs)
        {
            return (rhs == typeof (string) ? 2 : 0);
        }

        public bool TryConvert(object from, Type toType, object conversionHint, out object result)
        {
            // XXX: All Of The Localization
            result = from.ToString();
            return true;
        }
    }

#if !SILVERLIGHT && !WINRT
    public class ComponentModelTypeConverter : IBindingTypeConverter
    {
        readonly MemoizingMRUCache<Tuple<Type, Type>, TypeConverter> typeConverterCache = new MemoizingMRUCache<Tuple<Type, Type>, TypeConverter>((types, _) => {
            // NB: String is a Magical Type(tm) to TypeConverters. If we are
            // converting from string => int, we need the Int converter, not
            // the string converter :-/
            if (types.Item1 == typeof (string)) {
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
            var fromType = from.GetType();
            var converter = typeConverterCache.Get(Tuple.Create(fromType, toType));

            if (converter == null) {
                throw new ArgumentException(String.Format("Can't convert {0} to {1}. To fix this, register a IBindingTypeConverter", fromType, toType));
            }

            try {
                // TODO: This should use conversionHint to determine whether this is locale-aware or not
                result = converter.ConvertTo(from, toType);
                return true;
            } catch (FormatException) {
                result = null;
                return false;
            }
        }
    }
#endif
}