// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace ReactiveUI;

/// <summary>
/// Provides methods to bind properties to observables.
/// </summary>
/// <remarks>
/// <para>
/// This implementation uses dynamic expression parsing and reflection to support arbitrary property chains and dynamic
/// observation (<c>WhenAnyDynamic</c>). As such, it is not trimming- or AOT-friendly without additional preservation.
/// </para>
/// <para>
/// Trimming/AOT: this type is annotated because it performs reflection over runtime types and expression graphs that may
/// be trimmed, and may invoke members that are annotated for dynamic code.
/// </para>
/// </remarks>
[RequiresUnreferencedCode("Uses reflection over runtime types and expression graphs which may be trimmed.")]
[RequiresDynamicCode("Uses dynamic binding paths which may require runtime code generation or reflection-based invocation.")]
public class PropertyBinderImplementation : IPropertyBinderImplementation
{
    /// <summary>
    /// Caches the best set-method converter for a given (<c>fromType</c>, <c>toType</c>) pair.
    /// </summary>
    /// <remarks>
    /// The cached value is the selected <see cref="ISetMethodBindingConverter"/> instance, or <see langword="null"/> if none matches.
    /// </remarks>
    private static readonly System.Collections.Concurrent.ConcurrentDictionary<(Type fromType, Type? toType), ISetMethodBindingConverter?> _setMethodCache = new();

    /// <summary>
    /// Initializes static members of the <see cref="PropertyBinderImplementation"/> class.
    /// Ensures ReactiveUI static initialization is performed before bindings are used.
    /// </summary>

    /// <summary>
    /// Represents a converter that attempts conversion and returns success via an <see langword="out"/> parameter.
    /// </summary>
    /// <typeparam name="T1">The input value type.</typeparam>
    /// <typeparam name="T2">The output value type.</typeparam>
    /// <param name="t1">The input value.</param>
    /// <param name="t2">The converted output value.</param>
    /// <returns><see langword="true"/> if conversion succeeded; otherwise <see langword="false"/>.</returns>
    private delegate bool OutFunc<in T1, T2>(T1 t1, out T2 t2);

    /// <inheritdoc />
    public IReactiveBinding<TView, (object? view, bool isViewModel)> Bind<TViewModel, TView, TVMProp, TVProp, TDontCare>(
        TViewModel? viewModel,
        TView view,
        Expression<Func<TViewModel, TVMProp?>> vmProperty,
        Expression<Func<TView, TVProp>> viewProperty,
        IObservable<TDontCare>? signalViewUpdate,
        object? conversionHint,
        IBindingTypeConverter? vmToViewConverterOverride = null,
        IBindingTypeConverter? viewToVMConverterOverride = null,
        TriggerUpdate triggerUpdate = TriggerUpdate.ViewToViewModel)
        where TViewModel : class
        where TView : class, IViewFor
    {
        ArgumentExceptionHelper.ThrowIfNull(vmProperty);
        ArgumentExceptionHelper.ThrowIfNull(viewProperty);

        // Converter selection is a hot-path concern across many binds; keep it cached and loop-based.
        var vmToViewConverterObj = vmToViewConverterOverride ?? GetConverterForTypes(typeof(TVMProp), typeof(TVProp));
        var viewToVMConverterObj = viewToVMConverterOverride ?? GetConverterForTypes(typeof(TVProp), typeof(TVMProp));

        if (vmToViewConverterObj is null || viewToVMConverterObj is null)
        {
            throw new ArgumentException(
                $"Can't two-way convert between {typeof(TVMProp)} and {typeof(TVProp)}. To fix this, register a IBindingTypeConverter or call the version with the converter Func.");
        }

        bool VmToViewFunc(TVMProp? vmValue, out TVProp vValue)
        {
            var result = BindingTypeConverterDispatch.TryConvertAny(
                vmToViewConverterObj,
                typeof(TVMProp),
                vmValue,
                typeof(TVProp),
                conversionHint,
                out var tmp);

            vValue = result && tmp is not null ? (TVProp)tmp : default!;
            return result;
        }

        bool ViewToVmFunc(TVProp vValue, out TVMProp? vmValue)
        {
            var result = BindingTypeConverterDispatch.TryConvertAny(
                viewToVMConverterObj,
                typeof(TVProp),
                vValue,
                typeof(TVMProp?),
                conversionHint,
                out var tmp);

            vmValue = result && tmp is not null ? (TVMProp?)tmp : default;
            return result;
        }

        return BindImpl(viewModel, view, vmProperty, viewProperty, signalViewUpdate, VmToViewFunc, ViewToVmFunc, triggerUpdate);
    }

