// Copyright (c) 2020 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Linq.Expressions;
using System.Reactive.Linq;
#if WINUI3UWP
using Microsoft.UI.Xaml.Input;
#else
using System.Windows.Input;
#endif

namespace ReactiveUI
{
    /// <summary>
    /// Internal implementation details which performs Binding ICommand's to controls.
    /// </summary>
    internal static class CommandBinderImplementationMixins
    {
        public static IReactiveBinding<TView, TProp> BindCommand<TView, TViewModel, TProp, TControl>(
                this ICommandBinderImplementation @this,
                TViewModel viewModel,
                TView view,
                Expression<Func<TViewModel, TProp>> propertyName,
                Expression<Func<TView, TControl>> controlName,
                string? toEvent = null)
            where TViewModel : class
            where TView : class, IViewFor<TViewModel>
            where TProp : ICommand =>
            @this.BindCommand(viewModel, view, propertyName, controlName, Observable<object>.Empty, toEvent);

        public static IReactiveBinding<TView, TProp> BindCommand<TView, TViewModel, TProp, TControl, TParam>(
                this ICommandBinderImplementation @this,
                TViewModel viewModel,
                TView view,
                Expression<Func<TViewModel, TProp>> propertyName,
                Expression<Func<TView, TControl>> controlName,
                Expression<Func<TViewModel, TParam>> withParameter,
                string? toEvent = null)
            where TViewModel : class
            where TView : class, IViewFor<TViewModel>
            where TProp : ICommand
        {
            if (withParameter is null)
            {
                throw new ArgumentNullException(nameof(withParameter));
            }

            var paramExpression = Reflection.Rewrite(withParameter.Body);
            var param = Reflection.ViewModelWhenAnyValue(viewModel, view, paramExpression);

            return @this.BindCommand(viewModel, view, propertyName, controlName, param, toEvent);
        }
    }
}
