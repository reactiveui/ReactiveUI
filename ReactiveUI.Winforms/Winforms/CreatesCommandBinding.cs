using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Forms;
using System.Windows.Input;
using ReactiveUI;
using System.Reflection;
using System.ComponentModel;

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

            if (hasEventTarget) return 6;

            return defaultEventsToBind.Any(x => {
                var ei = type.GetEvent(x.Item1, BindingFlags.Public | BindingFlags.FlattenHierarchy | BindingFlags.Instance);
                return ei != null;
            }) ? 4 : 0;
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
            Type targetType = target.GetType();

            ret.Add(commandParameter.Subscribe(x => latestParameter = x));

            var evt = Observable.FromEventPattern<TEventArgs>(target, eventName);
            ret.Add(evt.Subscribe(ea => {
                if (command.CanExecute(latestParameter)) {
                    command.Execute(latestParameter);
                }
            }));

            // We initially only accepted Controls here, but this is too restrictive:
            // there are a number of Components that can trigger Commands and also
            // have an Enabled property, just like Controls.
            // For example: System.Windows.Forms.ToolStripButton.
            if (typeof(Component).IsAssignableFrom(targetType)) {
                PropertyInfo enabledProperty = targetType.GetRuntimeProperty("Enabled");

                if (enabledProperty != null) {
                    object latestParam = null;
                    ret.Add(commandParameter.Subscribe(x => latestParam = x));

                    ret.Add(Observable.FromEventPattern<EventHandler, EventArgs>(x => command.CanExecuteChanged += x, x => command.CanExecuteChanged -= x)
                        .Select(_ => command.CanExecute(latestParam))
                        .StartWith(command.CanExecute(latestParam))
                        .Subscribe(x => {
                            enabledProperty.SetValue(target, x, null);
                        }));
                }
            }

            return ret;
        }
    }
}
