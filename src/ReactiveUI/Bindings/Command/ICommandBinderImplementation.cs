﻿// Copyright (c) 2020 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

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
    /// Implementation logic for command binding.
    /// </summary>
    internal interface ICommandBinderImplementation : IEnableLogger
    {
        /// <summary>
        /// Binds the command on a ViewModel to a control on the View.
        /// </summary>
        /// <param name="viewModel">The view model to bind to.</param>
        /// <param name="view">The view to bind to.</param>
        /// <param name="propertyName">The name of the property on the View Model.</param>
        /// <param name="controlName">The name of the control on the View.</param>
        /// <param name="withParameter">A function if we want to pass a parameter to the ICommand.</param>
        /// <param name="toEvent">A event on the view that will also trigger the command.</param>
        /// <typeparam name="TView">The type of the view.</typeparam>
        /// <typeparam name="TViewModel">The type of the view model.</typeparam>
        /// <typeparam name="TProp">The type of the property on the view model.</typeparam>
        /// <typeparam name="TControl">The type of control on the view.</typeparam>
        /// <typeparam name="TParam">The type of the parameter to pass to the ICommand.</typeparam>
        /// <returns>A reactive binding. Often only used for disposing the binding.</returns>
        IReactiveBinding<TView, TViewModel, TProp> BindCommand<TView, TViewModel, TProp, TControl, TParam>(
                TViewModel viewModel,
                TView view,
                Expression<Func<TViewModel, TProp>> propertyName,
                Expression<Func<TView, TControl>> controlName,
                Func<TParam> withParameter,
                string? toEvent = null)
            where TViewModel : class
            where TView : class, IViewFor<TViewModel>
            where TProp : ICommand;

        /// <summary>
        /// Binds the command on a ViewModel to a control on the View.
        /// </summary>
        /// <param name="viewModel">The view model to bind to.</param>
        /// <param name="view">The view to bind to.</param>
        /// <param name="propertyName">The name of the property on the View Model.</param>
        /// <param name="controlName">The name of the control on the View.</param>
        /// <param name="withParameter">A observable if we want to pass a parameter to the ICommand.</param>
        /// <param name="toEvent">A event on the view that will also trigger the command.</param>
        /// <typeparam name="TView">The type of the view.</typeparam>
        /// <typeparam name="TViewModel">The type of the view model.</typeparam>
        /// <typeparam name="TProp">The type of the property on the view model.</typeparam>
        /// <typeparam name="TControl">The type of control on the view.</typeparam>
        /// <typeparam name="TParam">The type of the parameter to pass to the ICommand.</typeparam>
        /// <returns>A reactive binding. Often only used for disposing the binding.</returns>
        IReactiveBinding<TView, TViewModel, TProp> BindCommand<TView, TViewModel, TProp, TControl, TParam>(
                TViewModel viewModel,
                TView view,
                Expression<Func<TViewModel, TProp>> propertyName,
                Expression<Func<TView, TControl>> controlName,
                IObservable<TParam> withParameter,
                string? toEvent = null)
            where TViewModel : class
            where TView : class, IViewFor<TViewModel>
            where TProp : ICommand;
    }
}
