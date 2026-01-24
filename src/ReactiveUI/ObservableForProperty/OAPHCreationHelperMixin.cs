// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI;

/// <summary>
/// Provides extension methods for converting observables to ObservableAsPropertyHelper instances, enabling property
/// change notifications in reactive objects.
/// </summary>
/// <remarks>These helper methods simplify the process of binding observable sequences to properties on objects
/// implementing IReactiveObject, such as those in the ReactiveUI framework. They support both expression-based and
/// string-based property identification, allow for optional initial values, and provide control over subscription
/// timing and notification scheduling. Use these methods to implement read-only reactive properties that automatically
/// notify listeners when their values change.</remarks>
public static class OAPHCreationHelperMixin
{
    /// <summary>
    /// Converts an Observable to an ObservableAsPropertyHelper and
    /// automatically provides the onChanged method to raise the property
    /// changed notification.
    /// </summary>
    /// <typeparam name="TObj">The object type.</typeparam>
    /// <typeparam name="TRet">The result type.</typeparam>
    /// <param name="target">
    /// The observable to convert to an ObservableAsPropertyHelper.
    /// </param>
    /// <param name="source">
    /// The ReactiveObject that has the property.
    /// </param>
    /// <param name="property">
    /// An Expression representing the property (i.e. <c>x => x.SomeProperty</c>).
    /// </param>
    /// <param name="deferSubscription">
    /// A value indicating whether the <see cref="ObservableAsPropertyHelper{T}"/>
    /// should defer the subscription to the <paramref name="target"/> source
    /// until the first call to <see cref="ObservableAsPropertyHelper{T}.Value"/>,
    /// or if it should immediately subscribe to the <paramref name="target"/> source.
    /// </param>
    /// <param name="scheduler">
    /// The scheduler that the notifications will be provided on - this should normally
    /// be a Dispatcher-based scheduler.
    /// </param>
    /// <returns>
    /// An initialized ObservableAsPropertyHelper; use this as the backing field
    /// for your property.
    /// </returns>
    public static ObservableAsPropertyHelper<TRet> ToProperty<TObj, TRet>(
        this IObservable<TRet> target,
        TObj source,
        Expression<Func<TObj, TRet>> property,
        bool deferSubscription = false,
        IScheduler? scheduler = null) // TODO: Create Test
        where TObj : class, IReactiveObject
    {
        ArgumentExceptionHelper.ThrowIfNull(target);
        ArgumentExceptionHelper.ThrowIfNull(source);
        ArgumentExceptionHelper.ThrowIfNull(property);

        return source.ObservableToProperty(target, property, deferSubscription, scheduler);
    }

    /// <summary>
    /// Converts an Observable to an ObservableAsPropertyHelper and
    /// automatically provides the onChanged method to raise the property
    /// changed notification.
    /// </summary>
    /// <typeparam name="TObj">The object type.</typeparam>
    /// <typeparam name="TRet">The result type.</typeparam>
    /// <param name="target">
    /// The observable to convert to an ObservableAsPropertyHelper.
    /// </param>
    /// <param name="source">
    /// The ReactiveObject that has the property.
    /// </param>
    /// <param name="property">
    /// An Expression representing the property (i.e. <c>x => x.SomeProperty</c>).
    /// </param>
    /// <param name="initialValue">
    /// The initial value of the property.
    /// </param>
    /// <param name="deferSubscription">
    /// A value indicating whether the <see cref="ObservableAsPropertyHelper{T}"/>
    /// should defer the subscription to the <paramref name="target"/> source
    /// until the first call to <see cref="ObservableAsPropertyHelper{T}.Value"/>,
    /// or if it should immediately subscribe to the <paramref name="target"/> source.
    /// </param>
    /// <param name="scheduler">
    /// The scheduler that the notifications will be provided on - this should normally
    /// be a Dispatcher-based scheduler.
    /// </param>
    /// <returns>
    /// An initialized ObservableAsPropertyHelper; use this as the backing field
    /// for your property.
    /// </returns>
    public static ObservableAsPropertyHelper<TRet> ToProperty<TObj, TRet>(
        this IObservable<TRet> target,
        TObj source,
        Expression<Func<TObj, TRet>> property,
        TRet initialValue,
        bool deferSubscription = false,
        IScheduler? scheduler = null) // TODO: Create Test
        where TObj : class, IReactiveObject
        => ToProperty(target, source, property, () => initialValue, deferSubscription, scheduler);

