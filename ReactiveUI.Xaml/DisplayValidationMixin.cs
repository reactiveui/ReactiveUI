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

                lastProvider = provider;

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

            // filter the bindings.
            var elementBindings = bindings
                .Where(x =>
                    Enumerable.Zip(x.TargetPath, propertyNames, EqualityComparer<string>.Default.Equals)
                              .All(_ => _));

            elementBindings
                .Subscribe(b =>
                    {
                        var errorProvider = bindingErrorProviders
                            .OrderByDescending(x => x.GetAffinityForBinding(b))
                            .FirstOrDefault();

                        if (errorProvider == null) {
                            LogHost.Default.Info("No BindingErrorProvider for binding {0}", b);
                            return;
                        }

                        errorProvider.GetErrorsForBinding(b)
                                     .Subscribe(new ValidationObserver(b, bindingDisplayProviders));
                    });
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

    class XamlValidationDisplayProvider : IBindingDisplayProvider, IEnableLogger
    {
        static int lastFrameworkIndex(BindingInfo binding, string[] propNames)
        {
            var types = Reflection.GetTypesForPropChain(binding.Target.GetType(), propNames);

            var lastFramework =
                types.Select((x, i) => new KeyValuePair<int, Type>(i, x))
                     .LastOrDefault(x => typeof (FrameworkElement).IsAssignableFrom(x.Value));


            if (lastFramework.Value == null) {
                // default value
                return -1;
            }

            var lastFrameworkIndex = lastFramework.Key;
            return lastFrameworkIndex;
        }

        public int GetAffinityForBinding(BindingInfo binding)
        {
            var propNames = binding.TargetPath.ToArray();

            var lastFrameworkIndex = XamlValidationDisplayProvider.lastFrameworkIndex(binding, propNames);

            if (lastFrameworkIndex == -1) {
                // no framework element, can't bind.
                return 0;
            }

            if (lastFrameworkIndex == propNames.Length -1) {
                return 15; // it is the one before last, best case.
            }
            else {
                return 10; // we can still handle that.
            }
        }
        

        public void SetBindingError(BindingInfo binding, IEnumerable<object> errors)
        {
            var lastFrameworkIndex = XamlValidationDisplayProvider.lastFrameworkIndex(binding,
                binding.TargetPath.ToArray());

            FrameworkElement element;

            var frameworkElementPropertyPath = binding.TargetPath.Take(lastFrameworkIndex + 1).ToArray();
            Reflection.TryGetValueForPropertyChain(out element, binding.Target,
                frameworkElementPropertyPath);

            if (element == null) {
                this.Log()
                    .Info("Attempted to set error on null FrameworkElement, property path: {0}",
                        string.Join(".", frameworkElementPropertyPath));
            }

            var error = errors.FirstOrDefault(x => x != null);

            if (error != null) {
                ManualValidation.MarkInvalid(element, error);
            }
            else {
                ManualValidation.ClearValidation(element);
            }
        }
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

    class NotifyDataErrorInfoBindingProvider : IBindingErrorProvider
    {
        static int maxNotifyDataErrorInfoIndex(BindingInfo binding, string[] propNames)
        {
            IObservedChange<object, object>[] values;

            Reflection.TryGetAllValuesForPropertyChain(out values, binding.Source, propNames);

            var maxNotifyDataErrorInfo = 0;

            for (int i = 0; i < values.Length; i++) {
                if (values[i] == null || values[i].Value == null) {
                    break;
                }

                if (values[i].Value is INotifyDataErrorInfo) {
                    maxNotifyDataErrorInfo = i;
                }
            }
            return maxNotifyDataErrorInfo;
        }

        public int GetAffinityForBinding(BindingInfo binding)
        {
            var propNames = binding.SourcePath.ToArray();
            var maxNotifyDataErrorInfo = maxNotifyDataErrorInfoIndex(binding, propNames);

            if (maxNotifyDataErrorInfo == propNames.Length -1) {
                // bind more tightly if it is the one before last;
                return 25;
            }
            else {
                return maxNotifyDataErrorInfo > 0 ? 20 : 0;
            }
        }

        
        public IObservable<IDataError> GetErrorsForBinding(BindingInfo binding)
        {
            var propNames = binding.SourcePath.ToArray();
            var index = maxNotifyDataErrorInfoIndex(binding, propNames);

            var validationPropertyName = propNames.Skip(index).FirstOrDefault();

            //var source =
            //    binding.Source.SubscribeToExpressionChain<object, INotifyDataErrorInfo>(propNames.Take(index),
            //        skipInitial: false).Value();


            var source = Observable.Return(binding.Source as INotifyDataErrorInfo);

            return
                source
                       .SelectMany(x =>
                           {
                               return Observable.Return( // return one initial value to reset the errors
                                   new EventPattern<DataErrorsChangedEventArgs>(x, new DataErrorsChangedEventArgs(null)))
                                                .Concat(x == null // no errors on a null object
                                                    ? Observable.Empty<EventPattern<DataErrorsChangedEventArgs>>()
                                                    : Observable.FromEventPattern<DataErrorsChangedEventArgs>(
                                                        h => x.ErrorsChanged += h,
                                                        h => x.ErrorsChanged -= h));
                           })
                       .Where(
                           x => x.EventArgs.PropertyName == null || x.EventArgs.PropertyName == validationPropertyName)
                       .Select(x =>
                           new DataError
                               {
                                   Errors =
                                       ((INotifyDataErrorInfo) x.Sender).GetErrors(x.EventArgs.PropertyName)
                                                                        .Cast<object>(),
                                   PropertyName = x.EventArgs.PropertyName,
                                   Sender = x.Sender
                               });
        }
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

    class DataError : IDataError
    {
        public object Sender { get; set; }
        public string PropertyName { get; set; }
        public IEnumerable<object> Errors { get; set; }
    }
}
