// Copyright (c) 2019 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.PortableExecutable;
using ICSharpCode.Decompiler.Metadata;

namespace EventBuilder.Core.Reflection.Compilation
{
    internal static class PathSearchExtensions
    {
        /// <summary>
        /// Resolves the specified full assembly name.
        /// </summary>
        /// <param name="reference">A reference with details about the assembly.</param>
        /// <param name="targetAssemblyDirectories">The directories potentially containing the assemblies.</param>
        /// <param name="parameters">Parameters to provide to the reflection system..</param>
        /// <returns>The assembly definition.</returns>
        public static PEFile Resolve(this IAssemblyReference reference, IEnumerable<string> targetAssemblyDirectories, PEStreamOptions parameters = PEStreamOptions.PrefetchMetadata)
        {
            var dllName = reference.Name + ".dll";

            var fullPath = targetAssemblyDirectories.Select(x => Path.Combine(x, dllName)).FirstOrDefault(File.Exists);
            if (fullPath == null)
            {
                dllName = reference.Name + ".winmd";
                fullPath = targetAssemblyDirectories.Select(x => Path.Combine(x, dllName)).FirstOrDefault(File.Exists);
            }

            // NB: This hacks WinRT's weird mscorlib to just use the regular one
            // We forget why this was needed, maybe it's not needed anymore?
            if (reference.Name.IndexOf("mscorlib", StringComparison.InvariantCultureIgnoreCase) >= 0 && reference.Name.Contains("255"))
            {
                fullPath =
                    Environment.ExpandEnvironmentVariables(
                        @"%SystemRoot%\Microsoft.NET\Framework\v4.0.30319\mscorlib.dll");
            }

            if (fullPath == null)
            {
                return null;
            }

            return new PEFile(fullPath, parameters);
        }
    }
}
