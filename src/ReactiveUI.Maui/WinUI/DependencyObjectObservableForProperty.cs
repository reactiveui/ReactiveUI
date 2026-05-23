// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

#if WINUI_TARGET
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

#if IS_MAUI
#endif
using System.Linq.Expressions;
using System.Reflection;

using Microsoft.UI.Xaml;
using ReactiveUI.Internal;
using Splat;

namespace ReactiveUI;

/// <summary>
/// Creates a observable for a property if available that is based on a DependencyProperty.
/// </summary>
public class DependencyObjectObservableForProperty : ICreatesObservableForProperty
{
    /// <summary>
    /// The binding affinity returned for objects that expose a matching DependencyProperty.
    /// </summary>
    private const int DependencyPropertyAffinity = 6;

    /// <inheritdoc/>
    [RequiresUnreferencedCode("GetAffinityForObject uses methods that may require unreferenced code")]
    public int GetAffinityForObject(Type type, string propertyName) =>
        GetAffinityForObject(type, propertyName, false);

    /// <inheritdoc/>
    [RequiresUnreferencedCode("GetAffinityForObject uses methods that may require unreferenced code")]
    public int GetAffinityForObject(Type? type, string propertyName, bool beforeChanged)
    {
        if (type is null || !typeof(DependencyObject).GetTypeInfo().IsAssignableFrom(type.GetTypeInfo()))
        {
            return 0;
        }

        if (GetDependencyPropertyFetcher(type, propertyName) is null)
        {
            return 0;
        }

        return DependencyPropertyAffinity;
    }

    /// <inheritdoc/>
    [RequiresUnreferencedCode("GetNotificationForProperty uses methods that may require unreferenced code")]
    public IObservable<IObservedChange<object, object?>> GetNotificationForProperty(object sender, Expression expression, string propertyName) =>
        GetNotificationForProperty(sender, expression, propertyName, false, false);

    /// <inheritdoc/>
    [RequiresUnreferencedCode("GetNotificationForProperty uses methods that may require unreferenced code")]
    public IObservable<IObservedChange<object, object?>> GetNotificationForProperty(object sender, Expression expression, string propertyName, bool beforeChanged) =>
        GetNotificationForProperty(sender, expression, propertyName, beforeChanged, false);

    /// <inheritdoc/>
    [RequiresUnreferencedCode("GetNotificationForProperty uses methods that may require unreferenced code")]
    public IObservable<IObservedChange<object, object?>> GetNotificationForProperty(object sender, Expression expression, string propertyName, bool beforeChanged, bool suppressWarnings)
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

        return new FromEventObservable<IObservedChange<object, object?>>(onNext =>
        {
            var handler = new DependencyPropertyChangedCallback((_, _) =>
                onNext(new ObservedChange<object, object?>(sender, expression, default)));

            var dependencyProperty = dpFetcher();
            var token = depSender.RegisterPropertyChangedCallback(dependencyProperty, handler);
            return new ActionDisposable(() => depSender.UnregisterPropertyChangedCallback(dependencyProperty, token));
        });
    }

    /// <summary>
    /// Walks the type hierarchy to find a static property with the specified name.
    /// </summary>
    /// <param name="typeInfo">The type to start searching from.</param>
    /// <param name="propertyName">The name of the property to locate.</param>
    /// <returns>The matching static <see cref="PropertyInfo"/>, or <see langword="null"/> if none is found.</returns>
    [RequiresUnreferencedCode("ActuallyGetProperty uses methods that may require unreferenced code")]
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

    /// <summary>
    /// Walks the type hierarchy to find a static field with the specified name.
    /// </summary>
    /// <param name="typeInfo">The type to start searching from.</param>
    /// <param name="propertyName">The name of the field to locate.</param>
    /// <returns>The matching static <see cref="FieldInfo"/>, or <see langword="null"/> if none is found.</returns>
    [RequiresUnreferencedCode("ActuallyGetField uses methods that may require unreferenced code")]
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

    /// <summary>
    /// Builds a fetcher that resolves the <see cref="DependencyProperty"/> backing the named property.
    /// </summary>
    /// <param name="type">The type that declares the property.</param>
    /// <param name="propertyName">The name of the property whose backing DependencyProperty is required.</param>
    /// <returns>A function that returns the <see cref="DependencyProperty"/>, or <see langword="null"/> if it cannot be resolved.</returns>
    [RequiresUnreferencedCode("GetDependencyPropertyFetcher uses methods that may require unreferenced code")]
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
