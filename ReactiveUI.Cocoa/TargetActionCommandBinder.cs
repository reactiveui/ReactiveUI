using System;
using System.Linq;
using System.Windows.Input;
using ReactiveUI.Xaml;
using MonoMac.Foundation;
using MonoMac.AppKit;
using MonoMac.ObjCRuntime;
using System.Reactive.Disposables;

namespace ReactiveUI.Cocoa
{
    public class TargetActionCommandBinder : ICreatesCommandBinding
    {
        Type[] validTypes;
        public TargetActionCommandBinder() 
        {
            validTypes = new[]
            {
                typeof(NSControl),
                typeof(NSCell),
                typeof(NSMenu),
                typeof(NSMenuItem),
            };
        }

        public int GetAffinityForObject(Type type, bool hasEventTarget)
        {
            if (!validTypes.Any(x => x.IsAssignableFrom(type)))
                return 0;

            return !hasEventTarget ? 4 : 0;
        }

        public IDisposable BindCommandToObject(ICommand command, object target, IObservable<object> commandParameter)
        {
            var ctlDelegate = new ControlDelegate(x => {
                if (command.CanExecute(x)) command.Execute(x);
            });

            var sel = new Selector("theAction:");
            Reflection.GetValueSetterOrThrow(target.GetType(), "Action")(target, sel);

            var targetSetter = Reflection.GetValueSetterOrThrow(target.GetType(), "Target");
            targetSetter(target, ctlDelegate);

            return Disposable.Create(() => targetSetter(target, null));
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
