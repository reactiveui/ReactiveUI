// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using Splat;

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive;
#else
namespace ReactiveUI;
#endif
/// <summary>This class provides extension methods for the ReactiveUI view binding mechanism.</summary>
/// <remarks>
/// <para>
/// The helpers in this class are typically consumed within a view's <c>WhenActivated</c> block to connect a view model
/// property to a control and automatically dispose the binding when the view deactivates. Converters can be supplied via
/// <see cref="IBindingTypeConverter"/> instances or delegates, and <see cref="TriggerUpdate"/> indicates whether bindings
/// push values from the view model, the view, or both.
/// </para>
/// </remarks>
/// <example>
/// <code language="csharp">
/// <![CDATA[
/// this.WhenActivated(disposables =>
/// {
///     this.Bind(ViewModel, vm => vm.UserName, v => v.UserNameTextBox.Text)
///         .DisposeWith(disposables);
///
///     this.OneWayBind(ViewModel, vm => vm.IsBusy, v => v.Spinner.IsRunning)
///         .DisposeWith(disposables);
/// });
/// ]]>
/// </code>
/// </example>
[RequiresUnreferencedCode("Evaluates expression-based member chains via reflection; members may be trimmed.")]
[RequiresDynamicCode(
    "Uses dynamic binding paths which may require runtime code generation or reflection-based invocation.")]
public static class PropertyBindingMixins
{
    /// <summary>Initializes static members of the <see cref="PropertyBindingMixins"/> class.</summary>
    /// <remarks>This static constructor is called automatically to perform type-level initialization before
    /// any static members are accessed or any static methods are invoked. It is not intended to be called directly by
    /// user code.</remarks>
    static PropertyBindingMixins() => BinderImplementation = new PropertyBinderImplementation();

    /// <summary>
    /// Gets the binder implementation used by the extension methods in this class. Resolves the registered
    /// <see cref="IPropertyBinderImplementation"/> (e.g. the WPF/WinForms binder that marshals view updates onto the
    /// UI thread) from the locator, falling back to the platform-agnostic default when none is registered.
    /// </summary>
    private static IPropertyBinderImplementation BinderImplementation =>
        AppLocator.Current.GetService<IPropertyBinderImplementation>() ?? field;

    /// <summary>Provides BindTo extension members for <see cref="IObservable{T}"/> streams.</summary>
    /// <typeparam name="TValue">The source type.</typeparam>
    /// <param name="this">The observable stream to bind to a target property.</param>
    extension<TValue>(IObservable<TValue> @this)
    {
        /// <summary>BindTo takes an Observable stream and applies it to a target property.</summary>
        /// <typeparam name="TTarget">The target object type.</typeparam>
        /// <typeparam name="TTargetValue">The type of the property on the target object.</typeparam>
        /// <param name="target">The target object whose property will be set.</param>
        /// <param name="property">An expression representing the target property to set.</param>
        /// <returns>An object that when disposed, disconnects the binding.</returns>
        [RequiresUnreferencedCode("Evaluates expression-based member chains via reflection; members may be trimmed.")]
        [RequiresDynamicCode("Uses dynamic binding paths which may require runtime code generation or reflection-based invocation.")]
        public IDisposable BindTo<TTarget, TTargetValue>(
            TTarget? target,
            Expression<Func<TTarget, TTargetValue?>> property)
            where TTarget : class =>
            BindTo(@this, target, property, null, null);

        /// <summary>BindTo takes an Observable stream and applies it to a target property.</summary>
        /// <typeparam name="TTarget">The target object type.</typeparam>
        /// <typeparam name="TTargetValue">The type of the property on the target object.</typeparam>
        /// <param name="target">The target object whose property will be set.</param>
        /// <param name="property">An expression representing the target property to set.</param>
        /// <param name="conversionHint">An object that can provide a hint for the converter.</param>
        /// <returns>An object that when disposed, disconnects the binding.</returns>
        [RequiresUnreferencedCode("Evaluates expression-based member chains via reflection; members may be trimmed.")]
        [RequiresDynamicCode("Uses dynamic binding paths which may require runtime code generation or reflection-based invocation.")]
        public IDisposable BindTo<TTarget, TTargetValue>(
            TTarget? target,
            Expression<Func<TTarget, TTargetValue?>> property,
            object? conversionHint)
            where TTarget : class =>
            BindTo(@this, target, property, conversionHint, null);

