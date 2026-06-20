// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using Splat;

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive;
#else
namespace ReactiveUI;
#endif
/// <summary>
/// Provides extension methods for observing property change notifications on objects, enabling reactive programming
/// patterns for property changes without relying on expression tree analysis. These methods allow consumers to create
/// observable sequences that emit notifications when specified properties change, supporting both simple property names
/// and expression-based property access.
/// </summary>
/// <remarks>The methods in this class are designed to work with types that implement property change
/// notification, such as ReactiveObject or compatible types. Overloads are provided to observe property changes by
/// property name or by expression, with options to control notification timing (before or after change), initial value
/// emission, and distinct value filtering. These APIs are especially useful in scenarios where expression tree analysis
/// is not available or desirable, such as ahead-of-time (AOT) compilation environments. Consumers should be aware that
/// some methods require unreferenced code and may not be compatible with all trimming scenarios. For more information
/// on supported platforms and usage, refer to the ReactiveUI documentation.</remarks>
[Preserve(AllMembers = true)]
[RequiresUnreferencedCode(
    "Creating Expressions requires unreferenced code because the members being referenced by the Expression may be trimmed.")]
public static class ReactiveNotifyPropertyChangedMixins
{
    /// <summary>Caches the best available property-notification factory for each sender type, property name, and change timing.</summary>
    private static readonly
        MemoizingMRUCache<(Type? senderType, string propertyName, bool beforeChange), ICreatesObservableForProperty?>
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

    /// <summary>Initializes static members of the <see cref="ReactiveNotifyPropertyChangedMixins"/> class.</summary>
    static ReactiveNotifyPropertyChangedMixins() => RxAppBuilder.EnsureInitialized();

    /// <summary>Provides ObservableForProperty extension members for observing property changes on a sender object.</summary>
    /// <typeparam name="TSender">The sender type.</typeparam>
    /// <param name="item">The source object to observe properties of.</param>
    extension<TSender>(TSender? item)
    {
        /// <summary>
        /// ObservableForProperty returns an Observable representing the
        /// property change notifications for a specific property name on a
        /// ReactiveObject (or compatible type). This overload avoids expression tree
        /// analysis to be more AOT-friendly. The returned IObservedChange instances
        /// will always have the Value property populated via reflection.
        /// </summary>
        /// <typeparam name="TValue">The value type.</typeparam>
        /// <param name="propertyName">The property name to observe.</param>
        /// <param name="beforeChange">If true, the Observable will notify immediately before a property is going to change.</param>
        /// <param name="skipInitial">If true, the Observable will not notify with the initial value.</param>
        /// <param name="isDistinct">If set to <c>true</c>, values are filtered with DistinctUntilChanged.</param>
        /// <returns>An Observable representing the property change notifications for the given property name.</returns>
        [RequiresUnreferencedCode(
            "Creating Expressions requires unreferenced code because the members being referenced by the Expression may be trimmed.")]
        [SuppressMessage(
            "Major Code Smell",
            "S4018:Generic methods should provide type parameter",
            Justification = "Generic type parameter is supplied explicitly by the caller by design; it identifies the target type and cannot be inferred from the method's parameters.")]
        public IObservable<IObservedChange<TSender, TValue>> ObservableForProperty<TValue>(
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

            var factory = _notifyFactoryCache.Get((item.GetType(), propertyName, beforeChange))
                          ?? throw new InvalidOperationException(
                              $"Could not find a ICreatesObservableForProperty for {item.GetType()} property {propertyName}. " +
                              "This should never happen, your service locator is probably broken. Please make sure you have installed " +
                              "the latest version of the ReactiveUI packages for your platform. See https://reactiveui.net/docs/getting-started/installation for guidance.");

            // Helper to get current property value without expression analysis.
            static TValue GetCurrentValue(TSender sender, string name)
            {
                var t = sender?.GetType();
#if NETSTANDARD || NETFRAMEWORK
                var prop =
                    t?.GetProperty(
                        name,
                        System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.FlattenHierarchy);
#else
                var prop = t?.GetProperty(
                    name,
                    System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.FlattenHierarchy);
#endif

                var val = prop?.GetValue(sender);
                if (val is null)
                {
                    return default!;
                }

                return val is TValue tv ? tv : (TValue)val;
            }

            var notifications = factory.GetNotificationForProperty(item, expr, propertyName, beforeChange, suppressWarnings: false);
            return new ObservableForPropertySink<TSender, TValue>(item, expr, propertyName, notifications, GetCurrentValue, skipInitial, isDistinct);
        }

