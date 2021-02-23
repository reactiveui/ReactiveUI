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
    public class TestScreen : ReactiveObject, IScreen
    {
        private RoutingState? _router;

        public RoutingState? Router
        {
#pragma warning disable CS8766 // Nullability of reference types in return type doesn't match implicitly implemented member (possibly because of nullability attributes).
#pragma warning disable CS8613 // Nullability of reference types in return type doesn't match implicitly implemented member.
            get => _router;
#pragma warning restore CS8613 // Nullability of reference types in return type doesn't match implicitly implemented member.
#pragma warning restore CS8766 // Nullability of reference types in return type doesn't match implicitly implemented member (possibly because of nullability attributes).
            set => this.RaiseAndSetIfChanged(ref _router, value);
        }
    }
}
