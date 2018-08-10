// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reactive.Linq;
using Splat;

namespace ReactiveUI
{
    /// <summary>
    /// This class is the final fallback for WhenAny, and will simply immediately
    /// return the value of the type at the time it was created. It will also 
    /// warn the user that this is probably not what they want to do
    /// </summary>
    public class POCOObservableForProperty : ICreatesObservableForProperty 
    {
        public int GetAffinityForObject(Type type, string propertyName, bool beforeChanged = false)
        {
            return 1;
        }

        private static readonly Dictionary<(Type, string), bool> hasWarned = new Dictionary<(Type, string), bool>();
        public IObservable<IObservedChange<object, object>> GetNotificationForProperty(object sender, Expression expression, string propertyName, bool beforeChanged = false)
        {
            var type = sender.GetType();
            if (!hasWarned.ContainsKey((type, propertyName))) {
                this.Log().Warn($"The class {type.FullName} property {propertyName} is a POCO type and won't send change notifications, WhenAny will only return a single value!");
                hasWarned[(type, propertyName)] = true;
            }

            return Observable.Return(new ObservedChange<object, object>(sender, expression), RxApp.MainThreadScheduler)
                .Concat(Observable<IObservedChange<object, object>>.Never);
        }
    }
}
