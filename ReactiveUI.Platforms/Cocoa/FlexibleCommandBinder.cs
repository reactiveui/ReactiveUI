using System;
using ReactiveUI;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Reactive.Disposables;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using MonoTouch.ObjCRuntime;
using System.Windows.Input;

namespace ReactiveUI.Cocoa
{
    public abstract class FlexibleCommandBinder : ICreatesCommandBinding
    {
        public int GetAffinityForObject(Type type, bool hasEventTarget)
        {
            if (hasEventTarget) return 0;

            var match = config.Keys
                .Where(x => x.IsAssignableFrom(type))
                .OrderByDescending(x => config[x].Affinity)
                .FirstOrDefault();

            if (match == null) return 0;

            var typeProperties = config[match];
            return typeProperties.Affinity;
        }

        public IDisposable BindCommandToObject(ICommand command, object target, IObservable<object> commandParameter)
        {
            var type = target.GetType();

            var match = config.Keys
                .Where(x => x.IsAssignableFrom(type))
                .OrderByDescending(x => config[x].Affinity)
                .FirstOrDefault();

            if (match == null) {
                throw new NotSupportedException(String.Format("CommandBinding for {0} is not supported", type.Name));
            }

            var typeProperties = config[match];

            return typeProperties.CreateBinding(command, target, commandParameter);
        }

        public IDisposable BindCommandToObject<TEventArgs>(ICommand command, object target, IObservable<object> commandParameter, string eventName)
        {
            throw new NotImplementedException();
        }

        class CommandBindingInfo
        {
            public int Affinity;
            public Func<ICommand, object, IObservable<object>, IDisposable> CreateBinding;
        }

        readonly Dictionary<Type, CommandBindingInfo> config = new Dictionary<Type, CommandBindingInfo>();

        /// <summary>
        /// Registers an observable factory for the specified type and property.
        /// </summary>
        protected void Register(Type type, int affinity, Func<ICommand, object, IObservable<object>, IDisposable> createBinding)
        {
            config[type] = new CommandBindingInfo { Affinity = affinity, CreateBinding = createBinding };
        }

        /// <summary>
        /// Creates a commands binding from event and a property
        /// </summary>
        /// <returns>The binding from event.</returns>
        protected static IDisposable ForEvent(ICommand command, object target, IObservable<object> commandParameter, string eventName, string enablePropertyName)
        {
            commandParameter = commandParameter ?? Observable.Return(target);

            var latestParam = default(object);
            var ctl = target;

            var actionDisp = Observable.FromEventPattern(ctl, eventName).Subscribe((e) => {
                if (command.CanExecute(latestParam))
                    command.Execute(latestParam);
            });

            var enabledSetter = Reflection.GetValueSetterForProperty(target.GetType(), enablePropertyName);
            if (enabledSetter == null) return actionDisp;

            // initial enabled state
            enabledSetter(target, command.CanExecute(latestParam));

            var compDisp = new CompositeDisposable(
                actionDisp,
                commandParameter.Subscribe(x => latestParam = x),
                Observable.FromEventPattern<EventHandler, EventArgs>(x => command.CanExecuteChanged += x, x => command.CanExecuteChanged -= x)
                    .Select(_ => command.CanExecute(latestParam))
                    .Subscribe(x => enabledSetter(target, x)));

            return compDisp;
        }

        /// <summary>
        /// Creates a commands binding from event and a property
        /// </summary>
        protected static IDisposable ForTargetAction(ICommand command, object target, IObservable<object> commandParameter, string enablePropertyName)
        {
            commandParameter = commandParameter ?? Observable.Return(target);

            object latestParam = null;

#if UIKIT
            IDisposable actionDisp = null;

            var ctl = target as UIControl;
            if (ctl != null) {
                var eh = new EventHandler((o,e) => {
                    if (command.CanExecute(latestParam)) command.Execute(latestParam);
                });

                ctl.AddTarget(eh, UIControlEvent.TouchUpInside);
                actionDisp = Disposable.Create(() => ctl.RemoveTarget(eh, UIControlEvent.TouchUpInside));
            } 
#else
            Reflection.GetValueSetterOrThrow(target.GetType(), "Action")(target, sel);

            var targetSetter = Reflection.GetValueSetterOrThrow(target.GetType(), "Target");
            targetSetter(target, ctlDelegate);
            var actionDisp = Disposable.Create(() => targetSetter(target, null));
#endif

            var enabledSetter = Reflection.GetValueSetterForProperty(target.GetType(), enablePropertyName);
            if (enabledSetter == null) return actionDisp;

            // Initial enabled state
            enabledSetter(target, command.CanExecute(latestParam));

            var compDisp = new CompositeDisposable(
                actionDisp,
                commandParameter.Subscribe(x => latestParam = x),
                Observable.FromEventPattern<EventHandler, EventArgs>(x => command.CanExecuteChanged += x, x => command.CanExecuteChanged -= x)
                    .Select(_ => command.CanExecute(latestParam))
                    .Subscribe(x => enabledSetter(target, x)));

            return compDisp;
        }
    }
}

