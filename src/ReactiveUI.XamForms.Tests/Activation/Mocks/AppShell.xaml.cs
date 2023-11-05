﻿// Copyright (c) 2023 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.XamForms.Tests.Activation
{
    /// <summary>
    /// App Shell.
    /// </summary>
    /// <seealso cref="Xamarin.Forms.Shell" />
    public partial class AppShell
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AppShell"/> class.
        /// </summary>
        public AppShell()
        {
            InitializeComponent();
            ViewModel = new();
            this.WhenActivated(d =>
            {
                IsActiveCount++;
                d(Disposable.Create(() => IsActiveCount--));
            });
        }

        /// <summary>
        /// Gets or sets the active count.
        /// </summary>
        public int IsActiveCount { get; set; }
    }
}
