// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;

using Splat;

namespace ReactiveUI
{
    /// <summary>
    /// Provides methods to bind properties to observables.
    /// </summary>
    public class PropertyBinderImplementation : IPropertyBinderImplementation
    {
        private static readonly MemoizingMRUCache<(Type fromType, Type toType), IBindingTypeConverter?> _typeConverterCache = new(
            (types, _) => Locator.Current.GetServices<IBindingTypeConverter?>()
                    .Aggregate((currentAffinity: -1, currentBinding: default(IBindingTypeConverter)), (acc, x) =>
                    {
                        var score = x?.GetAffinityForObjects(types.fromType, types.toType) ?? -1;
                        return score > acc.currentAffinity && score > 0 ? (score, x) : acc;
                    }).currentBinding,
            RxApp.SmallCacheLimit);

        private static readonly MemoizingMRUCache<(Type? fromType, Type? toType), ISetMethodBindingConverter?> _setMethodCache = new(
            (type, _) => Locator.Current.GetServices<ISetMethodBindingConverter>()
                    .Aggregate((currentAffinity: -1, currentBinding: default(ISetMethodBindingConverter)), (acc, x) =>
                    {
                        var score = x.GetAffinityForObjects(type.fromType, type.toType);
                        return score > acc.currentAffinity && score > 0 ? (score, x) : acc;
                    }).currentBinding,
            RxApp.SmallCacheLimit);

        static PropertyBinderImplementation() => RxApp.EnsureInitialized();

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
                IBindingTypeConverter? viewToVMConverterOverride = null)
            where TViewModel : class
            where TView : class, IViewFor
        {
            var vmToViewConverter = vmToViewConverterOverride ?? GetConverterForTypes(typeof(TVMProp), typeof(TVProp));
            var viewToVMConverter = viewToVMConverterOverride ?? GetConverterForTypes(typeof(TVProp), typeof(TVMProp));

            if (vmToViewConverter == null || viewToVMConverter == null)
            {
                throw new ArgumentException(
                    $"Can't two-way convert between {typeof(TVMProp)} and {typeof(TVProp)}. To fix this, register a IBindingTypeConverter or call the version with the converter Func.");
            }

            bool VmToViewFunc(TVMProp? vmValue, out TVProp vValue)
            {
                var result = vmToViewConverter.TryConvert(vmValue, typeof(TVProp), conversionHint, out var tmp);

                vValue = result && tmp != null ? (TVProp)tmp : default!;
                return result;
            }

            bool ViewToVmFunc(TVProp vValue, out TVMProp? vmValue)
            {
                var result = viewToVMConverter.TryConvert(vValue, typeof(TVMProp?), conversionHint, out var tmp);

                vmValue = result && tmp != null ? (TVMProp?)tmp : default;
                return result;
            }

            return BindImpl(viewModel, view, vmProperty, viewProperty, signalViewUpdate, VmToViewFunc, ViewToVmFunc);
        }