        /// <summary>ObservableForProperty overload that avoids expression trees by using only a property name.</summary>
        /// <typeparam name="TValue">The value type.</typeparam>
        /// <param name="propertyName">The property name to observe.</param>
        /// <returns>An observable sequence of observed changes for the given property name.</returns>
        [RequiresUnreferencedCode(
            "Creating Expressions requires unreferenced code because the members being referenced by the Expression may be trimmed.")]
        [SuppressMessage(
            "Major Code Smell",
            "S4018:Generic methods should provide type parameter",
            Justification = "Generic type parameter is supplied explicitly by the caller by design; it identifies the target type and cannot be inferred from the method's parameters.")]
        public IObservable<IObservedChange<TSender, TValue>> ObservableForProperty<TValue>(
            string propertyName)
            => ObservableForProperty<TSender, TValue>(
                item,
                propertyName,
                beforeChange: false,
                skipInitial: true,
                isDistinct: true);

        /// <summary>ObservableForProperty overload that avoids expression trees by using a property name and beforeChange option.</summary>
        /// <typeparam name="TValue">The value type.</typeparam>
        /// <param name="propertyName">The property name to observe.</param>
        /// <param name="beforeChange">If true, the observable will notify immediately before a property is going to change.</param>
        /// <returns>An observable sequence of observed changes for the given property name.</returns>
        [RequiresUnreferencedCode(
            "Creating Expressions requires unreferenced code because the members being referenced by the Expression may be trimmed.")]
        [SuppressMessage(
            "Major Code Smell",
            "S4018:Generic methods should provide type parameter",
            Justification = "Generic type parameter is supplied explicitly by the caller by design; it identifies the target type and cannot be inferred from the method's parameters.")]
        public IObservable<IObservedChange<TSender, TValue>> ObservableForProperty<TValue>(
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
        /// <typeparam name="TValue">The value type.</typeparam>
        /// <param name="propertyName">The property name to observe.</param>
        /// <param name="beforeChange">If true, the observable will notify immediately before a property is going to change.</param>
        /// <param name="skipInitial">If true, the observable will not notify with the initial value.</param>
        /// <returns>An observable sequence of observed changes for the given property name.</returns>
        [RequiresUnreferencedCode(
            "Creating Expressions requires unreferenced code because the members being referenced by the Expression may be trimmed.")]
        [SuppressMessage(
            "Major Code Smell",
            "S4018:Generic methods should provide type parameter",
            Justification = "Generic type parameter is supplied explicitly by the caller by design; it identifies the target type and cannot be inferred from the method's parameters.")]
        public IObservable<IObservedChange<TSender, TValue>> ObservableForProperty<TValue>(
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
        /// <typeparam name="TValue">The value type.</typeparam>
        /// <param name="property">An Expression representing the property (i.e.
        /// 'x =&gt; x.SomeProperty.SomeOtherProperty'.</param>
        /// <returns>
        /// An Observable representing the property change
        /// notifications for the given property.
        /// </returns>
        [RequiresUnreferencedCode("Evaluates expression-based member chains via reflection; members may be trimmed.")]
        public IObservable<IObservedChange<TSender, TValue>> ObservableForProperty<TValue>(
            Expression<Func<TSender, TValue>> property) => ObservableForProperty(item, property, false, true, true);

        /// <summary>
        /// ObservableForProperty returns an Observable representing the
        /// property change notifications for a specific property on a
        /// ReactiveObject. This method (unlike other Observables that return
        /// IObservedChange) guarantees that the Value property of
        /// the IObservedChange is set.
        /// </summary>
        /// <typeparam name="TValue">The value type.</typeparam>
        /// <param name="property">An Expression representing the property (i.e.
        /// 'x =&gt; x.SomeProperty.SomeOtherProperty'.</param>
        /// <param name="beforeChange">If True, the Observable will notify
        /// immediately before a property is going to change.</param>
        /// <returns>
        /// An Observable representing the property change
        /// notifications for the given property.
        /// </returns>
        [RequiresUnreferencedCode("Evaluates expression-based member chains via reflection; members may be trimmed.")]
        public IObservable<IObservedChange<TSender, TValue>> ObservableForProperty<TValue>(
            Expression<Func<TSender, TValue>> property,
            bool beforeChange) => ObservableForProperty(item, property, beforeChange, true, true);

        /// <summary>
        /// ObservableForProperty returns an Observable representing the
        /// property change notifications for a specific property on a
        /// ReactiveObject. This method (unlike other Observables that return
        /// IObservedChange) guarantees that the Value property of
        /// the IObservedChange is set.
        /// </summary>
        /// <typeparam name="TValue">The value type.</typeparam>
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
        public IObservable<IObservedChange<TSender, TValue>> ObservableForProperty<TValue>(
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
        /// <typeparam name="TValue">The value type.</typeparam>
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
        [SuppressMessage("Major Code Smell", "S125:Sections of code should not be commented out", Justification = "False positive, pseudo code.")]
        public IObservable<IObservedChange<TSender, TValue>> ObservableForProperty<TValue>(
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
        /// Creates a observable which will subscribe to the each property and sub property
        /// specified in the Expression. eg It will subscribe to x =&gt; x.Property1.Property2.Property3
        /// each property in the lambda expression. It will then provide updates to the last value in the chain.
        /// </summary>
        /// <typeparam name="TValue">The end value we want to subscribe to.</typeparam>
        /// <param name="expression">A expression which will point towards the property.</param>
        /// <returns>
        /// A observable which notifies about observed changes.
        /// </returns>
        /// <exception cref="InvalidCastException">If we cannot cast from the target value from the specified last property.</exception>
        [RequiresUnreferencedCode("Evaluates expression-based member chains via reflection; members may be trimmed.")]
        [SuppressMessage(
            "Major Code Smell",
            "S4018:Generic methods should provide type parameter",
            Justification = "Generic type parameter is supplied explicitly by the caller by design; it identifies the target type and cannot be inferred from the method's parameters.")]
        public IObservable<IObservedChange<TSender, TValue>> SubscribeToExpressionChain<TValue>(
            Expression? expression)
            => SubscribeToExpressionChain<TSender, TValue>(item, expression, false, true, false, true);

        /// <summary>
        /// Creates a observable which will subscribe to the each property and sub property
        /// specified in the Expression. eg It will subscribe to x =&gt; x.Property1.Property2.Property3
        /// each property in the lambda expression. It will then provide updates to the last value in the chain.
        /// </summary>
        /// <typeparam name="TValue">The end value we want to subscribe to.</typeparam>
        /// <param name="expression">A expression which will point towards the property.</param>
        /// <param name="beforeChange">If we are interested in notifications before the property value is changed.</param>
        /// <returns>
        /// A observable which notifies about observed changes.
        /// </returns>
        /// <exception cref="InvalidCastException">If we cannot cast from the target value from the specified last property.</exception>
        [RequiresUnreferencedCode("Evaluates expression-based member chains via reflection; members may be trimmed.")]
        [SuppressMessage(
            "Major Code Smell",
            "S4018:Generic methods should provide type parameter",
            Justification = "Generic type parameter is supplied explicitly by the caller by design; it identifies the target type and cannot be inferred from the method's parameters.")]
        public IObservable<IObservedChange<TSender, TValue>> SubscribeToExpressionChain<TValue>(
            Expression? expression,
            bool beforeChange)
            => SubscribeToExpressionChain<TSender, TValue>(item, expression, beforeChange, true, false, true);

        /// <summary>
        /// Creates a observable which will subscribe to the each property and sub property
        /// specified in the Expression. eg It will subscribe to x =&gt; x.Property1.Property2.Property3
        /// each property in the lambda expression. It will then provide updates to the last value in the chain.
        /// </summary>
        /// <typeparam name="TValue">The end value we want to subscribe to.</typeparam>
        /// <param name="expression">A expression which will point towards the property.</param>
        /// <param name="beforeChange">If we are interested in notifications before the property value is changed.</param>
        /// <param name="skipInitial">If we don't want to get a notification about the default value of the property.</param>
        /// <returns>
        /// A observable which notifies about observed changes.
        /// </returns>
        /// <exception cref="InvalidCastException">If we cannot cast from the target value from the specified last property.</exception>
        [RequiresUnreferencedCode("Evaluates expression-based member chains via reflection; members may be trimmed.")]
        [SuppressMessage(
            "Major Code Smell",
            "S4018:Generic methods should provide type parameter",
            Justification = "Generic type parameter is supplied explicitly by the caller by design; it identifies the target type and cannot be inferred from the method's parameters.")]
        public IObservable<IObservedChange<TSender, TValue>> SubscribeToExpressionChain<TValue>(
            Expression? expression,
            bool beforeChange,
            bool skipInitial)
            => SubscribeToExpressionChain<TSender, TValue>(item, expression, beforeChange, skipInitial, false, true);

        /// <summary>
        /// Creates a observable which will subscribe to the each property and sub property
        /// specified in the Expression. eg It will subscribe to x =&gt; x.Property1.Property2.Property3
        /// each property in the lambda expression. It will then provide updates to the last value in the chain.
        /// </summary>
        /// <typeparam name="TValue">The end value we want to subscribe to.</typeparam>
        /// <param name="expression">A expression which will point towards the property.</param>
        /// <param name="beforeChange">If we are interested in notifications before the property value is changed.</param>
        /// <param name="skipInitial">If we don't want to get a notification about the default value of the property.</param>
        /// <param name="suppressWarnings">If true, no warnings should be logged.</param>
        /// <returns>
        /// A observable which notifies about observed changes.
        /// </returns>
        /// <exception cref="InvalidCastException">If we cannot cast from the target value from the specified last property.</exception>
        [RequiresUnreferencedCode("Evaluates expression-based member chains via reflection; members may be trimmed.")]
        [SuppressMessage(
            "Major Code Smell",
            "S4018:Generic methods should provide type parameter",
            Justification = "Generic type parameter is supplied explicitly by the caller by design; it identifies the target type and cannot be inferred from the method's parameters.")]
        public IObservable<IObservedChange<TSender, TValue>> SubscribeToExpressionChain<TValue>(
            Expression? expression,
            bool beforeChange,
            bool skipInitial,
            bool suppressWarnings)
            => SubscribeToExpressionChain<TSender, TValue>(
                item,
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
        /// <typeparam name="TValue">The end value we want to subscribe to.</typeparam>
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
        [SuppressMessage(
            "Major Code Smell",
            "S4018:Generic methods should provide type parameter",
            Justification = "Generic type parameter is supplied explicitly by the caller by design; it identifies the target type and cannot be inferred from the method's parameters.")]
        public IObservable<IObservedChange<TSender, TValue>> SubscribeToExpressionChain<TValue>(
            Expression? expression,
            bool beforeChange,
            bool skipInitial,
            bool suppressWarnings,
            bool isDistinct)
        {
            Expression[] links = [.. Reflection.Rewrite(expression).GetExpressionChain()];
            return new ExpressionChainSink<TSender, TValue>(
                new(
                    item,
                    expression,
                    links,
                    beforeChange,
                    suppressWarnings,
                    skipInitial,
                    isDistinct,
                    NotifyForProperty));
        }
    }

    /// <summary>Provides ObservableForProperty extension members that project observed changes through a selector for reference-type senders.</summary>
    /// <typeparam name="TSender">The sender type.</typeparam>
    /// <param name="item">The source object to observe properties of.</param>
    extension<TSender>(TSender? item)
        where TSender : class
    {
        /// <summary>
        /// ObservableForProperty returns an Observable representing the
        /// property change notifications for a specific property on a
        /// ReactiveObject, running the IObservedChange through a Selector
        /// function.
        /// </summary>
        /// <typeparam name="TValue">The value type.</typeparam>
        /// <typeparam name="TRet">The return value type.</typeparam>
        /// <param name="property">An Expression representing the property (i.e.
        /// 'x => x.SomeProperty'.</param>
        /// <param name="selector">A Select function that will be run on each
        /// item.</param>
        /// <returns>An Observable representing the property change
        /// notifications for the given property.</returns>
        [RequiresUnreferencedCode("Evaluates expression-based member chains via reflection; members may be trimmed.")]
        public IObservable<TRet> ObservableForProperty<TValue, TRet>(
            Expression<Func<TSender, TValue>> property,
            Func<TValue?, TRet> selector)
        {
            ArgumentExceptionHelper.ThrowIfNull(property);
            ArgumentExceptionHelper.ThrowIfNull(selector);

            return new ObservedChangeValueSelector<TSender, TValue, TRet>(item.ObservableForProperty(property, false), selector);
        }

        /// <summary>
        /// ObservableForProperty returns an Observable representing the
        /// property change notifications for a specific property on a
        /// ReactiveObject, running the IObservedChange through a Selector
        /// function.
        /// </summary>
        /// <typeparam name="TValue">The value type.</typeparam>
        /// <typeparam name="TRet">The return value type.</typeparam>
        /// <param name="property">An Expression representing the property (i.e.
        /// 'x => x.SomeProperty'.</param>
        /// <param name="selector">A Select function that will be run on each
        /// item.</param>
        /// <param name="beforeChange">If True, the Observable will notify
        /// immediately before a property is going to change.</param>
        /// <returns>An Observable representing the property change
        /// notifications for the given property.</returns>
        [RequiresUnreferencedCode("Evaluates expression-based member chains via reflection; members may be trimmed.")]
        public IObservable<TRet> ObservableForProperty<TValue, TRet>(
            Expression<Func<TSender, TValue>> property,
            Func<TValue?, TRet> selector,
            bool beforeChange)
        {
            ArgumentExceptionHelper.ThrowIfNull(property);
            ArgumentExceptionHelper.ThrowIfNull(selector);

            return new ObservedChangeValueSelector<TSender, TValue, TRet>(item.ObservableForProperty(property, beforeChange), selector);
        }
    }

    /// <summary>
    /// Creates an observable that signals when a specified property on an object changes, using an expression to
    /// identify the property.
    /// </summary>
    /// <remarks>This method uses reflection to evaluate the property specified by the expression. Members
    /// referenced in the expression may be trimmed when using certain linking or trimming tools, which can affect
    /// runtime behavior. The observable returned emits IObservedChange notifications for the specified
    /// property.</remarks>
    /// <param name="sender">The object whose property changes are to be observed. Cannot be null.</param>
    /// <param name="expression">An expression that identifies the property to observe. Must represent a valid property member.</param>
    /// <param name="beforeChange">true to observe notifications before the property value changes; otherwise, false to observe after the change.</param>
    /// <param name="suppressWarnings">true to suppress warnings related to property observation; otherwise, false.</param>
    /// <returns>An observable sequence that produces notifications when the specified property changes on the sender object.</returns>
    /// <exception cref="ArgumentException">Thrown if expression does not represent a valid property member.</exception>
    /// <exception cref="InvalidOperationException">Thrown if no suitable property change observable factory is found for the specified property and sender type.</exception>
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
                $"Could not find a ICreatesObservableForProperty for {sender.GetType()} property {propertyName}." +
                " This should never happen, your service locator is probably broken. Please make sure you have installed " +
                "the latest version of the ReactiveUI packages for your platform. See https://reactiveui.net/docs/getting-started/installation for guidance."),
            _ => result.GetNotificationForProperty(sender, expression, propertyName, beforeChange, suppressWarnings)
        };
    }
}
