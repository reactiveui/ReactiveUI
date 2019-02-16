// Copyright (c) 2019 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.Generic;
using EventBuilder.Core.Entities;
using ICSharpCode.Decompiler.TypeSystem;

namespace EventBuilder.Core.Reflection.Resolvers
{
    /// <summary>
    /// A resolver which will generate namespace information.
    /// </summary>
    internal interface INamespaceResolver
    {
        /// <summary>
        /// Gets the template to use to generate the file.
        /// </summary>
        string TemplatePath { get; }

        /// <summary>
        /// Creates the namespaces from the specified assembly definitions.
        /// </summary>
        /// <param name="compilation">The compilation to use to determine the dependencies.</param>
        /// <returns>A collection of namespace definitions.</returns>
        IEnumerable<NamespaceInfo> Create(ICompilation compilation);
    }
}
