using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactiveUI
{
    public interface IAppliedBindingInfo<TView, TViewModel> : IDisposable
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
}
