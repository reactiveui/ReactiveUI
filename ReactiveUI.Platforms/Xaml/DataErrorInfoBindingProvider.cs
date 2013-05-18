using System;
using System.Linq;
using System.Reactive.Linq;
using System.ComponentModel;
using ReactiveUI;

namespace ReactiveUI.Xaml
{
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
            } else {
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
                source = binding.Source
                    .SubscribeToExpressionChain<object, IDataErrorInfo>(propNames.Take(index), skipInitial: false)
                    .Value();
            }
            else {
                source = Observable.Return(binding.Source as IDataErrorInfo);
            }

            return source.SelectMany(x => SubscribeToDataErrors(x, validationPropertyName));
        }

        IObservable<IDataError> SubscribeToDataErrors(IDataErrorInfo instance, string property)
        {
            return instance
                .ObservableForProperty(new[] {property}, skipInitial: false)
                .Select(x => {
                    var instanceError = x.Sender[property];

                    var errors = string.IsNullOrEmpty(instanceError)
                        ? Enumerable.Empty<object>()
                        : new object[] {instanceError};

                    return (IDataError)new DataError {Errors = errors, PropertyName = property, Sender = x.Sender};
                });
        }
    }
}