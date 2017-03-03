using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reflection;
using System.Windows.Input;
#if UIKIT
using UIKit;
#endif

namespace ReactiveUI
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
                throw new NotSupportedException(string.Format("CommandBinding for {0} is not supported", type.Name));
            }

            var typeProperties = config[match];

            return typeProperties.CreateBinding(command, target, commandParameter);
        }

        public IDisposable BindCommandToObject<TEventArgs>(ICommand command, object target, IObservable<object> commandParameter, string eventName)
#if MONO
            where TEventArgs : EventArgs
#endif
        {
            throw new NotImplementedException();
        }

        class CommandBindingInfo
        {
            public int Affinity;
            public Func<ICommand, object, IObservable<object>, IDisposable> CreateBinding;
        }

        /// <summary>
        /// Configuration map
        /// </summary>
        readonly Dictionary<Type, CommandBindingInfo> config =
            new Dictionary<Type, CommandBindingInfo>();

        /// <summary>
        /// Registers an observable factory for the specified type and property.
        /// </summary>
        /// <param name="type">Type.</param>
        /// <param name="property">Property.</param>
        /// <param name="createObservable">Create observable.</param>
        protected void Register(Type type, int affinity, Func<System.Windows.Input.ICommand, object, IObservable<object>, IDisposable> createBinding)
        {
            config[type] = new CommandBindingInfo { Affinity = affinity, CreateBinding = createBinding };
        }

        /// <summary>
        /// Creates a commands binding from event and a property
        /// </summary>
        /// <returns>The binding from event.</returns>
        /// <param name="command">Command.</param>
        /// <param name="target">Target.</param>
        /// <param name="commandParameter">Command parameter.</param>
        /// <param name="eventName">Event name.</param>
        /// <param name="enablePropertyName">Enable property name.</param>
        protected static IDisposable ForEvent(ICommand command, object target, IObservable<object> commandParameter, string eventName, PropertyInfo enabledProperty)
        {
            commandParameter = commandParameter ?? Observable.Return(target);

            object latestParam = null;
            var ctl = target;

            var actionDisp = Observable.FromEventPattern(ctl, eventName).Subscribe((e) => {
                if (command.CanExecute(latestParam))
                    command.Execute(latestParam);
            });

            var enabledSetter = Reflection.GetValueSetterForProperty(enabledProperty);
            if (enabledSetter == null) return actionDisp;

            // initial enabled state
            enabledSetter(target, command.CanExecute(latestParam), null);

            var compDisp = new CompositeDisposable(
                actionDisp,
                commandParameter.Subscribe(x => latestParam = x),
                Observable.FromEventPattern<EventHandler, EventArgs>(x => command.CanExecuteChanged += x, x => command.CanExecuteChanged -= x)
                    .Select(_ => command.CanExecute(latestParam))
                    .Subscribe(x => enabledSetter(target, x, null)));

            return compDisp;
        }

#if UIKIT
        /// <summary>
        /// Creates a commands binding from event and a property
        /// </summary>
        protected static IDisposable ForTargetAction(ICommand command, object target, IObservable<object> commandParameter, PropertyInfo enabledProperty)
        {
            commandParameter = commandParameter ?? Observable.Return(target);

            object latestParam = null;

            IDisposable actionDisp = null;

            var ctl = target as UIControl;
            if (ctl != null) {
                var eh = new EventHandler((o, e) => {
                    if (command.CanExecute(latestParam)) command.Execute(latestParam);
                });

                ctl.AddTarget(eh, UIControlEvent.TouchUpInside);
                actionDisp = Disposable.Create(() => ctl.RemoveTarget(eh, UIControlEvent.TouchUpInside));
            }

            var enabledSetter = Reflection.GetValueSetterForProperty(enabledProperty);
            if (enabledSetter == null) return actionDisp;

            // Initial enabled state
            enabledSetter(target, command.CanExecute(latestParam), null);

            var compDisp = new CompositeDisposable(
                actionDisp,
                commandParameter.Subscribe(x => latestParam = x),
                Observable.FromEventPattern<EventHandler, EventArgs>(x => command.CanExecuteChanged += x, x => command.CanExecuteChanged -= x)
                    .Select(_ => command.CanExecute(latestParam))
                    .Subscribe(x => enabledSetter(target, x, null)));

            return compDisp;
        }
#endif
    }
}