    /// <summary>
    /// Converts an Observable to an ObservableAsPropertyHelper and
    /// automatically provides the onChanged method to raise the property
    /// changed notification.
    /// </summary>
    /// <typeparam name="TObj">The object type.</typeparam>
    /// <typeparam name="TRet">The result type.</typeparam>
    /// <param name="target">
    /// The observable to convert to an ObservableAsPropertyHelper.
    /// </param>
    /// <param name="source">
    /// The ReactiveObject that has the property.
    /// </param>
    /// <param name="property">
    /// An Expression representing the property (i.e. <c>x => x.SomeProperty</c>).
    /// </param>
    /// <param name="getInitialValue">
    /// The function used to retrieve the initial value of the property.
    /// </param>
    /// <param name="deferSubscription">
    /// A value indicating whether the <see cref="ObservableAsPropertyHelper{T}"/>
    /// should defer the subscription to the <paramref name="target"/> source
    /// until the first call to <see cref="ObservableAsPropertyHelper{T}.Value"/>,
    /// or if it should immediately subscribe to the <paramref name="target"/> source.
    /// </param>
    /// <param name="scheduler">
    /// The scheduler that the notifications will be provided on - this should normally
    /// be a Dispatcher-based scheduler.
    /// </param>
    /// <returns>
    /// An initialized ObservableAsPropertyHelper; use this as the backing field
    /// for your property.
    /// </returns>
    public static ObservableAsPropertyHelper<TRet> ToProperty<TObj, TRet>(
        this IObservable<TRet> target,
        TObj source,
        Expression<Func<TObj, TRet>> property,
        Func<TRet> getInitialValue,
        bool deferSubscription = false,
        IScheduler? scheduler = null) // TODO: Create Test
        where TObj : class, IReactiveObject
    {
        ArgumentExceptionHelper.ThrowIfNull(property);
        return source.ObservableToProperty(target, property, getInitialValue, deferSubscription, scheduler);
    }

    /// <summary>
    /// Converts an Observable to an ObservableAsPropertyHelper and
    /// automatically provides the onChanged method to raise the property
    /// changed notification.
    /// </summary>
    /// <typeparam name="TObj">The object type.</typeparam>
    /// <typeparam name="TRet">The result type.</typeparam>
    /// <param name="target">
    /// The observable to convert to an ObservableAsPropertyHelper.
    /// </param>
    /// <param name="source">
    /// The ReactiveObject that has the property.
    /// </param>
    /// <param name="property">
    /// An Expression representing the property (i.e. <c>x => x.SomeProperty</c>).
    /// </param>
    /// <param name="result">
    /// An out param matching the return value, provided for convenience.
    /// </param>
    /// <param name="deferSubscription">
    /// A value indicating whether the <see cref="ObservableAsPropertyHelper{T}"/>
    /// should defer the subscription to the <paramref name="target"/> source
    /// until the first call to <see cref="ObservableAsPropertyHelper{T}.Value"/>,
    /// or if it should immediately subscribe to the <paramref name="target"/> source.
    /// </param>
    /// <param name="scheduler">
    /// The scheduler that the notifications will be provided on - this should
    /// normally be a Dispatcher-based scheduler.
    /// </param>
    /// <returns>
    /// An initialized ObservableAsPropertyHelper; use this as the backing
    /// field for your property.
    /// </returns>
    public static ObservableAsPropertyHelper<TRet> ToProperty<TObj, TRet>(
        this IObservable<TRet> target,
        TObj source,
        Expression<Func<TObj, TRet>> property,
        out ObservableAsPropertyHelper<TRet> result,
        bool deferSubscription = false,
        IScheduler? scheduler = null) // TODO: Create Test
        where TObj : class, IReactiveObject
    {
        ArgumentExceptionHelper.ThrowIfNull(target);
        ArgumentExceptionHelper.ThrowIfNull(source);
        ArgumentExceptionHelper.ThrowIfNull(property);

        var ret = source.ObservableToProperty(target, property, deferSubscription, scheduler);

        result = ret;

        return ret;
    }

