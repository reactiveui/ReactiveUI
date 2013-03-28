using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactiveUI
{
    /// <summary>
    /// This interface contains information on the errors of a given entity
    /// </summary>
    public interface IDataError
    {
        /// <summary>
        /// The entity described by this instance of <see cref="IDataError"/>.
        /// </summary>
        object Sender { get; }

        /// <summary>
        /// The name of the property described by this instance of <see cref="IDataError"/>.
        /// </summary>
        string PropertyName { get; }

        /// <summary>
        /// A sequence representing the errors on the property described by this instance of <see cref="IDataError"/>.
        /// If there are no errors, the collection is empty.
        /// </summary>
        IEnumerable<object> Errors { get; }
    }

    class DataError : IDataError
    {
        public object Sender { get; set; }
        public string PropertyName { get; set; }
        public IEnumerable<object> Errors { get; set; }
    }

    /// <summary>
    /// This interface represents an entity that is capable of
    /// conveying to the user information about an error on a given binding.
    /// </summary>
    public interface IBindingDisplayProvider
    {
        /// <summary>
        /// Gets an integer representing the affinity of this instance of <see cref="IBindingDisplayProvider"/>
        /// for the given <paramref name="binding"/>. A positive value indicates that this instance of 
        /// <see cref="IBindingDisplayProvider"/> can work with the <paramref name="binding"/> to some extent.
        /// </summary>
        /// <param name="binding">The instance of <see cref="BindingInfo"/> to display error information for.</param>
        /// <returns>An integer representing the affinity of the display provider with the binding.</returns>
        int GetAffinityForBinding(BindingInfo binding);

        /// <summary>
        /// Sets the given <paramref name="errors"/> on the given <paramref name="binding"/>, 
        /// or clears all errors if <paramref name="errors"/> is empty.
        /// </summary>
        /// <param name="binding">The instance of <see cref="BindingInfo"/> to display information for.</param>
        /// <param name="errors">
        /// An IEnumerable containing the errors in the binding.
        /// It may be empty if the binding has no errors, in which case the error display should be reset.
        /// </param>
        void SetBindingError(BindingInfo binding, IEnumerable<object> errors);
    }

    /// <summary>
    /// This interface represents an entity that is capable of providing
    /// error information for a given binding.
    /// </summary>
    public interface IBindingErrorProvider
    {
        /// <summary>
        /// Gets an integer representing the affinity of this instance of <see cref="IBindingErrorProvider"/> 
        /// for the given <paramref name="binding"/>./ A positive value indicates that this instance of
        /// <see cref="IBindingErrorProvider"/> can work with the <paramref name="binding"/> to a certain extent.
        /// </summary>
        /// <param name="binding">The binding object to test.</param>
        /// <returns>
        /// An integer representing the affinity of this instance of <see cref="IBindingErrorProvider"/> 
        /// with the given <paramref name="binding"/>
        /// </returns>
        int GetAffinityForBinding(BindingInfo binding);

        IObservable<IDataError> GetErrorsForBinding(BindingInfo binding);
    }

    public static class DisplayValidationMixin
    {
        /// <summary>
        /// This class encapsulates the behaviour to
        /// set the validation error on a given binding.
        /// </summary>
        class ValidationSetter : IEnableLogger
        {
            readonly BindingInfo binding;
            readonly List<IBindingDisplayProvider> displayProviders;

            IBindingDisplayProvider lastProvider;

            public ValidationSetter(BindingInfo binding, List<IBindingDisplayProvider> displayProviders)
            {
                this.binding = binding;
                this.displayProviders = displayProviders;
            }

            public void SetValidationError(IDataError value)
            {
                var provider =
                    displayProviders.OrderByDescending(p => p.GetAffinityForBinding(binding)).FirstOrDefault();

                if (provider == null) {
                    this.Log().Warn("No display provider for binding {0}", binding);
                    return;
                }

                if (lastProvider != null && provider != lastProvider) {
                    lastProvider.SetBindingError(binding, Enumerable.Empty<object>());
                }

                lastProvider = provider;

                provider.SetBindingError(binding, value.Errors);
            }
        }

        static readonly IBindingRegistry registry;
        static readonly List<IBindingErrorProvider> bindingErrorProviders;
        static readonly List<IBindingDisplayProvider> bindingDisplayProviders;

        static DisplayValidationMixin()
        {
            registry = RxApp.GetService<IBindingRegistry>();
            registry.Monitor = true;

            bindingErrorProviders = RxApp.GetAllServices<IBindingErrorProvider>().ToList();
            bindingDisplayProviders = RxApp.GetAllServices<IBindingDisplayProvider>().ToList();
        }

        /// <summary>
        /// Causes validation information to be displayed for the given view model property.
        /// </summary>
        /// <typeparam name="TViewModel">The type of the view model.</typeparam>
        /// <typeparam name="TProp">The type of the property to displya validation for.</typeparam>
        /// <param name="view">The instance of <see cref="IViewFor{T}"/> on which the binding target is situated.</param>
        /// <param name="property">An expression indicating the property to display binding information for.</param>
        /// <returns>An instnace of <see cref="IDisposable"/>, that, when disposed, disconnects the display of the validation information.</returns>
        public static IDisposable DisplayValidationFor<TViewModel, TProp>(
            this IViewFor<TViewModel> view,
            Expression<Func<TViewModel, TProp>> property)
            where TViewModel : class
        {
            var bindings = registry.GetBindingForView(view);

            var propertyNames = Reflection.ExpressionToPropertyNames(property);

            var elementBindings = bindings
                .Where(x =>
#if WP7
                    {
                        int index = 0;
                        return x.SourcePath.All(pathSegment => propertyNames[index++] == pathSegment);
                    }
#else
                    Enumerable.Zip(x.SourcePath, propertyNames, EqualityComparer<string>.Default.Equals)
                              .All(_ => _)
#endif
                              );


            return displayValidationForBindings(elementBindings);
        }

        /// <summary>
        /// Causes validation information to be displayed for the given view property.
        /// </summary>
        /// <typeparam name="TView">The type of the view.</typeparam>
        /// <typeparam name="TProp">The type of the view property.</typeparam>
        /// <param name="view">The view instance on which the binding target is situated.</param>
        /// <param name="property">An expression indicating the target property of the binding.</param>
        /// <returns>An instance of <see cref="IDisposable"/>, that, when disposed, disconnects the display of the validation information.</returns>
        public static IDisposable DisplayValidationForView<TView, TProp>(this TView view,
            Expression<Func<TView, TProp>> property)
            where TView : IViewFor
        {
            var bindings = registry.GetBindingForView(view);

            var propertyNames = Reflection.ExpressionToPropertyNames(property);

            // filter the bindings.
            var elementBindings = bindings
                .Where(x =>
#if WP7
                    {
                        int index = 0;
                        return x.TargetPath.All(pathSegment => propertyNames[index++] == pathSegment);
                    }
#else
                    Enumerable.Zip(x.TargetPath, propertyNames, EqualityComparer<string>.Default.Equals)
                              .All(_ => _)
#endif
                );

            return displayValidationForBindings(elementBindings);
        }

        /// <summary>
        /// Displays the validation for the given bindings.
        /// </summary>
        static IDisposable displayValidationForBindings(IObservable<BindingInfo> elementBindings)
        {
            return elementBindings
                .SelectMany(b =>
                    {
                        var errorProvider = bindingErrorProviders
                            .OrderByDescending(x => x.GetAffinityForBinding(b))
                            .FirstOrDefault();

                        if (errorProvider == null) {
                            LogHost.Default.Info("No BindingErrorProvider for binding {0}", b);
                            return
                                Observable.Empty(
                                    new {Error = default(IDataError), ValidationObserver = default(ValidationSetter)});
                        }

                        var validationObserver = new ValidationSetter(b, bindingDisplayProviders);

                        return errorProvider.GetErrorsForBinding(b)
                                            .Select(e => new {Error = e, ValidationObserver = validationObserver});
                    })
                .Subscribe(x => x.ValidationObserver.SetValidationError(x.Error));
        }
    }
}