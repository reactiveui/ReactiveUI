// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Builder;

namespace ReactiveUI;

/// <summary>
/// Extension methods associated with the Observable Changes and the
/// Reactive Notify Property Changed based events.
/// </summary>
[Preserve(AllMembers = true)]
[RequiresUnreferencedCode(
    "Creating Expressions requires unreferenced code because the members being referenced by the Expression may be trimmed.")]
public static class ReactiveNotifyPropertyChangedMixin
{
    private static readonly
        MemoizingMRUCache<(Type senderType, string propertyName, bool beforeChange), ICreatesObservableForProperty?>
        _notifyFactoryCache =
            new(
                (t, _) => AppLocator.Current.GetServices<ICreatesObservableForProperty>()
                    .Aggregate(
                        (score: 0, binding: (ICreatesObservableForProperty?)null),
                        (acc, x) =>
                        {
                            var score = x.GetAffinityForObject(t.senderType, t.propertyName, t.beforeChange);
                            return score > acc.score ? (score, x) : acc;
                        }).binding,
                RxCacheSize.BigCacheLimit);

    /// <summary>
    /// Initializes static members of the <see cref="ReactiveNotifyPropertyChangedMixin"/> class.
    /// </summary>
    static ReactiveNotifyPropertyChangedMixin() => RxAppBuilder.EnsureInitialized();

    /// <summary>
    /// ObservableForProperty returns an Observable representing the
    /// property change notifications for a specific property name on a
    /// ReactiveObject (or compatible type). This overload avoids expression tree
    /// analysis to be more AOT-friendly. The returned IObservedChange instances
    /// will always have the Value property populated via reflection.
    /// </summary>
    /// <typeparam name="TSender">The sender type.</typeparam>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <param name="item">The source object to observe properties of.</param>
    /// <param name="propertyName">The property name to observe.</param>
    /// <param name="beforeChange">If true, the Observable will notify immediately before a property is going to change.</param>
    /// <param name="skipInitial">If true, the Observable will not notify with the initial value.</param>
    /// <param name="isDistinct">If set to <c>true</c>, values are filtered with DistinctUntilChanged.</param>
    /// <returns>An Observable representing the property change notifications for the given property name.</returns>
    [RequiresUnreferencedCode(
        "Creating Expressions requires unreferenced code because the members being referenced by the Expression may be trimmed.")]
    public static IObservable<IObservedChange<TSender, TValue>> ObservableForProperty<TSender, TValue>(
        this TSender? item,
        string propertyName,
        bool beforeChange,
        bool skipInitial,
        bool isDistinct)
    {
        ArgumentExceptionHelper.ThrowIfNull(item);
        ArgumentExceptionHelper.ThrowIfNull(propertyName);

        // Create a minimal expression to attach to ObservedChange for compatibility.
        var parameter = Expression.Parameter(typeof(TSender), "x");
        Expression expr;
        try
        {
            expr = Expression.Property(parameter, propertyName);
        }
        catch
        {
            // Fall back to a simple member access-less expression if property is not found at compile time.
            expr = parameter;
        }

        var factory = _notifyFactoryCache.Get((item!.GetType(), propertyName, beforeChange))
                      ?? throw new Exception(
                          $"Could not find a ICreatesObservableForProperty for {item!.GetType()} property {propertyName}. This should never happen, your service locator is probably broken. Please make sure you have installed the latest version of the ReactiveUI packages for your platform. See https://reactiveui.net/docs/getting-started/installation for guidance.");

        // Helper to get current property value without expression analysis.
        static TValue GetCurrentValue(object sender, string name)
        {
            var t = sender.GetType();
#if NETSTANDARD || NETFRAMEWORK
            var prop =
 t.GetProperty(name, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.FlattenHierarchy);
#else
            var prop = t.GetProperty(
                name,
                System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.FlattenHierarchy);
#endif
            if (prop is null)
            {
                return default!;
            }

            var val = prop.GetValue(sender);
            if (val is null)
            {
                return default!;
            }

            return val is TValue tv ? tv : (TValue)val;
        }

        var core = Observable.Create<IObservedChange<TSender, TValue>>(obs =>
        {
            // Emit initial value if requested.
            if (!skipInitial)
            {
                try
                {
                    var initial = GetCurrentValue(item!, propertyName);
                    obs.OnNext(new ObservedChange<TSender, TValue>(item!, expr, initial));
                }
                catch (Exception ex)
                {
                    obs.OnError(ex);
                }
            }

            var subscription = factory
                .GetNotificationForProperty(item!, expr, propertyName, beforeChange, suppressWarnings: false)
                .Subscribe(
                    _ =>
                    {
                        try
                        {
                            var current = GetCurrentValue(item!, propertyName);
                            obs.OnNext(new ObservedChange<TSender, TValue>(item!, expr, current));
                        }
                        catch (Exception ex)
                        {
                            obs.OnError(ex);
                        }
                    },
                    obs.OnError,
                    obs.OnCompleted);

            return subscription;
        });

        if (isDistinct)
        {
            return core.DistinctUntilChanged(x => x.Value);
        }

        return core;
    }

