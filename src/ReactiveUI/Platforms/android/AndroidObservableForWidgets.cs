// Copyright (c) 2019 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Linq;
using Android.OS;
using Android.Text;
using Android.Views;
using Android.Widget;
using Observable = System.Reactive.Linq.Observable;

namespace ReactiveUI
{
    /// <summary>
    /// Android view objects are not Generally Observable™, so hard-code some
    /// particularly useful types.
    /// </summary>
    public class AndroidObservableForWidgets : ICreatesObservableForProperty
    {
        private static readonly IDictionary<(Type viewType, string propertyName), Func<object, Expression, IObservable<IObservedChange<object, object>>>> dispatchTable;

        static AndroidObservableForWidgets()
        {
            dispatchTable = new[]
            {
                CreateFromWidget<TextView, TextChangedEventArgs>(v => v.Text, (v, h) => v.TextChanged += h, (v, h) => v.TextChanged -= h),
                CreateFromWidget<NumberPicker, NumberPicker.ValueChangeEventArgs>(v => v.Value, (v, h) => v.ValueChanged += h, (v, h) => v.ValueChanged -= h),
                CreateFromWidget<RatingBar, RatingBar.RatingBarChangeEventArgs>(v => v.Rating, (v, h) => v.RatingBarChange += h, (v, h) => v.RatingBarChange -= h),
                CreateFromWidget<CompoundButton, CompoundButton.CheckedChangeEventArgs>(v => v.Checked, (v, h) => v.CheckedChange += h, (v, h) => v.CheckedChange -= h),
                CreateFromWidget<CalendarView, CalendarView.DateChangeEventArgs>(v => v.Date, (v, h) => v.DateChange += h, (v, h) => v.DateChange -= h),
                CreateFromWidget<TabHost, TabHost.TabChangeEventArgs>(v => v.CurrentTab, (v, h) => v.TabChanged += h, (v, h) => v.TabChanged -= h),
                CreateTimePickerHourFromWidget(),
                CreateTimePickerMinuteFromWidget(),
                CreateFromAdapterView(),
            }.ToDictionary(k => (viewType: k.Type, propertyName: k.Property), v => v.Func);
        }

        /// <inheritdoc/>
        public int GetAffinityForObject(Type type, string propertyName, bool beforeChanged = false)
        {
            if (beforeChanged)
            {
                return 0;
            }

            return dispatchTable.Keys.Any(x => x.viewType.IsAssignableFrom(type) && x.propertyName == propertyName) ? 5 : 0;
        }

        /// <inheritdoc/>
        public IObservable<IObservedChange<object, object>> GetNotificationForProperty(object sender, Expression expression, string propertyName, bool beforeChanged = false, bool suppressWarnings = false)
        {
            if (sender == null)
            {
                throw new ArgumentNullException(nameof(sender));
            }

            var type = sender.GetType();
            var tableItem = dispatchTable.Keys.First(x => x.viewType.IsAssignableFrom(type) && x.propertyName == propertyName);

            return dispatchTable[tableItem](sender, expression);
        }

        private static DispatchItem CreateFromAdapterView()
        {
            // AdapterView is more complicated because there are two events - one for select and one for deselect
            // respond to both
            const string propName = "SelectedItem";

            return new DispatchItem
            {
                Type = typeof(AdapterView),
                Property = propName,
                Func = (x, ex) =>
                {
                    var adapterView = (AdapterView)x;

                    var itemSelected = Observable.FromEvent<EventHandler<AdapterView.ItemSelectedEventArgs>, ObservedChange<object, object>>(
                        eventHandler =>
                        {
                            void Handler(object sender, AdapterView.ItemSelectedEventArgs e) => eventHandler(new ObservedChange<object, object>(adapterView, ex));
                            return Handler;
                        },
                        h => adapterView.ItemSelected += h,
                        h => adapterView.ItemSelected -= h);

                    var nothingSelected = Observable.FromEvent<EventHandler<AdapterView.NothingSelectedEventArgs>, ObservedChange<object, object>>(
                        eventHandler =>
                        {
                            void Handler(object sender, AdapterView.NothingSelectedEventArgs e) => eventHandler(new ObservedChange<object, object>(adapterView, ex));
                            return Handler;
                        },
                        h => adapterView.NothingSelected += h,
                        h => adapterView.NothingSelected -= h);

                    return Observable.Merge(itemSelected, nothingSelected);
                }
            };
        }

        private static DispatchItem CreateTimePickerHourFromWidget()
        {
            if ((int)Build.VERSION.SdkInt >= 23)
            {
                return CreateFromWidget<TimePicker, TimePicker.TimeChangedEventArgs>(v => v.Hour, (v, h) => v.TimeChanged += h, (v, h) => v.TimeChanged -= h);
            }

#pragma warning disable 618
            return CreateFromWidget<TimePicker, TimePicker.TimeChangedEventArgs>(v => v.CurrentHour, (v, h) => v.TimeChanged += h, (v, h) => v.TimeChanged -= h);
#pragma warning restore 618
        }

        private static DispatchItem CreateTimePickerMinuteFromWidget()
        {
            if ((int)Build.VERSION.SdkInt >= 23)
            {
                return CreateFromWidget<TimePicker, TimePicker.TimeChangedEventArgs>(v => v.Minute, (v, h) => v.TimeChanged += h, (v, h) => v.TimeChanged -= h);
            }

#pragma warning disable 618
            return CreateFromWidget<TimePicker, TimePicker.TimeChangedEventArgs>(v => v.CurrentMinute, (v, h) => v.TimeChanged += h, (v, h) => v.TimeChanged -= h);
#pragma warning restore 618
        }

        private static DispatchItem CreateFromWidget<TView, TEventArgs>(Expression<Func<TView, object>> property, Action<TView, EventHandler<TEventArgs>> addHandler, Action<TView, EventHandler<TEventArgs>> removeHandler)
            where TView : View
            where TEventArgs : EventArgs
        {
            // ExpressionToPropertyNames is used here as it handles boxing expressions that might
            // occur due to our use of object
            var propName = property.Body.GetMemberInfo().Name;

            return new DispatchItem
            {
                Type = typeof(TView),
                Property = propName,
                Func = (x, ex) =>
                {
                    var view = (TView)x;

                    return Observable.FromEvent<EventHandler<TEventArgs>, ObservedChange<object, object>>(
                        eventHandler =>
                        {
                            void Handler(object sender, TEventArgs e) => eventHandler(new ObservedChange<object, object>(view, ex));
                            return Handler;
                        },
                        h => addHandler(view, h),
                        h => removeHandler(view, h));
                }
            };
        }

        private class DispatchItem
        {
            public Type Type { get; set; }

            public string Property { get; set; }

            public Func<object, Expression, IObservable<IObservedChange<object, object>>> Func { get; set; }
        }
    }
}
