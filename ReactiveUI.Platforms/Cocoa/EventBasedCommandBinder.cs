using System;
using ReactiveUI;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Reactive.Disposables;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;

namespace ReactiveUI.Cocoa
{
    public abstract class EventBasedCommandBinder :
        ICreatesCommandBinding
    {
        #region ICreatesCommandBinding implementation

        public int GetAffinityForObject(Type type, bool hasEventTarget)
        {
            CommandBindingInfo typeProperties;
            if(!config.TryGetValue(type, out typeProperties))
                return 0;

            return typeProperties.Affinity;
        }

        public IDisposable BindCommandToObject(System.Windows.Input.ICommand command, object target, IObservable<object> commandParameter)
        {
            var type = target.GetType();

            CommandBindingInfo typeProperties;
            if(!config.TryGetValue(type, out typeProperties))
                throw new NotSupportedException(string.Format("CommandBinding for {0} is not supported", type.Name));

            return typeProperties.CreateBinding(command, target, commandParameter);
        }

        public IDisposable BindCommandToObject<TEventArgs>(System.Windows.Input.ICommand command, object target, IObservable<object> commandParameter, string eventName)
        {
            throw new NotImplementedException();
        }

        #endregion

        internal class CommandBindingInfo
        {
            public int Affinity;
            public Func<System.Windows.Input.ICommand, object, IObservable<object>, IDisposable> CreateBinding;
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

        protected static IDisposable CommandBindingFromEvent(System.Windows.Input.ICommand command, object target,
            IObservable<object> commandParameter, string eventName, string enablePropertyName)
        {
            commandParameter = commandParameter ?? Observable.Return(target);

            object latestParam = null;
            var ctl = (UIRefreshControl)target;

            var actionDisp = Observable.FromEventPattern(ctl, eventName).Subscribe((e) => {
                if (command.CanExecute(latestParam))
                    command.Execute(latestParam);
            });

            var enabledSetter = Reflection.GetValueSetterForProperty(target.GetType(), enablePropertyName);
            if(enabledSetter == null) return actionDisp;

            // initial enabled state
            enabledSetter(target, command.CanExecute(latestParam));

            var compDisp = new CompositeDisposable(
                actionDisp,
                commandParameter.Subscribe(x => latestParam = x),
                Observable.FromEventPattern<EventHandler, EventArgs>(x => command.CanExecuteChanged += x, x => command.CanExecuteChanged -= x)
                .Select(_ => command.CanExecute(latestParam))
                .Subscribe(x => {
                enabledSetter(target, x);
            }));

            return compDisp;
        }
    }
}

