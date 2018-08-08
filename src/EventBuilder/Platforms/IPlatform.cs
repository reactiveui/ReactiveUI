// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace EventBuilder.Platforms
{
    public interface IPlatform
    {
        AutoPlatform Platform { get; }

        List<string> Assemblies { get; set; }

        // Cecil when run on Mono needs some direction as to the location of the platform specific MSCORLIB.
        List<string> CecilSearchDirectories { get; set; }
    }
}