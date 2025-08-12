// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reflection;

namespace ReactiveUI;

/// <summary>
/// Generates Observables based on observing Reactive objects.
/// </summary>
public class IROObservableForProperty : ICreatesObservableForProperty
{
    /// <inheritdoc/>
#if NET6_0_OR_GREATER
    [RequiresDynamicCode("GetAffinityForObject uses methods that require dynamic code generation")]
    [RequiresUnreferencedCode("GetAffinityForObject uses methods that may require unreferenced code")]
#endif
    public int GetAffinityForObject(Type type, string propertyName, bool beforeChanged = false)
    {
        // NB: Since every IReactiveObject is also an INPC, we need to bind more
        // tightly than INPCObservableForProperty, so we return 10 here
        // instead of one
#pragma warning disable IDE0022 // Use expression body for methods
        return typeof(IReactiveObject).GetTypeInfo().IsAssignableFrom(type.GetTypeInfo()) ? 10 : 0;
#pragma warning restore IDE0022 // Use expression body for methods
    }

    /// <inheritdoc/>
#if NET6_0_OR_GREATER
    [RequiresDynamicCode("GetNotificationForProperty uses methods that require dynamic code generation")]
    [RequiresUnreferencedCode("GetNotificationForProperty uses methods that may require unreferenced code")]
#endif
    public IObservable<IObservedChange<object, object?>> GetNotificationForProperty(object sender, Expression expression, string propertyName, bool beforeChanged = false, bool suppressWarnings = false)
    {
        expression.ArgumentNullExceptionThrowIfNull(nameof(expression));

        if (sender is not IReactiveObject iro)
        {
            throw new ArgumentException("Sender doesn't implement IReactiveObject");
        }

        var obs = beforeChanged ? iro.GetChangingObservable() : iro.GetChangedObservable();

        if (beforeChanged)
        {
            if (expression.NodeType == ExpressionType.Index)
            {
                return obs.Where(x => x.PropertyName?.Equals(propertyName + "[]", StringComparison.InvariantCulture) == true)
                          .Select(_ => new ObservedChange<object, object?>(sender, expression, default));
            }

            return obs.Where(x => x.PropertyName?.Equals(propertyName, StringComparison.InvariantCulture) == true)
                      .Select(_ => new ObservedChange<object, object?>(sender, expression, default));
        }

        if (expression.NodeType == ExpressionType.Index)
        {
            return obs.Where(x => x.PropertyName?.Equals(propertyName + "[]", StringComparison.InvariantCulture) == true)
                      .Select(_ => new ObservedChange<object, object?>(sender, expression, default));
        }

        return obs.Where(x => x.PropertyName?.Equals(propertyName, StringComparison.InvariantCulture) == true)
                  .Select(_ => new ObservedChange<object, object?>(sender, expression, default));
    }
}
