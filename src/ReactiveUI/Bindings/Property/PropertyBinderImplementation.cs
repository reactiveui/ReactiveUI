// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Globalization;
using System.Reflection;

namespace ReactiveUI;

/// <summary>
/// Provides methods to bind properties to observables.
/// </summary>
public class PropertyBinderImplementation : IPropertyBinderImplementation
{
    private static readonly MemoizingMRUCache<(Type fromType, Type toType), IBindingTypeConverter?> _typeConverterCache = new(
     (types, _) => AppLocator.Current.GetServices<IBindingTypeConverter?>()
                          .Aggregate((currentAffinity: -1, currentBinding: default(IBindingTypeConverter)), (acc, x) =>
                          {
                              var score = x?.GetAffinityForObjects(types.fromType, types.toType) ?? -1;
                              return score > acc.currentAffinity && score > 0 ? (score, x) : acc;
                          }).currentBinding,
     RxApp.SmallCacheLimit);

    [SuppressMessage("AOT", "IL3050:Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.", Justification = "Marked as Preserve")]
    [SuppressMessage("Trimming", "IL2026:Calling members annotated with 'RequiresUnreferencedCodeAttribute' may break functionality when trimming application code.", Justification = "Marked as Preserve")]
    private static readonly MemoizingMRUCache<(Type? fromType, Type? toType), ISetMethodBindingConverter?> _setMethodCache = new(
     (type, _) => AppLocator.Current.GetServices<ISetMethodBindingConverter>()
                         .Aggregate((currentAffinity: -1, currentBinding: default(ISetMethodBindingConverter)), (acc, x) =>
                         {
                             var score = x.GetAffinityForObjects(type.fromType, type.toType);
                             return score > acc.currentAffinity && score > 0 ? (score, x) : acc;
                         }).currentBinding,
     RxApp.SmallCacheLimit);

    static PropertyBinderImplementation() => RxApp.EnsureInitialized();

    private delegate bool OutFunc<in T1, T2>(T1 t1, out T2 t2);

    /// <inheritdoc />
#if NET6_0_OR_GREATER
    [RequiresDynamicCode("The handler may use serialization which requires dynamic code generation")]
    [RequiresUnreferencedCode("The handler may use serialization which may require unreferenced code")]
#endif
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
        vmProperty.ArgumentNullExceptionThrowIfNull(nameof(vmProperty));
        viewProperty.ArgumentNullExceptionThrowIfNull(nameof(viewProperty));
        var vmToViewConverter = vmToViewConverterOverride ?? GetConverterForTypes(typeof(TVMProp), typeof(TVProp));
        var viewToVMConverter = viewToVMConverterOverride ?? GetConverterForTypes(typeof(TVProp), typeof(TVMProp));

        if (vmToViewConverter is null || viewToVMConverter is null)
        {
            throw new ArgumentException(
                                        $"Can't two-way convert between {typeof(TVMProp)} and {typeof(TVProp)}. To fix this, register a IBindingTypeConverter or call the version with the converter Func.");
        }

        bool VmToViewFunc(TVMProp? vmValue, out TVProp vValue)
        {
            var result = vmToViewConverter.TryConvert(vmValue, typeof(TVProp), conversionHint, out var tmp);

            vValue = result && tmp is not null ? (TVProp)tmp : default!;
            return result;
        }

        bool ViewToVmFunc(TVProp vValue, out TVMProp? vmValue)
        {
            var result = viewToVMConverter.TryConvert(vValue, typeof(TVMProp?), conversionHint, out var tmp);

            vmValue = result && tmp is not null ? (TVMProp?)tmp : default;
            return result;
        }

        return BindImpl(viewModel, view, vmProperty, viewProperty, signalViewUpdate, VmToViewFunc, ViewToVmFunc, triggerUpdate);
    }

    /// <inheritdoc />
#if NET6_0_OR_GREATER
    [RequiresDynamicCode("The handler may use serialization which requires dynamic code generation")]
    [RequiresUnreferencedCode("The handler may use serialization which may require unreferenced code")]
#endif
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
        vmProperty.ArgumentNullExceptionThrowIfNull(nameof(vmProperty));
        viewProperty.ArgumentNullExceptionThrowIfNull(nameof(viewProperty));
        vmToViewConverter.ArgumentNullExceptionThrowIfNull(nameof(vmToViewConverter));
        viewToVmConverter.ArgumentNullExceptionThrowIfNull(nameof(viewToVmConverter));

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
#if NET6_0_OR_GREATER
    [RequiresDynamicCode("The handler may use serialization which requires dynamic code generation")]
    [RequiresUnreferencedCode("The handler may use serialization which may require unreferenced code")]
