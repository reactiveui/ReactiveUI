// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI.Fody.Helpers;

namespace ReactiveUI.Fody.Tests
{
    public class ObservableAsTestModel : ReactiveObject
    {
        public ObservableAsTestModel() => Observable.Return("foo").ToPropertyEx(this, x => x.TestProperty);

        [ObservableAsProperty]
        public string? TestProperty { get; private set; }
    }
}