    /// <inheritdoc />
    public IReactiveBinding<TView, (object? view, bool isViewModel)> Bind<TViewModel, TView, TVMProp, TVProp, TDontCare>(
        TViewModel? viewModel,
        TView view,
        Expression<Func<TViewModel, TVMProp?>> vmProperty,
        Expression<Func<TView, TVProp>> viewProperty,
        IObservable<TDontCare>? signalViewUpdate,
        Func<TVMProp?, TVProp> vmToViewConverter,
        Func<TVProp, TVMProp?> viewToVmConverter,
        TriggerUpdate triggerUpdate = TriggerUpdate.ViewToViewModel)
        where TViewModel : class
        where TView : class, IViewFor
    {
        ArgumentExceptionHelper.ThrowIfNull(vmProperty);
        ArgumentExceptionHelper.ThrowIfNull(viewProperty);
        ArgumentExceptionHelper.ThrowIfNull(vmToViewConverter);
        ArgumentExceptionHelper.ThrowIfNull(viewToVmConverter);

        bool VmToViewFunc(TVMProp? vmValue, out TVProp vValue)
        {
            vValue = vmToViewConverter(vmValue);
            return true;
        }

        bool ViewToVmFunc(TVProp vValue, out TVMProp? vmValue)
        {
            vmValue = viewToVmConverter(vValue);
            return true;
        }

        return BindImpl(viewModel, view, vmProperty, viewProperty, signalViewUpdate, VmToViewFunc, ViewToVmFunc, triggerUpdate);
    }

    /// <inheritdoc />
    public IReactiveBinding<TView, TVProp> OneWayBind<TViewModel, TView, TVMProp, TVProp>(
        TViewModel? viewModel,
        TView view,
        Expression<Func<TViewModel, TVMProp?>> vmProperty,
        Expression<Func<TView, TVProp>> viewProperty,
        object? conversionHint = null,
        IBindingTypeConverter? vmToViewConverterOverride = null)
        where TViewModel : class
        where TView : class, IViewFor
    {
        ArgumentExceptionHelper.ThrowIfNull(vmProperty);
        ArgumentExceptionHelper.ThrowIfNull(viewProperty);

        var vmExpression = Reflection.Rewrite(vmProperty.Body);
        var viewExpression = Reflection.Rewrite(viewProperty.Body);
        var viewType = viewExpression.Type;

        var converterObj =
            (vmToViewConverterOverride ?? GetConverterForTypes(typeof(TVMProp?), viewType)) ??
            throw new ArgumentException($"Can't convert {typeof(TVMProp)} to {viewType}. To fix this, register a IBindingTypeConverter");

        var ret = EvalBindingHooks(viewModel, view, vmExpression, viewExpression, BindingDirection.OneWay);
        if (!ret)
        {
            return new ReactiveBinding<TView, TVProp>(view, viewExpression, vmExpression, Observable.Empty<TVProp>(), BindingDirection.OneWay, Disposable.Empty);
        }

        var source =
            Reflection.ViewModelWhenAnyValue(viewModel, view, vmExpression)
                .SelectMany(x =>
                    !BindingTypeConverterDispatch.TryConvertAny(
                        converterObj,
                        x?.GetType() ?? typeof(object),
                        x,
                        viewType,
                        conversionHint,
                        out var tmp)
                        ? Observable<object>.Empty
                        : Observable.Return(tmp));

        var (disposable, obs) = BindToDirect<TView, TVProp, object?>(source, view, viewExpression);

        return new ReactiveBinding<TView, TVProp>(view, viewExpression, vmExpression, obs, BindingDirection.OneWay, disposable);
    }

    /// <inheritdoc />
    public IReactiveBinding<TView, TOut> OneWayBind<TViewModel, TView, TProp, TOut>(
        TViewModel? viewModel,
        TView view,
        Expression<Func<TViewModel, TProp>> vmProperty,
        Expression<Func<TView, TOut>> viewProperty,
        Func<TProp, TOut> selector)
        where TViewModel : class
        where TView : class, IViewFor
    {
        ArgumentExceptionHelper.ThrowIfNull(vmProperty);
        ArgumentExceptionHelper.ThrowIfNull(viewProperty);

        var vmExpression = Reflection.Rewrite(vmProperty.Body);
        var viewExpression = Reflection.Rewrite(viewProperty.Body);

        var ret = EvalBindingHooks(viewModel, view, vmExpression, viewExpression, BindingDirection.OneWay);
        if (!ret)
        {
            return new ReactiveBinding<TView, TOut>(view, viewExpression, vmExpression, Observable.Empty<TOut>(), BindingDirection.OneWay, Disposable.Empty);
        }

        var source = Reflection.ViewModelWhenAnyValue(viewModel, view, vmExpression).Cast<TProp>().Select(selector);

        var (disposable, obs) = BindToDirect<TView, TOut, TOut>(source, view, viewExpression);

        return new ReactiveBinding<TView, TOut>(view, viewExpression, vmExpression, obs, BindingDirection.OneWay, disposable);
    }

