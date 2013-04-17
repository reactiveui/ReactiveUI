using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Input;

#if WINRT
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
#endif

namespace ReactiveUI.Xaml
{
    public interface ICreatesCommandBinding
    {
        /// <summary>
        /// Returns a positive integer when this class supports 
        /// BindCommandToObject for this particular Type. If the method
        /// isn't supported at all, return a non-positive integer. When multiple
        /// implementations return a positive value, the host will use the one
        /// which returns the highest value. When in doubt, return '2' or '0'
        /// </summary>
        /// <param name="type">The type to query for.</param>
        /// <param name="hasEventTarget">If true, the host intends to use a custom
        /// event target.</param>
        /// <returns>A positive integer if BCTO is supported, zero or a negative
        /// value otherwise</returns>
        int GetAffinityForObject(Type type, bool hasEventTarget);

        /// <summary>
        /// Bind an ICommand to a UI object, in the "default" way. The meaning 
        /// of this is dependent on the implementation. Implement this if you
        /// have a new type of UI control that doesn't have 
        /// Command/CommandParameter like WPF or has a non-standard event name
        /// for "Invoke".
        /// </summary>
        /// <param name="command">The command to bind</param>
        /// <param name="target">The target object, usually a UI control of 
        /// some kind</param>
        /// <param name="commandParameter">An IObservable source whose latest 
        /// value will be passed as the command parameter to the command. Hosts
        /// will always pass a valid IObservable, but this may be 
        /// Observable.Empty</param>
        /// <returns>An IDisposable which will disconnect the binding when 
        /// disposed.</returns>
        IDisposable BindCommandToObject(ICommand command, object target, IObservable<object> commandParameter);

        /// <summary>
        /// Bind an ICommand to a UI object to a specific event. This event may
        /// be a standard .NET event, or it could be an event derived in another
        /// manner (i.e. in MonoTouch).
        /// </summary>
        /// <param name="command">The command to bind</param>
        /// <param name="target">The target object, usually a UI control of 
        /// some kind</param>
        /// <param name="commandParameter">An IObservable source whose latest 
        /// value will be passed as the command parameter to the command. Hosts
        /// will always pass a valid IObservable, but this may be 
        /// Observable.Empty</param>
        /// <param name="eventName">The event to bind to.</param>
        /// <returns></returns>
        /// <returns>An IDisposable which will disconnect the binding when 
        /// disposed.</returns>
        IDisposable BindCommandToObject<TEventArgs>(ICommand command, object target, IObservable<object> commandParameter, string eventName)
#if !WINRT
        where TEventArgs : EventArgs
#endif
        ; // Lol.
    }

    public class CreatesCommandBindingViaEvent : ICreatesCommandBinding
    {
        // NB: These are in priority order
        static readonly List<Tuple<string, Type>> defaultEventsToBind = new List<Tuple<string, Type>>() {
#if !MONO
            Tuple.Create("Click", typeof (RoutedEventArgs)),
#endif
            Tuple.Create("TouchUpInside", typeof (EventArgs)),
#if !MONO && !WINRT
            Tuple.Create("MouseUp", typeof (MouseButtonEventArgs)),
#elif WINRT
            Tuple.Create("PointerReleased", typeof(PointerRoutedEventArgs)),
            Tuple.Create("Tapped", typeof(TappedRoutedEventArgs)),
#endif
        };

        public int GetAffinityForObject(Type type, bool hasEventTarget)
        {
            if (hasEventTarget) return 5;

            return defaultEventsToBind.Any(x => {
                var ei = type.GetEvent(x.Item1, BindingFlags.Public | BindingFlags.FlattenHierarchy | BindingFlags.Instance);
                return ei != null;
            }) ? 3 : 0;
        }

        public IDisposable BindCommandToObject(ICommand command, object target, IObservable<object> commandParameter)
        {
            const BindingFlags bf = BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy;

            var type = target.GetType();
            var eventInfo = defaultEventsToBind
                .Select(x => new { EventInfo = type.GetEvent(x.Item1, bf), Args = x.Item2 })
                .FirstOrDefault(x => x.EventInfo != null);

            if (eventInfo == null) return null;

            var mi = GetType().GetMethods().First(x => x.Name == "BindCommandToObject" && x.IsGenericMethod);
            mi = mi.MakeGenericMethod(eventInfo.Args);

            return (IDisposable)mi.Invoke(this, new[] {command, target, commandParameter, eventInfo.EventInfo.Name});
        }

        public IDisposable BindCommandToObject<TEventArgs>(ICommand command, object target, IObservable<object> commandParameter, string eventName)
#if !WINRT
            where TEventArgs : EventArgs            
#endif
        {
            var ret = new CompositeDisposable();

            object latestParameter = null;
            bool useEventArgsInstead = false;

            // NB: This is a bit of a hack - if commandParameter isn't specified,
            // it will default to Observable.Empty. We're going to use termination
            // of the commandParameter as a signal to use EventArgs.
            ret.Add(commandParameter.Subscribe(
                x => latestParameter = x,
                () => useEventArgsInstead = true));

            var evt = Observable.FromEventPattern<TEventArgs>(target, eventName);
            ret.Add(evt.Subscribe(ea => {
                if (command.CanExecute(useEventArgsInstead ? ea : latestParameter)) {
                    command.Execute(useEventArgsInstead ? ea : latestParameter);
                }
            }));

            return ret;
        }
    }

    public class CreatesCommandBindingViaCommandParameter : ICreatesCommandBinding
    {
        public int GetAffinityForObject(Type type, bool hasEventTarget)
        {
            if (hasEventTarget) return 0;

            var propsToFind = new[] {
                new { Name = "Command", TargetType = typeof(ICommand) },
                new { Name = "CommandParameter", TargetType = typeof(object) },
            };

            return propsToFind.All(x => {
                var pi = Reflection.GetValueFetcherForProperty(type, x.Name);
                return pi != null;
            }) ? 5 : 0;
        }

        public IDisposable BindCommandToObject(ICommand command, object target, IObservable<object> commandParameter)
        {
            var type = target.GetType();
            var cmdPi = type.GetProperty("Command", BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Instance);
            var cmdParamPi = type.GetProperty("CommandParameter", BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Instance);
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

        public IDisposable BindCommandToObject<TEventArgs>(ICommand command, object target, IObservable<object> commandParameter, string eventName)
#if !WINRT
            where TEventArgs : EventArgs
#endif
        {
            // NB: We should fall back to the generic Event-based handler if
            // an event target is specified
            return null;
        }
    }
}