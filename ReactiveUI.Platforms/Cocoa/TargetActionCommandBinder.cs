using System;
using System.Linq;
using System.Windows.Input;
using ReactiveUI;
using ReactiveUI.Xaml;
using System.Reactive.Disposables;
using System.Reactive.Linq;

#if UIKIT
using MonoTouch.Foundation;
using MonoTouch.ObjCRuntime;
using MonoTouch.UIKit;
#else
using MonoMac.Foundation;
using MonoMac.AppKit;
using MonoMac.ObjCRuntime;
#endif

namespace ReactiveUI.Cocoa
{
    public class TargetActionCommandBinder : ICreatesCommandBinding
    {
        Type[] validTypes;
        public TargetActionCommandBinder() 
        {
#if UIKIT
            validTypes = new[]
            {
                typeof(UIControl),
            };
#else
            validTypes = new[]
            {
                typeof(NSControl),
                typeof(NSCell),
                typeof(NSMenu),
                typeof(NSMenuItem),
            };
#endif
        }

        public int GetAffinityForObject(Type type, bool hasEventTarget)
        {
            if (!validTypes.Any(x => x.IsAssignableFrom(type)))
                return 0;

            return !hasEventTarget ? 4 : 0;
        }

        public IDisposable BindCommandToObject(ICommand command, object target, IObservable<object> commandParameter)
        {
            commandParameter = commandParameter ?? Observable.Return(target);

            object latestParam = null;
            var ctlDelegate = new ControlDelegate(x => {
                if (command.CanExecute(latestParam))
                    command.Execute(latestParam);
            });

            var sel = new Selector("theAction:");
#if UIKIT
            var ctl = (UIControl)target;
            ctl.AddTarget(ctlDelegate, sel, UIControlEvent.TouchUpInside);
            var actionDisp = Disposable.Create(() => ctl.RemoveTarget(ctlDelegate, sel, UIControlEvent.TouchUpInside));
#else
            Reflection.GetValueSetterOrThrow(target.GetType(), "Action")(target, sel);
            var targetSetter = Reflection.GetValueSetterOrThrow(target.GetType(), "Target");
            targetSetter(target, ctlDelegate);
            var actionDisp = Disposable.Create(() => targetSetter(target, null));
#endif

            var enabledSetter = Reflection.GetValueSetterForProperty(target.GetType(), "Enabled");
            var disp = new CompositeDisposable(
                actionDisp,
                commandParameter.Subscribe(x => latestParam = x),
                Observable.FromEventPattern<EventHandler, EventArgs>(x => command.CanExecuteChanged += x, x => command.CanExecuteChanged -= x)
                    .Select(_ => command.CanExecute(latestParam))
                    .Subscribe(x => {
                        if (enabledSetter == null) return;
                        enabledSetter(target, x);
                    }));

            return disp;
        }

        class ControlDelegate : NSObject
        {
            readonly Action<NSObject> block;
            public ControlDelegate(Action<NSObject> block)
            {
                this.block = block;
            }

            [Export("theAction:")]
            public void TheAction(NSObject sender)
            {
                block(sender);
            }
        }

        public IDisposable BindCommandToObject<TEventArgs>(ICommand command, object target, IObservable<object> commandParameter, string eventName) 
            where TEventArgs : EventArgs
        {
            throw new NotImplementedException();
        }
    }
}
