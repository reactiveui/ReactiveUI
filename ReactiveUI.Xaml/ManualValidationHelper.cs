using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace ReactiveUI.Xaml
{
    /// <summary>
    /// Provides helpers to manually toggle validation errors.
    /// </summary>
    static class ManualValidation
    {
        // this dummy attached property is used as a source
        // of the binding.
        static readonly DependencyProperty dummyProperty =
                DependencyProperty.RegisterAttached("DummyProperty", typeof(object), typeof(ManualValidation), new PropertyMetadata(null));

        // this class implements a dummy validation rule without behaviour.
        private class DummyValidationRule : ValidationRule
        {
            public override ValidationResult Validate(object value, CultureInfo cultureInfo)
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Manually mark a validation error on the given framework element.
        /// </summary>
        /// <param name="element">The instance of <see cref="FrameworkElement"/> on which to mark the validation error.</param>
        /// <param name="errorContent">An object representing the content of the error.</param>
        public static void MarkInvalid(FrameworkElement element, object errorContent)
        {
            // create a dummy binding. Conveniently, we bind to the tag of the FrameworkElement,
            // so as to minimise the potential interaction with other code.
            var binding = new Binding("Tag") { Source = element, Mode = BindingMode.OneWayToSource };

            // set the binding on to our dummy property.
            BindingOperations.SetBinding(element, dummyProperty, binding);

            // we now get the live binding expression.
            var bindingExpression = element.GetBindingExpression(dummyProperty);

            // create a dummy binding error, with the specified error content.
            var validationError = new ValidationError(new DummyValidationRule(), binding, errorContent, null);

            // and manually set the validation error on the binding.
            Validation.MarkInvalid(bindingExpression, validationError);
        }

        /// <summary>
        /// Clears all manually assigned errors on the given <paramref name="element"/>.
        /// </summary>
        /// <param name="element">The instance of <see cref="FrameworkElement"/> on which to clear the validation.</param>
        public static void ClearValidation(FrameworkElement element)
        {
            // to clear an error, we simply remove all bindings to our dummy property.
            BindingOperations.ClearBinding(element, dummyProperty);
        }

    }
}
