using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using Splat;

namespace ReactiveUI
{
    /// <summary>
    /// The default converter, simply converts between types that are equal or can be converted (i.e.
    /// Button =&gt; UIControl)
    /// </summary>
    public class EqualityTypeConverter : IBindingTypeConverter
    {
        /// <summary>
        /// Returns a positive integer when this class supports TryConvert for this particular Type.
        /// If the method isn't supported at all, return a non-positive integer. When multiple
        /// implementations return a positive value, the host will use the one which returns the
        /// highest value. When in doubt, return '2' or '0'.
        /// </summary>
        /// <param name="fromType">The source type to convert from</param>
        /// <param name="toType">The target type to convert to</param>
        /// <returns>
        /// A positive integer if TryConvert is supported, zero or a negative value otherwise
        /// </returns>
        public int GetAffinityForObjects(Type fromType, Type toType)
        {
            if (toType.GetTypeInfo().IsAssignableFrom(fromType.GetTypeInfo())) {
                return 100;
            }

            // NB: WPF is terrible.
            if (fromType == typeof(object)) {
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

        private static MethodInfo mi = null;

        private static readonly MemoizingMRUCache<Type, MethodInfo> referenceCastCache = new MemoizingMRUCache<Type, MethodInfo>((t, _) => {
            return mi = mi ?? typeof(EqualityTypeConverter).GetRuntimeMethods().First(x => x.Name == nameof(DoReferenceCast));
        }, RxApp.SmallCacheLimit);

        /// <summary>
        /// Convert a given object to the specified type.
        /// </summary>
        /// <param name="from">The object to convert.</param>
        /// <param name="toType">The type to coerce the object to.</param>
        /// <param name="conversionHint">
        /// An implementation-defined value, usually to specify things like locale awareness.
        /// </param>
        /// <param name="result"></param>
        /// <returns>An object that is of the type 'to'</returns>
        public bool TryConvert(object from, Type toType, object conversionHint, out object result)
        {
            Contract.Requires(toType != null);

            var mi = default(MethodInfo);
            lock (referenceCastCache) {
                mi = referenceCastCache.Get(toType);
            }

            try {
                result = mi.Invoke(null, new[] { from, toType });
            } catch (Exception ex) {
                this.Log().WarnException("Couldn't convert object to type: " + toType, ex);
                result = null;
                return false;
            }
            return true;
        }

        /// <summary>
        /// Does the reference cast.
        /// </summary>
        /// <param name="from">From.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <returns></returns>
        /// <exception cref="System.InvalidCastException">
        /// Can't convert from nullable-type which is null to non-nullable type or or
        /// </exception>
        public static object DoReferenceCast(object from, Type targetType)
        {
            var backingNullableType = Nullable.GetUnderlyingType(targetType);

            if (backingNullableType == null) {
                if (from == null) {
                    if (targetType.GetTypeInfo().IsValueType) {
                        throw new InvalidCastException("Can't convert from nullable-type which is null to non-nullable type");
                    }

                    return null;
                }

                if (isInstanceOfType(from, targetType)) {
                    return from;
                }

                throw new InvalidCastException();
            }

            if (from == null) {
                return null;
            }

            var converted = Convert.ChangeType(from, backingNullableType, null);
            if (!isInstanceOfType(converted, targetType)) {
                throw new InvalidCastException();
            }

            return converted;
        }

        private static bool isInstanceOfType(object from, Type targetType)
        {
#if NETFX_CORE || PORTABLE
            return targetType.GetTypeInfo().IsAssignableFrom(from.GetType().GetTypeInfo());
#else
            return targetType.IsInstanceOfType(from);
#endif
        }
    }

    /// <summary>
    /// Calls ToString on types. In WPF, ComponentTypeConverter should win instead of this, since
    /// It's Better™.
    /// </summary>
    public class StringConverter : IBindingTypeConverter
    {
        /// <summary>
        /// Returns a positive integer when this class supports TryConvert for this particular Type.
        /// If the method isn't supported at all, return a non-positive integer. When multiple
        /// implementations return a positive value, the host will use the one which returns the
        /// highest value. When in doubt, return '2' or '0'.
        /// </summary>
        /// <param name="fromType">The source type to convert from</param>
        /// <param name="toType">The target type to convert to</param>
        /// <returns>
        /// A positive integer if TryConvert is supported, zero or a negative value otherwise
        /// </returns>
        public int GetAffinityForObjects(Type fromType, Type toType)
        {
            return (toType == typeof(string) ? 2 : 0);
        }

        /// <summary>
        /// Convert a given object to the specified type.
        /// </summary>
        /// <param name="from">The object to convert.</param>
        /// <param name="toType">The type to coerce the object to.</param>
        /// <param name="conversionHint">
        /// An implementation-defined value, usually to specify things like locale awareness.
        /// </param>
        /// <param name="result"></param>
        /// <returns>An object that is of the type 'to'</returns>
        public bool TryConvert(object from, Type toType, object conversionHint, out object result)
        {
            // XXX: All Of The Localization
            result = from.ToString();
            return true;
        }
    }
}