// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using Splat;

namespace ReactiveUI;

/// <summary>
/// This interface represents an object that is capable
/// of providing binding implementations.
/// </summary>
public interface IPropertyBinderImplementation : IEnableLogger
{
    /// <summary>
    /// Creates a two-way binding between a view model and a view using a conversion hint and no converter overrides.
    /// </summary>
    /// <typeparam name="TViewModel">The type of the view model that is bound.</typeparam>
    /// <typeparam name="TView">The type of the view being bound.</typeparam>
    /// <typeparam name="TViewModelPropertyType">The type of the property bound on the view model.</typeparam>
    /// <typeparam name="TViewPropertyType">The type of the property bound on the view.</typeparam>
    /// <typeparam name="TDontCare">A dummy type used only to signal view updates.</typeparam>
    /// <param name="viewModel">The instance of the view model object to be bound.</param>
    /// <param name="view">The instance of the view object to be bound.</param>
    /// <param name="viewModelProperty">An expression representing the property to be bound on the view model.</param>
    /// <param name="viewProperty">An expression representing the property to be bound on the view.</param>
    /// <param name="signalViewUpdate">An observable that signals when the view property has changed.</param>
    /// <param name="conversionHint">An object that can provide a hint for the converter.</param>
    /// <returns>An instance of IDisposable that, when disposed, disconnects the binding.</returns>
    IReactiveBinding<TView, (object? view, bool isViewModel)> Bind<TViewModel, TView, TViewModelPropertyType, TViewPropertyType, TDontCare>(
        TViewModel? viewModel,
        TView view,
        Expression<Func<TViewModel, TViewModelPropertyType?>> viewModelProperty,
        Expression<Func<TView, TViewPropertyType>> viewProperty,
        IObservable<TDontCare>? signalViewUpdate,
        object? conversionHint)
        where TViewModel : class
        where TView : class, IViewFor;

    /// <summary>
    /// Creates a two-way binding between a view model and a view using a conversion hint and an optional vm-to-view converter override.
    /// </summary>
    /// <typeparam name="TViewModel">The type of the view model that is bound.</typeparam>
    /// <typeparam name="TView">The type of the view being bound.</typeparam>
    /// <typeparam name="TViewModelPropertyType">The type of the property bound on the view model.</typeparam>
    /// <typeparam name="TViewPropertyType">The type of the property bound on the view.</typeparam>
    /// <typeparam name="TDontCare">A dummy type used only to signal view updates.</typeparam>
    /// <param name="viewModel">The instance of the view model object to be bound.</param>
    /// <param name="view">The instance of the view object to be bound.</param>
    /// <param name="viewModelProperty">An expression representing the property to be bound on the view model.</param>
    /// <param name="viewProperty">An expression representing the property to be bound on the view.</param>
    /// <param name="signalViewUpdate">An observable that signals when the view property has changed.</param>
    /// <param name="conversionHint">An object that can provide a hint for the converter.</param>
    /// <param name="viewModelToViewConverterOverride">An optional converter to use when converting from view model to view property.</param>
    /// <returns>An instance of IDisposable that, when disposed, disconnects the binding.</returns>
    IReactiveBinding<TView, (object? view, bool isViewModel)> Bind<TViewModel, TView, TViewModelPropertyType, TViewPropertyType, TDontCare>(
        TViewModel? viewModel,
        TView view,
        Expression<Func<TViewModel, TViewModelPropertyType?>> viewModelProperty,
        Expression<Func<TView, TViewPropertyType>> viewProperty,
        IObservable<TDontCare>? signalViewUpdate,
        object? conversionHint,
        IBindingTypeConverter? viewModelToViewConverterOverride)
        where TViewModel : class
        where TView : class, IViewFor;

