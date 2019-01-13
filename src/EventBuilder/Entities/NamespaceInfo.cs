// Copyright (c) 2019 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.Generic;

namespace EventBuilder.Entities
{
    /// <summary>
    /// Respresents namespace information.
    /// </summary>
    public class NamespaceInfo
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the types.
        /// </summary>
        public IEnumerable<PublicTypeInfo> Types { get; set; }
    }
}
