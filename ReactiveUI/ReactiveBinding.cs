using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactiveUI
{
    public interface IReactiveBinding<TView, TViewModel> : IDisposable
        where TViewModel : class
        where TView : IViewFor
    {
        /// <summary>
        /// The instance of the view model this binding is applied to.</param>
        /// </summary>
        /// <value>
        /// The view model.
        /// </value>
        TViewModel ViewModel { get; }

        /// <summary>
        /// An array representing the property names on the viewmodel bound to the view.
        /// This can be a child property, for example x.Foo.Bar.Baz</c> in which case
        /// the path will contain "Foo","Bar","Baz".
        /// </summary>
        /// <value>
        /// The property names of the path.
        /// </value>
        string[] ViewModelPath { get; }

        /// <summary>
        /// An enumerable representing the properties on the viewmodel bound to the view.
        /// This can contain child properties, for example x.Foo.Bar.Baz</c> in which case
        /// it will contain the Foo, Bar and Baz properties.
        /// </summary>
        /// <value>
        /// The properties of the path.
        /// </value>
        IObservedChange<object, object>[] ViewModelPathProperties { get; }

        /// <summary>
        /// The instance of the view this binding is applied to.
        /// </summary>
        /// <value>
        /// The view.
        /// </value>
        TView View { get; }

        /// <summary>
        /// An array representing the property names on the view bound to the viewmodel.
        /// This can be a child property, for example x.Foo.Bar.Baz</c> in which case
        /// the path will contain "Foo","Bar","Baz".
        /// </summary>
        /// <value>
        /// The property names of the path.
        /// </value>
        string[] ViewPath { get; }

        /// <summary>
        /// An enumerable representing the properties on the view bound to the viewmodel.
        /// This can contain child properties, for example x.Foo.Bar.Baz</c> in which case
        /// it will contain the Foo, Bar and Baz properties.
        /// </summary>
        /// <value>
        /// The properties of the path.
        /// </value>
        IObservedChange<object, object>[] ViewPathProperties { get; }

        /// <summary>
        /// Gets the direction of the binding.
        /// </summary>
        /// <value>
        /// The direction.
        /// </value>
        BindingDirection Direction { get; }
    }

    internal class ReactiveBinding<TView, TViewModel> : IReactiveBinding<TView, TViewModel>
        where TViewModel : class
        where TView : IViewFor
    {
        private IDisposable bindingDisposable;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppliedBindingInfo{TViewModel}" /> class.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <param name="viewModel">The view model.</param>
        /// <param name="viewPath">The view path.</param>
        /// <param name="viewModelPath">The view model path.</param>
        /// <param name="direction">The direction.</param>
        /// <param name="bindingDisposable">The binding disposable.</param>
        public ReactiveBinding(TView view, TViewModel viewModel, string[] viewPath, string[] viewModelPath, BindingDirection direction, IDisposable bindingDisposable)
        {
            this.View = view;
            this.ViewModel = viewModel;
            this.ViewPath = viewPath;
            this.ViewModelPath = viewModelPath;
            this.Direction = direction;

            this.bindingDisposable = bindingDisposable;
        }

        /// <summary>
        /// The instance of the view model this binding is applied to.</param>
        /// </summary>
        /// <value>
        /// The view model.
        /// </value>
        public TViewModel ViewModel { get; private set; }

        /// <summary>
        /// An array representing the property names on the viewmodel bound to the view.
        /// This can be a child property, for example x.Foo.Bar.Baz</c> in which case
        /// the path will contain "Foo","Bar","Baz".
        /// </summary>
        /// <value>
        /// The property names of the path.
        /// </value>
        public string[] ViewModelPath { get; private set; }

        /// <summary>
        /// An enumerable representing the properties on the viewmodel bound to the view.
        /// This can contain child properties, for example x.Foo.Bar.Baz</c> in which case
        /// it will contain the Foo, Bar and Baz properties.
        /// </summary>
        /// <value>
        /// The properties of the path.
        /// </value>
        public IObservedChange<object, object>[] ViewModelPathProperties
        {
            get {
                IObservedChange<object, object>[] fetchedValues;
                Reflection.TryGetAllValuesForPropertyChain(out fetchedValues, ViewModel, ViewModelPath);
                return fetchedValues;
            }
        }

        /// <summary>
        /// The instance of the view this binding is applied to.
        /// </summary>
        /// <value>
        /// The view.
        /// </value>
        public TView View { get; private set; }

        /// <summary>
        /// An array representing the property names on the view bound to the viewmodel.
        /// This can be a child property, for example x.Foo.Bar.Baz</c> in which case
        /// the path will contain "Foo","Bar","Baz".
        /// </summary>
        /// <value>
        /// The property names of the path.
        /// </value>
        public string[] ViewPath { get; private set;}

        /// <summary>
        /// An enumerable representing the properties on the view bound to the viewmodel.
        /// This can contain child properties, for example x.Foo.Bar.Baz</c> in which case
        /// it will contain the Foo, Bar and Baz properties.
        /// </summary>
        /// <value>
        /// The properties of the path.
        /// </value>
        public IObservedChange<object, object>[] ViewPathProperties
        {
            get {
                IObservedChange<object, object>[] fetchedValues;
                Reflection.TryGetAllValuesForPropertyChain(out fetchedValues, View, ViewPath);
                return fetchedValues;
            }
        }

        /// <summary>
        /// Gets the direction of the binding.
        /// </summary>
        /// <value>
        /// The direction.
        /// </value>
        public BindingDirection Direction { get; private set; }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        public void Dispose()
        {
            if (bindingDisposable != null) {
                bindingDisposable.Dispose();
                bindingDisposable = null;
            }
        }
    }
}
