// Copyright (c) 2024 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows;
using ReactiveUI.Wpf.Binding;

namespace ReactiveUI;

/// <summary>
/// ValidationBindingMixins.
/// </summary>
public static class ValidationBindingMixins
{
    /// <summary>
    /// Binds the specified view model property to the given view property.
    /// This binding will also validate the property and show the validation error in the view if enabled.
    /// Add AddValidation(() => ReactivePropertyInstance) to the ReactiveProperty to enable validation.
    /// At the moment, this binding only supports binding to a DependencyProperty for Validation.
    /// Binding Converters are not supported.
    /// </summary>
    /// <typeparam name="TViewModel">The type of the view model being bound.</typeparam>
    /// <typeparam name="TView">The type of the view being bound.</typeparam>
    /// <typeparam name="TVProp">The type of the property bound on the view.</typeparam>
    /// <typeparam name="TType">The property name.</typeparam>
    /// <param name="view">The instance of the view to bind.</param>
    /// <param name="viewModel">The instance of the view model to bind.</param>
    /// <param name="viewModelPropertySelector">The view model ReactiveProperty selector.</param>
    /// <param name="frameworkElementSelector">The framework element selector from the view.</param>
    /// <param name="propertySelector">The DependencyProperty name selector.</param>
    /// <returns>
    /// An instance of <see cref="IDisposable"/> that, when disposed,
    /// disconnects the binding.
    /// </returns>
    public static IReactiveBinding<TView, TType> Bind<TViewModel, TView, TVProp, TType>(this TView view, TViewModel viewModel, Func<TViewModel, IReactiveProperty<TType>> viewModelPropertySelector, Func<TView, TVProp> frameworkElementSelector, Func<TVProp, string> propertySelector)
        where TView : class, IViewFor
        where TViewModel : class
        where TVProp : FrameworkElement
    {
        if (viewModelPropertySelector == null)
        {
            throw new ArgumentNullException(nameof(viewModelPropertySelector));
        }

        if (frameworkElementSelector == null)
        {
            throw new ArgumentNullException(nameof(frameworkElementSelector));
        }

        if (propertySelector == null)
        {
            throw new ArgumentNullException(nameof(propertySelector));
        }

        return new ValidationBinding<TView, TViewModel, TVProp, TType>(view, viewModel, viewModelPropertySelector, frameworkElementSelector, propertySelector);
    }
}