#endif
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
        vmProperty.ArgumentNullExceptionThrowIfNull(nameof(vmProperty));
        viewProperty.ArgumentNullExceptionThrowIfNull(nameof(viewProperty));

        var vmExpression = Reflection.Rewrite(vmProperty.Body);
        var viewExpression = Reflection.Rewrite(viewProperty.Body);
        var viewType = viewExpression.Type;
        var converter = (vmToViewConverterOverride ?? GetConverterForTypes(typeof(TVMProp?), viewType)) ?? throw new ArgumentException($"Can't convert {typeof(TVMProp)} to {viewType}. To fix this, register a IBindingTypeConverter");
        var ret = EvalBindingHooks(viewModel, view, vmExpression, viewExpression, BindingDirection.OneWay);
        if (!ret)
        {
            return new ReactiveBinding<TView, TVProp>(view, viewExpression, vmExpression, Observable.Empty<TVProp>(), BindingDirection.OneWay, Disposable.Empty);
        }

        var source = Reflection.ViewModelWhenAnyValue(viewModel, view, vmExpression)
                               .SelectMany(x => !converter.TryConvert(x, viewType, conversionHint, out var tmp) ? Observable<object>.Empty : Observable.Return(tmp));

        var (disposable, obs) = BindToDirect<TView, TVProp, object?>(source, view, viewExpression);

        return new ReactiveBinding<TView, TVProp>(view, viewExpression, vmExpression, obs, BindingDirection.OneWay, disposable);
    }

    /// <inheritdoc />
#if NET6_0_OR_GREATER
    [RequiresDynamicCode("The handler may use serialization which requires dynamic code generation")]
    [RequiresUnreferencedCode("The handler may use serialization which may require unreferenced code")]
