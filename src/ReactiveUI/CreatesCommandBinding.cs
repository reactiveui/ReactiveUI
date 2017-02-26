using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Input;

#if NETFX_CORE
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
#endif

namespace ReactiveUI
{
    /// <summary>
    /// This binder is the default binder for connecting to arbitrary events
    /// </summary>
    public class CreatesCommandBindingViaEvent : ICreatesCommandBinding
    {
        // NB: These are in priority order
        private static readonly List<Tuple<string, Type>> defaultEventsToBind = new List<Tuple<string, Type>>() {
#if !MONO && !PORTABLE
            Tuple.Create("Click", typeof (RoutedEventArgs)),
#endif
            Tuple.Create("TouchUpInside", typeof (EventArgs)),
#if !MONO && !NETFX_CORE && !PORTABLE
            Tuple.Create("MouseUp", typeof (MouseButtonEventArgs)),
#elif NETFX_CORE
            Tuple.Create("PointerReleased", typeof(PointerRoutedEventArgs)),
            Tuple.Create("Tapped", typeof(TappedRoutedEventArgs)),
#endif
        };

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
            if (hasEventTarget) return 5;

            return defaultEventsToBind.Any(x => {
                var ei = type.GetRuntimeEvent(x.Item1);
                return ei != null;
            }) ? 3 : 0;
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
        /// <exception cref="System.Exception"></exception>
        public IDisposable BindCommandToObject(ICommand command, object target, IObservable<object> commandParameter)
        {
            var type = target.GetType();
            var eventInfo = defaultEventsToBind
                .Select(x => new { EventInfo = type.GetRuntimeEvent(x.Item1), Args = x.Item2 })
                .FirstOrDefault(x => x.EventInfo != null);

            if (eventInfo == null) {
                throw new Exception(
                    string.Format(
                        "Couldn't find a default event to bind to on {0}, specify an event expicitly",
                        target.GetType().FullName));
            }

            var mi = GetType().GetRuntimeMethods().First(x => x.Name == "BindCommandToObject" && x.IsGenericMethod);
            mi = mi.MakeGenericMethod(eventInfo.Args);

            return (IDisposable)mi.Invoke(this, new[] { command, target, commandParameter, eventInfo.EventInfo.Name });
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
        public IDisposable BindCommandToObject<TEventArgs>(ICommand command, object target, IObservable<object> commandParameter, string eventName)
#if MONO
            where TEventArgs : EventArgs
#endif
        {
            var ret = new CompositeDisposable();

            object latestParameter = null;
            var evt = Observable.FromEventPattern<TEventArgs>(target, eventName);

            ret.Add(commandParameter.Subscribe(x => latestParameter = x));

            ret.Add(evt.Subscribe(ea => {
                if (command.CanExecute(latestParameter)) {
                    command.Execute(latestParameter);
                }
            }));

            return ret;
        }
    }

    /// <summary>
    /// Creates Command Binding Via Command Parameter
    /// </summary>
    /// <seealso cref="ReactiveUI.ICreatesCommandBinding"/>
    public class CreatesCommandBindingViaCommandParameter : ICreatesCommandBinding
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

            var propsToFind = new[] {
                new { Name = "Command", TargetType = typeof(ICommand) },
                new { Name = "CommandParameter", TargetType = typeof(object) },
            };

            return propsToFind.All(x => {
                var pi = type.GetRuntimeProperty(x.Name);
                return pi != null;
            }) ? 5 : 0;
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
        public IDisposable BindCommandToObject(ICommand command, object target, IObservable<object> commandParameter)
        {
            var type = target.GetType();
            var cmdPi = type.GetRuntimeProperty("Command");
            var cmdParamPi = type.GetRuntimeProperty("CommandParameter");
            var ret = new CompositeDisposable();

            var originalCmd = cmdPi.GetValue(target, null);
            var originalCmdParam = cmdParamPi.GetValue(target, null);

            ret.Add(Disposable.Create(() => {
                cmdPi.SetValue(target, originalCmd, null);
                cmdParamPi.SetValue(target, originalCmdParam, null);
            }));

            ret.Add(commandParameter.Subscribe(x => cmdParamPi.SetValue(target, x, null)));
            cmdPi.SetValue(target, command, null);

            return ret;
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
        public IDisposable BindCommandToObject<TEventArgs>(ICommand command, object target, IObservable<object> commandParameter, string eventName)
#if MONO
            where TEventArgs : EventArgs
#endif
        {
            // NB: We should fall back to the generic Event-based handler if an event target is specified
            return null;
        }
    }
}