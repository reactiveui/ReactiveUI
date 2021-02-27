// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;

namespace ReactiveUI.Tests
{
    /// <summary>
    /// A mock view which throws.
    /// </summary>
    public class FooThatThrowsView : IFooView
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FooThatThrowsView"/> class.
        /// </summary>
        /// <exception cref="InvalidOperationException">This is a test failure.</exception>
        public FooThatThrowsView() => throw new InvalidOperationException("This is a test failure.");

        /// <inheritdoc/>
        object? IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (IFooViewModel?)value;
        }

        /// <inheritdoc/>
        public IFooViewModel? ViewModel { get; set; }
    }
}
