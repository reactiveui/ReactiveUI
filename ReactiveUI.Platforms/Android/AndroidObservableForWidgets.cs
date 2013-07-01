using System;
using System.Linq;
using System.Reactive.Linq;
using System.Collections.Generic;
using Android.Widget;
using System.Reactive;
using Android.Text;

namespace ReactiveUI.Android
{
    /// <summary>
    /// Android view objects are not Generally Observable™, so hard-code some
    /// particularly useful types.
    /// </summary>
    public class AndroidObservableForWidgets : ICreatesObservableForProperty
    {
        static readonly IDictionary<Tuple<Type, string>, Func<object, IObservable<IObservedChange<object, object>>>> dispatchTable;
        static AndroidObservableForWidgets()
        {
            dispatchTable = new[] { 
                new {
                    Type = typeof(TextView),
                    Property = "Text",
                    Func = new Func<object, IObservable<IObservedChange<object, object>>>(x => {
                        var w = (TextView)x;
                        var getter = Reflection.GetValueFetcherOrThrow(typeof(TextView), "Text");

                        return Observable.FromEventPattern<TextChangedEventArgs>(h => w.TextChanged += h, h => w.TextChanged -= h)
                            .Select(_ => new ObservedChange<object, object>() { Sender = w, PropertyName = "Text", Value = getter(w) });
                    }),
                }
            }.ToDictionary(k => Tuple.Create(k.Type, k.Property), v => v.Func);
        }

        public int GetAffinityForObject(Type type, string propertyName, bool beforeChanged = false)
        {
            if (beforeChanged) return 0;
            return dispatchTable.Keys.Any(x => x.Item1.IsAssignableFrom(type) && x.Item2 == propertyName) ? 5 : 0;
        }

        public IObservable<IObservedChange<object, object>> GetNotificationForProperty(object sender, string propertyName, bool beforeChanged = false)
        {
            var type = sender.GetType();
            var tableItem = dispatchTable.Keys.First(x => x.Item1.IsAssignableFrom(type) && x.Item2 == propertyName);
            return dispatchTable[tableItem](sender);
        }
    }
}

