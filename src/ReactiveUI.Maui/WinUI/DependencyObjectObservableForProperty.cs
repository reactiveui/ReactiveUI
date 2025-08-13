// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if WINUI_TARGET
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
#if IS_MAUI
using System.Linq.Expressions;
#endif
using System.Reflection;
using Microsoft.UI.Xaml;

namespace ReactiveUI;

/// <summary>
/// Creates a observable for a property if available that is based on a DependencyProperty.
/// </summary>
public class DependencyObjectObservableForProperty : ICreatesObservableForProperty
{
    /// <inheritdoc/>
#if NET6_0_OR_GREATER
    [RequiresDynamicCode("GetAffinityForObject uses methods that require dynamic code generation")]
    [RequiresUnreferencedCode("GetAffinityForObject uses methods that may require unreferenced code")]
#endif
    public int GetAffinityForObject(Type type, string propertyName, bool beforeChanged = false)
    {
        if (!typeof(DependencyObject).GetTypeInfo().IsAssignableFrom(type.GetTypeInfo()))
        {
            return 0;
        }

        if (GetDependencyPropertyFetcher(type, propertyName) is null)
        {
            return 0;
        }

        return 6;
    }

    /// <inheritdoc/>
#if NET6_0_OR_GREATER
    [RequiresDynamicCode("GetNotificationForProperty uses methods that require dynamic code generation")]
    [RequiresUnreferencedCode("GetNotificationForProperty uses methods that may require unreferenced code")]
#endif
    public IObservable<IObservedChange<object, object?>> GetNotificationForProperty(object sender, Expression expression, string propertyName, bool beforeChanged = false, bool suppressWarnings = false)
    {
        ArgumentNullException.ThrowIfNull(sender);

        if (sender is not DependencyObject depSender)
        {
            throw new ArgumentException("The sender must be a DependencyObject", nameof(sender));
        }

        var type = sender.GetType();

        if (beforeChanged)
        {
            this.Log().Warn(
                CultureInfo.InvariantCulture,
                "Tried to bind DO {0}.{1}, but DPs can't do beforeChanged. Binding as POCO object",
                type.FullName,
                propertyName);

            var ret = new POCOObservableForProperty();
            return ret.GetNotificationForProperty(sender, expression, propertyName, beforeChanged, suppressWarnings);
        }

        var dpFetcher = GetDependencyPropertyFetcher(type, propertyName);
        if (dpFetcher is null)
        {
            this.Log().Warn(
                CultureInfo.InvariantCulture,
                "Tried to bind DO {0}.{1}, but DP doesn't exist. Binding as POCO object",
                type.FullName,
                propertyName);

            var ret = new POCOObservableForProperty();
            return ret.GetNotificationForProperty(sender, expression, propertyName, beforeChanged, suppressWarnings);
        }

        return Observable.Create<IObservedChange<object, object?>>(subj =>
        {
            var handler = new DependencyPropertyChangedCallback((_, _) =>
                subj.OnNext(new ObservedChange<object, object?>(sender, expression, default)));

            var dependencyProperty = dpFetcher();
            var token = depSender.RegisterPropertyChangedCallback(dependencyProperty, handler);
            return Disposable.Create(() => depSender.UnregisterPropertyChangedCallback(dependencyProperty, token));
        });
    }

#if NET6_0_OR_GREATER
    [RequiresDynamicCode("ActuallyGetProperty uses methods that require dynamic code generation")]
    [RequiresUnreferencedCode("ActuallyGetProperty uses methods that may require unreferenced code")]
#endif
    private static PropertyInfo? ActuallyGetProperty(TypeInfo typeInfo, string propertyName)
    {
        var current = typeInfo;
        while (current is not null)
        {
            var ret = current.GetDeclaredProperty(propertyName);
            if (ret?.IsStatic() == true)
            {
                return ret;
            }

            current = current.BaseType?.GetTypeInfo();
        }

        return null;
    }

#if NET6_0_OR_GREATER
    [RequiresDynamicCode("ActuallyGetField uses methods that require dynamic code generation")]
    [RequiresUnreferencedCode("ActuallyGetField uses methods that may require unreferenced code")]
#endif
    private static FieldInfo? ActuallyGetField(TypeInfo typeInfo, string propertyName)
    {
        var current = typeInfo;
        while (current is not null)
        {
            var ret = current.GetDeclaredField(propertyName);
            if (ret?.IsStatic == true)
            {
                return ret;
            }

            current = current.BaseType?.GetTypeInfo();
        }

        return null;
    }

#if NET6_0_OR_GREATER
    [RequiresDynamicCode("GetDependencyPropertyFetcher uses methods that require dynamic code generation")]
    [RequiresUnreferencedCode("GetDependencyPropertyFetcher uses methods that may require unreferenced code")]
#endif
    private static Func<DependencyProperty>? GetDependencyPropertyFetcher(Type type, string propertyName)
    {
        var typeInfo = type.GetTypeInfo();

        // Look for the DependencyProperty attached to this property name
        var pi = ActuallyGetProperty(typeInfo, propertyName + "Property");
        if (pi is not null)
        {
            var value = pi.GetValue(null);

            if (value is null)
            {
                return null;
            }

            return () => (DependencyProperty)value;
        }

        var fi = ActuallyGetField(typeInfo, propertyName + "Property");
        if (fi is not null)
        {
            var value = fi.GetValue(null);

            if (value is null)
            {
                return null;
            }

            return () => (DependencyProperty)value;
        }

        return null;
    }
}
#endif
