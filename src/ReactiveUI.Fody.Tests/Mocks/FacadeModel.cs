// Copyright (c) 2021 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI.Fody.Helpers;

namespace ReactiveUI.Fody.Tests
{
    public class FacadeModel : ReactiveObject
    {
        private BaseModel _dependency;

        public FacadeModel() => _dependency = new BaseModel();

        public FacadeModel(BaseModel dependency) => _dependency = dependency;

        public BaseModel Dependency
        {
            get => _dependency;
            private set => _dependency = value;
        }

        // Property with the same name, will look for a like for like name on the named dependency
        [ReactiveDependency(nameof(Dependency))]
        public int IntProperty { get; set; }

        // Property named differently to that on the dependency but still pass through value
        [ReactiveDependency(nameof(Dependency), TargetProperty = "StringProperty")]
        public string? AnotherStringProperty { get; set; }
    }
}
