﻿// Copyright (c) 2022 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Windows;

namespace ReactiveUI.Tests.Wpf
{
    public class WpfActiveContentApp : Application
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        /// <summary>
        /// Gets the mock window factory.
        /// </summary>
        /// <value>
        /// The mock window factory.
        /// </value>
        public Func<MockWindow> MockWindowFactory { get; } = () => new();

        /// <summary>
        /// Gets the tc mock window factory.
        /// </summary>
        /// <value>
        /// The tc mock window factory.
        /// </value>
        public Func<TCMockWindow> TCMockWindowFactory { get; } = () => new();

        /// <summary>
        /// Gets the WPF test window factory.
        /// </summary>
        /// <value>
        /// The WPF test window factory.
        /// </value>
        public Func<WpfTestWindow> WpfTestWindowFactory { get; } = () => new();

#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    }
}
