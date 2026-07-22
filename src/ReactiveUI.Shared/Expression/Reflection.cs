// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using Splat;

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive;
#else
namespace ReactiveUI;
#endif
/// <summary>Helper class for handling reflection and expression-tree related operations.</summary>
/// <remarks>
/// <para>
/// This type is part of ReactiveUI's infrastructure and is used by binding, observation,
/// and other reflection-heavy code paths.
/// </para>
/// <para>
/// Trimming/AOT note: Some APIs in this type are inherently trimming-unfriendly (e.g. string-based type resolution
/// and expression-driven member traversal). Such APIs are annotated accordingly.
/// </para>
/// </remarks>
[Preserve(AllMembers = true)]
public static partial class Reflection
{
    /// <summary>Error message used when an expression chain is empty.</summary>
    private const string EmptyExpressionChainMessage = "Expression chain must contain at least one element.";

    /// <summary>The maximum number of resolved type-name entries retained by the type cache.</summary>
    private const int TypeCacheSize = 20;

    /// <summary>Cached expression rewriter used to simplify expression trees.</summary>
    /// <remarks>
    /// This instance is cached for performance. It is assumed that <c>ExpressionRewriter</c> is thread-safe for <c>Visit</c>.
    /// If that assumption is invalid, this should be changed to a per-call instance or an object pool.
    /// </remarks>
    private static readonly ExpressionRewriter _expressionRewriter = new();

    /// <summary>Cache for mapping type names to resolved <see cref="Type"/> instances.</summary>
    /// <remarks>
    /// Initialized lazily by <see cref="ReallyFindType"/> to keep trimming-risky initialization inside the RUC boundary.
    /// </remarks>
    private static MemoizingMRUCache<string, Type?>? _typeCache;

    /// <summary>Uses the expression re-writer to simplify the expression down to its simplest expression.</summary>
    /// <param name="expression">The expression to rewrite.</param>
    /// <returns>The rewritten expression, or <see langword="null"/> if <paramref name="expression"/> is <see langword="null"/>.</returns>
    public static Expression Rewrite(Expression? expression) => _expressionRewriter.Visit(expression);

    /// <summary>
    /// Converts an expression that points to a property chain into a dotted path string.
    /// Sub-properties are separated by <c>'.'</c>.
    /// Index-based values include <c>[]</c> after the name, with the index argument values.
    /// </summary>
    /// <param name="expression">The expression to generate the property names from.</param>
    /// <returns>A string representation for the property chain the expression points to.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="expression"/> is <see langword="null"/>.</exception>
    /// <remarks>
    /// This method intentionally follows existing behavior, including the assumption that index arguments are
    /// <see cref="ConstantExpression"/> instances.
    /// </remarks>
    public static string ExpressionToPropertyNames(Expression? expression)
    {
        ArgumentExceptionHelper.ThrowIfNull(expression);

        StringBuilder sb = new();
        var firstSegment = true;

        foreach (var exp in expression.GetExpressionChain())
        {
            if (exp.NodeType == ExpressionType.Parameter)
            {
                continue;
            }

            if (!firstSegment)
            {
                _ = sb.Append('.');
            }

            switch (exp.NodeType)
            {
                case ExpressionType.Index when
                    exp is IndexExpression indexExpression &&
                    indexExpression.Indexer is not null:
                    {
                        _ = sb.Append(indexExpression.Indexer.Name).Append('[');

                        var args = indexExpression.Arguments;
                        for (var i = 0; i < args.Count; i++)
                        {
                            if (i != 0)
                            {
                                _ = sb.Append(',');
                            }

                            _ = sb.Append(((ConstantExpression)args[i]).Value);
                        }

                        _ = sb.Append(']');
                        break;
                    }

                case ExpressionType.MemberAccess when exp is MemberExpression memberExpression:
                    {
                        _ = sb.Append(memberExpression.Member.Name);
                        break;
                    }

                default:
                    {
                        // Other node types contribute no textual segment to the expression path.
                        break;
                    }
            }

            firstSegment = false;
        }

        return sb.ToString();
    }