    /// <summary>
    /// ObservableForProperty overload that avoids expression trees by using only a property name.
    /// </summary>
    /// <typeparam name="TSender">The sender type.</typeparam>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <param name="item">The source object to observe properties of.</param>
    /// <param name="propertyName">The property name to observe.</param>
    /// <returns>An observable sequence of observed changes for the given property name.</returns>
    [RequiresUnreferencedCode(
        "Creating Expressions requires unreferenced code because the members being referenced by the Expression may be trimmed.")]
    public static IObservable<IObservedChange<TSender, TValue>> ObservableForProperty<TSender, TValue>(
        this TSender? item,
        string propertyName)
        => ObservableForProperty<TSender, TValue>(
            item,
            propertyName,
            beforeChange: false,
            skipInitial: true,
            isDistinct: true);

    /// <summary>
    /// ObservableForProperty overload that avoids expression trees by using a property name and beforeChange option.
    /// </summary>
    /// <typeparam name="TSender">The sender type.</typeparam>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <param name="item">The source object to observe properties of.</param>
    /// <param name="propertyName">The property name to observe.</param>
    /// <param name="beforeChange">If true, the observable will notify immediately before a property is going to change.</param>
    /// <returns>An observable sequence of observed changes for the given property name.</returns>
    [RequiresUnreferencedCode(
        "Creating Expressions requires unreferenced code because the members being referenced by the Expression may be trimmed.")]
    public static IObservable<IObservedChange<TSender, TValue>> ObservableForProperty<TSender, TValue>(
        this TSender? item,
        string propertyName,
        bool beforeChange)
        => ObservableForProperty<TSender, TValue>(
            item,
            propertyName,
            beforeChange: beforeChange,
            skipInitial: true,
            isDistinct: true);

    /// <summary>
    /// ObservableForProperty overload that avoids expression trees by using a property name with options to control initial emission and beforeChange.
    /// </summary>
    /// <typeparam name="TSender">The sender type.</typeparam>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <param name="item">The source object to observe properties of.</param>
    /// <param name="propertyName">The property name to observe.</param>
    /// <param name="beforeChange">If true, the observable will notify immediately before a property is going to change.</param>
    /// <param name="skipInitial">If true, the observable will not notify with the initial value.</param>
    /// <returns>An observable sequence of observed changes for the given property name.</returns>
    [RequiresUnreferencedCode(
        "Creating Expressions requires unreferenced code because the members being referenced by the Expression may be trimmed.")]
    public static IObservable<IObservedChange<TSender, TValue>> ObservableForProperty<TSender, TValue>(
        this TSender? item,
        string propertyName,
        bool beforeChange,
        bool skipInitial)
        => ObservableForProperty<TSender, TValue>(
            item,
            propertyName,
            beforeChange: beforeChange,
            skipInitial: skipInitial,
            isDistinct: true);

    /// <summary>
    /// ObservableForProperty returns an Observable representing the
    /// property change notifications for a specific property on a
    /// ReactiveObject. This method (unlike other Observables that return
    /// IObservedChange) guarantees that the Value property of
    /// the IObservedChange is set.
    /// </summary>
    /// <typeparam name="TSender">The sender type.</typeparam>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <param name="item">The source object to observe properties of.</param>
    /// <param name="property">An Expression representing the property (i.e.
    /// 'x =&gt; x.SomeProperty.SomeOtherProperty'.</param>
    /// <returns>
    /// An Observable representing the property change
    /// notifications for the given property.
    /// </returns>
    [RequiresUnreferencedCode("Evaluates expression-based member chains via reflection; members may be trimmed.")]
    public static IObservable<IObservedChange<TSender, TValue>> ObservableForProperty<TSender, TValue>(
        this TSender? item,
        Expression<Func<TSender, TValue>> property) => ObservableForProperty(item, property, false, true, true);