    /// <summary>
    /// Creates a two-way binding between a view model and a view using a conversion hint and converter overrides.
    /// </summary>
    /// <typeparam name="TViewModel">The type of the view model that is bound.</typeparam>
    /// <typeparam name="TView">The type of the view being bound.</typeparam>
    /// <typeparam name="TViewModelPropertyType">The type of the property bound on the view model.</typeparam>
    /// <typeparam name="TViewPropertyType">The type of the property bound on the view.</typeparam>
    /// <typeparam name="TDontCare">A dummy type used only to signal view updates.</typeparam>
    /// <param name="viewModel">The instance of the view model object to be bound.</param>
    /// <param name="view">The instance of the view object to be bound.</param>
    /// <param name="viewModelProperty">An expression representing the property to be bound on the view model.</param>
    /// <param name="viewProperty">An expression representing the property to be bound on the view.</param>
    /// <param name="signalViewUpdate">An observable that signals when the view property has changed.</param>
    /// <param name="conversionHint">An object that can provide a hint for the converter.</param>
    /// <param name="viewModelToViewConverterOverride">An optional converter to use when converting from view model to view property.</param>
    /// <param name="viewToViewModelConverterOverride">An optional converter to use when converting from view to view model property.</param>
    /// <returns>An instance of IDisposable that, when disposed, disconnects the binding.</returns>
    [SuppressMessage(
        "Major Code Smell",
        "S107:Methods should not have too many parameters",
        Justification = "This overload is part of the public binding API surface; the parameter count is intentional.")]
    IReactiveBinding<TView, (object? view, bool isViewModel)> Bind<TViewModel, TView, TViewModelPropertyType, TViewPropertyType, TDontCare>(
        TViewModel? viewModel,
        TView view,
        Expression<Func<TViewModel, TViewModelPropertyType?>> viewModelProperty,
        Expression<Func<TView, TViewPropertyType>> viewProperty,
        IObservable<TDontCare>? signalViewUpdate,
        object? conversionHint,
        IBindingTypeConverter? viewModelToViewConverterOverride,
        IBindingTypeConverter? viewToViewModelConverterOverride)
        where TViewModel : class
        where TView : class, IViewFor;

