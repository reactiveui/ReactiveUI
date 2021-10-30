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
    /// <summary>
    /// A model which is facading another object.
    /// </summary>
    public class FacadeModel : ReactiveObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FacadeModel"/> class.
        /// </summary>
        public FacadeModel() => Dependency = new BaseModel();

        /// <summary>
        /// Initializes a new instance of the <see cref="FacadeModel"/> class.
        /// </summary>
        /// <param name="dependency">The dependency to base again.</param>
        public FacadeModel(BaseModel dependency) => Dependency = dependency;

        /// <summary>
        /// Gets the base dependency.
        /// </summary>
        public BaseModel Dependency { get; private set; }

        /// <summary>
        /// Gets or sets a property with the same name in the dependency.
        /// </summary>
        [ReactiveDependency(nameof(Dependency))]
        public int IntProperty { get; set; }

        /// <summary>
        /// Gets or sets a string value that will be generated to pass through and from the dependency.
        /// </summary>
        [ReactiveDependency(nameof(Dependency), TargetProperty = "StringProperty")]
        public string? AnotherStringProperty { get; set; }
    }
}
