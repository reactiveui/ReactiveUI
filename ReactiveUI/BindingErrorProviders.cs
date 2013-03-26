using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactiveUI
{
#if NET_45 || SILVERLIGHT5
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
                        Observable.Return( // return one initial value to reset the errors
                            new EventPattern<DataErrorsChangedEventArgs>(x, new DataErrorsChangedEventArgs(null)))
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
#endif
}