    /// <summary>
    /// Creates a two-way binding between a view model and a view with full control over converter overrides and trigger direction.
    /// </summary>
    /// <typeparam name="TViewModel">The type of the view model that is bound.</typeparam>
    /// <typeparam name="TView">The type of the view being bound.</typeparam>
    /// <typeparam name="TViewModelPropertyType">The type of the property bound on the view model.</typeparam>
    /// <typeparam name="TViewPropertyType">The type of the property bound on the view.</typeparam>
    /// <typeparam name="TDontCare">A dummy type used only to signal view updates.</typeparam>
    /// <param name="viewModel">The instance of the view model object to be bound.</param>
    /// <param name="view">The instance of the view object to be bound.</param>
    /// <param name="viewModelProperty">An expression representing the property to be bound on the view model.</param>
    /// <param name="viewProperty">An expression representing the property to be bound on the view.</param>
    /// <param name="signalViewUpdate">An observable that signals when the view property has changed.</param>
    /// <param name="conversionHint">An object that can provide a hint for the converter.</param>
    /// <param name="viewModelToViewConverterOverride">An optional converter to use when converting from view model to view property.</param>
    /// <param name="viewToViewModelConverterOverride">An optional converter to use when converting from view to view model property.</param>
    /// <param name="triggerUpdate">The trigger update direction.</param>
    /// <returns>An instance of IDisposable that, when disposed, disconnects the binding.</returns>
    [SuppressMessage(
        "Major Code Smell",
        "S107:Methods should not have too many parameters",
        Justification = "This overload is part of the public binding API surface; the parameter count is intentional.")]
    IReactiveBinding<TView, (object? view, bool isViewModel)> Bind<TViewModel, TView, TViewModelPropertyType, TViewPropertyType, TDontCare>(
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
        where TView : class, IViewFor;

    /// <summary>
    /// Creates a two-way binding between a view model and a view using explicit converter delegates with default trigger direction.
    /// </summary>
    /// <typeparam name="TViewModel">The type of the view model that is bound.</typeparam>
    /// <typeparam name="TView">The type of the view being bound.</typeparam>
    /// <typeparam name="TViewModelPropertyType">The type of the property bound on the view model.</typeparam>
    /// <typeparam name="TViewPropertyType">The type of the property bound on the view.</typeparam>
    /// <typeparam name="TDontCare">A dummy type used only to signal view updates.</typeparam>
    /// <param name="viewModel">The instance of the view model object to be bound.</param>
    /// <param name="view">The instance of the view object to be bound.</param>
    /// <param name="viewModelProperty">An expression representing the property to be bound on the view model.</param>
    /// <param name="viewProperty">An expression representing the property to be bound on the view.</param>
    /// <param name="signalViewUpdate">An observable that signals when the view property has changed.</param>
    /// <param name="viewModelToViewConverter">Delegate to convert the view model property value to the view property type.</param>
    /// <param name="viewToViewModelConverter">Delegate to convert the view property value to the view model property type.</param>
    /// <returns>An instance of IDisposable that, when disposed, disconnects the binding.</returns>
    IReactiveBinding<TView, (object? view, bool isViewModel)> Bind<TViewModel, TView, TViewModelPropertyType, TViewPropertyType, TDontCare>(
        TViewModel? viewModel,
        TView view,
        Expression<Func<TViewModel, TViewModelPropertyType?>> viewModelProperty,
        Expression<Func<TView, TViewPropertyType>> viewProperty,
        IObservable<TDontCare>? signalViewUpdate,
        Func<TViewModelPropertyType?, TViewPropertyType> viewModelToViewConverter,
        Func<TViewPropertyType, TViewModelPropertyType?> viewToViewModelConverter)
        where TViewModel : class
        where TView : class, IViewFor;

    /// <summary>
    /// Creates a two-way binding between a view model and a view using explicit converter delegates and a specified trigger direction.
    /// </summary>
    /// <typeparam name="TViewModel">The type of the view model that is bound.</typeparam>
    /// <typeparam name="TView">The type of the view being bound.</typeparam>
    /// <typeparam name="TViewModelPropertyType">The type of the property bound on the view model.</typeparam>
    /// <typeparam name="TViewPropertyType">The type of the property bound on the view.</typeparam>
    /// <typeparam name="TDontCare">A dummy type used only to signal view updates.</typeparam>
    /// <param name="viewModel">The instance of the view model object to be bound.</param>
    /// <param name="view">The instance of the view object to be bound.</param>
    /// <param name="viewModelProperty">An expression representing the property to be bound on the view model.</param>
    /// <param name="viewProperty">An expression representing the property to be bound on the view.</param>
    /// <param name="signalViewUpdate">An observable that signals when the view property has changed.</param>
    /// <param name="viewModelToViewConverter">Delegate to convert the view model property value to the view property type.</param>
    /// <param name="viewToViewModelConverter">Delegate to convert the view property value to the view model property type.</param>
    /// <param name="triggerUpdate">The trigger update direction.</param>
    /// <returns>An instance of IDisposable that, when disposed, disconnects the binding.</returns>
    [SuppressMessage(
        "Major Code Smell",
        "S107:Methods should not have too many parameters",
        Justification = "This overload is part of the public binding API surface; the parameter count is intentional.")]
    IReactiveBinding<TView, (object? view, bool isViewModel)> Bind<TViewModel, TView, TViewModelPropertyType, TViewPropertyType, TDontCare>(
        TViewModel? viewModel,
        TView view,
        Expression<Func<TViewModel, TViewModelPropertyType?>> viewModelProperty,
        Expression<Func<TView, TViewPropertyType>> viewProperty,
        IObservable<TDontCare>? signalViewUpdate,
        Func<TViewModelPropertyType?, TViewPropertyType> viewModelToViewConverter,
        Func<TViewPropertyType, TViewModelPropertyType?> viewToViewModelConverter,
        TriggerUpdate triggerUpdate)
        where TViewModel : class
        where TView : class, IViewFor;

    /// <summary>
    /// Creates a one-way binding from view model to view using default converters and conversion hint.
    /// </summary>
    /// <typeparam name="TViewModel">The type of the view model that is bound.</typeparam>
    /// <typeparam name="TView">The type of the view that is bound.</typeparam>
    /// <typeparam name="TViewModelPropertyType">The type of the property bound on the view model.</typeparam>
    /// <typeparam name="TViewPropertyType">The type of the property bound on the view.</typeparam>
    /// <param name="viewModel">The instance of the view model to bind to.</param>
    /// <param name="view">The instance of the view to bind to.</param>
    /// <param name="viewModelProperty">An expression representing the property to be bound on the view model.</param>
    /// <param name="viewProperty">An expression representing the property to be bound on the view.</param>
    /// <returns>An instance of IDisposable that, when disposed, disconnects the binding.</returns>
    IReactiveBinding<TView, TViewPropertyType> OneWayBind<TViewModel, TView, TViewModelPropertyType, TViewPropertyType>(
        TViewModel? viewModel,
        TView view,
        Expression<Func<TViewModel, TViewModelPropertyType?>> viewModelProperty,
        Expression<Func<TView, TViewPropertyType>> viewProperty)
        where TViewModel : class
        where TView : class, IViewFor;

    /// <summary>
    /// Creates a one-way binding from view model to view with an optional converter override and default conversion hint.
    /// </summary>
    /// <typeparam name="TViewModel">The type of the view model that is bound.</typeparam>
    /// <typeparam name="TView">The type of the view that is bound.</typeparam>
    /// <typeparam name="TViewModelPropertyType">The type of the property bound on the view model.</typeparam>
    /// <typeparam name="TViewPropertyType">The type of the property bound on the view.</typeparam>
    /// <param name="viewModel">The instance of the view model to bind to.</param>
    /// <param name="view">The instance of the view to bind to.</param>
    /// <param name="viewModelProperty">An expression representing the property to be bound on the view model.</param>
    /// <param name="viewProperty">An expression representing the property to be bound on the view.</param>
    /// <param name="vmToViewConverterOverride">An optional converter to use when converting from view model to view property.</param>
    /// <returns>An instance of IDisposable that, when disposed, disconnects the binding.</returns>
    IReactiveBinding<TView, TViewPropertyType> OneWayBind<TViewModel, TView, TViewModelPropertyType, TViewPropertyType>(
        TViewModel? viewModel,
        TView view,
        Expression<Func<TViewModel, TViewModelPropertyType?>> viewModelProperty,
        Expression<Func<TView, TViewPropertyType>> viewProperty,
        IBindingTypeConverter? vmToViewConverterOverride)
        where TViewModel : class
        where TView : class, IViewFor;

    /// <summary>
    /// Creates a one-way binding from view model to view using only a conversion hint.
    /// </summary>
    /// <typeparam name="TViewModel">The type of the view model that is bound.</typeparam>
    /// <typeparam name="TView">The type of the view that is bound.</typeparam>
    /// <typeparam name="TViewModelPropertyType">The type of the property bound on the view model.</typeparam>
    /// <typeparam name="TViewPropertyType">The type of the property bound on the view.</typeparam>
    /// <param name="viewModel">The instance of the view model to bind to.</param>
    /// <param name="view">The instance of the view to bind to.</param>
    /// <param name="viewModelProperty">An expression representing the property to be bound on the view model.</param>
    /// <param name="viewProperty">An expression representing the property to be bound on the view.</param>
    /// <param name="conversionHint">An object that can provide a hint for the converter.</param>
    /// <returns>An instance of IDisposable that, when disposed, disconnects the binding.</returns>
    IReactiveBinding<TView, TViewPropertyType> OneWayBind<TViewModel, TView, TViewModelPropertyType, TViewPropertyType>(
        TViewModel? viewModel,
        TView view,
        Expression<Func<TViewModel, TViewModelPropertyType?>> viewModelProperty,
        Expression<Func<TView, TViewPropertyType>> viewProperty,
        object? conversionHint)
        where TViewModel : class
        where TView : class, IViewFor;

    /// <summary>
    /// Creates a one-way binding from view model to view with an optional converter override.
    /// </summary>
    /// <typeparam name="TViewModel">The type of the view model that is bound.</typeparam>
    /// <typeparam name="TView">The type of the view that is bound.</typeparam>
    /// <typeparam name="TViewModelPropertyType">The type of the property bound on the view model.</typeparam>
    /// <typeparam name="TViewPropertyType">The type of the property bound on the view.</typeparam>
    /// <param name="viewModel">The instance of the view model to bind to.</param>
    /// <param name="view">The instance of the view to bind to.</param>
    /// <param name="viewModelProperty">An expression representing the property to be bound on the view model.</param>
    /// <param name="viewProperty">An expression representing the property to be bound on the view.</param>
    /// <param name="conversionHint">An object that can provide a hint for the converter.</param>
    /// <param name="viewModelToViewConverterOverride">An optional converter to use when converting from view model to view property.</param>
    /// <returns>An instance of IDisposable that, when disposed, disconnects the binding.</returns>
    IReactiveBinding<TView, TViewPropertyType> OneWayBind<TViewModel, TView, TViewModelPropertyType, TViewPropertyType>(
        TViewModel? viewModel,
        TView view,
        Expression<Func<TViewModel, TViewModelPropertyType?>> viewModelProperty,
        Expression<Func<TView, TViewPropertyType>> viewProperty,
        object? conversionHint,
        IBindingTypeConverter? viewModelToViewConverterOverride)
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
    /// <param name="viewModelProperty">
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
        Expression<Func<TViewModel, TProp>> viewModelProperty,
        Expression<Func<TView, TOut>> viewProperty,
        Func<TProp, TOut> selector)
        where TViewModel : class
        where TView : class, IViewFor;