    /// <summary>Converts a <see cref="MemberInfo"/> into a delegate which fetches the value for the member. Supports fields and properties.</summary>
    /// <param name="member">The member info to convert.</param>
    /// <returns>
    /// A delegate that takes (target, indexArguments) and returns the value; or <see langword="null"/>
    /// when the member is not a field or property.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="member"/> is <see langword="null"/>.</exception>
    /// <remarks>
    /// <para>
    /// For fields, the existing behavior throws <see cref="InvalidOperationException"/> if the field value is <see langword="null"/>.
    /// </para>
    /// <para>
    /// Trimming note: this method does not discover members by name; it operates on an already-resolved <see cref="MemberInfo"/>.
    /// </para>
    /// </remarks>
    public static Func<object?, object?[]?, object?>?
        GetValueFetcherForProperty(MemberInfo? member)
    {
        ArgumentExceptionHelper.ThrowIfNull(member);

        if (member is FieldInfo field)
        {
            return (obj, _) =>
            {
                var value = field.GetValue(obj);
                return value ?? throw new InvalidOperationException();
            };
        }

        return member is not PropertyInfo property ? null : property.GetValue;
    }

    /// <summary>
    /// Converts a <see cref="MemberInfo"/> into a delegate which fetches the value for the member.
    /// Supports fields and properties and throws if the member is not supported.
    /// </summary>
    /// <param name="member">The member info to convert.</param>
    /// <returns>A delegate that takes (target, indexArguments) and returns the value.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="member"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="member"/> is not a field or property.</exception>
    public static Func<object?, object?[]?, object?> GetValueFetcherOrThrow(MemberInfo? member)
    {
        ArgumentExceptionHelper.ThrowIfNull(member);

        var ret = GetValueFetcherForProperty(member);
        return ret ??
               throw new ArgumentException($"Type '{member.DeclaringType}' must have a property '{member.Name}'");
    }

    /// <summary>Converts a <see cref="MemberInfo"/> into a delegate which sets the value for the member. Supports fields and properties.</summary>
    /// <param name="member">The member info to convert.</param>
    /// <returns>
    /// A delegate that takes (target, value, indexArguments) and sets the value; or <see langword="null"/>
    /// when the member is not a field or property.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="member"/> is <see langword="null"/>.</exception>
    /// <remarks>
    /// <para>
    /// This is the <em>soft-fail</em> setter API. It must not throw when the member is unsupported,
    /// because callers (including compiled chain setters) rely on it for <c>shouldThrow == false</c> paths.
    /// </para>
    /// <para>
    /// Trimming note: this method does not discover members by name; it operates on an already-resolved <see cref="MemberInfo"/>.
    /// </para>
    /// </remarks>
    public static Action<object?, object?, object?[]?>?
        GetValueSetterForProperty(MemberInfo? member)
    {
        ArgumentExceptionHelper.ThrowIfNull(member);

        if (member is FieldInfo field)
        {
            return (obj, val, _) => field.SetValue(obj, val);
        }

        return member is not PropertyInfo property ? null : property.SetValue;
    }

    /// <summary>
    /// Converts a <see cref="MemberInfo"/> into a delegate which sets the value for the member.
    /// Supports fields and properties and throws if the member is not supported.
    /// </summary>
    /// <param name="member">The member info to convert.</param>
    /// <returns>A delegate that takes (target, value, indexArguments) and sets the value.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="member"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">Thrown when <paramref name="member"/> is not a field or property.</exception>
    public static Action<object?, object?, object?[]?> GetValueSetterOrThrow(MemberInfo? member)
    {
        ArgumentExceptionHelper.ThrowIfNull(member);

        var ret = GetValueSetterForProperty(member);
        return ret ??
               throw new ArgumentException($"Type '{member.DeclaringType}' must have a property '{member.Name}'");
    }

