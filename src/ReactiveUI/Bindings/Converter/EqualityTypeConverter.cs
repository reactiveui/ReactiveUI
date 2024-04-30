﻿// Copyright (c) 2024 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reflection;

namespace ReactiveUI;

/// <summary>
/// The default converter, simply converts between types that are equal or
/// can be converted (i.e. Button => UIControl).
/// </summary>
public class EqualityTypeConverter : IBindingTypeConverter
{
    private static readonly MemoizingMRUCache<Type, MethodInfo> _referenceCastCache = new(
     (_, _) => _methodInfo ??= typeof(EqualityTypeConverter).GetRuntimeMethods().First(x => x.Name == nameof(DoReferenceCast)), RxApp.SmallCacheLimit);

    private static MethodInfo? _methodInfo;

    /// <summary>
    /// Handles casting for a reference. Understands about nullable types
    /// and can cast appropriately.
    /// </summary>
    /// <param name="from">The object we are casting from.</param>
    /// <param name="targetType">The target we want to cast to.</param>
    /// <returns>The new value after it has been casted.</returns>
    /// <exception cref="InvalidCastException">If we cannot cast the object.</exception>
    public static object? DoReferenceCast(object? from, Type targetType)
    {
        targetType.ArgumentNullExceptionThrowIfNull(nameof(targetType));
        var backingNullableType = Nullable.GetUnderlyingType(targetType);

        if (backingNullableType is null)
        {
            if (from is null)
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

        if (from is null)
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
        if (realType is not null)
        {
            return GetAffinityForObjects(realType, toType);
        }

        realType = Nullable.GetUnderlyingType(toType);
        if (realType is not null)
        {
            return GetAffinityForObjects(fromType, realType);
        }

        return 0;
    }

    /// <inheritdoc/>
    public bool TryConvert(object? from, Type toType, object? conversionHint, out object? result)
    {
        toType.ArgumentNullExceptionThrowIfNull(nameof(toType));

        var mi = _referenceCastCache.Get(toType);

        try
        {
            result = mi.Invoke(null, [from, toType]);
        }
        catch (Exception ex)
        {
            this.Log().Warn(ex, "Couldn't convert object to type: " + toType);
            result = null;
            return false;
        }

        return true;
    }

    private static bool IsInstanceOfType(object from, Type targetType)
    {
        from.ArgumentNullExceptionThrowIfNull(nameof(from));
        targetType.ArgumentNullExceptionThrowIfNull(nameof(targetType));
        return targetType.IsInstanceOfType(from);
    }
}
