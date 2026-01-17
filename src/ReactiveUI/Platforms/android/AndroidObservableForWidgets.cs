// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.Frozen;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Versioning;

using Android.OS;
using Android.Text;
using Android.Views;
using Android.Widget;

using Observable = System.Reactive.Linq.Observable;

namespace ReactiveUI;

/// <summary>
/// Provides property change notifications for a curated set of Android widget types which are not generally observable
/// through standard property change mechanisms.
/// </summary>
/// <remarks>
/// <para>
/// This implementation only supports after-change notifications; beforeChange is not supported.
/// </para>
/// <para>
/// A static dispatch table maps widget types and property names to observable factory functions.
/// </para>
/// <para>
/// Trimming/AOT: this type repeats the trimming/AOT annotations required by the
/// <see cref="ICreatesObservableForProperty"/> interface on its implementing members to satisfy the interface contract.
/// </para>
/// </remarks>
[Preserve(AllMembers = true)]
public sealed class AndroidObservableForWidgets : ICreatesObservableForProperty
{
    /// <summary>
    /// Stores observable factory functions keyed by (widget type, property name).
    /// </summary>
    /// <remarks>
    /// This table is immutable after type initialization and is safe for concurrent reads.
    /// </remarks>
    private static readonly FrozenDictionary<(Type ViewType, string PropertyName), Func<object, Expression, IObservable<IObservedChange<object, object?>>>> DispatchTable;

    /// <summary>
    /// Stores, per property name, the set of widget types that can produce notifications for that property.
    /// </summary>
    /// <remarks>
    /// This index supports efficient affinity checks and dispatch selection without scanning the entire dispatch table.
    /// </remarks>
    private static readonly FrozenDictionary<string, Type[]> TypesByPropertyName;

    /// <summary>
    /// Initializes static members of the <see cref="AndroidObservableForWidgets"/> class.
    /// Initializes the static dispatch tables for the supported Android widgets.
    /// </summary>
    /// <remarks>
    /// This constructor runs once and constructs immutable lookup tables for fast concurrent reads.
    /// </remarks>
    [ObsoletedOSPlatform("android23.0")]
    [SupportedOSPlatform("android23.0")]
    static AndroidObservableForWidgets()
    {
        var items = new[]
        {
            CreateFromWidget<TextView, TextChangedEventArgs>(
                static v => v.Text,
                static (v, h) => v.TextChanged += h,
                static (v, h) => v.TextChanged -= h),

            CreateFromWidget<NumberPicker, NumberPicker.ValueChangeEventArgs>(
                static v => v.Value,
                static (v, h) => v.ValueChanged += h,
                static (v, h) => v.ValueChanged -= h),

            CreateFromWidget<RatingBar, RatingBar.RatingBarChangeEventArgs>(
                static v => v.Rating,
                static (v, h) => v.RatingBarChange += h,
                static (v, h) => v.RatingBarChange -= h),

            CreateFromWidget<CompoundButton, CompoundButton.CheckedChangeEventArgs>(
                static v => v.Checked,
                static (v, h) => v.CheckedChange += h,
                static (v, h) => v.CheckedChange -= h),

            CreateFromWidget<CalendarView, CalendarView.DateChangeEventArgs>(
                static v => v.Date,
                static (v, h) => v.DateChange += h,
                static (v, h) => v.DateChange -= h),

            CreateFromWidget<TabHost, TabHost.TabChangeEventArgs>(
                static v => v.CurrentTab,
                static (v, h) => v.TabChanged += h,
                static (v, h) => v.TabChanged -= h),

            CreateTimePickerHourFromWidget(),
            CreateTimePickerMinuteFromWidget(),
            CreateFromAdapterView(),
        };

        var dispatch =
            new Dictionary<(Type ViewType, string PropertyName), Func<object, Expression, IObservable<IObservedChange<object, object?>>>>(
                capacity: items.Length);

        var byProperty =
            new Dictionary<string, List<Type>>(capacity: items.Length, comparer: StringComparer.Ordinal);

        for (var i = 0; i < items.Length; i++)
        {
            var item = items[i];

            if (item.Property is null)
            {
                continue;
            }

            dispatch[(item.Type, item.Property)] = item.Func;

            if (!byProperty.TryGetValue(item.Property, out var list))
            {
                list = new List<Type>(capacity: 2);
                byProperty.Add(item.Property, list);
            }

            list.Add(item.Type);
        }

        DispatchTable = dispatch.ToFrozenDictionary();

        var index = new Dictionary<string, Type[]>(byProperty.Count, StringComparer.Ordinal);
        foreach (var pair in byProperty)
        {
            index[pair.Key] = pair.Value.ToArray();
        }

        TypesByPropertyName = index.ToFrozenDictionary(StringComparer.Ordinal);
    }