    /// <summary>Based on a list of expressions, attempts to get the value of the last property in the chain.</summary>
    /// <typeparam name="TValue">The expected type of the final value.</typeparam>
    /// <param name="changeValue">Receives the value if the chain can be evaluated.</param>
    /// <param name="current">The object that starts the property chain.</param>
    /// <param name="expressionChain">A sequence of expressions that point to properties/fields.</param>
    /// <returns><see langword="true"/> if the value was successfully retrieved; otherwise <see langword="false"/>.</returns>
    /// <exception cref="InvalidOperationException">Thrown when <paramref name="expressionChain"/> is empty.</exception>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="expressionChain"/> is <see langword="null"/>.</exception>
    /// <remarks>
    /// Trimming note: this method may traverse arbitrary member chains represented by expressions; it is not possible
    /// to express a complete trimming contract locally.
    /// </remarks>
    [RequiresUnreferencedCode("Evaluates expression-based member chains via reflection; members may be trimmed.")]
    public static bool TryGetValueForPropertyChain<TValue>(
        out TValue changeValue,
        object? current,
        IEnumerable<Expression> expressionChain)
    {
        var expressions = MaterializeExpressions(expressionChain);
        var count = expressions.Length;

        if (count == 0)
        {
            throw new InvalidOperationException(EmptyExpressionChainMessage);
        }

        for (var i = 0; i < count - 1; i++)
        {
            if (current is null)
            {
                changeValue = default!;
                return false;
            }

            var expression = expressions[i];
            current = GetValueFetcherOrThrow(expression.GetMemberInfo())(current, expression.GetArgumentsArray());
        }

        if (current is null)
        {
            changeValue = default!;
            return false;
        }

        var lastExpression = expressions[count - 1];
        changeValue =
            (TValue)GetValueFetcherOrThrow(lastExpression.GetMemberInfo())(
                current,
                lastExpression.GetArgumentsArray())!;
        return true;
    }

    /// <summary>Based on a list of expressions, attempts to produce an array of <see cref="IObservedChange{TSender, TValue}"/> values representing each step in the property chain.</summary>
    /// <param name="changeValues">Receives an array with one entry per expression in the chain.</param>
    /// <param name="current">The object that starts the property chain.</param>
    /// <param name="expressionChain">A sequence of expressions that point to properties/fields.</param>
    /// <returns><see langword="true"/> if all values were successfully retrieved; otherwise <see langword="false"/>.</returns>
    /// <exception cref="InvalidOperationException">Thrown when <paramref name="expressionChain"/> is empty.</exception>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="expressionChain"/> is <see langword="null"/>.</exception>
    /// <remarks>
    /// This preserves the existing behavior: on early failure, the method writes a single <see langword="null"/>
    /// element at the failing index and returns <see langword="false"/>.
    /// </remarks>
    [RequiresUnreferencedCode("Evaluates expression-based member chains via reflection; members may be trimmed.")]
    public static bool TryGetAllValuesForPropertyChain(
        out IObservedChange<object, object?>[] changeValues,
        object? current,
        IEnumerable<Expression> expressionChain)
    {
        var expressions = MaterializeExpressions(expressionChain);
        var count = expressions.Length;

        changeValues = new IObservedChange<object, object?>[count];

        if (count == 0)
        {
            throw new InvalidOperationException(EmptyExpressionChainMessage);
        }

        var currentIndex = 0;

        for (; currentIndex < count - 1; currentIndex++)
        {
            if (current is null)
            {
                changeValues[currentIndex] = null!;
                return false;
            }

            var expression = expressions[currentIndex];
            var sender = current;
            current = GetValueFetcherOrThrow(expression.GetMemberInfo())(current, expression.GetArgumentsArray());
            changeValues[currentIndex] = new ObservedChange<object, object?>(sender, expression, current);
        }

        if (current is null)
        {
            changeValues[currentIndex] = null!;
            return false;
        }

        var lastExpression = expressions[count - 1];
        changeValues[currentIndex] = new ObservedChange<object, object?>(
            current,
            lastExpression,
            GetValueFetcherOrThrow(lastExpression.GetMemberInfo())(current, lastExpression.GetArgumentsArray()));

        return true;
    }

