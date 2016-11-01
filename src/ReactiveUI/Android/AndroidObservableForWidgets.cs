using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Linq;
using System.Collections.Generic;
using Android.Views;
using Android.Widget;
using System.Reactive;
using Android.Text;
using Java.Util;
using Observable = System.Reactive.Linq.Observable;

namespace ReactiveUI
{
    /// <summary>
    /// Android view objects are not Generally Observable™, so hard-code some
    /// particularly useful types.
    /// </summary>
    public class AndroidObservableForWidgets : ICreatesObservableForProperty
    {
        static readonly IDictionary<Tuple<Type, string>, Func<object, Expression, IObservable<IObservedChange<object, object>>>> dispatchTable;
        static AndroidObservableForWidgets()
        {
            dispatchTable = new[] { 
                createFromWidget<TextView, TextChangedEventArgs>(v => v.Text, (v, h) => v.TextChanged += h, (v, h) => v.TextChanged -= h),
                createFromWidget<NumberPicker, NumberPicker.ValueChangeEventArgs>(v => v.Value, (v, h) => v.ValueChanged += h, (v, h) => v.ValueChanged -= h),
                createFromWidget<RatingBar, RatingBar.RatingBarChangeEventArgs>(v => v.Rating, (v, h) => v.RatingBarChange += h, (v, h) => v.RatingBarChange -= h),
                createFromWidget<CompoundButton, CompoundButton.CheckedChangeEventArgs>(v => v.Checked, (v, h) => v.CheckedChange += h, (v, h) => v.CheckedChange -= h),
                createFromWidget<CalendarView, CalendarView.DateChangeEventArgs>(v => v.Date, (v, h) => v.DateChange += h, (v, h) => v.DateChange -= h),
                createFromWidget<TabHost, TabHost.TabChangeEventArgs>(v => v.CurrentTab, (v, h) => v.TabChanged += h, (v, h) => v.TabChanged -= h),
                createFromWidget<TimePicker, TimePicker.TimeChangedEventArgs>(v => v.Hour, (v, h) => v.TimeChanged += h, (v, h) => v.TimeChanged -= h),
                createFromWidget<TimePicker, TimePicker.TimeChangedEventArgs>(v => v.Minute, (v, h) => v.TimeChanged += h, (v, h) => v.TimeChanged -= h),
                createFromAdapterView(),
            }.ToDictionary(k => Tuple.Create(k.Type, k.Property), v => v.Func);
        }

        public int GetAffinityForObject(Type type, string propertyName, bool beforeChanged = false)
        {
            if (beforeChanged) return 0;
            return dispatchTable.Keys.Any(x => x.Item1.IsAssignableFrom(type) && x.Item2 == propertyName) ? 5 : 0;
        }

        public IObservable<IObservedChange<object, object>> GetNotificationForProperty(object sender, Expression expression, bool beforeChanged = false)
        {
            var type = sender.GetType();
            var tableItem = dispatchTable.Keys.First(x => x.Item1.IsAssignableFrom(type) && x.Item2 == expression.GetMemberInfo().Name);

            return dispatchTable[tableItem](sender, expression);
        }

        class DispatchTuple
        {
            public Type Type { get; set; }
            public string Property { get; set; }
            public Func<object, Expression, IObservable<IObservedChange<object, object>>> Func { get; set; } 
        }

        static DispatchTuple createFromAdapterView()
        {
            // AdapterView is more complicated because there are two events - one for select and one for deselect
            // respond to both

            const string propName = "SelectedItem";

            return new DispatchTuple
            {
                Type = typeof(AdapterView),
                Property = propName,
                Func = (x, ex) => {
                    var v = (AdapterView)x;

                    return Observable.Merge(
                        Observable.FromEventPattern<AdapterView.ItemSelectedEventArgs>(h => v.ItemSelected += h, h => v.ItemSelected -=h)
                            .Select(_ => new ObservedChange<object, object>(v, ex)),
                        Observable.FromEventPattern<AdapterView.NothingSelectedEventArgs>(h => v.NothingSelected += h, h => v.NothingSelected -= h)
                            .Select(_ => new ObservedChange<object, object>(v, ex))
                    );
                }
            };
        }

        static DispatchTuple createFromWidget<TView, TEventArgs>(Expression<Func<TView, object>> property, Action<TView, EventHandler<TEventArgs>> addHandler, Action<TView, EventHandler<TEventArgs>> removeHandler)
            where TView : View
            where TEventArgs : EventArgs
        {
            // ExpressionToPropertyNames is used here as it handles boxing expressions that might
            // occur due to our use of object
            var propName = property.Body.GetMemberInfo().Name;

            return new DispatchTuple {
                Type = typeof(TView),
                Property = propName,
                Func = (x, ex) => {
                    var v = (TView)x;

                    return Observable.FromEventPattern<TEventArgs>(h => addHandler(v, h) , h => removeHandler(v, h))
                        .Select(_ => new ObservedChange<object, object>(v, ex)); 
                }
            };
        }
    }
}

