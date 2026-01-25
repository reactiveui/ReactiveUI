// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reflection;

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
    private static readonly IBindingConverterResolver _staticConverterResolver = new BindingConverterResolver();

    private readonly IBindingConverterResolver _converterResolver;
    private readonly IPropertyBindingExpressionCompiler _expressionCompiler;
    private readonly IBindingHookEvaluator _hookEvaluator;

    /// <summary>
    /// Initializes a new instance of the <see cref="PropertyBinderImplementation"/> class
    /// with default dependencies.
    /// </summary>
    public PropertyBinderImplementation()
        : this(new BindingConverterResolver(), new PropertyBindingExpressionCompiler(), new BindingHookEvaluator())
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PropertyBinderImplementation"/> class
    /// with specified dependencies (for testing).
    /// </summary>
    /// <param name="converterResolver">The converter resolver to use.</param>
    /// <param name="expressionCompiler">The expression compiler to use.</param>
    /// <param name="hookEvaluator">The hook evaluator to use.</param>
    internal PropertyBinderImplementation(
        IBindingConverterResolver converterResolver,
        IPropertyBindingExpressionCompiler expressionCompiler,
        IBindingHookEvaluator hookEvaluator)
    {
        ArgumentExceptionHelper.ThrowIfNull(converterResolver);
        ArgumentExceptionHelper.ThrowIfNull(expressionCompiler);
        ArgumentExceptionHelper.ThrowIfNull(hookEvaluator);

        _converterResolver = converterResolver;
        _expressionCompiler = expressionCompiler;
        _hookEvaluator = hookEvaluator;
    }

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

        // First, try to find registered converters (prioritize user-registered converters)
        // If an override is provided, use it; otherwise fall back to service locator
        var vmToViewConverterObj = vmToViewConverterOverride ?? GetConverterForTypes(typeof(TVMProp), typeof(TVProp));

        var viewToVMConverterObj = viewToVMConverterOverride ?? GetConverterForTypes(typeof(TVProp), typeof(TVMProp?));

        // Check if we have converters or if types are assignable
        var hasConverters = vmToViewConverterObj is not null && viewToVMConverterObj is not null;
        var typesAreAssignable = typeof(TVProp).IsAssignableFrom(typeof(TVMProp)) && typeof(TVMProp).IsAssignableFrom(typeof(TVProp));

        if (!hasConverters && !typesAreAssignable)
        {
            throw new ArgumentException(
                $"Can't two-way convert between {typeof(TVMProp)} and {typeof(TVProp)}. To fix this, register a IBindingTypeConverter or call the version with the converter Func.");
        }

        bool VmToViewFunc(TVMProp? vmValue, out TVProp vValue)
        {
            if (vmToViewConverterObj is not null)
            {
                bool result;
                object? tmp;

                // If an explicit override was provided, call it directly (bypassing type checks)
                // Otherwise use the dispatch which validates types for auto-discovered converters
                if (vmToViewConverterOverride is not null)
                {
                    // Trust the user's explicit converter choice
                    if (vmToViewConverterObj is IBindingTypeConverter typedConverter)
                    {
                        result = typedConverter.TryConvertTyped(vmValue, conversionHint, out tmp);
                    }
                    else if (vmToViewConverterObj is IBindingFallbackConverter fallbackConverter)
                    {
                        // Fallback converters require non-null input
                        if (vmValue is null)
                        {
                            tmp = null;
                            result = false;
                        }
                        else
                        {
                            result = fallbackConverter.TryConvert(typeof(TVMProp), vmValue, typeof(TVProp), conversionHint, out tmp);
                        }
                    }
                    else
                    {
                        tmp = null;
                        result = false;
                    }

                    // If explicit override failed, try to find a better converter from registry
                    if (!result)
                    {
                        var fallbackConverter = GetConverterForTypes(typeof(TVMProp), typeof(TVProp));
                        if (fallbackConverter is not null && fallbackConverter != vmToViewConverterObj)
                        {
                            result = BindingTypeConverterDispatch.TryConvertAny(
                                fallbackConverter,
                                typeof(TVMProp),
                                vmValue,
                                typeof(TVProp),
                                conversionHint,
                                out tmp);

                            if (result)
                            {
                                vValue = (TVProp)tmp!;
                                return true;
                            }
                        }
                    }
                }
                else
                {
                    // No override - use type-checked dispatch for auto-discovered converter
                    result = BindingTypeConverterDispatch.TryConvertAny(
                        vmToViewConverterObj,
                        typeof(TVMProp),
                        vmValue,
                        typeof(TVProp),
                        conversionHint,
                        out tmp);

                    // If auto-discovered converter failed, try direct assignment
                    if (!result && typeof(TVProp).IsAssignableFrom(typeof(TVMProp)))
                    {
                        vValue = vmValue is TVProp fallbackValue ? fallbackValue : default!;
                        return true;
                    }
                }

                vValue = result ? (TVProp)tmp! : default!;
                return result;
            }

            // No converter - direct assignment
            vValue = vmValue is TVProp typedValue ? typedValue : default!;
            return true;
        }

        bool ViewToVmFunc(TVProp vValue, out TVMProp? vmValue)
        {
            if (viewToVMConverterObj is not null)
            {
                bool result;
                object? tmp;

                // If an explicit override was provided, call it directly (bypassing type checks)
                // Otherwise use the dispatch which validates types for auto-discovered converters
                if (viewToVMConverterOverride is not null)
                {
                    // Trust the user's explicit converter choice
                    if (viewToVMConverterObj is IBindingTypeConverter typedConverter)
                    {
                        result = typedConverter.TryConvertTyped(vValue, conversionHint, out tmp);
                    }
                    else if (viewToVMConverterObj is IBindingFallbackConverter fallbackConverter)
                    {
                        // Fallback converters require non-null input
                        if (vValue is null)
                        {
                            tmp = null;
                            result = false;
                        }
                        else
                        {
                            result = fallbackConverter.TryConvert(typeof(TVProp), vValue, typeof(TVMProp?), conversionHint, out tmp);
                        }
                    }
                    else
                    {
                        tmp = null;
                        result = false;
                    }

                    // If explicit override failed, try to find a better converter from registry
                    if (!result)
                    {
                        var fallbackConverter = GetConverterForTypes(typeof(TVProp), typeof(TVMProp?));
                        if (fallbackConverter is not null && fallbackConverter != viewToVMConverterObj)
                        {
                            result = BindingTypeConverterDispatch.TryConvertAny(
                                fallbackConverter,
                                typeof(TVProp),
                                vValue,
                                typeof(TVMProp?),
                                conversionHint,
                                out tmp);

                            if (result)
                            {
                                vmValue = (TVMProp?)tmp;
                                return true;
                            }
                        }
                    }
                }
                else
                {
                    // No override - use type-checked dispatch for auto-discovered converter
                    result = BindingTypeConverterDispatch.TryConvertAny(
                        viewToVMConverterObj,
                        typeof(TVProp),
                        vValue,
                        typeof(TVMProp?),
                        conversionHint,
                        out tmp);

                    // If auto-discovered converter failed, try direct assignment
                    if (!result && typeof(TVMProp).IsAssignableFrom(typeof(TVProp)))
                    {
                        vmValue = vValue is TVMProp fallbackValue ? fallbackValue : default;
                        return true;
                    }
                }

                vmValue = result ? (TVMProp?)tmp : default;
                return result;
            }

            // No converter - direct assignment
            vmValue = vValue is TVMProp typedValue ? typedValue : default;
            return true;
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

        var ret = _hookEvaluator.EvaluateBindingHooks(viewModel, view, vmExpression, viewExpression, BindingDirection.OneWay);
        if (!ret)
        {
            return new ReactiveBinding<TView, TVProp>(view, viewExpression, vmExpression, Observable.Empty<TVProp>(), BindingDirection.OneWay, Disposable.Empty);
        }

        // First, try to find a registered converter (prioritize user-registered converters)
        var converterObj = vmToViewConverterOverride ?? GetConverterForTypes(typeof(TVMProp?), viewType);

        if (converterObj is not null)
        {
            // Use the converter
            var source =
                Reflection.ViewModelWhenAnyValue(viewModel, view, vmExpression)
                    .SelectMany(x =>
                    {
                        var runtimeType = x?.GetType() ?? typeof(TVMProp);

                        // Try converter first
                        var convertResult = BindingTypeConverterDispatch.TryConvertAny(
                            converterObj,
                            runtimeType,
                            x,
                            viewType,
                            conversionHint,
                            out var tmp);

                        if (convertResult)
                        {
                            return Observable.Return(tmp);
                        }

                        // Converter failed
                        // If no override was provided (auto-discovered converter), try direct assignment
                        if (vmToViewConverterOverride is null)
                        {
                            // For null values, use the actual TVMProp type for assignability check
                            if (viewType.IsAssignableFrom(typeof(TVMProp)))
                            {
                                return Observable.Return((object?)x);
                            }
                        }

                        // Cannot convert - skip this update
                        return Observable<object>.Empty;
                    });

            var (disposable, obs) = BindToDirect<TView, TVProp, object?>(source, view, viewExpression);
            return new ReactiveBinding<TView, TVProp>(view, viewExpression, vmExpression, obs, BindingDirection.OneWay, disposable);
        }

        // No converter found - check if types are directly assignable
        if (viewType.IsAssignableFrom(typeof(TVMProp)))
        {
            // No conversion needed - direct assignment
            var source = Reflection.ViewModelWhenAnyValue(viewModel, view, vmExpression).Select(x => (object?)x);
            var (disposable, obs) = BindToDirect<TView, TVProp, object?>(source, view, viewExpression);
            return new ReactiveBinding<TView, TVProp>(view, viewExpression, vmExpression, obs, BindingDirection.OneWay, disposable);
        }

        // No converter and types not assignable - throw exception
        throw new ArgumentException($"Can't convert {typeof(TVMProp)} to {viewType}. To fix this, register a IBindingTypeConverter");
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

        var ret = _hookEvaluator.EvaluateBindingHooks(viewModel, view, vmExpression, viewExpression, BindingDirection.OneWay);
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

        var shouldBind = target is not IViewFor viewFor || _hookEvaluator.EvaluateBindingHooks<object, IViewFor>(null, viewFor, null!, viewExpression, BindingDirection.OneWay);
        if (!shouldBind)
        {
            return Disposable.Empty;
        }

        // First, try to find a registered converter (prioritize user-registered converters)
        var converterObj = vmToViewConverterOverride ?? GetConverterForTypes(typeof(TValue), typeof(TTValue?));

        if (converterObj is not null)
        {
            // Use the converter
            var source =
                observedChange.SelectMany(x =>
                {
                    // Try converter first
                    var convertResult = BindingTypeConverterDispatch.TryConvertAny(
                        converterObj,
                        typeof(TValue),
                        x,
                        typeof(TTValue?),
                        conversionHint,
                        out var tmp);

                    if (convertResult)
                    {
                        return Observable.Return(tmp);
                    }

                    // Converter failed
                    // If no override was provided (auto-discovered converter), try direct assignment
                    if (vmToViewConverterOverride is null && typeof(TTValue).IsAssignableFrom(typeof(TValue)))
                    {
                        return Observable.Return((object?)x);
                    }

                    // Cannot convert - skip this update
                    return Observable<object>.Empty;
                });

            var (disposable, _) = BindToDirect<TTarget, TTValue?, object?>(source, target!, viewExpression);
            return disposable;
        }

        // No converter found - check if types are directly assignable (includes same type and compatible reference types)
        if (typeof(TTValue).IsAssignableFrom(typeof(TValue)))
        {
            // No conversion needed - direct assignment
            var source = observedChange.Select(x => (object?)x);
            var (disposable, _) = BindToDirect<TTarget, TTValue?, object?>(source, target!, viewExpression);
            return disposable;
        }

        // No converter and types not assignable - throw exception
        throw new ArgumentException($"Can't convert {typeof(TValue)} to {typeof(TTValue)}. To fix this, register a IBindingTypeConverter");
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
        _staticConverterResolver.GetBindingConverter(lhs, rhs);

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

        var setObservableWithEmit =
            _expressionCompiler.IsDirectMemberAccess(viewExpression)
                ? _expressionCompiler.CreateDirectSetObservable<TTarget, TValue, TObs>(target, changeObservable, viewExpression, getter, setter, _converterResolver.GetSetMethodConverter)
                : _expressionCompiler.CreateChainedSetObservable<TTarget, TValue, TObs>(target, changeObservable, viewExpression, _expressionCompiler.GetExpressionChainArray(viewExpression.GetParent()!) ?? [], getter, setter, _converterResolver.GetSetMethodConverter);

        var setObservable = setObservableWithEmit.Select(x => x.Value);
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

        var ret = _hookEvaluator.EvaluateBindingHooks(viewModel, view, vmExpression, viewExpression, BindingDirection.TwoWay);
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
