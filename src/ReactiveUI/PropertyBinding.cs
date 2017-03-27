using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Splat;

namespace ReactiveUI
{
    /// <summary>
    /// This class provides extension methods for the ReactiveUI view binding mechanism.
    /// </summary>
    public static class BindingMixins
    {
        static IPropertyBinderImplementation binderImplementation;

        static BindingMixins()
        {
            RxApp.EnsureInitialized();
            binderImplementation = new PropertyBinderImplementation();
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
        /// This can be a chain of properties of the form <code>vm =&gt; vm.Foo.Bar.Baz</code>
        /// and the binder will attempt to subscribe to changes on each recursively.
        /// </param>
        /// <param name="viewProperty">
        /// The property on the view that is to be bound.
        /// This can be a chain of properties of the form <code>view => view.Foo.Bar.Baz</code>
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
        public static IReactiveBinding<TView, TViewModel, Tuple<object, bool>> Bind<TViewModel, TView, TVMProp, TVProp>(
                this TView view,
                TViewModel viewModel,
                Expression<Func<TViewModel, TVMProp>> vmProperty,
                Expression<Func<TView, TVProp>> viewProperty,
                object conversionHint = null,
                IBindingTypeConverter vmToViewConverterOverride = null,
                IBindingTypeConverter viewToVMConverterOverride = null)
            where TViewModel : class
            where TView : IViewFor
        {
            return binderImplementation.Bind(viewModel, view, vmProperty, viewProperty, (IObservable<Unit>)null, conversionHint,
                vmToViewConverterOverride, viewToVMConverterOverride);
        }

        /// <summary>
        /// Binds the specified view model property to the given view property, and 
        /// provide a custom view update signaller to signal when the view property has been updated.
        /// </summary>
        /// <typeparam name="TViewModel">The type of the view model being bound.</typeparam>
        /// <typeparam name="TView">The type of the view being bound.</typeparam>
        /// <typeparam name="TVMProp">The type of the property bound on the view model.</typeparam>
        /// <typeparam name="TVProp">The type of the property bound on the view.</typeparam>
        /// <param name="view">The instance of the view to bind.</param>
        /// <typeparam name="TDontCare">
        /// A dummy type, only the fact that <paramref name="signalViewUpdate"/> 
        /// emits values is considered, not the actual values emitted.
        /// </typeparam>
        /// <param name="viewModel">The instance of the view model to bind.</param>
        /// <param name="vmProperty">
        /// An expression indicating the property that is bound on the view model.
        /// This can be a chain of properties of the form <code>vm =&gt; vm.Foo.Bar.Baz</code>
        /// and the binder will attempt to subscribe to changes on each recursively.
        /// </param>
        /// <param name="viewProperty">
        /// The property on the view that is to be bound.
        /// This can be a chain of properties of the form <code>view => view.Foo.Bar.Baz</code>
        /// and the binder will attempt to set the last one each time the view model property is updated.
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
        public static IReactiveBinding<TView, TViewModel, Tuple<object, bool>> Bind<TViewModel, TView, TVMProp, TVProp, TDontCare>(
                this TView view,
                TViewModel viewModel,
                Expression<Func<TViewModel, TVMProp>> vmProperty,
                Expression<Func<TView, TVProp>> viewProperty,
                IObservable<TDontCare> signalViewUpdate,
                object conversionHint = null,
                IBindingTypeConverter vmToViewConverterOverride = null,
                IBindingTypeConverter viewToVMConverterOverride = null)
            where TViewModel : class
            where TView : IViewFor
        {
            return binderImplementation.Bind(viewModel, view, vmProperty, viewProperty, signalViewUpdate, conversionHint,
                vmToViewConverterOverride, viewToVMConverterOverride);
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
        /// This can be a chain of properties of the form <code>vm =&gt; vm.Foo.Bar.Baz</code>
        /// and the binder will attempt to subscribe to changes on each recursively.
        /// </param>
        /// <param name="viewProperty">
        /// The property on the view that is to be bound.
        /// This can be a chain of properties of the form <code>view => view.Foo.Bar.Baz</code>
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
        public static IReactiveBinding<TView, TViewModel, Tuple<object, bool>> Bind<TViewModel, TView, TVMProp, TVProp>(
            this TView view,
                TViewModel viewModel,
                Expression<Func<TViewModel, TVMProp>> vmProperty,
                Expression<Func<TView, TVProp>> viewProperty,
                Func<TVMProp, TVProp> vmToViewConverter,
                Func<TVProp, TVMProp> viewToVmConverter)
            where TViewModel : class
            where TView : IViewFor
        {
            return binderImplementation.Bind(viewModel, view, vmProperty, viewProperty, (IObservable<Unit>)null, vmToViewConverter, viewToVmConverter);
        }

        /// <summary>
        /// Binds the specified view model property to the given view property.
        /// </summary>
        /// <typeparam name="TViewModel">The type of the view model being bound.</typeparam>
        /// <typeparam name="TView">The type of the view being bound.</typeparam>
        /// <typeparam name="TVMProp">The type of the property bound on the view model.</typeparam>
        /// <typeparam name="TVProp">The type of the property bound on the view.</typeparam>
        /// /// <typeparam name="TDontCare">
        /// A dummy type, only the fact that <paramref name="signalViewUpdate"/> 
        /// emits values is considered, not the actual values emitted.
        /// </typeparam>
        /// <param name="view">The instance of the view to bind.</param>
        /// <param name="viewModel">The instance of the view model to bind.</param>
        /// <param name="vmProperty">
        /// An expression indicating the property that is bound on the view model.
        /// This can be a chain of properties of the form <code>vm =&gt; vm.Foo.Bar.Baz</code>
        /// and the binder will attempt to subscribe to changes on each recursively.
        /// </param>
        /// <param name="viewProperty">
        /// The property on the view that is to be bound.
        /// This can be a chain of properties of the form <code>view => view.Foo.Bar.Baz</code>
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
        public static IReactiveBinding<TView, TViewModel, Tuple<object, bool>> Bind<TViewModel, TView, TVMProp, TVProp, TDontCare>(
            this TView view,
                TViewModel viewModel,
                Expression<Func<TViewModel, TVMProp>> vmProperty,
                Expression<Func<TView, TVProp>> viewProperty,
                IObservable<TDontCare> signalViewUpdate,
                Func<TVMProp, TVProp> vmToViewConverter,
                Func<TVProp, TVMProp> viewToVmConverter)
            where TViewModel : class
            where TView : IViewFor
        {
            return binderImplementation.Bind(viewModel, view, vmProperty, viewProperty, signalViewUpdate, vmToViewConverter, viewToVmConverter);
        }

        /// <summary>
        /// Binds the given property on the view model to a given property on the view in a one-way (view model to view) fashion.
        /// </summary>
        /// <typeparam name="TViewModel">The type of the view model.</typeparam>
        /// <typeparam name="TView">The type of the view.</typeparam>
        /// <typeparam name="TVMProp">The type of view model property.</typeparam>
        /// <typeparam name="TVProp">The type of the property bound on the view.</typeparam>
        /// <param name="view">
        /// The instance of the view object which is bound. Usually, it is the <code>this</code>
        /// instance.
        /// </param>
        /// <param name="viewModel">
        /// The view model that is bound. 
        /// It is usually set to the <see cref="IViewFor.ViewModel"/> property of the <paramref name="view"/>.</param>
        /// <param name="vmProperty">
        /// An expression indicating the property that is bound on the view model.
        /// This can be a chain of properties of the form <code>vm => vm.Foo.Bar.Baz</code>
        /// and the binder will attempt to subscribe to changes on each recursively.
        /// </param>
        /// <param name="viewProperty">
        /// The property on the view that is to be bound.
        /// This can be a chain of properties of the form <code>view => view.Foo.Bar.Baz</code>
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
        public static IReactiveBinding<TView, TViewModel, TVProp> OneWayBind<TViewModel, TView, TVMProp, TVProp>(
                this TView view,
                TViewModel viewModel,
                Expression<Func<TViewModel, TVMProp>> vmProperty,
                Expression<Func<TView, TVProp>> viewProperty,
                object conversionHint = null,
                IBindingTypeConverter vmToViewConverterOverride = null)
            where TViewModel : class
            where TView : IViewFor
        {
            return binderImplementation.OneWayBind(viewModel, view, vmProperty, viewProperty, conversionHint,
                vmToViewConverterOverride);
        }

        /// <summary>
        /// Binds the specified view model property to the given view, in a one-way (view model to view) fashion,
        /// with the value of the view model property mapped through a <paramref name="selector"/> function.
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
        /// </param>
        /// <param name="selector">
        /// A function that will be used to transform the values of the property on the view model
        /// before being bound to the view property.
        /// </param>
        /// <returns>
        /// An instance of <see cref="IDisposable"/> that, when disposed,
        /// disconnects the binding.
        /// </returns>
        public static IReactiveBinding<TView, TViewModel, TOut> OneWayBind<TViewModel, TView, TProp, TOut>(
                this TView view,
                TViewModel viewModel,
                Expression<Func<TViewModel, TProp>> vmProperty,
                Expression<Func<TView, TOut>> viewProperty,
                Func<TProp, TOut> selector)
            where TViewModel : class
            where TView : IViewFor
        {
            return binderImplementation.OneWayBind(viewModel, view, vmProperty, viewProperty, selector);
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
        /// <param name="This">The observable stream to bind to a target property</param>
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
            this IObservable<TValue> This,
            TTarget target,
            Expression<Func<TTarget, TTValue>> property,
            object conversionHint = null,
            IBindingTypeConverter vmToViewConverterOverride = null)
        {
            return binderImplementation.BindTo(This, target, property, conversionHint, vmToViewConverterOverride);
        }
    }

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
        /// <typeparam name="TView">The type of the view model that is bound.</typeparam>
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
        IReactiveBinding<TView, TViewModel, Tuple<object, bool>> Bind<TViewModel, TView, TVMProp, TVProp, TDontCare>(
                TViewModel viewModel,
                TView view,
                Expression<Func<TViewModel, TVMProp>> vmProperty,
                Expression<Func<TView, TVProp>> viewProperty,
                IObservable<TDontCare> signalViewUpdate,
                object conversionHint,
                IBindingTypeConverter vmToViewConverterOverride = null,
                IBindingTypeConverter viewToVMConverterOverride = null)
            where TViewModel : class
            where TView : IViewFor;

        /// <summary>
        /// Creates a two-way binding between a view model and a view.
        /// This binding will attempt to convert the values of the 
        /// view and view model properties using a <see cref="IBindingTypeConverter"/>
        /// if they are not of the same type.
        /// </summary>
        /// <typeparam name="TViewModel">The type of the view model that is bound.</typeparam>
        /// <typeparam name="TView">The type of the view model that is bound.</typeparam>
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
        IReactiveBinding<TView, TViewModel, Tuple<object, bool>> Bind<TViewModel, TView, TVMProp, TVProp, TDontCare>(
                TViewModel viewModel,
                TView view,
                Expression<Func<TViewModel, TVMProp>> vmProperty,
                Expression<Func<TView, TVProp>> viewProperty,
                IObservable<TDontCare> signalViewUpdate,
                Func<TVMProp, TVProp> vmToViewConverter,
                Func<TVProp, TVMProp> viewToVmConverter)
            where TViewModel : class
            where TView : IViewFor;

        /// <summary>
        /// Creates a one-way binding, i.e. a binding that flows from the
        /// <paramref name="viewModel"/> to the <paramref name="view"/> only. This binding will
        /// attempt to convert the value of the view model property to the view property if they
        /// are not of the same type.
        /// </summary>
        /// <typeparam name="TViewModel">The type of the view model that is bound.</typeparam>
        /// <typeparam name="TView">The type of the view that is bound.</typeparam>
        /// <typeparam name="TVMProp">The type of the property bound on the view model.</typeparam>
        /// <typeparam name="TVProp">The type of the property bound on the view</typeparam>
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
        IReactiveBinding<TView, TViewModel, TVProp> OneWayBind<TViewModel, TView, TVMProp, TVProp>(
                TViewModel viewModel,
                TView view,
                Expression<Func<TViewModel, TVMProp>> vmProperty,
                Expression<Func<TView, TVProp>> viewProperty,
                object conversionHint,
                IBindingTypeConverter vmToViewConverterOverride = null)
            where TViewModel : class
            where TView : IViewFor;

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
        /// </param>
        /// <param name="selector">
        /// A function that will be used to transform the values of the property on the view model
        /// before being bound to the view property.
        /// </param>
        /// <returns>
        /// An instance of <see cref="IDisposable"/> that, when disposed,
        /// disconnects the binding.
        /// </returns>
        IReactiveBinding<TView, TViewModel, TOut> OneWayBind<TViewModel, TView, TProp, TOut>(
                TViewModel viewModel,
                TView view,
                Expression<Func<TViewModel, TProp>> vmProperty,
                Expression<Func<TView, TOut>> viewProperty,
                Func<TProp, TOut> selector)
            where TViewModel : class
            where TView : IViewFor;

        /// <summary>
        /// BindTo takes an Observable stream and applies it to a target
        /// property. Conceptually it is similar to <c>Subscribe(x =&gt;
        /// target.property = x)</c>, but allows you to use child properties
        /// without the null checks.
        /// </summary>
        /// <param name="This">The target observable to bind to.</param>
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
        IDisposable BindTo<TValue, TTarget, TTValue>(
            IObservable<TValue> This,
            TTarget target,
            Expression<Func<TTarget, TTValue>> property,
            object conversionHint,
            IBindingTypeConverter vmToViewConverterOverride = null);
    }

