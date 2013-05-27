using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ReactiveUI
{
    public class EqualityTypeConverter : IImplicitBindingTypeConverter
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

    public class StringConverter : IImplicitBindingTypeConverter
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
}