// Copyright (c) 2019 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
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
        private static readonly MemoizingMRUCache<(Type fromType, Type toType), IBindingTypeConverter> _typeConverterCache = new MemoizingMRUCache<(Type fromType, Type toType), IBindingTypeConverter>(
            (types, _) =>
            {
                return Locator.Current.GetServices<IBindingTypeConverter>()
                    .Aggregate((currentAffinity: -1, currentBinding: default(IBindingTypeConverter)), (acc, x) =>
                    {
                        var score = x.GetAffinityForObjects(types.fromType, types.toType);
                        return score > acc.currentAffinity && score > 0 ? (score, x) : acc;
                    }).Item2;
            }, RxApp.SmallCacheLimit);

        private static readonly MemoizingMRUCache<(Type fromType, Type toType), ISetMethodBindingConverter> _setMethodCache = new MemoizingMRUCache<(Type fromType, Type toType), ISetMethodBindingConverter>(
            (type, _) =>
            {
                return Locator.Current.GetServices<ISetMethodBindingConverter>()
                    .Aggregate((currentAffinity: -1, currentBinding: default(ISetMethodBindingConverter)), (acc, x) =>
                    {
                        var score = x.GetAffinityForObjects(type.fromType, type.toType);
                        return score > acc.currentAffinity && score > 0 ? (score, x) : acc;
                    }).Item2;
            }, RxApp.SmallCacheLimit);

        private delegate bool OutFunc<in T1, T2>(T1 t1, out T2 t2);

        /// <summary>
        /// Creates a two-way binding between a view model and a view.
        /// This binding will attempt to convert the values of the
        /// view and view model properties using a <see cref="IBindingTypeConverter"/>
        /// if they are not of the same type.
        /// </summary>
        /// <typeparam name="TViewModel">The type of the view model that is bound.</typeparam>
        /// <typeparam name="TView">The type of the view being bound.</typeparam>
        /// <typeparam name="TVMProp">The type of the property bound on the view model.</typeparam>
        /// <typeparam name="TVProp">The type of the property bound on the view.</typeparam>
        /// <typeparam name="TDontCare">
        /// A dummy type, only the fact that <paramref name="signalViewUpdate"/>
        /// emits values is considered, not the actual values emitted.
        /// </typeparam>
        /// <param name="viewModel">The instance of the view model object to be bound.</param>
        /// <param name="view">The instance of the view object to be bound.</param>
        /// <param name="vmProperty">
        /// An expression representing the property to be bound to on the view model.
        /// This can be a child property, for example <c>x =&gt; x.Foo.Bar.Baz</c> in which case
        /// the binding will attempt to subscribe recursively to updates in order to
        /// always get and set the correct property.
        /// </param>
        /// <param name="viewProperty">
        /// An expression representing the property to be bound to on the view.
        /// This can be a child property, for example <c>x =&gt; x.Foo.Bar.Baz</c> in which case
        /// the binding will attempt to subscribe recursively to updates in order to
        /// always get and set the correct property.
        ///
        /// If it is left null, the framework will attempt to automatically figure out
        /// the control and property that is to be bound, by looking for a control of the
        /// same name as the <paramref name="vmProperty"/>, and its most natural property.
        /// </param>
        /// <param name="signalViewUpdate">
        /// An observable, that when signaled, indicates that the view property
        /// has been changed, and that the binding should update the view model
        /// property accordingly.
        /// </param>
        /// <param name="conversionHint">
        /// An object that can provide a hint for the converter.
        /// The semantics of this object is defined by the converter used.
        /// </param>
        /// <param name="vmToViewConverterOverride">
        /// An optional <see cref="IBindingTypeConverter"/> to use when converting from the
        /// viewModel to view property.
        /// </param>
        /// <param name="viewToVMConverterOverride">
        /// An optional <see cref="IBindingTypeConverter"/> to use when converting from the
        /// view to viewModel property.
        /// </param>
        /// <returns>
        /// An instance of <see cref="IDisposable"/> that, when disposed,
        /// disconnects the binding.
        /// </returns>
        public IReactiveBinding<TView, TViewModel, Tuple<object, bool>> Bind<TViewModel, TView, TVMProp, TVProp, TDontCare>(
                TViewModel viewModel,
                TView view,
                Expression<Func<TViewModel, TVMProp>> vmProperty,
                Expression<Func<TView, TVProp>> viewProperty,
                IObservable<TDontCare> signalViewUpdate,
                object conversionHint,
                IBindingTypeConverter vmToViewConverterOverride = null,
                IBindingTypeConverter viewToVMConverterOverride = null)
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

            bool VmToViewFunc(TVMProp vmValue, out TVProp vValue)
            {
                var result = vmToViewConverter.TryConvert(vmValue, typeof(TVProp), conversionHint, out object tmp);

                vValue = result ? (TVProp)tmp : default(TVProp);
                return result;
            }

            bool ViewToVmFunc(TVProp vValue, out TVMProp vmValue)
            {
                var result = viewToVMConverter.TryConvert(vValue, typeof(TVMProp), conversionHint, out object tmp);

                vmValue = result ? (TVMProp)tmp : default(TVMProp);
                return result;
            }

            return BindImpl(viewModel, view, vmProperty, viewProperty, signalViewUpdate, VmToViewFunc, ViewToVmFunc);
        }

        /// <summary>
        /// Binds the specified view model property to the given view property.
        /// </summary>
        /// <typeparam name="TViewModel">The type of the view model being bound.</typeparam>
        /// <typeparam name="TView">The type of the view being bound.</typeparam>
        /// <typeparam name="TVMProp">The type of the property bound on the view model.</typeparam>
        /// <typeparam name="TVProp">The type of the property bound on the view.</typeparam>
        /// <typeparam name="TDontCare">
        /// A dummy type, only the fact that <paramref name="signalViewUpdate"/>
        /// emits values is considered, not the actual values emitted.
        /// </typeparam>
        /// <param name="viewModel">The instance of the view model to bind.</param>
        /// <param name="view">The instance of the view to bind.</param>
        /// <param name="vmProperty">
        /// An expression indicating the property that is bound on the view model.
        /// This can be a chain of properties of the form <c>vm =&gt; vm.Foo.Bar.Baz</c>
        /// and the binder will attempt to subscribe to changes on each recursively.
        /// </param>
        /// <param name="viewProperty">
        /// The property on the view that is to be bound.
        /// This can be a chain of properties of the form. <code>view => view.Foo.Bar.Baz</code>
        /// and the binder will attempt to set the last one each time the view model property is updated.
        /// </param>
        /// <param name="signalViewUpdate">
        /// An observable, that when signaled, indicates that the view property
        /// has been changed, and that the binding should update the view model
        /// property accordingly.
        /// </param>
        /// <param name="vmToViewConverter">
        /// Delegate to convert the value of the view model's property's type to a value of the
        /// view's property's type.
        /// </param>
        /// <param name="viewToVmConverter">
        /// Delegate to convert the value of the view's property's type to a value of the
        /// view model's property's type.
        /// </param>
        /// <returns>
        /// An instance of <see cref="IDisposable"/> that, when disposed,
        /// disconnects the binding.
        /// </returns>
        public IReactiveBinding<TView, TViewModel, Tuple<object, bool>> Bind<TViewModel, TView, TVMProp, TVProp, TDontCare>(
                TViewModel viewModel,
                TView view,
                Expression<Func<TViewModel, TVMProp>> vmProperty,
                Expression<Func<TView, TVProp>> viewProperty,
                IObservable<TDontCare> signalViewUpdate,
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

        /// <summary>
        /// Creates a one-way binding, i.e. a binding that flows from the
        /// <paramref name="viewModel"/> to the <paramref name="view"/> only. This binding will
        /// attempt to convert the value of the view model property to the view property if they
        /// are not of the same type.
        /// </summary>
        /// <typeparam name="TViewModel">The type of the view model that is bound.</typeparam>
        /// <typeparam name="TView">The type of the view that is bound.</typeparam>
        /// <typeparam name="TVMProp">The type of the property bound on the view model.</typeparam>
        /// <typeparam name="TVProp">The type of the property bound on the view.</typeparam>
        /// <param name="viewModel">The instance of the view model to bind to.</param>
        /// <param name="view">The instance of the view to bind to.</param>
        /// <param name="vmProperty">
        /// An expression representing the property to be bound to on the view model.
        /// This can be a child property, for example <c>x =&gt; x.Foo.Bar.Baz</c> in which case
        /// the binding will attempt to subscribe recursively to updates in order to
        /// always get the last value of the property chain.
        /// </param>
        /// <param name="viewProperty">
        /// An expression representing the property to be bound to on the view.
        /// This can be a child property, for example <c>x =&gt; x.Foo.Bar.Baz</c> in which case
        /// the binding will attempt to subscribe recursively to updates in order to
        /// always set the correct property.
        ///
        /// If it is left null, the framework will attempt to automatically figure out
        /// the control and property that is to be bound, by looking for a control of the
        /// same name as the <paramref name="vmProperty"/>, and its most natural property.
        /// </param>
        /// <param name="conversionHint">
        /// An object that can provide a hint for the converter.
        /// The semantics of this object is defined by the converter used.
        /// </param>
        /// <param name="vmToViewConverterOverride">
        /// Delegate to convert the value of the view model's property's type to a value of the
        /// view's property's type.
        /// </param>
        /// <returns>
        /// An instance of <see cref="IDisposable"/> that, when disposed,
        /// disconnects the binding.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// There is no registered converter from <typeparamref name="TVMProp"/> to <typeparamref name="TVProp"/>.
        /// </exception>
        public IReactiveBinding<TView, TViewModel, TVProp> OneWayBind<TViewModel, TView, TVMProp, TVProp>(
                TViewModel viewModel,
                TView view,
                Expression<Func<TViewModel, TVMProp>> vmProperty,
                Expression<Func<TView, TVProp>> viewProperty,
                object conversionHint = null,
                IBindingTypeConverter vmToViewConverterOverride = null)
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
                        if (!converter.TryConvert(x, viewType, conversionHint, out object tmp))
                        {
                            return Observable<object>.Empty;
                        }

                        return Observable.Return(tmp);
                    });

            var (disposable, obs) = BindToDirect<TView, TVProp, object>(source, view, viewExpression);

            return new ReactiveBinding<TView, TViewModel, TVProp>(view, viewModel, viewExpression, vmExpression, obs, BindingDirection.OneWay, disposable);
        }

        /// <summary>
        /// Creates a one way binding with a selector, i.e. a binding that flows from the
        /// <paramref name="viewModel"/> to the <paramref name="view"/> only, and where the value of the view model
        /// property is mapped through the <paramref name="selector"/> before being set to the view.
        /// </summary>
        /// <typeparam name="TViewModel">The type of the view model that is bound.</typeparam>
        /// <typeparam name="TView">The type of the view that is bound.</typeparam>
        /// <typeparam name="TProp">The type of the property bound on the view model.</typeparam>
        /// <typeparam name="TOut">The return type of the <paramref name="selector"/>.</typeparam>
        /// <param name="viewModel">The instance of the view model to bind to.</param>
        /// <param name="view">The instance of the view to bind to.</param>
        /// <param name="vmProperty">
        /// An expression representing the property to be bound to on the view model.
        /// This can be a child property, for example <c>x =&gt; x.Foo.Bar.Baz</c> in which case
        /// the binding will attempt to subscribe recursively to updates in order to
        /// always get the last value of the property chain.
        /// </param>
        /// <param name="viewProperty">
        /// An expression representing the property to be bound to on the view.
        /// This can be a child property, for example <c>x =&gt; x.Foo.Bar.Baz</c> in which case
        /// the binding will attempt to subscribe recursively to updates in order to
        /// always set the correct property.
        ///
        /// If it is left null, the framework will attempt to automatically figure out
        /// the control and property that is to be bound, by looking for a control of the
        /// same name as the <paramref name="vmProperty"/>, and its most natural property.
        /// </param>
        /// <param name="selector">
        /// A function that will be used to transform the values of the property on the view model
        /// before being bound to the view property.
        /// </param>
        /// <returns>
        /// An instance of <see cref="IDisposable"/> that, when disposed,
        /// disconnects the binding.
        /// </returns>
        public IReactiveBinding<TView, TViewModel, TOut> OneWayBind<TViewModel, TView, TProp, TOut>(
                TViewModel viewModel,
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

            var source = Reflection.ViewModelWhenAnyValue(viewModel, view, vmExpression).Select(x => (TProp)x).Select(selector);

            var viewType = viewExpression.Type;
            var (disposable, obs) = BindToDirect<TView, TOut, TOut>(source, view, viewExpression);

            return new ReactiveBinding<TView, TViewModel, TOut>(view, viewModel, viewExpression, vmExpression, obs, BindingDirection.OneWay, disposable);
        }

        /// <summary>
        /// BindTo takes an Observable stream and applies it to a target
        /// property. Conceptually it is similar to <c>Subscribe(x =&gt;
        /// target.property = x)</c>, but allows you to use child properties
        /// without the null checks.
        /// </summary>
        /// <typeparam name="TValue">The source type.</typeparam>
        /// <typeparam name="TTarget">The target object type.</typeparam>
        /// <typeparam name="TTValue">The type of the property on the target object.</typeparam>
        /// <param name="observedChange">The observable to apply to the target property.</param>
        /// <param name="target">The target object whose property will be set.</param>
        /// <param name="propertyExpression">
        /// An expression representing the target property to set.
        /// This can be a child property (i.e. <c>x.Foo.Bar.Baz</c>).</param>
        /// <param name="conversionHint">
        /// An object that can provide a hint for the converter.
        /// The semantics of this object is defined by the converter used.
        /// </param>
        /// <param name="vmToViewConverterOverride">
        /// Delegate to convert the value of the view model's property's type to a value of the
        /// view's property's type.
        /// </param>
        /// <returns>An object that when disposed, disconnects the binding.</returns>
        public IDisposable BindTo<TValue, TTarget, TTValue>(
            IObservable<TValue> observedChange,
            TTarget target,
            Expression<Func<TTarget, TTValue>> propertyExpression,
            object conversionHint = null,
            IBindingTypeConverter vmToViewConverterOverride = null)
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

            var ret = EvalBindingHooks(observedChange, target, null, viewExpression, BindingDirection.OneWay);
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

            var (disposable, obs) = BindToDirect<TTarget, TTValue, object>(source, target, viewExpression);

            return disposable;
        }

        internal static IBindingTypeConverter GetConverterForTypes(Type lhs, Type rhs)
        {
            return _typeConverterCache.Get((lhs, rhs));
        }

        private static Func<object, object, object[], object> GetSetConverter(Type fromType, Type targetType)
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
            object SetThenGet(object paramTarget, object paramValue, object[] paramParams)
            {
                Func<object, object, object[], object> converter = GetSetConverter(paramValue?.GetType(), viewExpression.Type);

                if (converter == null)
                {
                    defaultSetter(paramTarget, paramValue, paramParams);
                    return defaultGetter(paramTarget, paramParams);
                }

                var value = defaultGetter(paramTarget, paramParams);
                return converter(value, paramValue, paramParams);
            }

            IObservable<TValue> setObservable;

            if (viewExpression.GetParent().NodeType == ExpressionType.Parameter)
            {
                setObservable = changeObservable.Select(x => (TValue)SetThenGet(target, x, viewExpression.GetArgumentsArray()));
            }
            else
            {
                var bindInfo = changeObservable.CombineLatest(target.WhenAnyDynamic(viewExpression.GetParent(), x => x.Value), (val, host) => new { val, host });

                setObservable = bindInfo
                    .Where(x => x.host != null)
                    .Select(x => (TValue)SetThenGet(x.host, x.val, viewExpression.GetArgumentsArray()));
            }

            return (setObservable.Subscribe(_ => { }, ex => this.Log().Error(ex, $"{viewExpression} Binding received an Exception!")), setObservable);
        }

        private bool EvalBindingHooks<TViewModel, TView>(TViewModel viewModel, TView view, Expression vmExpression, Expression viewExpression, BindingDirection direction)
            where TViewModel : class
        {
            var hooks = Locator.Current.GetServices<IPropertyBindingHook>();

            Func<IObservedChange<object, object>[]> vmFetcher;
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
                vmFetcher = () => new IObservedChange<object, object>[]
                {
                    new ObservedChange<object, object>(null, null, viewModel)
                };
            }

            var vFetcher = new Func<IObservedChange<object, object>[]>(() =>
            {
                Reflection.TryGetAllValuesForPropertyChain(out var fetchedValues, view, viewExpression.GetExpressionChain());
                return fetchedValues;
            });

            var shouldBind = hooks.Aggregate(true, (acc, x) =>
                acc && x.ExecuteHook(viewModel, view, vmFetcher, vFetcher, direction));

            if (!shouldBind)
            {
                var vmString = $"{typeof(TViewModel).Name}.{string.Join(".", vmExpression)}";
                var vString = $"{typeof(TView).Name}.{string.Join(".", viewExpression)}";
                this.Log().Warn(CultureInfo.InvariantCulture, "Binding hook asked to disable binding {0} => {1}", vmString, vString);
            }

            return shouldBind;
        }

        private IReactiveBinding<TView, TViewModel, Tuple<object, bool>> BindImpl<TViewModel, TView, TVMProp, TVProp, TDontCare>(
            TViewModel viewModel,
            TView view,
            Expression<Func<TViewModel, TVMProp>> vmProperty,
            Expression<Func<TView, TVProp>> viewProperty,
            IObservable<TDontCare> signalViewUpdate,
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

            var changeWithValues = somethingChanged.Select(isVm =>
            {
                if (!Reflection.TryGetValueForPropertyChain(out TVMProp vmValue, view.ViewModel, vmExpression.GetExpressionChain()) ||
                    !Reflection.TryGetValueForPropertyChain(out TVProp vValue, view, viewExpression.GetExpressionChain()))
                {
                    return null;
                }

                if (isVm)
                {
                    if (!vmToViewConverter(vmValue, out TVProp vmAsView) || EqualityComparer<TVProp>.Default.Equals(vValue, vmAsView))
                    {
                        return null;
                    }

                    return Tuple.Create((object)vmAsView, isVm);
                }

                if (!viewToVmConverter(vValue, out TVMProp vAsViewModel) || EqualityComparer<TVMProp>.Default.Equals(vmValue, vAsViewModel))
                {
                    return null;
                }

                return Tuple.Create((object)vAsViewModel, isVm);
            });

            var ret = EvalBindingHooks(viewModel, view, vmExpression, viewExpression, BindingDirection.TwoWay);
            if (!ret)
            {
                return null;
            }

            IObservable<Tuple<object, bool>> changes = changeWithValues.Where(tuple => tuple != null).Publish().RefCount();

            IDisposable disposable = changes.Subscribe(isVmWithLatestValue =>
            {
                if (isVmWithLatestValue.Item2)
                {
                    Reflection.TrySetValueToPropertyChain(view, viewExpression.GetExpressionChain(), isVmWithLatestValue.Item1, false);
                }
                else
                {
                    Reflection.TrySetValueToPropertyChain(view.ViewModel, vmExpression.GetExpressionChain(), isVmWithLatestValue.Item1, false);
                }
            });

            // NB: Even though it's technically a two-way bind, most people
            // want the ViewModel to win at first.
            signalInitialUpdate.OnNext(true);

            return new ReactiveBinding<TView, TViewModel, Tuple<object, bool>>(
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
