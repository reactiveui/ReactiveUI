// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace EventBuilder.Platforms
{
    /// <summary>
    /// Interface representing a platform assemblies and events.
    /// </summary>
    public interface IPlatform
    {
        /// <summary>
        /// Gets the event builder platform.
        /// </summary>
        AutoPlatform Platform { get; }

        /// <summary>
        /// Gets the assemblies.
        /// </summary>
        List<string> Assemblies { get; }

        /// <summary>
        /// Gets the cecil search directories.
        /// Cecil when run on Mono needs some direction as to the location of the platform specific MSCORLIB.
        /// </summary>
        List<string> CecilSearchDirectories { get; }
    }
}
