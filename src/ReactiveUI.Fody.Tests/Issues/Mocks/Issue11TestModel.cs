// Copyright (c) 2020 .NET Foundation and Contributors. All rights reserved.
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

namespace ReactiveUI.Fody.Tests.Issues
{
    public class Issue11TestModel : ReactiveObject
    {
        public Issue11TestModel(string myProperty) => Observable.Return(myProperty).ToPropertyEx(this, x => x.MyProperty);

        public extern string MyProperty
        {
            [ObservableAsProperty]
            get;
        }
    }
}
