﻿// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;

namespace ReactiveUI.Tests
{
    /// <summary>
    /// Fake view model.
    /// </summary>
    public class FakeViewModel : ReactiveObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FakeViewModel"/> class.
        /// </summary>
        public FakeViewModel() => Cmd = ReactiveCommand.Create(() => { });

        /// <summary>
        /// Gets or sets the command.
        /// </summary>
        public ReactiveCommand<Unit, Unit> Cmd { get; protected set; }
    }
}
