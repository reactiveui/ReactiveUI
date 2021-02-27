// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReactiveUI.Tests
{
    public class TestViewModel : ReactiveObject, IRoutableViewModel
    {
        private string? _someProp;

        public string? SomeProp
        {
            get => _someProp;
            set => this.RaiseAndSetIfChanged(ref _someProp, value);
        }

        /// <inheritdoc/>
        public string UrlPathSegment => "Test";

        /// <inheritdoc/>
        public IScreen HostScreen { get; } = new TestScreen();
    }
}
