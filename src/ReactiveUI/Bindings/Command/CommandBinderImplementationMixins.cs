// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reflection;
using System.Windows.Input;
using Splat;

namespace ReactiveUI
{
    /// <summary>
    /// Internal implementation details which performs Binding ICommand's to controls.
    /// </summary>
    internal static class CommandBinderImplementationMixins
    {
        public static IReactiveBinding<TView, TViewModel, TProp> BindCommand<TView, TViewModel, TProp, TControl>(
                this ICommandBinderImplementation @this,
                TViewModel viewModel,
                TView view,
                Expression<Func<TViewModel, TProp>> propertyName,
                Expression<Func<TView, TControl>> controlName,
                string toEvent = null)
            where TViewModel : class
            where TView : class, IViewFor<TViewModel>
            where TProp : ICommand
        {
            return @this.BindCommand(viewModel, view, propertyName, controlName, Observable<object>.Empty, toEvent);
        }

        public static IReactiveBinding<TView, TViewModel, TProp> BindCommand<TView, TViewModel, TProp, TControl, TParam>(
                this ICommandBinderImplementation @this,
                TViewModel viewModel,
                TView view,
                Expression<Func<TViewModel, TProp>> propertyName,
                Expression<Func<TView, TControl>> controlName,
                Expression<Func<TViewModel, TParam>> withParameter,
                string toEvent = null)
            where TViewModel : class
            where TView : class, IViewFor<TViewModel>
            where TProp : ICommand
        {
            var paramExpression = Reflection.Rewrite(withParameter.Body);
            var param = Reflection.ViewModelWhenAnyValue(viewModel, view, paramExpression);

            return @this.BindCommand(viewModel, view, propertyName, controlName, param, toEvent);
        }
    }
}
