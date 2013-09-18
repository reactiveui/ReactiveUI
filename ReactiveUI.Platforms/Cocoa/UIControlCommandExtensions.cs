using System;
using System.Windows.Input;
using MonoTouch.UIKit;
using System.Reactive.Disposables;

namespace ReactiveUI.Cocoa
{
    public static class UIControlCommandExtensions
    {
        public static IDisposable BindToTarget(this ICommand This, UIControl control, UIControlEvent events)
        {
            var ev = new EventHandler((o,e) => {
                if (!This.CanExecute(null)) return;
                This.Execute(null);
            });

            control.AddTarget(ev, events);
            return Disposable.Create(() => control.RemoveTarget(ev, events));
        }
    }
}

