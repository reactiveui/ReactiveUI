// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reactive.Disposables;
using System.Reflection;
using ReactiveUI.Helpers;
using ReactiveUI.Internal;
using Splat;

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
[RequiresDynamicCode(
    "Uses dynamic binding paths which may require runtime code generation or reflection-based invocation.")]
public partial class PropertyBinderImplementation : IPropertyBinderImplementation
{
    /// <summary>Shared static converter resolver used for type-based binding lookups.</summary>
    private static readonly BindingConverterResolver _staticConverterResolver = new();

    /// <summary>Resolves binding type converters for this instance.</summary>
    private readonly IBindingConverterResolver _converterResolver;

    /// <summary>Compiles property binding expressions to optimized accessors.</summary>
    private readonly IPropertyBindingExpressionCompiler _expressionCompiler;

    /// <summary>Evaluates binding hooks before a binding is established.</summary>
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
    public IReactiveBinding<TView, (object? view, bool isViewModel)>
        Bind<TViewModel, TView, TViewModelPropertyType, TViewPropertyType, TDontCare>(
            TViewModel? viewModel,
            TView view,
            Expression<Func<TViewModel, TViewModelPropertyType?>> viewModelProperty,
            Expression<Func<TView, TViewPropertyType>> viewProperty,
            IObservable<TDontCare>? signalViewUpdate,
            object? conversionHint)
        where TViewModel : class
        where TView : class, IViewFor =>
        Bind(
            viewModel,
            view,
            viewModelProperty,
            viewProperty,
            signalViewUpdate,
            conversionHint,
            null,
            null,
            TriggerUpdate.ViewToViewModel);

    /// <inheritdoc />
    public IReactiveBinding<TView, (object? view, bool isViewModel)>
        Bind<TViewModel, TView, TViewModelPropertyType, TViewPropertyType, TDontCare>(
            TViewModel? viewModel,
            TView view,
            Expression<Func<TViewModel, TViewModelPropertyType?>> viewModelProperty,
            Expression<Func<TView, TViewPropertyType>> viewProperty,
            IObservable<TDontCare>? signalViewUpdate,
            object? conversionHint,
            IBindingTypeConverter? viewModelToViewConverterOverride)
        where TViewModel : class
        where TView : class, IViewFor =>
        Bind(
            viewModel,
            view,
            viewModelProperty,
            viewProperty,
            signalViewUpdate,
            conversionHint,
            viewModelToViewConverterOverride,
            null,
            TriggerUpdate.ViewToViewModel);

    /// <inheritdoc />
    [SuppressMessage(
        "Major Code Smell",
        "S107:Methods should not have too many parameters",
        Justification = "This overload mirrors the public IPropertyBinderImplementation contract; the parameter count is part of the binding API surface.")]
    public IReactiveBinding<TView, (object? view, bool isViewModel)>
        Bind<TViewModel, TView, TViewModelPropertyType, TViewPropertyType, TDontCare>(
            TViewModel? viewModel,
            TView view,
            Expression<Func<TViewModel, TViewModelPropertyType?>> viewModelProperty,
            Expression<Func<TView, TViewPropertyType>> viewProperty,
            IObservable<TDontCare>? signalViewUpdate,
            object? conversionHint,
            IBindingTypeConverter? viewModelToViewConverterOverride,
            IBindingTypeConverter? viewToViewModelConverterOverride)
        where TViewModel : class
        where TView : class, IViewFor =>
        Bind(
            viewModel,
            view,
            viewModelProperty,
            viewProperty,
            signalViewUpdate,
            conversionHint,
            viewModelToViewConverterOverride,
            viewToViewModelConverterOverride,
            TriggerUpdate.ViewToViewModel);