        /// <inheritdoc />
        public IReactiveBinding<TView, (object? view, bool isViewModel)> Bind<TViewModel, TView, TVMProp, TVProp, TDontCare>(
                TViewModel? viewModel,
                TView view,
                Expression<Func<TViewModel, TVMProp?>> vmProperty,
                Expression<Func<TView, TVProp>> viewProperty,
                IObservable<TDontCare>? signalViewUpdate,
                Func<TVMProp?, TVProp> vmToViewConverter,
                Func<TVProp, TVMProp?> viewToVmConverter)
            where TViewModel : class
            where TView : class, IViewFor
        {
            if (vmProperty == null)
            {
                throw new ArgumentNullException(nameof(vmProperty));
            }

            if (viewProperty == null)
            {
                throw new ArgumentNullException(nameof(viewProperty));
            }

            if (vmToViewConverter == null)
            {
                throw new ArgumentNullException(nameof(vmToViewConverter));
            }

            if (viewToVmConverter == null)
            {
                throw new ArgumentNullException(nameof(viewToVmConverter));
            }

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

            return BindImpl(viewModel, view, vmProperty, viewProperty, signalViewUpdate, VmToViewFunc, ViewToVmFunc);
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
            if (vmProperty == null)
            {
                throw new ArgumentNullException(nameof(vmProperty));
            }

            if (viewProperty == null)
            {
                throw new ArgumentNullException(nameof(viewProperty));
            }

            var vmExpression = Reflection.Rewrite(vmProperty.Body);
            var viewExpression = Reflection.Rewrite(viewProperty.Body);
            var viewType = viewExpression.Type;
            var converter = vmToViewConverterOverride ?? GetConverterForTypes(typeof(TVMProp?), viewType);

            if (converter == null)
            {
                throw new ArgumentException($"Can't convert {typeof(TVMProp)} to {viewType}. To fix this, register a IBindingTypeConverter");
            }

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
        public IReactiveBinding<TView, TOut> OneWayBind<TViewModel, TView, TProp, TOut>(
            TViewModel? viewModel,
            TView view,
            Expression<Func<TViewModel, TProp?>> vmProperty,
            Expression<Func<TView, TOut>> viewProperty,
            Func<TProp?, TOut> selector)
            where TViewModel : class
            where TView : class, IViewFor
        {
            if (vmProperty == null)
            {
                throw new ArgumentNullException(nameof(vmProperty));
            }

            if (viewProperty == null)
            {
                throw new ArgumentNullException(nameof(viewProperty));
            }

            var vmExpression = Reflection.Rewrite(vmProperty.Body);
            var viewExpression = Reflection.Rewrite(viewProperty.Body);
            var ret = EvalBindingHooks(viewModel, view, vmExpression, viewExpression, BindingDirection.OneWay);
            if (!ret)
            {
                return new ReactiveBinding<TView, TOut>(view, viewExpression, vmExpression, Observable.Empty<TOut>(), BindingDirection.OneWay, Disposable.Empty);
            }

            var source = Reflection.ViewModelWhenAnyValue(viewModel, view, vmExpression).Cast<TProp?>().Select(selector);

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
            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            if (propertyExpression == null)
            {
                throw new ArgumentNullException(nameof(propertyExpression));
            }

            var viewExpression = Reflection.Rewrite(propertyExpression.Body);

            var ret = EvalBindingHooks(observedChange, target, null!, viewExpression, BindingDirection.OneWay);
            if (!ret)
            {
                return Disposable.Empty;
            }

            var converter = vmToViewConverterOverride ?? GetConverterForTypes(typeof(TValue), typeof(TTValue?));

            if (converter == null)
            {
                throw new ArgumentException($"Can't convert {typeof(TValue)} to {typeof(TTValue)}. To fix this, register a IBindingTypeConverter");
            }

            var source = observedChange.SelectMany(x => !converter.TryConvert(x, typeof(TTValue?), conversionHint, out var tmp) ? Observable<object>.Empty : Observable.Return(tmp));

            var (disposable, _) = BindToDirect<TTarget, TTValue?, object?>(source, target, viewExpression);

            return disposable;
        }

        internal static IBindingTypeConverter? GetConverterForTypes(Type lhs, Type rhs) =>
            _typeConverterCache.Get((lhs, rhs));

        private static Func<object?, object?, object?[]?, object?>? GetSetConverter(Type? fromType, Type? targetType)
        {
            if (fromType == null)
            {
                return null;
            }

            var setter = _setMethodCache.Get((fromType, targetType));

#pragma warning disable IDE0031 // Use null propagation
            return setter == null ? null : setter.PerformSet;
#pragma warning restore IDE0031 // Use null propagation
        }

        private (IDisposable disposable, IObservable<TValue> value) BindToDirect<TTarget, TValue, TObs>(
                IObservable<TObs> changeObservable,
                TTarget target,
                Expression viewExpression)
            where TTarget : class
        {
            var defaultSetter = Reflection.GetValueSetterOrThrow(viewExpression.GetMemberInfo());
            var defaultGetter = Reflection.GetValueFetcherOrThrow(viewExpression.GetMemberInfo());

            object? SetThenGet(object? paramTarget, object? paramValue, object?[]? paramParams)
            {
                var converter = GetSetConverter(paramValue?.GetType(), viewExpression.Type);

                if (defaultGetter == null)
                {
                    throw new InvalidOperationException($"{nameof(defaultGetter)} was not found.");
                }

                if (converter == null)
                {
                    defaultSetter?.Invoke(paramTarget, paramValue, paramParams);
                    return defaultGetter(paramTarget, paramParams);
                }

                var value = defaultGetter(paramTarget, paramParams);
                return converter(value, paramValue, paramParams);
            }

            IObservable<TValue> setObservable;

            if (viewExpression.GetParent()?.NodeType == ExpressionType.Parameter)
            {
                setObservable = changeObservable.Select(x => (TValue)SetThenGet(target, x, viewExpression.GetArgumentsArray())!);
            }
            else
            {
                var bindInfo = changeObservable.CombineLatest(target.WhenAnyDynamic(viewExpression.GetParent(), x => x.Value), (val, host) => new { val, host });

                setObservable = bindInfo
                    .Where(x => x.host != null)
                    .Select(x =>
                    {
                        var value = SetThenGet(x.host, x.val, viewExpression.GetArgumentsArray());

                        return value == null ? default : (TValue)value;
                    })!;
            }

            return (setObservable.Subscribe(_ => { }, ex => this.Log().Error(ex, $"{viewExpression} Binding received an Exception!")), setObservable);
        }

        private bool EvalBindingHooks<TViewModel, TView>(TViewModel? viewModel, TView view, Expression vmExpression, Expression viewExpression, BindingDirection direction)
            where TViewModel : class
        {
            var hooks = Locator.Current.GetServices<IPropertyBindingHook>();

            if (view == null)
            {
                throw new ArgumentNullException(nameof(view));
            }

            Func<IObservedChange<object, object?>[]> vmFetcher = vmExpression != null
                ? (() =>
                {
                    Reflection.TryGetAllValuesForPropertyChain(out var fetchedValues, viewModel, vmExpression.GetExpressionChain());
                    return fetchedValues;
                })
                : (() => new IObservedChange<object, object?>[]
                {
                    new ObservedChange<object, object?>(null!, null!, viewModel)
                });

            var vFetcher = new Func<IObservedChange<object, object?>[]>(() =>
            {
                Reflection.TryGetAllValuesForPropertyChain(out var fetchedValues, view, viewExpression.GetExpressionChain());
                return fetchedValues;
            });

            var shouldBind = hooks.Aggregate(true, (acc, x) =>
                acc && x.ExecuteHook(viewModel, view, vmFetcher!, vFetcher!, direction));

            if (!shouldBind)
            {
                var vmString = $"{typeof(TViewModel).Name}.{string.Join(".", vmExpression)}";
                var vString = $"{typeof(TView).Name}.{string.Join(".", viewExpression)}";
                this.Log().Warn(CultureInfo.InvariantCulture, "Binding hook asked to disable binding {0} => {1}", vmString, vString);
            }

            return shouldBind;
        }

        private IReactiveBinding<TView, (object? view, bool isViewModel)> BindImpl<TViewModel, TView, TVMProp, TVProp, TDontCare>(
            TViewModel? viewModel,
            TView view,
            Expression<Func<TViewModel, TVMProp?>> vmProperty,
            Expression<Func<TView, TVProp>> viewProperty,
            IObservable<TDontCare>? signalViewUpdate,
            OutFunc<TVMProp?, TVProp> vmToViewConverter,
            OutFunc<TVProp, TVMProp?> viewToVmConverter)
            where TViewModel : class
            where TView : class, IViewFor
        {
            if (vmProperty == null)
            {
                throw new ArgumentNullException(nameof(vmProperty));
            }

            if (viewProperty == null)
            {
                throw new ArgumentNullException(nameof(viewProperty));
            }

            var signalInitialUpdate = new Subject<bool>();
            var vmExpression = Reflection.Rewrite(vmProperty.Body);
            var viewExpression = Reflection.Rewrite(viewProperty.Body);

            var somethingChanged = Observable.Merge(
                                                   signalViewUpdate == null ?
                                                   Reflection.ViewModelWhenAnyValue(viewModel, view, vmExpression).Select(_ => true) :
                                                   signalViewUpdate.Select(_ => true)
                                                        .Merge(Reflection.ViewModelWhenAnyValue(viewModel, view, vmExpression).Select(_ => true).Take(1)),
                                                   signalInitialUpdate.Select(_ => true),
                                                   view.WhenAnyDynamic(viewExpression, x => (TVProp?)x.Value).Select(_ => false));

            var changeWithValues = somethingChanged
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
}
