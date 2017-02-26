using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ReactiveUI
{
    /// <summary>
    /// This interface represents the result of a Bind/OneWayBind and gives
    /// information about the binding. When this object is disposed, it will
    /// destroy the binding it is describing (i.e. most of the time you won't
    /// actually care about this object, just that it is disposable)
    /// </summary>
    public interface IReactiveBinding<TView, TViewModel, TValue> : IDisposable
        where TViewModel : class
        where TView : IViewFor
    {
        /// <summary>
        /// The instance of the view model this binding is applied to.
        /// </summary>
        /// <value>
        /// The view model.
        /// </value>
        TViewModel ViewModel { get; }

        /// <summary>
        /// An expression representing the propertyon the viewmodel bound to the view.
        /// This can be a child property, for example x.Foo.Bar.Baz in which case
        /// that will be the expression.
        /// </summary>
        /// <value>
        /// The expression.
        /// </value>
        Expression ViewModelExpression { get; }

        /// <summary>
        /// The instance of the view this binding is applied to.
        /// </summary>
        /// <value>
        /// The view.
        /// </value>
        TView View { get; }

        /// <summary>
        /// An expression representing the property on the view bound to the viewmodel.
        /// This can be a child property, for example x.Foo.Bar.Baz in which case
        /// that will be the expression.
        /// </summary>
        /// <value>
        /// The expression.
        /// </value>
        Expression ViewExpression { get; }

        /// <summary>
        /// An observable representing changed values for the binding.
        /// </summary>
        /// <value>
        /// The changed.
        /// </value>
        IObservable<TValue> Changed { get; }

        /// <summary>
        /// Gets the direction of the binding.
        /// </summary>
        /// <value>
        /// The direction.
        /// </value>
        BindingDirection Direction { get; }
    }

    internal class ReactiveBinding<TView, TViewModel, TValue> : IReactiveBinding<TView, TViewModel, TValue>
        where TViewModel : class
        where TView : IViewFor
    {
        private IDisposable bindingDisposable;

        public ReactiveBinding(TView view, TViewModel viewModel, Expression viewExpression, Expression viewModelExpression, 
            IObservable<TValue> changed, BindingDirection direction, IDisposable bindingDisposable)
        {
            this.View = view;
            this.ViewModel = viewModel;
            this.ViewExpression = viewExpression;
            this.ViewModelExpression = viewModelExpression;
            this.Direction = direction;
            this.Changed = changed;

            this.bindingDisposable = bindingDisposable;
        }

        /// <summary>
        /// The instance of the view model this binding is applied to.
        /// </summary>
        /// <value>
        /// The view model.
        /// </value>
        public TViewModel ViewModel { get; private set; }

        /// <summary>
        /// An expression representing the propertyon the viewmodel bound to the view.
        /// This can be a child property, for example x.Foo.Bar.Baz in which case
        /// that will be the expression.
        /// </summary>
        /// <value>
        /// The expression.
        /// </value>
        public Expression ViewModelExpression { get; private set; }

        /// <summary>
        /// The instance of the view this binding is applied to.
        /// </summary>
        /// <value>
        /// The view.
        /// </value>
        public TView View { get; private set; }

        /// <summary>
        /// An expression representing the property on the view bound to the viewmodel.
        /// This can be a child property, for example x.Foo.Bar.Baz in which case
        /// that will be the expression.
        /// </summary>
        /// <value>
        /// The expression.
        /// </value>
        public Expression ViewExpression { get; private set; }

        /// <summary>
        /// An observable representing changed values for the binding.
        /// </summary>
        /// <value>
        /// The changed.
        /// </value>
        public IObservable<TValue> Changed { get; private set; }

        /// <summary>
        /// Gets the direction of the binding.
        /// </summary>
        /// <value>
        /// The direction.
        /// </value>
        public BindingDirection Direction { get; private set; }

        public void Dispose()
        {
            if (bindingDisposable != null) {
                bindingDisposable.Dispose();
                bindingDisposable = null;
            }
        }
    }
}