    /// <summary>
    /// ObservableForProperty returns an Observable representing the
    /// property change notifications for a specific property on a
    /// ReactiveObject. This method (unlike other Observables that return
    /// IObservedChange) guarantees that the Value property of
    /// the IObservedChange is set.
    /// </summary>
    /// <typeparam name="TSender">The sender type.</typeparam>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <param name="item">The source object to observe properties of.</param>
    /// <param name="property">An Expression representing the property (i.e.
    /// 'x =&gt; x.SomeProperty.SomeOtherProperty'.</param>
    /// <param name="beforeChange">If True, the Observable will notify
    /// immediately before a property is going to change.</param>
    /// <returns>
    /// An Observable representing the property change
    /// notifications for the given property.
    /// </returns>
    [RequiresUnreferencedCode("Evaluates expression-based member chains via reflection; members may be trimmed.")]
    public static IObservable<IObservedChange<TSender, TValue>> ObservableForProperty<TSender, TValue>(
        this TSender? item,
        Expression<Func<TSender, TValue>> property,
        bool beforeChange) => ObservableForProperty(item, property, beforeChange, true, true);

    /// <summary>
    /// ObservableForProperty returns an Observable representing the
    /// property change notifications for a specific property on a
    /// ReactiveObject. This method (unlike other Observables that return
    /// IObservedChange) guarantees that the Value property of
    /// the IObservedChange is set.
    /// </summary>
    /// <typeparam name="TSender">The sender type.</typeparam>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <param name="item">The source object to observe properties of.</param>
    /// <param name="property">An Expression representing the property (i.e.
    /// 'x =&gt; x.SomeProperty.SomeOtherProperty'.</param>
    /// <param name="beforeChange">If True, the Observable will notify
    /// immediately before a property is going to change.</param>
    /// <param name="skipInitial">If true, the Observable will not notify
    /// with the initial value.</param>
    /// <returns>
    /// An Observable representing the property change
    /// notifications for the given property.
    /// </returns>
    [RequiresUnreferencedCode("Evaluates expression-based member chains via reflection; members may be trimmed.")]
    public static IObservable<IObservedChange<TSender, TValue>> ObservableForProperty<TSender, TValue>(
        this TSender? item,
        Expression<Func<TSender, TValue>> property,
        bool beforeChange,
        bool skipInitial) => ObservableForProperty(item, property, beforeChange, skipInitial, true);

    /// <summary>
    /// ObservableForProperty returns an Observable representing the
    /// property change notifications for a specific property on a
    /// ReactiveObject. This method (unlike other Observables that return
    /// IObservedChange) guarantees that the Value property of
    /// the IObservedChange is set.
    /// </summary>
    /// <typeparam name="TSender">The sender type.</typeparam>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <param name="item">The source object to observe properties of.</param>
    /// <param name="property">An Expression representing the property (i.e.
    /// 'x =&gt; x.SomeProperty.SomeOtherProperty'.</param>
    /// <param name="beforeChange">If True, the Observable will notify
    /// immediately before a property is going to change.</param>
    /// <param name="skipInitial">If true, the Observable will not notify
    /// with the initial value.</param>
    /// <param name="isDistinct">if set to <c>true</c> [is distinct].</param>
    /// <returns>
    /// An Observable representing the property change
    /// notifications for the given property.
    /// </returns>
    [RequiresUnreferencedCode("Evaluates expression-based member chains via reflection; members may be trimmed.")]
    public static IObservable<IObservedChange<TSender, TValue>> ObservableForProperty<TSender, TValue>(
        this TSender? item,
        Expression<Func<TSender, TValue>> property,
        bool beforeChange,
        bool skipInitial,
        bool isDistinct)
    {
        ArgumentExceptionHelper.ThrowIfNull(property);

        /* x => x.Foo.Bar.Baz;
         *
         * Subscribe to This, look for Foo
         * Subscribe to Foo, look for Bar
         * Subscribe to Bar, look for Baz
         * Subscribe to Baz, publish to Subject
         * Return Subject
         *
         * If Bar changes (notification fires on Foo), resubscribe to new Bar
         *  Resubscribe to new Baz, publish to Subject
         *
         * If Baz changes (notification fires on Bar),
         *  Resubscribe to new Baz, publish to Subject
         */

        return SubscribeToExpressionChain<TSender, TValue>(
            item,
            property.Body,
            beforeChange,
            skipInitial,
            isDistinct);
    }

