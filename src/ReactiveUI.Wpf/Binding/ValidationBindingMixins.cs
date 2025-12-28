// Copyright (c) 2025 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Linq.Expressions;

using ReactiveUI.Wpf.Binding;

namespace ReactiveUI;

/// <summary>
/// ValidationBindingMixins.
/// </summary>
public static class ValidationBindingMixins
{
    /// <summary>
    /// Binds the validation.
    /// </summary>
    /// <typeparam name="TViewModel">The type of the view model.</typeparam>
    /// <typeparam name="TView">The type of the view.</typeparam>
    /// <typeparam name="TVProp">The type of the v property.</typeparam>
    /// <typeparam name="TType">The type of the type.</typeparam>
    /// <param name="view">The view.</param>
    /// <param name="viewModel">The view model.</param>
    /// <param name="viewModelPropertySelector">The view model property selector.</param>
    /// <param name="frameworkElementSelector">The framework element selector.</param>
    /// <returns>
    /// An instance of <see cref="IDisposable"/> that, when disposed,
    /// disconnects the binding.
    /// </returns>
#if NET6_0_OR_GREATER
    [RequiresDynamicCode("BindWithValidation uses methods that require dynamic code generation")]
    [RequiresUnreferencedCode("BindWithValidation uses methods that may require unreferenced code")]
#endif
    public static IReactiveBinding<TView, TType> BindWithValidation<TViewModel, TView, TVProp, TType>(this TView view, TViewModel viewModel, Expression<Func<TViewModel, TType?>> viewModelPropertySelector, Expression<Func<TView, TVProp>> frameworkElementSelector)
        where TView : class, IViewFor
        where TViewModel : class
    {
        ArgumentExceptionHelper.ThrowIfNull(viewModelPropertySelector);

        ArgumentExceptionHelper.ThrowIfNull(frameworkElementSelector);

        return new ValidationBindingWpf<TView, TViewModel, TVProp, TType>(view, viewModel, viewModelPropertySelector, frameworkElementSelector);
    }
}
