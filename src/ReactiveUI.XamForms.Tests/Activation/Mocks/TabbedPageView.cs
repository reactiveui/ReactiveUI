// Copyright (c) 2022 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Reactive.Disposables;

namespace ReactiveUI.XamForms.Tests.Activation
{
    /// <summary>
    /// Tabbed Page View.
    /// </summary>
    public class TabbedPageView : ReactiveTabbedPage<TabbedPageViewModel>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TabbedPageView"/> class.
        /// </summary>
        public TabbedPageView() =>
            this.WhenActivated(d =>
            {
                IsActiveCount++;
                d(Disposable.Create(() => IsActiveCount--));
            });

        /// <summary>
        /// Gets or sets the active count.
        /// </summary>
        public int IsActiveCount { get; set; }
    }
}
