// Copyright (c) 2019 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
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
    [SuppressMessage("StyleCop.CSharp.SpacingRules", "SA1011:Closing square brackets should be spaced correctly", Justification = "nullable object array.")]
    public class PropertyBinderImplementation : IPropertyBinderImplementation
    {
        private static readonly MemoizingMRUCache<(Type fromType, Type toType), IBindingTypeConverter?> _typeConverterCache = new MemoizingMRUCache<(Type fromType, Type toType), IBindingTypeConverter?>(
            (types, _) =>
            {
                return Locator.Current.GetServices<IBindingTypeConverter?>()
                    .Aggregate((currentAffinity: -1, currentBinding: default(IBindingTypeConverter)), (acc, x) =>
                    {
                        var score = x?.GetAffinityForObjects(types.fromType, types.toType) ?? -1;
                        return score > acc.currentAffinity && score > 0 ? (score, x) : acc;
                    }).currentBinding;
            }, RxApp.SmallCacheLimit);

        private static readonly MemoizingMRUCache<(Type fromType, Type toType), ISetMethodBindingConverter?> _setMethodCache = new MemoizingMRUCache<(Type fromType, Type toType), ISetMethodBindingConverter?>(
            (type, _) =>
            {
                return Locator.Current.GetServices<ISetMethodBindingConverter>()
                    .Aggregate((currentAffinity: -1, currentBinding: default(ISetMethodBindingConverter)), (acc, x) =>
                    {
                        var score = x.GetAffinityForObjects(type.fromType, type.toType);
                        return score > acc.currentAffinity && score > 0 ? (score, x) : acc;
                    }).currentBinding;
            }, RxApp.SmallCacheLimit);

        private delegate bool OutFunc<in T1, T2>(T1 t1, out T2 t2);

        /// <inheritdoc />
        public IReactiveBinding<TView, TViewModel, (object? view, bool isViewModel)> Bind<TViewModel, TView, TVMProp, TVProp, TDontCare>(
                TViewModel? viewModel,
                TView view,
                Expression<Func<TViewModel, TVMProp>> vmProperty,
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

#pragma warning disable CS8601 // Possible null reference assignment.
            bool VmToViewFunc(TVMProp vmValue, out TVProp vValue)
            {
                var result = vmToViewConverter.TryConvert(vmValue, typeof(TVProp), conversionHint, out object? tmp);

#pragma warning disable CS8605 // Unboxing a possibly null value.
                vValue = result ? (TVProp)tmp : default;
#pragma warning restore CS8605 // Unboxing a possibly null value.
                return result;
            }

            bool ViewToVmFunc(TVProp vValue, out TVMProp vmValue)
            {
                var result = viewToVMConverter.TryConvert(vValue, typeof(TVMProp), conversionHint, out object? tmp);

#pragma warning disable CS8605 // Unboxing a possibly null value.
                vmValue = result ? (TVMProp)tmp : default;
#pragma warning restore CS8605 // Unboxing a possibly null value.
                return result;
            }
#pragma warning restore CS8601 // Possible null reference assignment.

            return BindImpl(viewModel, view, vmProperty, viewProperty, signalViewUpdate, VmToViewFunc, ViewToVmFunc);
        }

        /// <inheritdoc />
        public IReactiveBinding<TView, TViewModel, (object? view, bool isViewModel)>? Bind<TViewModel, TView, TVMProp, TVProp, TDontCare>(
                TViewModel? viewModel,
                TView view,
                Expression<Func<TViewModel, TVMProp>> vmProperty,
                Expression<Func<TView, TVProp>> viewProperty,
                IObservable<TDontCare>? signalViewUpdate,
                Func<TVMProp, TVProp> vmToViewConverter,
                Func<TVProp, TVMProp> viewToVmConverter)
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

            bool VmToViewFunc(TVMProp vmValue, out TVProp vValue)
            {
                vValue = vmToViewConverter(vmValue);
                return true;
            }

            bool ViewToVmFunc(TVProp vValue, out TVMProp vmValue)
            {
                vmValue = viewToVmConverter(vValue);
                return true;
            }

            return BindImpl(viewModel, view, vmProperty, viewProperty, signalViewUpdate, VmToViewFunc, ViewToVmFunc);
        }

        /// <inheritdoc />
        public IReactiveBinding<TView, TViewModel, TVProp>? OneWayBind<TViewModel, TView, TVMProp, TVProp>(
                TViewModel? viewModel,
                TView view,
                Expression<Func<TViewModel, TVMProp>> vmProperty,
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
            var converter = vmToViewConverterOverride ?? GetConverterForTypes(typeof(TVMProp), viewType);

            if (converter == null)
            {
                throw new ArgumentException($"Can't convert {typeof(TVMProp)} to {viewType}. To fix this, register a IBindingTypeConverter");
            }

            var ret = EvalBindingHooks(viewModel, view, vmExpression, viewExpression, BindingDirection.OneWay);
            if (!ret)
            {
                return null;
            }

            var source = Reflection.ViewModelWhenAnyValue(viewModel, view, vmExpression)
                    .SelectMany(x =>
                    {
                        if (!converter.TryConvert(x, viewType, conversionHint, out object? tmp))
                        {
                            return Observable<object>.Empty;
                        }

                        return Observable.Return(tmp);
                    });

            var (disposable, obs) = BindToDirect<TView, TVProp, object?>(source, view, viewExpression);

            return new ReactiveBinding<TView, TViewModel, TVProp>(view, viewModel, viewExpression, vmExpression, obs, BindingDirection.OneWay, disposable);
        }

        /// <inheritdoc />
        public IReactiveBinding<TView, TViewModel, TOut>? OneWayBind<TViewModel, TView, TProp, TOut>(
            TViewModel? viewModel,
            TView view,
            Expression<Func<TViewModel, TProp>> vmProperty,
            Expression<Func<TView, TOut>> viewProperty,
            Func<TProp, TOut> selector)
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
                return null;
            }

            var source = Reflection.ViewModelWhenAnyValue(viewModel, view, vmExpression).Cast<TProp>().Select(selector);

            var (disposable, obs) = BindToDirect<TView, TOut, TOut>(source, view, viewExpression);

            return new ReactiveBinding<TView, TViewModel, TOut>(view, viewModel, viewExpression, vmExpression, obs, BindingDirection.OneWay, disposable);
        }

        /// <inheritdoc />
        public IDisposable BindTo<TValue, TTarget, TTValue>(
            IObservable<TValue> observedChange,
            TTarget target,
            Expression<Func<TTarget, TTValue>> propertyExpression,
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

            var converter = vmToViewConverterOverride ?? GetConverterForTypes(typeof(TValue), typeof(TTValue));

            if (converter == null)
            {
                throw new ArgumentException($"Can't convert {typeof(TValue)} to {typeof(TTValue)}. To fix this, register a IBindingTypeConverter");
            }

            var source = observedChange.SelectMany(x =>
            {
                if (!converter.TryConvert(x, typeof(TTValue), conversionHint, out var tmp))
                {
                    return Observable<object>.Empty;
                }

                return Observable.Return(tmp);
            });

            var (disposable, _) = BindToDirect<TTarget, TTValue, object?>(source, target, viewExpression);

            return disposable;
        }

        internal static IBindingTypeConverter? GetConverterForTypes(Type lhs, Type rhs)
        {
            return _typeConverterCache.Get((lhs, rhs));
        }

        private static Func<object?, object?, object[]?, object>? GetSetConverter(Type? fromType, Type targetType)
        {
            if (fromType == null)
            {
                return null;
            }

            var setter = _setMethodCache.Get((fromType, targetType));

            if (setter == null)
            {
                return null;
            }

            return setter.PerformSet;
        }

        private (IDisposable disposable, IObservable<TValue> value) BindToDirect<TTarget, TValue, TObs>(
                IObservable<TObs> changeObservable,
                TTarget target,
                Expression viewExpression)
            where TTarget : class
        {
            var defaultSetter = Reflection.GetValueSetterOrThrow(viewExpression.GetMemberInfo());
            var defaultGetter = Reflection.GetValueFetcherOrThrow(viewExpression.GetMemberInfo());

            object? SetThenGet(object paramTarget, object? paramValue, object[]? paramParams)
            {
                Func<object?, object?, object[]?, object>? converter = GetSetConverter(paramValue?.GetType(), viewExpression.Type);

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

            if (viewExpression.GetParent().NodeType == ExpressionType.Parameter)
            {
                setObservable = changeObservable.Select(x => (TValue)SetThenGet(target, x, viewExpression.GetArgumentsArray()) !);
            }
            else
            {
                var bindInfo = changeObservable.CombineLatest(target.WhenAnyDynamic(viewExpression.GetParent(), x => x.Value), (val, host) => new { val, host });

                setObservable = bindInfo
                    .Where(x => x.host != null)
                    .Select(x =>
                    {
                        var value = SetThenGet(x.host, x.val, viewExpression.GetArgumentsArray());

                        if (value == null)
                        {
#pragma warning disable CS8603 // Possible null reference return.
                            return default;
#pragma warning restore CS8603 // Possible null reference return.
                        }

                        return (TValue)value;
                    });
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

            Func<IObservedChange<object, object?>[]> vmFetcher;
            if (vmExpression != null)
            {
                vmFetcher = () =>
                {
                    Reflection.TryGetAllValuesForPropertyChain(out var fetchedValues, viewModel, vmExpression.GetExpressionChain());
                    return fetchedValues;
                };
            }
            else
            {
                vmFetcher = () => new IObservedChange<object, object?>[]
                {
                    new ObservedChange<object, object?>(null!, null!, viewModel)
                };
            }

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

        private IReactiveBinding<TView, TViewModel, (object? view, bool isViewModel)> BindImpl<TViewModel, TView, TVMProp, TVProp, TDontCare>(
            TViewModel? viewModel,
            TView view,
            Expression<Func<TViewModel, TVMProp>> vmProperty,
            Expression<Func<TView, TVProp>> viewProperty,
            IObservable<TDontCare>? signalViewUpdate,
            OutFunc<TVMProp, TVProp> vmToViewConverter,
            OutFunc<TVProp, TVMProp> viewToVmConverter)
            where TView : class, IViewFor
            where TViewModel : class
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

            var signalObservable = signalViewUpdate != null
                                       ? signalViewUpdate.Select(_ => false)
                                       : view.WhenAnyDynamic(viewExpression, x => (TVProp)x.Value).Select(_ => false);

            var somethingChanged = Observable.Merge(
                                                    Reflection.ViewModelWhenAnyValue(viewModel, view, vmExpression).Select(_ => true),
                                                    signalInitialUpdate.Select(_ => true),
                                                    signalObservable);

            var changeWithValues = somethingChanged.Select<bool, (bool isValid, object? view, bool isViewModel)>(isVm =>
            {
                if (!Reflection.TryGetValueForPropertyChain(out TVMProp vmValue, view.ViewModel, vmExpression.GetExpressionChain()) ||
                    !Reflection.TryGetValueForPropertyChain(out TVProp vValue, view, viewExpression.GetExpressionChain()))
                {
                    return (false, null, false);
                }

                if (isVm)
                {
                    if (!vmToViewConverter(vmValue, out TVProp vmAsView) || EqualityComparer<TVProp>.Default.Equals(vValue, vmAsView))
                    {
                        return (false, null, false);
                    }

                    return (true, vmAsView, isVm);
                }

                if (!viewToVmConverter(vValue, out TVMProp vAsViewModel) || EqualityComparer<TVMProp>.Default.Equals(vmValue, vAsViewModel))
                {
                    return (false, null, false);
                }

                return (true, vAsViewModel, isVm);
            });

            var ret = EvalBindingHooks(viewModel, view, vmExpression, viewExpression, BindingDirection.TwoWay);
            if (!ret)
            {
                return null!;
            }

            IObservable<(object? view, bool isViewModel)> changes = changeWithValues
                .Where(value => value.isValid)
                .Select(value => (value.view, value.isViewModel))
                .Publish()
                .RefCount();

            IDisposable disposable = changes.Subscribe(isVmWithLatestValue =>
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

            return new ReactiveBinding<TView, TViewModel, (object? view, bool isViewModel)>(
                   view,
                   viewModel,
                   viewExpression,
                   vmExpression,
                   changes,
                   BindingDirection.TwoWay,
                   disposable);
        }
    }
}