        /// <summary>BindTo takes an Observable stream and applies it to a target property.</summary>
        /// <typeparam name="TTarget">The target object type.</typeparam>
        /// <typeparam name="TTargetValue">The type of the property on the target object.</typeparam>
        /// <param name="target">The target object whose property will be set.</param>
        /// <param name="property">An expression representing the target property to set.</param>
        /// <param name="viewModelToViewConverterOverride">
        /// An optional <see cref="IBindingTypeConverter"/> to use when converting from the
        /// viewModel to view property.
        /// </param>
        /// <returns>An object that when disposed, disconnects the binding.</returns>
        [RequiresUnreferencedCode("Evaluates expression-based member chains via reflection; members may be trimmed.")]
        [RequiresDynamicCode("Uses dynamic binding paths which may require runtime code generation or reflection-based invocation.")]
        public IDisposable BindTo<TTarget, TTargetValue>(
            TTarget? target,
            Expression<Func<TTarget, TTargetValue?>> property,
            IBindingTypeConverter? viewModelToViewConverterOverride)
            where TTarget : class =>
            BindTo(@this, target, property, null, viewModelToViewConverterOverride);

        /// <summary>
        /// BindTo takes an Observable stream and applies it to a target
        /// property. Conceptually it is similar to. <code>Subscribe(x =&gt;
        /// target.property = x)</code>, but allows you to use child properties
        /// without the null checks.
        /// </summary>
        /// <typeparam name="TTarget">The target object type.</typeparam>
        /// <typeparam name="TTargetValue">The type of the property on the target object.</typeparam>
        /// <param name="target">The target object whose property will be set.</param>
        /// <param name="property">
        /// An expression representing the target property to set.
        /// This can be a child property (i.e. <c>x.Foo.Bar.Baz</c>).
        /// </param>
        /// <param name="conversionHint">
        /// An object that can provide a hint for the converter.
        /// The semantics of this object is defined by the converter used.
        /// </param>
        /// <param name="viewModelToViewConverterOverride">
        /// An optional <see cref="IBindingTypeConverter"/> to use when converting from the
        /// viewModel to view property.
        /// </param>
        /// <returns>An object that when disposed, disconnects the binding.</returns>
        [RequiresUnreferencedCode("Evaluates expression-based member chains via reflection; members may be trimmed.")]
        [RequiresDynamicCode("Uses dynamic binding paths which may require runtime code generation or reflection-based invocation.")]
        public IDisposable BindTo<TTarget, TTargetValue>(
            TTarget? target,
            Expression<Func<TTarget, TTargetValue?>> property,
            object? conversionHint,
            IBindingTypeConverter? viewModelToViewConverterOverride)
            where TTarget : class =>
            BinderImplementation.BindTo(@this, target, property, conversionHint, viewModelToViewConverterOverride);
    }

