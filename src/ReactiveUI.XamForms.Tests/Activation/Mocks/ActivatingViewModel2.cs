﻿// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive.Disposables;

namespace ReactiveUI.XamForms.Tests.Activation
{
    /// <summary>
    /// Activating View Model2.
    /// </summary>
    /// <seealso cref="ReactiveUI.ReactiveObject" />
    /// <seealso cref="ReactiveUI.IActivatableViewModel" />
    public class ActivatingViewModel2 : ReactiveObject, IActivatableViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ActivatingViewModel2"/> class.
        /// </summary>
        public ActivatingViewModel2()
        {
            Activator = new ViewModelActivator();

            this.WhenActivated(d =>
            {
                IsActiveCount++;
                d(Disposable.Create(() => IsActiveCount--));
            });
        }

        /// <summary>
        /// Gets or sets the Activator which will be used by the View when Activation/Deactivation occurs.
        /// </summary>
        public ViewModelActivator Activator { get; protected set; }

        /// <summary>
        /// Gets or sets the active count.
        /// </summary>
        public int IsActiveCount { get; protected set; }
    }
}
