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

            var realType = Nullable.GetUnderlyingType(lhs);
            if (realType != null) {
                return GetAffinityForObjects(realType, rhs);
            }

            realType = Nullable.GetUnderlyingType(rhs);
            if (realType != null) {
                return GetAffinityForObjects(lhs, realType);
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

            var mi = default(MethodInfo);
            lock (referenceCastCache) {
                mi = referenceCastCache.Get(toType);
            }

            try {
                result = mi.Invoke(null, new[] {from});
            } catch (Exception ex) {
                this.Log().WarnException("Couldn't convert object to type: " + toType, ex);
                result = null;
                return false;
            }
            return true;
        }

        public static object DoReferenceCast<T>(object from)
        {
            var targetType = typeof (T);
            var backingNullableType = Nullable.GetUnderlyingType(targetType);

            if (backingNullableType == null) {
                return (T) from;
            }

            if (from == null) {
                var ut = Nullable.GetUnderlyingType(targetType);
                if (ut == null) {
                    throw new Exception("Can't convert from nullable-type which is null to non-nullable type");
                }
                return default(T);
            }

            return (T) Convert.ChangeType(from, backingNullableType, null);
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
            if (from == null) {
                result = null;
                return true;
            }

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