    /// <inheritdoc />
    [SuppressMessage(
        "Major Code Smell",
        "S107:Methods should not have too many parameters",
        Justification = "This overload mirrors the public IPropertyBinderImplementation contract; the parameter count is part of the binding API surface.")]
    public IReactiveBinding<TView, (object? view, bool isViewModel)>
        Bind<TViewModel, TView, TViewModelPropertyType, TViewPropertyType, TDontCare>(
            TViewModel? viewModel,
            TView view,
            Expression<Func<TViewModel, TViewModelPropertyType?>> viewModelProperty,
            Expression<Func<TView, TViewPropertyType>> viewProperty,
            IObservable<TDontCare>? signalViewUpdate,
            object? conversionHint,
            IBindingTypeConverter? viewModelToViewConverterOverride,
            IBindingTypeConverter? viewToViewModelConverterOverride,
            TriggerUpdate triggerUpdate)
        where TViewModel : class
        where TView : class, IViewFor
    {
        ArgumentExceptionHelper.ThrowIfNull(viewModelProperty);
        ArgumentExceptionHelper.ThrowIfNull(viewProperty);

        var viewModelToViewConverterObj = viewModelToViewConverterOverride ?? GetConverterForTypes(typeof(TViewModelPropertyType), typeof(TViewPropertyType));

        var viewToViewModelConverterObj = viewToViewModelConverterOverride ?? GetConverterForTypes(typeof(TViewPropertyType), typeof(TViewModelPropertyType?));

        var hasConverters = viewModelToViewConverterObj is not null && viewToViewModelConverterObj is not null;
        var typesAreAssignable = typeof(TViewPropertyType).IsAssignableFrom(typeof(TViewModelPropertyType)) ||
                                 typeof(TViewModelPropertyType).IsAssignableFrom(typeof(TViewPropertyType));

        if (!hasConverters && !typesAreAssignable)
        {
            throw new ArgumentException(
                $"Can't two-way convert between {typeof(TViewModelPropertyType)} and {typeof(TViewPropertyType)}. " +
                "To fix this, register a IBindingTypeConverter or call the version with the converter Func.");
        }

        return BindImpl(new TwoWayBindRequest<TViewModel, TView, TViewModelPropertyType, TViewPropertyType, TDontCare>
        {
            ViewModel = viewModel,
            View = view,
            ViewModelProperty = viewModelProperty,
            ViewProperty = viewProperty,
            SignalViewUpdate = signalViewUpdate,
            ViewModelToViewConverter = (value, out converted) =>
                TryConvertViewModelToView(viewModelToViewConverterObj, viewModelToViewConverterOverride, conversionHint, value, out converted),
            ViewToViewModelConverter = (value, out converted) =>
                TryConvertViewToViewModel(viewToViewModelConverterObj, viewToViewModelConverterOverride, conversionHint, value, out converted),
            TriggerUpdate = triggerUpdate,
        });
    }

    /// <inheritdoc />
    public IReactiveBinding<TView, (object? view, bool isViewModel)>
        Bind<TViewModel, TView, TViewModelPropertyType, TViewPropertyType, TDontCare>(
            TViewModel? viewModel,
            TView view,
            Expression<Func<TViewModel, TViewModelPropertyType?>> viewModelProperty,
            Expression<Func<TView, TViewPropertyType>> viewProperty,
            IObservable<TDontCare>? signalViewUpdate,
            Func<TViewModelPropertyType?, TViewPropertyType> viewModelToViewConverter,
            Func<TViewPropertyType, TViewModelPropertyType?> viewToViewModelConverter)
        where TViewModel : class
        where TView : class, IViewFor =>
        Bind(
            viewModel,
            view,
            viewModelProperty,
            viewProperty,
            signalViewUpdate,
            viewModelToViewConverter,
            viewToViewModelConverter,
            TriggerUpdate.ViewToViewModel);

    /// <inheritdoc />
    [SuppressMessage(
        "Major Code Smell",
        "S107:Methods should not have too many parameters",
        Justification = "This overload mirrors the public IPropertyBinderImplementation contract; the parameter count is part of the binding API surface.")]
    public IReactiveBinding<TView, (object? view, bool isViewModel)>
        Bind<TViewModel, TView, TViewModelPropertyType, TViewPropertyType, TDontCare>(
            TViewModel? viewModel,
            TView view,
            Expression<Func<TViewModel, TViewModelPropertyType?>> viewModelProperty,
            Expression<Func<TView, TViewPropertyType>> viewProperty,
            IObservable<TDontCare>? signalViewUpdate,
            Func<TViewModelPropertyType?, TViewPropertyType> viewModelToViewConverter,
            Func<TViewPropertyType, TViewModelPropertyType?> viewToViewModelConverter,
            TriggerUpdate triggerUpdate)
        where TViewModel : class
        where TView : class, IViewFor
    {
        ArgumentExceptionHelper.ThrowIfNull(viewModelProperty);
        ArgumentExceptionHelper.ThrowIfNull(viewProperty);
        ArgumentExceptionHelper.ThrowIfNull(viewModelToViewConverter);
        ArgumentExceptionHelper.ThrowIfNull(viewToViewModelConverter);

        return BindImpl(new TwoWayBindRequest<TViewModel, TView, TViewModelPropertyType, TViewPropertyType, TDontCare>
        {
            ViewModel = viewModel,
            View = view,
            ViewModelProperty = viewModelProperty,
            ViewProperty = viewProperty,
            SignalViewUpdate = signalViewUpdate,
            ViewModelToViewConverter = (value, out converted) =>
            {
                converted = viewModelToViewConverter(value);
                return true;
            },
            ViewToViewModelConverter = (value, out converted) =>
            {
                converted = viewToViewModelConverter(value);
                return true;
            },
            TriggerUpdate = triggerUpdate,
        });
    }