    /// <summary>
    /// Converts an Observable to an ObservableAsPropertyHelper and
    /// automatically provides the onChanged method to raise the property
    /// changed notification.
    /// </summary>
    /// <typeparam name="TObj">The object type.</typeparam>
    /// <typeparam name="TRet">The result type.</typeparam>
    /// <param name="target">
    /// The observable to convert to an ObservableAsPropertyHelper.
    /// </param>
    /// <param name="source">
    /// The ReactiveObject that has the property.
    /// </param>
    /// <param name="property">
    /// An Expression representing the property (i.e. <c>x => x.SomeProperty</c>).
    /// </param>
    /// <param name="result">
    /// An out param matching the return value, provided for convenience.
    /// </param>
    /// <param name="initialValue">
    /// The initial value of the property.
    /// </param>
    /// <param name="deferSubscription">
    /// A value indicating whether the <see cref="ObservableAsPropertyHelper{T}"/>
    /// should defer the subscription to the <paramref name="target"/> source
    /// until the first call to <see cref="ObservableAsPropertyHelper{T}.Value"/>,
    /// or if it should immediately subscribe to the <paramref name="target"/> source.
    /// </param>
    /// <param name="scheduler">
    /// The scheduler that the notifications will be provided on - this should
    /// normally be a Dispatcher-based scheduler.
    /// </param>
    /// <returns>
    /// An initialized ObservableAsPropertyHelper; use this as the backing
    /// field for your property.
    /// </returns>
    public static ObservableAsPropertyHelper<TRet> ToProperty<TObj, TRet>(
        this IObservable<TRet> target,
        TObj source,
        Expression<Func<TObj, TRet>> property,
        out ObservableAsPropertyHelper<TRet> result,
        TRet initialValue,
        bool deferSubscription = false,
        IScheduler? scheduler = null) // TODO: Create Test
        where TObj : class, IReactiveObject
        => ToProperty(target, source, property, out result, () => initialValue, deferSubscription, scheduler);

    /// <summary>
    /// Converts an Observable to an ObservableAsPropertyHelper and
    /// automatically provides the onChanged method to raise the property
    /// changed notification.
    /// </summary>
    /// <typeparam name="TObj">The object type.</typeparam>
    /// <typeparam name="TRet">The result type.</typeparam>
    /// <param name="target">
    /// The observable to convert to an ObservableAsPropertyHelper.
    /// </param>
    /// <param name="source">
    /// The ReactiveObject that has the property.
    /// </param>
    /// <param name="property">
    /// An Expression representing the property (i.e. <c>x => x.SomeProperty</c>).
    /// </param>
    /// <param name="result">
    /// An out param matching the return value, provided for convenience.
    /// </param>
    /// <param name="getInitialValue">
    /// The function used to retrieve the initial value of the property.
    /// </param>
    /// <param name="deferSubscription">
    /// A value indicating whether the <see cref="ObservableAsPropertyHelper{T}"/>
    /// should defer the subscription to the <paramref name="target"/> source
    /// until the first call to <see cref="ObservableAsPropertyHelper{T}.Value"/>,
    /// or if it should immediately subscribe to the <paramref name="target"/> source.
    /// </param>
    /// <param name="scheduler">
    /// The scheduler that the notifications will be provided on - this should
    /// normally be a Dispatcher-based scheduler.
    /// </param>
    /// <returns>
    /// An initialized ObservableAsPropertyHelper; use this as the backing
    /// field for your property.
    /// </returns>
    public static ObservableAsPropertyHelper<TRet> ToProperty<TObj, TRet>(
        this IObservable<TRet> target,
        TObj source,
        Expression<Func<TObj, TRet>> property,
        out ObservableAsPropertyHelper<TRet> result,
        Func<TRet> getInitialValue,
        bool deferSubscription = false,
        IScheduler? scheduler = null) // TODO: Create Test
        where TObj : class, IReactiveObject
    {
        ArgumentExceptionHelper.ThrowIfNull(property);
        var ret = source.ObservableToProperty(target, property, getInitialValue, deferSubscription, scheduler);

        result = ret;
        return ret;
    }

