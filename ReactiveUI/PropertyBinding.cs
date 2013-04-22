using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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
        /// <returns>
        /// An instance of <see cref="IDisposable"/> that, when disposed,
        /// disconnects the binding.
        /// </returns>
        public static IReactiveBinding<TView, TViewModel> Bind<TViewModel, TView, TVMProp, TVProp>(
                this TView view,
                TViewModel viewModel,
                Expression<Func<TViewModel, TVMProp>> vmProperty,
                Expression<Func<TView, TVProp>> viewProperty)
            where TViewModel : class
            where TView : IViewFor
        {
            return binderImplementation.Bind(viewModel, view, vmProperty, viewProperty, (IObservable<Unit>)null, null);
        }


        /// <summary>
        /// Binds the specified view model property to the given view,
        /// and tries to automagically guess the control/property to be bound on the
        /// view by looking at the name of the property bound on the view model.
        /// </summary>
        /// <typeparam name="TViewModel">The type of the view model being bound.</typeparam>
        /// <typeparam name="TView">The type of the view being bound.</typeparam>
        /// <typeparam name="TProp">The type of the property bound on the view model.</typeparam>
        /// <param name="view">The instance of the view to bind.</param>
        /// <param name="viewModel">The instance of the view model to bind.</param>
        /// <param name="vmProperty">
        /// An expression indicating the property that is bound on the view model.
        /// This can be a chain of properties of the form <code>vm =&gt; vm.Foo.Bar.Baz</code>
        /// and the binder will attempt to subscribe to changes on each recursively.
        /// </param>
        /// <returns>
        /// An instance of <see cref="IDisposable"/> that, when disposed,
        /// disconnects the binding.
        /// </returns>
        public static IReactiveBinding<TView, TViewModel> Bind<TViewModel, TView, TProp>(
                this TView view,
                TViewModel viewModel,
                Expression<Func<TViewModel, TProp>> vmProperty)
            where TViewModel : class
            where TView : IViewFor
        {
            return binderImplementation.Bind<TViewModel, TView, TProp, TProp, Unit>(viewModel, view, vmProperty, null, null, null);
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
        /// <returns>
        /// An instance of <see cref="IDisposable"/> that, when disposed,
        /// disconnects the binding.
        /// </returns>
        public static IReactiveBinding<TView, TViewModel> Bind<TViewModel, TView, TVMProp, TVProp, TDontCare>(
                this TView view,
                TViewModel viewModel,
                Expression<Func<TViewModel, TVMProp>> vmProperty,
                Expression<Func<TView, TVProp>> viewProperty,
                IObservable<TDontCare> signalViewUpdate)
            where TViewModel : class
            where TView : IViewFor
        {
            return binderImplementation.Bind(viewModel, view, vmProperty, viewProperty, signalViewUpdate, null);
        }

        /// <summary>
        /// Binds the specified view model property to an automagically guessed control/property on the view, and 
        /// provide a custom view update signaller to signal when the view property has been updated.
        /// </summary>
        /// <typeparam name="TViewModel">The type of the view model being bound.</typeparam>
        /// <typeparam name="TView">The type of the view being bound.</typeparam>
        /// <param name="view">The instance of the view to bind.</param>
        /// <typeparam name="TProp">The type of the property bound on the view model.</typeparam>
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
        /// <param name="signalViewUpdate">
        /// An observable, that when signaled, indicates that the view property 
        /// has been changed, and that the binding should update the view model
        /// property accordingly.
        /// </param>
        /// <returns>
        /// An instance of <see cref="IDisposable"/> that, when disposed,
        /// disconnects the binding.
        /// </returns>
        public static IReactiveBinding<TView, TViewModel> Bind<TViewModel, TView, TProp, TDontCare>(
                this TView view,
                TViewModel viewModel,
                Expression<Func<TViewModel, TProp>> vmProperty,
                IObservable<TDontCare> signalViewUpdate)
            where TViewModel : class
            where TView : IViewFor
        {
            return binderImplementation.Bind<TViewModel, TView, TProp, TDontCare, TDontCare>(viewModel, view, vmProperty, null, signalViewUpdate, null);
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
        /// <param name="fallbackValue">
        /// A function providing a fallback value. 
        /// The parameter is currently IGNORED.
        /// </param>
        /// <param name="conversionHint">
        /// An object that can provide a hint for the converter.
        /// The semantics of this object is defined by the converter used.
        /// This parameter is currently IGNORED.
        /// </param>
        /// <returns>
        /// An instance of <see cref="IDisposable"/> that, when disposed,
        /// disconnects the binding.
        /// </returns>
        public static IReactiveBinding<TView, TViewModel> OneWayBind<TViewModel, TView, TVMProp, TVProp>(
                this TView view,
                TViewModel viewModel,
                Expression<Func<TViewModel, TVMProp>> vmProperty,
                Expression<Func<TView, TVProp>> viewProperty,
                Func<TVMProp> fallbackValue = null,
                object conversionHint = null)
            where TViewModel : class
            where TView : IViewFor
        {
            return binderImplementation.OneWayBind(viewModel, view, vmProperty, viewProperty, fallbackValue);
        }

        /// <summary>
        /// Binds the specified view model property property to the given view in a one-way (view model to view) fashion,
        /// and tries to automagically guess the control/property to be bound on the
        /// view by looking at the name of the property bound on the view model.
        /// </summary>
        /// <typeparam name="TViewModel">The type of the view model being bound.</typeparam>
        /// <typeparam name="TView">The type of the view being bound.</typeparam>
        /// <typeparam name="TProp">The type of the property bound on the view model.</typeparam>
        /// <param name="view">The instance of the view to bind.</param>
        /// <param name="viewModel">The instance of the view model to bind.</param>
        /// <param name="vmProperty">
        /// An expression indicating the property that is bound on the view model.
        /// This can be a chain of properties of the form <code>vm =&gt; vm.Foo.Bar.Baz</code>
        /// and the binder will attempt to subscribe to changes on each recursively.
        /// </param>
        /// <param name="fallbackValue">
        /// A function providing a fallback value. 
        /// The parameter is currently IGNORED.
        /// </param>
        /// <param name="conversionHint">
        /// An object that can provide a hint for the converter.
        /// The semantics of this object is defined by the converter used.
        /// This parameter is currently IGNORED.
        /// </param>
        /// <returns>
        /// An instance of <see cref="IDisposable"/> that, when disposed,
        /// disconnects the binding.
        /// </returns>
        public static IReactiveBinding<TView, TViewModel> OneWayBind<TViewModel, TView, TProp>(
                this TView view,
                TViewModel viewModel,
                Expression<Func<TViewModel, TProp>> vmProperty,
                Func<TProp> fallbackValue = null,
                object conversionHint = null)
            where TViewModel : class
            where TView : IViewFor
        {
            return binderImplementation.OneWayBind<TViewModel, TView, TProp, Unit>(viewModel, view, vmProperty, null, fallbackValue, null);
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
        /// <param name="fallbackValue">
        /// A function that provides a fallback value. Note that this property is IGNORED in this implementation.
        /// </param>
        /// <returns>
        /// An instance of <see cref="IDisposable"/> that, when disposed,
        /// disconnects the binding.
        /// </returns>
        public static IReactiveBinding<TView, TViewModel> OneWayBind<TViewModel, TView, TProp, TOut>(
                this TView view,
                TViewModel viewModel,
                Expression<Func<TViewModel, TProp>> vmProperty,
                Expression<Func<TView, TOut>> viewProperty,
                Func<TProp, TOut> selector,
                Func<TOut> fallbackValue = null)
            where TViewModel : class
            where TView : IViewFor
        {
            return binderImplementation.OneWayBind(viewModel, view, vmProperty, viewProperty, selector, fallbackValue);
        }

        /// <summary>
        /// Binds the specified view model property to the given view, automagically guessing
        /// the control/property to be bound, in a one-way (view model to view) fashion,
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
        /// <param name="selector">
        /// A function that will be used to transform the values of the property on the view model
        /// before being bound to the view property.
        /// </param>
        /// <param name="fallbackValue">
        /// A function that provides a fallback value. Note that this property is IGNORED in this implementation.
        /// </param>
        /// <returns>
        /// An instance of <see cref="IDisposable"/> that, when disposed,
        /// disconnects the binding.
        /// </returns>
        public static IReactiveBinding<TView, TViewModel> OneWayBind<TViewModel, TView, TProp, TOut>(
                this TView view,
                TViewModel viewModel,
                Expression<Func<TViewModel, TProp>> vmProperty,
                Func<TProp, TOut> selector,
                Func<TOut> fallbackValue = null)
            where TViewModel : class
            where TView : IViewFor
        {
            return binderImplementation.OneWayBind(viewModel, view, vmProperty, null, selector, fallbackValue);
        }

        /// <summary>
        /// Binds the specified view model property to the given view property in an asynchronous fashion.
        /// </summary>
        /// <typeparam name="TViewModel">The type of the view model that is bound.</typeparam>
        /// <typeparam name="TView">The type of the view that is bound.</typeparam>
        /// <typeparam name="TProp">The type of the property bound on the view model.</typeparam>
        /// <typeparam name="TOut">
        /// The return type of the <paramref name="selector"/>, 
        /// when considered synchronously.
        /// </typeparam>
        /// <param name="viewModel">The instance of the view model to bind to.</param>
        /// <param name="view">The instance of the view to bind to.</param>>
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
        /// same name as the <see cref="vmProperty"/>, and its most natural property.
        /// </param>
        /// <param name="selector">
        /// A function that maps values of the view model properties to
        /// an observable representing values of the view property.
        /// The view property will be updated for every value emitted by
        /// the returned observable as long as it corresponds to the current
        /// value of the view model property.
        /// </param>
        /// <param name="fallbackValue">
        /// A function that provides a fallback value. Note that this property is IGNORED in this implementation.
        /// </param>>
        /// <returns>
        /// An instance of <see cref="IDisposable"/> that, when disposed,
        /// disconnects the binding.
        /// </returns>
        public static IReactiveBinding<TView, TViewModel> AsyncOneWayBind<TViewModel, TView, TProp, TOut>(
                this TView view,
                TViewModel viewModel,
                Expression<Func<TViewModel, TProp>> vmProperty,
                Expression<Func<TView, TOut>> viewProperty,
                Func<TProp, IObservable<TOut>> selector,
                Func<TOut> fallbackValue = null)
            where TViewModel : class
            where TView : IViewFor
        {
            return binderImplementation.AsyncOneWayBind(viewModel, view, vmProperty, viewProperty, selector, fallbackValue);
        }

        /// <summary>
        /// Binds the specified view model property to the given view property in an asynchronous fashion.
        /// </summary>
        /// <typeparam name="TViewModel">The type of the view model that is bound.</typeparam>
        /// <typeparam name="TView">The type of the view that is bound.</typeparam>
        /// <typeparam name="TProp">The type of the property bound on the view model.</typeparam>
        /// <typeparam name="TOut">
        /// The return type of the <paramref name="selector"/>, 
        /// when considered synchronously.
        /// </typeparam>
        /// <param name="viewModel">The instance of the view model to bind to.</param>
        /// <param name="view">The instance of the view to bind to.</param>>
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
        /// same name as the <see cref="vmProperty"/>, and its most natural property.
        /// </param>
        /// <param name="selector">
        /// A function that maps values of the view model properties to
        /// a task. The view property is updated when the task completes.
        /// </param>
        /// <param name="fallbackValue">
        /// A function that provides a fallback value. Note that this property is IGNORED in this implementation.
        /// </param>>
        /// <returns>
        /// An instance of <see cref="IDisposable"/> that, when disposed,
        /// disconnects the binding.
        /// </returns>
        public static IReactiveBinding<TView, TViewModel> AsyncOneWayBind<TViewModel, TView, TProp, TOut>(
                this TView view,
                TViewModel viewModel,
                Expression<Func<TViewModel, TProp>> vmProperty,
                Expression<Func<TView, TOut>> viewProperty,
                Func<TProp, Task<TOut>> selector,
                Func<TOut> fallbackValue = null)
            where TViewModel : class
            where TView : IViewFor
        {
            return binderImplementation.AsyncOneWayBind(viewModel, view, vmProperty, viewProperty, x => selector(x).ToObservable(), fallbackValue);
        }

        /// <summary>
        /// Binds the specified view model property to an automagically guessed control/property on the view in an asynchronous fashion.
        /// </summary>
        /// <typeparam name="TViewModel">The type of the view model that is bound.</typeparam>
        /// <typeparam name="TView">The type of the view that is bound.</typeparam>
        /// <typeparam name="TProp">The type of the property bound on the view model.</typeparam>
        /// <typeparam name="TOut">
        /// The return type of the <paramref name="selector"/>, 
        /// when considered synchronously.
        /// </typeparam>
        /// <param name="viewModel">The instance of the view model to bind to.</param>
        /// <param name="view">The instance of the view to bind to.</param>>
        /// <param name="vmProperty">
        /// An expression representing the property to be bound to on the view model.
        /// This can be a child property, for example <c>x =&gt; x.Foo.Bar.Baz</c> in which case
        /// the binding will attempt to subscribe recursively to updates in order to
        /// always get the last value of the property chain.
        /// </param>
        /// <param name="selector">
        /// A function that maps values of the view model properties to
        /// an observable representing values of the view property.
        /// The view property will be updated for every value emitted by
        /// the returned observable as long as it corresponds to the current
        /// value of the view model property.
        /// </param>
        /// <param name="fallbackValue">
        /// A function that provides a fallback value. Note that this property is IGNORED in this implementation.
        /// </param>>
        /// <returns>
        /// An instance of <see cref="IDisposable"/> that, when disposed,
        /// disconnects the binding.
        /// </returns>
        public static IReactiveBinding<TView, TViewModel> AsyncOneWayBind<TViewModel, TView, TProp, TOut>(
                this TView view,
                TViewModel viewModel,
                Expression<Func<TViewModel, TProp>> vmProperty,
                Func<TProp, IObservable<TOut>> selector,
                Func<TOut> fallbackValue = null)
            where TViewModel : class
            where TView : IViewFor
        {
            return binderImplementation.AsyncOneWayBind(viewModel, view, vmProperty, null, selector, fallbackValue);
        }

        /// <summary>
        /// Binds the specified view model property to an automagically guessed control/property on the view in an asynchronous fashion.
        /// </summary>
        /// <typeparam name="TViewModel">The type of the view model that is bound.</typeparam>
        /// <typeparam name="TView">The type of the view that is bound.</typeparam>
        /// <typeparam name="TProp">The type of the property bound on the view model.</typeparam>
        /// <typeparam name="TOut">
        /// The return type of the <paramref name="selector"/>, 
        /// when considered synchronously.
        /// </typeparam>
        /// <param name="viewModel">The instance of the view model to bind to.</param>
        /// <param name="view">The instance of the view to bind to.</param>>
        /// <param name="vmProperty">
        /// An expression representing the property to be bound to on the view model.
        /// This can be a child property, for example <c>x =&gt; x.Foo.Bar.Baz</c> in which case
        /// the binding will attempt to subscribe recursively to updates in order to
        /// always get the last value of the property chain.
        /// </param>
        /// <param name="selector">
        /// A function that maps values of the view model properties to
        /// a task. The view property is updated when the task completes.
        /// </param>
        /// <param name="fallbackValue">
        /// A function that provides a fallback value. Note that this property is IGNORED in this implementation.
        /// </param>>
        /// <returns>
        /// An instance of <see cref="IDisposable"/> that, when disposed,
        /// disconnects the binding.
        /// </returns>
        public static IReactiveBinding<TView, TViewModel> AsyncOneWayBind<TViewModel, TView, TProp, TOut>(
                this TView view,
                TViewModel viewModel,
                Expression<Func<TViewModel, TProp>> vmProperty,
                Func<TProp, Task<TOut>> selector,
                Func<TOut> fallbackValue = null)
            where TViewModel : class
            where TView : IViewFor
        {
            return binderImplementation.AsyncOneWayBind(viewModel, view, vmProperty, null, x => selector(x).ToObservable(), fallbackValue);
        }

        /// <summary>
        /// BindTo takes an Observable stream and applies it to a target
        /// property. Conceptually it is similar to "Subscribe(x =&gt;
        /// target.property = x)", but allows you to use child properties
        /// without the null checks.
        /// </summary>
        /// <param name="target">The target object whose property will be set.</param>
        /// <param name="property">An expression representing the target
        /// property to set. This can be a child property (i.e. x.Foo.Bar.Baz).</param>
        /// <returns>An object that when disposed, disconnects the binding.</returns>
        public static IDisposable BindTo<TValue, TTarget, TTValue>(
            this IObservable<TValue> This,
            TTarget target,
            Expression<Func<TTarget, TTValue>> property,
            Func<TValue> fallbackValue = null,
            object conversionHint = null)
        {
            return binderImplementation.BindTo(This, target, property, fallbackValue);
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
        /// <returns>
        /// An instance of <see cref="IDisposable"/> that, when disposed,
        /// disconnects the binding.
        /// </returns>
        IReactiveBinding<TView, TViewModel> Bind<TViewModel, TView, TVMProp, TVProp, TDontCare>(
                TViewModel viewModel,
                TView view,
                Expression<Func<TViewModel, TVMProp>> vmProperty,
                Expression<Func<TView, TVProp>> viewProperty,
                IObservable<TDontCare> signalViewUpdate,
                object conversionHint)
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
        /// <param name="fallbackValue">
        /// A function that provides a fallback value. Note that this property is IGNORED in this implementation.
        /// </param>
        /// <param name="conversionHint">
        /// An object that can provide a hint for the converter.
        /// The semantics of this object is defined by the converter used.
        /// </param>
        /// <returns>
        /// An instance of <see cref="IDisposable"/> that, when disposed,
        /// disconnects the binding.
        /// </returns>
        /// <exception cref="System.ArgumentException">
        /// There is no registered converter from <typeparamref name="TVMProp"/> to <typeparamref name="TVProp"/>.
        /// </exception>
        IReactiveBinding<TView, TViewModel> OneWayBind<TViewModel, TView, TVMProp, TVProp>(
                TViewModel viewModel,
                TView view,
                Expression<Func<TViewModel, TVMProp>> vmProperty,
                Expression<Func<TView, TVProp>> viewProperty,
                Func<TVMProp> fallbackValue = null,
                object conversionHint = null)
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
        /// <param name="fallbackValue">
        /// A function that provides a fallback value. Note that this property is IGNORED in this implementation.
        /// </param>
        /// <returns>
        /// An instance of <see cref="IDisposable"/> that, when disposed,
        /// disconnects the binding.
        /// </returns>
        IReactiveBinding<TView, TViewModel> OneWayBind<TViewModel, TView, TProp, TOut>(
                TViewModel viewModel,
                TView view,
                Expression<Func<TViewModel, TProp>> vmProperty,
                Expression<Func<TView, TOut>> viewProperty,
                Func<TProp, TOut> selector,
                Func<TOut> fallbackValue = null)
            where TViewModel : class
            where TView : IViewFor;

        /// <summary>
        /// Creates a one way binding with a selector, i.e. a binding that flows from the
        /// <paramref name="viewModel"/> to the <paramref name="view"/> only, and where the value of the view model
        /// property is mapped through the <paramref name="selector"/> asynchronously before being set to the view.
        /// </summary>
        /// <typeparam name="TViewModel">The type of the view model that is bound.</typeparam>
        /// <typeparam name="TView">The type of the view that is bound.</typeparam>
        /// <typeparam name="TProp">The type of the property bound on the view model.</typeparam>
        /// <typeparam name="TOut">
        /// The return type of the <paramref name="selector"/>, 
        /// when considered synchronously.
        /// </typeparam>
        /// <param name="viewModel">The instance of the view model to bind to.</param>
        /// <param name="view">The instance of the view to bind to.</param>>
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
        /// A function that maps values of the view model properties to
        /// an observable representing values of the view property.
        /// The view property will be updated for every value emitted by
        /// the returned observable as long as it corresponds to the current
        /// value of the view model property.
        /// </param>
        /// <param name="fallbackValue">
        /// A function that provides a fallback value. Note that this property is IGNORED in this implementation.
        /// </param>>
        /// <returns>
        /// An instance of <see cref="IDisposable"/> that, when disposed,
        /// disconnects the binding.
        /// </returns>
        IReactiveBinding<TView, TViewModel> AsyncOneWayBind<TViewModel, TView, TProp, TOut>(
                TViewModel viewModel,
                TView view,
                Expression<Func<TViewModel, TProp>> vmProperty,
                Expression<Func<TView, TOut>> viewProperty,
                Func<TProp, IObservable<TOut>> selector,
                Func<TOut> fallbackValue = null)
            where TViewModel : class
            where TView : IViewFor;

        /// <summary>
        /// BindTo takes an Observable stream and applies it to a target
        /// property. Conceptually it is similar to "Subscribe(x =&gt;
        /// target.property = x)", but allows you to use child properties
        /// without the null checks.
        /// </summary>
        /// <param name="target">The target object whose property will be set.</param>
        /// <param name="property">An expression representing the target
        /// property to set. This can be a child property (i.e. x.Foo.Bar.Baz).</param>
        /// <returns>An object that when disposed, disconnects the binding.</returns>
        IDisposable BindTo<TValue, TTarget, TTValue>(
            IObservable<TValue> This,
            TTarget target,
            Expression<Func<TTarget, TTValue>> property,
            Func<TValue> fallbackValue = null,
            object conversionHint = null);
    }

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
        /// same name as the <see cref="vmProperty"/>, and its most natural property.
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
        /// <returns>
        /// An instance of <see cref="IDisposable"/> that, when disposed,
        /// disconnects the binding.
        /// </returns
        public IReactiveBinding<TView, TViewModel> Bind<TViewModel, TView, TVMProp, TVProp, TDontCare>(
                TViewModel viewModel,
                TView view,
                Expression<Func<TViewModel, TVMProp>> vmProperty,
                Expression<Func<TView, TVProp>> viewProperty,
                IObservable<TDontCare> signalViewUpdate,
                object conversionHint)
            where TViewModel : class
            where TView : IViewFor
        {
            var signalInitialUpdate = new Subject<bool>();
            var vmPropChain = Reflection.ExpressionToPropertyNames(vmProperty);
            string[] viewPropChain;

            if (viewProperty == null) {
                // NB: In this case, TVProp is possibly wrong due to type 
                // conversion. Figure out if this is the case, then re-call Bind
                // with the right TVProp
                viewPropChain = Reflection.getDefaultViewPropChain(view, vmPropChain);
                var tvProp = Reflection.GetTypesForPropChain(typeof (TView), viewPropChain).Last();
                if (tvProp != typeof (TVProp)) {
                    var mi = this.GetType().GetMethod("Bind").MakeGenericMethod(typeof (TViewModel), typeof (TView), typeof (TVMProp), tvProp, typeof (TDontCare));
                    return (IReactiveBinding<TView, TViewModel>)mi.Invoke(this, new[] { viewModel, view, vmProperty, null, signalViewUpdate, conversionHint });
                }
            } else {
                viewPropChain = Reflection.ExpressionToPropertyNames(viewProperty);
            }

            var vmToViewConverter = getConverterForTypes(typeof (TVMProp), typeof (TVProp));
            var viewToVMConverter = getConverterForTypes(typeof (TVProp), typeof (TVMProp));

            if (vmToViewConverter == null || viewToVMConverter == null) {
                throw new ArgumentException(
                    String.Format("Can't two-way convert between {0} and {1}. To fix this, register a IBindingTypeConverter", typeof (TVMProp), typeof(TVProp)));
            }

            var somethingChanged = Observable.Merge(
                Reflection.ViewModelWhenAnyValue(viewModel, view, vmProperty).Select(_ => true),
                signalInitialUpdate,
                signalViewUpdate != null ? 
                    signalViewUpdate.Select(_ => false) : 
                    view.WhenAnyDynamic(viewPropChain, x => (TVProp) x.Value).Select(_ => false));

            var vmString = String.Format("{0}.{1}", typeof (TViewModel).Name, String.Join(".", vmPropChain));
            var vString = String.Format("{0}.{1}", typeof (TView).Name, String.Join(".", viewPropChain));
            var vmChangedString = String.Format("Setting {0} => {1}", vmString, vString);
            var viewChangedString = String.Format("Setting {0} => {1}", vString, vmString);

            var changeWithValues = somethingChanged.Select(isVm => {
                TVMProp vmValue; TVProp vValue;
                if (!Reflection.TryGetValueForPropertyChain(out vmValue, view.ViewModel, vmPropChain) ||
                    !Reflection.TryGetValueForPropertyChain(out vValue, view, viewPropChain)) {
                    return null;
                }

                if (isVm) {
                    object tmp;
                    if (!vmToViewConverter.TryConvert(vmValue, typeof (TVProp), conversionHint, out tmp)) {
                        return null;
                    }

                    var vmAsView = (tmp == null ? default(TVProp) : (TVProp) tmp);
                    var changed = EqualityComparer<TVProp>.Default.Equals(vValue, vmAsView) != true;
                    if (!changed) return null;

                    this.Log().Info(vmChangedString + (vmAsView != null ? vmAsView.ToString() : "(null)"));
                    return Tuple.Create((object)vmAsView, isVm);
                } else {
                    object tmp;
                    if (!viewToVMConverter.TryConvert(vValue, typeof (TVMProp), conversionHint, out tmp)) {
                        return null;
                    }

                    var vAsViewModel = (tmp == null ? default(TVMProp) : (TVMProp) tmp);
                    var changed = EqualityComparer<TVMProp>.Default.Equals(vmValue, vAsViewModel) != true;
                    if (!changed) return null;

                    this.Log().Info(viewChangedString + (vAsViewModel != null ? vAsViewModel.ToString() : "(null)"));
                    return Tuple.Create((object)vAsViewModel, isVm);
                }
            });

            var ret = evalBindingHooks(viewModel, view, vmPropChain, viewPropChain, BindingDirection.TwoWay);
            //TODO: return something other than null
            if (!ret) return null;

            IDisposable disp = changeWithValues.Subscribe(isVmWithLatestValue => {
                if (isVmWithLatestValue == null) return;

                if (isVmWithLatestValue.Item2) {
                    Reflection.SetValueToPropertyChain(view, viewPropChain, isVmWithLatestValue.Item1, false);
                } else {
                    Reflection.SetValueToPropertyChain(view.ViewModel, vmPropChain, isVmWithLatestValue.Item1, false);
                }
            });

            // NB: Even though it's technically a two-way bind, most people 
            // want the ViewModel to win at first.
            signalInitialUpdate.OnNext(true);

            return new ReactiveBinding<TView, TViewModel>(view, viewModel, viewPropChain, vmPropChain, BindingDirection.TwoWay, disp);
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
        /// same name as the <see cref="vmProperty"/>, and its most natural property.
        /// </param>
        /// <param name="fallbackValue">
        /// A function that provides a fallback value. Note that this property is IGNORED in this implementation.
        /// </param>
        /// <param name="conversionHint">
        /// An object that can provide a hint for the converter.
        /// The semantics of this object is defined by the converter used.
        /// </param>
        /// <returns>
        /// An instance of <see cref="IDisposable"/> that, when disposed,
        /// disconnects the binding.
        /// </returns>
        /// <exception cref="System.ArgumentException">
        /// There is no registered converter from <typeparamref name="TVMProp"/> to <typeparamref name="TVProp"/>.
        /// </exception>
        public IReactiveBinding<TView, TViewModel> OneWayBind<TViewModel, TView, TVMProp, TVProp>(
                TViewModel viewModel,
                TView view,
                Expression<Func<TViewModel, TVMProp>> vmProperty,
                Expression<Func<TView, TVProp>> viewProperty,
                Func<TVMProp> fallbackValue = null,
                object conversionHint = null)
            where TViewModel : class
            where TView : IViewFor
        {
            var vmPropChain = Reflection.ExpressionToPropertyNames(vmProperty);
            var viewPropChain = default(string[]);
            var vmString = String.Format("{0}.{1}", typeof (TViewModel).Name, String.Join(".", vmPropChain));
            var source = default(IObservable<TVProp>);
            var fallbackWrapper = default(Func<TVProp>);

            if (viewProperty == null) {
                viewPropChain = Reflection.getDefaultViewPropChain(view, Reflection.ExpressionToPropertyNames(vmProperty));

                var viewType = Reflection.GetTypesForPropChain(typeof (TView), viewPropChain).Last();
                var converter = getConverterForTypes(typeof (TVMProp), viewType);

                if (converter == null) {
                    throw new ArgumentException(String.Format("Can't convert {0} to {1}. To fix this, register a IBindingTypeConverter", typeof (TVMProp), viewType));
                }

                var ret = evalBindingHooks(viewModel, view, vmPropChain, viewPropChain, BindingDirection.OneWay);
                //TODO: return something other than null
                if (!ret) return null;

                source = Reflection.ViewModelWhenAnyValue(viewModel, view, vmProperty)
                    .SelectMany(x => {
                        object tmp;
                        if (!converter.TryConvert(x, viewType, conversionHint, out tmp)) return Observable.Empty<TVProp>();
                        return Observable.Return((TVProp)tmp);
                    });

                fallbackWrapper = () => {
                    object tmp;
                    return converter.TryConvert(fallbackValue(), typeof(TVProp), conversionHint, out tmp) ? (TVProp)tmp : default(TVProp);
                };
            } else {
                var converter = getConverterForTypes(typeof (TVMProp), typeof (TVProp));

                if (converter == null) {
                    throw new ArgumentException(String.Format("Can't convert {0} to {1}. To fix this, register a IBindingTypeConverter", typeof (TVMProp), typeof(TVProp)));
                }

                viewPropChain = Reflection.ExpressionToPropertyNames(viewProperty);

                var ret = evalBindingHooks(viewModel, view, vmPropChain, viewPropChain, BindingDirection.OneWay);
                //TODO: return something other than null
                if (!ret) return null;

                source = Reflection.ViewModelWhenAnyValue(viewModel, view, vmProperty)
                    .SelectMany(x => {
                        object tmp;
                        if (!converter.TryConvert(x, typeof(TVProp), conversionHint, out tmp)) return Observable.Empty<TVProp>();
                        return Observable.Return(tmp == null ? default(TVProp) : (TVProp)tmp);
                    });

                fallbackWrapper = () => {
                    object tmp;
                    return converter.TryConvert(fallbackValue(), typeof(TVProp), conversionHint, out tmp) ?  (TVProp)tmp : default(TVProp);
                };
            }

            IDisposable disp = bindToDirect(source, view, viewProperty, fallbackWrapper);

            return new ReactiveBinding<TView, TViewModel>(view, viewModel, viewPropChain, vmPropChain, BindingDirection.OneWay, disp);
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
        /// same name as the <see cref="vmProperty"/>, and its most natural property.
        /// </param>
        /// <param name="selector">
        /// A function that will be used to transform the values of the property on the view model
        /// before being bound to the view property.
        /// </param>
        /// <param name="fallbackValue">
        /// A function that provides a fallback value. Note that this property is IGNORED in this implementation.
        /// </param>
        /// <returns>
        /// An instance of <see cref="IDisposable"/> that, when disposed,
        /// disconnects the binding.
        /// </returns>
        public IReactiveBinding<TView, TViewModel> OneWayBind<TViewModel, TView, TProp, TOut>(
                TViewModel viewModel,
                TView view,
                Expression<Func<TViewModel, TProp>> vmProperty,
                Expression<Func<TView, TOut>> viewProperty,
                Func<TProp, TOut> selector,
                Func<TOut> fallbackValue = null)
            where TViewModel : class
            where TView : IViewFor
        {
            var vmPropChain = Reflection.ExpressionToPropertyNames(vmProperty);
            var viewPropChain = default(string[]);
            var vmString = String.Format("{0}.{1}", typeof (TViewModel).Name, String.Join(".", vmPropChain));
            var source = default(IObservable<TOut>);

            if (viewProperty == null) {
                viewPropChain = Reflection.getDefaultViewPropChain(view, Reflection.ExpressionToPropertyNames(vmProperty));

                var ret = evalBindingHooks(viewModel, view, vmPropChain, viewPropChain, BindingDirection.OneWay);
                //TODO: return something other than null
                if (!ret) return null;

                source = Reflection.ViewModelWhenAnyValue(viewModel, view, vmProperty).Select(selector);
            } else {
                viewPropChain = Reflection.ExpressionToPropertyNames(viewProperty);
                var ret = evalBindingHooks(viewModel, view, vmPropChain, viewPropChain, BindingDirection.OneWay);
                //TODO: return something other than null
                if (!ret) return null;

                source = Reflection.ViewModelWhenAnyValue(viewModel, view, vmProperty).Select(selector);
            }

            IDisposable disp =  bindToDirect(source, view, viewProperty, fallbackValue);

            return new ReactiveBinding<TView, TViewModel>(view, viewModel, viewPropChain, vmPropChain, BindingDirection.OneWay, disp);
        }

        /// <summary>
        /// Creates a one way binding with a selector, i.e. a binding that flows from the
        /// <paramref name="viewModel"/> to the <paramref name="view"/> only, and where the value of the view model
        /// property is mapped through the <paramref name="selector"/> asynchronously before being set to the view.
        /// </summary>
        /// <typeparam name="TViewModel">The type of the view model that is bound.</typeparam>
        /// <typeparam name="TView">The type of the view that is bound.</typeparam>
        /// <typeparam name="TProp">The type of the property bound on the view model.</typeparam>
        /// <typeparam name="TOut">
        /// The return type of the <paramref name="selector"/>, 
        /// when considered synchronously.
        /// </typeparam>
        /// <param name="viewModel">The instance of the view model to bind to.</param>
        /// <param name="view">The instance of the view to bind to.</param>>
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
        /// same name as the <see cref="vmProperty"/>, and its most natural property.
        /// </param>
        /// <param name="selector">
        /// A function that maps values of the view model properties to
        /// an observable representing values of the view property.
        /// The view property will be updated for every value emitted by
        /// the returned observable as long as it corresponds to the current
        /// value of the view model property.
        /// </param>
        /// <param name="fallbackValue">
        /// A function that provides a fallback value. Note that this property is IGNORED in this implementation.
        /// </param>>
        /// <returns>
        /// An instance of <see cref="IDisposable"/> that, when disposed,
        /// disconnects the binding.
        /// </returns>
        public IReactiveBinding<TView, TViewModel> AsyncOneWayBind<TViewModel, TView, TProp, TOut>(
                TViewModel viewModel,
                TView view,
                Expression<Func<TViewModel, TProp>> vmProperty,
                Expression<Func<TView, TOut>> viewProperty,
                Func<TProp, IObservable<TOut>> selector,
                Func<TOut> fallbackValue = null)
            where TViewModel : class
            where TView : IViewFor
        {
            var vmPropChain = Reflection.ExpressionToPropertyNames(vmProperty);
            var viewPropChain = default(string[]);
            var vmString = String.Format("{0}.{1}", typeof (TViewModel).Name, String.Join(".", vmPropChain));
            var source = default(IObservable<TOut>);

            if (viewProperty == null) {
                viewPropChain = Reflection.getDefaultViewPropChain(view, Reflection.ExpressionToPropertyNames(vmProperty));

                var ret = evalBindingHooks(viewModel, view, vmPropChain, viewPropChain, BindingDirection.AsyncOneWay);
                //TODO: return something other than null
                if (!ret) return null;

                source = Reflection.ViewModelWhenAnyValue(viewModel, view, vmProperty).SelectMany(selector);
            } else {
                viewPropChain = Reflection.ExpressionToPropertyNames(viewProperty);

                var ret = evalBindingHooks(viewModel, view, vmPropChain, viewPropChain, BindingDirection.AsyncOneWay);
                //TODO: return something other than null
                if (!ret) return null;

                source = Reflection.ViewModelWhenAnyValue(viewModel, view, vmProperty).SelectMany(selector);
            }

            IDisposable disp = bindToDirect(source, view, viewProperty, fallbackValue);

            return new ReactiveBinding<TView, TViewModel>(view, viewModel, viewPropChain, vmPropChain, BindingDirection.OneWay, disp);
        }

        public IDisposable BindTo<TValue, TTarget, TTValue>(
            IObservable<TValue> This,
            TTarget target,
            Expression<Func<TTarget, TTValue>> property,
            Func<TValue> fallbackValue = null,
            object conversionHint = null)
        {
            var viewPropChain = Reflection.ExpressionToPropertyNames(property);
            var ret = evalBindingHooks(This, target, null, viewPropChain, BindingDirection.OneWay);
            if (!ret) return Disposable.Empty;
                
            var converter = getConverterForTypes(typeof (TValue), typeof(TTValue));

            if (converter == null) {
                throw new ArgumentException(String.Format("Can't convert {0} to {1}. To fix this, register a IBindingTypeConverter", typeof (TValue), typeof(TTValue)));
            }

            var source = This.SelectMany(x => {
                object tmp;
                if (!converter.TryConvert(x, typeof(TTValue), conversionHint, out tmp)) return Observable.Empty<TTValue>();
                return Observable.Return(tmp == null ? default(TTValue) : (TTValue)tmp);
            });

            return bindToDirect(source, target, property, fallbackValue == null ? default(Func<TTValue>) : new Func<TTValue>(() => {
                object tmp;
                if (!converter.TryConvert(fallbackValue(), typeof(TTValue), conversionHint, out tmp)) return default(TTValue);
                return tmp == null ? default(TTValue) : (TTValue)tmp;
            }));
        }

        IDisposable bindToDirect<TTarget, TValue>(
            IObservable<TValue> This,
            TTarget target,
            Expression<Func<TTarget, TValue>> property,
            Func<TValue> fallbackValue = null)
        {
            var types = new[] { typeof(TTarget) }.Concat(Reflection.ExpressionToPropertyTypes(property)).ToArray();
            var names = Reflection.ExpressionToPropertyNames(property);

            var setter = Reflection.GetValueSetterOrThrow(types.Reverse().Skip(1).First(), names.Last());
            if (names.Length == 1) {
                return This.Subscribe(
                    x => setter(target, x),
                    ex => {
                        this.Log().ErrorException("Binding recieved an Exception!", ex);
                        if (fallbackValue != null) setter(target, fallbackValue());
                    });
            }

            var bindInfo = Observable.CombineLatest(
                This, target.WhenAnyDynamic(names.SkipLast(1).ToArray(), x => x.Value),
                (val, host) => new { val, host });

            return bindInfo
                .Where(x => x.host != null)
                .Subscribe(
                    x => setter(x.host, x.val),
                    ex => {
                        this.Log().ErrorException("Binding recieved an Exception!", ex);
                        if (fallbackValue != null) setter(target, fallbackValue());
                    });
        }

        bool evalBindingHooks<TViewModel, TView>(TViewModel viewModel, TView view, string[] vmPropChain, string[] viewPropChain, BindingDirection direction)
            where TViewModel : class
        {
            var hooks = RxApp.DependencyResolver.GetServices<IPropertyBindingHook>();

            var vmFetcher = default(Func<IObservedChange<object, object>[]>);
            if (vmPropChain != null) {
                vmFetcher = () => {
                    IObservedChange<object, object>[] fetchedValues;
                    Reflection.TryGetAllValuesForPropertyChain(out fetchedValues, viewModel, vmPropChain);
                    return fetchedValues;
                };
            } else {
                vmFetcher = () => {
                    return new[] { 
                        new ObservedChange<object, object>() { 
                            Sender = null, PropertyName = null, Value = viewModel, 
                        } 
                    };
                };
            }
            
            var vFetcher = new Func<IObservedChange<object, object>[]>(() => {
                IObservedChange<object, object>[] fetchedValues;
                Reflection.TryGetAllValuesForPropertyChain(out fetchedValues, view, viewPropChain);
                return fetchedValues;
            });

            var shouldBind = hooks.Aggregate(true, (acc, x) =>
                acc && x.ExecuteHook(viewModel, view, vmFetcher, vFetcher, direction));

            if (!shouldBind) {
                var vmString = String.Format("{0}.{1}", typeof (TViewModel).Name, String.Join(".", vmPropChain));
                var vString = String.Format("{0}.{1}", typeof (TView).Name, String.Join(".", viewPropChain));
                this.Log().Warn("Binding hook asked to disable binding {0} => {1}", vmString, vString);
            }

            return shouldBind;
        }

        MemoizingMRUCache<Tuple<Type, Type>, IBindingTypeConverter> typeConverterCache = new MemoizingMRUCache<Tuple<Type, Type>, IBindingTypeConverter>(
            (types, _) => {
                return RxApp.DependencyResolver.GetServices<IBindingTypeConverter>()
                    .Aggregate(Tuple.Create(-1, default(IBindingTypeConverter)), (acc, x) =>
                    {
                        var score = x.GetAffinityForObjects(types.Item1, types.Item2);
                        return score > acc.Item1 && score > 0 ?
                            Tuple.Create(score, x) : acc;
                    }).Item2;
            }, 25);

        internal IBindingTypeConverter getConverterForTypes(Type lhs, Type rhs)
        {
            lock (typeConverterCache) {
                return typeConverterCache.Get(Tuple.Create(lhs, rhs));
            }
        }
    }
}