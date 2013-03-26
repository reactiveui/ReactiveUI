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
        static readonly IBindingRegistry registry;

        static DisplayValidationMixin()
        {
            registry = RxApp.GetService<IBindingRegistry>();
            registry.Monitor = true;
        }

        public static void DisplayValidationFor<TView, TProp>(this TView view, Expression<Func<TView, TProp>> property)
            where TView : IViewFor
        {
            var bindings = registry.GetBindingForView(view);

            var propertyNames = Reflection.ExpressionToPropertyNames(property);
            var propertyTypes = Reflection.ExpressionToPropertyTypes(property);

            // we want to display validation on the FrameworkElement furthest up the chain
            var propertyToBind = propertyTypes
                .Select((x, i) => new KeyValuePair<int, Type>(i, x))
                .LastOrDefault(x => typeof (FrameworkElement).IsAssignableFrom(x.Value));

            if (propertyToBind.Value == null) {
                LogHost.Default.Warn("Tried to display validation for an element that does not support validation.");
                return;
            }

            var frameworkElementPropertyPath = propertyNames.Take(propertyToBind.Key);

            var element = view.SubscribeToExpressionChain<IViewFor, FrameworkElement>(
                frameworkElementPropertyPath, skipInitial: false);

            var elementBindings = bindings
                .Where(x =>
                    Enumerable.Zip(x.TargetPath, propertyNames, EqualityComparer<string>.Default.Equals)
                              .All(_ => _));

            element
                .SelectMany(e => elementBindings
                    .SelectMany(b => b
                        .getNotifyDataErrorInfoSource()
                        .Select(ev => new {EventArgs = ev, Binding = b}))
                    .Select(b => Tuple.Create(e, b)))
                .Subscribe(x =>
                    {
                        var errorInfo = x.Item2.EventArgs;
                        var binding = x.Item2.Binding;
                        var frameworkElement = x.Item1.Value;

                        var notifyErrorSource = errorInfo.Sender as INotifyDataErrorInfo;

                        if (notifyErrorSource == null || !notifyErrorSource.HasErrors) {
                            // no data error info or no errors, just clear everything
                            ManualValidation.ClearValidation(frameworkElement);
                        }
                        else {
                            var propertyPath = binding.SourcePath.SkipLast(1).LastOrDefault();

                            // if propertyPath is null, we are binding to the object itself
                            // and conveniently, GetErrors(null) returns entity level errors.
                            var errors = notifyErrorSource.GetErrors(propertyPath);

                            var errorContent = errors.Cast<object>().FirstOrDefault();

                            if (errorContent != null) {
                                ManualValidation.MarkInvalid(frameworkElement, errorContent);
                            }
                        }
                    });
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
}
