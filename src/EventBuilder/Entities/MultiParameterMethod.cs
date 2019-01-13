// Copyright (c) 2019 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

namespace EventBuilder.Entities
{
    /// <summary>
    /// Respresents a method with a mutiple parameters.
    /// </summary>
    public class MultiParameterMethod
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the parameter list.
        /// </summary>
        public string ParameterList { get; set; } // "FooType foo, BarType bar, BazType baz"

        /// <summary>
        /// Gets or sets the parameter type list.
        /// </summary>
        public string ParameterTypeList { get; set; } // "FooType, BarType, BazType"

        /// <summary>
        /// Gets or sets the parameter name list.
        /// </summary>
        public string ParameterNameList { get; set; } // "foo, bar, baz"
    }
}