    /// <inheritdoc />
    public IDisposable BindTo<TValue, TTarget, TTValue>(
        IObservable<TValue> observedChange,
        TTarget? target,
        Expression<Func<TTarget, TTValue?>> propertyExpression,
        object? conversionHint = null,
        IBindingTypeConverter? vmToViewConverterOverride = null)
        where TTarget : class
    {
        ArgumentExceptionHelper.ThrowIfNull(target);
        ArgumentExceptionHelper.ThrowIfNull(propertyExpression);

        var viewExpression = Reflection.Rewrite(propertyExpression.Body);

        var shouldBind = target is not IViewFor viewFor || EvalBindingHooks<object, IViewFor>(null, viewFor, null!, viewExpression, BindingDirection.OneWay);
        if (!shouldBind)
        {
            return Disposable.Empty;
        }

        var converterObj =
            (vmToViewConverterOverride ?? GetConverterForTypes(typeof(TValue), typeof(TTValue?))) ??
            throw new ArgumentException($"Can't convert {typeof(TValue)} to {typeof(TTValue)}. To fix this, register a IBindingTypeConverter");

        var source =
            observedChange.SelectMany(x =>
                !BindingTypeConverterDispatch.TryConvertAny(
                    converterObj,
                    typeof(TValue),
                    x,
                    typeof(TTValue?),
                    conversionHint,
                    out var tmp)
                    ? Observable<object>.Empty
                    : Observable.Return(tmp));

        var (disposable, _) = BindToDirect<TTarget, TTValue?, object?>(source, target!, viewExpression);

        return disposable;
    }

    /// <summary>
    /// Gets a converter for the specified type pair.
    /// </summary>
    /// <param name="lhs">The source type.</param>
    /// <param name="rhs">The target type.</param>
    /// <returns>
    /// A converter instance (either <see cref="IBindingTypeConverter"/> or <see cref="IBindingFallbackConverter"/>)
    /// if one is registered for the specified types; otherwise, <see langword="null"/>.
    /// </returns>
    /// <remarks>
    /// <para>
    /// Typed converters (exact pair match) are preferred over fallback converters.
    /// </para>
    /// <para>
    /// This method uses <see cref="RxConverters.Current"/> which provides lock-free converter lookup
    /// with built-in affinity-based selection. No external caching is needed.
    /// </para>
    /// </remarks>
    internal static object? GetConverterForTypes(Type lhs, Type rhs) =>
        ResolveBestConverter(lhs, rhs);

    /// <summary>
    /// Resolves the best converter for a given type pair using the ConverterService.
    /// </summary>
    /// <param name="fromType">The source type.</param>
    /// <param name="toType">The target type.</param>
    /// <returns>
    /// The selected converter (typed preferred), or <see langword="null"/> if none matches.
    /// </returns>
    /// <remarks>
    /// <para>
    /// This method first attempts to use <see cref="RxConverters.Current"/> for lock-free converter resolution.
    /// If no ConverterService is available (legacy initialization), it falls back to Splat-based resolution.
    /// </para>
    /// <para>
    /// The ConverterService provides:
    /// <list type="bullet">
    /// <item><description>Lock-free reads via snapshot pattern</description></item>
    /// <item><description>Built-in affinity-based selection (highest wins)</description></item>
    /// <item><description>Two-phase resolution: typed converters first, then fallback converters</description></item>
    /// </list>
    /// </para>
    /// </remarks>
    private static object? ResolveBestConverter(Type fromType, Type toType)
    {
        // Try to use the new ConverterService first (lock-free, optimized)
        try
        {
            var converter = RxConverters.Current.ResolveConverter(fromType, toType);
            if (converter is not null)
            {
                return converter;
            }
        }
        catch
        {
            // ConverterService not available, fall back to Splat
        }

        // Fallback to Splat-based resolution for backward compatibility
        // Phase 1: exact-pair typed converters by affinity.
        var typed = AppLocator.Current.GetServices<IBindingTypeConverter>();
        var bestTypedScore = -1;
        IBindingTypeConverter? bestTyped = null;

        foreach (var c in typed)
        {
            if (c is null || c.FromType != fromType || c.ToType != toType)
            {
                continue;
            }

            var score = c.GetAffinityForObjects();
            if (score > bestTypedScore && score > 0)
            {
                bestTypedScore = score;
                bestTyped = c;
            }
        }

        if (bestTyped is not null)
        {
            return bestTyped;
        }

        // Phase 2: fallback converters by affinity.
        var fallbacks = AppLocator.Current.GetServices<IBindingFallbackConverter>();
        var bestFallbackScore = -1;
        IBindingFallbackConverter? bestFallback = null;

