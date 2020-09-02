﻿// Copyright (c) 2020 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reflection;

namespace ReactiveUI
{
    /// <summary>
    /// This class implements View Activation for classes that explicitly describe
    /// their activation via <see cref="ICanActivate"/>. This class is used by the framework.
    /// </summary>
    public class CanActivateViewFetcher : IActivationForViewFetcher
    {
        /// <summary>
        /// Returns a positive integer for derivates of the <see cref="ICanActivate"/> interface.
        /// </summary>
        /// <param name="view">The source type to check.</param>
        /// <returns>
        /// A positive integer if <see cref="GetActivationForView(IActivatableView)"/> is supported,
        /// zero otherwise.
        /// </returns>
        public int GetAffinityForView(Type view) => typeof(ICanActivate).GetTypeInfo().IsAssignableFrom(view.GetTypeInfo()) ?
                10 : 0;

        /// <summary>
        /// Get an observable defining whether the view is active.
        /// </summary>
        /// <param name="view">The view to observe.</param>
        /// <returns>An observable tracking whether the view is active.</returns>
        public IObservable<bool> GetActivationForView(IActivatableView view)
        {
            var canActivate = view as ICanActivate;

            if (canActivate == null)
            {
                return Observable.Empty(false);
            }

            return canActivate.Activated.Select(_ => true).Merge(canActivate.Deactivated.Select(_ => false));
        }
    }
}
