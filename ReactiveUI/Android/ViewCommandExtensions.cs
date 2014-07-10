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