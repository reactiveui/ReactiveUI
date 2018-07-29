// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Mono.Cecil;
using System.Collections.Generic;

namespace EventBuilder.Entities
{
    public class PublicTypeInfo
    {
        public string Name { get; set; }
        public string Abstract { get; set; }
        public TypeDefinition Type { get; set; }
        public ParentInfo Parent { get; set; }
        public IEnumerable<PublicEventInfo> Events { get; set; }
        public IEnumerable<ParentInfo> ZeroParameterMethods { get; set; }
        public IEnumerable<SingleParameterMethod> SingleParameterMethods { get; set; }
        public MultiParameterMethod[] MultiParameterMethods { get; set; }
    }
}