    /// <inheritdoc />
    /// <remarks>
    /// This implementation does not support before-change notifications.
    /// </remarks>
    [RequiresUnreferencedCode("Uses reflection over runtime types which is not trim- or AOT-safe.")]
    public int GetAffinityForObject(Type type, string propertyName, bool beforeChanged = false)
    {
        ArgumentNullException.ThrowIfNull(type);
        ArgumentNullException.ThrowIfNull(propertyName);

        if (beforeChanged)
        {
            return 0;
        }

        if (!TypesByPropertyName.TryGetValue(propertyName, out var candidates))
        {
            return 0;
        }

        for (var i = 0; i < candidates.Length; i++)
        {
            if (candidates[i].IsAssignableFrom(type))
            {
                return 5;
            }
        }

        return 0;
    }

    /// <inheritdoc />
    /// <remarks>
    /// This implementation does not support before-change notifications.
    /// </remarks>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="sender"/>, <paramref name="expression"/>, or <paramref name="propertyName"/> is
    /// <see langword="null"/>.
    /// </exception>
    [RequiresUnreferencedCode("Uses reflection over runtime types which is not trim- or AOT-safe.")]
    public IObservable<IObservedChange<object, object?>> GetNotificationForProperty(
        object sender,
        Expression expression,
        string propertyName,
        bool beforeChanged = false,
        bool suppressWarnings = false)
    {
        ArgumentNullException.ThrowIfNull(sender);
        ArgumentNullException.ThrowIfNull(expression);
        ArgumentNullException.ThrowIfNull(propertyName);

        if (beforeChanged)
        {
            return Observable.Never<IObservedChange<object, object?>>();
        }

        var senderType = sender.GetType();

        if (!TypesByPropertyName.TryGetValue(propertyName, out var candidates))
        {
            return Observable.Never<IObservedChange<object, object?>>();
        }

        for (var i = 0; i < candidates.Length; i++)
        {
            var candidateType = candidates[i];

            if (!candidateType.IsAssignableFrom(senderType))
            {
                continue;
            }

            return DispatchTable.TryGetValue((candidateType, propertyName), out var factory)
                ? factory(sender, expression)
                : Observable.Never<IObservedChange<object, object?>>();
        }

        return Observable.Never<IObservedChange<object, object?>>();
    }

    /// <summary>
    /// Creates a dispatch item for selection changes on <see cref="AdapterView"/> instances.
    /// </summary>
    /// <remarks>
    /// Adapter selection is represented by two distinct events: <see cref="AdapterView.ItemSelected"/> and
    /// <see cref="AdapterView.NothingSelected"/>. This dispatch item merges both into a single observable sequence.
    /// </remarks>
    /// <returns>
    /// A dispatch item mapping <see cref="AdapterView"/> and the <c>SelectedItem</c> property to an observable factory.
    /// </returns>
    private static DispatchItem CreateFromAdapterView()
    {
        const string propName = "SelectedItem";

        return new DispatchItem(
            typeof(AdapterView),
            propName,
            (x, ex) =>
            {
                var adapterView = (AdapterView)x;

                var itemSelected =
                    Observable.FromEvent<EventHandler<AdapterView.ItemSelectedEventArgs>, ObservedChange<object, object?>>(
                        eventHandler =>
                        {
                            void Handler(object? unusedSender, AdapterView.ItemSelectedEventArgs unusedEventArgs) =>
                                eventHandler(new ObservedChange<object, object?>(adapterView, ex, default));

                            return Handler;
                        },
                        h => adapterView.ItemSelected += h,
                        h => adapterView.ItemSelected -= h);

                var nothingSelected =
                    Observable.FromEvent<EventHandler<AdapterView.NothingSelectedEventArgs>, ObservedChange<object, object?>>(
                        eventHandler =>
                        {
                            void Handler(object? unusedSender, AdapterView.NothingSelectedEventArgs unusedEventArgs) =>
                                eventHandler(new ObservedChange<object, object?>(adapterView, ex, default));

                            return Handler;
                        },
                        h => adapterView.NothingSelected += h,
                        h => adapterView.NothingSelected -= h);

                return itemSelected.Merge(nothingSelected);
            });
    }

    /// <summary>
    /// Creates a dispatch item for the <see cref="TimePicker"/> hour property that is compatible with the current OS level.
    /// </summary>
    /// <remarks>
    /// Android introduced <see cref="TimePicker.Hour"/> at API level 23. Earlier OS versions use
    /// <see cref="TimePicker.CurrentHour"/>.
    /// </remarks>
    /// <returns>A dispatch item for observing the hour value on a <see cref="TimePicker"/>.</returns>
    [ObsoletedOSPlatform("android23.0")]
    [SupportedOSPlatform("android23.0")]
    private static DispatchItem CreateTimePickerHourFromWidget()
    {
        if ((int)Build.VERSION.SdkInt >= 23)
        {
            return CreateFromWidget<TimePicker, TimePicker.TimeChangedEventArgs>(
                static v => v.Hour,
                static (v, h) => v.TimeChanged += h,
                static (v, h) => v.TimeChanged -= h);
        }

        return CreateFromWidget<TimePicker, TimePicker.TimeChangedEventArgs>(
            static v => v.CurrentHour,
            static (v, h) => v.TimeChanged += h,
            static (v, h) => v.TimeChanged -= h);
    }