#endif
    public IReactiveBinding<TView, TOut> OneWayBind<TViewModel, TView, TProp, TOut>(
        TViewModel? viewModel,
        TView view,
        Expression<Func<TViewModel, TProp>> vmProperty,
        Expression<Func<TView, TOut>> viewProperty,
        Func<TProp, TOut> selector)
        where TViewModel : class
        where TView : class, IViewFor
    {
        vmProperty.ArgumentNullExceptionThrowIfNull(nameof(vmProperty));
        viewProperty.ArgumentNullExceptionThrowIfNull(nameof(viewProperty));

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
#if NET6_0_OR_GREATER
    [RequiresDynamicCode("The handler may use serialization which requires dynamic code generation")]
    [RequiresUnreferencedCode("The handler may use serialization which may require unreferenced code")]
#endif
    public IDisposable BindTo<TValue, TTarget, TTValue>(
        IObservable<TValue> observedChange,
        TTarget? target,
        Expression<Func<TTarget, TTValue?>> propertyExpression,
        object? conversionHint = null,
        IBindingTypeConverter? vmToViewConverterOverride = null)
        where TTarget : class
    {
        target.ArgumentNullExceptionThrowIfNull(nameof(target));
        propertyExpression.ArgumentNullExceptionThrowIfNull(nameof(propertyExpression));

        var viewExpression = Reflection.Rewrite(propertyExpression.Body);

        var shouldBind = target is not IViewFor viewFor || EvalBindingHooks(observedChange, viewFor, null!, viewExpression, BindingDirection.OneWay);
        if (!shouldBind)
        {
            return Disposable.Empty;
        }

        var converter = (vmToViewConverterOverride ?? GetConverterForTypes(typeof(TValue), typeof(TTValue?))) ?? throw new ArgumentException($"Can't convert {typeof(TValue)} to {typeof(TTValue)}. To fix this, register a IBindingTypeConverter");
        var source = observedChange.SelectMany(x => !converter.TryConvert(x, typeof(TTValue?), conversionHint, out var tmp) ? Observable<object>.Empty : Observable.Return(tmp));

        var (disposable, _) = BindToDirect<TTarget, TTValue?, object?>(source, target!, viewExpression);

        return disposable;
    }

    internal static IBindingTypeConverter? GetConverterForTypes(Type lhs, Type rhs) =>
        _typeConverterCache.Get((lhs, rhs));

#if NET6_0_OR_GREATER
    [RequiresDynamicCode("Type conversion requires dynamic code generation")]
    [RequiresUnreferencedCode("Type conversion may reference types that could be trimmed")]
#endif
    private static Func<object?, object?, object?[]?, object?>? GetSetConverter(Type? fromType, Type? targetType)
    {
        if (fromType is null)
        {
            return null;
        }

        var setter = _setMethodCache.Get((fromType, targetType));
        return setter is null ? null : setter.PerformSet;
    }

#if NET6_0_OR_GREATER
    [RequiresDynamicCode("Property binding requires dynamic code generation")]
    [RequiresUnreferencedCode("Property binding may reference members that could be trimmed")]
#endif
    private (IDisposable disposable, IObservable<TValue> value) BindToDirect<TTarget, TValue, TObs>(
        IObservable<TObs> changeObservable,
        TTarget target,
        Expression viewExpression)
        where TTarget : class
    {
        var memberInfo = viewExpression.GetMemberInfo();

        var defaultSetter = Reflection.GetValueSetterOrThrow(memberInfo);
        var defaultGetter = Reflection.GetValueFetcherOrThrow(memberInfo);
        var synchronizedChanges = changeObservable.Synchronize();

        (bool shouldEmit, object? value) SetThenGet(object? paramTarget, object? paramValue, object?[]? paramParams)
        {
            var converter = GetSetConverter(paramValue?.GetType(), viewExpression.Type);

            if (defaultGetter is null)
            {
                throw new InvalidOperationException($"{nameof(defaultGetter)} was not found.");
            }

            if (converter is null)
            {
                var currentValue = defaultGetter(paramTarget, paramParams);
                var shouldUpdate = !EqualityComparer<object?>.Default.Equals(currentValue, paramValue);

                if (!shouldUpdate)
                {
                    return (false, currentValue);
                }

                defaultSetter?.Invoke(paramTarget, paramValue, paramParams);
                return (true, defaultGetter(paramTarget, paramParams));
            }

            var value = defaultGetter(paramTarget, paramParams);
            var convertedValue = converter(value, paramValue, paramParams);
            var shouldEmit = !EqualityComparer<object?>.Default.Equals(value, convertedValue);

            return (shouldEmit, convertedValue);
        }

        IObservable<TValue> setObservable;

        if (viewExpression.GetParent()?.NodeType == ExpressionType.Parameter)
        {
            var arguments = viewExpression.GetArgumentsArray();
            setObservable = synchronizedChanges
                .Select(value => SetThenGet(target, value, arguments))
                .Where(result => result.shouldEmit)
                .Select(result => result.value is null ? default! : (TValue)result.value);
        }
        else
        {
            var hostExpression = viewExpression.GetParent();
            var hostExpressionChain = hostExpression?.GetExpressionChain()?.ToArray();
            var hostChanges = target.WhenAnyDynamic(hostExpression, x => x.Value);
            var arguments = viewExpression.GetArgumentsArray();
            var propertyDefaultValue = viewExpression.Type.GetTypeInfo().IsValueType ? Activator.CreateInstance(viewExpression.Type) : null;
            var shouldReplayOnHostChanges = hostExpressionChain?
                .OfType<MemberExpression>()
                .Any(static expression => string.Equals(expression.Member.Name, nameof(IViewFor.ViewModel), StringComparison.Ordinal)) != true;

            setObservable = Observable.Create<TValue>(observer =>
            {
                object? latestHost = null;
                object? lastObservedValue = null;
                object? currentHost = null;
                var hasObservedValue = false;

                bool HostPropertyEqualsDefault(object? host)
                {
                    if (host is null || defaultGetter is null)
                    {
                        return false;
                    }

                    var currentValue = defaultGetter(host, arguments);
                    return EqualityComparer<object?>.Default.Equals(currentValue, propertyDefaultValue);
                }

                void ApplyValueToHost(object? host, object? value)
                {
                    if (host is null || !hasObservedValue)
                    {
                        return;
                    }

                    var (shouldEmit, result) = SetThenGet(host, value, arguments);
                    if (!shouldEmit)
                    {
                        return;
                    }

                    observer.OnNext(result is null ? default! : (TValue)result);
                }

                var hostDisposable = hostChanges
                    .Subscribe(
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

                var changeDisposable = synchronizedChanges
                    .Subscribe(
                        value =>
                        {
                            hasObservedValue = true;
                            lastObservedValue = value;

                            var host = latestHost;
                            if (hostExpressionChain is not null)
                            {
                                if (!Reflection.TryGetValueForPropertyChain(out host, target, hostExpressionChain))
                                {
                                    host = null;
                                }

                                latestHost = host;
                                currentHost = host;
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

        return (setObservable.Subscribe(_ => { }, ex =>
        {
            this.Log().Error(ex, $"{viewExpression} Binding received an Exception!");
            if (ex.InnerException is null)
            {
                return;
            }

            // If the exception is not null, we throw it wrapped in a TargetInvocationException.
            throw new TargetInvocationException($"{viewExpression} Binding received an Exception!", ex.InnerException);
        }), setObservable);
    }

#if NET6_0_OR_GREATER
    [RequiresDynamicCode("Property binding requires dynamic code generation")]
    [RequiresUnreferencedCode("Property binding may reference members that could be trimmed")]
#endif
    private bool EvalBindingHooks<TViewModel, TView>(TViewModel? viewModel, TView view, Expression vmExpression, Expression viewExpression, BindingDirection direction)
        where TViewModel : class
        where TView : class, IViewFor
    {
        var hooks = AppLocator.Current.GetServices<IPropertyBindingHook>();
        view.ArgumentNullExceptionThrowIfNull(nameof(view));

        Func<IObservedChange<object, object?>[]> vmFetcher = vmExpression is not null
            ? (() =>
                {
                    Reflection.TryGetAllValuesForPropertyChain(out var fetchedValues, viewModel, vmExpression.GetExpressionChain());
                    return fetchedValues;
                })
            : (() =>
                  [
                      new ObservedChange<object, object?>(null!, null, viewModel)
                  ]);

        var vFetcher = new Func<IObservedChange<object, object?>[]>(() =>
        {
            Reflection.TryGetAllValuesForPropertyChain(out var fetchedValues, view, viewExpression.GetExpressionChain());
            return fetchedValues;
        });

        var shouldBind = hooks.Aggregate(true, (acc, x) =>
                                             acc && x.ExecuteHook(viewModel, view!, vmFetcher!, vFetcher!, direction));

        if (!shouldBind)
        {
            var vmString = $"{typeof(TViewModel).Name}.{string.Join(".", vmExpression)}";
            var vString = $"{typeof(TView).Name}.{string.Join(".", viewExpression)}";
            this.Log().Warn(CultureInfo.InvariantCulture, "Binding hook asked to disable binding {0} => {1}", vmString, vString);
        }

        return shouldBind;
    }

#if NET6_0_OR_GREATER
    [RequiresDynamicCode("Property binding requires dynamic code generation")]
    [RequiresUnreferencedCode("Property binding may reference members that could be trimmed")]
#endif
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
        vmProperty.ArgumentNullExceptionThrowIfNull(nameof(vmProperty));

        viewProperty.ArgumentNullExceptionThrowIfNull(nameof(viewProperty));

        var signalInitialUpdate = new Subject<bool>();
        var vmExpression = Reflection.Rewrite(vmProperty.Body);
        var viewExpression = Reflection.Rewrite(viewProperty.Body);

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
                !Reflection.TryGetValueForPropertyChain(out TVMProp vmValue, view.ViewModel, vmExpression.GetExpressionChain()) ||
                !Reflection.TryGetValueForPropertyChain(out TVProp vValue, view, viewExpression.GetExpressionChain())
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
                                                    signalViewUpdate is null ?
                                                        Reflection.ViewModelWhenAnyValue(viewModel, view, vmExpression).Select(_ => true) :
                                                        signalViewUpdate.Select(_ => true)
                                                                        .Merge(Reflection.ViewModelWhenAnyValue(viewModel, view, vmExpression).Select(_ => true).Take(1)),
                                                    signalInitialUpdate.Select(_ => true),
                                                    view.WhenAnyDynamic(viewExpression, x => (TVProp?)x.Value).Select(_ => false));

            changeWithValues = somethingChanged
                .Select<bool, (bool isValid, object? view, bool isViewModel)>(isVm =>
                                                                                  !Reflection.TryGetValueForPropertyChain(out TVMProp vmValue, view.ViewModel, vmExpression.GetExpressionChain()) ||
                                                                                  !Reflection.TryGetValueForPropertyChain(out TVProp vValue, view, viewExpression.GetExpressionChain())
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

        var changes = changeWithValues
                      .Where(value => value.isValid)
                      .Select(value => (value.view, value.isViewModel))
                      .Publish()
                      .RefCount();

        var disposable = changes.Subscribe(isVmWithLatestValue =>
        {
            if (isVmWithLatestValue.isViewModel)
            {
                Reflection.TrySetValueToPropertyChain(view, viewExpression.GetExpressionChain(), isVmWithLatestValue.view, false);
            }
            else
            {
                Reflection.TrySetValueToPropertyChain(view.ViewModel, vmExpression.GetExpressionChain(), isVmWithLatestValue.view, false);
            }
        });

        // NB: Even though it's technically a two-way bind, most people
        // want the ViewModel to win at first.
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
