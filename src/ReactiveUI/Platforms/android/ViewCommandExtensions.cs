// Copyright (c) 2019 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Text;
using System.Windows.Input;
using Android.Views;

namespace ReactiveUI
{
    /// <summary>
    /// Extension methods for view commands.
    /// </summary>
    public static class ViewCommandExtensions
    {
        /// <summary>
        /// Binds the command to target view control.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="control">The control.</param>
        /// <returns>A disposable.</returns>
        public static IDisposable BindToTarget(this ICommand command, View control)
        {
            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            if (control == null)
            {
                throw new ArgumentNullException(nameof(control));
            }

            var ev = new EventHandler((o, e) =>
            {
                if (!command.CanExecute(null))
                {
                    return;
                }

                command.Execute(null);
            });

            var cech = new EventHandler((o, e) =>
            {
                control.Enabled = command.CanExecute(null);
            });

            command.CanExecuteChanged += cech;
            control.Click += ev;

            control.Enabled = command.CanExecute(null);

            return Disposable.Create(() =>
            {
                command.CanExecuteChanged -= cech;
                control.Click -= ev;
            });
        }
    }
}