    /// <summary>
    /// Provides methods to bind properties to observables.
    /// </summary>
    public class PropertyBinderImplementation : IPropertyBinderImplementation
    {
        /// <summary>
        /// Creates a two-way binding between a view model and a view.
        /// This binding will attempt to convert the values of the 
        /// view and view model properties using a <see cref="IBindingTypeConverter"/>
        /// if they are not of the same type.
        /// </summary>
        /// <typeparam name="TViewModel">The type of the view model that is bound.</typeparam>
        /// <typeparam name="TView">The type of the view model that is bound.</typeparam>
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
        /// If it is left null, the framework will attempt to automagically figure out
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
            where TView : IViewFor
        {
            var vmToViewConverter = vmToViewConverterOverride ?? getConverterForTypes(typeof(TVMProp), typeof(TVProp));
            var viewToVMConverter = viewToVMConverterOverride ?? getConverterForTypes(typeof(TVProp), typeof(TVMProp));

            if (vmToViewConverter == null || viewToVMConverter == null) {
                throw new ArgumentException(
                    String.Format("Can't two-way convert between {0} and {1}. To fix this, register a IBindingTypeConverter or call the version with converter Funcs.", typeof(TVMProp), typeof(TVProp)));
            }

            OutFunc<TVMProp, TVProp> vmToViewFunc = (TVMProp vmValue, out TVProp vValue) => {
                object tmp;
                var result = vmToViewConverter.TryConvert(vmValue, typeof(TVProp), conversionHint, out tmp);

                vValue = result ? (TVProp)tmp : default(TVProp);
                return result;
            };
            OutFunc<TVProp, TVMProp> viewToVmFunc = (TVProp vValue, out TVMProp vmValue) => {
                object tmp;
                var result = viewToVMConverter.TryConvert(vValue, typeof(TVMProp), conversionHint, out tmp);

                vmValue = result ? (TVMProp)tmp : default(TVMProp);
                return result;
            };

            return bindImpl(viewModel, view, vmProperty, viewProperty, signalViewUpdate, vmToViewFunc, viewToVmFunc);
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
        /// <param name="view">The instance of the view to bind.</param>
        /// <param name="viewModel">The instance of the view model to bind.</param>
        /// <param name="vmProperty">
        /// An expression indicating the property that is bound on the view model.
        /// This can be a chain of properties of the form <c>vm =&gt; vm.Foo.Bar.Baz</c>
        /// and the binder will attempt to subscribe to changes on each recursively.
        /// </param>
        /// <param name="viewProperty">
        /// The property on the view that is to be bound.
        /// This can be a chain of properties of the form <code>view => view.Foo.Bar.Baz</code>
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
            where TView : IViewFor
        {
            if (vmToViewConverter == null) {
                throw new ArgumentNullException(nameof(vmToViewConverter));
            }

            if (viewToVmConverter == null) {
                throw new ArgumentNullException(nameof(viewToVmConverter));
            }

            OutFunc<TVMProp, TVProp> vmToViewFunc = (TVMProp vmValue, out TVProp vValue) => {
                vValue = vmToViewConverter(vmValue);
                return true;
            };
            OutFunc<TVProp, TVMProp> viewToVmFunc = (TVProp vValue, out TVMProp vmValue) => {
                vmValue = viewToVmConverter(vValue);
                return true;
            };

            return bindImpl(viewModel, view, vmProperty, viewProperty, signalViewUpdate, vmToViewFunc, viewToVmFunc);
        }

        delegate bool OutFunc<in T1, T2>(T1 t1, out T2 t2);

        IReactiveBinding<TView, TViewModel, Tuple<object, bool>> bindImpl<TViewModel, TView, TVMProp, TVProp, TDontCare>(
                TViewModel viewModel,
                TView view,
                Expression<Func<TViewModel, TVMProp>> vmProperty,
                Expression<Func<TView, TVProp>> viewProperty,
                IObservable<TDontCare> signalViewUpdate,
                OutFunc<TVMProp, TVProp> vmToViewConverter,
                OutFunc<TVProp, TVMProp> viewToVmConverter)
            where TViewModel : class
            where TView : IViewFor
        {
            var signalInitialUpdate = new Subject<bool>();
            var vmExpression = Reflection.Rewrite(vmProperty.Body);
            var viewExpression = Reflection.Rewrite(viewProperty.Body);

            var somethingChanged = Observable.Merge(
                Reflection.ViewModelWhenAnyValue(viewModel, view, vmExpression).Select(_ => true),
                signalInitialUpdate.Select(_ => true),
                signalViewUpdate != null ?
                    signalViewUpdate.Select(_ => false) :
                    view.WhenAnyDynamic(viewExpression, x => (TVProp)x.Value).Select(_ => false));

            var changeWithValues = somethingChanged.Select(isVm => {
                TVMProp vmValue; TVProp vValue;
                if (!Reflection.TryGetValueForPropertyChain(out vmValue, view.ViewModel, vmExpression.GetExpressionChain()) ||
                    !Reflection.TryGetValueForPropertyChain(out vValue, view, viewExpression.GetExpressionChain())) {
                    return null;
                }

                if (isVm) {
                    TVProp vmAsView;
                    if (!vmToViewConverter(vmValue, out vmAsView) || EqualityComparer<TVProp>.Default.Equals(vValue, vmAsView)) {
                        return null;
                    }

                    return Tuple.Create((object)vmAsView, isVm);
                }

                TVMProp vAsViewModel;
                if (!viewToVmConverter(vValue, out vAsViewModel) || EqualityComparer<TVMProp>.Default.Equals(vmValue, vAsViewModel)) {
                    return null;
                }

                return Tuple.Create((object)vAsViewModel, isVm);
            });

            var ret = evalBindingHooks(viewModel, view, vmExpression, viewExpression, BindingDirection.TwoWay);
            if (!ret) return null;

            IObservable<Tuple<object, bool>> changes = changeWithValues.Where(tuple => tuple != null).Publish().RefCount();

            IDisposable disp = changes.Subscribe(isVmWithLatestValue => {
                if (isVmWithLatestValue.Item2) {
                    Reflection.TrySetValueToPropertyChain(view, viewExpression.GetExpressionChain(), isVmWithLatestValue.Item1, false);
                } else {
                    Reflection.TrySetValueToPropertyChain(view.ViewModel, vmExpression.GetExpressionChain(), isVmWithLatestValue.Item1, false);
                }
            });

            // NB: Even though it's technically a two-way bind, most people 
            // want the ViewModel to win at first.
            signalInitialUpdate.OnNext(true);

            return new ReactiveBinding<TView, TViewModel, Tuple<object, bool>>(view, viewModel, viewExpression, vmExpression,
                changes, BindingDirection.TwoWay, disp);
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
        /// <typeparam name="TVProp">The type of the property bound on the view</typeparam>
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
        /// If it is left null, the framework will attempt to automagically figure out
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
            where TView : IViewFor
        {
            var vmExpression = Reflection.Rewrite(vmProperty.Body);
            var viewExpression = Reflection.Rewrite(viewProperty.Body);
            var viewType = viewExpression.Type;
            var converter = vmToViewConverterOverride ?? getConverterForTypes(typeof(TVMProp), viewType);

            if (converter == null) {
                throw new ArgumentException(String.Format("Can't convert {0} to {1}. To fix this, register a IBindingTypeConverter", typeof(TVMProp), viewType));
            }

            var ret = evalBindingHooks(viewModel, view, vmExpression, viewExpression, BindingDirection.OneWay);
            if (!ret) return null;

            var source = Reflection.ViewModelWhenAnyValue(viewModel, view, vmExpression)
                    .SelectMany(x => {
                        object tmp;
                        if (!converter.TryConvert(x, viewType, conversionHint, out tmp)) return Observable<TVProp>.Empty;
                        return Observable.Return(tmp == null ? default(TVProp) : (TVProp)tmp);
                    });

            IDisposable disp = bindToDirect(source, view, viewExpression);

            return new ReactiveBinding<TView, TViewModel, TVProp>(view, viewModel, viewExpression, vmExpression, source, BindingDirection.OneWay, disp);
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
        /// If it is left null, the framework will attempt to automagically figure out
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
            where TView : IViewFor
        {
            var vmExpression = Reflection.Rewrite(vmProperty.Body);
            var viewExpression = Reflection.Rewrite(viewProperty.Body);
            var ret = evalBindingHooks(viewModel, view, vmExpression, viewExpression, BindingDirection.OneWay);
            if (!ret) return null;

            var source = Reflection.ViewModelWhenAnyValue(viewModel, view, vmExpression).Select(x => (TProp)x).Select(selector);

            IDisposable disp = bindToDirect(source, view, viewExpression);

            return new ReactiveBinding<TView, TViewModel, TOut>(view, viewModel, viewExpression, vmExpression, source, BindingDirection.OneWay, disp);
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
        /// <param name="target">The target object whose property will be set.</param>
        /// <param name="property">
        /// An expression representing the target property to set. 
        /// This can be a child property (i.e. <c>x.Foo.Bar.Baz</c>).</param>
        /// <param name="This">The observable to apply to the target property.</param>
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
            IObservable<TValue> This,
            TTarget target,
            Expression<Func<TTarget, TTValue>> property,
            object conversionHint = null,
            IBindingTypeConverter vmToViewConverterOverride = null)
        {

            if (target == null) {
                throw new ArgumentNullException("target");
            }

            var viewExpression = Reflection.Rewrite(property.Body);

            var ret = evalBindingHooks(This, target, null, viewExpression, BindingDirection.OneWay);
            if (!ret) return Disposable.Empty;

            var converter = vmToViewConverterOverride ?? getConverterForTypes(typeof(TValue), typeof(TTValue));

            if (converter == null) {
                throw new ArgumentException(String.Format("Can't convert {0} to {1}. To fix this, register a IBindingTypeConverter", typeof(TValue), typeof(TTValue)));
            }

            var source = This.SelectMany(x => {
                object tmp;
                if (!converter.TryConvert(x, typeof(TTValue), conversionHint, out tmp)) return Observable<TTValue>.Empty;
                return Observable.Return(tmp == null ? default(TTValue) : (TTValue)tmp);
            });

            return bindToDirect(source, target, viewExpression);
        }

        IDisposable bindToDirect<TTarget, TValue>(
            IObservable<TValue> This,
            TTarget target,
            Expression viewExpression)
        {
            var setter = Reflection.GetValueSetterOrThrow(viewExpression.GetMemberInfo());
            if (viewExpression.GetParent().NodeType == ExpressionType.Parameter) {
                return This.Subscribe(
                    x => setter(target, x, viewExpression.GetArgumentsArray()),
                    ex => {
                        this.Log().ErrorException(String.Format("{0} Binding received an Exception!", viewExpression), ex);
                    });
            }

            var bindInfo = Observable.CombineLatest(
                This, target.WhenAnyDynamic(viewExpression.GetParent(), x => x.Value),
                (val, host) => new { val, host });

            return bindInfo
                .Where(x => x.host != null)
                .Subscribe(
                    x => setter(x.host, x.val, viewExpression.GetArgumentsArray()),
                    ex => {
                        this.Log().ErrorException(String.Format("{0} Binding received an Exception!", viewExpression), ex);
                    });
        }

        bool evalBindingHooks<TViewModel, TView>(TViewModel viewModel, TView view, Expression vmExpression, Expression viewExpression, BindingDirection direction)
            where TViewModel : class
        {
            var hooks = Locator.Current.GetServices<IPropertyBindingHook>();

            var vmFetcher = default(Func<IObservedChange<object, object>[]>);
            if (vmExpression != null) {
                vmFetcher = () => {
                    IObservedChange<object, object>[] fetchedValues;
                    Reflection.TryGetAllValuesForPropertyChain(out fetchedValues, viewModel, vmExpression.GetExpressionChain());
                    return fetchedValues;
                };
            } else {
                vmFetcher = () => {
                    return new[] {
                        new ObservedChange<object, object>(null, null, viewModel)
                    };
                };
            }

            var vFetcher = new Func<IObservedChange<object, object>[]>(() => {
                IObservedChange<object, object>[] fetchedValues;
                Reflection.TryGetAllValuesForPropertyChain(out fetchedValues, view, viewExpression.GetExpressionChain());
                return fetchedValues;
            });

            var shouldBind = hooks.Aggregate(true, (acc, x) =>
                acc && x.ExecuteHook(viewModel, view, vmFetcher, vFetcher, direction));

            if (!shouldBind) {
                var vmString = String.Format("{0}.{1}", typeof(TViewModel).Name, String.Join(".", vmExpression));
                var vString = String.Format("{0}.{1}", typeof(TView).Name, String.Join(".", viewExpression));
                this.Log().Warn("Binding hook asked to disable binding {0} => {1}", vmString, vString);
            }

            return shouldBind;
        }

        MemoizingMRUCache<Tuple<Type, Type>, IBindingTypeConverter> typeConverterCache = new MemoizingMRUCache<Tuple<Type, Type>, IBindingTypeConverter>(
            (types, _) => {
                return Locator.Current.GetServices<IBindingTypeConverter>()
                    .Aggregate(Tuple.Create(-1, default(IBindingTypeConverter)), (acc, x) => {
                        var score = x.GetAffinityForObjects(types.Item1, types.Item2);
                        return score > acc.Item1 && score > 0 ?
                            Tuple.Create(score, x) : acc;
                    }).Item2;
            }, RxApp.SmallCacheLimit);

        internal IBindingTypeConverter getConverterForTypes(Type lhs, Type rhs)
        {
            lock (typeConverterCache) {
                return typeConverterCache.Get(Tuple.Create(lhs, rhs));
            }
        }
    }
}
