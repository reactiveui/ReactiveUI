// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MS-PL license.
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
    public static class ViewCommandExtensions
    {
        public static IDisposable BindToTarget(this ICommand This, View control)
        {
            var ev = new EventHandler((o, e) => {
                if (!This.CanExecute(null)) return;
                This.Execute(null);
            });

            var cech = new EventHandler((o, e) => {
                var canExecute = This.CanExecute(null);
                control.Enabled = canExecute;
            });

            This.CanExecuteChanged += cech;
            control.Click += ev;

            control.Enabled = This.CanExecute(null);

            return Disposable.Create(() => {
                This.CanExecuteChanged -= cech;
                control.Click -= ev;
            });
        }
    }

}