    /// <summary>
    /// Based on a list of expressions, attempts to set the value of the last property in the chain, throwing on missing members.
    /// </summary>
    /// <typeparam name="TValue">The type of the end value being set.</typeparam>
    /// <param name="target">The object that starts the property chain.</param>
    /// <param name="expressionChain">A sequence of expressions that point to properties/fields.</param>
    /// <param name="value">The value to set on the last property in the chain.</param>
    /// <returns>True if the value was successfully set; otherwise false.</returns>
    /// <exception cref="InvalidOperationException">Thrown when expressionChain is empty.</exception>
    /// <exception cref="ArgumentNullException">Thrown when target is null and traversal is required.</exception>
    /// <exception cref="ArgumentException">Thrown when a required member is not settable.</exception>
    [RequiresUnreferencedCode("Evaluates expression-based member chains via reflection; members may be trimmed.")]
    public static bool TrySetValueToPropertyChain<TValue>(
        object? target,
        IEnumerable<Expression> expressionChain,
        TValue value) =>
        TrySetValueToPropertyChain(target, expressionChain, value, true);

    /// <summary>Based on a list of expressions, attempts to set the value of the last property in the chain.</summary>
    /// <typeparam name="TValue">The type of the end value being set.</typeparam>
    /// <param name="target">The object that starts the property chain.</param>
    /// <param name="expressionChain">A sequence of expressions that point to properties/fields.</param>
    /// <param name="value">The value to set on the last property in the chain.</param>
    /// <param name="shouldThrow">
    /// If true, throw when reflection members are missing; otherwise fail softly.
    /// </param>
    /// <returns>True if the value was successfully set; otherwise false.</returns>
    /// <exception cref="InvalidOperationException">Thrown when expressionChain is empty.</exception>
    /// <exception cref="ArgumentNullException">
    /// Thrown when target is null and traversal is required.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// Thrown when shouldThrow is true and a required member is not settable.
    /// </exception>
    /// <remarks>
    /// Trimming note: this method may traverse arbitrary member chains represented by expressions; it is not possible
    /// to express a complete trimming contract locally.
    /// </remarks>
    [RequiresUnreferencedCode("Evaluates expression-based member chains via reflection; members may be trimmed.")]
    public static bool TrySetValueToPropertyChain<TValue>(
        object? target,
        IEnumerable<Expression> expressionChain,
        TValue value,
        bool shouldThrow)
    {
        var expressions = MaterializeExpressions(expressionChain);
        var count = expressions.Length;

        if (count == 0)
        {
            throw new InvalidOperationException(EmptyExpressionChainMessage);
        }

        for (var i = 0; i < count - 1; i++)
        {
            var expression = expressions[i];

            var getter = shouldThrow
                ? GetValueFetcherOrThrow(expression.GetMemberInfo())
                : GetValueFetcherForProperty(expression.GetMemberInfo());

            if (getter is not null)
            {
                target = getter(
                    target ?? throw new ArgumentNullException(nameof(target)),
                    expression.GetArgumentsArray());
            }
        }

        if (target is null)
        {
            return false;
        }

        var lastExpression = expressions[count - 1];

        var setter = shouldThrow
            ? GetValueSetterOrThrow(lastExpression.GetMemberInfo())
            : GetValueSetterForProperty(lastExpression.GetMemberInfo());

        if (setter is null)
        {
            return false;
        }

        setter(target, value, lastExpression.GetArgumentsArray());
        return true;
    }

    /// <summary>Gets a <see cref="Type"/> from the specified type name, using a cache to avoid repeated reflection.</summary>
    /// <param name="type">The name of the type.</param>
    /// <param name="throwOnFailure">If <see langword="true"/>, throw when the type cannot be found.</param>
    /// <returns>
    /// The resolved <see cref="Type"/>, or <see langword="null"/> if not found and <paramref name="throwOnFailure"/> is <see langword="false"/>.
    /// </returns>
    /// <exception cref="TypeLoadException">
    /// Thrown when the type cannot be found and <paramref name="throwOnFailure"/> is <see langword="true"/>.
    /// </exception>
    /// <remarks>
    /// Trimming note: resolving types by string name is inherently trimming-unfriendly unless additional metadata is preserved externally.
    /// </remarks>
    [RequiresUnreferencedCode("Resolves types by name and loads assemblies; types may be trimmed.")]
    public static Type? ReallyFindType(string? type, bool throwOnFailure)
    {
        var cache = Volatile.Read(ref _typeCache);
        if (cache is null)
        {
            MemoizingMRUCache<string, Type?> created = new(
                static (typeName, _) => GetTypeHelper(typeName),
                TypeCacheSize);

            cache = Interlocked.CompareExchange(ref _typeCache, created, null) ?? created;
        }

        var ret = cache.Get(type ?? string.Empty);
        return ret is not null || !throwOnFailure ? ret : throw new TypeLoadException();
    }

