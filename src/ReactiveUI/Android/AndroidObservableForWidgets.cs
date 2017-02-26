using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Linq;
using Android.Text;
using Android.Views;
using Android.Widget;
using Observable = System.Reactive.Linq.Observable;

namespace ReactiveUI
{
    /// <summary>
    /// Android view objects are not Generally Observable™, so hard-code some particularly useful types.
    /// </summary>
    public class AndroidObservableForWidgets : ICreatesObservableForProperty
    {
        private static readonly IDictionary<Tuple<Type, string>, Func<object, Expression, IObservable<IObservedChange<object, object>>>> dispatchTable;

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

        /// <summary>
        /// Returns a positive integer when this class supports GetNotificationForProperty for this
        /// particular Type. If the method isn't supported at all, return a non-positive integer.
        /// When multiple implementations return a positive value, the host will use the one which
        /// returns the highest value. When in doubt, return '2' or '0'
        /// </summary>
        /// <param name="type">The type to query for.</param>
        /// <param name="propertyName"></param>
        /// <param name="beforeChanged"></param>
        /// <returns>A positive integer if GNFP is supported, zero or a negative value otherwise</returns>
        public int GetAffinityForObject(Type type, string propertyName, bool beforeChanged = false)
        {
            if (beforeChanged) return 0;
            return dispatchTable.Keys.Any(x => x.Item1.IsAssignableFrom(type) && x.Item2 == propertyName) ? 5 : 0;
        }

        /// <summary>
        /// Subscribe to notifications on the specified property, given an object and a property name.
        /// </summary>
        /// <param name="sender">The object to observe.</param>
        /// <param name="expression">
        /// The expression on the object to observe. This will be either a MemberExpression or an
        /// IndexExpression dependending on the property.
        /// </param>
        /// <param name="beforeChanged">
        /// If true, signal just before the property value actually changes. If false, signal after
        /// the property changes.
        /// </param>
        /// <returns>
        /// An IObservable which is signalled whenever the specified property on the object changes.
        /// If this cannot be done for a specified value of beforeChanged, return Observable.Never
        /// </returns>
        public IObservable<IObservedChange<object, object>> GetNotificationForProperty(object sender, Expression expression, bool beforeChanged = false)
        {
            var type = sender.GetType();
            var tableItem = dispatchTable.Keys.First(x => x.Item1.IsAssignableFrom(type) && x.Item2 == expression.GetMemberInfo().Name);

            return dispatchTable[tableItem](sender, expression);
        }

        private static DispatchTuple createFromAdapterView()
        {
            // AdapterView is more complicated because there are two events - one for select and one
            // for deselect respond to both

            const string propName = "SelectedItem";

            return new DispatchTuple
            {
                Type = typeof(AdapterView),
                Property = propName,
                Func = (x, ex) => {
                    var v = (AdapterView)x;

                    return Observable.Merge(
                        Observable.FromEventPattern<AdapterView.ItemSelectedEventArgs>(h => v.ItemSelected += h, h => v.ItemSelected -= h)
                            .Select(_ => new ObservedChange<object, object>(v, ex)),
                        Observable.FromEventPattern<AdapterView.NothingSelectedEventArgs>(h => v.NothingSelected += h, h => v.NothingSelected -= h)
                            .Select(_ => new ObservedChange<object, object>(v, ex))
                    );
                }
            };
        }

        private static DispatchTuple createFromWidget<TView, TEventArgs>(Expression<Func<TView, object>> property, Action<TView, EventHandler<TEventArgs>> addHandler, Action<TView, EventHandler<TEventArgs>> removeHandler)
            where TView : View
            where TEventArgs : EventArgs
        {
            // ExpressionToPropertyNames is used here as it handles boxing expressions that might
            // occur due to our use of object
            var propName = property.Body.GetMemberInfo().Name;

            return new DispatchTuple
            {
                Type = typeof(TView),
                Property = propName,
                Func = (x, ex) => {
                    var v = (TView)x;

                    return Observable.FromEventPattern<TEventArgs>(h => addHandler(v, h), h => removeHandler(v, h))
                        .Select(_ => new ObservedChange<object, object>(v, ex));
                }
            };
        }

        private class DispatchTuple
        {
            public Func<object, Expression, IObservable<IObservedChange<object, object>>> Func { get; set; }

            public string Property { get; set; }

            public Type Type { get; set; }
        }
    }
}