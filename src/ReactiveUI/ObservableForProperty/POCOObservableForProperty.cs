// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.Concurrent;

namespace ReactiveUI;

/// <summary>
/// This class is the final fallback for WhenAny, and will simply immediately
/// return the value of the type at the time it was created. It will also
/// warn the user that this is probably not what they want to do.
/// </summary>
public class POCOObservableForProperty : ICreatesObservableForProperty
{
    private static readonly ConcurrentDictionary<(Type, string), bool> _hasWarned = new();

    /// <inheritdoc/>
#if NET6_0_OR_GREATER
    [RequiresDynamicCode("GetAffinityForObject uses reflection and type analysis")]
    [RequiresUnreferencedCode("GetAffinityForObject may reference members that could be trimmed")]
#endif
    public int GetAffinityForObject(Type type, string propertyName, bool beforeChanged = false) => 1;

    /// <inheritdoc/>
#if NET6_0_OR_GREATER
    [RequiresDynamicCode("GetNotificationForProperty uses reflection and type analysis")]
    [RequiresUnreferencedCode("GetNotificationForProperty may reference members that could be trimmed")]
#endif
    public IObservable<IObservedChange<object, object?>> GetNotificationForProperty(object sender, Expression expression, string propertyName, bool beforeChanged = false, bool suppressWarnings = false)
    {
        sender.ArgumentNullExceptionThrowIfNull(nameof(sender));

        var type = sender.GetType();
        if (!_hasWarned.ContainsKey((type, propertyName)) && !suppressWarnings)
        {
            this.Log().Warn($"The class {type.FullName} property {propertyName} is a POCO type and won't send change notifications, WhenAny will only return a single value!");
            _hasWarned[(type, propertyName)] = true;
        }

        return Observable.Return(new ObservedChange<object, object?>(sender, expression, default), RxApp.MainThreadScheduler ?? CurrentThreadScheduler.Instance)
                         .Concat(Observable<IObservedChange<object, object?>>.Never);
    }
}
