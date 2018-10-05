// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Mono.Cecil;

namespace EventBuilder.Entities
{
    /// <summary>
    /// Represents public type information.
    /// </summary>
    public class PublicTypeInfo
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the abstract.
        /// </summary>
        public string Abstract { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        public TypeDefinition Type { get; set; }

        /// <summary>
        /// Gets or sets the parent.
        /// </summary>
        public ParentInfo Parent { get; set; }

        /// <summary>
        /// Gets or sets the events.
        /// </summary>
        public IEnumerable<PublicEventInfo> Events { get; set; }

        /// <summary>
        /// Gets or sets the zero parameter methods.
        /// </summary>
        public IEnumerable<ParentInfo> ZeroParameterMethods { get; set; }

        /// <summary>
        /// Gets or sets the single parameter methods.
        /// </summary>
        public IEnumerable<SingleParameterMethod> SingleParameterMethods { get; set; }

        /// <summary>
        /// Gets or sets the multi parameter methods.
        /// </summary>
        public MultiParameterMethod[] MultiParameterMethods { get; set; }
    }
}