    /// <summary>
    /// Converts an Observable to an ObservableAsPropertyHelper and
    /// automatically provides the onChanged method to raise the property
    /// changed notification.
    /// </summary>
    /// <typeparam name="TObj">The object type.</typeparam>
    /// <typeparam name="TRet">The result type.</typeparam>
    /// <param name="target">
    /// The observable to convert to an ObservableAsPropertyHelper.
    /// </param>
    /// <param name="source">
    /// The ReactiveObject that has the property.
    /// </param>
    /// <param name="property">
    /// The name of the property that has changed. Recommended for use with nameof() or a FODY.
    /// or a FODY.
    /// </param>
    /// <param name="initialValue">
    /// The initial value of the property.
    /// </param>
    /// <param name="deferSubscription">
    /// A value indicating whether the <see cref="ObservableAsPropertyHelper{T}"/>
    /// should defer the subscription to the <paramref name="target"/> source
    /// until the first call to <see cref="ObservableAsPropertyHelper{T}.Value"/>,
    /// or if it should immediately subscribe to the <paramref name="target"/> source.
    /// </param>
    /// <param name="scheduler">
    /// The scheduler that the notifications will be provided on - this should
    /// normally be a Dispatcher-based scheduler.
    /// </param>
    /// <returns>
    /// An initialized ObservableAsPropertyHelper; use this as the backing field
    /// for your property.
    /// </returns>
    public static ObservableAsPropertyHelper<TRet> ToProperty<TObj, TRet>(
        this IObservable<TRet> target,
        TObj source,
        string property,
        TRet initialValue,
        bool deferSubscription = false,
        IScheduler? scheduler = null) // TODO: Create Test
        where TObj : class, IReactiveObject
        => ToProperty(target, source, property, () => initialValue, deferSubscription, scheduler);

    /// <summary>
    /// Converts an Observable to an ObservableAsPropertyHelper and
    /// automatically provides the onChanged method to raise the property
    /// changed notification.
    /// </summary>
    /// <typeparam name="TObj">The object type.</typeparam>
    /// <typeparam name="TRet">The result type.</typeparam>
    /// <param name="target">
    /// The observable to convert to an ObservableAsPropertyHelper.
    /// </param>
    /// <param name="source">
    /// The ReactiveObject that has the property.
    /// </param>
    /// <param name="property">
    /// The name of the property that has changed. Recommended for use with nameof() or a FODY.
    /// or a FODY.
    /// </param>
    /// <param name="deferSubscription">
    /// A value indicating whether the <see cref="ObservableAsPropertyHelper{T}"/>
    /// should defer the subscription to the <paramref name="target"/> source
    /// until the first call to <see cref="ObservableAsPropertyHelper{T}.Value"/>,
    /// or if it should immediately subscribe to the <paramref name="target"/> source.
    /// </param>
    /// <param name="scheduler">
    /// The scheduler that the notifications will be provided on - this should
    /// normally be a Dispatcher-based scheduler.
    /// </param>
    /// <returns>
    /// An initialized ObservableAsPropertyHelper; use this as the backing field
    /// for your property.
    /// </returns>
    public static ObservableAsPropertyHelper<TRet> ToProperty<TObj, TRet>(
        this IObservable<TRet> target,
        TObj source,
        string property,
        bool deferSubscription = false,
        IScheduler? scheduler = null) // TODO: Create Test
        where TObj : class, IReactiveObject
    {
        ArgumentExceptionHelper.ThrowIfNull(target);
        ArgumentExceptionHelper.ThrowIfNull(source);

        ArgumentExceptionHelper.ThrowIfNullOrWhiteSpace(property);

        return source.ObservableToProperty(target, property, deferSubscription, scheduler);
    }

