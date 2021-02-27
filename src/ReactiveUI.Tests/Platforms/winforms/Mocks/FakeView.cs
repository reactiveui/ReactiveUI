﻿// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Windows.Forms;

namespace ReactiveUI.Tests.Winforms
{
    /// <summary>
    /// A fake view.
    /// </summary>
    public class FakeView : IViewFor<FakeViewModel>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FakeView"/> class.
        /// </summary>
        public FakeView()
        {
            TheTextBox = new TextBox();
            ViewModel = new FakeViewModel();
        }

        /// <summary>
        /// Gets or sets the text box.
        /// </summary>
        public TextBox TheTextBox { get; protected set; }

        /// <inheritdoc/>
        object? IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (FakeViewModel?)value;
        }

        /// <inheritdoc/>
        public FakeViewModel? ViewModel { get; set; }
    }
}