    /// <inheritdoc />
    public IReactiveBinding<TView, TViewPropertyType> OneWayBind<TViewModel, TView, TViewModelPropertyType, TViewPropertyType>(
        TViewModel? viewModel,
        TView view,
        Expression<Func<TViewModel, TViewModelPropertyType?>> viewModelProperty,
        Expression<Func<TView, TViewPropertyType>> viewProperty)
        where TViewModel : class
        where TView : class, IViewFor =>
        OneWayBind(viewModel, view, viewModelProperty, viewProperty, null, null);

    /// <inheritdoc />
    public IReactiveBinding<TView, TViewPropertyType> OneWayBind<TViewModel, TView, TViewModelPropertyType, TViewPropertyType>(
        TViewModel? viewModel,
        TView view,
        Expression<Func<TViewModel, TViewModelPropertyType?>> viewModelProperty,
        Expression<Func<TView, TViewPropertyType>> viewProperty,
        IBindingTypeConverter? vmToViewConverterOverride)
        where TViewModel : class
        where TView : class, IViewFor =>
        OneWayBind(viewModel, view, viewModelProperty, viewProperty, null, vmToViewConverterOverride);

    /// <inheritdoc />
    public IReactiveBinding<TView, TViewPropertyType> OneWayBind<TViewModel, TView, TViewModelPropertyType, TViewPropertyType>(
        TViewModel? viewModel,
        TView view,
        Expression<Func<TViewModel, TViewModelPropertyType?>> viewModelProperty,
        Expression<Func<TView, TViewPropertyType>> viewProperty,
        object? conversionHint)
        where TViewModel : class
        where TView : class, IViewFor =>
        OneWayBind(viewModel, view, viewModelProperty, viewProperty, conversionHint, null);

    /// <inheritdoc />
    public IReactiveBinding<TView, TViewPropertyType> OneWayBind<TViewModel, TView, TViewModelPropertyType, TViewPropertyType>(
        TViewModel? viewModel,
        TView view,
        Expression<Func<TViewModel, TViewModelPropertyType?>> viewModelProperty,
        Expression<Func<TView, TViewPropertyType>> viewProperty,
        object? conversionHint,
        IBindingTypeConverter? viewModelToViewConverterOverride)
        where TViewModel : class
        where TView : class, IViewFor
    {
        ArgumentExceptionHelper.ThrowIfNull(viewModelProperty);
        ArgumentExceptionHelper.ThrowIfNull(viewProperty);

        var viewModelExpression = Reflection.Rewrite(viewModelProperty.Body);
        var viewExpression = Reflection.Rewrite(viewProperty.Body);
        var viewType = viewExpression.Type;

        var ret = _hookEvaluator.EvaluateBindingHooks(
            viewModel,
            view,
            viewModelExpression,
            viewExpression,
            BindingDirection.OneWay);
        if (!ret)
        {
            return new ReactiveBinding<TView, TViewPropertyType>(
                view,
                viewExpression,
                viewModelExpression,
                EmptyObservable<TViewPropertyType>.Instance,
                BindingDirection.OneWay,
                EmptyDisposable.Instance);
        }

        var converterObj = viewModelToViewConverterOverride ?? GetConverterForTypes(typeof(TViewModelPropertyType?), viewType);

        if (converterObj is not null)
        {
            var source = new ChooseObservable<object, object?>(
                Reflection.ViewModelWhenAnyValue(viewModel, view, viewModelExpression),
                x =>
                {
                    var runtimeType = x?.GetType() ?? typeof(TViewModelPropertyType);

                    var convertResult = BindingTypeConverterDispatch.TryConvertAny(
                        converterObj,
                        runtimeType,
                        x,
                        viewType,
                        conversionHint,
                        out var tmp);

                    if (convertResult)
                    {
                        return (true, tmp);
                    }

                    return viewModelToViewConverterOverride is null && viewType.IsAssignableFrom(typeof(TViewModelPropertyType))
                        ? (true, (object?)x)
                        : (false, null);
                });

            var (disposable, obs) = BindToDirect<TView, TViewPropertyType, object?>(source, view, viewExpression);
            return new ReactiveBinding<TView, TViewPropertyType>(
                view,
                viewExpression,
                viewModelExpression,
                obs,
                BindingDirection.OneWay,
                disposable);
        }

        if (viewType.IsAssignableFrom(typeof(TViewModelPropertyType)))
        {
            var source = new SelectObservable<object, object?>(
                Reflection.ViewModelWhenAnyValue(viewModel, view, viewModelExpression),
                static x => x);
            var (disposable, obs) = BindToDirect<TView, TViewPropertyType, object?>(source, view, viewExpression);
            return new ReactiveBinding<TView, TViewPropertyType>(
                view,
                viewExpression,
                viewModelExpression,
                obs,
                BindingDirection.OneWay,
                disposable);
        }

        throw new ArgumentException(
            $"Can't convert {typeof(TViewModelPropertyType)} to {viewType}. To fix this, register a IBindingTypeConverter");
    }