    /// <summary>
    /// Converts an Observable to an ObservableAsPropertyHelper and
    /// automatically provides the onChanged method to raise the property
    /// changed notification.
    /// </summary>
    /// <typeparam name="TObj">The object type.</typeparam>
    /// <typeparam name="TRet">The result type.</typeparam>
    /// <param name="target">
    /// The observable to convert to an ObservableAsPropertyHelper.
    /// </param>
    /// <param name="source">
    /// The ReactiveObject that has the property.
    /// </param>
    /// <param name="property">
    /// The name of the property that has changed. Recommended for use with nameof() or a FODY.
    /// or a FODY.
    /// </param>
    /// <param name="getInitialValue">
    /// The function used to retrieve the initial value of the property.
    /// </param>
    /// <param name="deferSubscription">
    /// A value indicating whether the <see cref="ObservableAsPropertyHelper{T}"/>
    /// should defer the subscription to the <paramref name="target"/> source
    /// until the first call to <see cref="ObservableAsPropertyHelper{T}.Value"/>,
    /// or if it should immediately subscribe to the <paramref name="target"/> source.
    /// </param>
    /// <param name="scheduler">
    /// The scheduler that the notifications will be provided on - this should
    /// normally be a Dispatcher-based scheduler.
    /// </param>
    /// <returns>
    /// An initialized ObservableAsPropertyHelper; use this as the backing field
    /// for your property.
    /// </returns>
    public static ObservableAsPropertyHelper<TRet> ToProperty<TObj, TRet>(
        this IObservable<TRet> target,
        TObj source,
        string property,
        Func<TRet> getInitialValue,
        bool deferSubscription = false,
        IScheduler? scheduler = null) // TODO: Create Test
        where TObj : class, IReactiveObject
    {
        ArgumentExceptionHelper.ThrowIfNull(target);
        ArgumentExceptionHelper.ThrowIfNull(source);

        ArgumentExceptionHelper.ThrowIfNullOrWhiteSpace(property);

        return source.ObservableToProperty(target, property, getInitialValue, deferSubscription, scheduler);
    }

    /// <summary>
    /// Converts an Observable to an ObservableAsPropertyHelper and
    /// automatically provides the onChanged method to raise the property
    /// changed notification.
    /// </summary>
    /// <typeparam name="TObj">The object type.</typeparam>
    /// <typeparam name="TRet">The result type.</typeparam>
    /// <param name="target">
    /// The observable to convert to an ObservableAsPropertyHelper.
    /// </param>
    /// <param name="source">
    /// The ReactiveObject that has the property.
    /// </param>
    /// <param name="property">
    /// The name of the property that has changed. Recommended for use with nameof() or a FODY.
    /// </param>
    /// <param name="result">
    /// An out param matching the return value, provided for convenience.
    /// </param>
    /// <param name="deferSubscription">
    /// A value indicating whether the <see cref="ObservableAsPropertyHelper{T}"/>
    /// should defer the subscription to the <paramref name="target"/> source
    /// until the first call to <see cref="ObservableAsPropertyHelper{T}.Value"/>,
    /// or if it should immediately subscribe to the <paramref name="target"/> source.
    /// </param>
    /// <param name="scheduler">
    /// The scheduler that the notifications will be provided on - this should
    /// normally be a Dispatcher-based scheduler.
    /// </param>
    /// <returns>
    /// An initialized ObservableAsPropertyHelper; use this as the backing
    /// field for your property.
    /// </returns>
    public static ObservableAsPropertyHelper<TRet> ToProperty<TObj, TRet>(
        this IObservable<TRet> target,
        TObj source,
        string property,
        out ObservableAsPropertyHelper<TRet> result,
        bool deferSubscription = false,
        IScheduler? scheduler = null) // TODO: Create Test
        where TObj : class, IReactiveObject
    {
        ArgumentExceptionHelper.ThrowIfNull(target);
        ArgumentExceptionHelper.ThrowIfNull(source);

        ArgumentExceptionHelper.ThrowIfNullOrWhiteSpace(property);

        result = source.ObservableToProperty(target, property, deferSubscription, scheduler);

        return result;
    }

