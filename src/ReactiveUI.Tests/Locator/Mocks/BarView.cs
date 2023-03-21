// Copyright (c) 2023 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests
{
    /// <summary>
    /// A mock view.
    /// </summary>
    public class BarView : IViewFor<IBarViewModel>
    {
        /// <summary>
        /// Gets or sets the ViewModel corresponding to this specific View. This should be
        /// a DependencyProperty if you're using XAML.
        /// </summary>
        object? IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (IBarViewModel?)value;
        }

        /// <summary>
        /// Gets or sets the ViewModel corresponding to this specific View. This should be
        /// a DependencyProperty if you're using XAML.
        /// </summary>
        public IBarViewModel? ViewModel { get; set; }
    }
}