    /// <inheritdoc />
    public IReactiveBinding<TView, TOut> OneWayBind<TViewModel, TView, TProp, TOut>(
        TViewModel? viewModel,
        TView view,
        Expression<Func<TViewModel, TProp>> viewModelProperty,
        Expression<Func<TView, TOut>> viewProperty,
        Func<TProp, TOut> selector)
        where TViewModel : class
        where TView : class, IViewFor
    {
        ArgumentExceptionHelper.ThrowIfNull(viewModelProperty);
        ArgumentExceptionHelper.ThrowIfNull(viewProperty);

        var viewModelExpression = Reflection.Rewrite(viewModelProperty.Body);
        var viewExpression = Reflection.Rewrite(viewProperty.Body);

        var ret = _hookEvaluator.EvaluateBindingHooks(
            viewModel,
            view,
            viewModelExpression,
            viewExpression,
            BindingDirection.OneWay);
        if (!ret)
        {
            return new ReactiveBinding<TView, TOut>(
                view,
                viewExpression,
                viewModelExpression,
                EmptyObservable<TOut>.Instance,
                BindingDirection.OneWay,
                EmptyDisposable.Instance);
        }

        var source = new SelectObservable<object, TOut>(
            Reflection.ViewModelWhenAnyValue(viewModel, view, viewModelExpression),
            x => selector((TProp)x!));

        var (disposable, obs) = BindToDirect<TView, TOut, TOut>(source, view, viewExpression);

        return new ReactiveBinding<TView, TOut>(
            view,
            viewExpression,
            viewModelExpression,
            obs,
            BindingDirection.OneWay,
            disposable);
    }

    /// <inheritdoc />
    public IDisposable BindTo<TValue, TTarget, TTargetValue>(
        IObservable<TValue> observedChange,
        TTarget? target,
        Expression<Func<TTarget, TTargetValue?>> propertyExpression)
        where TTarget : class =>
        BindTo(observedChange, target, propertyExpression, null, null);

    /// <inheritdoc />
    public IDisposable BindTo<TValue, TTarget, TTargetValue>(
        IObservable<TValue> observedChange,
        TTarget? target,
        Expression<Func<TTarget, TTargetValue?>> propertyExpression,
        object? conversionHint)
        where TTarget : class =>
        BindTo(observedChange, target, propertyExpression, conversionHint, null);

