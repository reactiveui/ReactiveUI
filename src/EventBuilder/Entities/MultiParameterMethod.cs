// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace EventBuilder.Entities
{
    public class MultiParameterMethod
    {
        public string Name { get; set; }
        public string ParameterList { get; set; } // "FooType foo, BarType bar, BazType baz"
        public string ParameterTypeList { get; set; } // "FooType, BarType, BazType"
        public string ParameterNameList { get; set; } // "foo, bar, baz"
    }
}