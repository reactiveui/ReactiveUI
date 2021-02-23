// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Reactive.Disposables;
using System.Reflection;
using System.Windows.Input;

namespace ReactiveUI
{
    /// <summary>
    /// Class that registers Command Binding and Command Parameter Binding.
    /// </summary>
    public class CreatesCommandBindingViaCommandParameter : ICreatesCommandBinding
    {
        /// <inheritdoc/>
        public int GetAffinityForObject(Type type, bool hasEventTarget)
        {
            if (hasEventTarget)
            {
                return 0;
            }

            var propsToFind = new[]
            {
                new { Name = "Command", TargetType = typeof(ICommand) },
                new { Name = "CommandParameter", TargetType = typeof(object) },
            };

            return propsToFind.All(x =>
            {
                var pi = type.GetRuntimeProperty(x.Name);
                return pi is not null;
            }) ? 5 : 0;
        }

        /// <inheritdoc/>
        public IDisposable BindCommandToObject(ICommand command, object target, IObservable<object> commandParameter)
        {
            if (target is null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            var type = target.GetType();
            var cmdPi = type.GetRuntimeProperty("Command");
            var cmdParamPi = type.GetRuntimeProperty("CommandParameter");
            var ret = new CompositeDisposable();

            var originalCmd = cmdPi?.GetValue(target, null);
            var originalCmdParam = cmdParamPi?.GetValue(target, null);

            ret.Add(Disposable.Create(() =>
            {
                cmdPi?.SetValue(target, originalCmd, null);
                cmdParamPi?.SetValue(target, originalCmdParam, null);
            }));

            ret.Add(commandParameter.Subscribe(x => cmdParamPi?.SetValue(target, x, null)));
            cmdPi?.SetValue(target, command, null);

            return ret;
        }

        /// <inheritdoc/>
        public IDisposable? BindCommandToObject<TEventArgs>(ICommand command, object target, IObservable<object> commandParameter, string eventName)
#if MONO
            where TEventArgs : EventArgs
#endif
        {
            // NB: We should fall back to the generic Event-based handler if
            // an event target is specified
            return null;
        }
    }
}