    /// <summary>
    /// Binds an observable stream to a target property using default converters and conversion hint.
    /// </summary>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <typeparam name="TTarget">The target type.</typeparam>
    /// <typeparam name="TTargetValue">The target value type.</typeparam>
    /// <param name="observedChange">The target observable to bind to.</param>
    /// <param name="target">The target object whose property will be set.</param>
    /// <param name="propertyExpression">An expression representing the target property to set.</param>
    /// <returns>An object that when disposed, disconnects the binding.</returns>
    IDisposable BindTo<TValue, TTarget, TTargetValue>(
        IObservable<TValue> observedChange,
        TTarget? target,
        Expression<Func<TTarget, TTargetValue?>> propertyExpression)
        where TTarget : class;

    /// <summary>
    /// Binds an observable stream to a target property using only a conversion hint.
    /// </summary>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <typeparam name="TTarget">The target type.</typeparam>
    /// <typeparam name="TTargetValue">The target value type.</typeparam>
    /// <param name="observedChange">The target observable to bind to.</param>
    /// <param name="target">The target object whose property will be set.</param>
    /// <param name="propertyExpression">An expression representing the target property to set.</param>
    /// <param name="conversionHint">An object that can provide a hint for the converter.</param>
    /// <returns>An object that when disposed, disconnects the binding.</returns>
    IDisposable BindTo<TValue, TTarget, TTargetValue>(
        IObservable<TValue> observedChange,
        TTarget? target,
        Expression<Func<TTarget, TTargetValue?>> propertyExpression,
        object? conversionHint)
        where TTarget : class;

    /// <summary>
    /// Binds an observable stream to a target property with an optional converter override.
    /// </summary>
    /// <typeparam name="TValue">The value type.</typeparam>
    /// <typeparam name="TTarget">The target type.</typeparam>
    /// <typeparam name="TTargetValue">The target value type.</typeparam>
    /// <param name="observedChange">The target observable to bind to.</param>
    /// <param name="target">The target object whose property will be set.</param>
    /// <param name="propertyExpression">An expression representing the target property to set.</param>
    /// <param name="conversionHint">An object that can provide a hint for the converter.</param>
    /// <param name="viewModelToViewConverterOverride">An optional converter to use when converting from the observable value to the target property type.</param>
    /// <returns>An object that when disposed, disconnects the binding.</returns>
    IDisposable BindTo<TValue, TTarget, TTargetValue>(
        IObservable<TValue> observedChange,
        TTarget? target,
        Expression<Func<TTarget, TTargetValue?>> propertyExpression,
        object? conversionHint,
        IBindingTypeConverter? viewModelToViewConverterOverride)
        where TTarget : class;
}