    /// <summary>
    /// ObservableForProperty returns an Observable representing the
    /// property change notifications for a specific property on a
    /// ReactiveObject, running the IObservedChange through a Selector
    /// function.
    /// </summary>
    /// <typeparam name="TSender">The sender type.</typeparam>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <typeparam name="TRet">The return value type.</typeparam>
    /// <param name="item">The source object to observe properties of.</param>
    /// <param name="property">An Expression representing the property (i.e.
    /// 'x => x.SomeProperty'.</param>
    /// <param name="selector">A Select function that will be run on each
    /// item.</param>
    /// <returns>An Observable representing the property change
    /// notifications for the given property.</returns>
    [RequiresUnreferencedCode("Evaluates expression-based member chains via reflection; members may be trimmed.")]
    public static IObservable<TRet> ObservableForProperty<TSender, TValue, TRet>(
        this TSender? item,
        Expression<Func<TSender, TValue>> property,
        Func<TValue?, TRet> selector) // TODO: Create Test
        where TSender : class
    {
        ArgumentExceptionHelper.ThrowIfNull(property);
        ArgumentExceptionHelper.ThrowIfNull(selector);

        return item.ObservableForProperty(property, false).Select(x => selector(x.Value));
    }

    /// <summary>
    /// ObservableForProperty returns an Observable representing the
    /// property change notifications for a specific property on a
    /// ReactiveObject, running the IObservedChange through a Selector
    /// function.
    /// </summary>
    /// <typeparam name="TSender">The sender type.</typeparam>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <typeparam name="TRet">The return value type.</typeparam>
    /// <param name="item">The source object to observe properties of.</param>
    /// <param name="property">An Expression representing the property (i.e.
    /// 'x => x.SomeProperty'.</param>
    /// <param name="selector">A Select function that will be run on each
    /// item.</param>
    /// <param name="beforeChange">If True, the Observable will notify
    /// immediately before a property is going to change.</param>
    /// <returns>An Observable representing the property change
    /// notifications for the given property.</returns>
    [RequiresUnreferencedCode("Evaluates expression-based member chains via reflection; members may be trimmed.")]
    public static IObservable<TRet> ObservableForProperty<TSender, TValue, TRet>(
        this TSender? item,
        Expression<Func<TSender, TValue>> property,
        Func<TValue?, TRet> selector,
        bool beforeChange) // TODO: Create Test
        where TSender : class
    {
        ArgumentExceptionHelper.ThrowIfNull(property);
        ArgumentExceptionHelper.ThrowIfNull(selector);

        return item.ObservableForProperty(property, beforeChange).Select(x => selector(x.Value));
    }

    /// <summary>
    /// Creates a observable which will subscribe to the each property and sub property
    /// specified in the Expression. eg It will subscribe to x =&gt; x.Property1.Property2.Property3
    /// each property in the lambda expression. It will then provide updates to the last value in the chain.
    /// </summary>
    /// <typeparam name="TSender">The type of the origin of the expression chain.</typeparam>
    /// <typeparam name="TValue">The end value we want to subscribe to.</typeparam>
    /// <param name="source">The object where we start the chain.</param>
    /// <param name="expression">A expression which will point towards the property.</param>
    /// <returns>
    /// A observable which notifies about observed changes.
    /// </returns>
    /// <exception cref="InvalidCastException">If we cannot cast from the target value from the specified last property.</exception>
    [RequiresUnreferencedCode("Evaluates expression-based member chains via reflection; members may be trimmed.")]
    public static IObservable<IObservedChange<TSender, TValue>> SubscribeToExpressionChain<TSender, TValue>(
        this TSender? source,
        Expression? expression) // TODO: Create Test
        => SubscribeToExpressionChain<TSender, TValue>(source, expression, false, true, false, true);