    /// <summary>
    /// Creates a dispatch item for the <see cref="TimePicker"/> minute property that is compatible with the current OS level.
    /// </summary>
    /// <remarks>
    /// Android introduced <see cref="TimePicker.Minute"/> at API level 23. Earlier OS versions use
    /// <see cref="TimePicker.CurrentMinute"/>.
    /// </remarks>
    /// <returns>A dispatch item for observing the minute value on a <see cref="TimePicker"/>.</returns>
    [ObsoletedOSPlatform("android23.0")]
    [SupportedOSPlatform("android23.0")]
    private static DispatchItem CreateTimePickerMinuteFromWidget()
    {
        if ((int)Build.VERSION.SdkInt >= 23)
        {
            return CreateFromWidget<TimePicker, TimePicker.TimeChangedEventArgs>(
                static v => v.Minute,
                static (v, h) => v.TimeChanged += h,
                static (v, h) => v.TimeChanged -= h);
        }

        return CreateFromWidget<TimePicker, TimePicker.TimeChangedEventArgs>(
            static v => v.CurrentMinute,
            static (v, h) => v.TimeChanged += h,
            static (v, h) => v.TimeChanged -= h);
    }

    /// <summary>
    /// Creates a dispatch item for a widget type and property by subscribing to a widget event.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The observable produced by the dispatch item emits a change record when the widget event fires.
    /// </para>
    /// <para>
    /// Trimming/AOT: this helper uses expression inspection to derive the property name. It is preserved and suppressed
    /// to avoid trimming/AOT analysis noise in supported platform builds.
    /// </para>
    /// </remarks>
    /// <typeparam name="TView">The widget type.</typeparam>
    /// <typeparam name="TEventArgs">The event args type for the widget event.</typeparam>
    /// <param name="property">An expression selecting the widget property that is being observed.</param>
    /// <param name="addHandler">Registers the event handler on the widget.</param>
    /// <param name="removeHandler">Unregisters the event handler from the widget.</param>
    /// <returns>A dispatch item for the widget and property.</returns>
    /// <exception cref="ArgumentException">Thrown when <paramref name="property"/> does not expose valid member info.</exception>
    private static DispatchItem CreateFromWidget<TView, TEventArgs>(
        Expression<Func<TView, object?>> property,
        Action<TView, EventHandler<TEventArgs>> addHandler,
        Action<TView, EventHandler<TEventArgs>> removeHandler)
        where TView : View
        where TEventArgs : EventArgs
    {
        var memberInfo =
            property.Body.GetMemberInfo() ??
            throw new ArgumentException("Does not have a valid body member info.", nameof(property));

        // ExpressionToPropertyNames is used here as it handles boxing expressions that might occur due to our use of object.
        var propName = memberInfo.Name;

        return new DispatchItem(
            typeof(TView),
            propName,
            (x, ex) =>
            {
                var view = (TView)x;

                return Observable.FromEvent<EventHandler<TEventArgs>, ObservedChange<object, object?>>(
                    eventHandler =>
                    {
                        void Handler(object? unusedSender, TEventArgs unusedEventArgs) =>
                            eventHandler(new ObservedChange<object, object?>(view, ex, default));

                        return Handler;
                    },
                    h => addHandler(view, h),
                    h => removeHandler(view, h));
            });
    }

    /// <summary>
    /// Represents a single dispatch table entry for a widget type and property.
    /// </summary>
    private sealed record DispatchItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DispatchItem"/> class.
        /// </summary>
        /// <param name="type">The widget type for which observation is supported.</param>
        /// <param name="property">The property name that is observable for the widget type.</param>
        /// <param name="func">The observable factory function.</param>
        public DispatchItem(
            Type type,
            string? property,
            Func<object, Expression, IObservable<IObservedChange<object, object?>>> func) =>
            (Type, Property, Func) = (type, property, func);

        /// <summary>
        /// Gets the widget type for which observation is supported.
        /// </summary>
        public Type Type { get; }

        /// <summary>
        /// Gets the property name that is observable for the widget type.
        /// </summary>
        public string? Property { get; }

        /// <summary>
        /// Gets the observable factory function for the widget type and property.
        /// </summary>
        public Func<object, Expression, IObservable<IObservedChange<object, object?>>> Func { get; }
    }
}
