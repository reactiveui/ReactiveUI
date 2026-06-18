// Copyright (c) 2009-2026 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Linq.Expressions;
#if REACTIVE_SHIM
using ReactiveUI.Reactive.Wpf.Binding;
#else
using ReactiveUI.Wpf.Binding;
#endif

#if REACTIVE_SHIM
namespace ReactiveUI.Reactive;
#else
namespace ReactiveUI;
#endif
/// <summary>Provides validation-aware binding extension methods for WPF views.</summary>
public static class ValidationBindingMixins
{
    /// <summary>Provides validation-aware binding extension methods for WPF views.</summary>
    /// <param name="view">The view.</param>
    /// <typeparam name="TView">The type of the view.</typeparam>
    extension<TView>(TView view)
        where TView : class, IViewFor
    {
        /// <summary>Binds the validation.</summary>
        /// <typeparam name="TViewModel">The type of the view model.</typeparam>
        /// <typeparam name="TVProp">The type of the v property.</typeparam>
        /// <typeparam name="TType">The type of the type.</typeparam>
        /// <param name="viewModel">The view model.</param>
        /// <param name="viewModelPropertySelector">The view model property selector.</param>
        /// <param name="frameworkElementSelector">The framework element selector.</param>
        /// <returns>
        /// An instance of <see cref="IDisposable"/> that, when disposed,
        /// disconnects the binding.
        /// </returns>
        public IReactiveBinding<TView, TType> BindWithValidation<TViewModel, TVProp, TType>(
            TViewModel viewModel,
            Expression<Func<TViewModel, TType?>> viewModelPropertySelector,
            Expression<Func<TView, TVProp>> frameworkElementSelector)
            where TViewModel : class
        {
            ArgumentExceptionHelper.ThrowIfNull(viewModelPropertySelector);

            ArgumentExceptionHelper.ThrowIfNull(frameworkElementSelector);

            return new ValidationBindingWpf<TView, TViewModel, TVProp, TType>(
                view,
                viewModel,
                viewModelPropertySelector,
                frameworkElementSelector);
        }
    }
}