    /// <inheritdoc />
    public IDisposable BindTo<TValue, TTarget, TTargetValue>(
        IObservable<TValue> observedChange,
        TTarget? target,
        Expression<Func<TTarget, TTargetValue?>> propertyExpression,
        object? conversionHint,
        IBindingTypeConverter? viewModelToViewConverterOverride)
        where TTarget : class
    {
        ArgumentExceptionHelper.ThrowIfNull(target);
        ArgumentExceptionHelper.ThrowIfNull(propertyExpression);

        var viewExpression = Reflection.Rewrite(propertyExpression.Body);

        var shouldBind = target is not IViewFor viewFor ||
                         _hookEvaluator.EvaluateBindingHooks<object, IViewFor>(
                             null,
                             viewFor,
                             null!,
                             viewExpression,
                             BindingDirection.OneWay);
        if (!shouldBind)
        {
            return EmptyDisposable.Instance;
        }

        var converterObj = viewModelToViewConverterOverride ?? GetConverterForTypes(typeof(TValue), typeof(TTargetValue?));

        if (converterObj is not null)
        {
            var source = new ChooseObservable<TValue, object?>(
                observedChange,
                x =>
                {
                    var convertResult = BindingTypeConverterDispatch.TryConvertAny(
                        converterObj,
                        typeof(TValue),
                        x,
                        typeof(TTargetValue?),
                        conversionHint,
                        out var tmp);

                    if (convertResult)
                    {
                        return (true, tmp);
                    }

                    if (viewModelToViewConverterOverride is null && typeof(TTargetValue).IsAssignableFrom(typeof(TValue)))
                    {
                        return (true, (object?)x);
                    }

                    return (false, null);
                });

            var (disposable, _) = BindToDirect<TTarget, TTargetValue?, object?>(source, target, viewExpression);
            return disposable;
        }

        if (typeof(TTargetValue).IsAssignableFrom(typeof(TValue)))
        {
            var source = new SelectObservable<TValue, object?>(observedChange, static x => x);
            var (disposable, _) = BindToDirect<TTarget, TTargetValue?, object?>(source, target, viewExpression);
            return disposable;
        }

        throw new ArgumentException(
            $"Can't convert {typeof(TValue)} to {typeof(TTargetValue)}. To fix this, register a IBindingTypeConverter");
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
    /// Schedules a two-way change signal before the view is read or written. The default is a synchronous
    /// pass-through; platform binders (e.g. WPF) override this to marshal onto the UI thread, and only when
    /// off-thread.
    /// </summary>
    /// <typeparam name="TView">The view type.</typeparam>
    /// <param name="view">The view participating in the binding.</param>
    /// <param name="value">The change signal to forward once scheduled.</param>
    /// <returns>An observable that emits <paramref name="value"/> when the binding should continue.</returns>
    protected virtual IObservable<bool> ScheduleForBinding<TView>(TView view, bool value)
        where TView : class =>
        new SingleValueObservable<bool>(value);

    /// <summary>
    /// Applies a view-value setter. The default invokes it inline; platform binders (e.g. WPF) override this to
    /// marshal onto the UI thread, and only when off-thread.
    /// </summary>
    /// <typeparam name="TView">The view type.</typeparam>
    /// <param name="view">The view being updated.</param>
    /// <param name="setter">The view-value setter to invoke.</param>
    protected virtual void SetViewValue<TView>(TView view, Action setter)
        where TView : class
    {
        ArgumentExceptionHelper.ThrowIfNull(setter);

        setter();
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
    [SuppressMessage(
        "Major Code Smell",
        "S4018:Generic methods should provide type parameter",
        Justification = "Generic type parameter is supplied explicitly by the caller by design; it identifies the target type and cannot be inferred from the method's parameters.")]
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
        var getter = Reflection.GetValueFetcherOrThrow(memberInfo) ??
                     throw new InvalidOperationException("getter was not found.");

        var setObservableWithEmit =
            _expressionCompiler.IsDirectMemberAccess(viewExpression)
                ? _expressionCompiler.CreateDirectSetObservable<TTarget, TValue, TObs>(
                    target,
                    changeObservable,
                    viewExpression,
                    getter,
                    setter,
                    _converterResolver.GetSetMethodConverter) : _expressionCompiler.CreateChainedSetObservable<TTarget, TValue, TObs>(
                    target,
                    changeObservable,
                    viewExpression,
                    _expressionCompiler.GetExpressionChainArray(viewExpression.GetParent()!) ?? [],
                    getter,
                    setter,
                    _converterResolver.GetSetMethodConverter);

        var setObservable = new SelectObservable<(bool ShouldEmit, TValue Value), TValue>(setObservableWithEmit, static x => x.Value);
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
    private IDisposable SubscribeWithBindingErrorHandling<TValue>(
        IObservable<TValue> setObservable,
        Expression viewExpression)
    {
        ArgumentExceptionHelper.ThrowIfNull(setObservable);
        ArgumentExceptionHelper.ThrowIfNull(viewExpression);

        return setObservable.Subscribe(new DelegateObserver<TValue>(
            static _ => { },
            ex =>
            {
                this.Log().Error(ex, $"{viewExpression} Binding received an Exception!");
                if (ex.InnerException is null)
                {
                    return;
                }

                throw new TargetInvocationException(
                    $"{viewExpression} Binding received an Exception!",
                    ex.InnerException);
            }));
    }

    /// <summary>
    /// Core two-way binding implementation that wires up view-model-to-view and view-to-view-model change pipelines.
    /// </summary>
    /// <typeparam name="TViewModel">The type of the view model.</typeparam>
    /// <typeparam name="TView">The type of the view.</typeparam>
    /// <typeparam name="TViewModelPropertyType">The type of the view model property.</typeparam>
    /// <typeparam name="TViewPropertyType">The type of the view property.</typeparam>
    /// <typeparam name="TDontCare">A dummy type used only to signal view updates.</typeparam>
    /// <param name="request">The bundled inputs describing the two-way binding to create.</param>
    /// <returns>The configured two-way reactive binding, or null if hooks blocked the binding.</returns>
    private ReactiveBinding<TView, (object? view, bool isViewModel)> BindImpl<
        TViewModel,
        TView,
        TViewModelPropertyType,
        TViewPropertyType,
        TDontCare>(in TwoWayBindRequest<TViewModel, TView, TViewModelPropertyType, TViewPropertyType, TDontCare> request)
        where TViewModel : class
        where TView : class, IViewFor
    {
        ArgumentExceptionHelper.ThrowIfNull(request.ViewModelProperty);
        ArgumentExceptionHelper.ThrowIfNull(request.ViewProperty);

        var view = request.View;
        BroadcastSubject<bool> signalInitialUpdate = new();
        var viewModelExpression = Reflection.Rewrite(request.ViewModelProperty.Body);
        var viewExpression = Reflection.Rewrite(request.ViewProperty.Body);

        Expression[] viewModelExpressionChainArray = [.. viewModelExpression.GetExpressionChain()];
        Expression[] viewExpressionChainArray = [.. viewExpression.GetExpressionChain()];

        Reflection.CompiledPropertyChain<object?, TViewModelPropertyType> viewModelChainGetter = new(viewModelExpressionChainArray);
        Reflection.CompiledPropertyChain<TView, TViewPropertyType> viewChainGetter = new(viewExpressionChainArray);
        Reflection.CompiledPropertyChainSetter<TView, object?> viewChainSetter = new(viewExpressionChainArray);
        Reflection.CompiledPropertyChainSetter<object?, object?> viewModelChainSetter = new(viewModelExpressionChainArray);

        var viewModelToViewConverter = request.ViewModelToViewConverter;
        var viewToViewModelConverter = request.ViewToViewModelConverter;

        var viewChanges = new SelectObservable<TViewPropertyType?, bool>(
            view.WhenAnyDynamic(viewExpression, x => (TViewPropertyType?)x.Value),
            static _ => false);

        var somethingChanged = BuildChangeSource(
            request.TriggerUpdate,
            request.SignalViewUpdate,
            request.ViewModel,
            view,
            viewModelExpression,
            viewChanges,
            signalInitialUpdate);

        var changeWithValues = new SelectObservable<bool, (bool isValid, object? view, bool isViewModel)>(
            new ScheduledChangeObservable<TView>(somethingChanged, this, view),
            isViewModelChange =>
                ProjectChange(isViewModelChange, view, viewModelChainGetter, viewChainGetter, viewModelToViewConverter, viewToViewModelConverter));

        var ret = _hookEvaluator.EvaluateBindingHooks(
            request.ViewModel,
            view,
            viewModelExpression,
            viewExpression,
            BindingDirection.TwoWay);
        if (!ret)
        {
            return null!;
        }

        // Filter to valid changes and project to the (view, isViewModel) pair, then multicast through a shared subject
        // (Publish + RefCount) so the internal setter and external subscribers share one upstream subscription.
        var projected = new ChooseObservable<(bool isValid, object? view, bool isViewModel), (object? view, bool isViewModel)>(
            changeWithValues,
            value => value.isValid ? (true, (value.view, value.isViewModel)) : (false, default));

        var changes = new BroadcastSubject<(object? view, bool isViewModel)>();
        var upstreamConnection = projected.Subscribe(changes);

        var setterSubscription = changes.Subscribe(new DelegateObserver<(object? view, bool isViewModel)>(latestValue =>
        {
            if (latestValue.isViewModel)
            {
                SetViewValue(view, () => viewChainSetter.TrySetValue(view, latestValue.view, false));
            }
            else
            {
                viewModelChainSetter.TrySetValue(view.ViewModel, latestValue.view, false);
            }
        }));

        signalInitialUpdate.OnNext(true);

        return new(
            view,
            viewExpression,
            viewModelExpression,
            changes,
            BindingDirection.TwoWay,
            new CompositeDisposable(upstreamConnection, setterSubscription));
    }

    /// <summary>
    /// Routes each two-way change signal through <see cref="ScheduleForBinding{TView}"/> so the subsequent view
    /// read and write run where the platform binder requires (e.g. the WPF dispatcher). A fused
    /// <c>SelectMany</c>-over-single: it forwards each scheduled value, ignores the inner completion, and completes
    /// only when the source does.
    /// </summary>
    /// <typeparam name="TView">The view type.</typeparam>
    /// <param name="source">The change-signal source.</param>
    /// <param name="owner">The binder providing the scheduling hook.</param>
    /// <param name="view">The view participating in the binding.</param>
    private sealed class ScheduledChangeObservable<TView>(IObservable<bool> source, PropertyBinderImplementation owner, TView view)
        : IObservable<bool>
        where TView : class
    {
        /// <inheritdoc/>
        public IDisposable Subscribe(IObserver<bool> observer)
        {
            ArgumentExceptionHelper.ThrowIfNull(observer);

            return source.Subscribe(new Sink(observer, owner, view));
        }

        /// <summary>Forwards each source signal through the binder's scheduling hook.</summary>
        /// <param name="downstream">The downstream observer.</param>
        /// <param name="owner">The binder providing the scheduling hook.</param>
        /// <param name="view">The view participating in the binding.</param>
        private sealed class Sink(IObserver<bool> downstream, PropertyBinderImplementation owner, TView view) : IObserver<bool>
        {
            /// <inheritdoc/>
            public void OnNext(bool value) =>
                owner.ScheduleForBinding(view, value)
                    .Subscribe(new DelegateObserver<bool>(downstream.OnNext, downstream.OnError));

            /// <inheritdoc/>
            public void OnError(Exception error) => downstream.OnError(error);

            /// <inheritdoc/>
            public void OnCompleted() => downstream.OnCompleted();
        }
    }

    /// <summary>Projects each value of a source through a selector. Specialised binding projection; no generic operator.</summary>
    /// <typeparam name="TIn">The source element type.</typeparam>
    /// <typeparam name="TOut">The projected element type.</typeparam>
    /// <param name="source">The source observable.</param>
    /// <param name="selector">Projects a source value into a result.</param>
    private sealed class SelectObservable<TIn, TOut>(IObservable<TIn> source, Func<TIn, TOut> selector) : IObservable<TOut>
    {
        /// <inheritdoc/>
        public IDisposable Subscribe(IObserver<TOut> observer)
        {
            ArgumentExceptionHelper.ThrowIfNull(observer);
            return source.Subscribe(new Sink(observer, selector));
        }

        /// <summary>Applies the selector to each value and forwards the result.</summary>
        /// <param name="downstream">The observer receiving projected values.</param>
        /// <param name="selector">Projects a source value into a result.</param>
        private sealed class Sink(IObserver<TOut> downstream, Func<TIn, TOut> selector) : IObserver<TIn>
        {
            /// <inheritdoc/>
            public void OnNext(TIn value)
            {
                TOut result;
                try
                {
                    result = selector(value);
                }
                catch (Exception ex)
                {
                    downstream.OnError(ex);
                    return;
                }

                downstream.OnNext(result);
            }

            /// <inheritdoc/>
            public void OnError(Exception error) => downstream.OnError(error);

            /// <inheritdoc/>
            public void OnCompleted() => downstream.OnCompleted();
        }
    }

    /// <summary>
    /// For each source value, applies a chooser that either yields a value to forward or signals "skip". Fuses the
    /// binding's convert-or-skip and filter-then-project pipelines into one specialised sink.
    /// </summary>
    /// <typeparam name="TIn">The source element type.</typeparam>
    /// <typeparam name="TOut">The forwarded element type.</typeparam>
    /// <param name="source">The source observable.</param>
    /// <param name="chooser">Maps a source value to (forward, value); when forward is false the value is skipped.</param>
    private sealed class ChooseObservable<TIn, TOut>(IObservable<TIn> source, Func<TIn, (bool HasValue, TOut Value)> chooser) : IObservable<TOut>
    {
        /// <inheritdoc/>
        public IDisposable Subscribe(IObserver<TOut> observer)
        {
            ArgumentExceptionHelper.ThrowIfNull(observer);
            return source.Subscribe(new Sink(observer, chooser));
        }

        /// <summary>Applies the chooser to each value and forwards only the chosen ones.</summary>
        /// <param name="downstream">The observer receiving chosen values.</param>
        /// <param name="chooser">Maps a source value to (forward, value).</param>
        private sealed class Sink(IObserver<TOut> downstream, Func<TIn, (bool HasValue, TOut Value)> chooser) : IObserver<TIn>
        {
            /// <inheritdoc/>
            public void OnNext(TIn value)
            {
                (bool HasValue, TOut Value) result;
                try
                {
                    result = chooser(value);
                }
                catch (Exception ex)
                {
                    downstream.OnError(ex);
                    return;
                }

                if (!result.HasValue)
                {
                    return;
                }

                downstream.OnNext(result.Value);
            }

            /// <inheritdoc/>
            public void OnError(Exception error) => downstream.OnError(error);

            /// <inheritdoc/>
            public void OnCompleted() => downstream.OnCompleted();
        }
    }

    /// <summary>Forwards the values of every source and completes once all of them have completed. Specialised binding merge.</summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <param name="sources">The sources to merge.</param>
    private sealed class MergeObservable<T>(IObservable<T>[] sources) : IObservable<T>
    {
        /// <inheritdoc/>
        public IDisposable Subscribe(IObserver<T> observer)
        {
            ArgumentExceptionHelper.ThrowIfNull(observer);
            return new Sink(observer, sources);
        }

        /// <summary>Forwards every source value under a gate and completes once all sources complete.</summary>
        private sealed class Sink : IDisposable
        {
            /// <summary>Guards downstream delivery and the completion counter.</summary>
            #if NET9_0_OR_GREATER
            private readonly Lock _gate = new();
            #else
            private readonly object _gate = new();
            #endif

            /// <summary>The observer receiving the merged values.</summary>
            private readonly IObserver<T> _downstream;

            /// <summary>The subscriptions to each source.</summary>
            private readonly IDisposable?[] _subscriptions;

            /// <summary>The number of sources.</summary>
            private readonly int _count;

            /// <summary>The number of sources that have completed.</summary>
            private int _doneCount;

            /// <summary>Whether the downstream has terminated.</summary>
            private bool _stopped;

            /// <summary>Initializes a new instance of the <see cref="Sink"/> class and subscribes to every source.</summary>
            /// <param name="downstream">The observer receiving the merged values.</param>
            /// <param name="sources">The sources to merge.</param>
            public Sink(IObserver<T> downstream, IObservable<T>[] sources)
            {
                _downstream = downstream;
                _count = sources.Length;
                _subscriptions = new IDisposable?[sources.Length];
                for (var i = 0; i < sources.Length; i++)
                {
                    _subscriptions[i] = sources[i].Subscribe(new Element(this));
                }
            }

            /// <inheritdoc/>
            public void Dispose()
            {
                for (var i = 0; i < _subscriptions.Length; i++)
                {
                    _subscriptions[i]?.Dispose();
                }
            }

            /// <summary>Forwards one source value to the downstream.</summary>
            /// <param name="value">The value to forward.</param>
            private void OnNextAt(T value)
            {
                lock (_gate)
                {
                    if (_stopped)
                    {
                        return;
                    }

                    _downstream.OnNext(value);
                }
            }

            /// <summary>Forwards an error from any source.</summary>
            /// <param name="error">The error to forward.</param>
            private void OnErrorAt(Exception error)
            {
                lock (_gate)
                {
                    if (_stopped)
                    {
                        return;
                    }

                    _stopped = true;
                }

                _downstream.OnError(error);
            }

            /// <summary>Completes the downstream once every source has completed.</summary>
            private void OnCompletedAt()
            {
                lock (_gate)
                {
                    if (_stopped || ++_doneCount < _count)
                    {
                        return;
                    }

                    _stopped = true;
                }

                _downstream.OnCompleted();
            }

            /// <summary>Routes one source's notifications to the parent sink.</summary>
            /// <param name="parent">The owning sink.</param>
            private sealed class Element(Sink parent) : IObserver<T>
            {
                /// <inheritdoc/>
                public void OnNext(T value) => parent.OnNextAt(value);

                /// <inheritdoc/>
                public void OnError(Exception error) => parent.OnErrorAt(error);

                /// <inheritdoc/>
                public void OnCompleted() => parent.OnCompletedAt();
            }
        }
    }

    /// <summary>Forwards the first value of a source then completes and unsubscribes. Specialised binding <c>Take(1)</c>.</summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <param name="source">The source observable.</param>
    private sealed class Take1Observable<T>(IObservable<T> source) : IObservable<T>
    {
        /// <inheritdoc/>
        public IDisposable Subscribe(IObserver<T> observer)
        {
            ArgumentExceptionHelper.ThrowIfNull(observer);
            var sink = new Sink(observer);
            return sink.Run(source);
        }

        /// <summary>Forwards the first value, then completes the downstream and disposes the subscription.</summary>
        private sealed class Sink(IObserver<T> downstream) : IObserver<T>, IDisposable
        {
            /// <summary>The subscription to the source.</summary>
            private IDisposable? _subscription;

            /// <summary>Whether the first value has been delivered.</summary>
            private bool _done;

            /// <summary>Subscribes to the source.</summary>
            /// <param name="source">The source observable.</param>
            /// <returns>The sink, which disposes the run.</returns>
            public Sink Run(IObservable<T> source)
            {
                _subscription = source.Subscribe(this);
                return this;
            }

            /// <inheritdoc/>
            public void OnNext(T value)
            {
                if (_done)
                {
                    return;
                }

                _done = true;
                downstream.OnNext(value);
                downstream.OnCompleted();
                Dispose();
            }

            /// <inheritdoc/>
            public void OnError(Exception error)
            {
                if (_done)
                {
                    return;
                }

                downstream.OnError(error);
            }

            /// <inheritdoc/>
            public void OnCompleted()
            {
                if (_done)
                {
                    return;
                }

                downstream.OnCompleted();
            }

            /// <inheritdoc/>
            public void Dispose() => _subscription?.Dispose();
        }
    }
}
