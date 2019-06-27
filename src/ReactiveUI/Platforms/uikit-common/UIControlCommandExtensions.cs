// Copyright (c) 2019 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Reactive.Disposables;
using System.Windows.Input;
using UIKit;

namespace ReactiveUI
{
    /// <summary>
    /// Extension methods for binding <see cref="ICommand"/> to a <see cref="UIControl"/>.
    /// </summary>
    public static class UIControlCommandExtensions
    {
        /// <summary>
        /// Binds the <see cref="ICommand"/> to target <see cref="UIControl"/>.
        /// </summary>
        /// <param name="item">The command to bind to.</param>
        /// <param name="control">The control.</param>
        /// <param name="events">The events.</param>
        /// <returns>A disposable.</returns>
        public static IDisposable BindToTarget(this ICommand item, UIControl control, UIControlEvent events)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            if (control == null)
            {
                throw new ArgumentNullException(nameof(control));
            }

            var ev = new EventHandler((o, e) =>
            {
                if (!item.CanExecute(null))
                {
                    return;
                }

                item.Execute(null);
            });

            var cech = new EventHandler((o, e) =>
            {
                control.Enabled = item.CanExecute(null);
            });

            item.CanExecuteChanged += cech;
            control.AddTarget(ev, events);

            control.Enabled = item.CanExecute(null);

            return Disposable.Create(() =>
            {
                control.RemoveTarget(ev, events);
                item.CanExecuteChanged -= cech;
            });
        }
    }
}
