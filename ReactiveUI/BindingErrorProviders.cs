using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;

namespace ReactiveUI
{
    /// <summary>
    /// This class provides an implementation of <see cref="IBindingErrorProvider"/>
    /// for view models implementing the <see cref="INotifyDataErrorInfo"/> interface.
    /// </summary>
    class NotifyDataErrorInfoBindingProvider : IBindingErrorProvider
    {
        static int maxNotifyDataErrorInfoIndex(BindingInfo binding, string[] propNames)
        {
            IObservedChange<object, object>[] values;

            Reflection.TryGetAllValuesForPropertyChain(out values, binding.Source, propNames);

            var maxNotifyDataErrorInfo = -1;

            if (binding.Source is INotifyDataErrorInfo) {
                maxNotifyDataErrorInfo = 0;
            }

            for (int i = 0; i < values.Length; i++) {
                if (values[i] == null || values[i].Value == null) {
                    break;
                }

                if (values[i].Value is INotifyDataErrorInfo) {
                    maxNotifyDataErrorInfo = i + 1;
                }
            }
            return maxNotifyDataErrorInfo;
        }

        public int GetAffinityForBinding(BindingInfo binding)
        {
            var propNames = binding.SourcePath.ToArray();
            var maxNotifyDataErrorInfo = maxNotifyDataErrorInfoIndex(binding, propNames);

            if (maxNotifyDataErrorInfo == -1) {
                // no NotifyDataErrorInfo, we cannot bind.
                return 0;
            }

            if (maxNotifyDataErrorInfo == propNames.Length - 1) {
                // bind more tightly if it is the one before last;
                return 25;
            }
            else {
                return 20;
            }
        }

        public IObservable<IDataError> GetErrorsForBinding(BindingInfo binding)
        {
            var propNames = binding.SourcePath.ToArray();
            var index = maxNotifyDataErrorInfoIndex(binding, propNames);

            var validationPropertyName = propNames.Skip(index).FirstOrDefault();

            IObservable<INotifyDataErrorInfo> source;

            if (propNames.Length > 1) {
                source =
                    binding.Source.SubscribeToExpressionChain<object, INotifyDataErrorInfo>(propNames.Take(index),
                        skipInitial: false).Value();
            }
            else {
                source = Observable.Return(binding.Source as INotifyDataErrorInfo);
            }

            return
                source
                    .SelectMany(x =>
                        Observable.Return( // return one initial value to reset the errors
                            new EventPattern<DataErrorsChangedEventArgs>(x,
                                new DataErrorsChangedEventArgs(x.HasErrors ? validationPropertyName : null)))
                                  .Concat(x == null // no errors on a null object
                                      ? Observable.Empty<EventPattern<DataErrorsChangedEventArgs>>()
                                      : Observable.FromEventPattern<DataErrorsChangedEventArgs>(
                                          h => x.ErrorsChanged += h,
                                          h => x.ErrorsChanged -= h)))
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

    class DataErrorInfoBindingProvider : IBindingErrorProvider
    {
        static int maxDataErrorInfoIndex(BindingInfo binding, string[] propNames)
        {
            IObservedChange<object, object>[] values;

            Reflection.TryGetAllValuesForPropertyChain(out values, binding.Source, propNames);

            var maxDataErrorInfoIndex = -1;

            if (binding.Source is IDataErrorInfo) {
                maxDataErrorInfoIndex = 0;
            }

            for (int i = 0; i < values.Length; i++) {
                if (values[i] == null || values[i].Value == null) {
                    break;
                }

                if (values[i].Value is IDataErrorInfo) {
                    maxDataErrorInfoIndex = i + 1;
                }
            }
            return maxDataErrorInfoIndex;
        }

        public int GetAffinityForBinding(BindingInfo binding)
        {
            var propNames = binding.SourcePath.ToArray();
            var errorInfoIndex = maxDataErrorInfoIndex(binding, propNames);

            if (errorInfoIndex == -1) {
                // no NotifyDataErrorInfo, we cannot bind.
                return 0;
            }

            if (errorInfoIndex == propNames.Length - 1) {
                // bind more tightly if it is the one before last;
                return 15;
            }
            else {
                return 10;
            }
        }

        public IObservable<IDataError> GetErrorsForBinding(BindingInfo binding)
        {
            var propNames = binding.SourcePath.ToArray();
            var index = maxDataErrorInfoIndex(binding, propNames);

            var validationPropertyName = propNames.Skip(index).FirstOrDefault();

            IObservable<IDataErrorInfo> source;

            if (propNames.Length > 1) {
                source =
                    binding.Source.SubscribeToExpressionChain<object, IDataErrorInfo>(propNames.Take(index),
                        skipInitial: false).Value();
            }
            else {
                source = Observable.Return(binding.Source as IDataErrorInfo);
            }

            return
                source
                    .SelectMany(x => SubscribeToDataErrors(x, validationPropertyName));
        }

        IObservable<IDataError> SubscribeToDataErrors(IDataErrorInfo instance, string property)
        {
            return instance
                .ObservableForProperty(new[] {property}, skipInitial: false)
                .Select(x =>
                    {
                        var instanceError = x.Sender[property];

                        var errors = string.IsNullOrEmpty(instanceError)
                            ? Enumerable.Empty<object>()
                            : new object[] {instanceError};

                        return (IDataError)new DataError {Errors = errors, PropertyName = property, Sender = x.Sender};
                    });
        }
    }
}