        foreach (var c in fallbacks)
        {
            if (c is null)
            {
                continue;
            }

            var score = c.GetAffinityForObjects(fromType, toType);
            if (score > bestFallbackScore && score > 0)
            {
                bestFallbackScore = score;
                bestFallback = c;
            }
        }

        return bestFallback;
    }

    /// <summary>
    /// Converts an expression chain to a materialized array using collection expression syntax.
    /// </summary>
    /// <param name="expression">The expression whose chain should be materialized.</param>
    /// <returns>
    /// An array of expressions representing the chain, or <see langword="null"/> when the chain cannot be obtained.
    /// </returns>
    private static Expression[]? GetExpressionChainArrayOrNull(Expression? expression) =>
        expression is null ? null : [.. expression.GetExpressionChain()];

    /// <summary>
    /// Creates the default value instance for <paramref name="type"/> used by the "replay on host changes" logic.
    /// </summary>
    /// <param name="type">The member type.</param>
    /// <returns>
    /// A boxed default value for value types, or <see langword="null"/> for reference types.
    /// </returns>
    private static object? CreateDefaultValueForType(Type type)
    {
        ArgumentExceptionHelper.ThrowIfNull(type);

        return type.GetTypeInfo().IsValueType ? Activator.CreateInstance(type) : null;
    }

