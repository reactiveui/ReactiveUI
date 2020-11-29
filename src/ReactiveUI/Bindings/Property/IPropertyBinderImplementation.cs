// Copyright (c) 2020 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Linq.Expressions;
using Splat;

namespace ReactiveUI
{
    /// <summary>
    /// This interface represents an object that is capable
    /// of providing binding implementations.
    /// </summary>
    public interface IPropertyBinderImplementation : IEnableLogger
    {
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
        IReactiveBinding<TView, (object? view, bool isViewModel)> Bind<TViewModel, TView, TVMProp, TVProp, TDontCare>(
                TViewModel? viewModel,
                TView view,
                Expression<Func<TViewModel, TVMProp>> vmProperty,
                Expression<Func<TView, TVProp>> viewProperty,
                IObservable<TDontCare>? signalViewUpdate,
                object? conversionHint,
                IBindingTypeConverter? vmToViewConverterOverride = null,
                IBindingTypeConverter? viewToVMConverterOverride = null)
            where TViewModel : class
            where TView : class, IViewFor;

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
        IReactiveBinding<TView, (object? view, bool isViewModel)> Bind<TViewModel, TView, TVMProp, TVProp, TDontCare>(
                TViewModel? viewModel,
                TView view,
                Expression<Func<TViewModel, TVMProp>> vmProperty,
                Expression<Func<TView, TVProp>> viewProperty,
                IObservable<TDontCare>? signalViewUpdate,
                Func<TVMProp, TVProp> vmToViewConverter,
                Func<TVProp, TVMProp> viewToVmConverter)
            where TViewModel : class
            where TView : class, IViewFor;

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
        /// </param>
        /// <param name="conversionHint">
        /// An object that can provide a hint for the converter.
        /// The semantics of this object is defined by the converter used.
        /// </param>
        /// <param name="vmToViewConverterOverride">
        /// An optional <see cref="IBindingTypeConverter"/> to use when converting from the
        /// viewModel to view property.
        /// </param>
        /// <returns>
        /// An instance of <see cref="IDisposable"/> that, when disposed,
        /// disconnects the binding.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// There is no registered converter from <typeparamref name="TVMProp"/> to <typeparamref name="TVProp"/>.
        /// </exception>
        IReactiveBinding<TView, TVProp> OneWayBind<TViewModel, TView, TVMProp, TVProp>(
                TViewModel? viewModel,
                TView view,
                Expression<Func<TViewModel, TVMProp>> vmProperty,
                Expression<Func<TView, TVProp>> viewProperty,
                object? conversionHint,
                IBindingTypeConverter? vmToViewConverterOverride = null)
            where TViewModel : class
            where TView : class, IViewFor;

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
        ///     An expression representing the property to be bound to on the view model.
        ///     This can be a child property, for example <c>x =&gt; x.Foo.Bar.Baz</c> in which case
        ///     the binding will attempt to subscribe recursively to updates in order to
        ///     always get the last value of the property chain.
        /// </param>
        /// <param name="viewProperty">
        ///     An expression representing the property to be bound to on the view.
        ///     This can be a child property, for example <c>x =&gt; x.Foo.Bar.Baz</c> in which case
        ///     the binding will attempt to subscribe recursively to updates in order to
        ///     always set the correct property.
        /// </param>
        /// <param name="selector">
        ///     A function that will be used to transform the values of the property on the view model
        ///     before being bound to the view property.
        /// </param>
        /// <returns>
        /// An instance of <see cref="IDisposable"/> that, when disposed,
        /// disconnects the binding.
        /// </returns>
        IReactiveBinding<TView, TOut> OneWayBind<TViewModel, TView, TProp, TOut>(
            TViewModel? viewModel,
            TView view,
            Expression<Func<TViewModel, TProp>> vmProperty,
            Expression<Func<TView, TOut>> viewProperty,
            Func<TProp, TOut> selector)
            where TViewModel : class
            where TView : class, IViewFor;

        /// <summary>
        /// BindTo takes an Observable stream and applies it to a target
        /// property. Conceptually it is similar to <c>Subscribe(x =&gt;
        /// target.property = x)</c>, but allows you to use child properties
        /// without the null checks.
        /// </summary>
        /// <typeparam name="TValue">The value type.</typeparam>
        /// <typeparam name="TTarget">The target type.</typeparam>
        /// <typeparam name="TTValue">The target value type.</typeparam>
        /// <param name="observedChange">The target observable to bind to.</param>
        /// <param name="target">The target object whose property will be set.</param>
        /// <param name="propertyExpression">
        /// An expression representing the target property to set.
        /// This can be a child property (i.e. <c>x.Foo.Bar.Baz</c>).
        /// </param>
        /// <param name="conversionHint">
        /// An object that can provide a hint for the converter.
        /// The semantics of this object is defined by the converter used.
        /// </param>
        /// <param name="vmToViewConverterOverride">
        /// An optional <see cref="IBindingTypeConverter"/> to use when converting from the
        /// viewModel to view property.
        /// </param>
        /// <returns>An object that when disposed, disconnects the binding.</returns>
        IDisposable BindTo<TValue, TTarget, TTValue>(
            IObservable<TValue> observedChange,
            TTarget target,
            Expression<Func<TTarget, TTValue>> propertyExpression,
            object? conversionHint,
            IBindingTypeConverter? vmToViewConverterOverride = null)
            where TTarget : class;
    }
}
