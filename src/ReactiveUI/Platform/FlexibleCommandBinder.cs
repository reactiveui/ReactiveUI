using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reflection;
using System.Windows.Input;

#if UNIFIED
using UIKit;
#elif UIKIT
using MonoTouch.UIKit;
#endif

namespace ReactiveUI
{
    /// <summary>
    /// Flexible Command Binder
    /// </summary>
    /// <seealso cref="ReactiveUI.ICreatesCommandBinding"/>
    public abstract class FlexibleCommandBinder : ICreatesCommandBinding
    {
        /// <summary>
        /// Returns a positive integer when this class supports BindCommandToObject for this
        /// particular Type. If the method isn't supported at all, return a non-positive integer.
        /// When multiple implementations return a positive value, the host will use the one which
        /// returns the highest value. When in doubt, return '2' or '0'
        /// </summary>
        /// <param name="type">The type to query for.</param>
        /// <param name="hasEventTarget">If true, the host intends to use a custom event target.</param>
        /// <returns>A positive integer if BCTO is supported, zero or a negative value otherwise</returns>
        public int GetAffinityForObject(Type type, bool hasEventTarget)
        {
            if (hasEventTarget) return 0;

            var match = this.config.Keys
                .Where(x => x.IsAssignableFrom(type))
                .OrderByDescending(x => this.config[x].Affinity)
                .FirstOrDefault();

            if (match == null) return 0;

            var typeProperties = this.config[match];
            return typeProperties.Affinity;
        }

        /// <summary>
        /// Bind an ICommand to a UI object, in the "default" way. The meaning of this is dependent
        /// on the implementation. Implement this if you have a new type of UI control that doesn't
        /// have Command/CommandParameter like WPF or has a non-standard event name for "Invoke".
        /// </summary>
        /// <param name="command">The command to bind</param>
        /// <param name="target">The target object, usually a UI control of some kind</param>
        /// <param name="commandParameter">
        /// An IObservable source whose latest value will be passed as the command parameter to the
        /// command. Hosts will always pass a valid IObservable, but this may be Observable.Empty
        /// </param>
        /// <returns>An IDisposable which will disconnect the binding when disposed.</returns>
        /// <exception cref="System.NotSupportedException"></exception>
        public IDisposable BindCommandToObject(ICommand command, object target, IObservable<object> commandParameter)
        {
            var type = target.GetType();

            var match = this.config.Keys
                .Where(x => x.IsAssignableFrom(type))
                .OrderByDescending(x => this.config[x].Affinity)
                .FirstOrDefault();

            if (match == null) {
                throw new NotSupportedException(string.Format("CommandBinding for {0} is not supported", type.Name));
            }

            var typeProperties = this.config[match];

            return typeProperties.CreateBinding(command, target, commandParameter);
        }

        /// <summary>
        /// Bind an ICommand to a UI object to a specific event. This event may be a standard .NET
        /// event, or it could be an event derived in another manner (i.e. in MonoTouch).
        /// </summary>
        /// <typeparam name="TEventArgs"></typeparam>
        /// <param name="command">The command to bind</param>
        /// <param name="target">The target object, usually a UI control of some kind</param>
        /// <param name="commandParameter">
        /// An IObservable source whose latest value will be passed as the command parameter to the
        /// command. Hosts will always pass a valid IObservable, but this may be Observable.Empty
        /// </param>
        /// <param name="eventName">The event to bind to.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public IDisposable BindCommandToObject<TEventArgs>(ICommand command, object target, IObservable<object> commandParameter, string eventName)
#if MONO
            where TEventArgs : EventArgs
#endif
        {
            throw new NotImplementedException();
        }

        private class CommandBindingInfo
        {
            public int Affinity;
            public Func<ICommand, object, IObservable<object>, IDisposable> CreateBinding;
        }

        /// <summary>
        /// Configuration map
        /// </summary>
        private readonly Dictionary<Type, CommandBindingInfo> config =
            new Dictionary<Type, CommandBindingInfo>();

        /// <summary>
        /// Registers an observable factory for the specified type and property.
        /// </summary>
        /// <param name="type">Type.</param>
        /// <param name="affinity">The affinity.</param>
        /// <param name="createBinding">The create binding.</param>
        protected void Register(Type type, int affinity, Func<System.Windows.Input.ICommand, object, IObservable<object>, IDisposable> createBinding)
        {
            this.config[type] = new CommandBindingInfo { Affinity = affinity, CreateBinding = createBinding };
        }

        /// <summary>
        /// Creates a commands binding from event and a property
        /// </summary>
        /// <param name="command">Command.</param>
        /// <param name="target">Target.</param>
        /// <param name="commandParameter">Command parameter.</param>
        /// <param name="eventName">Event name.</param>
        /// <param name="enabledProperty">The enabled property.</param>
        /// <returns>The binding from event.</returns>
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

            return new CompositeDisposable(
                actionDisp,
                commandParameter.Subscribe(x => latestParam = x),
                Observable.FromEventPattern<EventHandler, EventArgs>(x => command.CanExecuteChanged += x, x => command.CanExecuteChanged -= x)
                    .Select(_ => command.CanExecute(latestParam))
                    .Subscribe(x => enabledSetter(target, x, null)));
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
                var eh = new EventHandler((o,e) => {
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