using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using Splat;

namespace ReactiveUI
{
    /// <summary>
    /// The default converter, simply converts between types that are equal or
    /// can be converted (i.e. Button => UIControl)
    /// </summary>
    public class EqualityTypeConverter : IBindingTypeConverter
    {
        public int GetAffinityForObjects(Type fromType, Type toType)
        {
            if (toType.GetTypeInfo().IsAssignableFrom(fromType.GetTypeInfo())) {
                return 100;
            }

            // NB: WPF is terrible.
            if (fromType == typeof (object)) {
                return 100;
            }

            var realType = Nullable.GetUnderlyingType(fromType);
            if (realType != null) {
                return GetAffinityForObjects(realType, toType);
            }

            realType = Nullable.GetUnderlyingType(toType);
            if (realType != null) {
                return GetAffinityForObjects(fromType, realType);
            }

            return 0;
        }

        static MethodInfo genericMi = null;
        static MemoizingMRUCache<Type, MethodInfo> referenceCastCache = new MemoizingMRUCache<Type, MethodInfo>((t, _) => {
            genericMi = genericMi ??
                typeof(EqualityTypeConverter).GetRuntimeMethods().First(x => x.Name == "DoReferenceCast");

            return genericMi.MakeGenericMethod(new[] {t});
        }, RxApp.SmallCacheLimit);

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

    /// <summary>
    /// Calls ToString on types. In WPF, ComponentTypeConverter should win
    /// instead of this, since It's Better™.
    /// </summary>
    public class StringConverter : IBindingTypeConverter
    {
        public int GetAffinityForObjects(Type fromType, Type toType)
        {
            return (toType == typeof (string) ? 2 : 0);
        }

        public bool TryConvert(object from, Type toType, object conversionHint, out object result)
        {
            // XXX: All Of The Localization
            result = from.ToString();
            return true;
        }
    }
}
