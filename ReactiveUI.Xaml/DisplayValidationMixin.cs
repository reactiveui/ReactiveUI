using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace ReactiveUI.Xaml
{
    public static class DisplayValidationMixin 
    {
        class ValidationObserver : IObserver<IDataError>, IEnableLogger
        {
            readonly BindingInfo binding;
            readonly List<IBindingDisplayProvider> displayProviders;

            IBindingDisplayProvider lastProvider;

            public ValidationObserver(BindingInfo binding, List<IBindingDisplayProvider> displayProviders)
            {
                this.binding = binding;
                this.displayProviders = displayProviders;
            }

            public void OnNext(IDataError value)
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

                provider.SetBindingError(binding, value.Errors);
            }

            public void OnError(Exception error)
            {
                throw error;
            }

            public void OnCompleted()
            {
                if (lastProvider != null) {
                    lastProvider.SetBindingError(binding, Enumerable.Empty<object>());
                }

                lastProvider = null;
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

        public static void DisplayValidationFor<TView, TProp>(this TView view, Expression<Func<TView, TProp>> property)
            where TView : IViewFor
        {
            var bindings = registry.GetBindingForView(view);

            var propertyNames = Reflection.ExpressionToPropertyNames(property);
            var element = view.WhenAny(property, c => c.Value);

            // filter the bindings.
            var elementBindings = bindings
                .Where(x =>
                    Enumerable.Zip(x.TargetPath, propertyNames, EqualityComparer<string>.Default.Equals)
                              .All(_ => _));

            elementBindings
                .Subscribe(b => element
                    .SelectMany(e =>
                        {
                            var errorProvider = bindingErrorProviders
                                .OrderByDescending(x => x.GetAffinityForBinding(b))
                                .FirstOrDefault();

                            if (errorProvider == null) {
                                LogHost.Default.Info("No BindingErrorProvider for binding {0}", b);
                                return Observable.Empty<IDataError>();
                            }

                            return errorProvider.GetErrorsForBinding(b);
                        })
                    .Subscribe(new ValidationObserver(b, bindingDisplayProviders)));
        }

        static IObservable<EventPattern<DataErrorsChangedEventArgs>> getNotifyDataErrorInfoSource(this BindingInfo info)
        {
            string propertyName = info.SourcePath.SkipLast(1).LastOrDefault();

            return info
                .Source
                .SubscribeToExpressionChain<object, object>(info.SourcePath.SkipLast(1))
                .Value()
                .Select(x => x as INotifyDataErrorInfo)
                .SelectMany(x =>
                    Observable.Return( // return one initial value to reset the errors
                        new EventPattern<DataErrorsChangedEventArgs>(x, new DataErrorsChangedEventArgs(null)))
                              .Concat(x == null // no errors on a null object
                                  ? Observable.Empty<EventPattern<DataErrorsChangedEventArgs>>()
                                  : Observable.FromEventPattern<DataErrorsChangedEventArgs>(
                                      h => x.ErrorsChanged += h,
                                      h => x.ErrorsChanged -= h)))
                .Where(e => e.EventArgs.PropertyName == null || e.EventArgs.PropertyName == propertyName);
        }
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
}
