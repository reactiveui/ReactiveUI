// Copyright (c) 2023 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Linq.Expressions;
using System.Reactive;

namespace ReactiveUI;

/// <summary>
/// This class provides extension methods for the ReactiveUI view binding mechanism.
/// </summary>
public static class PropertyBindingMixins
{
    private static readonly IPropertyBinderImplementation _binderImplementation;

    static PropertyBindingMixins()
    {
        RxApp.EnsureInitialized();
        _binderImplementation = new PropertyBinderImplementation();
    }

    /// <summary>
    /// Binds the specified view model property to the given view property.
    /// </summary>
    /// <typeparam name="TViewModel">The type of the view model being bound.</typeparam>
    /// <typeparam name="TView">The type of the view being bound.</typeparam>
    /// <typeparam name="TVMProp">The type of the property bound on the view model.</typeparam>
    /// <typeparam name="TVProp">The type of the property bound on the view.</typeparam>
    /// <param name="view">The instance of the view to bind.</param>
    /// <param name="viewModel">The instance of the view model to bind.</param>
    /// <param name="vmProperty">
    /// An expression indicating the property that is bound on the view model.
    /// This can be a chain of properties of the form. <c>vm =&gt; vm.Foo.Bar.Baz</c>
    /// and the binder will attempt to subscribe to changes on each recursively.
    /// </param>
    /// <param name="viewProperty">
    /// The property on the view that is to be bound.
    /// This can be a chain of properties of the form. <c>view => view.Foo.Bar.Baz</c>
    /// and the binder will attempt to set the last one each time the view model property is updated.
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
    public static IReactiveBinding<TView, (object? view, bool isViewModel)> Bind<TViewModel, TView, TVMProp, TVProp>(
        this TView view,
        TViewModel? viewModel,
        Expression<Func<TViewModel, TVMProp?>> vmProperty,
        Expression<Func<TView, TVProp>> viewProperty,
        object? conversionHint = null,
        IBindingTypeConverter? vmToViewConverterOverride = null,
        IBindingTypeConverter? viewToVMConverterOverride = null)
        where TViewModel : class
        where TView : class, IViewFor =>
        _binderImplementation.Bind(
                                   viewModel,
                                   view,
                                   vmProperty,
                                   viewProperty,
                                   (IObservable<Unit>?)null,
                                   conversionHint,
                                   vmToViewConverterOverride,
                                   viewToVMConverterOverride);

    /// <summary>
    /// Binds the specified view model property to the given view property, and
    /// provide a custom view update signaler to signal when the view property has been updated.
    /// </summary>
    /// <typeparam name="TViewModel">The type of the view model being bound.</typeparam>
    /// <typeparam name="TView">The type of the view being bound.</typeparam>
    /// <typeparam name="TVMProp">The type of the property bound on the view model.</typeparam>
    /// <typeparam name="TVProp">The type of the property bound on the view.</typeparam>
    /// <typeparam name="TDontCare">A dummy type, only the fact that <paramref name="signalViewUpdate" />
    /// emits values is considered, not the actual values emitted.</typeparam>
    /// <param name="view">The instance of the view to bind.</param>
    /// <param name="viewModel">The instance of the view model to bind.</param>
    /// <param name="vmProperty">An expression indicating the property that is bound on the view model.
    /// This can be a chain of properties of the form. <c>vm =&gt; vm.Foo.Bar.Baz</c>
    /// and the binder will attempt to subscribe to changes on each recursively.</param>
    /// <param name="viewProperty">The property on the view that is to be bound.
    /// This can be a chain of properties of the form. <c>view =&gt; view.Foo.Bar.Baz</c>
    /// and the binder will attempt to set the last one each time the view model property is updated.</param>
    /// <param name="signalViewUpdate">An observable, that when signaled, indicates that the view property
    /// has been changed, and that the binding should update the view model
    /// property accordingly.</param>
    /// <param name="conversionHint">An object that can provide a hint for the converter.
    /// The semantics of this object is defined by the converter used.</param>
    /// <param name="vmToViewConverterOverride">An optional <see cref="IBindingTypeConverter" /> to use when converting from the
    /// viewModel to view property.</param>
    /// <param name="viewToVMConverterOverride">An optional <see cref="IBindingTypeConverter" /> to use when converting from the
    /// view to viewModel property.</param>
    /// <param name="triggerUpdate">The trigger update direction.</param>
    /// <returns>
    /// An instance of <see cref="IDisposable" /> that, when disposed,
    /// disconnects the binding.
    /// </returns>
    public static IReactiveBinding<TView, (object? view, bool isViewModel)> Bind<TViewModel, TView, TVMProp, TVProp, TDontCare>(
        this TView view,
        TViewModel? viewModel,
        Expression<Func<TViewModel, TVMProp?>> vmProperty,
        Expression<Func<TView, TVProp>> viewProperty,
        IObservable<TDontCare>? signalViewUpdate,
        object? conversionHint = null,
        IBindingTypeConverter? vmToViewConverterOverride = null,
        IBindingTypeConverter? viewToVMConverterOverride = null,
        TriggerUpdate triggerUpdate = TriggerUpdate.ViewToViewModel)
        where TViewModel : class
        where TView : class, IViewFor =>
        _binderImplementation.Bind(viewModel, view, vmProperty, viewProperty, signalViewUpdate, conversionHint, vmToViewConverterOverride, viewToVMConverterOverride, triggerUpdate);