    /// <summary>
    /// Determines whether values should be replayed when the host changes.
    /// </summary>
    /// <param name="hostExpressionChain">The host expression chain.</param>
    /// <returns>
    /// <see langword="true"/> when replay-on-host-change behavior should be enabled; otherwise <see langword="false"/>.
    /// </returns>
    /// <remarks>
    /// This preserves the original behavior: when the chain includes <c>IViewFor.ViewModel</c>, replay is disabled.
    /// </remarks>
    private static bool ShouldReplayOnHostChanges(Expression[]? hostExpressionChain)
    {
        if (hostExpressionChain is null)
        {
            return true;
        }

        for (var i = 0; i < hostExpressionChain.Length; i++)
        {
            if (hostExpressionChain[i] is MemberExpression member &&
                string.Equals(member.Member.Name, nameof(IViewFor.ViewModel), StringComparison.Ordinal))
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Resolves the current host object for the binding by evaluating the host expression chain.
    /// </summary>
    /// <param name="target">The root binding target.</param>
    /// <param name="hostExpressionChain">The expression chain used to compute the host.</param>
    /// <returns>
    /// The resolved host object, or <see langword="null"/> if the chain cannot be evaluated.
    /// </returns>
    private static object? ResolveHostFromChainOrNull(object target, Expression[] hostExpressionChain)
    {
        ArgumentExceptionHelper.ThrowIfNull(target);
        ArgumentExceptionHelper.ThrowIfNull(hostExpressionChain);

        object? host = target;

        if (!Reflection.TryGetValueForPropertyChain(out host, host, hostExpressionChain))
        {
            return null;
        }

        return host;
    }

    /// <summary>
    /// Gets the set-method conversion shim for an assignment into a target member.
    /// </summary>
    /// <param name="fromType">The runtime type of the inbound value.</param>
    /// <param name="targetType">The target member type.</param>
    /// <returns>
    /// A conversion function used by set-method converters, or <see langword="null"/> if no converter is applicable.
    /// </returns>
    private static Func<object?, object?, object?[]?, object?>? GetSetConverter(Type? fromType, Type? targetType)
    {
        if (fromType is null)
        {
            return null;
        }

        var converter = _setMethodCache.GetOrAdd(
            (fromType, targetType),
            static key => ResolveBestSetMethodConverter(key.fromType, key.toType));

        if (converter is null)
        {
            return null;
        }

        // Adapt the converter's contract to the local call shape expected by SetThenGet.
        // Note: do not store this delegate in the cache; the cache stores the converter instance.
        return (currentValue, newValue, indexParameters) => converter.PerformSet(currentValue, newValue, indexParameters);
    }

    /// <summary>
    /// Resolves the best <see cref="ISetMethodBindingConverter"/> for a given pair.
    /// </summary>
    /// <param name="fromType">The inbound runtime type.</param>
    /// <param name="toType">The target type.</param>
    /// <returns>The selected converter, or <see langword="null"/> if none matches.</returns>
    private static ISetMethodBindingConverter? ResolveBestSetMethodConverter(Type fromType, Type? toType)
    {
        var converters = AppLocator.Current.GetServices<ISetMethodBindingConverter>();

        var bestScore = -1;
        ISetMethodBindingConverter? best = null;

        foreach (var c in converters)
        {
            if (c is null)
            {
                continue;
            }

            var score = c.GetAffinityForObjects(fromType, toType);
            if (score > bestScore && score > 0)
            {
                bestScore = score;
                best = c;
            }
        }

        return best;
    }

    /// <summary>
    /// Determines whether <paramref name="viewExpression"/> is a direct member access on the root parameter.
    /// </summary>
    /// <param name="viewExpression">The view expression to inspect.</param>
    /// <returns>
    /// <see langword="true"/> if the member is directly on the root parameter; otherwise <see langword="false"/>.
    /// </returns>
    private static bool IsDirectMemberOnRootParameter(Expression viewExpression) =>
        viewExpression.GetParent()?.NodeType == ExpressionType.Parameter;

    /// <summary>
    /// Creates an observable that applies changes directly to <paramref name="target"/> when the member is directly on the root parameter.
    /// </summary>
    /// <typeparam name="TTarget">The target object type.</typeparam>
    /// <typeparam name="TValue">The value type emitted by the returned observable.</typeparam>
    /// <typeparam name="TObs">The change element type.</typeparam>
    /// <param name="synchronizedChanges">The synchronized change stream.</param>
    /// <param name="target">The target object.</param>
    /// <param name="viewExpression">The view expression describing the member to set.</param>
    /// <param name="setThenGet">The set-then-get delegate.</param>
    /// <returns>An observable sequence of values that were effectively set.</returns>
    private static IObservable<TValue> CreateDirectSetObservable<TTarget, TValue, TObs>(
        IObservable<TObs> synchronizedChanges,
        TTarget target,
        Expression viewExpression,
        Func<object?, object?, object?[]?, (bool shouldEmit, object? value)> setThenGet)
        where TTarget : class
    {
        ArgumentExceptionHelper.ThrowIfNull(synchronizedChanges);
        ArgumentExceptionHelper.ThrowIfNull(target);
        ArgumentExceptionHelper.ThrowIfNull(viewExpression);
        ArgumentExceptionHelper.ThrowIfNull(setThenGet);

        var arguments = viewExpression.GetArgumentsArray();

        return synchronizedChanges.Select(value => setThenGet(target, value, arguments))
                                  .Where(result => result.shouldEmit)
                                  .Select(result => result.value is null ? default! : (TValue)result.value);
    }

    /// <summary>
    /// Creates the core "set then get" function that applies a value to a target member and returns whether a value should be emitted.
    /// </summary>
    /// <param name="viewExpression">The view expression describing the member to set.</param>
    /// <param name="getter">The compiled getter for the member.</param>
    /// <param name="setter">The compiled setter for the member.</param>
    /// <returns>
    /// A delegate that sets and then gets the value, returning whether the value should be emitted and the resulting value.
    /// </returns>
    private static Func<object?, object?, object?[]?, (bool shouldEmit, object? value)> CreateSetThenGet(
        Expression viewExpression,
        Func<object?, object?[]?, object?> getter,
        Action<object?, object?, object?[]?> setter)
    {
        ArgumentExceptionHelper.ThrowIfNull(viewExpression);
        ArgumentExceptionHelper.ThrowIfNull(getter);
        ArgumentExceptionHelper.ThrowIfNull(setter);

        return (paramTarget, paramValue, paramParams) =>
        {
            var converter = GetSetConverter(paramValue?.GetType(), viewExpression.Type);

            if (converter is null)
            {
                var currentValue = getter(paramTarget, paramParams);
                if (EqualityComparer<object?>.Default.Equals(currentValue, paramValue))
                {
                    return (false, currentValue);
                }

                setter(paramTarget, paramValue, paramParams);
                return (true, getter(paramTarget, paramParams));
            }

            var existing = getter(paramTarget, paramParams);
            var converted = converter(existing, paramValue, paramParams);
            var shouldEmit = !EqualityComparer<object?>.Default.Equals(existing, converted);
            return (shouldEmit, converted);
        };
    }

    /// <summary>
    /// Creates an observable that applies changes to a member whose host is obtained via a property chain.
    /// </summary>
    /// <typeparam name="TTarget">The root target object type.</typeparam>
    /// <typeparam name="TValue">The value type emitted by the returned observable.</typeparam>
    /// <typeparam name="TObs">The change element type.</typeparam>
    /// <param name="synchronizedChanges">The synchronized change stream.</param>
    /// <param name="target">The root target object.</param>
    /// <param name="viewExpression">The view expression describing the member to set.</param>
    /// <param name="setThenGet">The set-then-get delegate.</param>
    /// <param name="getter">The compiled getter for the member.</param>
    /// <returns>An observable sequence of values that were effectively set.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the host expression cannot be obtained.</exception>
    private static IObservable<TValue> CreateChainedSetObservable<TTarget, TValue, TObs>(
        IObservable<TObs> synchronizedChanges,
        TTarget target,
        Expression viewExpression,
        Func<object?, object?, object?[]?, (bool shouldEmit, object? value)> setThenGet,
        Func<object?, object?[]?, object?> getter)
        where TTarget : class
    {
        ArgumentExceptionHelper.ThrowIfNull(synchronizedChanges);
        ArgumentExceptionHelper.ThrowIfNull(target);
        ArgumentExceptionHelper.ThrowIfNull(viewExpression);
        ArgumentExceptionHelper.ThrowIfNull(setThenGet);
        ArgumentExceptionHelper.ThrowIfNull(getter);

        var hostExpression = viewExpression.GetParent() ?? throw new InvalidOperationException("Host expression was not found.");
        var hostChain = GetExpressionChainArrayOrNull(hostExpression);
        var hostChanges = target.WhenAnyDynamic(hostExpression, x => x.Value).Synchronize();
        var arguments = viewExpression.GetArgumentsArray();
        var propertyDefaultValue = CreateDefaultValueForType(viewExpression.Type);

        var shouldReplayOnHostChanges = ShouldReplayOnHostChanges(hostChain);

        return Observable.Create<TValue>(observer =>
        {
            ArgumentExceptionHelper.ThrowIfNull(observer);

            object? latestHost = null;
            object? currentHost = null;
            object? lastObservedValue = null;
            var hasObservedValue = false;

            bool HostPropertyEqualsDefault(object? host)
            {
                if (host is null)
                {
                    return false;
                }

                var currentValue = getter(host, arguments);
                return EqualityComparer<object?>.Default.Equals(currentValue, propertyDefaultValue);
            }

            void ApplyValueToHost(object? host, object? value)
            {
                if (host is null || !hasObservedValue)
                {
                    return;
                }

                var (shouldEmit, result) = setThenGet(host, value, arguments);
                if (!shouldEmit)
                {
                    return;
                }

                observer.OnNext(result is null ? default! : (TValue)result);
            }

            var hostDisposable = hostChanges.Subscribe(
                hostValue =>
                {
                    latestHost = hostValue;

                    if (ReferenceEquals(hostValue, currentHost))
                    {
                        return;
                    }

                    currentHost = hostValue;

                    if (!shouldReplayOnHostChanges || !hasObservedValue || !HostPropertyEqualsDefault(hostValue))
                    {
                        return;
                    }

                    ApplyValueToHost(hostValue, lastObservedValue);
                },
                observer.OnError);

            var changeDisposable = synchronizedChanges.Subscribe(
                value =>
                {
                    hasObservedValue = true;
                    lastObservedValue = value;

                    var host = latestHost;

                    if (hostChain is not null)
                    {
                        host = ResolveHostFromChainOrNull(target, hostChain);
                        latestHost = host;
                    }

                    if (host is null)
                    {
                        return;
                    }

                    ApplyValueToHost(host, value);
                },
                observer.OnError);

            return new CompositeDisposable(hostDisposable, changeDisposable);
        });
    }

    /// <summary>
    /// Binds an observable to a target member directly using compiled accessors.
    /// </summary>
    /// <typeparam name="TTarget">The target object type.</typeparam>
    /// <typeparam name="TValue">The value type emitted by the returned observable.</typeparam>
    /// <typeparam name="TObs">The element type produced by <paramref name="changeObservable"/>.</typeparam>
    /// <param name="changeObservable">The observable providing values to set.</param>
    /// <param name="target">The target object.</param>
    /// <param name="viewExpression">The rewritten member expression describing the target member.</param>
    /// <returns>
    /// A tuple containing the subscription <see cref="IDisposable"/> and an observable sequence of values that were effectively set.
    /// </returns>
    /// <exception cref="InvalidOperationException">Thrown when a required getter cannot be resolved.</exception>
    private (IDisposable disposable, IObservable<TValue> value) BindToDirect<TTarget, TValue, TObs>(
        IObservable<TObs> changeObservable,
        TTarget target,
        Expression viewExpression)
        where TTarget : class
    {
        ArgumentExceptionHelper.ThrowIfNull(changeObservable);
        ArgumentExceptionHelper.ThrowIfNull(target);
        ArgumentExceptionHelper.ThrowIfNull(viewExpression);

        var memberInfo = viewExpression.GetMemberInfo();

        var setter = Reflection.GetValueSetterOrThrow(memberInfo);
        var getter = Reflection.GetValueFetcherOrThrow(memberInfo) ?? throw new InvalidOperationException("getter was not found.");

        var synchronizedChanges = changeObservable.Synchronize();
        var setThenGet = CreateSetThenGet(viewExpression, getter, setter);

        IObservable<TValue> setObservable =
            IsDirectMemberOnRootParameter(viewExpression)
                ? CreateDirectSetObservable<TTarget, TValue, TObs>(synchronizedChanges, target, viewExpression, setThenGet)
                : CreateChainedSetObservable<TTarget, TValue, TObs>(synchronizedChanges, target, viewExpression, setThenGet, getter);

        var subscription = SubscribeWithBindingErrorHandling(setObservable, viewExpression);

        return (subscription, setObservable);
    }

    /// <summary>
    /// Subscribes to <paramref name="setObservable"/> and applies binding error handling consistent with the binding engine.
    /// </summary>
    /// <typeparam name="TValue">The element type of the observable.</typeparam>
    /// <param name="setObservable">The observable to subscribe to.</param>
    /// <param name="viewExpression">The view expression used for diagnostic messages.</param>
    /// <returns>The subscription disposable.</returns>
    /// <exception cref="TargetInvocationException">
    /// Thrown when the binding receives an exception with an inner exception, matching legacy behavior.
    /// </exception>
    private IDisposable SubscribeWithBindingErrorHandling<TValue>(IObservable<TValue> setObservable, Expression viewExpression)
    {
        ArgumentExceptionHelper.ThrowIfNull(setObservable);
        ArgumentExceptionHelper.ThrowIfNull(viewExpression);

        return setObservable.Subscribe(
            _ => { },
            ex =>
            {
                this.Log().Error(ex, $"{viewExpression} Binding received an Exception!");
                if (ex.InnerException is null)
                {
                    return;
                }

                throw new TargetInvocationException($"{viewExpression} Binding received an Exception!", ex.InnerException);
            });
    }

    /// <summary>
    /// Evaluates registered <see cref="IPropertyBindingHook"/> instances to determine whether a binding should be created.
    /// </summary>
    /// <typeparam name="TViewModel">The view model type.</typeparam>
    /// <typeparam name="TView">The view type.</typeparam>
    /// <param name="viewModel">The view model instance (may be <see langword="null"/>).</param>
    /// <param name="view">The view instance.</param>
    /// <param name="vmExpression">The rewritten view model expression.</param>
    /// <param name="viewExpression">The rewritten view expression.</param>
    /// <param name="direction">The binding direction.</param>
    /// <returns><see langword="true"/> if the binding should proceed; otherwise <see langword="false"/>.</returns>
    private bool EvalBindingHooks<TViewModel, TView>(TViewModel? viewModel, TView view, Expression vmExpression, Expression viewExpression, BindingDirection direction)
        where TViewModel : class
        where TView : class, IViewFor
    {
        var hooks = AppLocator.Current.GetServices<IPropertyBindingHook>();
        ArgumentExceptionHelper.ThrowIfNull(view);

        // Compile chains once for hook evaluation.
        var vmChainGetter = vmExpression != null
            ? new Reflection.CompiledPropertyChain<object?, object?>([.. vmExpression.GetExpressionChain()])
            : null;
        var viewChainGetter = new Reflection.CompiledPropertyChain<TView, object?>([.. viewExpression.GetExpressionChain()]);

        Func<IObservedChange<object, object?>[]> vmFetcher = vmExpression is not null
            ? (() =>
            {
                vmChainGetter!.TryGetAllValues(viewModel, out var fetchedValues);
                return fetchedValues;
            })
            : (() => [new ObservedChange<object, object?>(null!, null, viewModel)]);

        Func<IObservedChange<object, object?>[]> vFetcher = () =>
        {
            viewChainGetter.TryGetAllValues(view, out var fetchedValues);
            return fetchedValues;
        };

        // Replace Aggregate with a loop to avoid enumerator overhead and closures.
        var shouldBind = true;
        foreach (var hook in hooks)
        {
            if (hook is null)
            {
                continue;
            }

            if (!hook.ExecuteHook(viewModel, view!, vmFetcher!, vFetcher!, direction))
            {
                shouldBind = false;
                break;
            }
        }

        if (!shouldBind)
        {
            var vmString = $"{typeof(TViewModel).Name}.{string.Join(".", vmExpression)}";
            var vString = $"{typeof(TView).Name}.{string.Join(".", viewExpression)}";
            this.Log().Warn(CultureInfo.InvariantCulture, "Binding hook asked to disable binding {0} => {1}", vmString, vString);
        }

        return shouldBind;
    }

    private ReactiveBinding<TView, (object? view, bool isViewModel)> BindImpl<TViewModel, TView, TVMProp, TVProp, TDontCare>(
        TViewModel? viewModel,
        TView view,
        Expression<Func<TViewModel, TVMProp?>> vmProperty,
        Expression<Func<TView, TVProp>> viewProperty,
        IObservable<TDontCare>? signalViewUpdate,
        OutFunc<TVMProp?, TVProp> vmToViewConverter,
        OutFunc<TVProp, TVMProp?> viewToVmConverter,
        TriggerUpdate triggerUpdate = TriggerUpdate.ViewToViewModel)
        where TViewModel : class
        where TView : class, IViewFor
    {
        ArgumentExceptionHelper.ThrowIfNull(vmProperty);
        ArgumentExceptionHelper.ThrowIfNull(viewProperty);

        var signalInitialUpdate = new Subject<bool>();
        var vmExpression = Reflection.Rewrite(vmProperty.Body);
        var viewExpression = Reflection.Rewrite(viewProperty.Body);

        // Pre-compile expression chains ONCE at binding setup time.
        // This is the "reflection boundary".
        Expression[] vmExpressionChainArray = [.. vmExpression.GetExpressionChain()];
        Expression[] viewExpressionChainArray = [.. viewExpression.GetExpressionChain()];

        // VM chain expects root = view.ViewModel (object?).
        var vmChainGetter = new Reflection.CompiledPropertyChain<object?, TVMProp>(vmExpressionChainArray);

        // View chain expects root = view (TView).
        var viewChainGetter = new Reflection.CompiledPropertyChain<TView, TVProp>(viewExpressionChainArray);

        // Setters for two-way binding.
        var viewChainSetter = new Reflection.CompiledPropertyChainSetter<TView, object?>(viewExpressionChainArray);
        var vmChainSetter = new Reflection.CompiledPropertyChainSetter<object?, object?>(vmExpressionChainArray);

        IObservable<(bool isValid, object? view, bool isViewModel)>? changeWithValues = null;

        if (triggerUpdate == TriggerUpdate.ViewToViewModel)
        {
            var signalObservable = signalViewUpdate is not null
                ? signalViewUpdate.Select(_ => false)
                : view.WhenAnyDynamic(viewExpression, x => (TVProp?)x.Value).Select(_ => false);

            var somethingChanged = Observable.Merge(
                Reflection.ViewModelWhenAnyValue(viewModel, view, vmExpression).Select(_ => true),
                signalInitialUpdate.Select(_ => true),
                signalObservable);

            changeWithValues = somethingChanged.Select<bool, (bool isValid, object? view, bool isViewModel)>(isVm =>
                !vmChainGetter.TryGetValue(view.ViewModel, out TVMProp vmValue) ||
                !viewChainGetter.TryGetValue(view, out TVProp vValue)
                    ? (false, null, false)
                    : isVm
                        ? !vmToViewConverter(vmValue, out var vmAsView) || EqualityComparer<TVProp>.Default.Equals(vValue, vmAsView)
                            ? (false, null, false)
                            : (true, vmAsView, isVm)
                        : !viewToVmConverter(vValue, out var vAsViewModel) || EqualityComparer<TVMProp?>.Default.Equals(vmValue, vAsViewModel)
                            ? (false, null, false)
                            : (true, vAsViewModel, isVm));
        }
        else
        {
            var somethingChanged = Observable.Merge(
                signalViewUpdate is null
                    ? Reflection.ViewModelWhenAnyValue(viewModel, view, vmExpression).Select(_ => true)
                    : signalViewUpdate.Select(_ => true)
                        .Merge(Reflection.ViewModelWhenAnyValue(viewModel, view, vmExpression).Select(_ => true).Take(1)),
                signalInitialUpdate.Select(_ => true),
                view.WhenAnyDynamic(viewExpression, x => (TVProp?)x.Value).Select(_ => false));

            changeWithValues = somethingChanged.Select<bool, (bool isValid, object? view, bool isViewModel)>(isVm =>
                !vmChainGetter.TryGetValue(view.ViewModel, out TVMProp vmValue) ||
                !viewChainGetter.TryGetValue(view, out TVProp vValue)
                    ? (false, null, false)
                    : isVm
                        ? !vmToViewConverter(vmValue, out var vmAsView) || EqualityComparer<TVProp>.Default.Equals(vValue, vmAsView)
                            ? (false, null, false)
                            : (true, vmAsView, isVm)
                        : !viewToVmConverter(vValue, out var vAsViewModel) || EqualityComparer<TVMProp?>.Default.Equals(vmValue, vAsViewModel)
                            ? (false, null, false)
                            : (true, vAsViewModel, isVm));
        }

        var ret = EvalBindingHooks(viewModel, view, vmExpression, viewExpression, BindingDirection.TwoWay);
        if (!ret)
        {
            return null!;
        }

        var changes =
            changeWithValues
                .Where(value => value.isValid)
                .Select(value => (value.view, value.isViewModel))
                .Publish()
                .RefCount();

        var disposable = changes.Subscribe(isVmWithLatestValue =>
        {
            if (isVmWithLatestValue.isViewModel)
            {
                viewChainSetter.TrySetValue(view, isVmWithLatestValue.view, false);
            }
            else
            {
                vmChainSetter.TrySetValue(view.ViewModel, isVmWithLatestValue.view, false);
            }
        });

        // NB: Even though it's technically a two-way bind, most people want the ViewModel to win at first.
        signalInitialUpdate.OnNext(true);

        return new ReactiveBinding<TView, (object? view, bool isViewModel)>(
            view,
            viewExpression,
            vmExpression,
            changes,
            BindingDirection.TwoWay,
            disposable);
    }
}
