// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using Splat;

namespace ReactiveUI
{
    /// <summary>
    /// The default converter, simply converts between types that are equal or
    /// can be converted (i.e. Button => UIControl).
    /// </summary>
    public class EqualityTypeConverter : IBindingTypeConverter
    {
        /// <inheritdoc/>
        public int GetAffinityForObjects(Type fromType, Type toType)
        {
            if (toType.GetTypeInfo().IsAssignableFrom(fromType.GetTypeInfo()))
            {
                return 100;
            }

            // NB: WPF is terrible.
            if (fromType == typeof(object))
            {
                return 100;
            }

            var realType = Nullable.GetUnderlyingType(fromType);
            if (realType != null)
            {
                return GetAffinityForObjects(realType, toType);
            }

            realType = Nullable.GetUnderlyingType(toType);
            if (realType != null)
            {
                return GetAffinityForObjects(fromType, realType);
            }

            return 0;
        }

        private static MethodInfo mi;

        private static readonly MemoizingMRUCache<Type, MethodInfo> referenceCastCache = new MemoizingMRUCache<Type, MethodInfo>(
            (t, _) =>
        {
            return mi = mi ?? typeof(EqualityTypeConverter).GetRuntimeMethods().First(x => x.Name == nameof(DoReferenceCast));
        }, RxApp.SmallCacheLimit);

        /// <inheritdoc/>
        public bool TryConvert(object from, Type toType, object conversionHint, out object result)
        {
            Contract.Requires(toType != null);

            var mi = default(MethodInfo);
            lock (referenceCastCache)
            {
                mi = referenceCastCache.Get(toType);
            }

            try
            {
                result = mi.Invoke(null, new[] { from, toType });
            }
            catch (Exception ex)
            {
                this.Log().WarnException("Couldn't convert object to type: " + toType, ex);
                result = null;
                return false;
            }

            return true;
        }

        public static object DoReferenceCast(object from, Type targetType)
        {
            var backingNullableType = Nullable.GetUnderlyingType(targetType);

            if (backingNullableType == null)
            {
                if (from == null)
                {
                    if (targetType.GetTypeInfo().IsValueType)
                    {
                        throw new InvalidCastException("Can't convert from nullable-type which is null to non-nullable type");
                    }

                    return null;
                }

                if (IsInstanceOfType(from, targetType))
                {
                    return from;
                }

                throw new InvalidCastException();
            }

            if (from == null)
            {
                return null;
            }

            var converted = Convert.ChangeType(from, backingNullableType, null);
            if (!IsInstanceOfType(converted, targetType))
            {
                throw new InvalidCastException();
            }

            return converted;
        }

        private static bool IsInstanceOfType(object from, Type targetType)
        {
#if NETFX_CORE || PORTABLE
            return targetType.GetTypeInfo().IsAssignableFrom(from.GetType().GetTypeInfo());
#else
            return targetType.IsInstanceOfType(from);
#endif
        }
    }
}