    /// <summary>
    /// Binds the specified view model property to the given view property.
    /// </summary>
    /// <typeparam name="TViewModel">The type of the view model being bound.</typeparam>
    /// <typeparam name="TView">The type of the view being bound.</typeparam>
    /// <typeparam name="TVMProp">The type of the property bound on the view model.</typeparam>
    /// <typeparam name="TVProp">The type of the property bound on the view.</typeparam>
    /// <param name="view">The instance of the view to bind.</param>
    /// <param name="viewModel">The instance of the view model to bind.</param>
    /// <param name="vmProperty">
    /// An expression indicating the property that is bound on the view model.
    /// This can be a chain of properties of the form. <c>vm =&gt; vm.Foo.Bar.Baz</c>
    /// and the binder will attempt to subscribe to changes on each recursively.
    /// </param>
    /// <param name="viewProperty">
    /// The property on the view that is to be bound.
    /// This can be a chain of properties of the form. <c>view => view.Foo.Bar.Baz</c>
    /// and the binder will attempt to set the last one each time the view model property is updated.
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
    public static IReactiveBinding<TView, (object? view, bool isViewModel)> Bind<TViewModel, TView, TVMProp, TVProp>(
        this TView view,
        TViewModel? viewModel,
        Expression<Func<TViewModel, TVMProp?>> vmProperty,
        Expression<Func<TView, TVProp>> viewProperty,
        Func<TVMProp?, TVProp> vmToViewConverter,
        Func<TVProp, TVMProp?> viewToVmConverter)
        where TViewModel : class
        where TView : class, IViewFor => _binderImplementation.Bind(viewModel, view, vmProperty, viewProperty, (IObservable<Unit>?)null, vmToViewConverter, viewToVmConverter);

    /// <summary>
    /// Binds the specified view model property to the given view property.
    /// </summary>
    /// <typeparam name="TViewModel">The type of the view model being bound.</typeparam>
    /// <typeparam name="TView">The type of the view being bound.</typeparam>
    /// <typeparam name="TVMProp">The type of the property bound on the view model.</typeparam>
    /// <typeparam name="TVProp">The type of the property bound on the view.</typeparam>
    /// <typeparam name="TDontCare">A dummy type, only the fact that <paramref name="signalViewUpdate" />
    /// emits values is considered, not the actual values emitted.</typeparam>
    /// <param name="view">The instance of the view to bind.</param>
    /// <param name="viewModel">The instance of the view model to bind.</param>
    /// <param name="vmProperty">An expression indicating the property that is bound on the view model.
    /// This can be a chain of properties of the form. <c>vm =&gt; vm.Foo.Bar.Baz</c>
    /// and the binder will attempt to subscribe to changes on each recursively.</param>
    /// <param name="viewProperty">The property on the view that is to be bound.
    /// This can be a chain of properties of the form. <c>view =&gt; view.Foo.Bar.Baz</c>
    /// and the binder will attempt to set the last one each time the view model property is updated.</param>
    /// <param name="signalViewUpdate">An observable, that when signaled, indicates that the view property
    /// has been changed, and that the binding should update the view model
    /// property accordingly.</param>
    /// <param name="vmToViewConverter">Delegate to convert the value of the view model's property's type to a value of the
    /// view's property's type.</param>
    /// <param name="viewToVmConverter">Delegate to convert the value of the view's property's type to a value of the
    /// view model's property's type.</param>
    /// <param name="triggerUpdate">The trigger update direction.</param>
    /// <returns>
    /// An instance of <see cref="IDisposable" /> that, when disposed,
    /// disconnects the binding.
    /// </returns>
    public static IReactiveBinding<TView, (object? view, bool isViewModel)> Bind<TViewModel, TView, TVMProp, TVProp, TDontCare>(
        this TView view,
        TViewModel? viewModel,
        Expression<Func<TViewModel, TVMProp?>> vmProperty,
        Expression<Func<TView, TVProp>> viewProperty,
        IObservable<TDontCare>? signalViewUpdate,
        Func<TVMProp?, TVProp> vmToViewConverter,
        Func<TVProp, TVMProp?> viewToVmConverter,
        TriggerUpdate triggerUpdate = TriggerUpdate.ViewToViewModel)
        where TViewModel : class
        where TView : class, IViewFor =>
        _binderImplementation.Bind(viewModel, view, vmProperty, viewProperty, signalViewUpdate, vmToViewConverter, viewToVmConverter, triggerUpdate);

