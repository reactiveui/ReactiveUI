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
}
