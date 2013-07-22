using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Forms;
using System.Windows.Input;
using ReactiveUI;

namespace ReactiveUI.Winforms
{
   

    /// <summary>
    /// This binder is the default binder for connecting to arbitrary events
    /// </summary>
    public class CreatesWinformsCommandBinding : ICreatesCommandBinding
    {
        // NB: These are in priority order
        static readonly List<Tuple<string, Type>> defaultEventsToBind = new List<Tuple<string, Type>>() {

            Tuple.Create("Click", typeof (EventArgs)),
            Tuple.Create("MouseUp", typeof (MouseEventArgs)),
        };

        public int GetAffinityForObject(Type type, bool hasEventTarget)
        {
            bool isWinformControl = typeof(Control).IsAssignableFrom(type);

            if (isWinformControl) return 10;

            if (hasEventTarget) return 5;

            return defaultEventsToBind.Any(x =>{
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

            var mi = this.GetType().GetMethods().First(x => x.Name == "BindCommandToObject" && x.IsGenericMethod);
            mi = mi.MakeGenericMethod(eventInfo.Args);

            return (IDisposable)mi.Invoke(this, new[] { command, target, commandParameter, eventInfo.EventInfo.Name });
        }

        public IDisposable BindCommandToObject<TEventArgs>(ICommand command, object target, IObservable<object> commandParameter, string eventName)
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
            ret.Add(evt.Subscribe(ea =>
            {
                if (command.CanExecute(useEventArgsInstead ? ea : latestParameter)){
                    command.Execute(useEventArgsInstead ? ea : latestParameter);
                }
            }));

            return ret;
        }
    }
}