    /// <summary>
    /// Converts an Observable to an ObservableAsPropertyHelper and
    /// automatically provides the onChanged method to raise the property
    /// changed notification.
    /// </summary>
    /// <typeparam name="TObj">The object type.</typeparam>
    /// <typeparam name="TRet">The result type.</typeparam>
    /// <param name="target">
    /// The observable to convert to an ObservableAsPropertyHelper.
    /// </param>
    /// <param name="source">
    /// The ReactiveObject that has the property.
    /// </param>
    /// <param name="property">
    /// The name of the property that has changed. Recommended for use with nameof() or a FODY.
    /// </param>
    /// <param name="result">
    /// An out param matching the return value, provided for convenience.
    /// </param>
    /// <param name="getInitialValue">
    /// The function used to retrieve the initial value of the property.
    /// </param>
    /// <param name="deferSubscription">
    /// A value indicating whether the <see cref="ObservableAsPropertyHelper{T}"/>
    /// should defer the subscription to the <paramref name="target"/> source
    /// until the first call to <see cref="ObservableAsPropertyHelper{T}.Value"/>,
    /// or if it should immediately subscribe to the <paramref name="target"/> source.
    /// </param>
    /// <param name="scheduler">
    /// The scheduler that the notifications will be provided on - this should
    /// normally be a Dispatcher-based scheduler.
    /// </param>
    /// <returns>
    /// An initialized ObservableAsPropertyHelper; use this as the backing
    /// field for your property.
    /// </returns>
    public static ObservableAsPropertyHelper<TRet> ToProperty<TObj, TRet>(
        this IObservable<TRet> target,
        TObj source,
        string property,
        out ObservableAsPropertyHelper<TRet> result,
        Func<TRet> getInitialValue,
        bool deferSubscription = false,
        IScheduler? scheduler = null) // TODO: Create Test
        where TObj : class, IReactiveObject
    {
        ArgumentExceptionHelper.ThrowIfNull(target);
        ArgumentExceptionHelper.ThrowIfNull(source);

        ArgumentExceptionHelper.ThrowIfNullOrWhiteSpace(property);

        result = source.ObservableToProperty(target, property, getInitialValue, deferSubscription, scheduler);

        return result;
    }

    /// <summary>
    /// Creates an ObservableAsPropertyHelper that synchronizes the value of a property on the target object with the
    /// latest value from the specified observable sequence.
    /// </summary>
    /// <remarks>This method is intended for use with reactive UI patterns, enabling properties to be
    /// automatically updated in response to observable sequences. The returned ObservableAsPropertyHelper should be
    /// assigned to a backing field and exposed via a read-only property to ensure correct change notification
    /// behavior.</remarks>
    /// <typeparam name="TObj">The type of the target object that implements IReactiveObject.</typeparam>
    /// <typeparam name="TRet">The type of the property value.</typeparam>
    /// <param name="target">The object whose property will be updated in response to values emitted by the observable. Cannot be null.</param>
    /// <param name="observable">The observable sequence that provides values to assign to the property. Cannot be null.</param>
    /// <param name="property">An expression that identifies the property on the target object to be synchronized. Must be of the form 'x =>
    /// x.PropertyName'. Cannot be null.</param>
    /// <param name="getInitialValue">A function that returns the initial value for the property before any values are emitted by the observable.
    /// Cannot be null.</param>
    /// <param name="deferSubscription">true to defer subscribing to the observable until the property is first accessed; otherwise, false. The default
    /// is false.</param>
    /// <param name="scheduler">An optional scheduler used to deliver property change notifications. If null, the default scheduler is used.</param>
    /// <returns>An ObservableAsPropertyHelper that manages the synchronization between the observable sequence and the specified
    /// property.</returns>
    /// <exception cref="ArgumentException">Thrown if target, observable, or property is null, or if property is not a valid property expression.</exception>
    internal static ObservableAsPropertyHelper<TRet> ObservableToProperty<TObj, TRet>(
        this TObj target,
        IObservable<TRet?> observable,
        Expression<Func<TObj, TRet>> property,
        Func<TRet> getInitialValue,
        bool deferSubscription = false,
        IScheduler? scheduler = null)
        where TObj : class, IReactiveObject
    {
        ArgumentExceptionHelper.ThrowIfNull(target);
        ArgumentExceptionHelper.ThrowIfNull(observable);
        ArgumentExceptionHelper.ThrowIfNull(property);

        var expression = Reflection.Rewrite(property.Body);

        var parent = expression.GetParent() ?? throw new ArgumentException("The property expression does not have a valid parent.", nameof(property));
        if (parent.NodeType != ExpressionType.Parameter)
        {
            throw new ArgumentException("Property expression must be of the form 'x => x.SomeProperty'");
        }

        var memberInfo = expression.GetMemberInfo() ?? throw new ArgumentException("The property expression does not point towards a valid member.", nameof(property));
        var name = memberInfo.Name;
        if (expression is IndexExpression)
        {
            name += "[]";
        }

        return new ObservableAsPropertyHelper<TRet>(
                                                    observable,
                                                    _ => target.RaisingPropertyChanged(name),
                                                    _ => target.RaisingPropertyChanging(name),
                                                    getInitialValue,
                                                    deferSubscription,
                                                    scheduler);
    }

