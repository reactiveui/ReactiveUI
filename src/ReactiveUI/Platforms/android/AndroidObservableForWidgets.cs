// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.Frozen;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Runtime.Versioning;
using Android.OS;
using Android.Text;
using Android.Views;
using ReactiveUI.Primitives.Disposables;

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive;
#else
namespace ReactiveUI;
#endif
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
    /// The Android API level (23, Marshmallow) at which <see cref="TimePicker.Hour"/> and
    /// <see cref="TimePicker.Minute"/> were introduced, replacing the obsolete CurrentHour/CurrentMinute members.
    /// </summary>
    private const int TimePickerHourMinuteApiLevel = 23;

    /// <summary>The initial capacity used for the per-property list of candidate widget types.</summary>
    private const int CandidateTypesInitialCapacity = 2;

    /// <summary>Stores observable factory functions keyed by (widget type, property name).</summary>
    /// <remarks>
    /// This table is immutable after type initialization and is safe for concurrent reads.
    /// </remarks>
    private static readonly
        FrozenDictionary<
            (Type ViewType, string PropertyName),
            Func<object, Expression, IObservable<IObservedChange<object, object?>>>> DispatchTable;

    /// <summary>Stores, per property name, the set of widget types that can produce notifications for that property.</summary>
    /// <remarks>
    /// This index supports efficient affinity checks and dispatch selection without scanning the entire dispatch table.
    /// </remarks>
    private static readonly FrozenDictionary<string, Type[]> TypesByPropertyName;

    /// <summary>Initializes static members of the <see cref="AndroidObservableForWidgets"/> class. Initializes the static dispatch tables for the supported Android widgets.</summary>
    /// <remarks>
    /// This constructor runs once and constructs immutable lookup tables for fast concurrent reads.
    /// </remarks>
    [ObsoletedOSPlatform("android23.0")]
    [SupportedOSPlatform("android35.0")]
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
            CreateTimePickerHourFromWidget(), CreateTimePickerMinuteFromWidget(), CreateFromAdapterView()
        };

        Dictionary<(Type ViewType, string PropertyName), Func<object, Expression, IObservable<IObservedChange<object, object?>>>> dispatch =
            new(
                items.Length);

        Dictionary<string, List<Type>> byProperty = new(items.Length, StringComparer.Ordinal);

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
                list = new(CandidateTypesInitialCapacity);
                byProperty.Add(item.Property, list);
            }

            list.Add(item.Type);
        }

        DispatchTable = dispatch.ToFrozenDictionary();

        Dictionary<string, Type[]> index = new(byProperty.Count, StringComparer.Ordinal);
        foreach (var pair in byProperty)
        {
            index[pair.Key] = [.. pair.Value];
        }

        TypesByPropertyName = index.ToFrozenDictionary(StringComparer.Ordinal);
    }

    /// <inheritdoc />
    [RequiresUnreferencedCode("Uses reflection over runtime types which is not trim- or AOT-safe.")]
    public int GetAffinityForObject(Type type, string propertyName) =>
        GetAffinityForObject(type, propertyName, false);

    /// <inheritdoc />
    /// <remarks>
    /// This implementation does not support before-change notifications.
    /// </remarks>
    [RequiresUnreferencedCode("Uses reflection over runtime types which is not trim- or AOT-safe.")]
    public int GetAffinityForObject(Type? type, string propertyName, bool beforeChanged)
    {
        ArgumentExceptionHelper.ThrowIfNull(type);
        ArgumentExceptionHelper.ThrowIfNull(propertyName);

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
                return BindingAffinity.Explicit;
            }
        }

        return 0;
    }

    /// <inheritdoc />
    [RequiresUnreferencedCode("Uses reflection over runtime types which is not trim- or AOT-safe.")]
    public IObservable<IObservedChange<object, object?>> GetNotificationForProperty(
        object sender,
        Expression expression,
        string propertyName) =>
        GetNotificationForProperty(sender, expression, propertyName, false, false);

    /// <inheritdoc />
    [RequiresUnreferencedCode("Uses reflection over runtime types which is not trim- or AOT-safe.")]
    public IObservable<IObservedChange<object, object?>> GetNotificationForProperty(
        object sender,
        Expression expression,
        string propertyName,
        bool beforeChanged) =>
        GetNotificationForProperty(sender, expression, propertyName, beforeChanged, false);

    /// <inheritdoc />
    /// <remarks>
    /// This implementation does not support before-change notifications.
    /// </remarks>
    /// <exception cref="ArgumentNullException">
    /// Thrown when sender, expression, or propertyName is null.
    /// </exception>
    [RequiresUnreferencedCode("Uses reflection over runtime types which is not trim- or AOT-safe.")]
    public IObservable<IObservedChange<object, object?>> GetNotificationForProperty(
        object sender,
        Expression expression,
        string propertyName,
        bool beforeChanged,
        bool suppressWarnings)
    {
        ArgumentExceptionHelper.ThrowIfNull(sender);
        ArgumentExceptionHelper.ThrowIfNull(expression);
        ArgumentExceptionHelper.ThrowIfNull(propertyName);

        if (beforeChanged)
        {
            return ReactiveUI.Primitives.Signals.Signal.Silent<IObservedChange<object, object?>>();
        }

        var senderType = sender.GetType();

        if (!TypesByPropertyName.TryGetValue(propertyName, out var candidates))
        {
            return ReactiveUI.Primitives.Signals.Signal.Silent<IObservedChange<object, object?>>();
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
                : ReactiveUI.Primitives.Signals.Signal.Silent<IObservedChange<object, object?>>();
        }

        return ReactiveUI.Primitives.Signals.Signal.Silent<IObservedChange<object, object?>>();
    }

    /// <summary>Creates a dispatch item for selection changes on <see cref="AdapterView"/> instances.</summary>
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

        return new(
            typeof(AdapterView),
            propName,
            (x, ex) => new AdapterSelectionObservable((AdapterView)x, ex));
    }

    /// <summary>Creates a dispatch item for the <see cref="TimePicker"/> hour property that is compatible with the current OS level.</summary>
    /// <remarks>
    /// Android introduced <see cref="TimePicker.Hour"/> at API level 23. Earlier OS versions use
    /// <see cref="TimePicker.CurrentHour"/>.
    /// </remarks>
    /// <returns>A dispatch item for observing the hour value on a <see cref="TimePicker"/>.</returns>
    [ObsoletedOSPlatform("android23.0")]
    [SupportedOSPlatform("android35.0")]
    private static DispatchItem CreateTimePickerHourFromWidget()
    {
        return (int)Build.VERSION.SdkInt >= TimePickerHourMinuteApiLevel ? CreateFromWidget<TimePicker, TimePicker.TimeChangedEventArgs>(
                static v => v.Hour,
                static (v, h) => v.TimeChanged += h,
                static (v, h) => v.TimeChanged -= h) : CreateFromWidget<TimePicker, TimePicker.TimeChangedEventArgs>(
            static v => v.CurrentHour,
            static (v, h) => v.TimeChanged += h,
            static (v, h) => v.TimeChanged -= h);
    }

    /// <summary>Creates a dispatch item for the <see cref="TimePicker"/> minute property that is compatible with the current OS level.</summary>
    /// <remarks>
    /// Android introduced <see cref="TimePicker.Minute"/> at API level 23. Earlier OS versions use
    /// <see cref="TimePicker.CurrentMinute"/>.
    /// </remarks>
    /// <returns>A dispatch item for observing the minute value on a <see cref="TimePicker"/>.</returns>
    [ObsoletedOSPlatform("android23.0")]
    [SupportedOSPlatform("android35.0")]
    private static DispatchItem CreateTimePickerMinuteFromWidget()
    {
        return (int)Build.VERSION.SdkInt >= TimePickerHourMinuteApiLevel ? CreateFromWidget<TimePicker, TimePicker.TimeChangedEventArgs>(
                static v => v.Minute,
                static (v, h) => v.TimeChanged += h,
                static (v, h) => v.TimeChanged -= h) : CreateFromWidget<TimePicker, TimePicker.TimeChangedEventArgs>(
            static v => v.CurrentMinute,
            static (v, h) => v.TimeChanged += h,
            static (v, h) => v.TimeChanged -= h);
    }

    /// <summary>Creates a dispatch item for a widget type and property by subscribing to a widget event.</summary>
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

        var propName = memberInfo.Name;

        return new(
            typeof(TView),
            propName,
            (x, ex) => new WidgetEventObservable<TView, TEventArgs>((TView)x, ex, addHandler, removeHandler));
    }

    /// <summary>
    /// Bridges an <see cref="AdapterView"/>'s selection events into an observed-change stream — replacing the
    /// <c>FromEvent(ItemSelected).Merge(FromEvent(NothingSelected))</c> chain. Each <see cref="AdapterView.ItemSelected"/>
    /// or <see cref="AdapterView.NothingSelected"/> raise emits an observed change; both handlers are detached on dispose.
    /// </summary>
    /// <param name="adapterView">The adapter view to observe.</param>
    /// <param name="expression">The expression surfaced on the emitted change.</param>
    private sealed class AdapterSelectionObservable(AdapterView adapterView, Expression expression)
        : IObservable<IObservedChange<object, object?>>
    {
        /// <inheritdoc/>
        public IDisposable Subscribe(IObserver<IObservedChange<object, object?>> observer)
        {
            ArgumentExceptionHelper.ThrowIfNull(observer);

            void OnItemSelected(object? sender, AdapterView.ItemSelectedEventArgs args) =>
                observer.OnNext(new ObservedChange<object, object?>(adapterView, expression, null));

            void OnNothingSelected(object? sender, AdapterView.NothingSelectedEventArgs args) =>
                observer.OnNext(new ObservedChange<object, object?>(adapterView, expression, null));

            adapterView.ItemSelected += OnItemSelected;
            adapterView.NothingSelected += OnNothingSelected;

            return new ActionDisposable(() =>
            {
                adapterView.ItemSelected -= OnItemSelected;
                adapterView.NothingSelected -= OnNothingSelected;
            });
        }
    }

    /// <summary>
    /// Bridges a widget's typed CLR event into an observed-change stream — replacing
    /// <c>Observable.FromEvent</c>. Each event raise emits an <see cref="ObservedChange{TSender, TValue}"/> for the
    /// widget; the handler is detached when the subscription is disposed.
    /// </summary>
    /// <typeparam name="TView">The widget type.</typeparam>
    /// <typeparam name="TEventArgs">The widget event's argument type.</typeparam>
    /// <param name="view">The widget instance.</param>
    /// <param name="expression">The expression surfaced on the emitted change.</param>
    /// <param name="addHandler">Attaches an event handler to the widget.</param>
    /// <param name="removeHandler">Detaches an event handler from the widget.</param>
    private sealed class WidgetEventObservable<TView, TEventArgs>(
        TView view,
        Expression expression,
        Action<TView, EventHandler<TEventArgs>> addHandler,
        Action<TView, EventHandler<TEventArgs>> removeHandler)
        : IObservable<IObservedChange<object, object?>>
        where TView : View
        where TEventArgs : EventArgs
    {
        /// <inheritdoc/>
        public IDisposable Subscribe(IObserver<IObservedChange<object, object?>> observer)
        {
            ArgumentExceptionHelper.ThrowIfNull(observer);

            void Handler(object? sender, TEventArgs args) =>
                observer.OnNext(new ObservedChange<object, object?>(view!, expression, null));

            addHandler(view, Handler);
            return new ActionDisposable(() => removeHandler(view, Handler));
        }
    }

    /// <summary>Represents a single dispatch table entry for a widget type and property.</summary>
    private sealed record DispatchItem
    {
        /// <summary>Initializes a new instance of the <see cref="DispatchItem"/> class.</summary>
        /// <param name="type">The widget type for which observation is supported.</param>
        /// <param name="property">The property name that is observable for the widget type.</param>
        /// <param name="func">The observable factory function.</param>
        public DispatchItem(
            Type type,
            string? property,
            Func<object, Expression, IObservable<IObservedChange<object, object?>>> func) =>
            (Type, Property, Func) = (type, property, func);

        /// <summary>Gets the widget type for which observation is supported.</summary>
        public Type Type { get; }

        /// <summary>Gets the property name that is observable for the widget type.</summary>
        public string? Property { get; }

        /// <summary>Gets the observable factory function for the widget type and property.</summary>
        public Func<object, Expression, IObservable<IObservedChange<object, object?>>> Func { get; }
    }
}
