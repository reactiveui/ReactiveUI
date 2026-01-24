// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reflection;

namespace ReactiveUI;

/// <summary>
/// Provides an implementation of property change notification observation for objects implementing either
/// INotifyPropertyChanged or INotifyPropertyChanging.
/// </summary>
/// <remarks>This class enables the creation of observables that emit notifications when a property value changes
/// or is about to change on objects that support the standard .NET property change notification interfaces. It is
/// typically used in reactive programming scenarios to monitor property changes in data-binding or MVVM patterns.
/// Reflection is used to inspect runtime types, which may have implications for trimming or ahead-of-time (AOT)
/// compilation.</remarks>
public class INPCObservableForProperty : ICreatesObservableForProperty
{
    /// <inheritdoc/>
    [RequiresUnreferencedCode("Uses reflection over runtime types which is not trim- or AOT-safe.")]
    public int GetAffinityForObject(Type type, string propertyName, bool beforeChanged)
    {
        var target = beforeChanged ? typeof(INotifyPropertyChanging) : typeof(INotifyPropertyChanged);
        return target.GetTypeInfo().IsAssignableFrom(type.GetTypeInfo()) ? 5 : 0;
    }

    /// <inheritdoc/>
    [RequiresUnreferencedCode("Uses reflection over runtime types which is not trim- or AOT-safe.")]
    public IObservable<IObservedChange<object?, object?>> GetNotificationForProperty(object sender, Expression expression, string propertyName, bool beforeChanged = false, bool suppressWarnings = false)
    {
        ArgumentExceptionHelper.ThrowIfNull(expression);

        if (beforeChanged && sender is INotifyPropertyChanging before)
        {
            var obs = Observable.FromEvent<PropertyChangingEventHandler, string?>(
             eventHandler =>
             {
                 void Handler(object? eventSender, PropertyChangingEventArgs e) => eventHandler(e.PropertyName);
                 return Handler;
             },
             x => before!.PropertyChanging += x,
             x => before!.PropertyChanging -= x);

            if (expression.NodeType == ExpressionType.Index)
            {
                return obs.Where(x => string.IsNullOrEmpty(x)
                                      || x?.Equals(propertyName + "[]", StringComparison.InvariantCulture) == true)
                          .Select(_ => new ObservedChange<object?, object?>(sender, expression, default));
            }

            return obs.Where(x => string.IsNullOrEmpty(x)
                                  || x?.Equals(propertyName, StringComparison.InvariantCulture) == true)
                      .Select(_ => new ObservedChange<object?, object?>(sender, expression, default));
        }
        else if (sender is INotifyPropertyChanged after)
        {
            var obs = Observable.FromEvent<PropertyChangedEventHandler, string?>(
             eventHandler =>
             {
                 void Handler(object? eventSender, PropertyChangedEventArgs e) => eventHandler(e.PropertyName);
                 return Handler;
             },
             x => after!.PropertyChanged += x,
             x => after!.PropertyChanged -= x);

            if (expression.NodeType == ExpressionType.Index)
            {
                return obs.Where(x => string.IsNullOrEmpty(x)
                                      || x?.Equals(propertyName + "[]", StringComparison.InvariantCulture) == true)
                          .Select(_ => new ObservedChange<object?, object?>(sender, expression, default));
            }

            return obs.Where(x => string.IsNullOrEmpty(x)
                                  || x?.Equals(propertyName, StringComparison.InvariantCulture) == true)
                      .Select(_ => new ObservedChange<object?, object?>(sender, expression, default));
        }
        else
        {
            return Observable<IObservedChange<object?, object?>>.Never;
        }
    }
}
