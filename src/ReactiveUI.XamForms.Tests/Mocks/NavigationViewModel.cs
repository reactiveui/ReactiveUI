// Copyright (c) 2022 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Reactive;
using System.Reactive.Linq;

using Splat;

namespace ReactiveUI.XamForms.Tests.Mocks
{
    /// <summary>
    /// The navigation view model.
    /// </summary>
    public class NavigationViewModel : ReactiveObject, IScreen
    {
        /// <inheritdoc/>
        public RoutingState Router { get; } = new();

        /// <summary>
        /// Navigates the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>An observable with the view model.</returns>
        public IObservable<IRoutableViewModel> Navigate(string name)
        {
            var viewModel = Locator.Current.GetService<IRoutableViewModel>(name);

            if (viewModel is null)
            {
                return Observable.Throw<IRoutableViewModel>(new InvalidOperationException("Could not find the view model with the name."));
            }

            return Router.Navigate.Execute(viewModel);
        }

        /// <summary>
        /// Navigates to child.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>An observable with the view model.</returns>
        public IObservable<IRoutableViewModel> NavigateToChild(string value)
        {
            var viewModel = new ChildViewModel(value);
            return Router.Navigate.Execute(viewModel);
        }

        /// <summary>
        /// Navigates and reset to child.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>An observable with the view model.</returns>
        public IObservable<IRoutableViewModel> NavigateAndResetToChild(string value)
        {
            var viewModel = new ChildViewModel(value);
            return Router.NavigateAndReset.Execute(viewModel);
        }

        /// <summary>
        /// Navigates back.
        /// </summary>
        /// <returns>An observable.</returns>
        public IObservable<IRoutableViewModel?> NavigateBack() => Router.NavigateBack.Execute();
    }
}
