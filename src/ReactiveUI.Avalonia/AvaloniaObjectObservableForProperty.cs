// Copyright (c) 2024 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Reactive.Linq;
using System.Reflection;
using Avalonia;
using Splat;

namespace ReactiveUI.Avalonia;

/// <summary>
/// Creates an observable for a property if available that is based on a DependencyProperty.
/// </summary>
internal class AvaloniaObjectObservableForProperty : ICreatesObservableForProperty
{
    /// <inheritdoc/>
    public int GetAffinityForObject(Type type, string propertyName, bool beforeChanged = false)
    {
        if (!typeof(AvaloniaObject).GetTypeInfo().IsAssignableFrom(type.GetTypeInfo()))
        {
            return 0;
        }

        return GetAvaloniaProperty(type, propertyName) is not null ? 4 : 0;
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

        if (sender is not AvaloniaObject avaloniaObject)
        {
            throw new InvalidOperationException("The sender is not a AvaloniaObject");
        }

        var type = sender.GetType();

        var avaloniaProperty = GetAvaloniaProperty(type, propertyName);

        if (avaloniaProperty is null)
        {
            if (!suppressWarnings)
            {
                this.Log().Error("Couldn't find avalonia property " + propertyName + " on " + type.Name);
            }

            throw new NullReferenceException("Couldn't find avalonia property " + propertyName + " on " + type.Name);
        }

        return avaloniaObject
            .GetPropertyChangedObservable(avaloniaProperty)
            .Select(args => new ObservedChange<object, object?>(args.Sender, expression, args.NewValue));
    }

    private static AvaloniaProperty? GetAvaloniaProperty(Type type, string propertyName)
    {
        foreach (var property in AvaloniaPropertyRegistry.Instance.GetRegistered(type))
        {
            if (string.Equals(property.Name, propertyName, StringComparison.Ordinal))
            {
                return property;
            }
        }

        return null;
    }
}
