// Copyright (c) 2020 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Tests
{
    public class RoutableFooView : IViewFor<IRoutableFooViewModel>
    {
        object? IViewFor.ViewModel
        {
            get => ViewModel;
            set => ViewModel = (IRoutableFooViewModel?)value;
        }

        public IRoutableFooViewModel? ViewModel { get; set; }
    }
}