    /// <summary>Provides Bind and OneWayBind extension members for views implementing <see cref="IViewFor"/>.</summary>
    /// <typeparam name="TView">The type of the view being bound.</typeparam>
    /// <param name="view">The instance of the view to bind.</param>
    extension<TView>(TView view)
        where TView : class, IViewFor
    {
        /// <summary>Binds the specified view model property to the given view property.</summary>
        /// <typeparam name="TViewModel">The type of the view model being bound.</typeparam>
        /// <typeparam name="TViewModelPropertyType">The type of the property bound on the view model.</typeparam>
        /// <typeparam name="TViewPropertyType">The type of the property bound on the view.</typeparam>
        /// <param name="viewModel">The instance of the view model to bind.</param>
        /// <param name="viewModelProperty">An expression indicating the property that is bound on the view model.</param>
        /// <param name="viewProperty">The property on the view that is to be bound.</param>
        /// <returns>An instance of IDisposable that, when disposed, disconnects the binding.</returns>
        [RequiresUnreferencedCode("Evaluates expression-based member chains via reflection; members may be trimmed.")]
        [RequiresDynamicCode("Uses dynamic binding paths which may require runtime code generation or reflection-based invocation.")]
        public IReactiveBinding<TView, (object? view, bool isViewModel)> Bind<TViewModel, TViewModelPropertyType, TViewPropertyType>(
            TViewModel? viewModel,
            Expression<Func<TViewModel, TViewModelPropertyType?>> viewModelProperty,
            Expression<Func<TView, TViewPropertyType>> viewProperty)
            where TViewModel : class =>
            Bind(view, viewModel, viewModelProperty, viewProperty, null, null, null);

        /// <summary>Binds the specified view model property to the given view property.</summary>
        /// <typeparam name="TViewModel">The type of the view model being bound.</typeparam>
        /// <typeparam name="TViewModelPropertyType">The type of the property bound on the view model.</typeparam>
        /// <typeparam name="TViewPropertyType">The type of the property bound on the view.</typeparam>
        /// <param name="viewModel">The instance of the view model to bind.</param>
        /// <param name="viewModelProperty">An expression indicating the property that is bound on the view model.</param>
        /// <param name="viewProperty">The property on the view that is to be bound.</param>
        /// <param name="conversionHint">An object that can provide a hint for the converter.</param>
        /// <returns>An instance of IDisposable that, when disposed, disconnects the binding.</returns>
        [RequiresUnreferencedCode("Evaluates expression-based member chains via reflection; members may be trimmed.")]
        [RequiresDynamicCode("Uses dynamic binding paths which may require runtime code generation or reflection-based invocation.")]
        public IReactiveBinding<TView, (object? view, bool isViewModel)> Bind<TViewModel, TViewModelPropertyType, TViewPropertyType>(
            TViewModel? viewModel,
            Expression<Func<TViewModel, TViewModelPropertyType?>> viewModelProperty,
            Expression<Func<TView, TViewPropertyType>> viewProperty,
            object? conversionHint)
            where TViewModel : class =>
            Bind(view, viewModel, viewModelProperty, viewProperty, conversionHint, null, null);

        /// <summary>Binds the specified view model property to the given view property.</summary>
        /// <typeparam name="TViewModel">The type of the view model being bound.</typeparam>
        /// <typeparam name="TViewModelPropertyType">The type of the property bound on the view model.</typeparam>
        /// <typeparam name="TViewPropertyType">The type of the property bound on the view.</typeparam>
        /// <param name="viewModel">The instance of the view model to bind.</param>
        /// <param name="viewModelProperty">An expression indicating the property that is bound on the view model.</param>
        /// <param name="viewProperty">The property on the view that is to be bound.</param>
        /// <param name="conversionHint">An object that can provide a hint for the converter.</param>
        /// <param name="viewModelToViewConverterOverride">An optional converter to use when converting from the view model to view property.</param>
        /// <returns>An instance of IDisposable that, when disposed, disconnects the binding.</returns>
        [RequiresUnreferencedCode("Evaluates expression-based member chains via reflection; members may be trimmed.")]
        [RequiresDynamicCode("Uses dynamic binding paths which may require runtime code generation or reflection-based invocation.")]
        public IReactiveBinding<TView, (object? view, bool isViewModel)> Bind<TViewModel, TViewModelPropertyType, TViewPropertyType>(
            TViewModel? viewModel,
            Expression<Func<TViewModel, TViewModelPropertyType?>> viewModelProperty,
            Expression<Func<TView, TViewPropertyType>> viewProperty,
            object? conversionHint,
            IBindingTypeConverter? viewModelToViewConverterOverride)
            where TViewModel : class =>
            Bind(view, viewModel, viewModelProperty, viewProperty, conversionHint, viewModelToViewConverterOverride, null);

        /// <summary>Binds the specified view model property to the given view property.</summary>
        /// <typeparam name="TViewModel">The type of the view model being bound.</typeparam>
        /// <typeparam name="TViewModelPropertyType">The type of the property bound on the view model.</typeparam>
        /// <typeparam name="TViewPropertyType">The type of the property bound on the view.</typeparam>
        /// <param name="viewModel">The instance of the view model to bind.</param>
        /// <param name="viewModelProperty">
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
        /// <param name="viewModelToViewConverterOverride">
        /// An optional <see cref="IBindingTypeConverter"/> to use when converting from the
        /// viewModel to view property.
        /// </param>
        /// <param name="viewToViewModelConverterOverride">
        /// An optional <see cref="IBindingTypeConverter"/> to use when converting from the
        /// view to viewModel property.
        /// </param>
        /// <returns>
        /// An instance of <see cref="IDisposable"/> that, when disposed,
        /// disconnects the binding.
        /// </returns>
        [RequiresUnreferencedCode("Evaluates expression-based member chains via reflection; members may be trimmed.")]
        [RequiresDynamicCode("Uses dynamic binding paths which may require runtime code generation or reflection-based invocation.")]
        public IReactiveBinding<TView, (object? view, bool isViewModel)> Bind<TViewModel, TViewModelPropertyType, TViewPropertyType>(
            TViewModel? viewModel,
            Expression<Func<TViewModel, TViewModelPropertyType?>> viewModelProperty,
            Expression<Func<TView, TViewPropertyType>> viewProperty,
            object? conversionHint,
            IBindingTypeConverter? viewModelToViewConverterOverride,
            IBindingTypeConverter? viewToViewModelConverterOverride)
            where TViewModel : class =>
            BinderImplementation.Bind(
                viewModel,
                view,
                viewModelProperty,
                viewProperty,
                (IObservable<RxVoid>?)null,
                conversionHint,
                viewModelToViewConverterOverride,
                viewToViewModelConverterOverride);

        /// <summary>Binds the specified view model property to the given view property with a custom view update signal.</summary>
        /// <typeparam name="TViewModel">The type of the view model being bound.</typeparam>
        /// <typeparam name="TViewModelPropertyType">The type of the property bound on the view model.</typeparam>
        /// <typeparam name="TViewPropertyType">The type of the property bound on the view.</typeparam>
        /// <typeparam name="TDontCare">A dummy type; only the fact that signalViewUpdate emits values is used.</typeparam>
        /// <param name="viewModel">The instance of the view model to bind.</param>
        /// <param name="viewModelProperty">An expression indicating the property that is bound on the view model.</param>
        /// <param name="viewProperty">The property on the view that is to be bound.</param>
        /// <param name="signalViewUpdate">An observable that, when signaled, indicates the view property has been changed.</param>
        /// <returns>An instance of IDisposable that, when disposed, disconnects the binding.</returns>
        [RequiresUnreferencedCode("Evaluates expression-based member chains via reflection; members may be trimmed.")]
        [RequiresDynamicCode("Uses dynamic binding paths which may require runtime code generation or reflection-based invocation.")]
        public IReactiveBinding<TView, (object? view, bool isViewModel)> Bind<
            TViewModel,
            TViewModelPropertyType,
            TViewPropertyType,
            TDontCare>(
            TViewModel? viewModel,
            Expression<Func<TViewModel, TViewModelPropertyType?>> viewModelProperty,
            Expression<Func<TView, TViewPropertyType>> viewProperty,
            IObservable<TDontCare>? signalViewUpdate)
            where TViewModel : class =>
            Bind(view, viewModel, viewModelProperty, viewProperty, signalViewUpdate, null, null, null, TriggerUpdate.ViewToViewModel);

        /// <summary>Binds the specified view model property to the given view property with a custom view update signal.</summary>
        /// <typeparam name="TViewModel">The type of the view model being bound.</typeparam>
        /// <typeparam name="TViewModelPropertyType">The type of the property bound on the view model.</typeparam>
        /// <typeparam name="TViewPropertyType">The type of the property bound on the view.</typeparam>
        /// <typeparam name="TDontCare">A dummy type; only the fact that signalViewUpdate emits values is used.</typeparam>
        /// <param name="viewModel">The instance of the view model to bind.</param>
        /// <param name="viewModelProperty">An expression indicating the property that is bound on the view model.</param>
        /// <param name="viewProperty">The property on the view that is to be bound.</param>
        /// <param name="signalViewUpdate">An observable that, when signaled, indicates the view property has been changed.</param>
        /// <param name="conversionHint">An object that can provide a hint for the converter.</param>
        /// <returns>An instance of IDisposable that, when disposed, disconnects the binding.</returns>
        [RequiresUnreferencedCode("Evaluates expression-based member chains via reflection; members may be trimmed.")]
        [RequiresDynamicCode("Uses dynamic binding paths which may require runtime code generation or reflection-based invocation.")]
        public IReactiveBinding<TView, (object? view, bool isViewModel)> Bind<
            TViewModel,
            TViewModelPropertyType,
            TViewPropertyType,
            TDontCare>(
            TViewModel? viewModel,
            Expression<Func<TViewModel, TViewModelPropertyType?>> viewModelProperty,
            Expression<Func<TView, TViewPropertyType>> viewProperty,
            IObservable<TDontCare>? signalViewUpdate,
            object? conversionHint)
            where TViewModel : class =>
            Bind(view, viewModel, viewModelProperty, viewProperty, signalViewUpdate, conversionHint, null, null, TriggerUpdate.ViewToViewModel);

        /// <summary>Binds the specified view model property to the given view property with a custom view update signal.</summary>
        /// <typeparam name="TViewModel">The type of the view model being bound.</typeparam>
        /// <typeparam name="TViewModelPropertyType">The type of the property bound on the view model.</typeparam>
        /// <typeparam name="TViewPropertyType">The type of the property bound on the view.</typeparam>
        /// <typeparam name="TDontCare">A dummy type; only the fact that signalViewUpdate emits values is used.</typeparam>
        /// <param name="viewModel">The instance of the view model to bind.</param>
        /// <param name="viewModelProperty">An expression indicating the property that is bound on the view model.</param>
        /// <param name="viewProperty">The property on the view that is to be bound.</param>
        /// <param name="signalViewUpdate">An observable that, when signaled, indicates the view property has been changed.</param>
        /// <param name="conversionHint">An object that can provide a hint for the converter.</param>
        /// <param name="viewModelToViewConverterOverride">An optional converter to use when converting from the view model to view property.</param>
        /// <returns>An instance of IDisposable that, when disposed, disconnects the binding.</returns>
        [RequiresUnreferencedCode("Evaluates expression-based member chains via reflection; members may be trimmed.")]
        [RequiresDynamicCode("Uses dynamic binding paths which may require runtime code generation or reflection-based invocation.")]
        public IReactiveBinding<TView, (object? view, bool isViewModel)> Bind<
            TViewModel,
            TViewModelPropertyType,
            TViewPropertyType,
            TDontCare>(
            TViewModel? viewModel,
            Expression<Func<TViewModel, TViewModelPropertyType?>> viewModelProperty,
            Expression<Func<TView, TViewPropertyType>> viewProperty,
            IObservable<TDontCare>? signalViewUpdate,
            object? conversionHint,
            IBindingTypeConverter? viewModelToViewConverterOverride)
            where TViewModel : class =>
            Bind(view, viewModel, viewModelProperty, viewProperty, signalViewUpdate, conversionHint, viewModelToViewConverterOverride, null, TriggerUpdate.ViewToViewModel);

        /// <summary>Binds the specified view model property to the given view property with a custom view update signal.</summary>
        /// <typeparam name="TViewModel">The type of the view model being bound.</typeparam>
        /// <typeparam name="TViewModelPropertyType">The type of the property bound on the view model.</typeparam>
        /// <typeparam name="TViewPropertyType">The type of the property bound on the view.</typeparam>
        /// <typeparam name="TDontCare">A dummy type; only the fact that signalViewUpdate emits values is used.</typeparam>
        /// <param name="viewModel">The instance of the view model to bind.</param>
        /// <param name="viewModelProperty">An expression indicating the property that is bound on the view model.</param>
        /// <param name="viewProperty">The property on the view that is to be bound.</param>
        /// <param name="signalViewUpdate">An observable that, when signaled, indicates the view property has been changed.</param>
        /// <param name="conversionHint">An object that can provide a hint for the converter.</param>
        /// <param name="viewModelToViewConverterOverride">An optional converter to use when converting from the view model to view property.</param>
        /// <param name="viewToViewModelConverterOverride">An optional converter to use when converting from the view to view model property.</param>
        /// <returns>An instance of IDisposable that, when disposed, disconnects the binding.</returns>
        [SuppressMessage(
            "Major Code Smell",
            "S107:Methods should not have too many parameters",
            Justification = "This overload is part of the public binding API surface; the parameter count is intentional.")]
        [RequiresUnreferencedCode("Evaluates expression-based member chains via reflection; members may be trimmed.")]
        [RequiresDynamicCode("Uses dynamic binding paths which may require runtime code generation or reflection-based invocation.")]
        public IReactiveBinding<TView, (object? view, bool isViewModel)> Bind<
            TViewModel,
            TViewModelPropertyType,
            TViewPropertyType,
            TDontCare>(
                TViewModel? viewModel,
                Expression<Func<TViewModel, TViewModelPropertyType?>> viewModelProperty,
                Expression<Func<TView, TViewPropertyType>> viewProperty,
                IObservable<TDontCare>? signalViewUpdate,
                object? conversionHint,
                IBindingTypeConverter? viewModelToViewConverterOverride,
                IBindingTypeConverter? viewToViewModelConverterOverride)
            where TViewModel : class =>
            Bind(view, viewModel, viewModelProperty, viewProperty, signalViewUpdate, conversionHint, viewModelToViewConverterOverride, viewToViewModelConverterOverride, TriggerUpdate.ViewToViewModel);

        /// <summary>
        /// Binds the specified view model property to the given view property, and
        /// provide a custom view update signaller to signal when the view property has been updated.
        /// </summary>
        /// <typeparam name="TViewModel">The type of the view model being bound.</typeparam>
        /// <typeparam name="TViewModelPropertyType">The type of the property bound on the view model.</typeparam>
        /// <typeparam name="TViewPropertyType">The type of the property bound on the view.</typeparam>
        /// <typeparam name="TDontCare">A dummy type, only the fact that <paramref name="signalViewUpdate" />
        /// emits values is considered, not the actual values emitted.</typeparam>
        /// <param name="viewModel">The instance of the view model to bind.</param>
        /// <param name="viewModelProperty">An expression indicating the property that is bound on the view model.
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
        /// <param name="viewModelToViewConverterOverride">An optional <see cref="IBindingTypeConverter" /> to use when converting from the
        /// viewModel to view property.</param>
        /// <param name="viewToViewModelConverterOverride">An optional <see cref="IBindingTypeConverter" /> to use when converting from the
        /// view to viewModel property.</param>
        /// <param name="triggerUpdate">The trigger update direction.</param>
        /// <returns>
        /// An instance of <see cref="IDisposable" /> that, when disposed,
        /// disconnects the binding.
        /// </returns>
        [SuppressMessage(
            "Major Code Smell",
            "S107:Methods should not have too many parameters",
            Justification = "This overload is part of the public binding API surface; the parameter count is intentional.")]
        [RequiresUnreferencedCode("Evaluates expression-based member chains via reflection; members may be trimmed.")]
        [RequiresDynamicCode("Uses dynamic binding paths which may require runtime code generation or reflection-based invocation.")]
        public IReactiveBinding<TView, (object? view, bool isViewModel)> Bind<
            TViewModel,
            TViewModelPropertyType,
            TViewPropertyType,
            TDontCare>(
            TViewModel? viewModel,
            Expression<Func<TViewModel, TViewModelPropertyType?>> viewModelProperty,
            Expression<Func<TView, TViewPropertyType>> viewProperty,
            IObservable<TDontCare>? signalViewUpdate,
            object? conversionHint,
            IBindingTypeConverter? viewModelToViewConverterOverride,
            IBindingTypeConverter? viewToViewModelConverterOverride,
            TriggerUpdate triggerUpdate)
            where TViewModel : class =>
            BinderImplementation.Bind(
                viewModel,
                view,
                viewModelProperty,
                viewProperty,
                signalViewUpdate,
                conversionHint,
                viewModelToViewConverterOverride,
                viewToViewModelConverterOverride,
                triggerUpdate);

        /// <summary>Binds the specified view model property to the given view property.</summary>
        /// <typeparam name="TViewModel">The type of the view model being bound.</typeparam>
        /// <typeparam name="TViewModelPropertyType">The type of the property bound on the view model.</typeparam>
        /// <typeparam name="TViewPropertyType">The type of the property bound on the view.</typeparam>
        /// <param name="viewModel">The instance of the view model to bind.</param>
        /// <param name="viewModelProperty">
        /// An expression indicating the property that is bound on the view model.
        /// This can be a chain of properties of the form. <c>vm =&gt; vm.Foo.Bar.Baz</c>
        /// and the binder will attempt to subscribe to changes on each recursively.
        /// </param>
        /// <param name="viewProperty">
        /// The property on the view that is to be bound.
        /// This can be a chain of properties of the form. <c>view => view.Foo.Bar.Baz</c>
        /// and the binder will attempt to set the last one each time the view model property is updated.
        /// </param>
        /// <param name="viewModelToViewConverter">
        /// Delegate to convert the value of the view model's property's type to a value of the
        /// view's property's type.
        /// </param>
        /// <param name="viewToViewModelConverter">
        /// Delegate to convert the value of the view's property's type to a value of the
        /// view model's property's type.
        /// </param>
        /// <returns>
        /// An instance of <see cref="IDisposable"/> that, when disposed,
        /// disconnects the binding.
        /// </returns>
        [RequiresUnreferencedCode("Evaluates expression-based member chains via reflection; members may be trimmed.")]
        [RequiresDynamicCode("Uses dynamic binding paths which may require runtime code generation or reflection-based invocation.")]
        public IReactiveBinding<TView, (object? view, bool isViewModel)> Bind<TViewModel, TViewModelPropertyType, TViewPropertyType>(
            TViewModel? viewModel,
            Expression<Func<TViewModel, TViewModelPropertyType?>> viewModelProperty,
            Expression<Func<TView, TViewPropertyType>> viewProperty,
            Func<TViewModelPropertyType?, TViewPropertyType> viewModelToViewConverter,
            Func<TViewPropertyType, TViewModelPropertyType?> viewToViewModelConverter)
            where TViewModel : class => BinderImplementation.Bind(
                viewModel,
                view,
                viewModelProperty,
                viewProperty,
                (IObservable<RxVoid>?)null,
                viewModelToViewConverter,
                viewToViewModelConverter);

        /// <summary>Binds the specified view model property to the given view property using delegate converters.</summary>
        /// <typeparam name="TViewModel">The type of the view model being bound.</typeparam>
        /// <typeparam name="TViewModelPropertyType">The type of the property bound on the view model.</typeparam>
        /// <typeparam name="TViewPropertyType">The type of the property bound on the view.</typeparam>
        /// <typeparam name="TDontCare">A dummy type; only the fact that signalViewUpdate emits values is used.</typeparam>
        /// <param name="viewModel">The instance of the view model to bind.</param>
        /// <param name="viewModelProperty">An expression indicating the property that is bound on the view model.</param>
        /// <param name="viewProperty">The property on the view that is to be bound.</param>
        /// <param name="signalViewUpdate">An observable that, when signaled, indicates the view property has been changed.</param>
        /// <param name="viewModelToViewConverter">Delegate to convert a view model property value to a view property value.</param>
        /// <param name="viewToViewModelConverter">Delegate to convert a view property value to a view model property value.</param>
        /// <returns>An instance of IDisposable that, when disposed, disconnects the binding.</returns>
        [RequiresUnreferencedCode("Evaluates expression-based member chains via reflection; members may be trimmed.")]
        [RequiresDynamicCode("Uses dynamic binding paths which may require runtime code generation or reflection-based invocation.")]
        public IReactiveBinding<TView, (object? view, bool isViewModel)> Bind<
            TViewModel,
            TViewModelPropertyType,
            TViewPropertyType,
            TDontCare>(
            TViewModel? viewModel,
            Expression<Func<TViewModel, TViewModelPropertyType?>> viewModelProperty,
            Expression<Func<TView, TViewPropertyType>> viewProperty,
            IObservable<TDontCare>? signalViewUpdate,
            Func<TViewModelPropertyType?, TViewPropertyType> viewModelToViewConverter,
            Func<TViewPropertyType, TViewModelPropertyType?> viewToViewModelConverter)
            where TViewModel : class =>
            Bind(view, viewModel, viewModelProperty, viewProperty, signalViewUpdate, viewModelToViewConverter, viewToViewModelConverter, TriggerUpdate.ViewToViewModel);

        /// <summary>Binds the specified view model property to the given view property.</summary>
        /// <typeparam name="TViewModel">The type of the view model being bound.</typeparam>
        /// <typeparam name="TViewModelPropertyType">The type of the property bound on the view model.</typeparam>
        /// <typeparam name="TViewPropertyType">The type of the property bound on the view.</typeparam>
        /// <typeparam name="TDontCare">A dummy type, only the fact that <paramref name="signalViewUpdate" />
        /// emits values is considered, not the actual values emitted.</typeparam>
        /// <param name="viewModel">The instance of the view model to bind.</param>
        /// <param name="viewModelProperty">An expression indicating the property that is bound on the view model.
        /// This can be a chain of properties of the form. <c>vm =&gt; vm.Foo.Bar.Baz</c>
        /// and the binder will attempt to subscribe to changes on each recursively.</param>
        /// <param name="viewProperty">The property on the view that is to be bound.
        /// This can be a chain of properties of the form. <c>view =&gt; view.Foo.Bar.Baz</c>
        /// and the binder will attempt to set the last one each time the view model property is updated.</param>
        /// <param name="signalViewUpdate">An observable, that when signaled, indicates that the view property
        /// has been changed, and that the binding should update the view model
        /// property accordingly.</param>
        /// <param name="viewModelToViewConverter">Delegate to convert the value of the view model's property's type to a value of the
        /// view's property's type.</param>
        /// <param name="viewToViewModelConverter">Delegate to convert the value of the view's property's type to a value of the
        /// view model's property's type.</param>
        /// <param name="triggerUpdate">The trigger update direction.</param>
        /// <returns>
        /// An instance of <see cref="IDisposable" /> that, when disposed,
        /// disconnects the binding.
        /// </returns>
        [SuppressMessage(
            "Major Code Smell",
            "S107:Methods should not have too many parameters",
            Justification = "This overload is part of the public binding API surface; the parameter count is intentional.")]
        [RequiresUnreferencedCode("Evaluates expression-based member chains via reflection; members may be trimmed.")]
        [RequiresDynamicCode("Uses dynamic binding paths which may require runtime code generation or reflection-based invocation.")]
        public IReactiveBinding<TView, (object? view, bool isViewModel)> Bind<
            TViewModel,
            TViewModelPropertyType,
            TViewPropertyType,
            TDontCare>(
            TViewModel? viewModel,
            Expression<Func<TViewModel, TViewModelPropertyType?>> viewModelProperty,
            Expression<Func<TView, TViewPropertyType>> viewProperty,
            IObservable<TDontCare>? signalViewUpdate,
            Func<TViewModelPropertyType?, TViewPropertyType> viewModelToViewConverter,
            Func<TViewPropertyType, TViewModelPropertyType?> viewToViewModelConverter,
            TriggerUpdate triggerUpdate)
            where TViewModel : class =>
            BinderImplementation.Bind(
                viewModel,
                view,
                viewModelProperty,
                viewProperty,
                signalViewUpdate,
                viewModelToViewConverter,
                viewToViewModelConverter,
                triggerUpdate);

        /// <summary>Binds the given property on the view model to a given property on the view in a one-way fashion.</summary>
        /// <typeparam name="TViewModel">The type of the view model.</typeparam>
        /// <typeparam name="TViewModelPropertyType">The type of view model property.</typeparam>
        /// <typeparam name="TViewPropertyType">The type of the property bound on the view.</typeparam>
        /// <param name="viewModel">The view model that is bound.</param>
        /// <param name="viewModelProperty">An expression indicating the property that is bound on the view model.</param>
        /// <param name="viewProperty">The property on the view that is to be bound.</param>
        /// <returns>An instance of IDisposable that, when disposed, disconnects the binding.</returns>
        [RequiresUnreferencedCode("Evaluates expression-based member chains via reflection; members may be trimmed.")]
        [RequiresDynamicCode("Uses dynamic binding paths which may require runtime code generation or reflection-based invocation.")]
        public IReactiveBinding<TView, TViewPropertyType> OneWayBind<TViewModel, TViewModelPropertyType, TViewPropertyType>(
            TViewModel? viewModel,
            Expression<Func<TViewModel, TViewModelPropertyType?>> viewModelProperty,
            Expression<Func<TView, TViewPropertyType>> viewProperty)
            where TViewModel : class =>
            OneWayBind(view, viewModel, viewModelProperty, viewProperty, null, null);

        /// <summary>Binds the given property on the view model to a given property on the view in a one-way fashion.</summary>
        /// <typeparam name="TViewModel">The type of the view model.</typeparam>
        /// <typeparam name="TViewModelPropertyType">The type of view model property.</typeparam>
        /// <typeparam name="TViewPropertyType">The type of the property bound on the view.</typeparam>
        /// <param name="viewModel">The view model that is bound.</param>
        /// <param name="viewModelProperty">An expression indicating the property that is bound on the view model.</param>
        /// <param name="viewProperty">The property on the view that is to be bound.</param>
        /// <param name="conversionHint">An object that can provide a hint for the converter.</param>
        /// <returns>An instance of IDisposable that, when disposed, disconnects the binding.</returns>
        [RequiresUnreferencedCode("Evaluates expression-based member chains via reflection; members may be trimmed.")]
        [RequiresDynamicCode("Uses dynamic binding paths which may require runtime code generation or reflection-based invocation.")]
        public IReactiveBinding<TView, TViewPropertyType> OneWayBind<TViewModel, TViewModelPropertyType, TViewPropertyType>(
            TViewModel? viewModel,
            Expression<Func<TViewModel, TViewModelPropertyType?>> viewModelProperty,
            Expression<Func<TView, TViewPropertyType>> viewProperty,
            object? conversionHint)
            where TViewModel : class =>
            OneWayBind(view, viewModel, viewModelProperty, viewProperty, conversionHint, null);

        /// <summary>Binds the given property on the view model to a given property on the view in a one-way (view model to view) fashion.</summary>
        /// <typeparam name="TViewModel">The type of the view model.</typeparam>
        /// <typeparam name="TViewModelPropertyType">The type of view model property.</typeparam>
        /// <typeparam name="TViewPropertyType">The type of the property bound on the view.</typeparam>
        /// <param name="viewModel">
        /// The view model that is bound.
        /// It is usually set to the <see cref="IViewFor.ViewModel"/> property of the <paramref name="view"/>.</param>
        /// <param name="viewModelProperty">
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
        /// <param name="viewModelToViewConverterOverride">
        /// An optional <see cref="IBindingTypeConverter"/> to use when converting from the
        /// viewModel to view property.
        /// </param>
        /// <returns>
        /// An instance of <see cref="IDisposable"/> that, when disposed,
        /// disconnects the binding.
        /// </returns>
        [RequiresUnreferencedCode("Evaluates expression-based member chains via reflection; members may be trimmed.")]
        [RequiresDynamicCode("Uses dynamic binding paths which may require runtime code generation or reflection-based invocation.")]
        public IReactiveBinding<TView, TViewPropertyType> OneWayBind<TViewModel, TViewModelPropertyType, TViewPropertyType>(
            TViewModel? viewModel,
            Expression<Func<TViewModel, TViewModelPropertyType?>> viewModelProperty,
            Expression<Func<TView, TViewPropertyType>> viewProperty,
            object? conversionHint,
            IBindingTypeConverter? viewModelToViewConverterOverride)
            where TViewModel : class =>
            BinderImplementation.OneWayBind(
                viewModel,
                view,
                viewModelProperty,
                viewProperty,
                conversionHint,
                viewModelToViewConverterOverride);

        /// <summary>
        /// Binds the specified view model property to the given view, in a one-way (view model to view) fashion,
        /// with the value of the view model property mapped through a <paramref name="selector"/> function.
        /// </summary>
        /// <typeparam name="TViewModel">The type of the view model that is bound.</typeparam>
        /// <typeparam name="TProp">The type of the property bound on the view model.</typeparam>
        /// <typeparam name="TOut">The return type of the <paramref name="selector"/>.</typeparam>
        /// <param name="viewModel">The instance of the view model to bind to.</param>
        /// <param name="viewModelProperty">
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
        [RequiresUnreferencedCode("Evaluates expression-based member chains via reflection; members may be trimmed.")]
        [RequiresDynamicCode("Uses dynamic binding paths which may require runtime code generation or reflection-based invocation.")]
        public IReactiveBinding<TView, TOut> OneWayBind<TViewModel, TProp, TOut>(
            TViewModel? viewModel,
            Expression<Func<TViewModel, TProp>> viewModelProperty,
            Expression<Func<TView, TOut>> viewProperty,
            Func<TProp, TOut> selector)
            where TViewModel : class =>
            BinderImplementation.OneWayBind(viewModel, view, viewModelProperty, viewProperty, selector);
    }
}