    /// <summary>
    /// Binds the given property on the view model to a given property on the view in a one-way (view model to view) fashion.
    /// </summary>
    /// <typeparam name="TViewModel">The type of the view model.</typeparam>
    /// <typeparam name="TView">The type of the view.</typeparam>
    /// <typeparam name="TVMProp">The type of view model property.</typeparam>
    /// <typeparam name="TVProp">The type of the property bound on the view.</typeparam>
    /// <param name="view">
    /// The instance of the view object which is bound. Usually, it is the. <c>this</c>
    /// instance.
    /// </param>
    /// <param name="viewModel">
    /// The view model that is bound.
    /// It is usually set to the <see cref="IViewFor.ViewModel"/> property of the <paramref name="view"/>.</param>
    /// <param name="vmProperty">
    /// An expression indicating the property that is bound on the view model.
    /// This can be a chain of properties of the form. <c>vm => vm.Foo.Bar.Baz</c>
    /// and the binder will attempt to subscribe to changes on each recursively.
    /// </param>
    /// <param name="viewProperty">
    /// The property on the view that is to be bound.
    /// This can be a chain of properties of the form. <c>view => view.Foo.Bar.Baz</c>
    /// and the binder will attempt to set the last one each time the view model property is updated.
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
    public static IReactiveBinding<TView, TVProp> OneWayBind<TViewModel, TView, TVMProp, TVProp>(
        this TView view,
        TViewModel? viewModel,
        Expression<Func<TViewModel, TVMProp?>> vmProperty,
        Expression<Func<TView, TVProp>> viewProperty,
        object? conversionHint = null,
        IBindingTypeConverter? vmToViewConverterOverride = null)
        where TViewModel : class
        where TView : class, IViewFor =>
        _binderImplementation.OneWayBind(
                                         viewModel,
                                         view,
                                         vmProperty,
                                         viewProperty,
                                         conversionHint,
                                         vmToViewConverterOverride);

    /// <summary>
    /// Binds the specified view model property to the given view, in a one-way (view model to view) fashion,
    /// with the value of the view model property mapped through a <paramref name="selector"/> function.
    /// </summary>
    /// <typeparam name="TViewModel">The type of the view model that is bound.</typeparam>
    /// <typeparam name="TView">The type of the view that is bound.</typeparam>
    /// <typeparam name="TProp">The type of the property bound on the view model.</typeparam>
    /// <typeparam name="TOut">The return type of the <paramref name="selector"/>.</typeparam>
    /// <param name="view">The instance of the view to bind to.</param>
    /// <param name="viewModel">The instance of the view model to bind to.</param>
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
    /// <param name="selector">
    /// A function that will be used to transform the values of the property on the view model
    /// before being bound to the view property.
    /// </param>
    /// <returns>
    /// An instance of <see cref="IDisposable"/> that, when disposed,
    /// disconnects the binding.
    /// </returns>
    public static IReactiveBinding<TView, TOut> OneWayBind<TViewModel, TView, TProp, TOut>(
        this TView view,
        TViewModel? viewModel,
        Expression<Func<TViewModel, TProp?>> vmProperty,
        Expression<Func<TView, TOut>> viewProperty,
        Func<TProp, TOut> selector)
        where TViewModel : class
        where TView : class, IViewFor =>
        _binderImplementation.OneWayBind(viewModel, view, vmProperty, viewProperty, selector);

    /// <summary>
    /// BindTo takes an Observable stream and applies it to a target
    /// property. Conceptually it is similar to. <code>Subscribe(x =&gt;
    /// target.property = x)</code>, but allows you to use child properties
    /// without the null checks.
    /// </summary>
    /// <typeparam name="TValue">The source type.</typeparam>
    /// <typeparam name="TTarget">The target object type.</typeparam>
    /// <typeparam name="TTValue">The type of the property on the target object.</typeparam>
    /// <param name="this">The observable stream to bind to a target property.</param>
    /// <param name="target">The target object whose property will be set.</param>
    /// <param name="property">
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
    public static IDisposable BindTo<TValue, TTarget, TTValue>(
        this IObservable<TValue> @this,
        TTarget? target,
        Expression<Func<TTarget, TTValue?>> property,
        object? conversionHint = null,
        IBindingTypeConverter? vmToViewConverterOverride = null)
        where TTarget : class =>
        _binderImplementation.BindTo(@this, target, property, conversionHint, vmToViewConverterOverride);
}