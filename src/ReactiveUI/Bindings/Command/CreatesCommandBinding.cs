// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Reflection;
using System.Windows.Input;
using Splat;

#pragma warning disable RCS1102 // Make class static. Used as base class.

namespace ReactiveUI
{
    internal class CreatesCommandBinding
    {
        private static readonly MemoizingMRUCache<Type, ICreatesCommandBinding?> bindCommandCache =
            new(
                (t, _) =>
            {
                return Locator.Current.GetServices<ICreatesCommandBinding>()
                    .Aggregate((score: 0, binding: (ICreatesCommandBinding?)null), (acc, x) =>
                    {
                        int score = x.GetAffinityForObject(t, false);
                        return (score > acc.score) ? (score, x) : acc;
                    }).binding;
            }, RxApp.SmallCacheLimit);

        private static readonly MemoizingMRUCache<Type, ICreatesCommandBinding?> bindCommandEventCache =
            new(
                (t, _) =>
            {
                return Locator.Current.GetServices<ICreatesCommandBinding>()
                    .Aggregate((score: 0, binding: (ICreatesCommandBinding?)null), (acc, x) =>
                    {
                        int score = x.GetAffinityForObject(t, true);
                        return (score > acc.score) ? (score, x) : acc;
                    }).binding;
            }, RxApp.SmallCacheLimit);

        public static IDisposable BindCommandToObject(ICommand command, object target, IObservable<object> commandParameter)
        {
            var type = target.GetType();

            var binder = bindCommandCache.Get(type);

            if (binder is null)
            {
                throw new Exception($"Couldn't find a Command Binder for {type.FullName}");
            }

            var ret = binder.BindCommandToObject(command, target, commandParameter);
            if (ret is null)
            {
                throw new Exception($"Couldn't bind Command Binder for {type.FullName}");
            }

            return ret;
        }

        public static IDisposable BindCommandToObject(ICommand command, object target, IObservable<object> commandParameter, string eventName)
        {
            var type = target.GetType();
            var binder = bindCommandEventCache.Get(type);
            if (binder is null)
            {
                throw new Exception($"Couldn't find a Command Binder for {type.FullName} and event {eventName}");
            }

            var eventArgsType = Reflection.GetEventArgsTypeForEvent(type, eventName);
            var mi = binder.GetType().GetTypeInfo().DeclaredMethods.First(x => x.Name == "BindCommandToObject" && x.IsGenericMethod);
            mi = mi.MakeGenericMethod(eventArgsType);

            var ret = (IDisposable)mi.Invoke(binder, new[] { command, target, commandParameter, eventName })!;
            if (ret is null)
            {
                throw new Exception($"Couldn't bind Command Binder for {type.FullName} and event {eventName}");
            }

            return ret;
        }
    }
}