    /// <summary>
    /// Creates a observable which will subscribe to the each property and sub property
    /// specified in the Expression. eg It will subscribe to x =&gt; x.Property1.Property2.Property3
    /// each property in the lambda expression. It will then provide updates to the last value in the chain.
    /// </summary>
    /// <typeparam name="TSender">The type of the origin of the expression chain.</typeparam>
    /// <typeparam name="TValue">The end value we want to subscribe to.</typeparam>
    /// <param name="source">The object where we start the chain.</param>
    /// <param name="expression">A expression which will point towards the property.</param>
    /// <param name="beforeChange">If we are interested in notifications before the property value is changed.</param>
    /// <returns>
    /// A observable which notifies about observed changes.
    /// </returns>
    /// <exception cref="InvalidCastException">If we cannot cast from the target value from the specified last property.</exception>
    [RequiresUnreferencedCode("Evaluates expression-based member chains via reflection; members may be trimmed.")]
    public static IObservable<IObservedChange<TSender, TValue>> SubscribeToExpressionChain<TSender, TValue>(
        this TSender? source,
        Expression? expression,
        bool beforeChange) // TODO: Create Test
        => SubscribeToExpressionChain<TSender, TValue>(source, expression, beforeChange, true, false, true);

    /// <summary>
    /// Creates a observable which will subscribe to the each property and sub property
    /// specified in the Expression. eg It will subscribe to x =&gt; x.Property1.Property2.Property3
    /// each property in the lambda expression. It will then provide updates to the last value in the chain.
    /// </summary>
    /// <typeparam name="TSender">The type of the origin of the expression chain.</typeparam>
    /// <typeparam name="TValue">The end value we want to subscribe to.</typeparam>
    /// <param name="source">The object where we start the chain.</param>
    /// <param name="expression">A expression which will point towards the property.</param>
    /// <param name="beforeChange">If we are interested in notifications before the property value is changed.</param>
    /// <param name="skipInitial">If we don't want to get a notification about the default value of the property.</param>
    /// <returns>
    /// A observable which notifies about observed changes.
    /// </returns>
    /// <exception cref="InvalidCastException">If we cannot cast from the target value from the specified last property.</exception>
    [RequiresUnreferencedCode("Evaluates expression-based member chains via reflection; members may be trimmed.")]
    public static IObservable<IObservedChange<TSender, TValue>> SubscribeToExpressionChain<TSender, TValue>(
        this TSender? source,
        Expression? expression,
        bool beforeChange,
        bool skipInitial) // TODO: Create Test
        => SubscribeToExpressionChain<TSender, TValue>(source, expression, beforeChange, skipInitial, false, true);

    /// <summary>
    /// Creates a observable which will subscribe to the each property and sub property
    /// specified in the Expression. eg It will subscribe to x =&gt; x.Property1.Property2.Property3
    /// each property in the lambda expression. It will then provide updates to the last value in the chain.
    /// </summary>
    /// <typeparam name="TSender">The type of the origin of the expression chain.</typeparam>
    /// <typeparam name="TValue">The end value we want to subscribe to.</typeparam>
    /// <param name="source">The object where we start the chain.</param>
    /// <param name="expression">A expression which will point towards the property.</param>
    /// <param name="beforeChange">If we are interested in notifications before the property value is changed.</param>
    /// <param name="skipInitial">If we don't want to get a notification about the default value of the property.</param>
    /// <param name="suppressWarnings">If true, no warnings should be logged.</param>
    /// <returns>
    /// A observable which notifies about observed changes.
    /// </returns>
    /// <exception cref="InvalidCastException">If we cannot cast from the target value from the specified last property.</exception>
    [RequiresUnreferencedCode("Evaluates expression-based member chains via reflection; members may be trimmed.")]
    public static IObservable<IObservedChange<TSender, TValue>> SubscribeToExpressionChain<TSender, TValue>(
        this TSender? source,
        Expression? expression,
        bool beforeChange,
        bool skipInitial,
        bool suppressWarnings) // TODO: Create Test
        => SubscribeToExpressionChain<TSender, TValue>(
            source,
            expression,
            beforeChange,
            skipInitial,
            suppressWarnings,
            true);

