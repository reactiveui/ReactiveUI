// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;

namespace ReactiveUI.Tests
{
    public class FooThatThrowsView : IFooView
    {
        public FooThatThrowsView() => throw new InvalidOperationException("This is a test failure.");

        object? IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (IFooViewModel?)value;
        }

        public IFooViewModel? ViewModel { get; set; }
    }
}
