﻿// Copyright (c) 2020 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace ReactiveUI.Benchmarks
{
    /// <summary>
    /// A mock for the screen in ReactiveUI. This will only contain the routing state.
    /// </summary>
    public class MockHostScreen : IScreen
    {
        /// <summary>
        /// Gets the routing state for our mock.
        /// </summary>
        public RoutingState Router { get; } = new RoutingState();
    }
}