    /// <summary>Gets the appropriate <see cref="EventArgs"/>-derived type for the specified event name on a <see cref="Type"/>.</summary>
    /// <param name="type">The type of object to find the event on. Must preserve public events under trimming.</param>
    /// <param name="eventName">The name of the event.</param>
    /// <returns>The type of the event args used by the event handler.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="type"/> is <see langword="null"/>.</exception>
    /// <exception cref="Exception">Thrown if there is no event matching the name on the target type.</exception>
    /// <exception cref="MissingMethodException">Thrown if the event handler type does not expose an <c>Invoke</c> method.</exception>
    /// <remarks>
    /// Trimming note: the event handler type is obtained from <see cref="EventInfo.EventHandlerType"/>, which does not carry
    /// <see cref="DynamicallyAccessedMembersAttribute"/> annotations. This prevents expressing a complete trimming contract here.
    /// </remarks>
    [RequiresUnreferencedCode("Reflects over custom delegate Invoke signature; members may be trimmed.")]
    public static Type GetEventArgsTypeForEvent(
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
        Type type,
        string? eventName)
    {
        ArgumentExceptionHelper.ThrowIfNull(type);

        var eventInfo = type.GetRuntimeEvent(eventName!);
        if (eventInfo is null || eventInfo.EventHandlerType is null)
        {
            throw new InvalidOperationException($"Couldn't find {type.FullName}.{eventName}");
        }

        var invoke = eventInfo.EventHandlerType.GetMethod("Invoke") ??
                     throw new MissingMethodException(eventInfo.EventHandlerType.FullName, "Invoke");
        var parameters = invoke.GetParameters();
        return parameters[1].ParameterType;
    }

