// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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
        /// <param name="this">The this.</param>
        /// <param name="control">The control.</param>
        /// <returns>A disposable.</returns>
        public static IDisposable BindToTarget(this ICommand @this, View control)
        {
            var ev = new EventHandler((o, e) =>
            {
                if (!@this.CanExecute(null))
                {
                    return;
                }

                @this.Execute(null);
            });

            var cech = new EventHandler((o, e) =>
            {
                var canExecute = @this.CanExecute(null);
                control.Enabled = canExecute;
            });

            @this.CanExecuteChanged += cech;
            control.Click += ev;

            control.Enabled = @this.CanExecute(null);

            return Disposable.Create(() =>
            {
                @this.CanExecuteChanged -= cech;
                control.Click -= ev;
            });
        }
    }
}
