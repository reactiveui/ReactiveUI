// Copyright (c) 2022 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reactive.Linq;
using Splat;

namespace ReactiveUI;

/// <summary>
/// This class is the final fallback for WhenAny, and will simply immediately
/// return the value of the type at the time it was created. It will also
/// warn the user that this is probably not what they want to do.
/// </summary>
public class POCOObservableForProperty : ICreatesObservableForProperty
{
    private static readonly IDictionary<(Type, string), bool> _hasWarned = new ConcurrentDictionary<(Type, string), bool>();

    /// <inheritdoc/>
    public int GetAffinityForObject(Type type, string propertyName, bool beforeChanged = false) => 1;

    /// <inheritdoc/>
    public IObservable<IObservedChange<object, object?>> GetNotificationForProperty(object sender, Expression expression, string propertyName, bool beforeChanged = false, bool suppressWarnings = false)
    {
        if (sender is null)
        {
            throw new ArgumentNullException(nameof(sender));
        }

        var type = sender.GetType();
        if (!_hasWarned.ContainsKey((type, propertyName)) && !suppressWarnings)
        {
            this.Log().Debug($"The class {type.FullName} property {propertyName} is a POCO type and won't send change notifications, WhenAny will only return a single value!");
            _hasWarned[(type, propertyName)] = true;
        }

        return Observable.Return(new ObservedChange<object, object?>(sender, expression, default), RxApp.MainThreadScheduler)
                         .Concat(Observable<IObservedChange<object, object?>>.Never);
    }
}