    /// <summary>
    /// Creates an ObservableAsPropertyHelper that synchronizes the specified observable sequence with a property on the
    /// target object.
    /// </summary>
    /// <remarks>This method is intended for use with reactive objects to facilitate property change
    /// notifications based on observable sequences. It ensures that property change events are raised appropriately
    /// when the observable emits new values.</remarks>
    /// <typeparam name="TObj">The type of the target object that implements IReactiveObject.</typeparam>
    /// <typeparam name="TRet">The type of the property and the values produced by the observable sequence.</typeparam>
    /// <param name="target">The object whose property will be updated in response to the observable sequence. Cannot be null.</param>
    /// <param name="observable">The observable sequence whose values will be used to update the property. Cannot be null.</param>
    /// <param name="property">An expression that identifies the property on the target object to synchronize with the observable sequence.
    /// Must be of the form 'x => x.Property'. Cannot be null.</param>
    /// <param name="deferSubscription">true to defer subscription to the observable sequence until the property is first accessed; otherwise, false.
    /// The default is false.</param>
    /// <param name="scheduler">An optional scheduler used to deliver property change notifications. If null, the default scheduler is used.</param>
    /// <returns>An ObservableAsPropertyHelper that manages the synchronization between the observable sequence and the specified
    /// property.</returns>
    /// <exception cref="ArgumentException">Thrown if target, observable, or property is null, or if property does not represent a valid property
    /// expression.</exception>
    internal static ObservableAsPropertyHelper<TRet> ObservableToProperty<TObj, TRet>(
        this TObj target,
        IObservable<TRet?> observable,
        Expression<Func<TObj, TRet>> property,
        bool deferSubscription = false,
        IScheduler? scheduler = null)
        where TObj : class, IReactiveObject
    {
        ArgumentExceptionHelper.ThrowIfNull(target);
        ArgumentExceptionHelper.ThrowIfNull(observable);
        ArgumentExceptionHelper.ThrowIfNull(property);

        var expression = Reflection.Rewrite(property.Body);

        var parent = expression.GetParent() ?? throw new ArgumentException("The property expression does not have a valid parent.", nameof(property));
        if (parent.NodeType != ExpressionType.Parameter)
        {
            throw new ArgumentException("Property expression must be of the form 'x => x.SomeProperty'");
        }

        var memberInfo = expression.GetMemberInfo() ?? throw new ArgumentException("The property expression does not point towards a valid member.", nameof(property));
        var name = memberInfo.Name;
        if (expression is IndexExpression)
        {
            name += "[]";
        }

        return new ObservableAsPropertyHelper<TRet>(
                                                    observable,
                                                    _ => target.RaisingPropertyChanged(name),
                                                    _ => target.RaisingPropertyChanging(name),
                                                    () => default,
                                                    deferSubscription,
                                                    scheduler);
    }

