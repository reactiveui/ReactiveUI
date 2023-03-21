// Copyright (c) 2023 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

using PublicApiGenerator;

using VerifyTests;

using VerifyXunit;

namespace ReactiveUI.Tests
{
    [ExcludeFromCodeCoverage]
    [UsesVerify]
    public abstract class ApiApprovalBase
    {
        protected static Task CheckApproval(Assembly assembly, [CallerFilePath]string? filePath = null)
        {
            if (filePath is null)
            {
                return Task.CompletedTask;
            }

            var generatorOptions = new ApiGeneratorOptions { WhitelistedNamespacePrefixes = new[] { "ReactiveUI" } };
            var apiText = assembly.GeneratePublicApi(generatorOptions);
            return Verifier.Verify(apiText, null, filePath)
                .UniqueForRuntimeAndVersion()
                .ScrubEmptyLines()
                .ScrubLines(l =>
                    l.StartsWith("[assembly: AssemblyVersion(", StringComparison.InvariantCulture) ||
                    l.StartsWith("[assembly: AssemblyFileVersion(", StringComparison.InvariantCulture) ||
                    l.StartsWith("[assembly: AssemblyInformationalVersion(", StringComparison.InvariantCulture) ||
                    l.StartsWith("[assembly: System.Reflection.AssemblyMetadata(", StringComparison.InvariantCulture));
        }
    }
}
