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
    internal interface ICommandBinderImplementation : IEnableLogger
    {
        IReactiveBinding<TView, TViewModel, TProp> BindCommand<TView, TViewModel, TProp, TControl, TParam>(
                TViewModel viewModel,
                TView view,
                Expression<Func<TViewModel, TProp>> propertyName,
                Expression<Func<TView, TControl>> controlName,
                Func<TParam> withParameter,
                string toEvent = null)
            where TViewModel : class
            where TView : class, IViewFor<TViewModel>
            where TProp : ICommand;

        IReactiveBinding<TView, TViewModel, TProp> BindCommand<TView, TViewModel, TProp, TControl, TParam>(
                TViewModel viewModel,
                TView view,
                Expression<Func<TViewModel, TProp>> propertyName,
                Expression<Func<TView, TControl>> controlName,
                IObservable<TParam> withParameter,
                string toEvent = null)
            where TViewModel : class
            where TView : class, IViewFor<TViewModel>
            where TProp : ICommand;
    }
}