    /// <summary>
    /// Creates an ObservableAsPropertyHelper that synchronizes the specified observable sequence with a property on the
    /// target object, raising property change notifications as values are emitted.
    /// </summary>
    /// <remarks>This method is intended for use in reactive view models to facilitate property updates based
    /// on observable sequences. The returned ObservableAsPropertyHelper should be stored in a backing field to ensure
    /// proper subscription management and to avoid memory leaks.</remarks>
    /// <typeparam name="TObj">The type of the target object that implements IReactiveObject.</typeparam>
    /// <typeparam name="TRet">The type of the property value.</typeparam>
    /// <param name="target">The object whose property will be updated in response to the observable sequence. Must implement
    /// IReactiveObject.</param>
    /// <param name="observable">The observable sequence whose values will be used to update the property. Cannot be null.</param>
    /// <param name="property">The name of the property to synchronize with the observable sequence. Cannot be null.</param>
    /// <param name="getInitialValue">A function that returns the initial value of the property before any values are emitted by the observable.</param>
    /// <param name="deferSubscription">true to defer subscribing to the observable until the property is first accessed; otherwise, false. The default
    /// is false.</param>
    /// <param name="scheduler">An optional scheduler used to deliver property change notifications. If null, the default scheduler is used.</param>
    /// <returns>An ObservableAsPropertyHelper that manages the synchronization between the observable sequence and the specified
    /// property.</returns>
    internal static ObservableAsPropertyHelper<TRet> ObservableToProperty<TObj, TRet>(
        this TObj target,
        IObservable<TRet?> observable,
        string property,
        Func<TRet> getInitialValue,
        bool deferSubscription = false,
        IScheduler? scheduler = null)
        where TObj : class, IReactiveObject
    {
        ArgumentExceptionHelper.ThrowIfNull(target);
        ArgumentExceptionHelper.ThrowIfNull(observable);
        ArgumentExceptionHelper.ThrowIfNull(property);

        return new ObservableAsPropertyHelper<TRet>(
                                                    observable,
                                                    _ => target.RaisingPropertyChanged(property),
                                                    _ => target.RaisingPropertyChanging(property),
                                                    getInitialValue,
                                                    deferSubscription,
                                                    scheduler);
    }

    /// <summary>
    /// Creates an ObservableAsPropertyHelper that synchronizes the specified observable sequence with a property on the
    /// target object.
    /// </summary>
    /// <remarks>Use this method to connect an observable sequence to a property, enabling reactive updates
    /// and change notifications on the target object. This is commonly used in reactive UI patterns to keep properties
    /// in sync with asynchronous or event-driven data sources.</remarks>
    /// <typeparam name="TObj">The type of the target object that implements IReactiveObject.</typeparam>
    /// <typeparam name="TRet">The type of the values produced by the observable sequence and exposed by the property.</typeparam>
    /// <param name="target">The object whose property will be updated in response to the observable sequence. Cannot be null.</param>
    /// <param name="observable">The observable sequence whose values will be used to update the property. Cannot be null.</param>
    /// <param name="property">The name of the property to synchronize with the observable sequence. Cannot be null.</param>
    /// <param name="deferSubscription">true to defer subscribing to the observable sequence until the property is accessed; otherwise, false. The
    /// default is false.</param>
    /// <param name="scheduler">An optional scheduler used to deliver property change notifications. If null, the default scheduler is used.</param>
    /// <returns>An ObservableAsPropertyHelper that manages the synchronization between the observable sequence and the specified
    /// property.</returns>
    internal static ObservableAsPropertyHelper<TRet> ObservableToProperty<TObj, TRet>(
        this TObj target,
        IObservable<TRet?> observable,
        string property,
        bool deferSubscription = false,
        IScheduler? scheduler = null)
        where TObj : class, IReactiveObject
    {
        ArgumentExceptionHelper.ThrowIfNull(target);
        ArgumentExceptionHelper.ThrowIfNull(observable);
        ArgumentExceptionHelper.ThrowIfNull(property);

        return new ObservableAsPropertyHelper<TRet>(
                                                    observable,
                                                    _ => target.RaisingPropertyChanged(property),
                                                    _ => target.RaisingPropertyChanging(property),
                                                    () => default,
                                                    deferSubscription,
                                                    scheduler);
    }
}
