// Copyright (c) 2023 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reflection;
using System.Windows;

namespace ReactiveUI;

/// <summary>
/// Creates a observable for a property if available that is based on a DependencyProperty.
/// </summary>
public class DependencyObjectObservableForProperty : ICreatesObservableForProperty
{
    /// <inheritdoc/>
    public int GetAffinityForObject(Type type, string propertyName, bool beforeChanged = false)
    {
        if (!typeof(DependencyObject).GetTypeInfo().IsAssignableFrom(type.GetTypeInfo()))
        {
            return 0;
        }

        return GetDependencyProperty(type, propertyName) is not null ? 4 : 0;
    }

    /// <inheritdoc/>
    public IObservable<IObservedChange<object, object?>> GetNotificationForProperty(object sender, System.Linq.Expressions.Expression expression, string propertyName, bool beforeChanged = false, bool suppressWarnings = false)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(sender);
#else
        if (sender is null)
        {
            throw new ArgumentNullException(nameof(sender));
        }
#endif

        var type = sender.GetType();

        var dependencyProperty = GetDependencyProperty(type, propertyName) ?? throw new ArgumentException(
                                        $"The property {propertyName} does not have a dependency property.",
                                        nameof(propertyName));
        var dependencyPropertyDescriptor = DependencyPropertyDescriptor.FromProperty(dependencyProperty, type);

        if (dependencyPropertyDescriptor is null)
        {
            if (!suppressWarnings)
            {
                this.Log().Error("Couldn't find dependency property " + propertyName + " on " + type.Name);
            }

            throw new NullReferenceException("Couldn't find dependency property " + propertyName + " on " + type.Name);
        }

        return Observable.Create<IObservedChange<object, object?>>(subj =>
        {
            var handler = new EventHandler((_, _) => subj.OnNext(new ObservedChange<object, object?>(sender, expression, default)));

            dependencyPropertyDescriptor.AddValueChanged(sender, handler);
            return Disposable.Create(() => dependencyPropertyDescriptor.RemoveValueChanged(sender, handler));
        });
    }

    private static DependencyProperty? GetDependencyProperty(Type type, string propertyName)
    {
        var fi = Array.Find(type.GetTypeInfo().GetFields(BindingFlags.FlattenHierarchy | BindingFlags.Static | BindingFlags.Public), x => x.Name == propertyName + "Property" && x.IsStatic);

        return (DependencyProperty?)fi?.GetValue(null);
    }
}