    /// <summary>
    /// Creates a observable which will subscribe to the each property and sub property
    /// specified in the Expression. eg It will subscribe to x =&gt; x.Property1.Property2.Property3
    /// each property in the lambda expression. It will then provide updates to the last value in the chain.
    /// </summary>
    /// <typeparam name="TSender">The type of the origin of the expression chain.</typeparam>
    /// <typeparam name="TValue">The end value we want to subscribe to.</typeparam>
    /// <param name="source">The object where we start the chain.</param>
    /// <param name="expression">A expression which will point towards the property.</param>
    /// <param name="beforeChange">If we are interested in notifications before the property value is changed.</param>
    /// <param name="skipInitial">If we don't want to get a notification about the default value of the property.</param>
    /// <param name="suppressWarnings">If true, no warnings should be logged.</param>
    /// <param name="isDistinct">if set to <c>true</c> [is distinct].</param>
    /// <returns>
    /// A observable which notifies about observed changes.
    /// </returns>
    /// <exception cref="InvalidCastException">If we cannot cast from the target value from the specified last property.</exception>
    [RequiresUnreferencedCode("Evaluates expression-based member chains via reflection; members may be trimmed.")]
    public static IObservable<IObservedChange<TSender, TValue>> SubscribeToExpressionChain<TSender, TValue>(
        this TSender? source,
        Expression? expression,
        bool beforeChange,
        bool skipInitial,
        bool suppressWarnings,
        bool isDistinct) // TODO: Create Test
    {
        IObservable<IObservedChange<object?, object?>> notifier =
            Observable.Return(new ObservedChange<object?, object?>(null, null, source));

        var chain = Reflection.Rewrite(expression).GetExpressionChain();
        notifier = chain.Aggregate(
            notifier,
            (n, expr) => n
                .Select(y => NestedObservedChanges(expr, y, beforeChange, suppressWarnings))
                .Switch());

        if (skipInitial)
        {
            notifier = notifier.Skip(1);
        }

        notifier = notifier.Where(x => x.Sender is not null);

        var r = notifier.Select(x =>
        {
            // ensure cast to TValue will succeed, throw useful exception otherwise
            var val = x.GetValue();
            if (val is not null && val is not TValue)
            {
                throw new InvalidCastException($"Unable to cast from {val.GetType()} to {typeof(TValue)}.");
            }

            return new ObservedChange<TSender, TValue>(source!, expression, (TValue)val!);
        });

        return isDistinct ? r.DistinctUntilChanged(x => x.Value) : r;
    }

    [RequiresUnreferencedCode("Evaluates expression-based member chains via reflection; members may be trimmed.")]
    private static IObservable<IObservedChange<object?, object?>> NestedObservedChanges(
        Expression expression,
        IObservedChange<object?, object?> sourceChange,
        bool beforeChange,
        bool suppressWarnings)
    {
        // Make sure a change at a root node propagates events down
        var kicker = new ObservedChange<object?, object?>(sourceChange.Value, expression, default);

        // Handle null values in the chain
        if (sourceChange.Value is null)
        {
            return Observable.Return(kicker);
        }

        // Handle non null values in the chain
        return NotifyForProperty(sourceChange.Value, expression, beforeChange, suppressWarnings)
            .StartWith(kicker)
            .Select(static x => new ObservedChange<object?, object?>(x.Sender, x.Expression, x.GetValueOrDefault()));
    }

    [RequiresUnreferencedCode("Evaluates expression-based member chains via reflection; members may be trimmed.")]
    private static IObservable<IObservedChange<object?, object?>> NotifyForProperty(
        object sender,
        Expression expression,
        bool beforeChange,
        bool suppressWarnings)
    {
        ArgumentExceptionHelper.ThrowIfNull(expression);

        var memberInfo = expression.GetMemberInfo() ?? throw new ArgumentException(
            "The expression does not have valid member info",
            nameof(expression));
        var propertyName = memberInfo.Name;
        var result = _notifyFactoryCache.Get((sender.GetType(), propertyName, beforeChange));

        return result switch
        {
            null => throw new InvalidOperationException(
                $"Could not find a ICreatesObservableForProperty for {sender.GetType()} property {propertyName}. This should never happen, your service locator is probably broken. Please make sure you have installed the latest version of the ReactiveUI packages for your platform. See https://reactiveui.net/docs/getting-started/installation for guidance."),
            _ => result.GetNotificationForProperty(sender, expression, propertyName, beforeChange, suppressWarnings)
        };
    }
}
