// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using ReactiveUI.Builder;

namespace ReactiveUI;

/// <summary>
/// Provides extension methods for working with observed property changes, enabling retrieval of property names and
/// values from change notifications, and conversion of change streams to value streams.
/// </summary>
/// <remarks>These methods are intended to simplify handling of property change notifications in reactive
/// programming scenarios. They support extracting property names, retrieving current property values, and projecting
/// streams of change notifications into streams of property values. Some methods use reflection to evaluate property
/// expressions, which may have implications for trimming and performance in certain environments.</remarks>
public static class ObservedChangedMixin
{
    /// <summary>
    /// Initializes static members of the <see cref="ObservedChangedMixin"/> class.
    /// </summary>
    static ObservedChangedMixin() => RxAppBuilder.EnsureInitialized();

    /// <summary>
    /// Retrieves the name of the property associated with the observed change.
    /// </summary>
    /// <typeparam name="TSender">The type of the object that raised the change notification.</typeparam>
    /// <typeparam name="TValue">The type of the property value being observed.</typeparam>
    /// <param name="item">The observed change instance from which to extract the property name. Cannot be null.</param>
    /// <returns>A string containing the name of the property associated with the observed change.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="item"/> is null.</exception>
    public static string GetPropertyName<TSender, TValue>(this IObservedChange<TSender, TValue> item) =>
        item is null
            ? throw new ArgumentNullException(nameof(item))
            : Reflection.ExpressionToPropertyNames(item.Expression);

    /// <summary>
    /// Retrieves the current value from the observed change, evaluating the property chain represented by the change
    /// notification.
    /// </summary>
    /// <remarks>This method uses reflection to evaluate the property chain described by the observed change.
    /// If any property in the chain is null, an exception is thrown. Use with caution when members may be trimmed or
    /// unavailable at runtime.</remarks>
    /// <typeparam name="TSender">The type of the object that raised the change notification.</typeparam>
    /// <typeparam name="TValue">The type of the value being observed.</typeparam>
    /// <param name="item">The observed change instance from which to retrieve the value. Cannot be null.</param>
    /// <returns>The value obtained from the observed property chain. The value is of type TValue.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the item parameter is null.</exception>
    /// <exception cref="Exception">Thrown if any property in the observed property chain is null, preventing the value from being retrieved.</exception>
    [RequiresUnreferencedCode("Evaluates expression-based member chains via reflection; members may be trimmed.")]
    public static TValue GetValue<TSender, TValue>(this IObservedChange<TSender, TValue> item) =>
        item is null
            ? throw new ArgumentNullException(nameof(item))
            : !item.TryGetValue(out var returnValue)
                ? throw new Exception($"One of the properties in the expression '{item.GetPropertyName()}' was null")
                : returnValue;

    /// <summary>
    /// Gets the current value from the observed change, or the default value for the type if the value cannot be
    /// retrieved.
    /// </summary>
    /// <typeparam name="TSender">The type of the object that is the source of the change notification.</typeparam>
    /// <typeparam name="TValue">The type of the value being observed.</typeparam>
    /// <param name="item">The observed change instance from which to retrieve the value. Cannot be null.</param>
    /// <returns>The value associated with the observed change if available; otherwise, the default value for the type
    /// <typeparamref name="TValue"/>.</returns>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="item"/> is null.</exception>
    [RequiresUnreferencedCode("Evaluates expression-based member chains via reflection; members may be trimmed.")]
    public static TValue? GetValueOrDefault<TSender, TValue>(this IObservedChange<TSender, TValue> item) => // TODO: Create Test
        item is null ? throw new ArgumentNullException(nameof(item)) : !item.TryGetValue(out var returnValue) ? default : returnValue;

    /// <summary>
    /// Projects each observed change notification to the current value of the observed property or member chain.
    /// </summary>
    /// <remarks>This method uses reflection to evaluate expression-based member chains, which may be affected
    /// by trimming in some deployment scenarios. Use caution when linking against assemblies that may be trimmed, as
    /// required members may be removed.</remarks>
    /// <typeparam name="TSender">The type of the object that owns the property or member being observed.</typeparam>
    /// <typeparam name="TValue">The type of the value being observed and returned.</typeparam>
    /// <param name="item">An observable sequence of change notifications representing changes to a property or member chain.</param>
    /// <returns>An observable sequence that emits the current value of the observed property or member chain each time a change
    /// notification is received.</returns>
    [RequiresUnreferencedCode("Evaluates expression-based member chains via reflection; members may be trimmed.")]
    public static IObservable<TValue> Value<TSender, TValue>(this IObservable<IObservedChange<TSender, TValue>> item) => // TODO: Create Test
        item.Select(GetValue);

    /// <summary>
    /// Attempts to retrieve the value associated with the observed change, using the value directly if available or
    /// evaluating the expression chain if necessary.
    /// </summary>
    /// <remarks>This method may use reflection to evaluate expression-based member chains if the value is not
    /// directly available. Members accessed via reflection may be trimmed during linking, which can affect the ability
    /// to retrieve the value in some scenarios.</remarks>
    /// <typeparam name="TSender">The type of the object that raised the change notification.</typeparam>
    /// <typeparam name="TValue">The type of the value associated with the observed change.</typeparam>
    /// <param name="item">The observed change instance from which to retrieve the value.</param>
    /// <param name="changeValue">When this method returns, contains the value associated with the observed change if retrieval was successful;
    /// otherwise, the default value for the type.</param>
    /// <returns>true if the value was successfully retrieved; otherwise, false.</returns>
    [RequiresUnreferencedCode("Evaluates expression-based member chains via reflection; members may be trimmed.")]
    internal static bool TryGetValue<TSender, TValue>(this IObservedChange<TSender, TValue> item, out TValue changeValue)
    {
        if (!Equals(item.Value, default(TValue)))
        {
            changeValue = item.Value;
            return true;
        }

        return Reflection.TryGetValueForPropertyChain(out changeValue, item.Sender, item.Expression!.GetExpressionChain());
    }

    /// <summary>
    /// Sets the value from the observed change to the specified property on the target object using an expression-based
    /// property chain.
    /// </summary>
    /// <remarks>This method uses reflection to evaluate the property expression and set the value, which may
    /// be affected by trimming in some environments. The method does not throw if the target is null.</remarks>
    /// <typeparam name="TSender">The type of the object that raised the change notification.</typeparam>
    /// <typeparam name="TValue">The type of the value being observed and set.</typeparam>
    /// <typeparam name="TTarget">The type of the target object whose property will be set.</typeparam>
    /// <param name="item">The observed change containing the value to set.</param>
    /// <param name="target">The target object whose property will be updated. If null, no action is taken.</param>
    /// <param name="property">An expression that identifies the property on the target object to set. Must be a simple or nested property
    /// access expression.</param>
    [RequiresUnreferencedCode("Evaluates expression-based member chains via reflection; members may be trimmed.")]
    internal static void SetValueToProperty<TSender, TValue, TTarget>(
        this IObservedChange<TSender, TValue> item,
        TTarget target,
        Expression<Func<TTarget, TValue>> property)
    {
        if (target is not null)
        {
            Reflection.TrySetValueToPropertyChain(target, Reflection.Rewrite(property.Body).GetExpressionChain(), item.GetValue());
        }
    }
}
