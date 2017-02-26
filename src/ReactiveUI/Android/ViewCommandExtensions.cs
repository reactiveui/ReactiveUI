using System;
using System.Reactive.Disposables;
using System.Windows.Input;
using Android.Views;

namespace ReactiveUI
{
    /// <summary>
    /// View Command Extensions
    /// </summary>
    public static class ViewCommandExtensions
    {
        /// <summary>
        /// Binds to target.
        /// </summary>
        /// <param name="This">The this.</param>
        /// <param name="control">The control.</param>
        /// <returns></returns>
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