    /// <summary>Checks to make sure that the specified method names on the target type are overridden.</summary>
    /// <param name="callingTypeName">The name of the calling type.</param>
    /// <param name="targetType">The type to check. Must preserve public and non-public methods under trimming.</param>
    /// <param name="methodsToCheck">The method names to check.</param>
    /// <exception cref="Exception">Thrown if any method is not overridden on the target type.</exception>
    /// <remarks>
    /// Trimming note: this method inspects declared method names; the <paramref name="targetType"/> parameter is annotated accordingly.
    /// </remarks>
    public static void ThrowIfMethodsNotOverloaded(
        string callingTypeName,
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods |
                                    DynamicallyAccessedMemberTypes.NonPublicMethods)]
        Type targetType,
        params string[] methodsToCheck)
    {
        ArgumentExceptionHelper.ThrowIfNull(methodsToCheck);

        var methods = targetType.GetTypeInfo().DeclaredMethods;

        for (var i = 0; i < methodsToCheck.Length; i++)
        {
            var name = methodsToCheck[i];
            MethodInfo? found = null;

            foreach (var m in methods)
            {
                if (string.Equals(m.Name, name, StringComparison.Ordinal))
                {
                    found = m;
                    break;
                }
            }

            if (found is null)
            {
                throw new InvalidOperationException($"Your class must implement {name} and call {callingTypeName}.{name}");
            }
        }
    }

    /// <summary>Checks to make sure that the specified method names on the target object are overridden.</summary>
    /// <param name="callingTypeName">The name of the calling type.</param>
    /// <param name="targetObject">The object to check.</param>
    /// <param name="methodsToCheck">The method names to check.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="targetObject"/> is <see langword="null"/>.</exception>
    /// <exception cref="Exception">Thrown if any method is not overridden on the target object.</exception>
    /// <remarks>
    /// Trimming note: the runtime type is discovered dynamically via <see cref="object.GetType"/>, so this method
    /// cannot express a complete trimming contract locally.
    /// </remarks>
    [RequiresUnreferencedCode("Inspects declared methods on a runtime type; members may be trimmed.")]
    public static void ThrowIfMethodsNotOverloaded(
        string callingTypeName,
        object targetObject,
        params string[] methodsToCheck)
    {
        ArgumentExceptionHelper.ThrowIfNull(targetObject);
        ArgumentExceptionHelper.ThrowIfNull(methodsToCheck);

        var methods = targetObject.GetType().GetTypeInfo().DeclaredMethods;

        for (var i = 0; i < methodsToCheck.Length; i++)
        {
            var name = methodsToCheck[i];
            MethodInfo? found = null;

            foreach (var m in methods)
            {
                if (string.Equals(m.Name, name, StringComparison.Ordinal))
                {
                    found = m;
                    break;
                }
            }

            if (found is null)
            {
                throw new InvalidOperationException(
                    $"Your class must implement {name} and call {callingTypeName}.{name}");
            }
        }
    }

    /// <summary>Creates an observable that switches to observing the provided expression on the current ViewModel.</summary>
    /// <typeparam name="TView">The view type.</typeparam>
    /// <typeparam name="TViewModel">The view model type.</typeparam>
    /// <param name="viewModel">The current view model (not used directly; preserved for signature compatibility).</param>
    /// <param name="view">The view instance.</param>
    /// <param name="expression">The expression to observe dynamically.</param>
    /// <returns>An observable that emits values produced by the dynamic observation.</returns>
    /// <remarks>
    /// Trimming note: dynamic observation via expression trees typically requires reflection over members
    /// that may be trimmed; callers should preserve metadata for observed members.
    /// </remarks>
    [RequiresUnreferencedCode("Dynamic observation uses reflection over members that may be trimmed.")]
    public static IObservable<object> ViewModelWhenAnyValue<TView, TViewModel>(
        TViewModel? viewModel,
        TView view,
        Expression? expression)
        where TView : class, IViewFor
        where TViewModel : class =>
        view.WhenAnyValue(x => x.ViewModel)
            .SwitchSelect(x => ((TViewModel?)x).WhenAnyDynamic(expression, static y => y.Value))!;

    /// <summary>
    /// Attempts to resolve a type name using <see cref="Type.GetType(string, Func{AssemblyName, Assembly?}?, Func{Assembly, string, bool, Type?}?, bool)"/>
    /// with custom assembly resolution that first searches loaded assemblies and then tries to load by name.
    /// </summary>
    /// <param name="type">The type name.</param>
    /// <returns>The resolved type or <see langword="null"/>.</returns>
    /// <remarks>
    /// Trimming note: this is string-based type resolution and may fail under trimming without explicit preservation.
    /// </remarks>
    [RequiresUnreferencedCode("Resolves types by name and loads assemblies; types may be trimmed.")]
    private static Type? GetTypeHelper(string type) =>
        Type.GetType(
            type,
            static assemblyName =>
            {
                var assemblies = AppDomain.CurrentDomain.GetAssemblies();
                for (var i = 0; i < assemblies.Length; i++)
                {
                    var a = assemblies[i];
                    if (a.FullName == assemblyName.FullName)
                    {
                        return a;
                    }
                }

                try
                {
                    return Assembly.Load(assemblyName);
                }
                catch
                {
                    return null;
                }
            },
            null,
            false);

    /// <summary>Materializes an expression chain into an array to enable index-based iteration without LINQ.</summary>
    /// <param name="expressionChain">The expression chain to materialize.</param>
    /// <returns>An array containing the expressions in enumeration order.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="expressionChain"/> is <see langword="null"/>.</exception>
    /// <remarks>
    /// This helper is used to reduce allocations and virtual dispatch in hot paths by enabling for-loop iteration.
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static Expression[] MaterializeExpressions(IEnumerable<Expression> expressionChain)
    {
        ArgumentExceptionHelper.ThrowIfNull(expressionChain);

        switch (expressionChain)
        {
            case Expression[] arr:
                return arr;
            case ICollection<Expression> coll:
                {
                    if (coll.Count == 0)
                    {
                        return [];
                    }

                    var result = new Expression[coll.Count];
                    coll.CopyTo(result, 0);
                    return result;
                }

            default:
                return [.. expressionChain];
        }
    }
}
