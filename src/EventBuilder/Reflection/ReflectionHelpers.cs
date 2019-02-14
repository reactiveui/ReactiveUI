// Copyright (c) 2019 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using System.Reflection.PortableExecutable;
using EventBuilder.Reflection.Compilation;
using ICSharpCode.Decompiler.Metadata;
using ICSharpCode.Decompiler.TypeSystem;

namespace EventBuilder.Reflection
{
    internal static class ReflectionHelpers
    {
        public static ICompilation GetCompilation(IEnumerable<string> targetAssemblies, IEnumerable<string> searchDirectories)
        {
            var modules = targetAssemblies.Select(x => new PEFile(x, PEStreamOptions.PrefetchMetadata));

            return new EventBuilderCompiler(modules, searchDirectories);
        }
    }
}
