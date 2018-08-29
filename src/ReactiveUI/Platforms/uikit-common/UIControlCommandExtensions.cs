// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Reactive.Disposables;
using System.Windows.Input;
using UIKit;

namespace ReactiveUI
{
    public static class UIControlCommandExtensions
    {
        public static IDisposable BindToTarget(this ICommand @this, UIControl control, UIControlEvent events)
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
            control.AddTarget(ev, events);

            control.Enabled = @this.CanExecute(null);

            return Disposable.Create(() =>
            {
                control.RemoveTarget(ev, events);
                @this.CanExecuteChanged -= cech;
            });
        }
    }
}
